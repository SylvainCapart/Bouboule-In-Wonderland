using UnityEngine;

public class AirBarMgt : MonoBehaviour {

    [SerializeField] private RectTransform m_OxygenBarRect;

    private void Start()
    {
        m_OxygenBarRect = this.GetComponent<RectTransform>();
        if (m_OxygenBarRect == null)
            Debug.LogError(this.name + " : m_OxygenBarRect no found");
    }

    public void SetOxygen(int _cur, int _max)
    {
        float _value = ((float)_cur / (float)_max);

        m_OxygenBarRect.localScale = new Vector3(_value, m_OxygenBarRect.localScale.y, m_OxygenBarRect.localScale.z);

    }
}
