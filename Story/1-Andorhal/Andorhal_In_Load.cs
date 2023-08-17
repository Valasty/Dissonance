using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Andorhal_In_Load : MonoBehaviour {

    GeneralFunctions general;
    public GameObject chestPlanks;
    public GameObject tutorialWalls;
    public GameObject chestOar;
    public GameObject tutorialTools;
    public GameObject chest4;
    public GameObject mysteriousCorpse;

    public GameObject monsters;
    
    void Start() {

        general = FindObjectOfType<GeneralFunctions>();
        general.sceneElements = new int[6] { 1, 2, 1, 2, 1, 3 };

        ////////////////// TRANSITION //////////////////
        if (general.previousScene == "Battle")
            general.audioController.mainMusicSource.UnPause();
        else
            general.rain.ToggleRain(false);

        ////////////////// SWITCHS //////////////////
        if (general.switchs.andorhal_chestPlanksLooted) {
            Destroy(chestPlanks);
            Destroy(tutorialTools);
        }
        if (general.switchs.andorhal_chestOarLooted){
            Destroy(chestOar);
            Destroy(tutorialWalls);
        }
        
        if (general.switchs.andorhal_chest4looted)
            Destroy(chest4);
        if (general.switchs.andorhal_mysteriousCorpseLooted)
            Destroy(mysteriousCorpse);
        if (general.switchs.andorhal_tutorialMonstersDefeated)
            Destroy(monsters);            
    }
}