using UnityEngine;
using UnityEngine.UI;

public class ReadTime : MonoBehaviour
{
    private Text m_TimeText;

    private void Start()
    {
        float minutes = GameMaster.gm.m_FinalTime / 60f;
        float secondes = GameMaster.gm.m_FinalTime % 60f;
        m_TimeText = GetComponent<Text>();
        m_TimeText.text = "in only " + minutes.ToString("F0") + "min" + secondes.ToString("F0") + "s";
    }


}
