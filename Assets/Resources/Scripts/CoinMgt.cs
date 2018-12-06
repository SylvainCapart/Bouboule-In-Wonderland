using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinMgt : MonoBehaviour {

    private Vector3 m_RespawnPos;
    [SerializeField] private GameObject m_ExplosionCoin;

    // Use this for initialization
    void Start()
    {
        m_RespawnPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (this.transform.position.y < Globals.WORLD_BOTTOM)
        {
            Respawn();
        }
    }

    void Respawn()
    {
        this.transform.position = m_RespawnPos;
        this.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        this.GetComponent<Rigidbody2D>().angularVelocity = 0f;
        this.transform.Rotate(new Vector3(0, 0, 0));
        this.GetComponent<Rigidbody2D>().Sleep();
    }

    private void OnDestroy()
    {
        

    }

    private void OnDisable()
    {
 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            GameObject explosionClone;
            GameObject coinCounter = GameObject.Find("CoinCounter");
            if (coinCounter != null)
            {
                coinCounter.GetComponent<CoinCounter>().IncreaseCoinCount();
            }
            else
            {
                Debug.LogError(gameObject.name + " : coinCounter not found");
            }

            this.gameObject.GetComponent<SpriteRenderer>().enabled = false;
            this.gameObject.GetComponent<Collider2D>().enabled = false;
            
            explosionClone = Instantiate(m_ExplosionCoin, transform.position, Quaternion.identity);
            Destroy(explosionClone, 1f);
            Destroy(this.gameObject, 1f);
        }
    }
}
