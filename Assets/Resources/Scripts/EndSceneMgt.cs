using UnityEngine;

public class EndSceneMgt : MonoBehaviour
{
    private AudioManager m_AudioManager;

    // Start is called before the first frame update
    void Start()
    {
        m_AudioManager = AudioManager.instance;
        m_AudioManager.StopSoundSmooth(m_AudioManager.MainSound.name, 2f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
