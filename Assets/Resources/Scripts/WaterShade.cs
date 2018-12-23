using System.Collections;
using UnityEngine;

public class WaterShade : MonoBehaviour
{
    [SerializeField] private Renderer m_SpriteRenderer;
    private Animator m_Anim;
    [SerializeField] private Color m_FullAlphaColor;
    [SerializeField] private Color m_FadedAlphaColor;
    private bool m_ShadeFadeRunning = false;
    private bool m_ShadeAppearRunning = false;
    [SerializeField] private readonly float m_fadeTime = 1f;
    [SerializeField] private readonly float m_appearTime = 1f;
    private bool m_SwimMode = false;

    private void Start()
    {
        m_Anim = this.GetComponent<Animator>();
        if (m_Anim == null)
            Debug.LogError(this.name + " : Animator not found");

        if (m_SpriteRenderer == null)
            m_SpriteRenderer = GetComponentInChildren<Renderer>();

        m_SpriteRenderer.transform.parent = this.transform;
        m_SpriteRenderer.material.color = m_FadedAlphaColor;
    }

    private void OnEnable()
    {
        CharacterController2D.MovementStatusChange += SwimShader;
        GameMaster.ResetDelegate += ResetShade;
    }

    private void OnDisable()
    {
        CharacterController2D.MovementStatusChange -= SwimShader;
        GameMaster.ResetDelegate -= ResetShade;
    }

    private void ResetShade()
    {
        m_SpriteRenderer.material.color = m_FadedAlphaColor;
    }

    void SwimShader(string statusname, bool state)
    {
        if (statusname == "Swim")
        {
            m_SwimMode = state;

            if (state && !m_ShadeAppearRunning)
            {
                if (m_ShadeFadeRunning)
                {
                    m_ShadeFadeRunning = false;
                    StopCoroutine(ShadeFade());

                }

                StartCoroutine(ShadeAppear());
            }
            else if (!state && !m_ShadeFadeRunning)
            {
                if (m_ShadeAppearRunning)
                {
                    m_ShadeAppearRunning = false;
                    StopCoroutine(ShadeAppear());

                }
                StartCoroutine(ShadeFade());
            }
        }
    }


    public IEnumerator ShadeFade()
    {
        m_ShadeFadeRunning = true;

        float baseAlpha = m_SpriteRenderer.material.color.a;

        for (float t = 0.0f; t < m_fadeTime && m_ShadeFadeRunning; t += Time.deltaTime)
        {
            m_SpriteRenderer.material.color = new Color(m_FullAlphaColor.r, m_FullAlphaColor.g, m_FullAlphaColor.b, (baseAlpha - m_FadedAlphaColor.a) * (1 - t / m_fadeTime) + m_FadedAlphaColor.a);
            yield return null;
        }
        m_ShadeFadeRunning = false;
    }

    public IEnumerator ShadeAppear()
    {
        m_ShadeAppearRunning = true;

        float baseAlpha = m_SpriteRenderer.material.color.a;

        for (float t = 0.0f; t < m_appearTime && m_ShadeAppearRunning; t += Time.deltaTime)
        {
            m_SpriteRenderer.material.color = new Color(m_FullAlphaColor.r, m_FullAlphaColor.g, m_FullAlphaColor.b, (m_FullAlphaColor.a - baseAlpha) * (t / m_appearTime) + baseAlpha);
            yield return null;
        }
        m_ShadeAppearRunning = false;
    }
}
