using System.Collections;
using UnityEngine;

public class MovingGround : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((collision.transform.tag == "Player"))
        {
            StopAllCoroutines();
            collision.transform.parent = transform;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if ((collision.transform.tag == "Player"))
        {
            if (collision.transform.parent != transform)
            {
                StopAllCoroutines();
                collision.transform.parent = transform;
            }

        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            StartCoroutine(UnparentCo(collision.transform, 0.4f));
        }
    }

    private IEnumerator UnparentCo(Transform child, float delay)
    {
        yield return new WaitForSeconds(delay);
        child.parent = null;
    }

}

