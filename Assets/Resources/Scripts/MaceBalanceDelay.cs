using System.Collections;
using UnityEngine;

public class MaceBalanceDelay : MonoBehaviour {

    [SerializeField]
    private Animator m_Anim;

    public float m_animDelay;
    public float m_animSpd;


    // Use this for initialization
    void Start () {
        m_Anim.enabled = false;
        
        StartCoroutine(LaunchAnimation());
    }

    private IEnumerator LaunchAnimation()
    {
        
        yield return new WaitForSeconds(m_animDelay);
        m_Anim.enabled = true;
        m_Anim.speed = m_animSpd;
    }
}
