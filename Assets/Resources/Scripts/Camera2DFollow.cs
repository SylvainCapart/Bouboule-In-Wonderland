using System.Collections;
using UnityEngine;

namespace UnityStandardAssets._2D
{
    public class Camera2DFollow : MonoBehaviour
    {
        public static Camera2DFollow instance;

        public Transform target;
        public float m_Damping = 1;
        public float lookAheadFactor = 3;
        public float lookAheadReturnSpeed = 0.5f;
        public float lookAheadMoveThreshold = 0.1f;
        public float yPosRestriction = -1;
        public float xPosRestriction = 1.42f;
        public float zNewPosition = -10f;
        public bool m_ResetParentAtStart = false;

        private float m_OffsetZ;
        private Vector3 m_LastTargetPosition;
        private Vector3 m_CurrentVelocity;
        private Vector3 m_LookAheadPos;

        // Use this for initialization
        private void Start()
        {
            m_LastTargetPosition = target.position;
            m_OffsetZ = (transform.position - target.position).z;
            if (m_ResetParentAtStart)
                transform.parent = null;
        }

        private void Awake()
        {

            Transform playerTransfom = FindObjectOfType<Player>().transform;
            if (playerTransfom != null)
                target = playerTransfom;
            this.transform.position = new Vector3(target.transform.position.x, target.transform.position.y, zNewPosition);


            if (instance != null)
            {
                if (instance != this)
                {
                    Destroy(this.gameObject);
                }
            }
            else
            {
                instance = this;
            }

        }

        // Update is called once per frame
        private void Update()
        {
            if (target == null)
            {
                GameObject sResult = GameObject.FindGameObjectWithTag("Player");
                if (sResult == null)
                {
                    return;
                }
                else
                {
                    target = sResult.transform;
                }
            }

            // only update lookahead pos if accelerating or changed direction
            float xMoveDelta = (target.position - m_LastTargetPosition).x;

            bool updateLookAheadTarget = Mathf.Abs(xMoveDelta) > lookAheadMoveThreshold;

            if (updateLookAheadTarget)
            {
                m_LookAheadPos = lookAheadFactor * Vector3.right * Mathf.Sign(xMoveDelta);
            }
            else
            {
                m_LookAheadPos = Vector3.MoveTowards(m_LookAheadPos, Vector3.zero, Time.deltaTime * lookAheadReturnSpeed);
            }

            Vector3 aheadTargetPos = target.position + m_LookAheadPos + Vector3.forward * m_OffsetZ;
            Vector3 newPos = Vector3.SmoothDamp(transform.position, aheadTargetPos, ref m_CurrentVelocity, m_Damping);

            if (newPos.x < xPosRestriction)
                newPos.x = xPosRestriction;

            newPos = new Vector3(newPos.x, Mathf.Clamp(newPos.y, yPosRestriction, Mathf.Infinity), newPos.z);

            transform.position = newPos;

            m_LastTargetPosition = target.position;
        }


        public IEnumerator DampingShutOff(float delay)
        {
            float oldamping = m_Damping;

            m_Damping = 0f;

            yield return new WaitForSeconds(delay);

            m_Damping = oldamping;
        }
    }

}
