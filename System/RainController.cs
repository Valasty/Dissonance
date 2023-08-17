//a chuva da INTRO 1 é um gameobject exclusivo, não utiliza esse

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RainController : MonoBehaviour{

    public GeneralFunctions general;
    public GameObject rainObject;
    public ParticleSystem raindropsParticle;
    public ParticleSystem splashesParticle;
    public RawImage thunderImage;
    private IEnumerator rainCoroutine;

    //RAIN SETTINGS
    string rainIntensity;
    public bool sceneAutoThunder = false; //seta o padrão de auto thunder pra quando ele for alterado por outras coisas
    public bool autoThunder = false;
    public float thunderTimer;
    public int thunderCooldown = 10;    

    //AUDIO SETTINGS
    const float normalRainVolume = 0.06f;
    const float stormRainVolume = 0.1f;
    const float thunderExteriorVolume = 0.4f;

    void Start(){

        thunderImage.CrossFadeAlpha(0, 0, false);
        rainCoroutine = general.audioController.VolumeFade("Rain", true, 1); //value setup at the start so that it doesnt give an reference error in the first try
    }

    void Update(){

        if (!autoThunder)
            return;

        thunderTimer += Time.deltaTime;
        if (thunderTimer > thunderCooldown){
            StartCoroutine(TriggerThunder());
            thunderTimer = 0;
            thunderCooldown = Random.Range(10, 15);
        }
    }

    public void SetRainIntensity(string type) {
        
        rainIntensity = type;
        var raindropsEmission = raindropsParticle.emission;
        var splashesEmission = splashesParticle.emission;

        switch (type){
            case "Normal":
                sceneAutoThunder = false;
                autoThunder = false;
                general.audioController.rainSFXSource.volume = normalRainVolume;
                raindropsEmission.rateOverTime = 1000;
                splashesEmission.rateOverTime = 300;
                break;
            case "Storm":
                sceneAutoThunder = true;
                autoThunder = true;
                general.audioController.rainSFXSource.volume = stormRainVolume;
                raindropsEmission.rateOverTime = 2000;
                splashesEmission.rateOverTime = 600;
                break;
        }
    }

    public void StartRain(){

        rainObject.SetActive(true);
        autoThunder = sceneAutoThunder;
        general.audioController.rainSFXSource.Play();
        StartCoroutine(general.audioController.VolumeFade("Rain", true, 1));
    }

    /*public void StopRain(){

        rainObject.SetActive(false);
        sceneAutoThunder = false;
        autoThunder = false;
        general.audioController.rainSFXSource.Stop();
    }*/

    public void ToggleRain(bool isExterior){

        StopCoroutine(rainCoroutine);
        if (isExterior){
            rainObject.SetActive(true);
            general.audioController.thunderSFXSource.volume = thunderExteriorVolume;
            rainCoroutine = general.audioController.VolumeFade("Rain", true, 1);
        }
        else{
            rainObject.SetActive(false);
            general.audioController.thunderSFXSource.volume = thunderExteriorVolume / 2;
            rainCoroutine = general.audioController.VolumeFade("Rain", false, 0.5f);
        }
        StartCoroutine(rainCoroutine);
    }

    public IEnumerator TriggerThunder(){

        thunderImage.CrossFadeAlpha(0.5f, 0.1f, false);
        yield return new WaitForSeconds(0.1f);
        thunderImage.CrossFadeAlpha(0, 0.1f, false);
        yield return new WaitForSeconds(0.3f);
        thunderImage.CrossFadeAlpha(0.5f, 0.1f, false);
        yield return new WaitForSeconds(0.15f);
        thunderImage.CrossFadeAlpha(0, 0.1f, false);
        if (gameObject.activeInHierarchy)
            general.audioController.PlayThunderAudio();
    }
}
