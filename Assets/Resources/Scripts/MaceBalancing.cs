﻿using UnityEngine;

public class MaceBalancing : MonoBehaviour {

	// Update is called once per frame
	void Update () {
        Debug.Log(this.gameObject.GetComponent<Rigidbody2D>().position);
        this.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(10f, 0f));
    }
}
