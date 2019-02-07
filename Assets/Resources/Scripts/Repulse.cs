using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Repulse : MonoBehaviour
{

    private float m_bumpActivationDelay = 0.5f;
    private float m_lastBumpActivation = 0.0f;
    public float m_repulsiveForce = 10f;
    public float m_jewelRepulsiveForce = 20f;
    private Animator m_Anim;

    // Use this for initialization
    void Start()
    {
        m_Anim = this.GetComponentInParent<Animator>();
        if (m_Anim == null)
            Debug.LogError(this.name + " : Animator not found");
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        m_Anim.SetBool("BounceActivation", true);

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Rigidbody2D incRb;
        Vector2 repulsiveVector;


        if (collision.tag == "Player" || collision.tag == "Jewel")
        {
            if ((Time.time - m_lastBumpActivation) > m_bumpActivationDelay)
            {
                repulsiveVector = new Vector2(collision.transform.position.x - this.transform.position.x, collision.transform.position.y - this.transform.position.y);
                incRb = collision.GetComponent<Rigidbody2D>();
                if (incRb != null && m_Anim.GetBool("BounceActivation"))
                {
                    if (collision.tag == "Player")
                        incRb.AddForce(repulsiveVector * m_repulsiveForce, ForceMode2D.Impulse);
                    else if (collision.tag == "Jewel")
                        incRb.AddForce(repulsiveVector * m_jewelRepulsiveForce, ForceMode2D.Impulse);
                }

                m_lastBumpActivation = Time.time;
            }


        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        m_Anim.SetBool("BounceActivation", false);
    }



}
