using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats instance;

    private int m_currentHealth;
    public int maxHealth = 100;
    public float startPcHealh = 1f;

    public int CurrentHealth
    {
        get { return m_currentHealth; }
        set { m_currentHealth = Mathf.Clamp(value, 0, maxHealth); }
    }

    public float healthRegenRate = 2f;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        CurrentHealth = (int)(startPcHealh * maxHealth);
    }
}

