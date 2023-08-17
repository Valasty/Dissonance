using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Andorhal_ClosedDoor : MonoBehaviour, IInteractable {

    public Andorhal_Load andorhalLoad;
    GeneralFunctions general;

    IEnumerator Start(){

        general = FindObjectOfType<GeneralFunctions>();
        if (general.eventSequence == "Closed Door"){
            general.switchs.andorhal_lockedDoorChecked = true;
            yield return StartCoroutine(general.Cutscene(false, andorhalLoad));
        }
    }

    public IEnumerator Interact(){

        if (general.switchs.andorhal_lockedDoorChecked){
            yield return StartCoroutine(general.Dialog("", general.localization.localizationStrings["BlockedDoor-01"]));
            general.playerController.eventHappening = false;
        }
        else{
            yield return StartCoroutine(general.Cutscene(true, andorhalLoad, "Closed Door"));
            yield return StartCoroutine(general.Dialog("", general.localization.localizationStrings["ClosedDoor-01"]));
            StartCoroutine(general.MoveObjectTowards(andorhalLoad.cutsceneCaesar.transform, new Vector2(andorhalLoad.cutsceneCaesar.transform.position.x, -7.2f), 2, false));
            yield return StartCoroutine(general.MoveObjectTowards(andorhalLoad.cutsceneArya.transform, new Vector2(-8.45f, andorhalLoad.cutsceneArya.transform.position.y), 2));
            general.TurnCharacter(andorhalLoad.cutsceneArya, "Up");
            yield return StartCoroutine(general.Dialog("Arya", general.localization.localizationStrings["ClosedDoor-02"]));
            general.DisplayReactionBubble(ReactionBubbleType.Surprised, new Vector2(-8.45f, -6.2f));
            yield return new WaitForSeconds(1);
            yield return StartCoroutine(general.Dialog("Ms. White", general.localization.localizationStrings["ClosedDoor-03"]));
            yield return StartCoroutine(general.Dialog("Mr. White", general.localization.localizationStrings["ClosedDoor-04"]));            
            yield return StartCoroutine(general.Dialog("Arya", general.localization.localizationStrings["ClosedDoor-05"]));
            yield return new WaitForSeconds(1);
            yield return StartCoroutine(general.Dialog("Mr. White", general.localization.localizationStrings["ClosedDoor-06"]));
            yield return StartCoroutine(general.Dialog("Arya", general.localization.localizationStrings["ClosedDoor-07"]));
            yield return new WaitForSeconds(1.5f);
            yield return StartCoroutine(general.Dialog("Ms. White", general.localization.localizationStrings["ClosedDoor-08"]));
            yield return StartCoroutine(general.Dialog("Mr. White", general.localization.localizationStrings["ClosedDoor-09"]));
            yield return new WaitForSeconds(1);
            yield return StartCoroutine(general.Dialog("Caesar", general.localization.localizationStrings["ClosedDoor-10"]));
            general.TurnCharacter(andorhalLoad.cutsceneArya, "Down");
            yield return StartCoroutine(general.MoveObjectTowards(andorhalLoad.cutsceneCaesar.transform, new Vector2(-8f, andorhalLoad.cutsceneCaesar.transform.position.y), 0.5f));
            yield return StartCoroutine(general.MoveObjectTowards(andorhalLoad.cutsceneArya.transform, new Vector2(andorhalLoad.cutsceneArya.transform.position.x, -7.2f), 2));
            general.TurnCharacter(andorhalLoad.cutsceneArya, "Right");
            yield return StartCoroutine(general.Dialog("Arya", general.localization.localizationStrings["ClosedDoor-11"]));
            yield return new WaitForSeconds(1);
            yield return StartCoroutine(general.Dialog("Caesar", general.localization.localizationStrings["ClosedDoor-12"]));
            yield return new WaitForSeconds(1);
            yield return StartCoroutine(general.Dialog("Arya", general.localization.localizationStrings["ClosedDoor-13"]));
            general.TurnCharacter(andorhalLoad.cutsceneCaesar, "Down");
            yield return new WaitForSeconds(2);
            yield return StartCoroutine(general.Dialog("Caesar", general.localization.localizationStrings["ClosedDoor-14"]));
            List<Character> encounter = new List<Character> {
                general.database.Enemies.Find(x => x.Name == "Wind Slime"),
                general.database.Enemies.Find(x => x.Name == "Earth Slime")
            };
            StartCoroutine(general.LoadBattle(encounter));
        }
    }
}