using System.Collections;
using UnityEngine;

public class Andorhal_Load : MonoBehaviour, ISceneManager {

    GeneralFunctions general;    
    public GameObject caesarPrefab;
    public GameObject aryaPrefab;
    public Animator cutsceneCaesar;
    public Animator cutsceneArya;

    //objects
    public GameObject chestHammer;
    public GameObject chest1;
    public GameObject chest2;
    public GameObject chest3;
    public GameObject barrelItem1;
    public GameObject barrelItem2;
    public GameObject barrelItem3;
    public GameObject corpse1;
    public GameObject corpse2;
    public GameObject corpse3;
    public GameObject dyingCharacter;

    void Start() {

        general = FindObjectOfType<GeneralFunctions>();                     

        ////////////////// SWITCHS //////////////////
        if (general.switchs.andorhal_chestHammerLooted)
            Destroy(chestHammer);
        if (general.switchs.andorhal_chest1looted)
            Destroy(chest1);
        if (general.switchs.andorhal_chest2looted)
            Destroy(chest2);
        if (general.switchs.andorhal_chest3looted)
            Destroy(chest3);
        if (general.switchs.andorhal_barrel1looted)
            Destroy(barrelItem1);
        if (general.switchs.andorhal_barrel2looted)
            Destroy(barrelItem2);
        if (general.switchs.andorhal_barrel3looted)
            Destroy(barrelItem3);
        if (general.switchs.andorhal_corpse1looted)
            Destroy(corpse1);
        if (general.switchs.andorhal_corpse2looted)
            Destroy(corpse2);
        if (general.switchs.andorhal_corpse3looted)
            Destroy(corpse3);
        if (general.switchs.andorhal_characterDied)
            Destroy(dyingCharacter);

        ////////////////// TRANSITION //////////////////
        switch (general.previousScene){
            case "Main Menu":
                general.playerController.transform.position = new Vector2(-0.3f, 0);
                general.audioController.mainMusicSource.clip = general.audioController.andorhalMusicClip;
                general.audioController.mainMusicSource.Play();
                general.rain.StartRain();
                break;
            case "Intro 2":
                general.playerController.transform.position = new Vector2(-0.3f, 0);
                general.audioController.mainMusicSource.clip = general.audioController.andorhalMusicClip;
                general.audioController.mainMusicSource.Play();
                StartCoroutine(Tutorial());
                break;
            case "Battle":
                general.audioController.mainMusicSource.UnPause();
                break;
            case "Andorhal_In":
                general.rain.ToggleRain(true);
                break;
        }

        general.sceneElements = new int[6] { 0, 3, 2, 1, 2, 2 };



        /*System.Collections.Generic.List<Character> encounter = new System.Collections.Generic.List<Character> {
            //general.database.Enemies.Find(x => x.Name == "Water Lizard")
            general.database.Enemies.Find(x => x.Name == "Water Slime"),
            general.database.Enemies.Find(x => x.Name == "Earth Slime"),
            general.database.Enemies.Find(x => x.Name == "Wind Slime")
        };
        StartCoroutine(general.LoadBattle(encounter));*/
        




    }

    IEnumerator Tutorial(){

        yield return new WaitForSeconds(3);
        yield return StartCoroutine(general.Dialog(general.localization.tutorialTitle, general.localization.localizationStrings["Tutorial-01"]));
        yield return new WaitForSeconds(1);
        general.eventSequence = null; //starts on MainMenu
        general.playerController.eventHappening = false;
    }

    public void CutsceneSetup(bool start){
        
        if (start){
            cutsceneCaesar = Instantiate(caesarPrefab).GetComponent<Animator>();
            cutsceneArya = Instantiate(aryaPrefab).GetComponent<Animator>();
            switch (general.eventSequence){
                case "Father Burial":
                    general.playerController.transform.position = new Vector2(-0.3f, 0.2f);
                    break;
                case "Closed Door":
                    general.playerController.transform.position = new Vector2(-8.45f, -6.5f);
                    general.TurnCharacter(cutsceneCaesar, "Up");
                    general.TurnCharacter(cutsceneArya, "Left");
                    break;
                case "Family Graves":
                    general.playerController.transform.position = new Vector2(31.1f, 17.5f);
                    break;
                case "Boat Fix":
                    general.playerController.transform.position = new Vector2(-14.8f, -7.2f);
                    general.TurnCharacter(cutsceneCaesar, "Left");
                    general.TurnCharacter(cutsceneArya, "Left");
                    break;
            }
            cutsceneCaesar.transform.position = new Vector2(general.playerController.transform.position.x, general.playerController.transform.position.y);
            cutsceneArya.transform.position = new Vector2(general.playerController.transform.position.x + 0.5f, general.playerController.transform.position.y);
        }
        else{
            if (cutsceneCaesar != null)
                Destroy(cutsceneCaesar.gameObject);
            if (cutsceneArya != null)
                Destroy(cutsceneArya.gameObject);
        }
    }
}