using System;
using UnityEngine;
using UnityEngine.UI;
public class TargetIndicator : MonoBehaviour
{
    private GameObject m_player;
     private GameObject m_UIOverlay;
     private Transform m_ArrowParent;
    private Camera mainCamera;
    private RectTransform m_icon;
    private Text m_ArrowText;
    private Image m_iconImage;
    private Canvas mainCanvas;
    [SerializeField] private Sprite m_targetIcon;
    [SerializeField] private Vector3 m_targetIconScale;
    [SerializeField] private float m_ArrowOffs;

    void Start()
    {

        m_player = FindObjectOfType<Player>().gameObject;
        m_UIOverlay = GameObject.Find("UIOverlay");
        m_ArrowParent = GameObject.Find("CoinsOTI").transform;
        mainCamera = Camera.main;
        mainCanvas = m_UIOverlay.GetComponent<Canvas>();
        Debug.Assert((mainCanvas != null), "There needs to be a Canvas object in the scene for the OTI to display");
        InstantiateTargetIcon();
    }

    void Update()
    {
        UpdateTargetIconPosition();
    }

    private void OnEnable()
    {
        RespawnFlagMgt.OnRespawnFlagStay += SetOTIState;
    }

    private void OnDisable()
    {
        RespawnFlagMgt.OnRespawnFlagStay -= SetOTIState;
    }

    private void UpdateTargetIconPosition()
    {

            Vector3 newPos;
            Vector3 screencoordinate = Camera.main.WorldToScreenPoint(transform.position);

        if (screencoordinate.x > mainCamera.pixelWidth || screencoordinate.x < 0 || screencoordinate.y > mainCamera.pixelHeight || screencoordinate.y < 0)
        {
            //m_icon.gameObject.SetActive(true);

            Vector3 difference = screencoordinate - m_icon.transform.position;
            difference.Normalize();
            float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
            m_icon.transform.rotation = Quaternion.Euler(0f, 0f, rotZ);

            if (screencoordinate.x < 0 && screencoordinate.y < 0)
                newPos = new Vector3(0 + m_ArrowOffs, 0 + m_ArrowOffs, 0f);
            else if (screencoordinate.x < 0 && screencoordinate.y > mainCamera.pixelHeight)
                newPos = new Vector3(0 + m_ArrowOffs, mainCamera.pixelHeight - m_ArrowOffs, 0f);
            else if (screencoordinate.x > mainCamera.pixelWidth && screencoordinate.y > mainCamera.pixelHeight)
                newPos = new Vector3(mainCamera.pixelWidth - m_ArrowOffs, mainCamera.pixelHeight - m_ArrowOffs, 0f);
            else if (screencoordinate.x > mainCamera.pixelWidth && screencoordinate.y < 0)
                newPos = new Vector3(mainCamera.pixelWidth - m_ArrowOffs, 0 + m_ArrowOffs, 0f);
            else if (screencoordinate.x > mainCamera.pixelWidth)
                newPos = new Vector3(mainCamera.pixelWidth - m_ArrowOffs, screencoordinate.y, 0f);
            else if (screencoordinate.x < 0)
                newPos = new Vector3(0 + m_ArrowOffs, screencoordinate.y, 0f);
            else if (screencoordinate.y > mainCamera.pixelHeight)
                newPos = new Vector3(screencoordinate.x, mainCamera.pixelHeight - m_ArrowOffs, 0f);
            else if (screencoordinate.y < 0)
                newPos = new Vector3(screencoordinate.x, 0 + m_ArrowOffs, 0f);
            else
            {
                m_icon.gameObject.SetActive(false);
                newPos = new Vector3(screencoordinate.x, screencoordinate.y, 0f);
            }
            m_icon.transform.position = newPos;
        }
        else
        {
            m_icon.gameObject.SetActive(false);
        }

    }

    private void SetOTIState(bool state)
    {
        m_icon.gameObject.SetActive(state);
    }
         

    private void InstantiateTargetIcon()
    {
        m_icon = new GameObject().AddComponent<RectTransform>();
        //m_icon.transform.SetParent(mainCanvas.transform);
        //m_icon.transform.position = 
        m_icon.transform.SetParent(m_ArrowParent);
        m_icon.localScale = m_targetIconScale;
        m_icon.name = name + ": OTI icon";
        m_iconImage = m_icon.gameObject.AddComponent<Image>();
        m_iconImage.sprite = m_targetIcon;
        m_icon.gameObject.SetActive(false);
    }


}