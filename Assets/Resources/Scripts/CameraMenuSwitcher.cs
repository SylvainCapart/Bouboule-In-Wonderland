using System.Collections;
using UnityEngine;
using UnityStandardAssets._2D;

public class CameraMenuSwitcher : MonoBehaviour
{
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

    private void Update()
    {
        if (Input.GetKey(KeyCode.L))
        {
            m_CameraFollow.target = m_CameraSpots[(int)CameraSpot.CREDITS];
        }
        if (Input.GetKey(KeyCode.K))
        {
            m_CameraFollow.target = m_CameraSpots[(int)CameraSpot.MENU];
        }

        if (Vector2.Distance(transform.position, m_CameraFollow.target.position) < EPSILON)
        {
            m_ButtonsOff = false;
        }


    }

    private void OnEnable()
    {
        GeneralSceneMgt.OnMenuInit += Init;
    }

    private void OnDisable()
    {
        GeneralSceneMgt.OnMenuInit -= Init;
    }

    private void Init()
    {
        SetCamera(CameraSpot.INIT);
        StartCoroutine(SetCameraCor(CameraSpot.MENU, m_InitDelay));
    }

    void SetCamera(CameraSpot cameraspot)
    {
        this.transform.position = m_CameraSpots[(int)cameraspot].position;
    }

    IEnumerator SetCameraCor(CameraSpot cameraspot, float delay)
    {
        yield return new WaitForSeconds(delay);
        m_CameraFollow.target = m_CameraSpots[(int)cameraspot];
        yield return new WaitForSeconds(6f);
        m_CameraFollow.m_Damping = m_SwitchDamp;
    }

    public void SetCameraToMenu()
    {
        if (!m_ButtonsOff)
        {
            m_ButtonsOff = true;
            m_CameraFollow.target = m_CameraSpots[(int)CameraSpot.MENU];
            AudioManager.instance.PlaySound("AirCut");
        }

    }

    public void SetCameraToCredits()
    {
        if (!m_ButtonsOff)
        {
            m_ButtonsOff = true;
            m_CameraFollow.target = m_CameraSpots[(int)CameraSpot.CREDITS];
            AudioManager.instance.PlaySound("AirCut");
        }

    }

}
