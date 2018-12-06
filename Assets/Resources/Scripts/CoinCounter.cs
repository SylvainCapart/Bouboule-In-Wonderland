using UnityEngine;
using UnityEngine.UI;

public class CoinCounter : MonoBehaviour {

    [SerializeField] private Text m_CoinText;
    private int m_CoinCounter;
    private float m_lastCoinTime = 0f;
    [SerializeField] private float m_CoinShutOffDelay = 0.1f;

    // Use this for initialization
    void Start () {
        m_CoinCounter = 0;

    }
	
	// Update is called once per frame
	void Update () {
        m_CoinText.text = m_CoinCounter.ToString();

    }

    public void IncreaseCoinCount()
    {
        if (Time.time - m_lastCoinTime > m_CoinShutOffDelay)
        {
            m_CoinCounter++;
            m_lastCoinTime = Time.time;
        }

    }

}
