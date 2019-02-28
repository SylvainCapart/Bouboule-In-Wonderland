using Pathfinding;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyAI : MonoBehaviour
{
    private Animator m_Anim;
    public bool DebugMode;

    /* ---- DRAGON BEHAVIOR ---- */

    public enum EnemyState { PATROL, TARGET, SLEEP, SURPRISED, GIVEUP }; // different EnemyAI modes
    private const int m_ModeNb = 5; // Number of AI states
    [SerializeField] private EnemyState m_State; // current state of the AI
    private EnemyState m_LastState; // previous state of the AI
    [SerializeField] private AILerp m_AiLerpScript; // Lerp script used by the astar path
    [SerializeField] private AIDestinationSetter m_AISetter; // Setter script used by the astar pat, to set the target
    public bool[] m_ModeEnabled; // Which modes are enabled for the AI, among the EnemyStates above
    private Enemy m_Enemy; // Stats class used for the AI (health and other stats)
    private ExpressionMgt m_ExpressionManager; // Expression manager allows to render some expressions near the AI (example exclamation mark)
    [HideInInspector] public bool m_FacingRight = true; // direction the AI is facing

    /* ---- MODE TARGET ---- */

    [System.Serializable]
    public class TargetData // data class for the potential targets
    {
        public string targettag; // tag of the potential target
        public int priority; // priority of the potential target, in case of conflict with other detected targets
    };

    [System.Serializable]
    public class Target // data class for the current target
    {
        public Transform targettransform; // transform reference of the target
        public int targetpriority; // priority of the current target, in case of conflict with other detected targets
    };

    [SerializeField] private Target m_Target; // current target
    public TargetData[] m_TargetsArray; // potential targets
    public LayerMask m_WhatIsTarget; // layers where the potential targets will be searched for
    [SerializeField] private float m_GiveUpDistance = 15f; // after detecting a target, the maximum distance the AI can move before going back to the detecting point
    [SerializeField] private bool m_CanMoveInTargetMode = true; // should the AI move or not in target mode
    [SerializeField] private float m_TargetSpeed; // speed of the AI in target mode

    // Detection
    [SerializeField] private GameObject m_Detection; // gameobject with triggers and detection script
    [SerializeField] private GameObject m_Predetection; // gameobject with triggers and predetection script
    private Transform m_DetectionTransform; // transform of the point where the AI switches to target mode when a target is detected


    [Range(0f, 1f)] [SerializeField] private float m_SleepScaleTrigger = 1f; // reducing factor of the triggers when sleeping
    [Header("Optional : ")] public EnemySpecificGiveup m_SpecificGiveup; // optional additional giveup conditions following the tag of the AI

    /* ---- MODE PATROL ---- */

    public Transform[] m_PatrolPoints; // array of patrol points where the AI moves
    [SerializeField] private int m_LastPatrolIndex = 0; // index of the last patrol point that was reached
    [SerializeField] private int m_PatrolPointIndex = 0; // current targeted patrol point index in the array
    public bool m_Randomize; // is the path between the patrol points random or not. If not, AI moves from 0 -> 1 -> ... -> n and goes back to 0
    private int[] m_validChoices; // attay of valid choices for the random patroling, excluding the point that is reached
    [SerializeField] private float m_PatrolSpeed; // speed of the AI in patrol mode

    /* ---- MODE SLEEP ---- */

    private Transform m_StartPosition; // fixed position iwhere the AI should sleep
    public bool m_SleepRight; // should the AI sleeps right or left

    /* ---- MODE SURPRISED ---- */

    [SerializeField] private float m_SurprisedDelay = 3f; // how long should the AI be surprised
    private float m_InitSurprisedTime; // initial time when the suprised mode is triggered
    private bool m_IsSurprisedDelayInit; // is initial time for the surprised mode initialized, aka is the surprised mode triggered

    /* ----------------------------- */

    public EnemyState State
    {
        get
        {
            return m_State;
        }

        set
        {
            if (m_ModeEnabled[(int)value] == false) return;
            switch (value)
            {
                case EnemyState.PATROL:
                    m_AiLerpScript.canMove = true;
                    m_AiLerpScript.speed = m_PatrolSpeed;
                    m_ExpressionManager.CancelExpression();
                    break;

                case EnemyState.TARGET:
                    m_AiLerpScript.canMove = m_CanMoveInTargetMode;
                    m_AiLerpScript.speed = m_TargetSpeed;
                    m_ExpressionManager.CancelExpression();
                    m_ExpressionManager.CallExpression(ExpressionMgt.ExpressionSymbol.EXCLAMATION);
                    DetectionTransform.position = transform.position;
                    break;

                case EnemyState.SLEEP:
                    if (m_LastState != EnemyState.SLEEP)
                        m_AiLerpScript.canMove = true;
                    m_ExpressionManager.CancelExpression();
                    m_ExpressionManager.CallExpression(ExpressionMgt.ExpressionSymbol.SLEEP);
                    m_Anim.SetBool("Sleep", true);
                    break;

                case EnemyState.SURPRISED:
                    m_AiLerpScript.canMove = false;
                    m_ExpressionManager.CancelExpression();
                    m_ExpressionManager.CallExpression(ExpressionMgt.ExpressionSymbol.QUESTION);
                    break;

                case EnemyState.GIVEUP:
                    m_AiLerpScript.canMove = true;
                    m_ExpressionManager.CancelExpression();
                    break;

                default:
                    Debug.Log("Unknown state in " + this.name);
                    break;
            }

            if (value != EnemyState.SLEEP)
                m_Anim.SetBool("Sleep", false);

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

    public Transform DetectionTransform { get => m_DetectionTransform; set => m_DetectionTransform = value; }

    private void OnDestroy()
    {
        Destroy(transform.parent.gameObject);
    }

    private void Awake()
    {

        if (m_AiLerpScript == null)
            m_AiLerpScript = GetComponent<AILerp>();
        if (m_AISetter == null)
            m_AISetter = GetComponent<AIDestinationSetter>();
        if (m_ExpressionManager == null)
            m_ExpressionManager = GetComponentInChildren<ExpressionMgt>();
        if (m_Enemy == null)
            m_Enemy = GetComponent<Enemy>();
        if (m_Anim == null)
            m_Anim = GetComponent<Animator>();
        if (m_SpecificGiveup == null)
            m_SpecificGiveup = GetComponent<EnemySpecificGiveup>();
        if (m_Predetection == null)
            m_Predetection = GetComponentInChildren<EnemyPredetection>().gameObject;
        if (m_Detection == null)
            m_Detection = GetComponentInChildren<EnemyDetection>().gameObject;
            
        m_ModeEnabled = new bool[m_ModeNb];

        for (int i = 0; i < m_ModeNb; i++)
        {
            m_ModeEnabled[i] = true;
        }

        if (m_TargetsArray.Length < 1)
        {
            if (DebugMode)
                Debug.Log(this.name + " will not follow any target, possible target array is empty. Mode TARGET disabled");
            m_ModeEnabled[(int)EnemyState.TARGET] = false;
        }

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

    private void Start()
    {

        if (m_PatrolPoints.Length <= 1)
        {
            m_ModeEnabled[(int)EnemyState.PATROL] = false;
            m_ModeEnabled[(int)EnemyState.SLEEP] = true;
            m_LastState = EnemyState.SLEEP;

            if (m_PatrolPoints.Length < 1)
                m_StartPosition = transform.parent;
            else
                m_StartPosition = m_PatrolPoints[0];

        }
        else if (m_PatrolPoints.Length >= 2)
        {
            m_ModeEnabled[(int)EnemyState.PATROL] = true;
            m_ModeEnabled[(int)EnemyState.SLEEP] = false;
            m_LastState = EnemyState.PATROL;
            m_StartPosition = m_PatrolPoints[0];
        }
        State = (m_ModeEnabled[(int)EnemyState.PATROL] == true) ? EnemyState.PATROL : EnemyState.SLEEP;

        DetectionTransform = new GameObject().transform;
        DetectionTransform.parent = this.transform.parent;
        DetectionTransform.name = "DetectionTransform";

    }

    private void OnEnable()
    {
        if (State != EnemyState.GIVEUP)
            State = (m_ModeEnabled[(int)EnemyState.PATROL] == true) ? EnemyState.PATROL : EnemyState.SLEEP;
    }

    private void Update()
    {
        switch (State)
        {
            case EnemyState.PATROL:
                PatrolMode();
                break;
            case EnemyState.TARGET:
                TargetMode();
                break;
            case EnemyState.SLEEP:
                SleepMode();
                break;
            case EnemyState.SURPRISED:
                SurprisedMode();
                break;
            case EnemyState.GIVEUP:
                GiveUpMode();
                break;
            default:
                Debug.Log("Unknown state in " + this.name);
                break;
        }

        if (m_Target.targettransform != null && State != EnemyState.SLEEP  && State != EnemyState.SURPRISED)
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
        m_Detection.transform.localScale = new Vector3(1, 1, 1);
        m_Predetection.transform.localScale = new Vector3(1, 1, 1);

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
                m_LastPatrolIndex = m_PatrolPointIndex;
                m_PatrolPointIndex = GetRandomTagetIndex();
                m_validChoices[GetValueIndex(m_PatrolPointIndex)] = m_LastPatrolIndex;

            }
            else
            {
                if (m_PatrolPointIndex >= m_PatrolPoints.Length - 1)
                {
                    m_PatrolPointIndex = 0;
                    m_LastPatrolIndex = m_PatrolPoints.Length - 1;
                }
                else
                {
                    m_LastPatrolIndex = m_PatrolPointIndex;
                    ++m_PatrolPointIndex;
                }
            }
        }
    }

    void TargetMode()
    {
        m_Enemy.SetRegenHealthStatus(false);
        m_IsSurprisedDelayInit = false;

        if (m_Target.targettransform != null)
        {
            if (Vector3.Distance(m_Target.targettransform.position, DetectionTransform.position) >= m_GiveUpDistance)
            {
                SetTargetState(DetectionTransform, 0, EnemyState.GIVEUP);
            }
        }
        else
        {
            SetTargetState(DetectionTransform, 0, EnemyState.GIVEUP);
        }
    }

    void SleepMode()
    {
        m_Enemy.SetRegenHealthStatus(false);

        m_Detection.transform.localScale = new Vector3(m_SleepScaleTrigger, m_SleepScaleTrigger, m_SleepScaleTrigger);
        m_Predetection.transform.localScale = new Vector3(m_SleepScaleTrigger, m_SleepScaleTrigger, m_SleepScaleTrigger);

        if (m_AISetter != null)
            m_AISetter.target = m_StartPosition;
        m_Target.targettransform = m_StartPosition;

        if (m_SleepRight != m_FacingRight)
            FlipRotate();

    }

    void SurprisedMode()
    {
        m_Enemy.SetRegenHealthStatus(false);

        if (!m_IsSurprisedDelayInit)
        {
            m_InitSurprisedTime = Time.time;
            m_IsSurprisedDelayInit = true;
        }

        if (Time.time - m_InitSurprisedTime > m_SurprisedDelay)
        {
            State = m_LastState;
            m_IsSurprisedDelayInit = false;
        }

    }

    void GiveUpMode()
    {
        m_Enemy.SetRegenHealthStatus(true);
        m_ModeEnabled[(int)EnemyState.TARGET] = false;

        if (Vector2.Distance(transform.position, DetectionTransform.position) <= 0.2f)
        {
            m_ModeEnabled[(int)EnemyState.TARGET] = true;
            if (m_ModeEnabled[(int)EnemyState.SLEEP] == true)
                SetTargetState(transform.parent.transform, 0, EnemyState.SLEEP);
            else
                SetTargetState(transform.parent.transform, 0, EnemyState.PATROL);
        }
    }

    public void FlipRotate()
    {
        transform.Rotate(new Vector3(0, 180, 0));
        m_ExpressionManager.transform.Rotate(new Vector3(0, 180, 0));
        m_FacingRight = !m_FacingRight;
    }

    public void SetTargetState(Transform targettransform, int priority, EnemyState state)
    {
        if (State != state)
        {
            m_AISetter.target = targettransform.transform;
            m_Target.targettransform = targettransform.transform;

            m_Target.targetpriority = priority;

            State = state;
        }
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


    private void OnParticleCollision(GameObject other)
    {
        if (State == EnemyState.SLEEP && (other.tag == "FireSource" || other.tag == "FireSourceGreen") && other.transform.parent.tag == "Player")
            FlipRotate();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (State == EnemyState.SLEEP && collision.gameObject.tag == "Player")
            FlipRotate();
    }


}
