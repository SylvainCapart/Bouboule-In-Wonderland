using System.Collections;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;

    public AudioSource source;

    [Range(0f,1f)]
    public float volume = 0.7f;

    [Range(0.5f, 1.5f)]
    public float pitch = 1f;

    [Range(0f, 0.5f)]
    public float randomVolume = 0.1f;

    [Range(0f, 0.5f)]
    public float randomPitch = 0.1f;

    public bool loop = false;

    [HideInInspector] public bool playedOnce;
    public float initVol;

    public void SetSource(AudioSource _source)
    {
        source = _source;
        source.clip = clip;
        source.loop = loop;
    }

    public void Play()
    {
        source.volume = volume * ( 1 + Random.Range(-randomVolume/2f, randomVolume/2f));
        source.pitch = pitch * (1 + Random.Range(-randomPitch / 2f, randomPitch / 2f)); ;
        source.Play();
    }

    public void Stop()
    {
        if (source != null)
        source.Stop();
    }

}

public class AudioManager : MonoBehaviour {

    [SerializeField] private Sound[] sounds;
    private Sound m_MainSound;

    public static AudioManager instance;
    private bool m_IsCrossFading;
    private Coroutine m_CrossFadeCo;
    [SerializeField] private string m_InitSoundStr = "Music";

    private void Awake()
    {
        if (instance != null)
        {
            if (instance != this)
            {
                Destroy(this.gameObject);
            }
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
    }

    private void Start()
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            GameObject _obj = new GameObject("Sound_" + i + "_" + sounds[i].name);
            _obj.transform.SetParent(this.transform);
            sounds[i].SetSource(_obj.AddComponent<AudioSource>());
            sounds[i].initVol = sounds[i].volume;
        }
        MainSound = GetSound(m_InitSoundStr);
        if (!GameMaster.gm.m_DebugMode)
            PlaySound(MainSound.name);
        else
            PlaySound("Music");

    }

    private void OnEnable()
    {
        GameMaster.OnPlayerKill += MuteWaterSounds;
        GameMaster.OnPlayerRespawn += AllowWaterSounds;
    }

    private void OnDisable()
    {
        GameMaster.OnPlayerKill -= MuteWaterSounds;
        GameMaster.OnPlayerRespawn -= AllowWaterSounds;
    }

    public void PlaySound(string _name)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == _name)
            {
                if(sounds[i].name == "Coin")
                {
                    sounds[i].pitch += Random.Range(-sounds[i].randomPitch / 2f, sounds[i].randomPitch / 2f);
                }
                if (!sounds[i].source.isPlaying)
                    sounds[i].Play();
                return;
            }

        }
        Debug.Log("No sound found in AudioManager with that name : " + _name);
    }

    public void PlaySoundAt(string _name, float start)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == _name)
            {
                if (!sounds[i].source.isPlaying)
                {
                    sounds[i].source.time = start;
                    sounds[i].Play();
                }

                return;
            }

        }
        Debug.Log("No sound found in AudioManager with that name : " + _name);
    }

    public bool IsSoundPlayed(string _name)
    {

        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == _name)
            {
                return sounds[i].source.isPlaying;
            }

        }
        Debug.Log("No sound found in AudioManager with that name : " + _name);
        return false;
    }

    public AudioSource GetSource(string _name)
    {
        AudioSource source = null;
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == _name)
            {
                source = sounds[i].source;
            }
        }
        return source;

    }

    public Sound GetSound(string _name)
    {
        Sound sound = null;
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == _name)
            {
                sound = sounds[i];
            }
        }
        return sound;

    }


    public void StopSound(string _name)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == _name)
            {
                sounds[i].Stop();
                return;
            }

        }
        Debug.Log("No sound found in AudioManager with that name : " + _name);
    }

    public void CrossFade(string soundToFade, string soundToPlay, float fadeDelay, float appearDelay, float targetVolume)
    {
        if (soundToFade == soundToPlay)
            return;

        Sound source1 = GetSound(soundToFade);
        if (source1 == null)
        {
            Debug.Log("No sound found in AudioManager with that name : " + soundToFade);
            return;
        }
        Sound source2 = GetSound(soundToPlay);

        MainSound = source2;

        if (source2 == null)
        {
            Debug.Log("No sound found in AudioManager with that name : " + soundToPlay);
            return;
        }

        if (m_IsCrossFading == true)
        {
            StopCoroutine(m_CrossFadeCo);
            m_IsCrossFading = false;
        }

        if (m_IsCrossFading == false)
            m_CrossFadeCo = StartCoroutine(CrossFade(source1, source2, fadeDelay, appearDelay, targetVolume));
    }

    public IEnumerator CrossFade(Sound soundToFade, Sound soundToPlay, float fadeDelay, float appearDelay, float targetVolume)
    {
        m_IsCrossFading = true;

        float initialFadeVolume = soundToFade.source.volume;
        float initialPlayVolume = soundToPlay.source.volume;

        if (soundToPlay.source.isPlaying == false)
        {
            initialPlayVolume = 0f;
            soundToPlay.Play();
        }

        for (float t = 0.0f; t < fadeDelay ; t += Time.deltaTime)
        {
            soundToFade.source.volume = initialFadeVolume * (1 - t/ fadeDelay) ;
            soundToPlay.source.volume = initialPlayVolume * (1 - t/appearDelay) + targetVolume * t / appearDelay;

            yield return null;
        }
        soundToFade.Stop();
        m_IsCrossFading = false;

        yield return null;
    }

    public void StopSoundSmooth(string soundToFade, float fadeDelay)
    {

        Sound source1 = GetSound(soundToFade);
        if (source1 == null)
        {
            Debug.Log("No sound found in AudioManager with that name : " + soundToFade);
            return;
        }

        m_CrossFadeCo = StartCoroutine(StopSoundSmooth(source1, fadeDelay));
    }

    public IEnumerator StopSoundSmooth(Sound soundToFade, float fadeDelay)
    {

        float initialFadeVolume = soundToFade.source.volume;

        for (float t = 0.0f; t < fadeDelay; t += Time.deltaTime)
        {
            soundToFade.source.volume = initialFadeVolume * (1 - t / fadeDelay);
            yield return null;
        }
        soundToFade.Stop();

        yield return null;
    }


    public Sound MainSound
    {
        get
        {
            return m_MainSound;
        }

        set
        {
            m_MainSound = value;
        }
    }

    public void MuteWaterSounds()
    {
        Sound tempsound;
        tempsound = GetSound("Bubble");
        if (tempsound != null && tempsound.source != null)
            tempsound.source.mute = true;

        tempsound = GetSound("Swimming");
        if (tempsound != null && tempsound.source != null)
            tempsound.source.mute = true;


    }

    public void AllowWaterSounds()
    {
        Sound tempsound;
        tempsound = GetSound("Bubble");
        if (tempsound != null && tempsound.source != null)
            tempsound.source.mute = false;

        tempsound = GetSound("Swimming");
        if (tempsound != null && tempsound.source != null)
            tempsound.source.mute = false;
    }

}
