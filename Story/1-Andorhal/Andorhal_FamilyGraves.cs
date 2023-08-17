using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Andorhal_FamilyGraves : MonoBehaviour, IInteractable {

    public Andorhal_Load andorhalLoad;
    GeneralFunctions general;
    
    void Awake() {

        general = FindObjectOfType<GeneralFunctions>();
        //general.LoadScenePortraits(scenePortraitsList);        
    }
    
    public IEnumerator Interact(){

        switch (name){
            case "Grave 1":
                yield return StartCoroutine(general.Dialog("Caesar", "Alana..."));
                general.switchs.andorhal_grave1checked = true;
                yield return StartCoroutine(LastGraveCheck());
                break;
            case "Grave 2":
                yield return StartCoroutine(general.Dialog("Caesar", "Irina..."));
                general.switchs.andorhal_grave2checked = true;
                yield return StartCoroutine(LastGraveCheck());
                break;
            case "Grave 3":
                yield return StartCoroutine(general.Dialog("Caesar", "Elisa..."));
                general.switchs.andorhal_grave3checked = true;
                yield return StartCoroutine(LastGraveCheck());
                break;
        }
        general.playerController.eventHappening = false;
    }

    IEnumerator LastGraveCheck(){

        if (!general.switchs.andorhal_gravesChecked && (general.switchs.andorhal_grave1checked && general.switchs.andorhal_grave2checked && general.switchs.andorhal_grave3checked)){
            yield return StartCoroutine(general.Cutscene(true, andorhalLoad, "Family Graves"));
            yield return new WaitForSeconds(2);
            yield return StartCoroutine(general.Dialog("Caesar", general.localization.localizationStrings["Grave-01"]));
            yield return StartCoroutine(general.Dialog("Caesar", general.localization.localizationStrings["Grave-02"]));
            general.TurnCharacter(andorhalLoad.cutsceneArya, "Left");
            yield return StartCoroutine(general.Dialog("Arya", general.localization.localizationStrings["Grave-03"]));
            general.TurnCharacter(andorhalLoad.cutsceneCaesar, "Left");
            yield return StartCoroutine(general.Dialog("Caesar", general.localization.localizationStrings["Grave-04"]));
            StartCoroutine(general.MoveObjectTowards(andorhalLoad.cutsceneCaesar.transform, new Vector2(30.1f, andorhalLoad.cutsceneCaesar.transform.position.y), 0.5f));
            yield return StartCoroutine(general.Dialog("Arya", general.localization.localizationStrings["Grave-05"]));
            yield return new WaitForSeconds(1);
            yield return StartCoroutine(general.Dialog("Caesar", general.localization.localizationStrings["Grave-06"]));
            general.audioController.PlayRewardSound();
            yield return StartCoroutine(general.Dialog("", general.localization.localizationStrings["Grave-07"]));
            yield return StartCoroutine(general.Dialog(general.localization.tutorialTitle, general.localization.localizationStrings["Grave-08"]));

            foreach (Skill skill in general.database.Player.Characters[0].Skills) {
                if (skill.Status == SkillStatus.None)
                    skill.Status = SkillStatus.Learned;
            }

            StartCoroutine(general.MoveObjectTowards(andorhalLoad.cutsceneCaesar.transform,
                new Vector2(andorhalLoad.cutsceneCaesar.transform.position.x - 1.5f, andorhalLoad.cutsceneCaesar.transform.position.y), 0.5f));
            StartCoroutine(general.MoveObjectTowards(andorhalLoad.cutsceneArya.transform, 
                new Vector2(andorhalLoad.cutsceneArya.transform.position.x - 2.2f, andorhalLoad.cutsceneArya.transform.position.y), 0.8f));
            yield return new WaitForSeconds(1);
            general.switchs.andorhal_gravesChecked = true;
            yield return StartCoroutine(general.Cutscene(false, andorhalLoad));
        }
    }    
}