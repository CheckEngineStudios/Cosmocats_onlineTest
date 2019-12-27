using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    [HideInInspector]
    public AudioSource source;

    [Range(0f, 1f)]
    public float volume = 0.7f;
    [Range(0.5f, 4f)]
    public float pitch = 1f;
    public float originalVolume;

    [Range(0f, 0.5f)]
    public float randomVolume = 0.1f;
    [Range(0f, 0.5f)]
    public float randomPitch = 0.1f;

    public bool isRandom = false;

    public bool loop = false;

    public void SetSource(AudioSource _source)
    {
        source = _source;
        source.clip = clip;
        source.loop = loop;
    }

    public void Play()
    {
        if (isRandom)
        {
            source.volume = volume * (1 + Random.Range(-randomVolume / 2f, randomVolume / 2f));
            source.pitch = pitch * (1 + Random.Range(-randomPitch / 2f, randomPitch / 2f));
        }
        else
        {
            source.volume = volume;
            source.pitch = pitch;
        }
        source.Play();
    }
    public void Stop()
    {
        source.Stop();
    }
    public void ChangeMute()
    {
        source.mute = !source.mute;
    }
    public void ChangePitch()
    {
        source.pitch = pitch;
    }
    public bool IsMuted()
    {
        return source.mute;
    }
    public void ChangeVolume(float desVolume)
    {
        volume = originalVolume * desVolume;
        source.volume = volume;
    }

}

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    [SerializeField]
    Sound[] sounds;

    void Awake()
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
            //this needs further checking
            DontDestroyOnLoad(this);

        }
    }

    void Start()
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            GameObject _go = new GameObject("Sound_" + i + "_" + sounds[i].name);
            _go.transform.SetParent(this.transform);
            sounds[i].SetSource(_go.AddComponent<AudioSource>());
            sounds[i].originalVolume = sounds[i].volume;
        }
    }


    public void PlaySound(string _name)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == _name)
            {
                sounds[i].Play();
                return;
            }
        }

        //no sound with _name
        Debug.LogWarning("AudioManager: Sound not found in list: " + _name);
    }
    public void PlayOnce(string _name)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == _name)
            {
                if (!sounds[i].source.isPlaying)
                {
                    sounds[i].Play();
                    return;
                }
            }
        }
    }
    public void PitchControl(string _name, float _pitch)
    {
        //this loop may fuck up the performance. Maybe needs a cache? 
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == _name)
            {
                //instead of calling it twice, changing value bla bla bla I could just make an function which will pass the value
                sounds[i].pitch = _pitch;
                sounds[i].ChangePitch();
                return;
            }
        }
    }
    public bool IsMuted(string _name)
    {
        for (int i = 0; i < sounds.Length; i++)
            if (sounds[i].name == _name)
                return sounds[i].IsMuted();
        //at this point all else failed
        Debug.LogError("Can't find desired sound name or the sound doesnt know if it's muted");
        return false;
    }
    public void ChangeMute(string _name)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == _name)
            {
                sounds[i].ChangeMute();
                return;
            }
        }
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

        //no sound with _name
        Debug.LogWarning("AudioManager: Sound not found in list: " + _name);
    }
    public void ChangeVolume(float desVolume)
    {
        //there is a better way
        //foreach (Sound s in sounds)
        //    s.ChangeVolume(desVolume);
        AudioListener.volume = desVolume;
    }
}
