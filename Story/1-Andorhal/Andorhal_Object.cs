using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Andorhal_Object : MonoBehaviour, IInteractable {

    public bool preserveInteractable; //must be set on inspector for re-interactable objects!

    public IEnumerator Interact(){

        GeneralFunctions general = FindObjectOfType<GeneralFunctions>();

        //////////////////////////////// RE-INTERACTABLE ////////////////////////////////
        switch (name) {
            case "House 1 (blocked)":
                yield return StartCoroutine(general.Dialog("", general.localization.localizationStrings["BlockedDoor-01"]));
                break;
            case "Useless Boat":
                yield return StartCoroutine(general.Dialog("Arya", general.localization.localizationStrings["UselessBoat-01"]));
                yield return StartCoroutine(general.Dialog("Caesar", general.localization.localizationStrings["UselessBoat-02"]));
                yield return StartCoroutine(general.Dialog("Arya", general.localization.localizationStrings["UselessBoat-03"]));
                break;
            case "Dying Character":
                yield return StartCoroutine(general.Dialog("Man", general.localization.localizationStrings["DyingChar-01"]));
                general.DisplayReactionBubble(ReactionBubbleType.Surprised, general.playerController.transform.position);
                yield return new WaitForSeconds(0.5f);
                yield return StartCoroutine(general.Dialog("Arya", general.localization.localizationStrings["DyingChar-02"]));
                yield return StartCoroutine(general.Dialog("Man", general.localization.localizationStrings["DyingChar-03"]));
                yield return StartCoroutine(general.ShakeObject(transform));
                yield return new WaitForSeconds(1);
                yield return StartCoroutine(general.Dialog("Man", general.localization.localizationStrings["DyingChar-04"]));
                yield return new WaitForSeconds(1);
                yield return StartCoroutine(general.Dialog("Arya", general.localization.localizationStrings["DyingChar-05"]));
                yield return new WaitForSeconds(1);
                yield return StartCoroutine(general.Dialog("Caesar", general.localization.localizationStrings["DyingChar-06"]));
                yield return new WaitForSeconds(1);
                yield return StartCoroutine(general.Dialog("Arya", general.localization.localizationStrings["DyingChar-07"]));
                yield return StartCoroutine(general.Dialog("Caesar", general.localization.localizationStrings["DyingChar-08"]));
                yield return new WaitForSeconds(1);
                yield return StartCoroutine(general.Dialog("Arya", general.localization.localizationStrings["DyingChar-09"]));
                yield return new WaitForSeconds(1);
                general.switchs.andorhal_characterDied = true;
                break;

            //////////////////////////////// ONE TIME INTERACTABLE ////////////////////////////////
            ///////////// EVENT CHESTS
            case "Treasure Chest Hammer":
                yield return StartCoroutine(general.AddItemDialog(general.database.Items[(int)ItemType.Key].Find(x => x.Name == "Hammer")));
                general.switchs.andorhal_chestHammerLooted = true;
                break;
            case "Treasure Chest Planks":
                yield return StartCoroutine(general.AddItemDialog(general.database.Items[(int)ItemType.Key].Find(x => x.Name == "Planks")));
                general.switchs.andorhal_chestPlanksLooted = true;
                break;
            case "Treasure Chest Oar":
                yield return StartCoroutine(general.AddItemDialog(general.database.Items[(int)ItemType.Key].Find(x => x.Name == "Oar")));
                general.switchs.andorhal_chestOarLooted = true;
                break;
            case "Grave Monsters":
                if (!general.switchs.andorhal_gravesChecked || general.switchs.andorhal_graveMonstersDefeated)
                    break;
                general.playerController.animator.SetFloat("Velocity", 0);
                general.DisplayReactionBubble(ReactionBubbleType.Surprised, general.playerController.transform.position);
                yield return new WaitForSeconds(0.5f);
                yield return StartCoroutine(general.Dialog("Caesar", general.localization.localizationStrings["ClosedDoor-14"]));
                general.switchs.andorhal_graveMonstersDefeated = true;
                List<Character> encounter = new List<Character> {
                    general.database.Enemies.Find(x => x.Name == "Wind Slime"),
                    general.database.Enemies.Find(x => x.Name == "Water Slime"),
                    general.database.Enemies.Find(x => x.Name == "Earth Slime")
                };
                yield return StartCoroutine(general.LoadBattle(encounter));
                break;
            ///////////// REGULAR CHESTS
            case "Treasure Chest 1":
                yield return StartCoroutine(general.AddItemDialog(general.database.Items[(int)ItemType.Boots].Find(x => x.Name == "Old Boots")));
                general.switchs.andorhal_chest1looted = true;
                break;
            case "Treasure Chest 2":
                yield return StartCoroutine(general.AddItemDialog(general.database.Items[(int)ItemType.Helmet].Find(x => x.Name == "Old Helmet")));
                general.switchs.andorhal_chest2looted = true;
                break;
            case "Treasure Chest 3":
                yield return StartCoroutine(general.AddItemDialog(general.database.Items[(int)ItemType.Weapon].Find(x => x.Name == "Leather Gloves")));
                general.switchs.andorhal_chest3looted = true;
                break;
            case "Treasure Chest 4":
                yield return StartCoroutine(general.AddItemDialog(general.database.Items[(int)ItemType.Armor].Find(x => x.Name == "Old Armor")));
                general.switchs.andorhal_chest4looted = true;
                break;

            ///////////// CORPSES
            case "Corpse 1":
                yield return StartCoroutine(general.Dialog("Caesar", "..."));
                yield return StartCoroutine(general.AddItemDialog(general.database.Items[(int)ItemType.Usable].Find(x => x.Name == "Potion"), react: true));
                general.switchs.andorhal_corpse1looted = true;                
                break;
            case "Corpse 2":
                yield return StartCoroutine(general.Dialog("Caesar", "..."));
                yield return StartCoroutine(general.AddItemDialog(general.database.Items[(int)ItemType.Usable].Find(x => x.Name == "Potion"), react: true));
                general.switchs.andorhal_corpse2looted = true;                
                break;
            case "Corpse 3":
                yield return StartCoroutine(general.Dialog("Caesar", "..."));
                yield return StartCoroutine(general.AddItemDialog(general.database.Items[(int)ItemType.Accessory].Find(x => x.Name == "Magic Ring"), react: true));
                general.switchs.andorhal_corpse3looted = true;                
                break;
            case "Mysterious Corpse":
                yield return StartCoroutine(general.Dialog("Caesar", "..."));
                yield return StartCoroutine(general.AddItemDialog(general.database.Items[(int)ItemType.Key].Find(x => x.Name == "Dark Orb A"), react: true));
                general.switchs.andorhal_mysteriousCorpseLooted = true;
                break;

            ///////////// ENVIRONMENT
            case "Barrel 1":
                yield return StartCoroutine(general.AddItemDialog(general.database.Items[(int)ItemType.Usable].Find(x => x.Name == "Antidote"), react: true));
                general.switchs.andorhal_barrel1looted = true;
                break;
            case "Barrel 2":
                yield return StartCoroutine(general.AddItemDialog(general.database.Items[(int)ItemType.Usable].Find(x => x.Name == "Potion"), react: true));
                general.switchs.andorhal_barrel2looted = true;
                break;
            case "Barrel 3":
                yield return StartCoroutine(general.AddItemDialog(general.database.Items[(int)ItemType.Usable].Find(x => x.Name == "Antidote"), react: true));
                general.switchs.andorhal_barrel3looted = true;
                break;
        }

        if (!preserveInteractable)
            Destroy(transform.GetChild(0).gameObject);

        general.playerController.eventHappening = false;
    }    
}