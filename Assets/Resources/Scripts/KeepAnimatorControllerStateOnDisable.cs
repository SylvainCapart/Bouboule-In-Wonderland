using UnityEngine;

public class KeepAnimatorControllerStateOnDisable : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Animator anim = GetComponent<Animator>();
        if (anim != null)
            anim.keepAnimatorControllerStateOnDisable = true;
    }

}