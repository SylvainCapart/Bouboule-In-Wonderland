using System.Collections;
using UnityEngine;

public class GravityChange : MonoBehaviour
{

    [SerializeField] private AreaEffector2D m_AreaEffector;
    private const float EPSILON = 0.01f;
    [SerializeField] private CharacterController2D.GroundDirection m_TargetDir;
    private float[] m_DirAngles;
    private bool m_ShutOffExit;
    private float m_ForceMagnitude;
    private bool m_ZoneEntered;

    public CharacterController2D.GroundDirection TargetDir
    {
        get
        {
            return m_TargetDir;
        }

        set
        {
            if (m_TargetDir == value) return;
            m_TargetDir = value;
        }
    }



    private void Start()
    {
        m_DirAngles = new float[4];
        m_DirAngles[(int)CharacterController2D.GroundDirection.LEFT] = 180f;
        m_DirAngles[(int)CharacterController2D.GroundDirection.RIGHT] = 0f;
        m_DirAngles[(int)CharacterController2D.GroundDirection.TOP] = 90f;
        m_DirAngles[(int)CharacterController2D.GroundDirection.BOTTOM] = 270f;

        if (System.Math.Abs(m_AreaEffector.forceAngle - m_DirAngles[(int)CharacterController2D.GroundDirection.LEFT]) < EPSILON)
            TargetDir = CharacterController2D.GroundDirection.LEFT;
        else if (System.Math.Abs(m_AreaEffector.forceAngle - m_DirAngles[(int)CharacterController2D.GroundDirection.RIGHT]) < EPSILON)
            TargetDir = CharacterController2D.GroundDirection.RIGHT;
        else if (System.Math.Abs(m_AreaEffector.forceAngle - m_DirAngles[(int)CharacterController2D.GroundDirection.TOP]) < EPSILON)
            TargetDir = CharacterController2D.GroundDirection.TOP;
        else if (System.Math.Abs(m_AreaEffector.forceAngle - m_DirAngles[(int)CharacterController2D.GroundDirection.BOTTOM]) < EPSILON)
            TargetDir = CharacterController2D.GroundDirection.BOTTOM;
        else
        {
            TargetDir = CharacterController2D.GroundDirection.BOTTOM;
            Debug.LogError(name + " : gravity change direction unkown");
        }

        m_ForceMagnitude = m_AreaEffector.forceMagnitude;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            StartCoroutine(ShutOffExit(0.01f));
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.GetComponent<Rigidbody2D>().gravityScale = 0f;
            collision.GetComponent<CharacterController2D>().m_GroundDir = TargetDir;
            collision.transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, transform.rotation.y, m_DirAngles[(int)TargetDir] - 270f));
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (collision.GetComponent<CharacterController2D>().Climbing)
                m_AreaEffector.forceMagnitude = 0;
            else
                m_AreaEffector.forceMagnitude = m_ForceMagnitude;

            collision.GetComponent<Rigidbody2D>().gravityScale = 0f;

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player" && !m_ShutOffExit)
        {
            if (!collision.GetComponent<CharacterController2D>().Climbing)
                collision.GetComponent<Rigidbody2D>().gravityScale = 3.5f;
            collision.GetComponent<CharacterController2D>().m_GroundDir = CharacterController2D.GroundDirection.BOTTOM;
            collision.transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, transform.rotation.y, 0f));

        }
    }

    private IEnumerator ShutOffExit(float delay)
    {
        m_ShutOffExit = true;
        yield return new WaitForSeconds(delay);
        m_ShutOffExit = false;
    }

}
