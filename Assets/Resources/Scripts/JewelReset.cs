using UnityEngine;

public class JewelReset : MonoBehaviour
{
    public void ResetJewels()
    {
        foreach(Transform tr in transform)
        {
            if (tr.tag == "Jewel")
            {
                if (Time.timeScale > 0f)
                    tr.GetComponent<JewelMgt>().Respawn();
            }
        }
    }

}
