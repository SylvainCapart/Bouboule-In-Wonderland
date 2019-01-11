using System;
using UnityEngine;
using UnityEngine.UI;

public class AirBarMgt : MonoBehaviour {

    [SerializeField] private RectTransform m_OxygenBarRect;

    [SerializeField] private GameObject[] m_IconArray;
    [SerializeField] private Sprite[] m_BarImageArray;
    [SerializeField] private Image m_BarImageSlot;
    [SerializeField] private Canvas m_BarCanvas;

    private void Start()
    {
        if (m_OxygenBarRect == null)
            Debug.LogError(this.name + " : m_OxygenBarRect no found");

        if (m_BarCanvas == null)
            Debug.LogError(this.name + " : m_BarCanvas no found");

        EnableSpitIcon(PlayerSpit.SpitParticle.FIRE);
    }

    private void OnEnable()
    {
        //GameMaster.ResetDelegate += AirBarReset;
        PlayerSpit.OnSpitModeChange += OnSpitModeChanged;
        PlayerSpit.OnSpitActivationChange += ShowAirBarUI;
    }

    private void OnDisable()
    {
        //GameMaster.ResetDelegate -= AirBarReset;
        PlayerSpit.OnSpitModeChange -= OnSpitModeChanged;
        PlayerSpit.OnSpitActivationChange -= ShowAirBarUI;
    }

    private void ShowAirBarUI(bool state)
    {
        m_BarCanvas.enabled = state;
    }

    public void SetOxygen(int _cur, int _max)
    {
        float _value = ((float)_cur / (float)_max);

        m_OxygenBarRect.localScale = new Vector3(_value, m_OxygenBarRect.localScale.y, m_OxygenBarRect.localScale.z);

    }


    private void OnSpitModeChanged(PlayerSpit.SpitParticle spitmode)
    {
        EnableSpitIcon(spitmode);
        EnableSpitBar(spitmode);
    }

    private void EnableSpitIcon(PlayerSpit.SpitParticle spitmode)
    {
        for (int i = 0; i < m_IconArray.Length; i++)
        {
            m_IconArray[i].SetActive(false);
        }

        m_IconArray[(int)spitmode].SetActive(true);
    }

    private void EnableSpitBar(PlayerSpit.SpitParticle spitmode)
    {
        m_BarImageSlot.sprite = m_BarImageArray[(int)spitmode];
    }


}
