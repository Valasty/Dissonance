using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Andorhal_In_Tutorials : MonoBehaviour, IInteractable {

    public IEnumerator Interact(){

        GeneralFunctions general = FindObjectOfType<GeneralFunctions>();
        general.playerController.animator.SetFloat("Velocity", 0);
        general.DisplayReactionBubble(ReactionBubbleType.Surprised, general.playerController.transform.position);
        yield return new WaitForSeconds(0.5f);

        switch (name){
            case "Event - Walls Tutorial":
                yield return StartCoroutine(general.Dialog("Caesar", general.localization.localizationStrings["Wall-01"]));
                yield return StartCoroutine(general.Dialog(general.localization.tutorialTitle, general.localization.localizationStrings["Tutorial-01"]));
                Destroy(gameObject);
                general.playerController.eventHappening = false;
                break;
            case "Event - Monsters":
                yield return StartCoroutine(general.Dialog("Arya", general.localization.localizationStrings["Monsters-01"]));
                yield return StartCoroutine(general.Dialog("Caesar", general.localization.localizationStrings["Monsters-02"]));
                general.switchs.andorhal_tutorialMonstersDefeated = true;
                List<Character> encounter = new List<Character> {
                    general.database.Enemies.Find(x => x.Name == "Water Slime"),
                    general.database.Enemies.Find(x => x.Name == "Earth Slime")
                };
                StartCoroutine(general.LoadBattle(encounter));
                break;
            case "Event - Tools Tutorial":
                yield return StartCoroutine(general.Dialog("Caesar", general.localization.localizationStrings["Boulder-01"]));
                yield return StartCoroutine(general.Dialog(general.localization.tutorialTitle, general.localization.localizationStrings["Tutorial-02"]));
                Destroy(gameObject);
                general.playerController.eventHappening = false;
                break;
        }
    }
}