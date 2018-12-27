using System;
using System.Collections;
using System.Runtime.Serialization;
using Pathfinding;
using UnityEngine;

public class DragonMgt : MonoBehaviour
{
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
    private bool m_TargetFireClose;

    // orientation
    private bool m_FacingRight = true;

    // behavior related
    public enum DragonState { PATROL, TARGET, SLEEP };
    [SerializeField] private DragonState m_State;
    [SerializeField] private AILerp m_AiLerpScript;
    [SerializeField] private PointSwitch3D m_PointSwitchScript;
    [SerializeField] private AIDestinationSetter m_AISetter;





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
            m_State = value;
            switch (m_State)
            {
                case DragonState.PATROL:
                    m_FacingRight = true;
                    m_PointSwitchScript.enabled = true;
                    m_AiLerpScript.enabled = false;
                    m_AISetter.target = null;
                    StartCoroutine(ShutOffDetection());
                    break;
                case DragonState.TARGET:
                    m_FacingRight = true;
                    m_PointSwitchScript.enabled = false;
                    m_AISetter.target = m_Target.targettransform;
                    m_AiLerpScript.enabled = true;

                    break;
                case DragonState.SLEEP:
                    m_PointSwitchScript.enabled = false;
                    m_AiLerpScript.enabled = false;
                    break;
                default:
                    Debug.Log("Unknown state in " + this.name);
                    break;
            }

        }
    }



    private void Start()
    {
        if (m_FireSource == null)
        {
            ParticleSystem[] parts = GetComponentsInChildren<ParticleSystem>();
            for (int i = 0; i < parts.Length; i++)
            {
                if (parts[i].tag == "FireSource")
                {
                    m_FireSource = parts[i];
                    return;
                }

            }
        }

        if (m_TargetsArray.Length < 1)
            Debug.Log(this.name + " will not follow any target, possible target array is empty");

        if (m_AiLerpScript == null)
            m_AiLerpScript = GetComponent<AILerp>();
        if (m_PointSwitchScript == null)
            m_PointSwitchScript = GetComponent<PointSwitch3D>();
        if (m_AISetter == null)
            m_AISetter = GetComponent<AIDestinationSetter>();

        if (m_PointSwitchScript.enabled && !m_AiLerpScript.enabled)
            State = DragonState.PATROL;
        else if (m_PointSwitchScript.enabled && !m_AiLerpScript.enabled)
            State = DragonState.TARGET;
        else // else default mode is PATROL
            State = DragonState.PATROL;


    }

    private void OnEnable()
    {

    }

    private void OnDisable()
    {

    }

    private void Update()
    {
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
                if (State == DragonState.PATROL && !m_DetectCoIsRunning && !m_ShutOffDetect)
                {
                    for (int i = 0; i < m_TargetsArray.Length; i++)
                    {
                        StartCoroutine(DetectTargetCo(m_TargetsArray[i].targetname, m_TargetsArray[i].targettag, m_TargetsArray[i].detectiondistance, m_TargetsArray[i].giveupdistance, m_TargetsArray[i].priority));
                    }

                }
                break;
            case DragonState.TARGET:
                if (State == DragonState.TARGET)
                {
                    if (Vector3.Distance(transform.position, transform.parent.position) >= m_Target.targetdata.giveupdistance)
                        State = DragonState.PATROL;
                    Debug.LogError(Vector3.Distance(transform.position, transform.parent.position));
                }
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
        if (m_Target.targettransform != null)
            TargetClose = Vector3.Distance(transform.position, m_Target.targettransform.position) <= m_FireTriggerDistance;
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
        m_Target.targettransform = targetobj.transform;
        m_Target.targetdata.detectiondistance = detectiondistance;
        m_Target.targetdata.giveupdistance = giveupdistance;
        m_Target.targetdata.priority = priority;
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
            m_FireSource.Play();
        else
            m_FireSource.Stop();
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
                    target = potentialtargets[i];
            }
            if (target == null)
            {
                Debug.LogError("No target foud with name : " + targettag + " in tags : " + targettag);
                return;
            }
        }
        else
        {
            Debug.LogError("No target foud with tag : " + targettag);
            return;
        }

        if (Vector3.Distance(transform.position, target.transform.position) <= detectiondistance)
        {
            if (m_Target.targettransform != null)
            {
                if (priority >= m_Target.targetdata.priority)
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
}
