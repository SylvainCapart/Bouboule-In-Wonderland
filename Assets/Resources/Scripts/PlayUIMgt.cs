using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayUIMgt : MonoBehaviour
{

    [SerializeField] private Animator m_PanelAnim;
    [SerializeField] private bool m_IsPanelOpen;
    [SerializeField] private bool m_CanPanelToggle = true;
    private const float EPSILON = 0.01f;
    private Rect m_PanelRect;
    // Start is called before the first frame update

    void Start()
    {
        m_PanelAnim.SetFloat("Speed", -1f);
        RectTransform rectTrans = GetComponent<RectTransform>();
        m_PanelRect = new Rect(rectTrans.position.x, rectTrans.position.y, rectTrans.rect.width, rectTrans.rect.height);
        m_PanelAnim.Play("QuitUIAppear", 0, 0f);
    }

    private void OnGUI()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        bool mouseoverlap = (pos.x > m_PanelRect.position.x - m_PanelRect.width/2 && pos.x < m_PanelRect.position.x + m_PanelRect.width/2 &&
             pos.y > m_PanelRect.position.y - m_PanelRect.height/2 && pos.y < m_PanelRect.position.y + m_PanelRect.height/2);
        
        if (m_IsPanelOpen && (Input.GetKey(KeyCode.Escape) || (Input.GetButton("Fire1") && !mouseoverlap) ) )
        {
            PanelToggle();
        }
    }


    public void OpenPanel()
    {
        //Time.timeScale = 0f;

        m_IsPanelOpen = true;
        m_PanelAnim.SetFloat("Speed", 1f);

        if (m_PanelAnim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0f)
            m_PanelAnim.Play("QuitUIAppear", 0, 0f);
        else
            m_PanelAnim.Play("QuitUIAppear", 0, m_PanelAnim.GetCurrentAnimatorStateInfo(0).normalizedTime);

    }

    public void ClosePanel()
    {
        //Time.timeScale = 1f;

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
