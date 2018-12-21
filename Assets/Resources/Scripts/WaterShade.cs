using System.Collections;
using UnityEngine;

public class WaterShade : MonoBehaviour
{
    [SerializeField] private Renderer m_SpriteRenderer;
    private Animator m_Anim;
    public Color m_OriginalColor;
    public Color m_CurrentColor;
    public bool m_ShadeRoutineRunning = false;
    public float m_fadeTime = 1f;
    public float m_appearTime = 1f;
    public bool m_SwimMode = false;

    private void Start()
    {
        m_Anim = this.GetComponent<Animator>();
        if (m_Anim == null)
            Debug.LogError(this.name + " : Animator not found");

        if (m_SpriteRenderer == null)
            m_SpriteRenderer = GetComponentInChildren<Renderer>();
        m_SpriteRenderer.transform.parent = this.transform;

        StartCoroutine(ShadeRoutine());
    }



    private void OnEnable()
    {
        CharacterController2D.MovementStatusChange += SwimShader;
        GameMaster.ResetDelegate += ShaderInactive;
    }

    private void OnDisable()
    {
        CharacterController2D.MovementStatusChange -= SwimShader;
        GameMaster.ResetDelegate -= ShaderInactive;
    }

    void SwimShader(string statusname, bool state)
    {
        if (statusname == "Swim")
        {
            m_SwimMode = state;
            if (m_ShadeRoutineRunning != true)
                StartCoroutine(ShadeRoutine());
            //m_Anim.SetBool("Fade", !state);
            //m_Anim.SetBool("Appear", state);

        
        }
    }

    void ShaderInactive()
    {
        //m_Anim.SetBool("Fade", false);
    }

    public IEnumerator SwitchShutOff(float delay)
    {
 

        yield return new WaitForSeconds(delay);

    }


    public IEnumerator ShadeRoutine()
    {
        m_CurrentColor = m_SpriteRenderer.material.color;
        m_ShadeRoutineRunning = true;
        m_SpriteRenderer.material.color = m_OriginalColor;
        //In case the text color has its alpha component not set to 255, reinitialize it :
        //m_OriginalColor.a = 1f;
        bool loop = true;

        Debug.Log("0 " + m_CurrentColor.a);
        while (loop)
        {
            if (m_SwimMode)
            {
                for (float t = 0.0f; t < m_appearTime; t += Time.deltaTime)
                {
                    m_SpriteRenderer.material.color = new Color(m_OriginalColor.r, m_OriginalColor.g, m_OriginalColor.b, m_OriginalColor.a * (t / m_appearTime));
                    Debug.Log("1 " + m_SpriteRenderer.material.color.a);
                    yield return null;
                }
                loop = false;
            }
            else
            {
                for (float t = 0.0f; t < m_fadeTime; t += Time.deltaTime)
                {
                    m_SpriteRenderer.material.color = new Color(m_OriginalColor.r, m_OriginalColor.g, m_OriginalColor.b, m_OriginalColor.a * (1 - t / m_fadeTime));
                    Debug.Log("2 " + m_SpriteRenderer.material.color.a);
                    yield return null;
                }

                loop = false;
            }
        }


        m_ShadeRoutineRunning = false;
        yield return null;
    }


}
