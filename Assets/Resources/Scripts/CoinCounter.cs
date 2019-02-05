using UnityEngine;
using UnityEngine.UI;

public class CoinCounter : MonoBehaviour
{

    [SerializeField] private Text m_CoinText;
    private int m_CoinCounter;
    private int m_CoinsLimit;
    [SerializeField] private ChestMgt[] m_ChestsMgt;
    [SerializeField] private ChestSimpleMgt[] m_ChestsSimpleMgt;

    // Use this for initialization
    void Start()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Coin");

        m_CoinsLimit = objs.Length;

        for (int i = 0; i < m_ChestsMgt.Length; i++)
        {
            m_CoinsLimit += m_ChestsMgt[i].m_CoinsMax;
        }
        for (int i = 0; i < m_ChestsSimpleMgt.Length; i++)
        {
            m_CoinsLimit += m_ChestsSimpleMgt[i].m_CoinsMax;
        }

        m_CoinCounter = 0;

    }

    // Update is called once per frame
    void Update()
    {
        m_CoinText.text = m_CoinCounter.ToString() + " / " + m_CoinsLimit;

    }

    public void IncreaseCoinCount()
    {

        m_CoinCounter++;


    }

}
