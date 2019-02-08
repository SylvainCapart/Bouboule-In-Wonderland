using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TextAppear : MonoBehaviour
{
    private Text m_Text;
    [SerializeField] private float m_appearTime = 1f;

    private void OnEnable()
    {
        m_Text = GetComponent<Text>();

        StartCoroutine(Appear());
    }

    public IEnumerator Appear()
    {
        float baseAlpha = 0f;

        for (float t = 0.0f; t < m_appearTime; t += Time.deltaTime)
        {
            m_Text.color = new Color(m_Text.color.r, m_Text.color.g, m_Text.color.b, (1f - baseAlpha) * (t / m_appearTime) + baseAlpha);
            yield return null;
        }
    }
}
