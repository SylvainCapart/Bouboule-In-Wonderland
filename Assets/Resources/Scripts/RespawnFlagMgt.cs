using UnityEngine;

public class RespawnFlagMgt : MonoBehaviour {

    private bool m_FlagTriggered = false;
    private Animator m_Anim;

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
    }
	

    private void OnTriggerEnter2D(Collider2D collision)
    {
        m_FlagTriggered = true;
        m_Anim.SetBool("FlagTriggered", true);
    }


}
