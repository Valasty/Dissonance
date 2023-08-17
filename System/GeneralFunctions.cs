using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class GeneralFunctions : MonoBehaviour {

    public PlayerController playerController;
    public Database database;
    public Switchs switchs;
    public LocalizationController localization;
    public RainController rain;
    public AudioController audioController;
    static string savePath;
    public string currentScene;
    public string previousScene;
    public string eventSequence;
    public int[] sceneElements;
    public RawImage transitionCanvas;
    public GameObject cutsceneCanvas;
    public GameObject dialogCanvas;
    public Image dialogPortrait;
    public Text dialogTitle;
    public Text dialogText;
    public Text[] choiceDialog = new Text[3];
    public int lastDialogSelection;
    public GameObject reactionBubblePrefab;
    public Sprite[] reactionBubbles;

    //EACH SCENE NEEDS TO FILL UP THESE VARIABLES
    public Dictionary<string, Sprite> scenePortraitsList = new Dictionary<string, Sprite>();
    public List<Character> sceneEnemies;
    
    ///////////////////////////////////////////////////////////////////////////
    /////// UPDATE READS KEYS TO SKIP DIALOGS (keys have a small delay) ///////
    ///////////////////////////////////////////////////////////////////////////
    void Awake(){

        savePath = Application.dataPath + "/SaveData.dat";
    }
    
    void OnLevelWasLoaded(int level) {

        //LOCALIZATION DOES NOT APPLY TO MAIN MENU OR BATTLE
        if (level > 2)
            localization.SetLocalization(currentScene + "/");
    }

    public static void SaveGame(Switchs switchs, Player Player) {

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = new FileStream(savePath, FileMode.Create);

        List<SaveCharacter> saveCharacters = new List<SaveCharacter>();
        List<SaveItem> saveItems = new List<SaveItem>();

        foreach (Character character in Player.Characters){
            int[] skillsStatus = new int[character.Skills.Count];
            int[] equipmentIds = new int[character.Equipments.Length];
            for (int i = 0; i < character.Skills.Count; i++)
                skillsStatus[i] = (int)character.Skills[i].Status;
            for (int i = 0; i < character.Equipments.Length; i++)
                equipmentIds[i] = character.GetEquipmentId(character.Equipments[i]);            

            saveCharacters.Add(new SaveCharacter(character.Id, character.Level, character.CurrentXP, character.HPCurrent, character.FrontRow, skillsStatus, equipmentIds));
        }

        foreach (List<PlayerItem> itemList in Player.Inventory){
            foreach (PlayerItem item in itemList)
                saveItems.Add(new SaveItem(item.Item.Id, item.Item.Type, item.Quantity));
        }

        GameSave gameSave = new GameSave("1.0", SceneManager.GetActiveScene().name, switchs, saveCharacters, saveItems);
        bf.Serialize(file, gameSave);
        file.Close();
        print("Game saved on path: " + savePath);
    }

    public static GameSave LoadGameFile(){

        if (File.Exists(savePath)){
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = new FileStream(savePath, FileMode.Open);
            GameSave gameSave = bf.Deserialize(file) as GameSave;
            file.Close();
            return gameSave;
        }
        else{
            print("File doesn't exist on path: " + savePath);
            return null;
        }
    }

    public string LoadGame(){

        GameSave gameSave = LoadGameFile();
        switchs = gameSave.Switchs;
        foreach (SaveCharacter saveChar in gameSave.SaveCharacters){
            Character newChar = database.Characters.Find(x => x.Id == saveChar.Id);
            database.Player.Characters.Add(new Character(newChar.Id, newChar.Name, saveChar.FrontRow, newChar.HPMax, saveChar.HPCurrent, newChar.Attack, newChar.MagicATK, newChar.Defense,
                newChar.MagicDEF, newChar.Speed, newChar.Elemental, CurrentXP: saveChar.CurrentXP, Skills: newChar.Skills));
            for (int i = 0; i < database.Player.Characters[database.Player.Characters.Count - 1].Skills.Count; i++)
                database.Player.Characters[database.Player.Characters.Count - 1].Skills[i].Status = (SkillStatus)saveChar.SkillsStatus[i];
            for (int i = 0; i < saveChar.Equipments.Length; i++){
                int adjustedI = (i == 6) ? 5 : i;  //necessary because of the two accessories!!!
                database.Player.Characters[database.Player.Characters.Count - 1].Equipments[i] = (saveChar.Equipments[i] == 9999) ? null : database.Items[adjustedI].Find(x => x.Id == saveChar.Equipments[i]);
            }
            CheckLevelUp(database.Player.Characters[database.Player.Characters.Count - 1], targetLevel: saveChar.Level);
        }
        foreach (SaveItem saveItem in gameSave.SaveInventory)
            database.Player.Inventory[(int)saveItem.Type].Add(new PlayerItem(database.Items[(int)saveItem.Type].Find(x => x.Id == saveItem.Id), saveItem.Quantity));
        return gameSave.Scene;
    }

    public void LoadScenePortraits(List<SpritesList> scenePortraits){

        scenePortraitsList.Clear();
        foreach (SpritesList scenePortrait in scenePortraits)
            scenePortraitsList.Add(scenePortrait.Name, scenePortrait.Sprite);        
    }
    
    public IEnumerator SceneFadeIn(float fadeTime){

        //rain.autoThunder = false;
        transitionCanvas.CrossFadeAlpha(1, fadeTime, false);
        yield return new WaitForSeconds(fadeTime);        
    }

    public IEnumerator SceneFadeOut(float fadeTime){ //////////////////////// needs to be called by EVERY SCENE to exit scene fade

        //rain.autoThunder = rain.sceneAutoThunder;
        transitionCanvas.CrossFadeAlpha(0, fadeTime, false);
        yield return new WaitForSeconds(fadeTime);
        if (eventSequence == null)
            playerController.eventHappening = false;
    }

    public IEnumerator LoadScene(string nextSceneName, float fadeInTime = 0.5f, float fadeOutTime = 0.5f, bool delayedFade = false){
                
        playerController.eventHappening = true;
        playerController.animator.SetFloat("Velocity", 0);
        
        yield return StartCoroutine(SceneFadeIn(fadeInTime));
        previousScene = SceneManager.GetActiveScene().name;
        currentScene = nextSceneName;
        if (delayedFade)
            yield return new WaitForSeconds(4);
        SceneManager.LoadScene(nextSceneName);
        yield return StartCoroutine(SceneFadeOut(fadeOutTime));
    }

    public IEnumerator LoadBattle(List<Character> enemyList = null, bool enter = true, bool boss = false){

        if (enter){
            playerController.eventHappening = true;
            rain.thunderTimer = 0;
            rain.autoThunder = false;
            playerController.animator.SetFloat("Velocity", 0);
            audioController.mainMusicSource.Pause();
            audioController.SFX2Source.Play();
            audioController.battleMusicSource.clip = boss ? audioController.alternateBattleMusicClip : audioController.normalBattleMusicClip;
            sceneEnemies = enemyList;
            transitionCanvas.color = Color.red;
        }
        else 
            transitionCanvas.color = Color.black;
            
        yield return StartCoroutine(SceneFadeIn(1));
        yield return new WaitForSeconds(0.5f);

        if (enter){
            cutsceneCanvas.SetActive(false);
            playerController.spriteRenderer.color = Color.clear;
            Camera.main.transform.parent = null;
            Camera.main.transform.localPosition = new Vector3(0, 0, -10);
            SceneManager.LoadScene("Battle");

            //manual fadeout added to avoid setting eventhappening false on battle start
            transitionCanvas.CrossFadeAlpha(0, 1, false);
            yield return new WaitForSeconds(1);
        }
        else{
            previousScene = "Battle";
            ResetCamera();
            rain.autoThunder = rain.sceneAutoThunder;
            if (eventSequence == null)
                playerController.spriteRenderer.color = Color.white;
            else
                cutsceneCanvas.SetActive(true);
            SceneManager.LoadScene(currentScene); //////////////////// move uns golinho pra baixo... mandar de volta pra cima?
            yield return StartCoroutine(SceneFadeOut(1));
        }                
    }

    public IEnumerator Cutscene(bool start, ISceneManager sceneManager, string cutscene = null){

        if (start){
            yield return StartCoroutine(SceneFadeIn(0.5f));
            yield return new WaitForSeconds(0.2f);
            eventSequence = cutscene;
            sceneManager.CutsceneSetup(true);
            TurnCharacter(playerController.animator, "Down");
            playerController.spriteRenderer.color = Color.clear;
            cutsceneCanvas.SetActive(true);
            yield return StartCoroutine(SceneFadeOut(0.5f));
        }
        else{
            yield return StartCoroutine(SceneFadeIn(2));
            yield return new WaitForSeconds(0.2f);
            eventSequence = null;
            playerController.spriteRenderer.color = Color.white;
            cutsceneCanvas.SetActive(false);
            sceneManager.CutsceneSetup(false);
            yield return StartCoroutine(SceneFadeOut(2));
        }
    }

    /////////////////////////////////////////////
    /////// CALLED BY EACH LINE OF DIALOG ///////
    /////////////////////////////////////////////
    public IEnumerator Dialog(string title, string text, List<string> dialogChoices = null){

        playerController.animator.SetFloat("Velocity", 0);
        
        //item found -> main portrait -> scene portrait -> unknown
        if (title == localization.itemFoundTitle)
            dialogPortrait.sprite = database.Portraits.Find(x => x.Name == "Treasure").Sprite;        
        else if (database.Portraits.Exists(x => x.Name == title))
            dialogPortrait.sprite = database.Portraits.Find(x => x.Name == title).Sprite;
        else if (scenePortraitsList.ContainsKey(title))
            dialogPortrait.sprite = scenePortraitsList[title];
        else
            dialogPortrait.sprite = database.Portraits.Find(x => x.Name == "Unkown").Sprite;

        if (localization.localizationStrings.ContainsKey(title))
            title = localization.localizationStrings[title];

        dialogTitle.text = title;
        dialogText.text = text;
        dialogCanvas.SetActive(true);

        if (title == localization.itemFoundTitle || title == localization.tutorialTitle)
            yield return new WaitForSeconds(1); //small delay for item found
        else
            yield return new WaitForSeconds(0.2f); //makes so that button press wont be doubled
        if (dialogChoices == null){
            while (!Input.GetButtonDown("Confirm") && !Input.GetButtonDown("Cancel"))
                yield return null;
        }
        else{
            for (int i = 0; i < dialogChoices.Count; i++){
                choiceDialog[i].transform.parent.gameObject.SetActive(true);
                choiceDialog[i].text = dialogChoices[i];
            }
            EventSystem.current.SetSelectedGameObject(choiceDialog[0].gameObject.transform.parent.gameObject);

            while (!Input.GetButtonDown("Confirm"))
                yield return null;

            for (int i = 0; i < dialogChoices.Count; i++)
                choiceDialog[i].transform.parent.gameObject.SetActive(false);            
        }
        audioController.PlayMenuConfirmSound();
        yield return new WaitForSeconds(0.2f);

        dialogCanvas.SetActive(false);
    }

    public void ChosenDialogOption(int choice){

        lastDialogSelection = choice;
    }

    public void DisplayReactionBubble(ReactionBubbleType reaction, Vector2 position){

        GameObject reactionBubble = Instantiate(reactionBubblePrefab);
        reactionBubble.transform.position = new Vector2(position.x + 0.15f, position.y + 0.7f);
        switch (reaction){
            case ReactionBubbleType.Surprised:
                reactionBubble.GetComponent<SpriteRenderer>().sprite = reactionBubbles[0];
                break;
        }
    }

    public IEnumerator AddItemDialog(Item item, int quantity = 1, bool react = false){

        if (react)
            yield return StartCoroutine(Dialog("Caesar", localization.itemFoundText1));
        audioController.PlayRewardSound();
        yield return StartCoroutine(Dialog(localization.itemFoundTitle, localization.itemFoundText2 + "'" + item.LocalizedName + "'!"));
        AddItem(item, quantity);
    }

    public void AddItem(Item item, int quantity = 1){
        
        PlayerItem existingItem = database.Player.Inventory[(int)item.Type].Find(x => x.Item.Id == item.Id);

        if (existingItem == null)
            database.Player.Inventory[(int)item.Type].Add(new PlayerItem(item, quantity));
        else
            existingItem.Quantity += quantity;

        database.Player.Inventory[(int)item.Type].Sort((x, y) => x.Item.Id.CompareTo(y.Item.Id));
    }

    public void RemoveItem(Item item, int quantity = 1){

        PlayerItem existingItem = database.Player.Inventory[(int)item.Type].Find(x => x.Item.Id == item.Id);

        if (existingItem.Quantity == quantity)
            database.Player.Inventory[(int)item.Type].Remove(existingItem);
        else
            existingItem.Quantity -= quantity;
    }

    public void CheckLevelUp(Character character, int experience = 0, int targetLevel = 0){

        Character characterModifiers = database.Characters.Find(x => x.Id == character.Id);

        if (experience == 0) //levels up with direct level
            LevelUpCharacter(character, characterModifiers, targetLevel);
        else{ //levels up with experience
            character.CurrentXP += experience;
            while (character.CurrentXP >= character.NextLevelXP){
                character.CurrentXP -= character.NextLevelXP;
                LevelUpCharacter(character, characterModifiers, character.Level + 1);
            }
        }
    }

    void LevelUpCharacter(Character character, Character characterModifiers, int targetLevel){

        character.Level = targetLevel;
        character.NextLevelXP = 5 + (character.Level * 10);
        character.HPMax = characterModifiers.HPMax + (int)(character.Level * (characterModifiers.HPMax / 20f));
        character.Attack = characterModifiers.Attack + (int)(character.Level * (characterModifiers.Attack / 20f));
        character.MagicATK = characterModifiers.MagicATK + (int)(character.Level * (characterModifiers.MagicATK / 20f));
        character.Defense = characterModifiers.Defense + (int)(character.Level * (characterModifiers.Defense / 20f));
        character.MagicDEF = characterModifiers.MagicDEF + (int)(character.Level * (characterModifiers.MagicDEF / 20f));
        character.Speed = characterModifiers.Speed + (int)(character.Level * (characterModifiers.Speed / 20f));
    }

    public IEnumerator ShakeObject(Transform Object){

        Vector2 startPosition = Object.position;
        float t = 0;
        while (t < 0.4f){
            t += Time.deltaTime;
            Object.position = new Vector2(startPosition.x + Mathf.Sin(Time.time * 100) * 0.01f, Object.position.y);
            yield return null;
        }
        Object.position = startPosition;
    }

    public IEnumerator MoveObjectTowards(Transform character, Vector2 targetPosition, float speed, bool lookAt = true){

        Animator animator = character.GetComponent<Animator>();
        if (animator != null){
            if (lookAt){
                Vector2 normalized = (targetPosition - (Vector2)character.transform.position).normalized;
                if (Mathf.Abs(normalized.x) > Mathf.Abs(normalized.y)){
                    animator.SetFloat("Horizontal Direction", normalized.x);
                    animator.SetFloat("Vertical Direction", 0);
                }
                else{
                    animator.SetFloat("Horizontal Direction", 0);
                    animator.SetFloat("Vertical Direction", normalized.y);
                }
            }
            animator.SetFloat("Velocity", 1);
        }

        while ((Vector2)character.transform.position != targetPosition) {
            character.transform.position = Vector2.MoveTowards(character.transform.position, targetPosition, Time.deltaTime * speed);
            yield return null;
        }

        if (animator != null)        
            animator.SetFloat("Velocity", 0);
    }

    public IEnumerator CharacterJump(Transform character, Vector2 direction) {

        float speed = 6;
        float heightModifier = 0;
        Vector3 targetPosition = (Vector2)character.transform.position + direction;
        Vector3[] P = new Vector3[6];
        P[0] = Vector3.Lerp(character.transform.position, targetPosition, 0);
        P[1] = Vector3.Lerp(character.transform.position, targetPosition, 0.2f);
        P[2] = Vector3.Lerp(character.transform.position, targetPosition, 0.4f);
        P[3] = Vector3.Lerp(character.transform.position, targetPosition, 0.6f);
        P[4] = Vector3.Lerp(character.transform.position, targetPosition, 0.8f);
        P[5] = Vector3.Lerp(character.transform.position, targetPosition, 1);
        
        for (int i = 0; i < 6; i++) {
            switch (i){
                case 5:
                    speed = 6;
                    heightModifier = 0;
                    break;
                case 1:
                case 4:
                    speed = 5;
                    heightModifier = 0.2f;
                    break;
                case 2:
                    speed = 4;
                    heightModifier = 0.4f;
                    break;
            }
            targetPosition = P[i];
            targetPosition.y += heightModifier;
            while (character.transform.position != targetPosition){
                character.transform.position = Vector2.MoveTowards(character.transform.position, targetPosition, Time.deltaTime * speed);
                yield return null;
            }
        }
    }

    public void TurnCharacter(Animator character, string direction) {
        
        switch (direction){
            case "Right":
                character.SetFloat("Horizontal Direction", 1);
                character.SetFloat("Vertical Direction", 0);
                break;
            case "Left":
                character.SetFloat("Horizontal Direction", -1);
                character.SetFloat("Vertical Direction", 0);
                break;
            case "Up":
                character.SetFloat("Horizontal Direction", 0);
                character.SetFloat("Vertical Direction", 1);
                break;
            case "Down":
                character.SetFloat("Horizontal Direction", 0);
                character.SetFloat("Vertical Direction", -1);
                break;
        }
    }

    public void ResetCamera(){

        Camera.main.transform.parent = playerController.transform;
        Camera.main.orthographicSize = 4;
        Camera.main.transform.localPosition = new Vector3(0, 0, -10);
    }
}