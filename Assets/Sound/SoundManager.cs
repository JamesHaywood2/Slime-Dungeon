using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    public AudioSource audioSource;
    public AudioClip[] audioClips;

    public float musicVolume = 1f;
    public float soundVolume=1f;


    private void Start() {
        instance = this;
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(string sound){
        switch (sound){
            case "attack":
                audioSource.PlayOneShot(audioClips[0], soundVolume);
                break;
            case "playerHit":
                audioSource.PlayOneShot(audioClips[1], soundVolume);
                break;
            case "dash":
                audioSource.PlayOneShot(audioClips[2], soundVolume);
                break;
        }
    }

    public void SetVolume(float musicVolume, float soundVolume){
        this.musicVolume = musicVolume;
        audioSource.volume = musicVolume;
        this.soundVolume = soundVolume;
    }


}
