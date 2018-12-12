using UnityEngine;

public class RespawnFlagMgt : MonoBehaviour {

    private bool m_FlagTriggered = false;
    private Animator m_Anim;
    private BoxCollider2D m_Box2D;

    public bool FlagTriggered
    {
        get
        {
            return m_FlagTriggered;
        }

        set
        {
            m_FlagTriggered = value;
        }
    }

    // Use this for initialization
    void Start () {
        m_Anim = this.GetComponentInParent<Animator>();
        if (m_Anim == null)
            Debug.LogError(this.name + " : Animator not found");

        m_Box2D = this.GetComponentInParent<BoxCollider2D>();
        if (m_Box2D == null)
            Debug.LogError(this.name + " : BoxCollider2D not found");

    }
	

    private void OnTriggerEnter2D(Collider2D collision)
    {
        m_FlagTriggered = true;
        m_Anim.SetBool("FlagTriggered", true);
        m_Box2D.enabled = false;
        GameMaster.SpawnPoint = this.transform.position;
    }


}
