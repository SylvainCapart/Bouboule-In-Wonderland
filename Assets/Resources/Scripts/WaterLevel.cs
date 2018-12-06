
using UnityEngine;

public class WaterLevel : MonoBehaviour {

    [SerializeField] private Transform m_WaterLevelPosition;

    public Transform WaterLevelPosition
    {
        get
        {
            return m_WaterLevelPosition;
        }

        set
        {
            m_WaterLevelPosition = value;
        }
    }
}
