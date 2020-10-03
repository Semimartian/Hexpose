using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum SoundNames:byte
{
    AllGlocken, WallHit, GroundUp,BallSent, LowGlocken,
    WinChord, LoseChord, EnemyDeath, HexPressed, BombActivation, PathInfected
}
[Serializable]
public class CorrectedSoundClip
{
    public AudioClip clip;
    [Range(0, 1)] public float volume;
    public bool ignoreMe;


}
[Serializable]
public class Sound
{
    public SoundNames name;
    public CorrectedSoundClip[] audioClips;
    public float minPitch;
    public float maxPitch;
    public float volumeModifier;

}

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;
    private void Awake()
    {
        instance = this;
    }
    [SerializeField] private Sound[] sounds;
    [SerializeField] private AudioSource oneShotSoundPreFab;
    private static List<AudioSource> availableAudioSources;
    private static List<AudioSource> occupiedAudioSources;

    private void Start()
    {
        int sourcesCount = 32;
        availableAudioSources = new List<AudioSource>(sourcesCount);
        for (int i = 0; i < sourcesCount; i++)
        {
            availableAudioSources.Add(Instantiate(oneShotSoundPreFab));
        }
        occupiedAudioSources = new List<AudioSource>();
    }

    public static void PlayOneShotSoundAt(SoundNames name, Vector3 position, float delay = 0)
    {
        instance.StartCoroutine(instance.PlayOneShotSoundAtCoroutine(name,position,delay));     
    }

    public static void PlayOneShotSoundAt(SoundNames name,AudioSource audioSource, float delay = 0)
    {
        instance.StartCoroutine(instance.PlayOneShotSoundAtCoroutine(name, audioSource, delay));
    }

    private IEnumerator PlayOneShotSoundAtCoroutine(SoundNames name, Vector3 position, float delay)
    {
        if (delay > 0)
        {
            yield return new WaitForSeconds(delay);
        }
        CorrectedSoundClip clip = null;
        float minPitch = 0;
        float maxPitch = 0;
        float volumeModification=0;
        for (int i = 0; i < sounds.Length; i++)
        {
            Sound sound = sounds[i];
            if (sounds[i].name == name)
            {
                int clipIndex = 0;
                for (int j = 0; j < 8; j++)
                {
                    clipIndex = UnityEngine.Random.Range(0, sound.audioClips.Length);
                    if (!sound.audioClips[clipIndex].ignoreMe)
                    {
                        break;
                    }
                }
                clip = sound.audioClips[clipIndex];
                minPitch = sound.minPitch; maxPitch = sound.maxPitch;
                volumeModification = UnityEngine.Random.Range(0, sound.volumeModifier);
            }
        }
        if (clip == null)
        {
            Debug.LogError("NO SOUND FOUND: "+ name.ToString());
        }
        else
        {
            AudioSource oneShotSound = GetAvailableAudioSource();
            oneShotSound.transform.position = position;
            oneShotSound.clip = clip.clip;
            oneShotSound.pitch = UnityEngine.Random.Range(minPitch, maxPitch);
            oneShotSound.volume = clip.volume + volumeModification;
            oneShotSound.Play();
        }
    }

    private IEnumerator PlayOneShotSoundAtCoroutine(SoundNames name, AudioSource audioSource, float delay)
    {
        if (delay > 0)
        {
            yield return new WaitForSeconds(delay);
        }
        CorrectedSoundClip clip = null;
        float minPitch = 0;
        float maxPitch = 0;
        for (int i = 0; i < sounds.Length; i++)
        {
            Sound sound = sounds[i];
            if (sounds[i].name == name)
            {
                clip = sound.audioClips[UnityEngine.Random.Range(0, sound.audioClips.Length)];
                minPitch = sound.minPitch; maxPitch = sound.maxPitch;
            }
        }
        if (clip == null)
        {
            Debug.LogError("NO SOUND FOUND!");
        }
        else
        {
            audioSource.clip = clip.clip;
            audioSource.pitch = UnityEngine.Random.Range(minPitch, maxPitch);
            audioSource.volume = clip.volume;
            audioSource.Play();
        }
    }

    private static AudioSource GetAvailableAudioSource()
    {
        if (availableAudioSources.Count <= 0)
        {
            Debug.LogWarning("No avialable Sound Sources! Creating more.");
            int sourcesCount = 32;
            for (int i = 0; i < sourcesCount; i++)
            {
                availableAudioSources.Add(Instantiate(instance.oneShotSoundPreFab));
            }
        }
        AudioSource source = availableAudioSources[0];
        availableAudioSources.RemoveAt(0);
        occupiedAudioSources.Add(source);
        return source;
    }

    private void Update()
    {
        for (int i = occupiedAudioSources.Count - 1; i >-1; i--)
        {
            AudioSource source = occupiedAudioSources[i];
            if (!source.isPlaying)
            {
                availableAudioSources.Add(source);
                occupiedAudioSources.RemoveAt(i);
            }

        }
    }

}
