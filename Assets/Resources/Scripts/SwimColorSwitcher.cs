using UnityEngine;
using UnityEngine.UI;

public class SwimColorSwitcher : MonoBehaviour
{
    [SerializeField] private Color m_AirColor;
    [SerializeField] private Color m_SwimColor;
    [SerializeField] private Text[] m_Texts;
    [SerializeField] private Button[] m_Buttons;

    private void OnEnable()
    {
        CharacterController2D.MovementStatusChange += SwitchColor;
    }

    private void OnDisable()
    {
        CharacterController2D.MovementStatusChange -= SwitchColor;
    }

    void SwitchColor(string action, bool state)
    {
        if (action == "Swim")
        {
            if (state)
            {
                for (int i = 0; i < m_Texts.Length; i++)
                {
                    m_Texts[i].color = m_SwimColor;
                }
                for (int i = 0; i < m_Buttons.Length; i++)
                {
                    ColorBlock colorblock = m_Buttons[i].colors;
                    colorblock.normalColor = m_SwimColor;
                    m_Buttons[i].colors = colorblock;
                }
            }
            else
            {
                for (int i = 0; i < m_Texts.Length; i++)
                {
                    m_Texts[i].color = m_AirColor;
                }
                for (int i = 0; i < m_Buttons.Length; i++)
                {
                    ColorBlock colorblock = m_Buttons[i].colors;
                    colorblock.normalColor = m_SwimColor;
                    m_Buttons[i].colors = colorblock;
                }
            }
        }
    }

}
