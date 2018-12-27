using System.Collections;
using UnityEngine;
using UnityStandardAssets._2D;

[RequireComponent(typeof(PlayerMovement))]
public class Player : MonoBehaviour
{

    private PlayerStats stats;

    private AudioManager audioManager;
    private Animator m_Anim;

    [SerializeField] private float m_FallBoundary = -20f;
    private bool m_Drowning = false;
    private const float m_DamageAnimShutOnDelay = 0.6f;
    private bool m_Damagable = true;

    public string deathSoundName = "DeathVoice";
    public string damageSoundName = "Grunt";


    [SerializeField]
    private StatusIndicator statusIndicator;

    public bool Drowning
    {
        get { return m_Drowning; }
        set
        {
            if (m_Drowning == value) return;
            m_Drowning = value;
            if (OnDrowning != null)
                OnDrowning(value);
        }
    }

    public delegate void OnDrowningDelegate(bool drowning);
    public static event OnDrowningDelegate OnDrowning;


    private void Start()
    {
        stats = PlayerStats.Instance;

        stats.CurrentHealth = stats.m_MaxHealth;

        if (GameObject.FindObjectsOfType<Player>().Length > 1)
        {
            Debug.LogError("Only one player can be instatiated");
            Destroy(this.transform.gameObject);
            return;
        }

        if (statusIndicator == null)
        {
            StatusIndicator ind = GameObject.Find("PlayerHP").GetComponent<StatusIndicator>();
            if (ind != null)
                statusIndicator = ind;
        }
        

        statusIndicator.SetHealth(stats.CurrentHealth, stats.m_MaxHealth);

        if (m_Anim == null)
            m_Anim = GetComponent<Animator>();


        //GameMaster.gm.onToggleUpgrademenu += OnUpgradeMenuToggle;


        audioManager = AudioManager.instance;
        if (audioManager == null)
        {
            Debug.LogError("No audioManager found in " + this.name);
        }


        //GameMaster.InitializePlayerRespawn(this);
        //statusIndicator = Camera.main.GetComponentInChildren<StatusIndicator>();

    }


    private void OnEnable()
    {
        GameMaster.ResetDelegate += OnReset;
    }

    private void OnDisable()
    {
        GameMaster.ResetDelegate -= OnReset;
    }

    public void DamagePlayer(int damageReceived)
    {
        if (m_Damagable == true)
        {
            stats.CurrentHealth -= damageReceived;
            if (stats.CurrentHealth <= 0)
            {
                //audioManager.PlaySound(deathSoundName);
                GameMaster.KillPlayer(this);
            }
            else
            {
                audioManager.PlaySound(damageSoundName);
            }

            statusIndicator.SetHealth(stats.CurrentHealth, stats.m_MaxHealth);
            StartCoroutine(TriggerDamageAnim());
            StartCoroutine(DamageInShutOff(m_DamageAnimShutOnDelay));
        }

    }

    private void Update()
    {
        if (transform.position.y <= m_FallBoundary)
        {
            DamagePlayer(100000);
        }

        if (stats.CurrentOxygen <= 0)
        {
            if (!IsInvoking("OnDrowningCall"))
                InvokeRepeating("OnDrowningCall", 1f / stats.DrowningDamageRate, 1f / stats.DrowningDamageRate);
            Drowning = true;
        }
        else
        {
            if (IsInvoking("OnDrowningCall"))
                CancelInvoke("OnDrowningCall");
            Drowning = false;
        }

        statusIndicator.SetHealth(stats.CurrentHealth, stats.m_MaxHealth);

    }

    void RegenHealth()
    {
        stats.CurrentHealth += 1;
        statusIndicator.SetHealth(stats.CurrentHealth, stats.m_MaxHealth);
    }

    void OnDrowningCall()
    {
        stats.CurrentHealth -= stats.m_DrowningDamage;
        if (stats.CurrentHealth <= 0)
        {
            //audioManager.PlaySound(deathSoundName);
            GameMaster.KillPlayer(this);
        }
        else
        {
            audioManager.PlaySound(damageSoundName);
        }
        statusIndicator.SetHealth(stats.CurrentHealth, stats.m_MaxHealth);
    }

    private IEnumerator TriggerDamageAnim()
    {
        m_Anim.SetBool("Hurt", true);
        yield return (new WaitForSeconds(m_DamageAnimShutOnDelay));
        m_Anim.SetBool("Hurt", false);
    }

    public IEnumerator DamageInShutOff(float delay)
    {
        m_Damagable = false;

        yield return new WaitForSeconds(delay);

        m_Damagable = true;
    }

    private void OnReset()
    {


        if (statusIndicator == null)
        {
            StatusIndicator ind = GameObject.Find("UIOverlay").GetComponentInChildren<StatusIndicator>();
            if (ind != null)
                statusIndicator = ind;
        }
    }
}
