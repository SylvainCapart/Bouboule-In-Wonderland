using UnityEngine;
using UnityEngine.UI;

public class ChestMgt : MonoBehaviour {

    private const string m_CoinStr = "Coin";
    private int m_CoinsPrs = 0;
    private int m_CoinsRqMax = 3;
    [SerializeField] private Text m_CoinsText;
    [SerializeField] private GameObject m_ExplosionOnDeletePrefab;


    private void Start()
    {
        CoinsRqTxtUpdate();
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == m_CoinStr)
        {
            if (m_CoinsPrs < m_CoinsRqMax)
            {
                m_CoinsPrs++;
                CoinsRqTxtUpdate();
                Destroy(collision.gameObject, 0.25f);
            }

            if (m_CoinsPrs == m_CoinsRqMax)
            {
                GameObject explosionClone;
                explosionClone = Instantiate(m_ExplosionOnDeletePrefab, transform.position, Quaternion.identity);
                Destroy(explosionClone, m_ExplosionOnDeletePrefab.GetComponent<ParticleSystem>().main.startLifetime.constant + m_ExplosionOnDeletePrefab.GetComponent<ParticleSystem>().main.duration);

                //this.transform.GetComponent<SpriteRenderer>().enabled = false;
                //this.transform.GetComponentInChildren<Canvas>().enabled = false;
                Destroy(this.gameObject);

            }
        }
    }

    private void CoinsRqTxtUpdate()
    {
        m_CoinsText.text = m_CoinsPrs.ToString() + " / " + m_CoinsRqMax.ToString();
    }
    

}
