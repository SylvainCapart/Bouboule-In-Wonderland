using System.Collections;
using UnityEngine;


public class QuitUIMgt : MonoBehaviour
{

    [SerializeField] private Animator m_PanelAnim;
    [SerializeField] private bool m_IsPanelOpen;
    [SerializeField] private bool m_CanPanelToggle = true;
    private const float EPSILON = 0.01f;
    // Start is called before the first frame update

    void Start()
    {
        m_PanelAnim.SetFloat("Speed" ,-1f);

        m_PanelAnim.Play("QuitUIAppear", 0, 0f);
    }

    private void OnGUI()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            PanelToggle();
        }

    }

    public void OpenPanel()
    {
        Time.timeScale = 0f;

        m_IsPanelOpen = true;
        m_PanelAnim.SetFloat("Speed", 1f);

        if (m_PanelAnim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0f)
            m_PanelAnim.Play("QuitUIAppear", 0, 0f);
        else
            m_PanelAnim.Play("QuitUIAppear", 0, m_PanelAnim.GetCurrentAnimatorStateInfo(0).normalizedTime);

    }

    public void ClosePanel()
    {
        Time.timeScale = 1f;

        m_IsPanelOpen = false;

        m_PanelAnim.SetFloat("Speed", -1f);
        if (m_PanelAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            m_PanelAnim.Play("QuitUIAppear", 0, 1f);
        else
            m_PanelAnim.Play("QuitUIAppear", 0, m_PanelAnim.GetCurrentAnimatorStateInfo(0).normalizedTime);

    }

    private IEnumerator PanelToggleDelay(float delay)
    {
        m_CanPanelToggle = false;
        if (m_IsPanelOpen)
        {
            ClosePanel();
        }
        else
        {
            OpenPanel();
        }
        yield return new WaitForSecondsRealtime(delay);
        m_CanPanelToggle = true;


    }

    public void PanelToggle()
    {
        if (m_CanPanelToggle)
            StartCoroutine(PanelToggleDelay(0.2f));
    }

    public void ResumeGame()
    {
        PanelToggle();
    }

    public void BackToMenu()
    {
        GeneralSceneMgt.instance.GoToMenu();
    }

}
