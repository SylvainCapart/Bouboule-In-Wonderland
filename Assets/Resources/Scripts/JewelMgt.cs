using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JewelMgt : MonoBehaviour {


    private Vector2 m_Speed;
    private Vector2 m_RespawnPos;
    private float m_RespawnLimitY;

    private void Start()
    {

        m_RespawnPos = this.transform.position;

        GameObject waterMap = GameObject.Find("Tilemap_Water");
        if (waterMap != null)
            m_RespawnLimitY = waterMap.GetComponentInChildren<WaterLevel>().WaterLevelPosition.position.y;
        else
        {
            Debug.LogError(this.gameObject.name + " : tilemap_water could not be found");
        }

        
    }

    // Update is called once per frame
    void Update () {

        m_Speed = GetComponent<Rigidbody2D>().velocity;

        if (Mathf.Abs(m_Speed.x) > 4f || Mathf.Abs(m_Speed.y) > 6f)
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));


        if (this.transform.position.y < m_RespawnLimitY )
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


}
