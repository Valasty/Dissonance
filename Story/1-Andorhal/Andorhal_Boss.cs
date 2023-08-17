using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Andorhal_Boss : MonoBehaviour, IInteractable {

    GeneralFunctions general;

    void Start(){

        general = FindObjectOfType<GeneralFunctions>();

        if (general.switchs.andorhal_bossDefeated)
            Destroy(gameObject);
    }

    public IEnumerator Interact(){
        
        general.playerController.animator.SetFloat("Velocity", 0);
        general.DisplayReactionBubble(ReactionBubbleType.Surprised, general.playerController.transform.position);
        yield return new WaitForSeconds(0.5f);

        yield return StartCoroutine(general.Dialog("Caesar", general.localization.localizationStrings["Boss-01"],
            new List<string> { general.localization.localizationStrings["Boss-02"], general.localization.localizationStrings["Boss-03"] }));

        //accepts battle
        if (general.lastDialogSelection == 1){
            general.switchs.andorhal_bossDefeated = true;
            List<Character> encounter = new List<Character> {
                general.database.Enemies.Find(x => x.Name == "Wind Slime"),
                general.database.Enemies.Find(x => x.Name == "Water Lizard"),
                general.database.Enemies.Find(x => x.Name == "Earth Slime")
            };
            yield return StartCoroutine(general.LoadBattle(encounter, boss: true));
        }

        //refuses battle
        else{
            yield return StartCoroutine(general.MoveObjectTowards(general.playerController.transform, new Vector2(-15, general.playerController.transform.position.y), 1));
            yield return new WaitForSeconds(0.5f);
            general.playerController.eventHappening = false;
        }
    }
}