using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    private static PlayerStats instance;

    public static PlayerStats Instance
    {
        get
        {
            return instance;
        }
        set
        {
            instance = value;
        }

    }

    [SerializeField] private int m_CurrentHealth;
    [SerializeField] private int m_CurrentOxygen;
    public int m_MaxHealth = 100;
    public float m_StartPcHealh = 1f;
    public float m_StartPcOxygen = 1f;
    public int m_MaxOxygen = 100;
    public float m_OxygenDecreaseRate = 1f;
    public float m_OxygenIncreaseRate = 1f;
    public float m_PassiveOxygenLossRate = 7f;
    public float m_DrowningDamageRate = 1f;
    public int m_DrowningDamage = 25;
    public float m_HealthRegenRate = 2f;

    public int CurrentHealth
    {
        get { return m_CurrentHealth; }
        set { m_CurrentHealth = Mathf.Clamp(value, 0, m_MaxHealth); }
    }

    public int CurrentOxygen
    {
        get { return m_CurrentOxygen;}
        set { m_CurrentOxygen = Mathf.Clamp(value, 0, m_MaxOxygen); }
    }

    public float DrowningDamageRate
    {
        get
        {
            return m_DrowningDamageRate;
        }

        set
        {
            m_DrowningDamageRate = value;
        }
    }


    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        CurrentHealth = (int)(m_StartPcHealh * m_MaxHealth);
        CurrentOxygen = (int)(m_StartPcOxygen * m_MaxOxygen);
    }
}

