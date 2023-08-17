using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Intro_2 : MonoBehaviour{

    GeneralFunctions general;
    public Animator caesar;
    public Animator arya;
    public Transform father;
    public Transform monster;
    public Text chapterText;
    public List<SpritesList> scenePortraitsList = new List<SpritesList>();
    
    IEnumerator Start(){
                
        general = FindObjectOfType<GeneralFunctions>();
        general.LoadScenePortraits(scenePortraitsList);
        chapterText.CrossFadeAlpha(0, 0, false);
        chapterText.CrossFadeAlpha(0, 0, false);
        chapterText.text = general.localization.localizationStrings["ChapterTitle"] + "\n" + general.localization.localizationStrings["ChapterName"];
        Camera.main.orthographicSize = 3;
        general.TurnCharacter(arya, "Left");

        if (general.previousScene != "Battle"){
            general.rain.SetRainIntensity("Storm");
            general.rain.sceneAutoThunder = false; //turned off because the cutscene itself will control the thunders!!!
            general.rain.StartRain();
            
            yield return new WaitForSeconds(3); //this has to be 1.5 higher than the fade out of intro 1
            general.transitionCanvas.color = Color.black;
            yield return StartCoroutine(general.Dialog("Hurt Father", general.localization.localizationStrings["Intro02-01"]));
            StartCoroutine(general.rain.TriggerThunder());
            yield return StartCoroutine(general.MoveObjectTowards(caesar.transform, new Vector2(3, caesar.transform.position.y), 3));
            general.DisplayReactionBubble(ReactionBubbleType.Surprised, arya.transform.position);
            yield return new WaitForSeconds(0.5f);
            general.TurnCharacter(arya, "Right");
            yield return new WaitForSeconds(1);
            yield return StartCoroutine(general.MoveObjectTowards(arya.transform, new Vector2(2, arya.transform.position.y), 6));
            yield return StartCoroutine(general.Dialog("Little Girl", general.localization.localizationStrings["Intro02-02"]));
            yield return StartCoroutine(general.MoveObjectTowards(arya.transform, new Vector2(2, arya.transform.position.y), 6));
            yield return StartCoroutine(general.MoveObjectTowards(monster, new Vector2(-1, monster.transform.position.y), 12));
            yield return StartCoroutine(general.ShakeObject(father));
            father.eulerAngles = new Vector3(0, 0, -90);
            general.DisplayReactionBubble(ReactionBubbleType.Surprised, arya.transform.position);
            general.TurnCharacter(arya, "Left");
            yield return StartCoroutine(general.Dialog("Hurt Father", "AAAAAARGHHHHH!!!"));
            yield return StartCoroutine(general.MoveObjectTowards(monster, new Vector2(-2, monster.transform.position.y), 3));
            yield return StartCoroutine(general.Dialog("Little Girl", general.localization.localizationStrings["Intro02-03"]));
            yield return StartCoroutine(general.MoveObjectTowards(arya.transform, new Vector2(0.5f, arya.transform.position.y), 3));
            StartCoroutine(general.rain.TriggerThunder());
            yield return StartCoroutine(general.MoveObjectTowards(caesar.transform, new Vector2(-0.5f, caesar.transform.position.y), 8));
            yield return StartCoroutine(general.Dialog("Mysterious Man", general.localization.localizationStrings["Intro02-04"]));
            yield return new WaitForSeconds(0.5f);
            
            general.sceneElements = new int[6] { 0, 3, 2, 1, 2, 2 };
            List<Character> encounter = new List<Character> {
                general.database.Enemies.Find(x => x.Name == "Earth Slime")
            };
            StartCoroutine(general.LoadBattle(encounter));
        }
        else{
            monster.gameObject.SetActive(false);
            father.eulerAngles = new Vector3(0, 0, -90);
            caesar.transform.position = new Vector2(-0.5f, 0);
            general.TurnCharacter(caesar, "Right");

            yield return new WaitForSeconds(3);
            yield return StartCoroutine(general.Dialog("Hurt Father", general.localization.localizationStrings["Intro03-01"]));
            yield return StartCoroutine(general.Dialog("Little Girl", general.localization.localizationStrings["Intro03-02"]));
            yield return StartCoroutine(general.Dialog("Hurt Father", general.localization.localizationStrings["Intro03-03"]));
            yield return StartCoroutine(general.Dialog("Little Girl", general.localization.localizationStrings["Intro03-04"]));
            yield return StartCoroutine(general.Dialog("Hurt Father", general.localization.localizationStrings["Intro03-05"]));
            yield return StartCoroutine(general.Dialog("Caesar", general.localization.localizationStrings["Intro03-06"]));
            StartCoroutine(general.rain.TriggerThunder());
            yield return StartCoroutine(general.Dialog("Hurt Father", general.localization.localizationStrings["Intro03-07"]));
            yield return StartCoroutine(general.Dialog("Caesar", general.localization.localizationStrings["Intro03-08"]));
            StartCoroutine(general.MoveObjectTowards(caesar.transform, new Vector2(-1.3f, caesar.transform.position.y), 1));
            yield return StartCoroutine(general.Dialog("Hurt Father", general.localization.localizationStrings["Intro03-09"]));
            yield return StartCoroutine(general.Dialog("Caesar", general.localization.localizationStrings["Intro03-10"]));
            yield return StartCoroutine(general.ShakeObject(father.transform));
            general.TurnCharacter(caesar, "Right");
            StartCoroutine(general.rain.TriggerThunder());
            yield return StartCoroutine(general.Dialog("Hurt Father", general.localization.localizationStrings["Intro03-11"]));
            yield return StartCoroutine(general.ShakeObject(father.transform));
            yield return new WaitForSeconds(2);
            yield return StartCoroutine(general.Dialog("Arya", general.localization.localizationStrings["Intro03-12"]));
            general.TurnCharacter(caesar, "Left");
            yield return StartCoroutine(general.Dialog("Hurt Father", general.localization.localizationStrings["Intro03-13"]));
            yield return StartCoroutine(general.Dialog("Arya", general.localization.localizationStrings["Intro03-14"]));
            yield return StartCoroutine(general.Dialog("Hurt Father", general.localization.localizationStrings["Intro03-15"]));
            yield return StartCoroutine(general.Dialog("Arya", general.localization.localizationStrings["Intro03-16"]));
            yield return new WaitForSeconds(1);
            yield return StartCoroutine(general.Dialog("Caesar", general.localization.localizationStrings["Intro03-17"]));
            yield return new WaitForSeconds(1);
            StartCoroutine(general.ShakeObject(caesar.transform));
            yield return StartCoroutine(general.Dialog("Caesar", general.localization.localizationStrings["Intro03-18"]));
            yield return new WaitForSeconds(1);
            yield return StartCoroutine(general.MoveObjectTowards(caesar.transform, new Vector2(-1, caesar.transform.position.y), 2));
            yield return StartCoroutine(general.Dialog("Caesar", general.localization.localizationStrings["Intro03-19"]));
            yield return StartCoroutine(general.Dialog("Arya", general.localization.localizationStrings["Intro03-20"]));
            yield return StartCoroutine(general.Dialog("Caesar", general.localization.localizationStrings["Intro03-21"]));
            yield return StartCoroutine(general.Dialog("Caesar", general.localization.localizationStrings["Intro03-22"]));
            yield return StartCoroutine(general.Dialog("Arya", general.localization.localizationStrings["Intro03-23"]));
            yield return StartCoroutine(general.Dialog("Caesar", general.localization.localizationStrings["Intro03-24"]));
            yield return StartCoroutine(general.ShakeObject(caesar.transform));
            yield return new WaitForSeconds(1);
            yield return StartCoroutine(general.Dialog("Caesar", general.localization.localizationStrings["Intro03-25"]));
            yield return StartCoroutine(general.Dialog("Arya", general.localization.localizationStrings["Intro03-26"]));
            StartCoroutine(general.rain.TriggerThunder());
            yield return new WaitForSeconds(1);
            general.TurnCharacter(caesar, "Left");
            yield return StartCoroutine(general.Dialog("Caesar", general.localization.localizationStrings["Intro03-27"]));
            yield return StartCoroutine(general.Dialog("Arya", general.localization.localizationStrings["Intro03-28"]));

            //DISLAY CHAPTER
            yield return StartCoroutine(general.SceneFadeIn(5));
        yield return new WaitForSeconds(1);
            chapterText.CrossFadeAlpha(1, 3, false);
            yield return new WaitForSeconds(8);
            chapterText.CrossFadeAlpha(0, 3, false);
            yield return new WaitForSeconds(4);
            //DISLAY CHAPTER
            
            general.rain.sceneAutoThunder = true;
            general.rain.autoThunder = true;
            general.ResetCamera();
            general.playerController.transform.position = new Vector2(-0.3f, 0);
            general.playerController.spriteRenderer.color = Color.white;
            StartCoroutine(general.LoadScene("Andorhal", fadeOutTime: 3));
        }
    }    
}