using System;
using System.Collections;
using System.Runtime.Serialization;
using Pathfinding;
using UnityEngine;
using Random = UnityEngine.Random;

public class DragonMgt : MonoBehaviour
{
    private AudioManager m_AudioManager;
    public bool DebugMode;

    /* ---- DRAGON BEHAVIOR ---- */

    public enum DragonState { PATROL, TARGET, SLEEP, SURPRISED, GIVEUP };
    [SerializeField] private DragonState m_State;
    private DragonState m_LastState;
    [SerializeField] private AILerp m_AiLerpScript;
    [SerializeField] private AIDestinationSetter m_AISetter;
    public bool[] m_ModeEnabled;
    private Enemy m_Enemy;
    private ExpressionMgt m_ExpressionManager;
    private const int m_ModeNb = 5;
    public bool m_IsPlayerSwimming;


    /* ----------------------------- */

    /* ---- MODE TARGET ---- */

    // target related
    [System.Serializable]
    public class TargetData
    {
        public string targetname;
        public string targettag;
        public int priority;
    };

    [System.Serializable]
    public class Target
    {
        public Transform targettransform;
        public int targetpriority;

    };

    [SerializeField] private Target m_Target;
    [SerializeField] private TargetData[] m_TargetsArray;
    [SerializeField] private float m_GiveUpDistance = 15f;

    // spit fire related
    private ParticleSystem m_FireSource;
    private float m_FireTriggerDistance = 1.5f;
    private bool m_TargetFireClose;

    // orientation
    private bool m_FacingRight = true;



    /* ----------------------------- */

    /* ---- MODE PATROL ---- */


    public Transform[] m_PatrolPoints;
    private int m_PatrolPointIndex = 0;
    public bool m_Randomize;
    private int[] m_validChoices;
    [SerializeField] private LayerMask m_WhatIsTarget;



    /* ----------------------------- */

    /* ---- MODE SLEEP ---- */

    private Transform m_StartPosition;


    /* ----------------------------- */

    /* ---- MODE SURPRISED ---- */

    [SerializeField] private float m_SurprisedDelay = 3f;
    [SerializeField] private bool m_IsSurprisedCoRunning;

    /* ----------------------------- */

    public bool TargetClose
    {
        get
        {
            return m_TargetFireClose;
        }

        set
        {
            if (m_TargetFireClose == value) return;
            m_TargetFireClose = value;
            SpitFire(m_TargetFireClose);
        }
    }

    public DragonState State
    {
        get
        {
            return m_State;
        }

        set
        {
            if (m_State == value) return;
            if (m_ModeEnabled[(int)value] == false) return;
            switch (value)
            {
                case DragonState.PATROL:
                    TargetClose = false;
                    m_AiLerpScript.canMove = true;
                    //m_ExpressionManager.CancelExpression();
                    break;

                case DragonState.TARGET:
                    m_AiLerpScript.canMove = true;
                    //m_ExpressionManager.CancelExpression();
                    m_ExpressionManager.CallExpression(ExpressionMgt.ExpressionSymbol.EXCLAMATION);
                    break;

                case DragonState.SLEEP:
                    if (m_LastState != DragonState.SLEEP)
                        m_AiLerpScript.canMove = true;
                    m_ExpressionManager.CancelExpression();
                    m_ExpressionManager.CallExpression(ExpressionMgt.ExpressionSymbol.SLEEP);
                    break;

                case DragonState.SURPRISED:
                    m_AiLerpScript.canMove = false;
                    m_ExpressionManager.CancelExpression();
                    m_ExpressionManager.CallExpression(ExpressionMgt.ExpressionSymbol.QUESTION);
                    break;

                case DragonState.GIVEUP:
                    TargetClose = false;
                    m_AiLerpScript.canMove = true;
                    break;

                default:
                    if (DebugMode)
                        Debug.Log("Unknown state in " + this.name);
                    break;
            }
            if (m_IsSurprisedCoRunning == true)
            {
                StopCoroutine(SurprisedModeCo());
                m_IsSurprisedCoRunning = false;
            }

            m_LastState = m_State;
            m_State = value;

        }
    }


    public Target GetTarget
    {
        get
        {
            return m_Target;
        }
    }

    private void OnDisable()
    { 
        CharacterController2D.OnSwimChangeRaw -= OnPlayerSwim;
    }

    private void Start()
    {
        m_AudioManager = AudioManager.instance;
        if (m_AudioManager == null)
        {
            if (DebugMode)
                Debug.LogError("No audioManager found in " + this.name);
        }
        if (m_AiLerpScript == null)
            m_AiLerpScript = GetComponent<AILerp>();
        if (m_AISetter == null)
            m_AISetter = GetComponent<AIDestinationSetter>();
        if (m_ExpressionManager == null)
            m_ExpressionManager = GetComponentInChildren<ExpressionMgt>();
        if (m_Enemy == null)
            m_Enemy = GetComponent<Enemy>();

        CharacterController2D.OnSwimChangeRaw += OnPlayerSwim;

        m_ModeEnabled = new bool[m_ModeNb];

        if (m_FireSource == null)
        {
            ParticleSystem[] parts = GetComponentsInChildren<ParticleSystem>();
            for (int i = 0; i < parts.Length; i++)
            {
                if (parts[i].tag == "FireSource")
                {
                    m_FireSource = parts[i];
                    break;
                }

            }
        }

        for (int i = 0; i < m_ModeNb; i++)
        {
            m_ModeEnabled[i] = true;
        }

        if (m_TargetsArray.Length < 1)
        {
            if (DebugMode)
                Debug.Log(this.name + " will not follow any target, possible target array is empty. Mode TARGET disabled");
            m_ModeEnabled[(int)DragonState.TARGET] = false;
        }

        if (m_PatrolPoints.Length <= 1)
        {
            m_ModeEnabled[(int)DragonState.PATROL] = false;
            m_ModeEnabled[(int)DragonState.SLEEP] = true;
            m_LastState = DragonState.SLEEP;

            if (m_PatrolPoints.Length < 1)
                m_StartPosition = transform.parent;
            else
                m_StartPosition = m_PatrolPoints[0];

        }
        else if (m_PatrolPoints.Length >= 2)
        {
            m_ModeEnabled[(int)DragonState.PATROL] = true;
            m_ModeEnabled[(int)DragonState.SLEEP] = false;
            m_LastState = DragonState.PATROL;
            m_StartPosition = m_PatrolPoints[0];
        }

        State = (m_ModeEnabled[(int)DragonState.PATROL] == true) ? DragonState.PATROL : DragonState.SLEEP;



        if (m_Randomize && m_PatrolPoints.Length < 3)
        {
            if (DebugMode)
                Debug.Log("Not enough points to randomize path");
        }

        if (m_Randomize)
        {
            m_validChoices = new int[m_PatrolPoints.Length - 1];
            for (int i = 0; i < m_PatrolPoints.Length - 1; ++i)
            {
                m_validChoices[i] = i + 1;
            }
        }




    }

    private void Update()
    {
        switch (State)
        {
            case DragonState.PATROL:
                PatrolMode();
                break;
            case DragonState.TARGET:
                TargetMode();
                break;
            case DragonState.SLEEP:
                SleepMode();
                break;
            case DragonState.SURPRISED:
                SurprisedMode();
                break;
            case DragonState.GIVEUP:
                GiveUpMode();
                break;
            default:
                if (DebugMode)
                    Debug.Log("Unknown state in " + this.name);
                break;
        }

        if (m_Target.targettransform != null && State != DragonState.SLEEP && State != DragonState.SURPRISED)
        {
            if (!m_FacingRight && m_Target.targettransform.position.x - this.transform.position.x > 0)
            {
                FlipRotate();
            }
            else if (m_FacingRight && m_Target.targettransform.position.x - this.transform.position.x < 0)
            {
                FlipRotate();
            }
        }

    }

    void PatrolMode()
    {
        int lastIndex;

        m_Enemy.SetRegenHealthStatus(false);

        if (m_Target.targettransform != m_PatrolPoints[m_PatrolPointIndex])
        {
            m_AISetter.target = m_PatrolPoints[m_PatrolPointIndex];
            m_Target.targettransform = m_PatrolPoints[m_PatrolPointIndex];
            m_Target.targetpriority = 0;

        }

        if (Vector2.Distance(m_PatrolPoints[m_PatrolPointIndex].position, this.transform.position) < 0.3f)
        {
            if (m_Randomize)
            {
                lastIndex = m_PatrolPointIndex;
                m_PatrolPointIndex = GetRandomTagetIndex();
                m_validChoices[GetValueIndex(m_PatrolPointIndex)] = lastIndex;

            }
            else
            {
                if (m_PatrolPointIndex >= m_PatrolPoints.Length - 1)
                {
                    m_PatrolPointIndex = 0;

                }
                else
                {
                    ++m_PatrolPointIndex;
                }
            }

        }

    }

    void TargetMode()
    {
        m_Enemy.SetRegenHealthStatus(false);

        if (m_Target.targettransform != null)
            TargetClose = Vector3.Distance(transform.position, m_Target.targettransform.position) <= m_FireTriggerDistance;

        if (m_Target.targettransform != null && m_FireSource.isPlaying)
        {
            Vector3 difference = m_Target.targettransform.position - transform.position;
            difference.Normalize();
            float rotX = -Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;

            m_FireSource.transform.rotation = Quaternion.Euler(rotX, 90, 0);
        }
        else if (m_Target.targettransform == null && m_FireSource.isPlaying)
        {
            SpitFire(false);
        }

        if (Vector3.Distance(transform.position, m_StartPosition.position) >= m_GiveUpDistance)
        {
            SetTargetState(m_StartPosition, 0, DragonState.GIVEUP);
        }

        if (m_IsPlayerSwimming)
            SetTargetState(m_StartPosition, 0, DragonState.GIVEUP);

    }

    void SleepMode()
    {
        m_Enemy.SetRegenHealthStatus(false);

        m_AISetter.target = m_StartPosition;
        m_Target.targettransform = m_StartPosition;

    }

    void SurprisedMode()
    {
        m_Enemy.SetRegenHealthStatus(false);

        if (!m_IsSurprisedCoRunning)
            StartCoroutine(SurprisedModeCo());
    }

    void GiveUpMode()
    {
        m_Enemy.SetRegenHealthStatus(true);
        m_ModeEnabled[(int)DragonState.TARGET] = false;

        if (Vector2.Distance(transform.position, m_StartPosition.position) <= 0.2f)
        {
            m_ModeEnabled[(int)DragonState.TARGET] = true;
            if (m_ModeEnabled[(int)DragonState.SLEEP] == true)
                SetTargetState(transform.parent.transform, 0, DragonState.SLEEP);
            else
                SetTargetState(transform.parent.transform, 0, DragonState.PATROL);
        }

    }

    private void FlipScale()
    {
        // unparent expressionmanager then reparent it after scaling
        //m_ExpressionManager.transform.parent = null;

        m_FacingRight = !m_FacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        //m_ExpressionManager.transform.parent = transform;
    }

    private void FlipRotate()
    {

        transform.Rotate(new Vector3(0, 180, 0));
        m_ExpressionManager.transform.Rotate(new Vector3(0, 180, 0));
        //   transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, 0, transform.rotation.z));
        //transform.Rotate Quaternion.Euler(new Vector3(transform.rotation.x, 180, transform.rotation.z));
        // Switch the way the player is labelled as facing.
        m_FacingRight = !m_FacingRight;

        /*Vector3 scale = m_ExpressionManager.transform.localScale;
        scale.x *= -1;
        m_ExpressionManager.transform.localScale = scale;*/

    }

    public void SetTargetState(Transform targettransform, int priority, DragonState state)
    {
        if (State != state)
        {
            m_AISetter.target = targettransform.transform;
            m_Target.targettransform = targettransform.transform;

            m_Target.targetpriority = priority;

            State = state;
        }
    }


    /* public void SetTarget(GameObject targetobj, int priority)
     {
         if (State != DragonState.TARGET)
         {
             m_AISetter.target = targetobj.transform;
             m_Target.targettransform = targetobj.transform;

             m_Target.targetdata.priority = priority;

             State = DragonState.TARGET;
         }

     }

     public void SetPreTarget(GameObject targetobj, int priority)
     {
         m_AISetter.target = targetobj.transform;
         m_Target.targettransform = targetobj.transform;

         m_Target.targetdata.priority = priority;

         State = DragonState.SURPRISED;
     }

     private void UnsetTarget(GameObject targetobj, float detectiondistance, float giveupdistance, int priority)
     {
         m_Target.targettransform = null;
         State = DragonState.TARGET;
     }*/

    private void SpitFire(bool fire)
    {
        if (fire != false)
        {
            m_FireSource.Play();
            if (!m_AudioManager.IsSoundPlayed("DragonFire"))
                m_AudioManager.PlaySound("DragonFire");
        }
        else
        {
            m_FireSource.Stop();
            m_AudioManager.StopSound("DragonFire");
        }

    }

    /* private void DetectTarget(string targetname, string targettag, float detectiondistance, float giveupdistance, int priority)
     {
         GameObject[] potentialtargets = GameObject.FindGameObjectsWithTag(targettag);
         GameObject target = null;

         if (potentialtargets.Length >= 1)
         {
             for (int i = 0; i < potentialtargets.Length; i++)
             {
                 if (potentialtargets[i].name == targetname)
                 {
                     if (target != null)
                         Debug.Log("More than one target found with name : " + targetname + " in tags : " + targettag);
                     target = potentialtargets[i];
                 }

             }
             if (target == null)
             {
                 Debug.Log("No target found with name : " + targetname + " in tags : " + targettag);
                 return;
             }
         }
         else
         {
             Debug.Log("No target found with tag : " + targettag);
             return;
         }

         if (Vector3.Distance(transform.position, target.transform.position) <= detectiondistance )
         {
             RaycastHit2D hit = Physics2D.Raycast(this.transform.position, target.transform.position - this.transform.position, detectiondistance, m_WhatIsTarget);
             //if (hit)
               //  Debug.Log("hit : " + hit.transform.gameObject.name + " " + hit.transform.gameObject.tag);
             //Debug.DrawLine(this.transform.position, target.transform.position - this.transform.position, Color.red);

             if (m_Target.targettransform != null)
             {
                 if (priority >= m_Target.targetdata.priority && hit.collider != null && hit.collider.gameObject.name == targetname)
                     SetTarget(target, detectiondistance, giveupdistance, priority);
             }
             else
                 SetTarget(target, detectiondistance, giveupdistance, priority);
         }
     }*/

    /*private IEnumerator ShutOffDetection()
    {
        m_ShutOffDetect = true;
        yield return new WaitForSeconds(3f);
        m_ShutOffDetect = false;
    }*/

    private int GetRandomTagetIndex()
    {
        return m_validChoices[(Random.Range(0, m_PatrolPoints.Length - 1))];
    }

    private int GetValueIndex(int value)
    {
        for (int i = 0; i < m_validChoices.Length; i++)
        {
            if (m_validChoices[i] == value)
                return i;

        }
        return 0;
    }

    private IEnumerator SurprisedModeCo()
    {
        m_IsSurprisedCoRunning = true;

        yield return new WaitForSeconds(m_SurprisedDelay);

        State = m_LastState;
        m_IsSurprisedCoRunning = false;
    }

    private void OnPlayerSwim(bool state)
    {
        m_IsPlayerSwimming = state;

    }
}
