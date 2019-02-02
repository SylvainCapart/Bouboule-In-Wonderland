using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinMgt : MonoBehaviour {


    [SerializeField] private GameObject m_ExplosionCoin;
    [SerializeField] private GameObject m_Aura;
    private AudioManager audioManager;
    private bool m_IsCollected;
    private bool m_IsCollectable = false;
    private float m_StartCollectDelay = 0.7f;


    private void Start()
    {
        audioManager = AudioManager.instance;
        if (audioManager == null)
        {
            Debug.LogError("No audioManager found in " + this.name);
        }
        StartCoroutine(CollectShutoffCo());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && m_IsCollectable && !m_IsCollected)
        {
            m_IsCollected = true;
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
            Destroy(m_Aura);
            Destroy(explosionClone, 1f);
            Destroy(this.gameObject, 1f);
        }
    }

    private IEnumerator CollectShutoffCo()
    {
        yield return new WaitForSeconds(m_StartCollectDelay);
        m_IsCollectable = true;
    }
}
