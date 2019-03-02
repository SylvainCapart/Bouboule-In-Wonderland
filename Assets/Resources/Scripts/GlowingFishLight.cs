using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SpriteRenderer))]
public class GlowingFishLight : MonoBehaviour
{

    [SerializeField] private SpriteRenderer m_SpriteRenderer;
    private IEnumerator m_co; //coroutine to launch
    [SerializeField] private float m_GlowPeriod; // time taken for the object for a complete glowing period
    private bool m_loopActive = true; // will the sprite glow forever or not
    [SerializeField] private float m_MinAlphaGlow;
    [SerializeField] private float m_MaxAlphaGlow;
    [SerializeField] private float m_HalfPeriodGlowingPause;

    // Use this for initialization
    void Start()
    {
        if (m_SpriteRenderer == null)
            Debug.LogError(this.name + " : SpriteRenderer not found");

        m_co = Glow();
        StartCoroutine(m_co);

        if (!m_loopActive)
            Destroy(this.gameObject, 0f);
    }

    IEnumerator Glow()
    {

        Color originalColor = m_SpriteRenderer.color;

        //In case the text color has its alpha component not set to 255, reinitialize it :
        originalColor.a = m_MaxAlphaGlow;

        // glowing forever is activated by default
        bool localLoop = true;

        while (localLoop)
        {
            for (float t = 0.0f; t < (0.5f * m_GlowPeriod); t += Time.deltaTime)
            {
                m_SpriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, m_MaxAlphaGlow + (m_MinAlphaGlow - m_MaxAlphaGlow) * (2 * t / m_GlowPeriod));
                yield return null;
            }

            yield return new WaitForSeconds(m_HalfPeriodGlowingPause);

            for (float t = 0.0f; t < (0.5f * m_GlowPeriod); t += Time.deltaTime)
            {
                m_SpriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, m_MinAlphaGlow + (m_MaxAlphaGlow - m_MinAlphaGlow) * (2 * t / m_GlowPeriod));
                yield return null;
            }
            yield return new WaitForSeconds(m_HalfPeriodGlowingPause);

            localLoop = m_loopActive;
        }


        yield return null;
    }
}
