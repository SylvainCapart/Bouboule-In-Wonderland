using System;
using System.Collections;
using System.Runtime.Serialization;
using Pathfinding;
using UnityEngine;
using Random = UnityEngine.Random;

public class DragonMgt : MonoBehaviour
{
    private AudioManager m_AudioManager;

    /* ---- DRAGON BEHAVIOR ---- */

    public enum DragonState { PATROL, TARGET, SLEEP, SURPRISED, GIVEUP };
    [SerializeField] private DragonState m_State;
    [SerializeField] private AILerp m_AiLerpScript;
    [SerializeField] private AIDestinationSetter m_AISetter;
    [SerializeField] private bool[] m_ModeEnabled;
    private const int m_ModeNb = 5;

    /* ----------------------------- */
    
    /* ---- MODE TARGET ---- */

    // target related
    [System.Serializable]
    public class TargetData
    {
        public string targetname;
        public string targettag;
        public float detectiondistance;
        public float giveupdistance;
        public int priority;
    };

    [System.Serializable]
    public class Target
    {
        public Transform targettransform;
        public TargetData targetdata;
    };

    [SerializeField] private Target m_Target;
    [SerializeField] private TargetData[] m_TargetsArray;
    [SerializeField] private bool m_DetectCoIsRunning;
    private bool m_ShutOffDetect = false;


    // spit fire related
    private ParticleSystem m_FireSource;
    [SerializeField] private float m_FireTriggerDistance = 1.5f;
    [SerializeField] private bool m_TargetFireClose;

    // orientation
    private bool m_FacingRight = true;



    /* ----------------------------- */

    /* ---- MODE PATROL ---- */

   
    public Transform[] m_PatrolPoints;
    private int patrolPointIndex = 0;
    public float moveSpeed = 40f;
    private Rigidbody2D m_Rigidbody2D;
    [Range(0, .5f)] [SerializeField] private float m_AxisAdjustment = 0f;
    private Vector3 velocity = Vector3.zero;
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;  // How much to smooth out the movement
    //private bool m_FacingRight = false;
    [SerializeField] private bool m_isObjectFlying = false;
    public bool m_Randomize;
    [SerializeField] private int[] m_validChoices;
    [SerializeField] private LayerMask m_WhatIsTarget;



    /* ----------------------------- */

    /* ---- MODE SLEEP ---- */




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
                    m_AISetter.target = m_PatrolPoints[0];
                    StartCoroutine(ShutOffDetection());
                    break;
                case DragonState.TARGET:
                    //m_AISetter.target = m_Target.targettransform;

                    break;
                case DragonState.SLEEP:
                    break;
                default:
                    Debug.Log("Unknown state in " + this.name);
                    break;
            }
            m_State = value;

        }
    }



    private void Start()
    {

        m_AudioManager = AudioManager.instance;
        if (m_AudioManager == null)
        {
            Debug.LogError("No audioManager found in " + this.name);
        }


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
            Debug.Log(this.name + " will not follow any target, possible target array is empty. Mode TARGET disabled");
            m_ModeEnabled[(int)DragonState.TARGET] = false;
        }


        if (m_PatrolPoints.Length >= 2)
        {
            m_ModeEnabled[(int)DragonState.PATROL] = true;
            m_ModeEnabled[(int)DragonState.SLEEP] = false;
        }
        else if (m_PatrolPoints.Length < 2 && m_PatrolPoints.Length >= 1)
        {
            m_ModeEnabled[(int)DragonState.PATROL] = false;
            m_ModeEnabled[(int)DragonState.SLEEP] = true;
        }
        else
        {
            Debug.LogError(this.name + " : not enough patrol point to enable SLEEP mode");
        }

        if (m_AiLerpScript == null)
            m_AiLerpScript = GetComponent<AILerp>();
        if (m_AISetter == null)
            m_AISetter = GetComponent<AIDestinationSetter>();
            
        State = m_ModeEnabled[(int)DragonState.PATROL] == true ? DragonState.PATROL : DragonState.SLEEP;




        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        if (m_Rigidbody2D == null)
            Debug.LogError(this.name + " : RB not found");

        if (m_Rigidbody2D != null)
        {
            m_Rigidbody2D.gravityScale = 0f;
        }

        if (m_Randomize && m_PatrolPoints.Length < 3)
        {
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

    private void OnEnable()
    {

    }

    private void OnDisable()
    {

    }

    private void Update()
    {
        if (m_Target.targettransform != null)
        {
            if (!m_FacingRight && m_Target.targettransform.position.x - this.transform.position.x > 0)
            {
                FlipScale();
            }
            else if (m_FacingRight && m_Target.targettransform.position.x - this.transform.position.x < 0)
            {
                FlipScale();
            }
        }



        switch (State)
        {
            case DragonState.PATROL:
                PatrolMode();
                break;
            case DragonState.TARGET:
                TargetMode();
                break;
            case DragonState.SLEEP:
                break;
            default:
                Debug.Log("Unknown state in " + this.name);
                break;
        }



    }

    private void FixedUpdate()
    {

    }

    void PatrolMode()
    {
        int lastIndex;

        /*if (m_isObjectFlying || Mathf.Abs(m_PatrolPoints[patrolPointIndex].position.x - this.transform.position.x) > m_AxisAdjustment)
       {
           horizontalMove = ((m_PatrolPoints[patrolPointIndex].position.x - this.transform.position.x) / Mathf.Abs(m_PatrolPoints[patrolPointIndex].position.x - this.transform.position.x));
       }

       if (m_isObjectFlying || Mathf.Abs(m_PatrolPoints[patrolPointIndex].position.y - this.transform.position.y) > m_AxisAdjustment)
       {
           verticalMove = ((m_PatrolPoints[patrolPointIndex].position.y - this.transform.position.y) / Mathf.Abs(m_PatrolPoints[patrolPointIndex].position.y - this.transform.position.y));
       }*/
        if (m_Target.targettransform != m_PatrolPoints[patrolPointIndex])
        {
            m_AISetter.target = m_PatrolPoints[patrolPointIndex];
            m_Target.targettransform = m_PatrolPoints[patrolPointIndex];
            m_Target.targetdata.priority = 0;
            m_Target.targetdata.detectiondistance = 0f;
            m_Target.targetdata.giveupdistance = 100f;
            m_Target.targetdata.targetname = "Dummy";
            m_Target.targetdata.targettag = "Dummy";
        }


        //Vector2 checkDistance = new Vector2(Vector2.Distance(points[patrolPointIndex].position, this.transform.position), 0);


        if (Vector2.Distance(m_PatrolPoints[patrolPointIndex].position, this.transform.position) < 0.3f)
        {
            if (m_Randomize)
            {
                lastIndex = patrolPointIndex;
                patrolPointIndex = GetRandomTagetIndex();
                m_validChoices[GetValueIndex(patrolPointIndex)] = lastIndex;

            }
            else
            {
                if (patrolPointIndex >= m_PatrolPoints.Length - 1)
                {
                    patrolPointIndex = 0;

                }
                else
                {
                    ++patrolPointIndex;
                }
            }

        }

        if (!m_DetectCoIsRunning && !m_ShutOffDetect)
        {
            for (int i = 0; i < m_TargetsArray.Length; i++)
            {
                StartCoroutine(DetectTargetCo(m_TargetsArray[i].targetname, m_TargetsArray[i].targettag, m_TargetsArray[i].detectiondistance, m_TargetsArray[i].giveupdistance, m_TargetsArray[i].priority));
            }

        }

    }

    void TargetMode()
    {
        if (Vector3.Distance(transform.position, transform.parent.position) >= m_Target.targetdata.giveupdistance)
            State = DragonState.PATROL;

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
    }

    void SleepMode()
    {

    }

    private void FlipScale()
    {

        m_FacingRight = !m_FacingRight;


        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }


    private void SetTarget(GameObject targetobj, float detectiondistance, float giveupdistance, int priority)
    {
        m_AISetter.target = targetobj.transform;
        m_Target.targettransform = targetobj.transform;
        m_Target.targetdata.detectiondistance = detectiondistance;
        m_Target.targetdata.giveupdistance = giveupdistance;
        m_Target.targetdata.priority = priority;
        m_Target.targetdata.targettag = targetobj.tag;
        m_Target.targetdata.targetname = targetobj.name;
        State = DragonState.TARGET;

    }

    private void UnsetTarget(GameObject targetobj, float detectiondistance, float giveupdistance, int priority)
    {
        m_Target.targettransform = null;
        State = DragonState.TARGET;
    }

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

    private IEnumerator DetectTargetCo(string targetname, string targettag, float detectiondistance, float giveupdistance, int priority)
    {
        m_DetectCoIsRunning = true;
        DetectTarget(targetname, targettag, detectiondistance, giveupdistance, priority);
        yield return new WaitForSeconds(0.1f);

        m_DetectCoIsRunning = false;
    }

    private void DetectTarget(string targetname, string targettag, float detectiondistance, float giveupdistance, int priority)
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
                        Debug.LogError("More than one target found with name : " + targetname + " in tags : " + targettag);
                    target = potentialtargets[i];
                }

            }
            if (target == null)
            {
                Debug.LogError("No target found with name : " + targetname + " in tags : " + targettag);
                return;
            }
        }
        else
        {
            Debug.LogError("No target found with tag : " + targettag);
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
    }

    private IEnumerator ShutOffDetection()
    {
        m_ShutOffDetect = true;
        yield return new WaitForSeconds(3f);
        m_ShutOffDetect = false;
    }

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
}
