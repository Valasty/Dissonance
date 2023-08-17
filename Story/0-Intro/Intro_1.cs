using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Intro_1 : MonoBehaviour{

    GeneralFunctions general;
    public Animator caesar;
    public GameObject rain;
    public GameObject monsters;
    public Text narrationText;
    public Text gameTitle;

    IEnumerator Start(){
        
        general = FindObjectOfType<GeneralFunctions>();
        general.playerController.spriteRenderer.color = Color.clear;
        gameTitle.CrossFadeAlpha(0, 0, false);
        narrationText.CrossFadeAlpha(0, 0, false);
        general.cutsceneCanvas.SetActive(true);
        Camera.main.transform.localPosition = new Vector3(-1, 32, -10);
        Camera.main.orthographicSize = 3;
        yield return new WaitForSeconds(1);

        general.audioController.mainMusicSource.Play();
        yield return new WaitForSeconds(8.1f);
        StartCoroutine(StartCameraDescent());
        yield return new WaitForSeconds(10);
        yield return StartCoroutine(StartNarrationText(general.localization.localizationStrings["Intro01-01"]));
        yield return StartCoroutine(general.rain.TriggerThunder());
        rain.SetActive(true);
        yield return new WaitForSeconds(2);
        yield return StartCoroutine(StartNarrationText(general.localization.localizationStrings["Intro01-02"]));
        yield return StartCoroutine(StartNarrationText(general.localization.localizationStrings["Intro01-03"]));
        yield return StartCoroutine(StartNarrationText(general.localization.localizationStrings["Intro01-04"], 5));
        yield return StartCoroutine(StartNarrationText(general.localization.localizationStrings["Intro01-05"]));
        yield return StartCoroutine(StartNarrationText(general.localization.localizationStrings["Intro01-06"], 2));
        yield return new WaitForSeconds(4.5f);
        StartCoroutine(general.rain.TriggerThunder());
        monsters.SetActive(true);
        Camera.main.transform.localPosition = new Vector3(Camera.main.transform.localPosition.x, 2, Camera.main.transform.localPosition.z);
        Camera.main.orthographicSize = 1.5f;
        yield return new WaitForSeconds(4);
        narrationText.rectTransform.localPosition = new Vector2(-500, 0);
        yield return StartCoroutine(StartNarrationText(general.localization.localizationStrings["Intro01-07"], 5));
        yield return StartCoroutine(StartNarrationText(general.localization.localizationStrings["Intro01-08"], 5));
        yield return StartCoroutine(StartNarrationText(general.localization.localizationStrings["Intro01-09"], 6));
        StartCoroutine(general.rain.TriggerThunder());
        yield return new WaitForSeconds(0.5f);
        monsters.SetActive(true);
        general.TurnCharacter(caesar, "Up");
        yield return new WaitForSeconds(1.1f);
        narrationText.rectTransform.localPosition = new Vector2(500, 0);
        yield return StartCoroutine(StartNarrationText(general.localization.localizationStrings["Intro01-10"], 2));
        yield return StartCoroutine(StartNarrationText(general.localization.localizationStrings["Intro01-11"], 3));
        yield return new WaitForSeconds(0.3f);
        StartCoroutine(StartCameraZoom());
        yield return new WaitForSeconds(0.7f);
        Vector2 movement = new Vector2 (0, 1);
        StartCoroutine(general.MoveObjectTowards(caesar.transform, new Vector2(caesar.transform.position.x, 5), 3));
        general.transitionCanvas.color = Color.white;
        yield return StartCoroutine(TriggerGameTitleDisplay());
        general.ResetCamera();
        general.audioController.mainMusicSource.Stop();
        yield return StartCoroutine(general.Dialog("Male Voice", general.localization.localizationStrings["Intro01-12"]));
        StartCoroutine(general.LoadScene("Intro 2", 1, 1.5f));
    }

    IEnumerator StartCameraDescent(){

        while (Camera.main.transform.localPosition.y > 0.6f){
            Camera.main.transform.Translate(Vector2.down * 0.5f * Time.deltaTime);
            yield return null;
        }
    }

    IEnumerator StartCameraZoom(){

        float t = 0;
        while (Camera.main.orthographicSize > 1){
            t += Time.deltaTime;
            Camera.main.orthographicSize -= 0.002f * Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator TriggerGameTitleDisplay(){

        general.transitionCanvas.CrossFadeAlpha(1, 0.1f, false);
        yield return new WaitForSeconds(3);
        gameTitle.CrossFadeAlpha(1, 6, false);
        yield return new WaitForSeconds(16);
        gameTitle.CrossFadeAlpha(0, 6, false);
        yield return new WaitForSeconds(12);
    }

    IEnumerator StartNarrationText(string text, float textDuration = 4) {

        narrationText.text = text;
        narrationText.CrossFadeAlpha(1, 2, false);
        yield return new WaitForSeconds(2 + textDuration);
        narrationText.CrossFadeAlpha(0, 2, false);
        yield return new WaitForSeconds(2);
    }
}