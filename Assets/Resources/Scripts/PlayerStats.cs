using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats instance;

    private int _currentHealth;
    public int maxHealth = 100;
    public float startPcHealh = 1f;

    public int CurrentHealth
    {
        get { return _currentHealth; }
        set { _currentHealth = Mathf.Clamp(value, 0, maxHealth); }
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

