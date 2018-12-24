using UnityEngine;

public class JewelReset : MonoBehaviour
{
    public void ResetJewels()
    {
        foreach(Transform tr in transform)
        {
            if (tr.tag == "Jewel")
            {
                tr.GetComponent<JewelMgt>().Respawn();
            }
        }
    }

}
