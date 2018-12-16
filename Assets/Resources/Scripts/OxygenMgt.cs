using UnityEngine;

public class OxygenMgt : MonoBehaviour {

    private PlayerStats stats;
    [SerializeField] private AirBarMgt m_AirBarUI;

    // Use this for initialization
    void Start () {
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
	
	// Update is called once per frame
	void Update () {

        bool m_button = Input.GetButton("Fire1");
        //Debug.Log(m_button);
        if (Input.GetButtonDown("Fire1") && (stats.CurrentOxygen > 0))
        {
            CancelInvoke();
            InvokeRepeating("DecreaseOxygen", 1f / stats.m_OxygenDecreaseRate, 1f / stats.m_OxygenDecreaseRate);
        }
        else if (Input.GetButtonUp("Fire1") || (stats.CurrentOxygen <= 0))
        {
            CancelInvoke();
            InvokeRepeating("IncreaseOxygen", 1f / stats.m_OxygenIncreaseRate, 1f / stats.m_OxygenIncreaseRate);
        }

        m_AirBarUI.SetOxygen(stats.CurrentOxygen, stats.m_MaxOxygen);

    }

    void DecreaseOxygen()
    {
        Debug.Log("Decrease Ox");
        stats.CurrentOxygen -= 1;
    }

    void IncreaseOxygen()
    {
        Debug.Log("Increase Ox");
        stats.CurrentOxygen += 1;
    }
}
