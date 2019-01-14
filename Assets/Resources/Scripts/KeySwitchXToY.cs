using UnityEngine;
using UnityEngine.UI;

public class KeySwitchXToY : MonoBehaviour
{
    [SerializeField] private string m_KeyToChange; // just to remember what is to be changed
    [SerializeField] private string m_KeyNew;
    private Text m_Text;
    [SerializeField] private SystemLanguage[] m_LanguageConditions;


    // Start is called before the first frame update
    void Start()
    {
        m_Text = GetComponent<Text>();
        if (!m_Text)
            Debug.LogError(name + " : No Text attached to this object");

        for (int i = 0; i < m_LanguageConditions.Length; i++)
        {
            if (Application.systemLanguage == m_LanguageConditions[i])
            {
                m_Text.text = m_KeyNew;
            }
        }


    }

}
