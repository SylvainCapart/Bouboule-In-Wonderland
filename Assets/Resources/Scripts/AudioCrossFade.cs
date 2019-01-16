using UnityEngine;

public class AudioCrossFade : MonoBehaviour
{
    enum Side { BOTTOM, TOP, RIGHT, LEFT };
    private AudioManager m_AudioManager;
    private Collider2D m_Collider;
    [SerializeField] private Side m_TriggerSide;
    [SerializeField] private string m_SoundPrs;
    [SerializeField] private string m_SoundNxt;
    [SerializeField] private float m_FadeDelay;
    [SerializeField] private float m_AppearDelay;

    // Start is called before the first frame update
    void Start()
    {
        m_AudioManager = FindObjectOfType<AudioManager>();
        m_Collider = GetComponent<Collider2D>();
    }



    private void OnTriggerExit2D(Collider2D collision)
    {

        switch (m_TriggerSide)
        {
            case Side.BOTTOM:
                if (collision.transform.position.y > m_Collider.bounds.center.y)
                    m_AudioManager.CrossFade(m_SoundPrs, m_SoundNxt, m_FadeDelay, m_AppearDelay, m_AudioManager.GetSound(m_SoundNxt).initVol);
                else
                    m_AudioManager.CrossFade(m_SoundNxt, m_SoundPrs, m_AppearDelay, m_FadeDelay, m_AudioManager.GetSound(m_SoundPrs).initVol);
                break;
            case Side.TOP:
                if (collision.transform.position.y < m_Collider.bounds.center.y)
                    m_AudioManager.CrossFade(m_SoundPrs, m_SoundNxt, m_FadeDelay, m_AppearDelay, m_AudioManager.GetSound(m_SoundNxt).initVol);
                else
                    m_AudioManager.CrossFade(m_SoundNxt, m_SoundPrs, m_AppearDelay, m_FadeDelay, m_AudioManager.GetSound(m_SoundPrs).initVol);
                break;
            case Side.RIGHT:
                if (collision.transform.position.x < m_Collider.bounds.center.x)
                    m_AudioManager.CrossFade(m_SoundPrs, m_SoundNxt, m_FadeDelay, m_AppearDelay, m_AudioManager.GetSound(m_SoundNxt).initVol);
                else
                    m_AudioManager.CrossFade(m_SoundNxt, m_SoundPrs, m_AppearDelay, m_FadeDelay, m_AudioManager.GetSound(m_SoundPrs).initVol);
                break;
            case Side.LEFT:
                if (collision.transform.position.x > m_Collider.bounds.center.x)
                    m_AudioManager.CrossFade(m_SoundPrs, m_SoundNxt, m_FadeDelay, m_AppearDelay, m_AudioManager.GetSound(m_SoundNxt).initVol);
                else
                    m_AudioManager.CrossFade(m_SoundNxt, m_SoundPrs, m_AppearDelay, m_FadeDelay, m_AudioManager.GetSound(m_SoundPrs).initVol);
                break;
            default:
                break;
        }
    }


}
