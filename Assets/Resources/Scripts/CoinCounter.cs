using UnityEngine;
using UnityEngine.UI;

public class CoinCounter : MonoBehaviour
{

    [SerializeField] private Text m_CoinText;
    private int m_CoinCounter;
    private int m_CoinsLimit;
    [SerializeField] private ChestMgt[] m_ChestsMgt;
    [SerializeField] private ChestSimpleMgt[] m_ChestsSimpleMgt;

    public delegate void CoinCounterDelegate(bool state);
    public static event CoinCounterDelegate OnCoinCounterEnabled;

    public int CoinCounterGetSet
    {
        get
        {
            return m_CoinCounter;
        }

        set
        {
            m_CoinCounter = value;
            m_CoinText.text = m_CoinCounter.ToString() + " / " + m_CoinsLimit;
            if (m_CoinCounter == m_CoinsLimit)
                GameMaster.gm.EndReached = true;
        }
    }


    private void OnEnable()
    {
        if (OnCoinCounterEnabled != null)
            OnCoinCounterEnabled(true);
    }

    private void OnDisable()
    {
        if (OnCoinCounterEnabled != null)
            OnCoinCounterEnabled(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            CoinCounterGetSet = m_CoinsLimit - 1;
        }
    }

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

        CoinCounterGetSet = 0;

    }

    public void IncreaseCoinCount()
    {
        CoinCounterGetSet++;
    }

}
