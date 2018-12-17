using System.Collections;
using UnityEngine;
using UnityStandardAssets._2D;

[RequireComponent(typeof(PlayerMovement))]
public class Player : MonoBehaviour {

    private PlayerStats stats;

    AudioManager audioManager;

    [SerializeField] private float m_FallBoundary = -20f;

    public string deathSoundName = "DeathVoice";
    public string damageSoundName = "Grunt";
    private bool m_Drowning = false;

    [SerializeField]
    private StatusIndicator statusIndicator;

    public bool Drowning
    {
        get
        {
            return m_Drowning;
        }

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
        stats = PlayerStats.instance;

        stats.CurrentHealth = stats.m_MaxHealth;

        if (GameObject.FindObjectsOfType<Player>().Length > 1)
        {
            Debug.LogError("Only one player can be instatiated");
            Destroy(this.transform.gameObject);
            return;
        }

        if (statusIndicator == null)
        {
            Debug.Log("No status indicator on Player");
        }
        else
        {
            statusIndicator.SetHealth(stats.CurrentHealth, stats.m_MaxHealth);
        }

        //GameMaster.gm.onToggleUpgrademenu += OnUpgradeMenuToggle;

        audioManager = AudioManager.instance;
        if (audioManager == null)
        {
            Debug.LogError("No audioManager found in Player");
        }

       

        //GameMaster.InitializePlayerRespawn(this);
        //statusIndicator = Camera.main.GetComponentInChildren<StatusIndicator>();

    }

    public void DamagePlayer(int damageReceived)
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


    }

    private void Update()
    {
        if (transform.position.y <= m_FallBoundary)
        {
            DamagePlayer( 10^6);
        }

        if (stats.CurrentOxygen <= 0)
        {
            if (!IsInvoking("OnDrowningCall"))
                InvokeRepeating("OnDrowningCall", 1f / stats.m_DrowningDamageRate, 1f / stats.m_DrowningDamageRate);
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
        statusIndicator.SetHealth(stats.CurrentHealth, stats.m_MaxHealth);
    }



}
