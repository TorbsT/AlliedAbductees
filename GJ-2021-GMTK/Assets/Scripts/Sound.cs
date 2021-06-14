using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume = 1f;
    [Range(.1f, 3f)]
    public float pitch = 1f;

    public float actualVolume;

    public bool loop;
    public bool muted;

    [HideInInspector] public AudioSource source;

    public void mute()
    {
        muted = true;
        source.volume = 0;
    }
    public void unmute()
    {
        muted = false;
        source.volume = actualVolume;
    }
    public void stop()
    {
        source.Stop();
    }
    public void play()
    {
        source.volume = actualVolume;
        source.Play();
    }
}
