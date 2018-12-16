using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats instance;

    private int m_CurrentHealth;
    public int m_CurrentOxygen;
    public int m_MaxHealth = 100;
    public float m_StartPcHealh = 1f;
    public float m_StartPcOxygen = 1f;
    public int m_MaxOxygen = 100;
    public float m_OxygenDecreaseRate = 1f;
    public float m_OxygenIncreaseRate = 1f;
    public float m_HealthRegenRate = 2f;

    public int CurrentHealth
    {
        get { return m_CurrentHealth; }
        set { m_CurrentHealth = Mathf.Clamp(value, 0, m_MaxHealth); }
    }

    public int CurrentOxygen
    {
        get { return m_CurrentOxygen;}
        set { m_CurrentOxygen = Mathf.Clamp(value, 0, m_MaxHealth); }
    }



    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        CurrentHealth = (int)(m_StartPcHealh * m_MaxHealth);
        CurrentOxygen = (int)(m_StartPcOxygen * m_MaxOxygen);
    }
}

