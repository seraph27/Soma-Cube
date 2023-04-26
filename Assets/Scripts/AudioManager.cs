using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public AudioClip musicClip; 
    public AudioMixerGroup mixerGroup;
    private AudioSource musicSource;

    void Start()
    {
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.clip = musicClip;
        musicSource.loop = true;
        musicSource.outputAudioMixerGroup = mixerGroup;
        musicSource.Play();
    }
}