using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boat_Ride : MonoBehaviour{

    public Animator cutsceneCaesar;
    public Animator cutsceneArya;
    public Text chapterText;
    public GameObject thankYouForPlaying;
    public GameObject healingSkill;

    IEnumerator Start(){

        GeneralFunctions general = FindObjectOfType<GeneralFunctions>();
        general.audioController.mainMusicSource.clip = general.audioController.aryaSadMusicClip;
        StartCoroutine(general.audioController.VolumeFade("Music", true, 1, 0.01f));
        chapterText.CrossFadeAlpha(0, 0, false);
        chapterText.text = general.localization.localizationStrings["ChapterTitle"] + "\n" + general.localization.localizationStrings["ChapterName"];
        general.playerController.transform.position = Vector2.zero;
        general.rain.SetRainIntensity("Normal");
        Camera.main.orthographicSize = 3;
        general.TurnCharacter(cutsceneCaesar, "Right");
        general.TurnCharacter(cutsceneArya, "Right");

        general.audioController.mainMusicSource.Play();
        yield return new WaitForSeconds(8);
        general.TurnCharacter(cutsceneCaesar, "Left");
        yield return StartCoroutine(general.Dialog("Caesar", general.localization.localizationStrings["BoatRide-01"]));
        yield return StartCoroutine(general.Dialog("Arya", general.localization.localizationStrings["BoatRide-02"]));
        yield return StartCoroutine(general.Dialog("Caesar", general.localization.localizationStrings["BoatRide-03"]));
        general.TurnCharacter(cutsceneCaesar, "Right");
        yield return new WaitForSeconds(4);
        yield return StartCoroutine(general.Dialog("Arya", general.localization.localizationStrings["BoatRide-04"]));
        yield return new WaitForSeconds(1);
        yield return StartCoroutine(general.MoveObjectTowards(cutsceneCaesar.transform, new Vector2(0.7f, cutsceneCaesar.transform.position.y), 0.2f));
        yield return StartCoroutine(general.Dialog("Caesar", general.localization.localizationStrings["BoatRide-05"]));
        yield return new WaitForSeconds(2);
        general.TurnCharacter(cutsceneArya, "Left");
        yield return StartCoroutine(general.Dialog("Arya", general.localization.localizationStrings["BoatRide-06"]));
        yield return new WaitForSeconds(2);
        yield return StartCoroutine(general.Dialog("Caesar", general.localization.localizationStrings["BoatRide-07"]));
        yield return new WaitForSeconds(2);
        general.TurnCharacter(cutsceneArya, "Right");
        yield return StartCoroutine(general.Dialog("Arya", general.localization.localizationStrings["BoatRide-08"]));
        yield return StartCoroutine(general.MoveObjectTowards(cutsceneCaesar.transform, new Vector2(cutsceneCaesar.transform.position.x - 0.1f, cutsceneCaesar.transform.position.y), 1));
        yield return StartCoroutine(general.MoveObjectTowards(cutsceneArya.transform, new Vector2(cutsceneArya.transform.position.x - 0.04f, cutsceneArya.transform.position.y), 1, false));
        yield return StartCoroutine(general.Dialog("Caesar", general.localization.localizationStrings["BoatRide-09"]));
        yield return StartCoroutine(general.MoveObjectTowards(cutsceneArya.transform, new Vector2(cutsceneArya.transform.position.x - 0.04f, cutsceneArya.transform.position.y), 1, false));
        yield return StartCoroutine(general.Dialog("Caesar", general.localization.localizationStrings["BoatRide-10"]));
        yield return StartCoroutine(general.MoveObjectTowards(cutsceneArya.transform, new Vector2(cutsceneArya.transform.position.x - 0.04f, cutsceneArya.transform.position.y), 1, false));
        yield return StartCoroutine(general.Dialog("Caesar", general.localization.localizationStrings["BoatRide-11"]));
        StartCoroutine(general.ShakeObject(cutsceneCaesar.transform));
        yield return StartCoroutine(general.Dialog("Caesar", general.localization.localizationStrings["BoatRide-12"])); //ugh
        yield return new WaitForSeconds(0.5f);
        general.TurnCharacter(cutsceneCaesar, "Right");

        general.DisplayReactionBubble(ReactionBubbleType.Surprised, cutsceneArya.transform.position);
        yield return new WaitForSeconds(1);
        StartCoroutine(general.MoveObjectTowards(cutsceneArya.transform, new Vector2(cutsceneArya.transform.position.x + 0.05f, cutsceneArya.transform.position.y), 1));
        yield return StartCoroutine(general.Dialog("Arya", general.localization.localizationStrings["BoatRide-13"]));
        yield return StartCoroutine(general.Dialog("Caesar", general.localization.localizationStrings["BoatRide-14"]));
        StartCoroutine(general.MoveObjectTowards(cutsceneArya.transform, new Vector2(cutsceneArya.transform.position.x + 0.25f, cutsceneArya.transform.position.y), 0.3f));
        yield return StartCoroutine(general.Dialog("Arya", general.localization.localizationStrings["BoatRide-15"]));        
        general.TurnCharacter(cutsceneCaesar, "Left");
        yield return StartCoroutine(general.Dialog("Caesar", general.localization.localizationStrings["BoatRide-16"]));

        yield return StartCoroutine(general.MoveObjectTowards(cutsceneArya.transform, new Vector2(cutsceneArya.transform.position.x - 0.2f, cutsceneArya.transform.position.y), 2));
        yield return new WaitForSeconds(2);
        yield return StartCoroutine(general.Dialog("Arya", general.localization.localizationStrings["BoatRide-17"]));
        yield return new WaitForSeconds(3);
        StartCoroutine(general.MoveObjectTowards(cutsceneCaesar.transform, new Vector2(cutsceneCaesar.transform.position.x - 0.1f, cutsceneCaesar.transform.position.y), 0.3f));
        yield return StartCoroutine(general.Dialog("Caesar", general.localization.localizationStrings["BoatRide-18"]));

        general.TurnCharacter(cutsceneArya, "Right");
        StartCoroutine(general.ShakeObject(cutsceneArya.transform));
        yield return StartCoroutine(general.MoveObjectTowards(cutsceneCaesar.transform, new Vector2(cutsceneCaesar.transform.position.x + 0.1f, cutsceneCaesar.transform.position.y), 1, false));
        general.DisplayReactionBubble(ReactionBubbleType.Surprised, cutsceneCaesar.transform.position);
        yield return StartCoroutine(general.Dialog("Arya", general.localization.localizationStrings["BoatRide-19"]));
        general.TurnCharacter(cutsceneCaesar, "Right");
        yield return new WaitForSeconds(1);
        yield return StartCoroutine(general.Dialog("Arya", general.localization.localizationStrings["BoatRide-20"]));
        general.TurnCharacter(cutsceneArya, "Left");
        yield return new WaitForSeconds(4);
        general.TurnCharacter(cutsceneCaesar, "Left");
        yield return StartCoroutine(general.Dialog("Caesar", general.localization.localizationStrings["BoatRide-21"]));
        yield return new WaitForSeconds(1);
        yield return StartCoroutine(general.MoveObjectTowards(cutsceneArya.transform, new Vector2(cutsceneArya.transform.position.x + 0.2f, cutsceneArya.transform.position.y), 0.5f));
        yield return StartCoroutine(general.Dialog("Arya", general.localization.localizationStrings["BoatRide-22"]));
        yield return StartCoroutine(general.Dialog("Caesar", general.localization.localizationStrings["BoatRide-23"]));
        yield return StartCoroutine(general.Dialog("Arya", general.localization.localizationStrings["BoatRide-24"]));
        yield return new WaitForSeconds(2);
        yield return StartCoroutine(general.Dialog("Caesar", general.localization.localizationStrings["BoatRide-25"]));
        yield return new WaitForSeconds(1);
        yield return StartCoroutine(general.MoveObjectTowards(cutsceneArya.transform, new Vector2(cutsceneArya.transform.position.x + 0.2f, cutsceneArya.transform.position.y), 0.3f));
        yield return new WaitForSeconds(2);
        healingSkill.SetActive(true);
        general.DisplayReactionBubble(ReactionBubbleType.Surprised, cutsceneCaesar.transform.position);
        yield return new WaitForSeconds(3);
        healingSkill.SetActive(false);
        yield return new WaitForSeconds(3);
        yield return StartCoroutine(general.Dialog("Caesar", general.localization.localizationStrings["BoatRide-26"]));
        yield return new WaitForSeconds(1);
        yield return StartCoroutine(general.MoveObjectTowards(cutsceneArya.transform, new Vector2(cutsceneArya.transform.position.x - 0.2f, cutsceneArya.transform.position.y), 1));
        yield return StartCoroutine(general.Dialog("Arya", general.localization.localizationStrings["BoatRide-27"]));
        yield return StartCoroutine(general.Dialog("Caesar", general.localization.localizationStrings["BoatRide-28"]));
        general.TurnCharacter(cutsceneArya, "Right");
        yield return StartCoroutine(general.Dialog("Arya", general.localization.localizationStrings["BoatRide-29"]));
        StartCoroutine(general.audioController.VolumeFade("Music", false, 0, 5));
        yield return new WaitForSeconds(2);

        //DISLAY CHAPTER
        yield return StartCoroutine(general.SceneFadeIn(3));
        yield return new WaitForSeconds(2);
        chapterText.CrossFadeAlpha(1, 5, false);
        yield return new WaitForSeconds(8);
        chapterText.CrossFadeAlpha(0, 5, false);
        yield return new WaitForSeconds(6);
        //DISLAY CHAPTER

        general.switchs.andorhal_escapedIsland = true;
        //StartCoroutine(general.LoadScene("", 1, 3));
        //RESETAR CAMERA???

        thankYouForPlaying.SetActive(true);
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(thankYouForPlaying.transform.GetChild(0).gameObject);
    }

    public void QuitGameButton(){

        print("aaa");
        Application.Quit();
    }
}