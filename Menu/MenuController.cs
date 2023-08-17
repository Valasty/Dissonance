using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuController : MonoBehaviour {

    public GeneralFunctions general;
    public Text[] mainButtons = new Text[6];
    public Transform[] mainCanvas = new Transform[6];
    public Transform skillsCanvas;
    public GameObject equipmentsCanvas;
    public Transform inventoryCanvas;
    public GameObject itemButtonPrefab;
    public GameObject skillButtonPrefab;
    public GameObject equipButtonPrefab;
    public GameObject characterButtonPrefab;

    public Image characterPortrait;
    public Text descriptionText;
    public Text[] skillPointsText;
    public Text[] characterStats;
    public Text[] characterStatsBonus;
    public Button[] characterEquipments;

    UIButton currentSelectedButton;
    UIButton currentCharacterButton;
    UIButton replaceCharacterButton;
    int currentCanvasIndex;
    int currentSelectedEquipType;
    int[] skillPoints = new int[3];
    int[] maxSkillPoints = new int[3]; //set to half of char learned skills

    public int subMenuLevel;
    
    void Update(){

        if (Input.GetButtonDown("Cancel")){
            if (subMenuLevel == 0){
                general.audioController.enableButtonSound = false;
                general.playerController.eventHappening = false;
                gameObject.SetActive(false);
                return;
            }

            switch (currentCanvasIndex) {
                case 0: //ITEMS
                    mainCanvas[1].gameObject.SetActive(false);
                    mainCanvas[0].gameObject.SetActive(true);
                    EventSystem.current.SetSelectedGameObject(currentSelectedButton.gameObject);
                    break;
                case 1: //SKILLS
                    skillsCanvas.gameObject.SetActive(false);
                    mainCanvas[1].gameObject.SetActive(true);
                    EventSystem.current.SetSelectedGameObject(currentCharacterButton.gameObject);
                    break;
                case 2: //EQUIPS
                    if (subMenuLevel == 2){ //to back from character equip
                        EnableMainEquipmentButtons(true);
                        EventSystem.current.SetSelectedGameObject(characterEquipments[currentSelectedEquipType].gameObject);
                    }
                    else{
                        equipmentsCanvas.SetActive(false);
                        mainCanvas[1].gameObject.SetActive(true);
                        EventSystem.current.SetSelectedGameObject(currentCharacterButton.gameObject);
                    }
                    break;
                case 3: //FORMATION
                    currentCharacterButton.GetComponent<Image>().color = Color.white;
                    replaceCharacterButton = null;
                    break;
            }
            descriptionText.text = null;
            subMenuLevel--;
        }

        if (Input.GetButtonDown("L1"))
            BackButtonPress("L1");        

        if (Input.GetButtonDown("R1"))
            BackButtonPress("R1");        
    }

    void BackButtonPress(string button){
        
        if (subMenuLevel > 0){
            general.audioController.PlayInvalidNavigateSound();
            return;
        }

        descriptionText.text = null;
        mainButtons[currentCanvasIndex].color = Color.white;
        mainCanvas[currentCanvasIndex].gameObject.SetActive(false);

        if (button == "L1"){
            if (currentCanvasIndex == 0)
                currentCanvasIndex = mainCanvas.Length - 1;
            else
                currentCanvasIndex--;
        }
        else{
            if (currentCanvasIndex == mainCanvas.Length - 1)
                currentCanvasIndex = 0;
            else
                currentCanvasIndex++;
        }
        mainButtons[currentCanvasIndex].color = Color.yellow;
        mainCanvas[currentCanvasIndex].gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null); //necessary to avoid visual bug
        EventSystem.current.SetSelectedGameObject(mainCanvas[currentCanvasIndex].GetChild(0).gameObject);
    }
    
    void OnEnable(){
        
        EventSystem.current.SetSelectedGameObject(null);

        mainButtons[currentCanvasIndex].color = Color.white;
        mainCanvas[currentCanvasIndex].gameObject.SetActive(false);
        currentCanvasIndex = 0;
        mainButtons[currentCanvasIndex].color = Color.yellow;
        mainCanvas[currentCanvasIndex].gameObject.SetActive(false);
        mainCanvas[0].gameObject.SetActive(true);

        //INSTANTIATE ITEMS
        for (int i = 1; i < mainCanvas[0].childCount; i++)
            Destroy(mainCanvas[0].GetChild(i).gameObject);

        List<PlayerItem> itemList = new List<PlayerItem>();
        itemList.AddRange(general.database.Player.Inventory[(int)ItemType.Usable]);
        itemList.AddRange(general.database.Player.Inventory[(int)ItemType.Key]);
        itemList.AddRange(general.database.Player.Inventory[(int)ItemType.Weapon]);
        itemList.AddRange(general.database.Player.Inventory[(int)ItemType.Helmet]);
        itemList.AddRange(general.database.Player.Inventory[(int)ItemType.Armor]);
        itemList.AddRange(general.database.Player.Inventory[(int)ItemType.Boots]);
        itemList.AddRange(general.database.Player.Inventory[(int)ItemType.Accessory]);

        GameObject itemButton = null;
        foreach (PlayerItem playerItem in itemList){
            itemButton = (itemButton == null) ? itemButtonPrefab : Instantiate(itemButtonPrefab, mainCanvas[0]);
            UIButton uiButton = itemButton.GetComponent<UIButton>();
            uiButton.playerItem = playerItem;
        }
        EventSystem.current.SetSelectedGameObject(itemButtonPrefab);

        //INSTANTIATE CHARACTERS
        for (int i = 1; i < mainCanvas[1].childCount; i++)
            Destroy(mainCanvas[1].GetChild(i).gameObject);
        GameObject characterButton = null;
        foreach (Character character in general.database.Player.Characters){
            characterButton = (characterButton == null) ? characterButtonPrefab : Instantiate(characterButtonPrefab, mainCanvas[1]);
            UIButton uiButton = characterButton.GetComponent<UIButton>();
            uiButton.character = character;
        }
    }

    public void ItemButtonSelect(){

        currentSelectedButton = EventSystem.current.currentSelectedGameObject.GetComponent<UIButton>();
        descriptionText.text = (currentSelectedButton.playerItem == null) ? null : currentSelectedButton.playerItem.Item.Description;
    }
    
    public void ItemButtonClick() {

        if (currentSelectedButton.playerItem.Item.Type == ItemType.Usable && currentSelectedButton.playerItem.Item.Skill.Damage < 0){
            subMenuLevel++;
            mainCanvas[0].gameObject.SetActive(false);
            mainCanvas[1].gameObject.SetActive(true);
            EventSystem.current.SetSelectedGameObject(characterButtonPrefab);
        }
        else
            general.audioController.PlayInvalidNavigateSound();
    }

    public void CharacterButtonSelect(){

        if (replaceCharacterButton == null)
            return;

        if (replaceCharacterButton.gameObject == EventSystem.current.currentSelectedGameObject)
            descriptionText.text = general.localization.changeDistanceText;
        else
            descriptionText.text = general.localization.changePositionText;
    }

    public void CharacterButtonClick() {

        currentCharacterButton = EventSystem.current.currentSelectedGameObject.GetComponent<UIButton>();

        switch (currentCanvasIndex){
            
            //ITEM
            case 0:
                if ((currentCharacterButton.character.HPCurrent == 0 && !currentSelectedButton.playerItem.Item.Skill.DeadTarget)
                || (currentCharacterButton.character.HPCurrent > 0 && currentSelectedButton.playerItem.Item.Skill.DeadTarget)
                || currentCharacterButton.character.HPCurrent == currentCharacterButton.character.HPMax){
                    general.audioController.PlayInvalidNavigateSound();
                    return;
                }

                currentCharacterButton.character.HPCurrent = Mathf.Clamp(currentCharacterButton.character.HPCurrent - (int)currentSelectedButton.playerItem.Item.Skill.Damage, 0, currentCharacterButton.character.HPMax);
                currentCharacterButton.hpText.text = currentCharacterButton.character.HPCurrent + " / " + currentCharacterButton.character.HPMax;


                general.RemoveItem(currentSelectedButton.playerItem.Item);
                if (currentSelectedButton.playerItem.Quantity > 1)
                    currentSelectedButton.transform.GetChild(1).GetComponent<Text>().text = currentSelectedButton.playerItem.Quantity.ToString();                
                else{
                    if (currentSelectedButton.gameObject == itemButtonPrefab) //if deleting the first box, moves the prefab to the second one
                        itemButtonPrefab = mainCanvas[0].GetChild(1).gameObject;
                    Destroy(currentSelectedButton.gameObject);
                    mainCanvas[1].gameObject.SetActive(false);
                    mainCanvas[0].gameObject.SetActive(true);
                    EventSystem.current.SetSelectedGameObject(itemButtonPrefab);
                    subMenuLevel--;
                }
                break;

            //SKILL
            case 1:
                for (int i = 0; i < 3; i++){
                    skillPoints[i] = 0;
                    maxSkillPoints[i] = 0;
                }
                for (int i = 1; i < skillsCanvas.GetChild(0).childCount; i++)
                    Destroy(skillsCanvas.GetChild(0).GetChild(i).gameObject);
                for (int i = 0; i < skillsCanvas.GetChild(1).childCount; i++)
                    Destroy(skillsCanvas.GetChild(1).GetChild(i).gameObject);
                for (int i = 0; i < skillsCanvas.GetChild(2).childCount; i++)
                    Destroy(skillsCanvas.GetChild(2).GetChild(i).gameObject);
                GameObject skillButton = null;
                foreach (Skill skill in currentCharacterButton.character.Skills){
                    if (skill.Status == SkillStatus.None)
                        continue;
                    skillButton = (skillButton == null) ? skillButtonPrefab : Instantiate(skillButtonPrefab, skillsCanvas.GetChild((int)skill.Type));
                    skillButton.GetComponent<UIButton>().skill = skill;
                    maxSkillPoints[(int)skill.Type]++;
                    if (skill.Status == SkillStatus.Active)
                        skillPoints[(int)skill.Type]++;
                }
                for (int i = 0; i < 3; i++){
                    maxSkillPoints[i] = Mathf.Clamp(maxSkillPoints[i] / 2, 1, 6);
                    skillPointsText[i].text = skillPoints[i] + "/" + maxSkillPoints[i];
                }

                mainCanvas[1].gameObject.SetActive(false);
                skillsCanvas.gameObject.SetActive(true);
                subMenuLevel++;
                EventSystem.current.SetSelectedGameObject(skillButtonPrefab);
                break;

            //EQUIPS
            case 2:
                mainCanvas[1].gameObject.SetActive(false);
                equipmentsCanvas.SetActive(true);
                subMenuLevel++;
                LoadCharacterData();
                EventSystem.current.SetSelectedGameObject(characterEquipments[0].gameObject);
                break;

            //FORMATION
            case 3:

                //char select
                if (replaceCharacterButton == null){
                    replaceCharacterButton = currentCharacterButton;
                    currentCharacterButton.GetComponent<Image>().color = Color.green;
                    descriptionText.text = general.localization.changeDistanceText;
                    subMenuLevel++;
                }

                //char formation swap
                else if (replaceCharacterButton == currentCharacterButton){
                    currentCharacterButton.character.FrontRow = !currentCharacterButton.character.FrontRow;
                    currentCharacterButton.formationText.text = currentCharacterButton.character.FrontRow ? "F" : "B";
                    currentCharacterButton.GetComponent<Image>().color = Color.white;
                    replaceCharacterButton = null;
                    descriptionText.text = null;
                    subMenuLevel--;
                }

                //char position swap
                else{
                    int index1 = general.database.Player.Characters.IndexOf(currentCharacterButton.character);
                    int index2 = general.database.Player.Characters.IndexOf(replaceCharacterButton.character);
                    Character temp = general.database.Player.Characters[index1];
                    general.database.Player.Characters[index1] = general.database.Player.Characters[index2];
                    general.database.Player.Characters[index2] = temp;
                    currentCharacterButton.character = general.database.Player.Characters[index1];
                    replaceCharacterButton.character = general.database.Player.Characters[index2];
                    replaceCharacterButton.gameObject.GetComponent<Image>().color = Color.white;
                    replaceCharacterButton = null;
                    descriptionText.text = null;
                    subMenuLevel--;
                }
                break;
        }
    }
    
    public void SkillButtonSelect(){

        currentSelectedButton = EventSystem.current.currentSelectedGameObject.GetComponent<UIButton>();

        if (currentSelectedButton.skill.Accuracy != 999)
            descriptionText.text = "[" + (currentCharacterButton.character.Accuracy + currentSelectedButton.skill.Accuracy) + " Acc] " + currentSelectedButton.skill.Description;
        else
            descriptionText.text = currentSelectedButton.skill.Description;
    }

    public void SkillButtonClick(){
                
        //REMOVE SKILL
        if (currentSelectedButton.buttonImage.color == Color.green) {
            if (currentSelectedButton.skill.Type == SkillType.Active && skillPoints[(int)currentSelectedButton.skill.Type] == 1){ //you cant empty active skills
                general.audioController.PlayInvalidNavigateSound();
                return;
            }
            currentSelectedButton.buttonImage.color = Color.white;
            currentSelectedButton.skill.Status = SkillStatus.Learned;
            skillPoints[(int)currentSelectedButton.skill.Type]--;
        }

        //ADD SKILL
        else if (skillPoints[(int)currentSelectedButton.skill.Type] < maxSkillPoints[(int)currentSelectedButton.skill.Type]){
            currentSelectedButton.buttonImage.color = Color.green;
            currentSelectedButton.skill.Status = SkillStatus.Active;
            skillPoints[(int)currentSelectedButton.skill.Type]++;
        }

        else
            general.audioController.PlayInvalidNavigateSound();

        skillPointsText[(int)currentSelectedButton.skill.Type].text = skillPoints[(int)currentSelectedButton.skill.Type] + "/" + maxSkillPoints[(int)currentSelectedButton.skill.Type];
    }
    
    void LoadCharacterData(){

        Item equipmentAttributes = currentCharacterButton.character.GetEquipmentsAttributes();
        
        //STATS
        characterPortrait.sprite = general.database.Portraits.Find(x => x.Name == currentCharacterButton.character.Name).Sprite;
        characterStats[0].text = currentCharacterButton.character.Name;
        characterStats[1].text = currentCharacterButton.character.Level.ToString();
        characterStats[2].text = currentCharacterButton.character.HPCurrent.ToString("000") + " / " + currentCharacterButton.character.HPMax.ToString("000");
        characterStats[3].text = currentCharacterButton.character.CurrentXP + " / " + currentCharacterButton.character.NextLevelXP;
        characterStats[4].text = (currentCharacterButton.character.Attack + equipmentAttributes.Attack).ToString();
        characterStats[5].text = (currentCharacterButton.character.MagicATK + equipmentAttributes.MagicATK).ToString();
        characterStats[6].text = (currentCharacterButton.character.Defense + equipmentAttributes.Defense).ToString();
        characterStats[7].text = (currentCharacterButton.character.MagicDEF + equipmentAttributes.MagicDEF).ToString();
        characterStats[8].text = (currentCharacterButton.character.Speed + equipmentAttributes.Speed).ToString();
        characterStats[9].text = (currentCharacterButton.character.Accuracy + equipmentAttributes.Accuracy).ToString() + "%";
        characterStats[10].text = (currentCharacterButton.character.Evasion + equipmentAttributes.Evasion).ToString() + "%";
        characterStats[11].text = Mathf.Clamp(currentCharacterButton.character.Elemental[0] + equipmentAttributes.Elemental[0], -100, 100).ToString();
        characterStats[12].text = Mathf.Clamp(currentCharacterButton.character.Elemental[1] + equipmentAttributes.Elemental[1], -100, 100).ToString();
        characterStats[13].text = Mathf.Clamp(currentCharacterButton.character.Elemental[2] + equipmentAttributes.Elemental[2], -100, 100).ToString();
        characterStats[14].text = Mathf.Clamp(currentCharacterButton.character.Elemental[3] + equipmentAttributes.Elemental[3], -100, 100).ToString();
        characterStats[15].text = Mathf.Clamp(currentCharacterButton.character.Elemental[4] + equipmentAttributes.Elemental[4], -100, 100).ToString();
        characterStats[16].text = Mathf.Clamp(currentCharacterButton.character.Elemental[5] + equipmentAttributes.Elemental[5], -100, 100).ToString();

        characterStatsBonus[0].text = "(" + equipmentAttributes.Attack.ToString("+#;-#;0") + ")";
        characterStatsBonus[1].text = "(" + equipmentAttributes.MagicATK.ToString("+#;-#;0") + ")";
        characterStatsBonus[2].text = "(" + equipmentAttributes.Defense.ToString("+#;-#;0") + ")";
        characterStatsBonus[3].text = "(" + equipmentAttributes.MagicDEF.ToString("+#;-#;0") + ")";
        characterStatsBonus[4].text = "(" + equipmentAttributes.Speed.ToString("+#;-#;0") + ")";
        characterStatsBonus[5].text = "(" + equipmentAttributes.Accuracy.ToString("+#;-#;0") + ")";
        characterStatsBonus[6].text = "(" + equipmentAttributes.Evasion.ToString("+#;-#;0") + ")";
        characterStatsBonus[7].text = "(" + equipmentAttributes.Elemental[0].ToString("+#;-#;0") + ")";
        characterStatsBonus[8].text = "(" + equipmentAttributes.Elemental[1].ToString("+#;-#;0") + ")";
        characterStatsBonus[9].text = "(" + equipmentAttributes.Elemental[2].ToString("+#;-#;0") + ")";
        characterStatsBonus[10].text = "(" + equipmentAttributes.Elemental[3].ToString("+#;-#;0") + ")";
        characterStatsBonus[11].text = "(" + equipmentAttributes.Elemental[4].ToString("+#;-#;0") + ")";
        characterStatsBonus[12].text = "(" + equipmentAttributes.Elemental[5].ToString("+#;-#;0") + ")";

        //EQUIPMENTS
        for (int i = 0; i < characterEquipments.Length; i++)
            characterEquipments[i].GetComponentInChildren<Text>().text = (currentCharacterButton.character.Equipments[i] == null) ? null : currentCharacterButton.character.Equipments[i].LocalizedName;
    }
        
    void EnableMainEquipmentButtons(bool enable){

        for (int i = 0; i < characterEquipments.Length; i++)
            characterEquipments[i].interactable = enable;
    }

    public void MainEquipmentButtonSelect(int type){

        currentSelectedEquipType = type;

        //gets the item and updates the description
        descriptionText.text = (currentCharacterButton.character.Equipments[currentSelectedEquipType] == null) ? null : currentCharacterButton.character.Equipments[currentSelectedEquipType].Description;

        //if the item is found, updates the list of equipments of that type
        for (int i = 1; i < inventoryCanvas.childCount; i++)
            Destroy(inventoryCanvas.GetChild(i).gameObject);

        int adjustedEquipType = (currentSelectedEquipType == 6) ? 5 : currentSelectedEquipType; //necessary because of the two accessories!!!
        for (int i = 0; i < general.database.Player.Inventory[adjustedEquipType].Count; i++){
            if ((currentSelectedEquipType == 0 || currentSelectedEquipType == 1) && general.database.Player.Inventory[adjustedEquipType][i].Item.CharId != currentCharacterButton.character.Id)
                continue;
            GameObject itemButton = Instantiate(equipButtonPrefab, inventoryCanvas);
            UIButton uiButton = itemButton.GetComponent<UIButton>();
            uiButton.playerItem = general.database.Player.Inventory[adjustedEquipType][i];
            itemButton.transform.GetChild(2).GetComponent<Text>().text = "x";
        }
    }

    public void MainEquipmentButtonClick(){

        subMenuLevel++;
        EnableMainEquipmentButtons(false);
        EventSystem.current.SetSelectedGameObject(equipButtonPrefab);
    }

    public void InventoryEquipmentButtonClick() {
                
        if (currentCharacterButton.character.Equipments[currentSelectedEquipType] != null)
            general.AddItem(currentCharacterButton.character.Equipments[currentSelectedEquipType]);
        if (currentSelectedButton.playerItem == null){
            currentCharacterButton.character.Equipments[currentSelectedEquipType] = null;
            characterEquipments[currentSelectedEquipType].GetComponentInChildren<Text>().text = null;
        }
        else{
            currentCharacterButton.character.Equipments[currentSelectedEquipType] = currentSelectedButton.playerItem.Item;
            general.RemoveItem(currentSelectedButton.playerItem.Item);
        }
        subMenuLevel--;
        LoadCharacterData();
        EnableMainEquipmentButtons(true);
        EventSystem.current.SetSelectedGameObject(characterEquipments[currentSelectedEquipType].gameObject);
    }

    public void SaveGameButton(){

        GeneralFunctions.SaveGame(general.switchs, general.database.Player);
    }
}