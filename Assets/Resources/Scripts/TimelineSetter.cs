using UnityEngine;
using UnityEngine.Playables;

public class TimelineSetter : MonoBehaviour
{

    private PlayableDirector m_TimelineInit;
    // Start is called before the first frame update

    void Start()
    {
        m_TimelineInit = GetComponent<PlayableDirector>();
        if (GeneralSceneMgt.instance != null && GeneralSceneMgt.instance.IsMenuPlayedOnce)
            m_TimelineInit.time = m_TimelineInit.duration;
    }

}
