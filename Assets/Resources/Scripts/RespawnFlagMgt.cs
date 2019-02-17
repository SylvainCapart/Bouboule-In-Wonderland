using UnityEngine;

public class RespawnFlagMgt : MonoBehaviour {

    public bool m_FlagTriggered = false;
    private Animator m_Anim;
    private BoxCollider2D m_Box2D;

    public enum FlagState { RED, GREEN };
    [SerializeField] private FlagState m_State;

    public delegate void RespawnFlagDelegate(bool state);
    public static event RespawnFlagDelegate OnRespawnFlagStay;

    public FlagState State
    {
        get
        {
            return m_State;
        }

        set
        {
            if (m_State == value) return;
            switch(value)
            {
                case FlagState.RED:
                    m_FlagTriggered = false;
                    m_Anim.SetBool("FlagTriggered", false);
                    break;
                case FlagState.GREEN:
                    m_FlagTriggered = true;
                    m_Anim.SetBool("FlagTriggered", true);
                    break;
                default: // do nothing
                    break;
            }
            m_State = value;
        }
    }

    // Use this for initialization
    void Start () {

        if (m_Anim == null)
            m_Anim = this.GetComponentInParent<Animator>();

        m_Box2D = this.GetComponentInParent<BoxCollider2D>();
        if (m_Box2D == null)
            Debug.LogError(this.name + " : BoxCollider2D not found");

    }

    private void Awake()
    {
        if (m_Anim == null)
            m_Anim = this.GetComponentInParent<Animator>();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            GameMaster.gm.ResetFlagsExcept(gameObject);
            State = FlagState.GREEN;
            GameMaster.gm.SpawnPoint = this.gameObject;

            if (OnRespawnFlagStay != null)
                OnRespawnFlagStay(true);

            GameMaster.gm.LastMainSoundStr = AudioManager.instance.MainSound.name;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (OnRespawnFlagStay != null)
                OnRespawnFlagStay(true);
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (OnRespawnFlagStay != null)
                OnRespawnFlagStay(false);
        }
    }




}
