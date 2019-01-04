using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallaxing : MonoBehaviour {

    public Transform[] backgrounds; // array of backgrounds
    public float[] m_InitialBackgroundZpos; // array of backgrounds
    private float[] parallaxScales; // proportions of the camera's movement to move the backgrounds by
    public float smoothing = 1f;    // how smooth the parralax is going to be

    private Transform cam;          // reference to the mains camera transform
    private Vector3 previousCamPos;  // position of the camera in the previous frame

    // called before Start(). Great for references
    void Awake()
    {
        // set up camera reference
        cam = Camera.main.transform;
    }

    // Use this for initialization
    void Start () {
        // the previous frame had the current frame's camera position
        previousCamPos = cam.position;
        parallaxScales = new float[backgrounds.Length];

        // assigning corresponding parallax scales
        for (int i = 0; i < backgrounds.Length; i++)
        {

            backgrounds[i].position = new Vector3(cam.position.x, backgrounds[i].position.y, m_InitialBackgroundZpos[i]);

            parallaxScales[i] = backgrounds[i].position.z * -1;



        }
    }
	
	// Update is called once per frame
	void Update () {
        for (int i = 0; i < backgrounds.Length; i++)
        {
            // the parallax is the opposite of the camerra movemen because the previous frame multiplied by the scale
            float parallax = (previousCamPos.x - cam.position.x) * parallaxScales[i];

            // set a target x position which is the current position plus the parallax
            float backgroungTargetPosX = backgrounds[i].position.x + parallax;

            // create a target position which is the background's current position with its taget x position
            Vector3 backgroundTargetPos = new Vector3(backgroungTargetPosX, backgrounds[i].position.y, backgrounds[i].position.z);

            // fade between current position and the target position using lerp
            backgrounds[i].position = Vector3.Lerp(backgrounds[i].position, backgroundTargetPos, smoothing * Time.deltaTime);
        }

        //set previousCamPos to the camera's position at the end of the frame
        previousCamPos = cam.position;
	}
}
