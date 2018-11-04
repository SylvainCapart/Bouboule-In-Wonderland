using UnityEngine;


public class CoinRespawn : MonoBehaviour {

    private Vector3 m_RespawnPos;

	// Use this for initialization
	void Start () {
        m_RespawnPos = transform.position;
    }
	
	// Update is called once per frame
	void Update () {
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
        this.GetComponent<Rigidbody2D>().Sleep();
    }
}
