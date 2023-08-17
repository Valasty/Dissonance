using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//ALL LOCALIZATION .TXT FILES HAVE TO BE SAVED AS UFT-8 OR SPECIAL CHARACTERS WONT WORK
//ALL LOCALIZATION .TXT FILES HAVE TO BE SAVED AS UFT-8 OR SPECIAL CHARACTERS WONT WORK
//ALL LOCALIZATION .TXT FILES HAVE TO BE SAVED AS UFT-8 OR SPECIAL CHARACTERS WONT WORK
public class LocalizationController : MonoBehaviour {

    public Database database;
    const string localizationFolder = "Localization/";
    public Dictionary<string, string> localizationStrings = new Dictionary<string, string>();
    //public GameObject menu;

    public Text[] mainMenuButtons;
    
    //menu
    public Text[] mainMenuOptionsTexts;
    public Text[] characterStatsTexts;
    public Text[] characterEquipsTexts;
    //public Dictionary<ItemType, string> itemTypes = new Dictionary<ItemType, string>();
    public string changeDistanceText;
    public string changePositionText;

    //game
    public string gameOverText;
    public string retryButtonText;
    public string backToMenuButtonText;
    public string tutorialTitle;
    public string itemFoundTitle;
    public string itemFoundText1;
    public string itemFoundText2;
    /*public string pointEarnedTitle;
    public string pointEarnedText;*/
    public string itemsButton;
    public string missAttackText;
    public string battleEndingText;
    public string levelUpText;

    void Awake(){

        GameSettings.Language = Application.systemLanguage;
        LocalizeUI();
    }

    public void LocalizeGame(){

        LocalizeMenu();
        LocalizeItems();
        LocalizeSkills();
        //LocalizeConditions();
    }

    void LocalizeMenu(){

        SetLocalization("Menu/");

        for (int i = 0; i < mainMenuOptionsTexts.Length; i++) //só pode ir ate 5
            mainMenuOptionsTexts[i].text = localizationStrings[mainMenuOptionsTexts[i].name];

        for (int i = 0; i < characterStatsTexts.Length; i++) //só pode ir ate 7
            characterStatsTexts[i].text = localizationStrings[characterStatsTexts[i].name];

        for (int i = 0; i < characterEquipsTexts.Length; i++) //só pode ir ate 8
            characterEquipsTexts[i].text = localizationStrings[characterEquipsTexts[i].name];

        changeDistanceText = localizationStrings["ChangeDistanceText"];
        changePositionText = localizationStrings["ChangePositionText"];

        /*itemTypes.Add(ItemType.Key, localizationStrings[ItemType.Key.ToString()]);
        itemTypes.Add(ItemType.Usable, localizationStrings[ItemType.Usable.ToString()]);
        itemTypes.Add(ItemType.Weapon, localizationStrings[ItemType.Weapon.ToString()]);
        itemTypes.Add(ItemType.Support, localizationStrings[ItemType.Support.ToString()]);
        itemTypes.Add(ItemType.Helmet, localizationStrings[ItemType.Helmet.ToString()]);
        itemTypes.Add(ItemType.Armor, localizationStrings[ItemType.Armor.ToString()]);
        itemTypes.Add(ItemType.Boots, localizationStrings[ItemType.Boots.ToString()]);
        itemTypes.Add(ItemType.Accessory, localizationStrings[ItemType.Accessory.ToString()]);*/
    }

    ////////////////////////////////////////////////////
    /////// SETS UP LOCALIZATION FILES AS NEEDED ///////
    ////////////////////////////////////////////////////
    public void SetLocalization(string subfolder){

        TextAsset localizationText = Resources.Load<TextAsset>(localizationFolder + subfolder + GameSettings.Language.ToString());
        string[] lines = localizationText.text.Split(new string[] { "\r\n", "\n\r", "\n" }, System.StringSplitOptions.RemoveEmptyEntries);
        localizationStrings.Clear();
        for (int i = 0; i < lines.Length; i++){
            string[] pairs = lines[i].Split(new char[] { '\t', '=' }, 2);
            localizationStrings.Add(pairs[0].Trim(), pairs[1].Trim());
        }
    }

    ////////////////////////////////////////////////
    /////// APPLIES LOCALIZATION FILES ON UI ///////
    ////////////////////////////////////////////////
    public void LocalizeUI() { //////////////////////////////////////////////////////////////////////////////////////////////////////////////////// trocar isso pra array igual menu????

        SetLocalization("UI/");

        mainMenuButtons[0].text = localizationStrings["NewGameButton"];
        mainMenuButtons[1].text = localizationStrings["LoadGameButton"];
        mainMenuButtons[2].text = localizationStrings["LanguageButton"];
        mainMenuButtons[3].text = localizationStrings["QuitButton"];

        gameOverText = localizationStrings["GameOverText"];
        retryButtonText = localizationStrings["RetryButton"];
        backToMenuButtonText = localizationStrings["BackToMenuButton"];
        tutorialTitle = localizationStrings["TutorialTitle"];
        itemFoundTitle = localizationStrings["ItemFoundTitle"];
        itemFoundText1 = localizationStrings["ItemFoundText1"];
        itemFoundText2 = localizationStrings["ItemFoundText2"] + " ";
        itemsButton = localizationStrings["ItemsButton"];
        /*pointEarnedTitle = " " + localizationStrings["PointEarnedTitle"];
        pointEarnedText = " " + localizationStrings["PointEarnedText"];*/
        missAttackText = localizationStrings["MissAttackText"];
        battleEndingText = " " + localizationStrings["BattleEndingText"];
        levelUpText = " " + localizationStrings["LevelUpText"] + " ";

        /*database.Elements[0].LocalizedName = localizationStrings["FireElement"];
        database.Elements[1].LocalizedName = localizationStrings["WaterElement"];
        database.Elements[2].LocalizedName = localizationStrings["WindElement"];
        database.Elements[3].LocalizedName = localizationStrings["EarthElement"];
        database.Elements[4].LocalizedName = localizationStrings["LightElement"];
        database.Elements[5].LocalizedName = localizationStrings["DarkElement"];*/
    }

    ///////////////////////////////////////////////////
    /////// APPLIES LOCALIZATION FILES ON ITEMS ///////
    ///////////////////////////////////////////////////
    void LocalizeItems() {
        
        SetLocalization("Items/");
        for (int i = 0; i < database.Items.Count; i++){
            for (int y = 0; y < database.Items[i].Count; y++){
                database.Items[i][y].LocalizedName = localizationStrings[database.Items[i][y].Name];
                database.Items[i][y].Description = localizationStrings[database.Items[i][y].Name + " Desc"];
            }
        }
    }

    void LocalizeSkills() {
        
        SetLocalization("Skills/");
        for (int i = 0; i < database.Skills.Count; i++){
            for (int y = 0; y < database.Skills[i].Count; y++){
                database.Skills[i][y].LocalizedName = localizationStrings[database.Skills[i][y].Name];
                database.Skills[i][y].Description = localizationStrings[database.Skills[i][y].Name + " Desc"];
            }
        }
    }

    /*void LocalizeConditions() {
        
        for (int i = 0; i < database.conditionsList.Count; i++){
            database.conditionsList[i].Name = localizationStrings[database.conditionsList[i].Id];
        }
    }*/
}