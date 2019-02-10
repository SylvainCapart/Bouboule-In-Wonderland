using UnityEngine;

public class FlipOnDestroy : MonoBehaviour
{
    private EnemyAI m_EnemyAI;
    [SerializeField] Transform m_PointZero;


    private void Start()
    {
        m_EnemyAI = GetComponent<EnemyAI>();
    }

    private void OnEnable()
    {
        UpperChestDestroy.OnUpperChestDestroy += Flip;
    }

    private void OnDisable()
    {
        UpperChestDestroy.OnUpperChestDestroy -= Flip;
    }

    void Flip()
    {
        Player player = FindObjectOfType<Player>();
        if (player != null)
            m_EnemyAI.SetTargetState(player.transform, 2, EnemyAI.EnemyState.TARGET);
    }
}
