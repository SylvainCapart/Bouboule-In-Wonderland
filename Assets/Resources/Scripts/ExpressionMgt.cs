using System.Collections;
using UnityEngine;

[System.Serializable]
public class Expression
{
    public Sprite sprite;
    public string name;
    public float duration;
    private bool isResetting;

    public bool IsResetting
    {
        get{return isResetting;}
        set{isResetting = value;}
    }
};


[RequireComponent(typeof(Animator), typeof(SpriteRenderer))]
public class ExpressionMgt : MonoBehaviour
{
    public enum ExpressionSymbol { QUESTION, EXCLAMATION, SLEEP, NONE };

    private Animator m_Anim;
    private SpriteRenderer m_SpriteRenderer;
    [SerializeField] private Expression[] m_ExpressionArray;
    private const int m_ExprNb = 3;

    private void Start()
    {
        if (m_Anim == null)
            m_Anim = GetComponent<Animator>();
        if (m_SpriteRenderer == null)
            m_SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void CallExpression(ExpressionSymbol expressionid)
    {
        switch(expressionid)
        {
            case ExpressionSymbol.QUESTION:
            case ExpressionSymbol.EXCLAMATION:
                m_SpriteRenderer.sprite = m_ExpressionArray[(int)expressionid].sprite;
                if (!m_ExpressionArray[(int)expressionid].IsResetting)
                    StartCoroutine(ResetExpression(expressionid, m_ExpressionArray[(int)expressionid].duration));
                break;
            case ExpressionSymbol.SLEEP:
                m_SpriteRenderer.sprite = m_ExpressionArray[(int)expressionid].sprite;
                m_Anim.SetBool(m_ExpressionArray[(int)expressionid].name, true);
                break;
            case ExpressionSymbol.NONE:
                m_SpriteRenderer.sprite = null;
                for (int i = 0; i < m_ExprNb; i++)
                {
                    m_Anim.SetBool(m_ExpressionArray[i].name, false);
                }

                break;
            default:
                break;
        }
    }

    private IEnumerator ResetExpression(ExpressionSymbol expressionid, float delay)
    {
        m_ExpressionArray[(int)expressionid].IsResetting = true;

        yield return new WaitForSeconds(delay);
        m_SpriteRenderer.sprite = null;
        m_Anim.SetBool(m_ExpressionArray[(int)expressionid].name, false);

        m_ExpressionArray[(int)expressionid].IsResetting = false;
    }



}
