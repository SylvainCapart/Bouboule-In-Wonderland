using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ChestSimpleMgt : MonoBehaviour
{

    [SerializeField] private GameObject m_ExplosionOnDeletePrefab;
    [SerializeField] private GameObject m_CoinPrefab;
    public int m_CoinsMax = 10;
    [SerializeField] private float m_CoinsRangeX = 1f;
    [SerializeField] private float m_CoinsRangeY = 1f;

    private Animator m_Anim;
    [SerializeField] private CameraShake cameraShake;
    private AudioManager m_AudioManager;

    private void Start()
    {
    
        m_Anim = this.GetComponent<Animator>();
        if (m_Anim == null)
            Debug.LogError(this.name + " : Animator not found");
            
        if (cameraShake == null)
            cameraShake = FindObjectOfType<CameraShake>();

        m_AudioManager = FindObjectOfType<AudioManager>();

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Explode();
        }
    }

    private void Explode()
    {
        bool slotFilled = true;

        if (slotFilled == true)
        {
            GameObject explosionClone;
            GameObject coinClone;

            explosionClone = Instantiate(m_ExplosionOnDeletePrefab, transform.position, Quaternion.identity);
            for (int i = 0; i < m_CoinsMax; i++)
            {
                Vector3 position = new Vector3(Random.Range(transform.position.x, transform.position.x + m_CoinsRangeX), Random.Range(transform.position.y, transform.position.y + m_CoinsRangeY), 0);
                coinClone = Instantiate(m_CoinPrefab, position, Quaternion.identity);
                coinClone.transform.parent = GameObject.Find("Coins").transform;
            }
            m_AudioManager.PlaySound("Explosion");
            cameraShake.Shake(0.2f, 0.4f);

            Destroy(explosionClone, m_ExplosionOnDeletePrefab.GetComponent<ParticleSystem>().main.startLifetime.constant + m_ExplosionOnDeletePrefab.GetComponent<ParticleSystem>().main.duration);
            Destroy(this.gameObject);
        }

    }



}

