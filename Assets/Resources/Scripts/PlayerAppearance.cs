using UnityEngine;

public class PlayerAppearance : MonoBehaviour {

    [SerializeField] private Renderer m_SpriteRenderer;
    [SerializeField] private Color m_NormalColor;
    [SerializeField] private Color m_SwimColor;

    // Use this for initialization
    void Start () {
        if (m_SpriteRenderer == null)
            m_SpriteRenderer = GetComponent<Renderer>();
    }

    private void OnEnable()
    {
        CharacterController2D.MovementStatusChange += ChangeColor;
    }

    private void OnDisable()
    {
        CharacterController2D.MovementStatusChange -= ChangeColor;
    }

    void ChangeColor(string statusname, bool state)
    {
        if (statusname == "Swim")
        {
            if (state == true)
            {
                m_SpriteRenderer.material.color = m_SwimColor;
            }
            else
            {
                m_SpriteRenderer.material.color = m_NormalColor;
            }
        }
    }
}
