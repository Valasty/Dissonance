using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Andorhal_In_WallsBattle : MonoBehaviour, IInteractable {

    public IEnumerator Interact(){

        GeneralFunctions general = FindObjectOfType<GeneralFunctions>();

        if (general.switchs.andorhal_chestOarLooted){
            yield return StartCoroutine(general.Dialog("Caesar", general.localization.localizationStrings["Monsters-02"]));
            List<Character> encounter = new List<Character> {
                general.database.Enemies.Find(x => x.Name == "Wind Slime"),
                general.database.Enemies.Find(x => x.Name == "Water Slime")
            };
            StartCoroutine(general.LoadBattle(encounter));
        }
        else
            general.playerController.eventHappening = false;
    }
}