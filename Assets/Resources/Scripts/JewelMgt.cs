using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JewelMgt : MonoBehaviour {


    private Vector2 m_Speed;
    private Vector2 m_RespawnPos;
    private float m_RespawnLimitY;
    [SerializeField] private LayerMask m_WhatIsGround;

    private void Start()
    {

        m_RespawnPos = this.transform.position;

        /*GameObject waterMap = GameObject.Find("Tilemap_Water");
        if (waterMap != null)
            m_RespawnLimitY = waterMap.GetComponentInChildren<WaterLevel>().WaterLevelPosition.position.y;
        else
        {
            Debug.LogError(this.gameObject.name + " : tilemap_water could not be found");
        }*/

        
    }

    // Update is called once per frame
    void Update () {

        m_Speed = GetComponent<Rigidbody2D>().velocity;
        bool speedExceeded;

        speedExceeded = Mathf.Abs(m_Speed.y) > 4f;

        //  This speed check was done to avoid a collision bug when the jewel was falling at high speed, causing it to enter in the ground
        if (speedExceeded)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector3.down, 1f, m_WhatIsGround);

            if (hit.collider != null && hit.transform.gameObject.tag == "Ground")
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            }
        }
    }

    public void Respawn()
    {
        this.transform.position = m_RespawnPos;
        this.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        this.GetComponent<Rigidbody2D>().angularVelocity = 0f;
        this.transform.Rotate(new Vector3(0, 0, 0));
        this.GetComponent<Rigidbody2D>().Sleep();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Water")
            Respawn();
    }

}
