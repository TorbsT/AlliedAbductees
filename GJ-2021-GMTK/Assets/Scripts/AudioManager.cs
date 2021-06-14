using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    public float masterVolume = 1f;

    void Awake()
    {
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }
    void Start()
    {

    }

    public void restart()
    {
        play("MenuMusic");
        stop("BaseMusic");
        stop("FrogMusic");
        stop("KangarooMusic");
        stop("SheepMusic");
        stop("TrenchCoatMusic");
        stop("ChickenMusic");
    }
    public void begin()
    {
        stop("MenuMusic");
        play("BaseMusic");
        play("TrenchCoatMusic");
        play("FrogMusic", true);
        play("KangarooMusic", true);
        play("SheepMusic", true);
        play("ChickenMusic", true);
    }

    public void stop(string name)
    {
        Sound s = getSound(name);
        if (s == null) return;
        s.stop();
    }
    public void unmute(string name)
    {
        Sound s = getSound(name);
        if (s == null) return;
        s.unmute();
    }
    public void mute(string name)
    {
        Sound s = getSound(name);
        if (s == null) return;
        s.mute();
    }

    public void play(string name, bool muted = false, float volume = 1f, float pitch = 1f)
    {
        Sound s = getSound(name);
        if (s == null) return;
        s.actualVolume = s.volume * volume * masterVolume;
        s.source.pitch = s.pitch * pitch;
        s.play();
        if (muted) s.mute();
    }

    private Sound getSound(string name)
    {
        if (name == "") { Debug.LogWarning("Nonexistent sound called"); return null; }
        Sound s = System.Array.Find(sounds, sound => sound.name == name);
        if (s == null) Debug.LogWarning("No sound of name " + name);
        return s;
    }
}
