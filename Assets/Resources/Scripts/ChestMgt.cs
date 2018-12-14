using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ChestMgt : MonoBehaviour
{
    private const string m_JewelStr = "Jewel";
    [SerializeField] private GameObject m_ExplosionOnDeletePrefab;
    [SerializeField] private GameObject m_CoinPrefab;
    [SerializeField] private int m_CoinsMax = 10;
    [SerializeField] private float m_CoinsRangeX = 1f;
    [SerializeField] private float m_CoinsRangeY = 1f;
    [SerializeField] private bool[] m_SlotFilled;
    private const float JEWEL_ANIM_LENGTH = 1f;
    private Animator m_Anim;

    private void Start()
    {

        for (int i = 0; i < m_SlotFilled.Length; i++)
        {
            m_SlotFilled[i] = false;
        }

        m_Anim = this.GetComponent<Animator>();
        if (m_Anim == null)
            Debug.LogError(this.name + " : Animator not found");

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == m_JewelStr)
        {
            string collidingJewel = collision.gameObject.name;

            // get the jewel index corresponding to the last char of the string name JewelX, converting to int and substracting 1
            int jewelIndex = (int)System.Char.GetNumericValue(collidingJewel.Substring(collidingJewel.Length - 1)[0]) - 1;

            m_Anim.SetBool("SlotFilled" + (jewelIndex + 1).ToString(), true);

            StartCoroutine(SlotFilledDelay(true, jewelIndex));

            Destroy(collision.gameObject);

        }
    }
    private IEnumerator SlotFilledDelay(bool status, int index)
    {
        bool slotFilled = true;

        yield return new WaitForSeconds(JEWEL_ANIM_LENGTH);
        m_SlotFilled[index] = true;

        for (int i = 0; i < m_SlotFilled.Length; i++)
        {
            if (m_SlotFilled[i] == false)
            {
                slotFilled = false;
                break;
            }
        }

        if (slotFilled == true)
        {
            GameObject explosionClone;
            GameObject coinClone;
            
            explosionClone = Instantiate(m_ExplosionOnDeletePrefab, transform.position, Quaternion.identity);
            for (int i = 0; i < m_CoinsMax; i++)
            {
                Vector3 position = new Vector3( Random.Range(transform.position.x, transform.position.x + m_CoinsRangeX), Random.Range(transform.position.y, transform.position.y + m_CoinsRangeY), 0 );
                coinClone = Instantiate(m_CoinPrefab, position, Quaternion.identity);
                coinClone.transform.parent = GameObject.Find("Coins").transform;
            }
            
            Destroy(explosionClone, m_ExplosionOnDeletePrefab.GetComponent<ParticleSystem>().main.startLifetime.constant + m_ExplosionOnDeletePrefab.GetComponent<ParticleSystem>().main.duration);
            Destroy(this.gameObject);
        }

    }

}

