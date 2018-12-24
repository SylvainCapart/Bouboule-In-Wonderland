using UnityEngine;

public class SourceFire : MonoBehaviour
{
    public delegate void FireHit();
    public static event FireHit OnFireHit;

    private void OnParticleTrigger()
    {
        Debug.Log("We Triggered something");
        if (OnFireHit != null)
            OnFireHit();
    }
}
