using System.Collections;
using UnityEngine;

[System.Serializable]
public class Expression
{
    public Sprite sprite;
    public string name;
};


[RequireComponent(typeof(Animator), typeof(SpriteRenderer))]
public class ExpressionMgt : MonoBehaviour
{
    public enum ExpressionSymbol { QUESTION, EXCLAMATION, SLEEP };

    private Animator m_Anim;
    private SpriteRenderer m_SpriteRenderer;
    [SerializeField] private Expression[] m_ExpressionArray;
    private const int m_ExprNb = 3;

    private void Awake()
    {
        if (m_Anim == null)
            m_Anim = GetComponent<Animator>();
        if (m_SpriteRenderer == null)
            m_SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void CallExpression(ExpressionSymbol expressionid)
    {
        if (m_Anim != null)
        {
            m_Anim.SetBool(m_ExpressionArray[(int)expressionid].name, true);
        }
    }

    public void CancelExpression()
    {
        if (m_SpriteRenderer != null)
            m_SpriteRenderer.sprite = null;

        if (m_Anim != null)
        {
            for (int i = 0; i < m_ExprNb; i++)
            {
                m_Anim.SetBool(m_ExpressionArray[i].name, false);
            }
        }
    }

}
