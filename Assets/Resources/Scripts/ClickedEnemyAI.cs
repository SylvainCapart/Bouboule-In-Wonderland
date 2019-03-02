using Pathfinding;
using UnityEngine;
using Random = UnityEngine.Random;

public class ClickedEnemyAI : MonoBehaviour
{
    private Animator m_Anim;
    public bool DebugMode;

    [System.Serializable]
    public class Spot
    {
        public Transform transform;
        public bool sleepright;
    }

    /* ---- DRAGON BEHAVIOR ---- */

    public enum EnemyState { PATROL, TARGET, SLEEP, SURPRISED, GIVEUP };
    [SerializeField] private EnemyState m_State;
    private EnemyState m_LastState;
    [SerializeField] private AILerp m_AiLerpScript;
    [SerializeField] private AIDestinationSetter m_AISetter;
    private Enemy m_Enemy;
    private ExpressionMgt m_ExpressionManager;
    private const int m_ModeNb = 5;


    [SerializeField] private CapsuleCollider2D m_CapsuleCollider;

    /* ----------------------------- */

    /* ---- MODE TARGET ---- */

    // target related
    [System.Serializable]
    public class TargetData
    {
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

    // orientation
    [HideInInspector] public bool m_FacingRight = true;

    /* ----------------------------- */

    /* ---- MODE PATROL ---- */

    public Spot[] m_PatrolPoints;
    private int m_LastPatrolIndex = 0;
    [HideInInspector] public int m_PatrolPointIndex = 0;
    public bool m_Randomize;
    private int[] m_validChoices;
    private float m_PatrolSpeed = 5f;
    [SerializeField] private float m_DistanceTolerance = 0.15f;

    /* ----------------------------- */

    /* ---- MODE SLEEP ---- */

    [SerializeField] private Transform m_WakeningTarget;



    public EnemyState State
    {
        get
        {
            return m_State;
        }

        set
        {
            if (m_State == value) return;
            switch (value)
            {
                case EnemyState.PATROL:
                    m_AiLerpScript.canMove = true;
                    m_AiLerpScript.speed = m_PatrolSpeed;
                    m_ExpressionManager.CancelExpression();
                    break;

                case EnemyState.SLEEP:
                    m_ExpressionManager.CancelExpression();
                    m_ExpressionManager.CallExpression(ExpressionMgt.ExpressionSymbol.SLEEP);
                    m_Anim.SetBool("Sleep", true);
                    m_PatrolSpeed = 5f;
                    break;

                default:
                    if (DebugMode)
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

        m_LastState = EnemyState.SLEEP;

        State = EnemyState.PATROL;

    }

    private void Update()
    {
        switch (State)
        {
            case EnemyState.PATROL:
                PatrolMode();
                break;
            case EnemyState.SLEEP:
                SleepMode();
                break;
            default:
                if (DebugMode)
                    Debug.Log("Unknown state in " + this.name);
                break;
        }

        if (m_Target.targettransform != null && State != EnemyState.SLEEP)
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

        if (m_Target.targettransform != m_PatrolPoints[m_PatrolPointIndex].transform)
        {
            m_AISetter.target = m_PatrolPoints[m_PatrolPointIndex].transform;
            m_Target.targettransform = m_PatrolPoints[m_PatrolPointIndex].transform;
            m_Target.targetpriority = 0;

        }

        if (Vector2.Distance(m_PatrolPoints[m_PatrolPointIndex].transform.position, this.transform.position) <= m_DistanceTolerance)
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
            State = EnemyState.SLEEP;
        }

    }

    void SleepMode()
    {

        if (Input.GetButtonDown("Fire1") && m_CapsuleCollider.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
        {
            State = EnemyState.PATROL;
        }
        else if (m_WakeningTarget != null && Vector2.Distance(m_WakeningTarget.transform.position, this.transform.position) <= m_DistanceTolerance)
        {
            m_PatrolSpeed = 8f;
            State = EnemyState.PATROL;
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

}
