using UnityEngine;

public class OxygenMgt : MonoBehaviour
{

    private PlayerStats stats;
    [SerializeField] private AirBarMgt m_AirBarUI;
    [SerializeField] private bool m_SwimMode = false;

    // Use this for initialization
    void Start()
    {
        stats = PlayerStats.instance;

        stats.CurrentOxygen = stats.m_MaxOxygen;

        if (m_AirBarUI == null)
        {
            Debug.Log("No airBarUI on Player");
        }
        else
        {
            m_AirBarUI.SetOxygen(stats.CurrentOxygen, stats.m_MaxOxygen);
        }

    }


    private void OnEnable()
    {
        CharacterController2D.MovementStatusChange += OnSwimModeChanged;
    }

    private void OnDisable()
    {
        CharacterController2D.MovementStatusChange -= OnSwimModeChanged;
    }

    // Update is called once per frame
    void Update()
    {

        if (m_SwimMode == true)
        {
            if (Input.GetButton("Fire1") && (stats.CurrentOxygen > 10))
            {
                CancelInvoke("IncreaseOxygen");
                if (!IsInvoking("DecreaseOxygen"))
                    InvokeRepeating("DecreaseOxygen", 1f / stats.m_OxygenDecreaseRate, 1f / stats.m_OxygenDecreaseRate);
            }
            else if (!Input.GetButton("Fire1") )
            {
                CancelInvoke("DecreaseOxygen");
                
            }
        }
        else
        {
            if (Input.GetButton("Fire1") && (stats.CurrentOxygen > 10))
            {
                CancelInvoke("IncreaseOxygen");
                if (!IsInvoking("DecreaseOxygen"))
                    InvokeRepeating("DecreaseOxygen", 1f / stats.m_OxygenDecreaseRate, 1f / stats.m_OxygenDecreaseRate);
            }
            else if (!Input.GetButton("Fire1") || stats.CurrentOxygen <= 0)
            {
                CancelInvoke("DecreaseOxygen");
                if (!IsInvoking("IncreaseOxygen"))
                    InvokeRepeating("IncreaseOxygen", 1f / stats.m_OxygenIncreaseRate, 1f / stats.m_OxygenIncreaseRate);
            }
        }

        m_AirBarUI.SetOxygen(stats.CurrentOxygen, stats.m_MaxOxygen);

    }

    private void OnSwimModeChanged(string statusName, bool state)
    {
        if (statusName == "Swim")
        {
            m_SwimMode = state;

            if (state == true)
            {
                CancelInvoke("IncreaseOxygen");
                if (!IsInvoking("PassiveOxygenLoss"))
                    InvokeRepeating("PassiveOxygenLoss", 1f / stats.m_PassiveOxygenLossRate, 1f / stats.m_PassiveOxygenLossRate);
            }
            else
            {
                CancelInvoke("DecreaseOxygen");
                CancelInvoke("PassiveOxygenLoss");
            }
        }
    }

    void DecreaseOxygen()
    {
        stats.CurrentOxygen -= 1;
    }

    void IncreaseOxygen()
    {
        stats.CurrentOxygen += 1;
    }

    void PassiveOxygenLoss()
    {
        stats.CurrentOxygen -= 1;
    }
}
