using UnityEngine;
using UnityEngine.UI;

public class CoinCounter : MonoBehaviour
{

    [SerializeField] private Text m_CoinText;
    private int m_CoinCounter;
    private int m_CoinsLimit;

    // Use this for initialization
    void Start()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Coin");
        GameObject[] chests = GameObject.FindGameObjectsWithTag("Chest");
        GameObject[] chestsghost = GameObject.FindGameObjectsWithTag("ChestGhost");
        GameObject[] chestsSimple = GameObject.FindGameObjectsWithTag("ChestSimple");
        m_CoinsLimit = objs.Length;

        for (int i = 0; i < chests.Length; i++)
        {
            m_CoinsLimit += chests[i].GetComponent<ChestMgt>().m_CoinsMax;
        }
        for (int i = 0; i < chestsSimple.Length; i++)
        {
            m_CoinsLimit += chestsSimple[i].GetComponent<ChestSimpleMgt>().m_CoinsMax;
        }
        /*for (int i = 0; i < chestsghost.Length; i++)
        {
            m_CoinsLimit += chestsghost[i].GetComponentInChildren<ChestSimpleMgt>().m_CoinsMax;
        }*/
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
