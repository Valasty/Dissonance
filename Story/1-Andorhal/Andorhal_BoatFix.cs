using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Andorhal_BoatFix : MonoBehaviour, IInteractable {

    GeneralFunctions general;
    public Andorhal_Load andorhalLoad;    
    public Transform monsters1;
    public Transform monsters2;
    
    public IEnumerator Interact(){

        general = FindObjectOfType<GeneralFunctions>();
        yield return StartCoroutine(BoatCheck());
    }

    IEnumerator BoatCheck(){

        yield return StartCoroutine(general.Cutscene(true, andorhalLoad, "Boat Fix"));
        yield return StartCoroutine(general.Dialog("Arya", general.localization.localizationStrings["Boat-01-01"]));
        yield return StartCoroutine(general.Dialog("Caesar", general.localization.localizationStrings["Boat-01-02"]));
        yield return new WaitForSeconds(1);
        yield return StartCoroutine(general.Dialog("Caesar", general.localization.localizationStrings["Boat-01-03"]));
        yield return StartCoroutine(general.Dialog("Arya", general.localization.localizationStrings["Boat-01-04"]));
        yield return StartCoroutine(general.Dialog("Caesar", general.localization.localizationStrings["Boat-01-05"]));
        yield return StartCoroutine(general.Dialog("Arya", general.localization.localizationStrings["Boat-01-06"]));
        yield return new WaitForSeconds(1);
        if (!general.switchs.andorhal_chestHammerLooted || !general.switchs.andorhal_chestPlanksLooted || !general.switchs.andorhal_chestOarLooted){
            yield return StartCoroutine(general.Dialog("Caesar", general.localization.localizationStrings["Boat-02-01"]));
            yield return StartCoroutine(general.Dialog("Arya", general.localization.localizationStrings["Boat-02-02"]));
            yield return StartCoroutine(general.Cutscene(false, andorhalLoad));
        }
        else{
            yield return StartCoroutine(general.Dialog("Caesar", general.localization.localizationStrings["Boat-03-01"], 
                new List<string> {general.localization.localizationStrings["Boat-03-02"], general.localization.localizationStrings["Boat-03-03"]}));
            if (general.lastDialogSelection == 1){
                general.RemoveItem(general.database.Items[(int)ItemType.Key].Find(x => x.Name == "Hammer"));
                general.RemoveItem(general.database.Items[(int)ItemType.Key].Find(x => x.Name == "Planks"));
                general.RemoveItem(general.database.Items[(int)ItemType.Key].Find(x => x.Name == "Oar"));
                yield return StartCoroutine(general.Dialog("Caesar", general.localization.localizationStrings["Boat-03-04"]));
                yield return StartCoroutine(general.Dialog("Arya", general.localization.localizationStrings["Boat-03-05"]));
                general.TurnCharacter(andorhalLoad.cutsceneArya, "Right");
                yield return new WaitForSeconds(1);
                general.TurnCharacter(andorhalLoad.cutsceneArya, "Up");
                yield return new WaitForSeconds(1);
                yield return StartCoroutine(general.SceneFadeIn(1));
                andorhalLoad.cutsceneCaesar.transform.position = new Vector2(-15.5f, -8.3f);
                general.TurnCharacter(andorhalLoad.cutsceneCaesar, "Up");
                general.TurnCharacter(andorhalLoad.cutsceneArya, "Down");
                yield return StartCoroutine(general.SceneFadeOut(1));
                yield return new WaitForSeconds(1);
                general.TurnCharacter(andorhalLoad.cutsceneArya, "Right");
                yield return new WaitForSeconds(1);
                yield return StartCoroutine(general.SceneFadeIn(1));
                andorhalLoad.cutsceneCaesar.transform.position = new Vector2(-15, -7.4f);
                general.TurnCharacter(andorhalLoad.cutsceneCaesar, "Left");
                general.TurnCharacter(andorhalLoad.cutsceneArya, "Up");
                yield return StartCoroutine(general.SceneFadeOut(1));
                yield return new WaitForSeconds(1);
                general.TurnCharacter(andorhalLoad.cutsceneArya, "Down");
                yield return new WaitForSeconds(1);
                yield return StartCoroutine(general.SceneFadeIn(1));
                andorhalLoad.cutsceneCaesar.transform.position = new Vector2(-15.5f, -6.8f);
                general.TurnCharacter(andorhalLoad.cutsceneCaesar, "Down");
                general.TurnCharacter(andorhalLoad.cutsceneArya, "Right");
                yield return StartCoroutine(general.SceneFadeOut(1));
                yield return new WaitForSeconds(1);
                general.TurnCharacter(andorhalLoad.cutsceneArya, "Up");
                yield return new WaitForSeconds(1);
                yield return StartCoroutine(general.Dialog("Caesar", general.localization.localizationStrings["Boat-03-06"]));
                general.TurnCharacter(andorhalLoad.cutsceneArya, "Right");
                StartCoroutine(general.audioController.VolumeFade("Music", false, 0, 3));
                yield return new WaitForSeconds(2);
                yield return general.ShakeObject(andorhalLoad.cutsceneArya.transform);
                general.TurnCharacter(andorhalLoad.cutsceneCaesar, "Right");
                general.audioController.mainMusicSource.Stop();
                general.audioController.mainMusicSource.clip = general.audioController.tenseMusicClip;
                yield return StartCoroutine(general.Dialog("Arya", general.localization.localizationStrings["Boat-03-07"]));
                StartCoroutine(general.audioController.VolumeFade("Music", true, 1, 0.01f));
                general.audioController.mainMusicSource.Play();
                general.transitionCanvas.color = Color.red;                
                general.audioController.SFX2Source.Play();                
                yield return StartCoroutine(general.SceneFadeIn(0.2f));                
                monsters1.gameObject.SetActive(true);
                yield return StartCoroutine(general.SceneFadeOut(0.2f));
                yield return StartCoroutine(general.MoveObjectTowards(monsters1, new Vector2(-12.3f, monsters1.position.y), 0.2f));
                yield return StartCoroutine(general.MoveObjectTowards(andorhalLoad.cutsceneArya.transform, new Vector2(-14.5f, andorhalLoad.cutsceneArya.transform.position.y), 0.2f, false));
                yield return general.ShakeObject(andorhalLoad.cutsceneArya.transform);
                yield return StartCoroutine(general.Dialog("Arya", general.localization.localizationStrings["Boat-03-08"]));
                general.audioController.SFX2Source.Play();
                yield return StartCoroutine(general.SceneFadeIn(0.2f));
                monsters2.gameObject.SetActive(true);
                yield return StartCoroutine(general.SceneFadeOut(0.2f));
                general.transitionCanvas.color = Color.black;
                yield return StartCoroutine(general.MoveObjectTowards(monsters2, new Vector2(-12f, monsters2.position.y), 0.2f));
                yield return StartCoroutine(general.MoveObjectTowards(andorhalLoad.cutsceneCaesar.transform, new Vector2(-14.5f, andorhalLoad.cutsceneCaesar.transform.position.y), 4));
                yield return StartCoroutine(general.Dialog("Caesar", general.localization.localizationStrings["Boat-03-09"]));
                general.TurnCharacter(andorhalLoad.cutsceneArya, "Left");
                yield return StartCoroutine(general.CharacterJump(andorhalLoad.cutsceneArya.transform, Vector2.left));
                yield return new WaitForSeconds(0.2f);
                yield return StartCoroutine(general.MoveObjectTowards(andorhalLoad.cutsceneArya.transform, new Vector2(-16.5f, andorhalLoad.cutsceneArya.transform.position.y), 4));
                andorhalLoad.cutsceneArya.transform.SetParent(transform);
                general.TurnCharacter(andorhalLoad.cutsceneArya, "Right");
                general.TurnCharacter(andorhalLoad.cutsceneCaesar, "Left");
                yield return StartCoroutine(general.MoveObjectTowards(andorhalLoad.cutsceneCaesar.transform, new Vector2(-15, -7.2f), 4));
                yield return new WaitForSeconds(1);
                yield return StartCoroutine(general.Dialog("Arya", general.localization.localizationStrings["Boat-03-10"]));
                yield return new WaitForSeconds(0.5f);
                yield return StartCoroutine(general.MoveObjectTowards(monsters1.transform, new Vector2(-14f, monsters1.position.y), 25));
                StartCoroutine(general.ShakeObject(andorhalLoad.cutsceneCaesar.transform));
                StartCoroutine(general.MoveObjectTowards(monsters1.transform, new Vector2(-12.5f, monsters1.position.y), 4));
                yield return StartCoroutine(general.Dialog("Caesar", general.localization.localizationStrings["Boat-03-11"]));
                yield return StartCoroutine(general.Dialog("Arya", general.localization.localizationStrings["Boat-03-12"]));
                yield return new WaitForSeconds(1);
                yield return StartCoroutine(general.ShakeObject(transform));
                yield return new WaitForSeconds(0.5f);
                yield return StartCoroutine(general.MoveObjectTowards(monsters1.transform, new Vector2(-14f, monsters1.position.y), 25));
                StartCoroutine(general.ShakeObject(andorhalLoad.cutsceneCaesar.transform));
                yield return StartCoroutine(general.MoveObjectTowards(monsters1.transform, new Vector2(-12.5f, monsters1.position.y), 4));
                yield return StartCoroutine(general.Dialog("Caesar", general.localization.localizationStrings["Boat-03-13"]));
                yield return new WaitForSeconds(0.5f);
                yield return StartCoroutine(general.ShakeObject(transform));
                yield return new WaitForSeconds(0.5f);
                StartCoroutine(general.ShakeObject(transform));
                yield return StartCoroutine(general.MoveObjectTowards(transform, new Vector2(-17, transform.position.y), 2));
                StartCoroutine(CaesarJump());
                yield return StartCoroutine(general.MoveObjectTowards(transform, new Vector2(-18, transform.position.y), 1.5f));
                StartCoroutine(general.MoveObjectTowards(transform, new Vector2(-30, transform.position.y), 1));
                StartCoroutine(general.MoveObjectTowards(monsters1.transform, new Vector2(-15f, monsters1.position.y), 5));
                StartCoroutine(general.MoveObjectTowards(monsters2.transform, new Vector2(-14.5f, monsters2.position.y), 5));
                yield return new WaitForSeconds(0.2f);
                general.TurnCharacter(andorhalLoad.cutsceneCaesar, "Right");
                yield return new WaitForSeconds(0.5f);
                yield return StartCoroutine(general.Dialog("Arya", general.localization.localizationStrings["Boat-03-14"]));
                yield return StartCoroutine(general.Dialog("Caesar", general.localization.localizationStrings["Boat-03-15"]));
                yield return StartCoroutine(general.Dialog("Caesar", general.localization.localizationStrings["Boat-03-16"]));
                yield return new WaitForSeconds(1);
                StartCoroutine(general.audioController.VolumeFade("Music", false, 0, 5));
                StartCoroutine(general.LoadScene("Boat Ride", 5, 5, true));
            }
            else
                yield return StartCoroutine(general.Cutscene(false, andorhalLoad));
        }
    }

    IEnumerator CaesarJump(){

        yield return StartCoroutine(general.CharacterJump(andorhalLoad.cutsceneCaesar.transform, Vector2.left));
        andorhalLoad.cutsceneCaesar.transform.SetParent(transform);
    }
}