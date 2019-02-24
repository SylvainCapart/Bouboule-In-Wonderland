using UnityEngine;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour
{
    public enum ButtonAction
    {
        QUIT, PLAY_NORMAL, PLAY_HARD, CREDITS, BACK_CREDITS
    }

    private Button m_Button;
    [SerializeField] private ButtonAction m_Action;

    // Start is called before the first frame update
    void Start()
    {
        m_Button = GetComponent<Button>();
        if (GeneralSceneMgt.instance != null)
        {
            switch (m_Action)
            {
                case ButtonAction.QUIT:
                    m_Button.onClick.AddListener(GeneralSceneMgt.instance.QuitGame);
                    break;
                case ButtonAction.PLAY_NORMAL:
                    m_Button.onClick.AddListener(GeneralSceneMgt.instance.GoToGameNormal);
                    break;
                case ButtonAction.PLAY_HARD:
                    m_Button.onClick.AddListener(GeneralSceneMgt.instance.GoToGameHard);
                    break;
                case ButtonAction.CREDITS:
                    m_Button.onClick.AddListener(CameraMenuSwitcher.instance.SetCameraToCredits);
                    break;
                case ButtonAction.BACK_CREDITS:
                    m_Button.onClick.AddListener(CameraMenuSwitcher.instance.SetCameraToMenu);
                    break;
                default:
                    break;
            }
        }
    }

}

