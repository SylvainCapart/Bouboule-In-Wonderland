using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ImageAppear : MonoBehaviour
{
    private Image m_Image;
    [SerializeField] private float m_appearTime = 1f;

    private void OnEnable()
    {
        m_Image = GetComponent<Image>();

        StartCoroutine(Appear());
    }

    public IEnumerator Appear()
    {
        float baseAlpha = 0f;

        for (float t = 0.0f; t < m_appearTime; t += Time.deltaTime)
        {
            m_Image.color = new Color(m_Image.color.r, m_Image.color.g, m_Image.color.b, (1f - baseAlpha) * (t / m_appearTime) + baseAlpha);
            yield return null;
        }
    }
}
