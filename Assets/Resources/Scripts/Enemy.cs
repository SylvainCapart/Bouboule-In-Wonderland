using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum Mode { IDLE, PATROL, TARGET };

    [SerializeField] private int m_CurrentHealth;

    [SerializeField] private int m_MaxHealth;
    [SerializeField] private float m_StartPcHealh = 1f;
    [SerializeField] private int m_Damage = 40;
    [SerializeField] private float m_HealthRegenRate = 0.2f;
    [SerializeField] private int m_HealthRegenAmount = 10;

    public float m_ShakeAmountAmt = 0.1f;
    public float m_ShakeLength = 0.1f;

    public string m_DeathSoundName = "Explosion";

    [SerializeField] private bool m_Burnable;

    public int CurrentHealth
    {
        get { return m_CurrentHealth; }
        set { m_CurrentHealth = Mathf.Clamp(value, 0, MaxHealth); }
    }

    public int MaxHealth
    {
        get { return m_MaxHealth; }
        set { m_MaxHealth = value; }
    }

    public void Init()
    {
        CurrentHealth = (int)(m_StartPcHealh * MaxHealth);
    }

    [Header("Optional : ")]
    [SerializeField]
    private ParticleSystem m_BurnEffect;

    [Header("Optional : ")]
    [SerializeField]
    private StatusIndicator statusIndicator;

    private void Start()
    {
        Init();

        if (statusIndicator != null)
        {
            statusIndicator.SetHealth(CurrentHealth, MaxHealth);
        }

        if (m_Burnable && m_BurnEffect == null)
            m_BurnEffect = GameObject.FindGameObjectWithTag("BurnEffect").GetComponent<ParticleSystem>();
    }

    public void DamageEnemy(int damageReceived)
    {
        CurrentHealth -= damageReceived;
        if (CurrentHealth <= 0)
        {
            GameMaster.KillEnemy(this);
        }

        if (statusIndicator != null)
        {
            statusIndicator.SetHealth(CurrentHealth, MaxHealth);
        }
    }

    private void OnCollisionEnter2D(Collision2D _colInfo)
    {
        Player _player = _colInfo.collider.GetComponent<Player>();

        if (_player != null)
        {
            _player.DamagePlayer(m_Damage);
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        if (m_Burnable && (other.tag == "FireSource" || other.tag == "FireSourceGreen" ) && other.transform.parent.tag == "Player")
            Burn();
    }

    void RegenHealth()
    {
        CurrentHealth += m_HealthRegenAmount;
    }

    public void SetRegenHealthStatus(bool status)
    {
        if (status)
        {
            if (!IsInvoking("RegenHealth"))
                InvokeRepeating("RegenHealth", 1f / m_HealthRegenRate, 1f / m_HealthRegenRate);
        }
        else
        {
            if (IsInvoking("RegenHealth"))
                CancelInvoke("RegenHealth");
        }
    }

    private void Burn()
    {
        m_BurnEffect.Play();
        DamageEnemy(1);
    }

    private IEnumerator BurnCoroutine()
    {
        m_BurnEffect.Play();
        yield return (new WaitForSeconds(m_BurnEffect.main.duration));
    }
}
