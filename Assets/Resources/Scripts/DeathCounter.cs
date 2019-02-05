using UnityEngine;
using UnityEngine.UI;

public class DeathCounter : MonoBehaviour
{

    public static DeathCounter instance;
    [SerializeField] private int m_DeathCount = 0;
    [SerializeField] private Text m_DeathText;

    public int DeathCount
    {
        get
        {
            return m_DeathCount;
        }

        set
        {
            m_DeathCount = value;
            m_DeathText.text = m_DeathCount.ToString();
        }
    }

    private void Awake()
    {
        instance = this;
    }

}
