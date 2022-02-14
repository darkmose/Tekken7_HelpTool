using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    static private AudioManager instance;
    static public AudioManager Instance { get; }


    private Dictionary<string, AudioClip> _sounds;
    private Dictionary<string, AudioClip> _musics;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource musicSource;

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
        }
        _sounds = InitializeSounds();
        _musics = InitializeMusics();

        Debug.Log($"_sounds: {_sounds.Count}");
        Debug.Log($"_musics: {_musics.Count}");
    }

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private Dictionary<string, AudioClip> InitializeMusics()
    {
        AudioClip[] musicClips = Resources.LoadAll<AudioClip>("AudioSystem/Music");
        var musics = new Dictionary<string, AudioClip>();
        Debug.Log($"Music clips count: {musicClips.Length}");

        for (int i = 0; i < musicClips.Length; i++)
        {
            musics.Add(musicClips[i].name, musicClips[i]);
        }

        return musics;
    }

    public static void AudioMute(bool mute) 
    {
        instance.audioSource.mute = mute;
    }
    public static void MusicMute(bool mute) 
    {
        instance.musicSource.mute = mute;
    }


    private Dictionary<string, AudioClip> InitializeSounds()
    {
        AudioClip[] audioClips = Resources.LoadAll<AudioClip>("AudioSystem/Audio");
        var sounds = new Dictionary<string, AudioClip>();
        Debug.Log($"Audio clips count: {audioClips.Length}");

        for (int i = 0; i < audioClips.Length; i++)
        {
            sounds.Add(audioClips[i].name, audioClips[i]);
        }

        return sounds;
    }

    public static void StopAudio() 
    {
        instance.audioSource.Stop();
    }
    public static void StopMusic() 
    { 
        instance.musicSource.Stop();
    }

    public static AudioClip GetAudio(string sound) 
    {
        return instance._sounds[sound];
    }
    public static AudioClip GetMusic(string music) 
    {
        return instance._musics[music];
    }

    public static void PlayClip(AudioClip clip) 
    {
        instance.audioSource.PlayOneShot(clip);
    }

    static public void PlaySound(string sound)
    {        
        if (instance._sounds.ContainsKey(sound))
        {
            var _soundFx = instance._sounds[sound];
            instance.audioSource.PlayOneShot(_soundFx);
        }
        else
        {
            Debug.Log($"Sound {sound} doesn't exist!");
        }


    }
    static public void PlayMusic(string music) 
    {        
        if (instance._musics.ContainsKey(music))
        {
            var _soundFx = instance._musics[music];
            instance.musicSource.PlayOneShot(_soundFx);
        }
        else
        {
            Debug.Log($"Music {music} doesn't exist!");
        }
    }




}
