using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioController : MonoBehaviour{

    public AudioMixer audioMixer;

    public AudioSource mainMusicSource;
    public AudioSource battleMusicSource;
    public AudioSource rainSFXSource;
    public AudioSource thunderSFXSource;
    //public AudioSource SFX1Source; //general sounds
    public AudioSource SFX2Source; //battle start
    public AudioSource menuSFXSource; //menus navigation sounds

    public AudioClip andorhalMusicClip;
    //public AudioClip aryaTheme;
    public AudioClip normalBattleMusicClip;
    public AudioClip alternateBattleMusicClip;
    public AudioClip battleVictoryClip;
    //public AudioClip bossBattleTheme;
    public AudioClip tenseMusicClip;
    public AudioClip aryaSadMusicClip;

    public AudioClip battleStartSFXClip;
    public AudioClip thunderClipSFX1;
    public AudioClip thunderClipSFX2;

    public AudioClip rewardSFX;
    public AudioClip openMenuSFX;
    public AudioClip navigateSFX;
    public AudioClip confirmSFX;
    public AudioClip cancelSFX;
    public AudioClip invalidSFX;



    public bool enableButtonSound;

    void Update(){

        if (!enableButtonSound)
            return;

        if (Input.GetButtonDown("Horizontal") || Input.GetButtonDown("Vertical"))
            PlayMenuNavigateSound();
        if (Input.GetButtonDown("Confirm"))
            PlayMenuConfirmSound();
        if (Input.GetButtonDown("Cancel"))
            PlayMenuCancelSound();
    }


    public IEnumerator VolumeFade(string exposedParameter, bool increase, float targetVolume, float fadeDuration = 1){

        //exposedParameter = "Rain" or "Music"
        //rain volumes are controlled by RainController

        float currentTime = 0;
        float currentVolume;
        audioMixer.GetFloat(exposedParameter, out currentVolume);
        currentVolume = Mathf.Pow(10, currentVolume / 20);
        targetVolume = Mathf.Clamp(targetVolume, 0.0001f, 1);

        while (currentTime < fadeDuration){
            currentTime += Time.deltaTime;
            float newVolume = Mathf.Lerp(currentVolume, targetVolume, currentTime / fadeDuration);
            audioMixer.SetFloat(exposedParameter, Mathf.Log10(newVolume) * 20);
            yield return null;
        }        
    }

    /*public IEnumerator StartMusic(bool battle = false){

        yield return StartCoroutine(VolumeFade("Music", true, 1));

        if (battle)
            battleMusicSource.Play();
        else
            mainMusicSource.Play();
    }

    public IEnumerator StopMusic(bool battle = false){
        
        yield return StartCoroutine(VolumeFade("Music", false, 0));

        if (battle)
            battleMusicSource.Stop();
        else
            mainMusicSource.Pause();
    }*/

    /*public IEnumerator ChangeMusic(AudioClip newMusic, float delay = 0.1f){

        yield return StartCoroutine(VolumeFade("Music", false, 0));
        mainMusicSource.Stop();
        mainMusicSource.clip = newMusic;
        yield return StartCoroutine(VolumeFade("Music", true, 1, delay));
        mainMusicSource.Play();      
    }*/

    public void PlayThunderAudio(){

        if (thunderSFXSource.clip == thunderClipSFX1)
            thunderSFXSource.clip = thunderClipSFX2;
        else
            thunderSFXSource.clip = thunderClipSFX1;
        thunderSFXSource.Play();
    }


    //MENU SOUNDS
    public void PlayRewardSound(){

        menuSFXSource.clip = rewardSFX;
        menuSFXSource.Play();
    }

    public void PlayMenuOpenSound(){
        
        menuSFXSource.clip = openMenuSFX;
        menuSFXSource.Play();
    }

    public void PlayInvalidNavigateSound(){

        menuSFXSource.clip = invalidSFX;
        menuSFXSource.Play();
    }

    void PlayMenuNavigateSound(){

        menuSFXSource.clip = navigateSFX;
        menuSFXSource.Play();
    }

    public void PlayMenuConfirmSound(){

        menuSFXSource.clip = confirmSFX;
        menuSFXSource.Play();
    }

    void PlayMenuCancelSound(){

        menuSFXSource.clip = cancelSFX;
        menuSFXSource.Play();
    }
}