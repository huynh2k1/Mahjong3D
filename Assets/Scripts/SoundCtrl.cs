using System.Collections.Generic;
using UnityEngine;

public class SoundCtrl : MonoBehaviour
{
    public static SoundCtrl I;
    [Header("Music")]
    public AudioSource music_source;
    public AudioClip music_sound;

    [Header("Sound")]
    public AudioSource[] sound_sources;
    private Queue<AudioSource> queue_sources;

    public AudioClip click, win, merge, incorrect, selectblock;

    private void Awake()
    {
        I = this;
    }

    private void Start()
    {
        Initialize();
    }

    public virtual void Initialize()
    {
        queue_sources = new Queue<AudioSource>(sound_sources);
        //UpdateVolume();
        StartMusic();
    }

    public void UpdateVolume()
    {
        OnChangeVolumnSounds();
        OnChangeVolumnMusic();
    }

    public virtual void OnChangeVolumnSounds()
    {
        foreach (var sound in sound_sources)
        {
            sound.mute = Data.Sound;
        }
    }

    public virtual void OnChangeVolumnMusic()
    {
        music_source.mute = Data.Music;
    }


    public virtual void PlaySound(TypeSound type)
    {
        switch (type)
        {
            case TypeSound.CLICK:
                PlayClip(click);
                break;
            case TypeSound.WIN:
                PlayClip(win);
                break;
            case TypeSound.MERGE:
                PlayClip(merge);
                break;
            case TypeSound.INCORRECT:
                PlayClip(incorrect);
                break;
            case TypeSound.SELECTBLOCK:
                PlayClip(selectblock);
                break;
        }
    }



    public virtual void PlayClip(AudioClip clip)
    {
        var source = queue_sources.Dequeue();
        if (source == null)
            return;
        //source.volume = Data.Sound;
        source.PlayOneShot(clip);
        queue_sources.Enqueue(source);
    }

    public virtual void StartMusic()
    {
        music_source.clip = music_sound;
        music_source.Play();
    }

    public void StopMusic()
    {
        music_source.Stop();
    }
}

public enum TypeSound
{
    CLICK,
    WIN, 
    INCORRECT,
    MERGE,
    SELECTBLOCK,
}
