using System.Collections;
using UnityEngine;

[RequireComponent (typeof(SpriteRenderer))]

public class Tiling : MonoBehaviour {

    public float offsetX = 2; // the offset so that we don't get any weird errors

    // check before instantiating stuff
    public bool hasRightBuddy = false;
    public bool hasLeftBuddy = false;

    public bool reverseScale = false; // used if the object is not tilable

    private float spriteWidth = 0f; // wifdth of our element
    private Camera cam;
    private Transform myTransform;

    private void Awake()
    {
        cam = Camera.main;
        myTransform = transform;
    }


    // Use this for initialization
    void Start () {
        SpriteRenderer sRenderer = GetComponent<SpriteRenderer>();
        spriteWidth = sRenderer.sprite.bounds.size.x;
		
	}
	
	// Update is called once per frame
	void Update () {
		if (hasLeftBuddy == false || hasRightBuddy == false)
        {
            // calculate the camera's extent ( half the width ) of what the camera can see in world coodinates
            float camHorizontalExtent = cam.orthographicSize * Screen.width / Screen.height;

            // calculate the x position where the camera can see the edge of the sprite
            float edgeVisiblePositionRight = (myTransform.position.x + spriteWidth / 2) - camHorizontalExtent;
            float edgeVisiblePositionLeft = (myTransform.position.x - spriteWidth / 2) + camHorizontalExtent;

            // can we see the edge of the element and the call MakeNewBuddy if we can
            if (cam.transform.position.x >= edgeVisiblePositionRight - offsetX && !hasRightBuddy)
            {
                MakeNewBuddy(1);
                hasRightBuddy = true;
            }
            else if (cam.transform.position.x <= edgeVisiblePositionLeft + offsetX && !hasLeftBuddy)
            {
                MakeNewBuddy(-1);
                hasLeftBuddy = true;
            }
        }
	}

    // creates a new buddy on the side required
    void MakeNewBuddy(int rightOrLeft)
    {
        //calculating the new position for our new buddy
        Vector3 newPosition = new Vector3(myTransform.position.x + spriteWidth * rightOrLeft, myTransform.position.y, myTransform.position.z);

        // instantiating new buddy and storing it
        Transform newBuddy = Instantiate(myTransform, newPosition, myTransform.rotation);

        newBuddy.parent = myTransform;

        //if not tilable, reverse x size of our object to get rid of ugly seams
        if (reverseScale)
        {
            newBuddy.localScale = new Vector3(-1, newBuddy.localScale.y, newBuddy.localScale.z);
            //newBuddy.localScale = new Vector3( -1, newBuddy.localScale.y, newBuddy.localScale.z);
            //newBuddy.GetComponent<Tiling>().reverseScale = !reverseScale;
        }

        

        if(rightOrLeft > 0)
        {
            newBuddy.GetComponent<Tiling>().hasLeftBuddy = true;
        }
        else
        {
            newBuddy.GetComponent<Tiling>().hasRightBuddy = true;
        }
    }
}
