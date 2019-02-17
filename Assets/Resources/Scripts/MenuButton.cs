using UnityEngine;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour
{
    public enum ButtonAction
    {
        QUIT, PLAY, CREDITS, BACK_CREDITS
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
                case ButtonAction.PLAY:
                    m_Button.onClick.AddListener(GeneralSceneMgt.instance.GoToGame);
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

    void OnDestroy()
    {
        m_Button.onClick.RemoveAllListeners();
    }

}

