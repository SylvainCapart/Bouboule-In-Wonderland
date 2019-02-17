using System.Collections;
using UnityEngine;
using UnityStandardAssets._2D;

public class CameraMenuSwitcher : MonoBehaviour
{

    public static CameraMenuSwitcher instance;
    public enum CameraSpot
    {
        INIT,
        MENU,
        CREDITS
    }
    [SerializeField] private Camera2DFollow m_CameraFollow;
    [SerializeField] private Transform[] m_CameraSpots;
    [SerializeField] private float m_InitDelay = 2f;
    private const float EPSILON = 0.05f;
    [SerializeField] private float m_SwitchDamp = 0.45f;

    private bool m_ButtonsOff = true;

    private void Awake()
    {
        if (instance != null)
        {
            if (instance != this)
            {
                Destroy(this.gameObject);
            }
        }
        else
        {
            instance = this;
        }
    }

    private void Update()
    {
        if (Vector2.Distance(transform.position, m_CameraFollow.target.position) < EPSILON)
        {
            m_ButtonsOff = false;
        }
    }

    private void Start()
    {
        if (GeneralSceneMgt.instance.IsMenuPlayedOnce)
            NormalSetup();
    }

    private void OnEnable()
    {
        GeneralSceneMgt.OnMenuInit += Init;
        GeneralSceneMgt.OnMenuAlreadyInit += NormalSetup;

    }

    private void OnDisable()
    {
        GeneralSceneMgt.OnMenuInit -= Init;
        GeneralSceneMgt.OnMenuAlreadyInit -= NormalSetup;
    }

    private void Init()
    {
        m_CameraFollow.m_Damping = 1.5f;
        SetCamera(CameraSpot.INIT);
        StartCoroutine(SetCameraCor(CameraSpot.MENU, m_InitDelay));
    }

    private void NormalSetup()
    {
        SetCamera(CameraSpot.MENU);
        m_CameraFollow.m_Damping = m_SwitchDamp;
    }

    void SetCamera(CameraSpot cameraspot)
    {
        this.transform.position = m_CameraSpots[(int)cameraspot].position;
    }

    IEnumerator SetCameraCor(CameraSpot cameraspot, float delay)
    {
        yield return new WaitForSeconds(delay);
        m_CameraFollow.target = m_CameraSpots[(int)cameraspot];
    }

    public void SetCameraToMenu()
    {
        StopAllCoroutines();
        if (System.Math.Abs(m_CameraFollow.m_Damping - m_SwitchDamp) > EPSILON)
            m_CameraFollow.m_Damping = m_SwitchDamp;

        if (!m_ButtonsOff)
        {
            m_ButtonsOff = true;
            m_CameraFollow.target = m_CameraSpots[(int)CameraSpot.MENU];
            AudioManager.instance.PlaySound("AirCut");
        }

    }

    public void SetCameraToCredits()
    {
        StopAllCoroutines();
        if (System.Math.Abs(m_CameraFollow.m_Damping - m_SwitchDamp) > EPSILON)
            m_CameraFollow.m_Damping = m_SwitchDamp;

        if (!m_ButtonsOff)
        {
            m_ButtonsOff = true;
            m_CameraFollow.target = m_CameraSpots[(int)CameraSpot.CREDITS];
            AudioManager.instance.PlaySound("AirCut");
        }

    }

}
