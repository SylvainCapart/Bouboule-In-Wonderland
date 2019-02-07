using System.Collections;
using UnityEngine;

public class AudioSourceResetShutoff : MonoBehaviour
{
    private AudioSource m_AudioSource;

    private void Start()
    {
        m_AudioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        GameMaster.OnPlayerKill += MuteSourceOn;
        GameMaster.OnPlayerRespawn += MuteSourceOff;
    }

    private void OnDisable()
    {
        GameMaster.OnPlayerKill -= MuteSourceOn;
        GameMaster.OnPlayerRespawn -= MuteSourceOff;
    }


    private void AudioSourceShutoff()
    {

        StartCoroutine(AudioSourceShutoffCo());

    }

    private IEnumerator AudioSourceShutoffCo()
    {
        m_AudioSource.mute = true;
        yield return new WaitForSeconds(2.5f);
        m_AudioSource.mute = false;
    }

    private void MuteSourceOn()
    {
        m_AudioSource.mute = true;
    }

    private void MuteSourceOff()
    {
        m_AudioSource.mute = false;
    }

}
