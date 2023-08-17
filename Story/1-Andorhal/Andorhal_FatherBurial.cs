using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Andorhal_FatherBurial : MonoBehaviour, IInteractable {

    GeneralFunctions general;
    public Andorhal_Load andorhalLoad;

    public GameObject fatherCorpse;
    public GameObject fatherGrave;

    void Start(){

        general = FindObjectOfType<GeneralFunctions>();
        if (general.eventSequence == "Father Burial")
            StartCoroutine(FatherBurial(false));
        else if (general.switchs.andorhal_fatherBuried)
            ChangeObjects();
    }

    void ChangeObjects(){

        fatherCorpse.SetActive(false);
        fatherGrave.SetActive(true);
    }

    public IEnumerator Interact(){

        if (!general.switchs.andorhal_fatherBuried)
            yield return StartCoroutine(FatherBurial(true));
        else{
            yield return StartCoroutine(general.Dialog("Caesar", "..."));
            general.playerController.eventHappening = false;
        }
    }

    public IEnumerator FatherBurial(bool before){


        if (before){
            yield return StartCoroutine(general.Cutscene(true, andorhalLoad, "Father Burial"));
            yield return StartCoroutine(general.Dialog("Caesar", general.localization.localizationStrings["FatherBurial-01-01"]));
            general.TurnCharacter(andorhalLoad.cutsceneArya, "Left");
            yield return StartCoroutine(general.Dialog("Arya", general.localization.localizationStrings["FatherBurial-01-02"]));
            yield return StartCoroutine(general.Dialog("Caesar", general.localization.localizationStrings["FatherBurial-01-03"]));
            general.TurnCharacter(andorhalLoad.cutsceneCaesar, "Left");
            yield return StartCoroutine(general.Dialog("Arya", general.localization.localizationStrings["FatherBurial-01-04"]));
            yield return StartCoroutine(general.Dialog("Caesar", general.localization.localizationStrings["FatherBurial-01-05"]));
            StartCoroutine(general.ShakeObject(andorhalLoad.cutsceneArya.transform));
            yield return StartCoroutine(general.Dialog("Arya", general.localization.localizationStrings["FatherBurial-01-06"]));
            yield return StartCoroutine(general.ShakeObject(andorhalLoad.cutsceneCaesar.transform));
            yield return StartCoroutine(general.Dialog("Caesar", general.localization.localizationStrings["FatherBurial-01-07"]));
            yield return new WaitForSeconds(1);
            general.TurnCharacter(andorhalLoad.cutsceneCaesar, "Right");
            yield return StartCoroutine(general.Dialog("Caesar", general.localization.localizationStrings["FatherBurial-01-08"]));
            yield return StartCoroutine(general.Dialog("Arya", null,
                new List<string> { general.localization.localizationStrings["FatherBurial-02-01"], general.localization.localizationStrings["FatherBurial-02-02"] }));
            if (general.lastDialogSelection == 1){
                yield return StartCoroutine(general.Dialog("Caesar", general.localization.localizationStrings["FatherBurial-02-03"]));
                general.TurnCharacter(andorhalLoad.cutsceneCaesar, "Down");
                yield return StartCoroutine(general.Dialog("Arya", general.localization.localizationStrings["FatherBurial-02-04"]));
                general.TurnCharacter(andorhalLoad.cutsceneArya, "Up");
                yield return new WaitForSeconds(1);
                general.TurnCharacter(andorhalLoad.cutsceneArya, "Right");
                yield return StartCoroutine(general.SceneFadeIn(1));
                general.TurnCharacter(andorhalLoad.cutsceneArya, "Down");
                yield return StartCoroutine(general.SceneFadeOut(1));
                general.TurnCharacter(andorhalLoad.cutsceneArya, "Right");
                yield return new WaitForSeconds(1);
                general.TurnCharacter(andorhalLoad.cutsceneArya, "Up");
                yield return new WaitForSeconds(1);
                general.TurnCharacter(andorhalLoad.cutsceneArya, "Right");
                yield return new WaitForSeconds(1);
                yield return StartCoroutine(general.ShakeObject(andorhalLoad.cutsceneArya.transform));
                yield return StartCoroutine(general.Dialog("Arya", general.localization.localizationStrings["FatherBurial-02-05"]));
                general.TurnCharacter(andorhalLoad.cutsceneCaesar, "Right");
                yield return new WaitForSeconds(1);
                yield return StartCoroutine(general.Dialog("Caesar", general.localization.localizationStrings["FatherBurial-02-06"]));
                yield return StartCoroutine(general.MoveObjectTowards(andorhalLoad.cutsceneArya.transform,
                    new Vector2(andorhalLoad.cutsceneArya.transform.position.x - 1, andorhalLoad.cutsceneArya.transform.position.y), 5));
                general.TurnCharacter(andorhalLoad.cutsceneArya, "Right");
                yield return new WaitForSeconds(0.5f);
                List<Character> encounter = new List<Character> {
                    general.database.Enemies.Find(x => x.Name == "Wind Slime"),
                    general.database.Enemies.Find(x => x.Name == "Water Slime"),
                    general.database.Enemies.Find(x => x.Name == "Earth Slime")
                };
                StartCoroutine(general.LoadBattle(encounter));
            }
            else
                yield return StartCoroutine(general.Cutscene(false, andorhalLoad));            
        }
        else{
            andorhalLoad.cutsceneCaesar = Instantiate(andorhalLoad.caesarPrefab).GetComponent<Animator>();
            andorhalLoad.cutsceneArya = Instantiate(andorhalLoad.aryaPrefab).GetComponent<Animator>();
            andorhalLoad.cutsceneCaesar.transform.position = new Vector2(general.playerController.transform.position.x, general.playerController.transform.position.y);
            andorhalLoad.cutsceneArya.transform.position = new Vector2(general.playerController.transform.position.x + 0.5f, general.playerController.transform.position.y);
            general.TurnCharacter(andorhalLoad.cutsceneCaesar, "Right");
            general.TurnCharacter(andorhalLoad.cutsceneArya, "Right");
            yield return new WaitForSeconds(2);
            general.TurnCharacter(andorhalLoad.cutsceneArya, "Left");
            yield return StartCoroutine(general.Dialog("Arya", general.localization.localizationStrings["FatherBurial-03-01"]));
            yield return StartCoroutine(general.Dialog("Caesar", general.localization.localizationStrings["FatherBurial-03-02"]));
            general.TurnCharacter(andorhalLoad.cutsceneCaesar, "Down");
            StartCoroutine(general.ShakeObject(andorhalLoad.cutsceneArya.transform));
            yield return StartCoroutine(general.Dialog("Arya", general.localization.localizationStrings["FatherBurial-03-03"]));
            general.TurnCharacter(andorhalLoad.cutsceneArya, "Right");
            yield return new WaitForSeconds(1);
            general.TurnCharacter(andorhalLoad.cutsceneArya, "Up");
            yield return StartCoroutine(general.SceneFadeIn(1));
            ChangeObjects();
            general.TurnCharacter(andorhalLoad.cutsceneArya, "Down");
            yield return StartCoroutine(general.SceneFadeOut(1));
            yield return new WaitForSeconds(1);
            general.TurnCharacter(andorhalLoad.cutsceneArya, "Right");
            yield return StartCoroutine(general.Dialog("Caesar", general.localization.localizationStrings["FatherBurial-03-04"]));
            yield return new WaitForSeconds(1);
            general.TurnCharacter(andorhalLoad.cutsceneArya, "Left");
            yield return StartCoroutine(general.Dialog("Arya", general.localization.localizationStrings["FatherBurial-03-05"]));
            general.TurnCharacter(andorhalLoad.cutsceneCaesar, "Right");
            yield return StartCoroutine(general.Dialog("Caesar", general.localization.localizationStrings["FatherBurial-03-06"]));
            yield return StartCoroutine(general.AddItemDialog(general.database.Items[(int)ItemType.Key].Find(x => x.Name == "Father's Memento")));
            yield return StartCoroutine(general.Dialog("Arya", general.localization.localizationStrings["FatherBurial-03-07"]));
            yield return StartCoroutine(general.Dialog("Caesar", general.localization.localizationStrings["FatherBurial-03-08"]));
            yield return StartCoroutine(general.Dialog("Arya", general.localization.localizationStrings["FatherBurial-03-09"]));
            general.TurnCharacter(andorhalLoad.cutsceneCaesar, "Left");
            yield return StartCoroutine(general.Dialog("Caesar", general.localization.localizationStrings["FatherBurial-03-10"]));
            yield return new WaitForSeconds(0.5f);
            yield return StartCoroutine(general.MoveObjectTowards(andorhalLoad.cutsceneCaesar.transform, new Vector2(-1, andorhalLoad.cutsceneCaesar.transform.position.y), 1));
            yield return new WaitForSeconds(0.5f);
            yield return StartCoroutine(general.Dialog("Arya", general.localization.localizationStrings["FatherBurial-03-11"]));
            StartCoroutine(general.MoveObjectTowards(andorhalLoad.cutsceneArya.transform,
                new Vector2(andorhalLoad.cutsceneArya.transform.position.x - 1, andorhalLoad.cutsceneArya.transform.position.y), 0.5f));
            general.switchs.andorhal_fatherBuried = true;
            yield return StartCoroutine(general.Cutscene(false, andorhalLoad));
            //////////////////////////////////////////////////////////////////////////////// REMOVER ALGUNS ITENS!!!!!!!!!!!!
            //////////////////////////////////////////////////////////////////////////////// REMOVER ALGUNS ITENS!!!!!!!!!!!!
            //////////////////////////////////////////////////////////////////////////////// REMOVER ALGUNS ITENS!!!!!!!!!!!!
            //////////////////////////////////////////////////////////////////////////////// REMOVER ALGUNS ITENS!!!!!!!!!!!!
            //////////////////////////////////////////////////////////////////////////////// REMOVER ALGUNS ITENS!!!!!!!!!!!!
            //////////////////////////////////////////////////////////////////////////////// REMOVER ALGUNS ITENS!!!!!!!!!!!!
            //////////////////////////////////////////////////////////////////////////////// REMOVER ALGUNS ITENS!!!!!!!!!!!!
        }
    }
}