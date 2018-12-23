using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinMgt : MonoBehaviour {


    [SerializeField] private GameObject m_ExplosionCoin;
    private AudioManager audioManager;


    private void Start()
    {
        audioManager = AudioManager.instance;
        if (audioManager == null)
        {
            Debug.LogError("No audioManager found in " + this.name);
        }
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

            audioManager.PlaySound("Coin");

            explosionClone = Instantiate(m_ExplosionCoin, transform.position, Quaternion.identity);
            explosionClone.transform.SetParent(GameObject.Find("Coins").transform);
            Destroy(explosionClone, 1f);
            Destroy(this.gameObject, 1f);
        }
    }
}
