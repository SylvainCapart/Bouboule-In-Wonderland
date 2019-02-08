using UnityEngine;
using UnityEngine.UI;

public class ReadDeathCount : MonoBehaviour
{
    private Text m_DeathText;

    private void Start()
    {
        m_DeathText = GetComponent<Text>();

        int deathnb = DeathCounter.instance.DeathCount;
        string space = "";

        if (deathnb > 99)
            space = "";
        else if (deathnb > 9)
            space = " ";
        else if (deathnb <= 9)
            space = "  ";

        m_DeathText.text = "Only " + space + DeathCounter.instance.DeathCount.ToString();
    }

}
