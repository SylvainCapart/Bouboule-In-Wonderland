using UnityEngine;

public class UpperChestDestroy : MonoBehaviour
{

    public delegate void UpperChestDestroyDel();
    public static event UpperChestDestroyDel OnUpperChestDestroy;

    private void OnDestroy()
    {
        if (OnUpperChestDestroy != null)
            OnUpperChestDestroy();
    }
}
