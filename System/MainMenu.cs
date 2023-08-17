using UnityEngine;

public class MainMenu : MonoBehaviour {

    public GeneralFunctions general;
    public GameObject mainMenuCanvas;
    
    void Start(){

        Cursor.visible = false;
        general.playerController.eventHappening = true;
        general.audioController.enableButtonSound = true;
        general.database.Player = new Player();


        //NewGameButton(); /////////////////////////////////////////////////////// DELETAR
        //LoadGameButton(); /////////////////////////////////////////////////////// DELETAR

    }

    public void NewGameButton(){

        mainMenuCanvas.SetActive(false);
        general.localization.LocalizeGame();

        //LOADS BASE GAME DATA
        general.switchs = new Switchs();
        Character newChar = general.database.Characters[0];
        general.database.Player.Characters.Add(new Character(newChar.Id, newChar.Name, newChar.FrontRow, newChar.HPMax, newChar.HPCurrent, newChar.Attack, newChar.MagicATK, newChar.Defense, 
            newChar.MagicDEF, newChar.Speed, newChar.Elemental, Skills: newChar.Skills));
        general.database.Player.Characters[0].Equipments[(int)ItemType.Weapon] = general.database.Items[(int)ItemType.Weapon][0];
        general.database.Player.Characters[0].Equipments[(int)ItemType.Support] = general.database.Items[(int)ItemType.Support][0];
        general.CheckLevelUp(general.database.Player.Characters[0], targetLevel: 10);
        general.database.Player.Characters[0].HPCurrent = general.database.Player.Characters[0].HPMax; //necessary to start full hp since level up doesnt heal
        general.AddItem(general.database.Items[(int)ItemType.Key].Find(x => x.Name == "Family Memento"));
        general.AddItem(general.database.Items[(int)ItemType.Usable].Find(x => x.Name == "Potion"), 3);
        general.AddItem(general.database.Items[(int)ItemType.Usable].Find(x => x.Name == "Antidote"), 2);

        general.eventSequence = "Intro"; //ends on Andorhal_Load.Tutorial
        general.audioController.enableButtonSound = false;
        StartCoroutine(general.LoadScene("Intro 1", 3, 5));
    }

    public void LoadGameButton(){

        mainMenuCanvas.SetActive(false);
        general.localization.LocalizeGame(); // ------------------------------- NAO PODE REMOVER ISSO?
        string currentScene = general.LoadGame();
        
        //SET RAIN INTENSITY BASED ON THE GAME PROGRESSION
        if (!general.switchs.andorhal_escapedIsland)
            general.rain.SetRainIntensity("Storm");
        else if (!general.switchs.middleGameReached)
            general.rain.SetRainIntensity("Normal");



        ///////////////////////////////////// TESTES /////////////////////////////////////
        //general.database.Player.Inventory[6].Add(new PlayerItem(new Item(88, "Ether", ItemType.Usable, Skill: new Skill(9999, "Ether", EnemyTarget: 0, PhysicalDamage: false, Damage: -40)), 2));

        /*general.database.Player.Characters[0].HPCurrent = 1;
        general.AddItem(general.database.Items[(int)ItemType.Weapon].Find(x => x.Name == "Old Sword"), 2);
        general.AddItem(general.database.Items[(int)ItemType.Weapon].Find(x => x.Name == "Iron Sword"));
        general.AddItem(general.database.Items[(int)ItemType.Weapon].Find(x => x.Name == "Old Dagger"));
        general.AddItem(general.database.Items[(int)ItemType.Weapon].Find(x => x.Name == "Iron Dagger"), 2);
        general.AddItem(general.database.Items[(int)ItemType.Weapon].Find(x => x.Name == "Old Bow"), 2);
        general.AddItem(general.database.Items[(int)ItemType.Weapon].Find(x => x.Name == "Iron Bow"));
        general.AddItem(general.database.Items[(int)ItemType.Support].Find(x => x.Name == "Old Shield"));
        general.AddItem(general.database.Items[(int)ItemType.Support].Find(x => x.Name == "Old Off Dagger"));
        general.AddItem(general.database.Items[(int)ItemType.Support].Find(x => x.Name == "Old Arrow"));
        general.AddItem(general.database.Items[(int)ItemType.Helmet].Find(x => x.Name == "Old Helmet"));
        general.AddItem(general.database.Items[(int)ItemType.Armor].Find(x => x.Name == "Old Armor"));
        general.AddItem(general.database.Items[(int)ItemType.Boots].Find(x => x.Name == "Old Boots"));
        general.AddItem(general.database.Items[(int)ItemType.Key].Find(x => x.Name == "Hammer"));
        general.AddItem(general.database.Items[(int)ItemType.Key].Find(x => x.Name == "Oar"));
        general.AddItem(general.database.Items[(int)ItemType.Key].Find(x => x.Name == "Planks"));
        general.AddItem(general.database.Items[(int)ItemType.Accessory].Find(x => x.Name == "Magic Ring"), 3);
        general.AddItem(general.database.Items[(int)ItemType.Usable].Find(x => x.Name == "Potion"), 2);
        general.AddItem(general.database.Items[(int)ItemType.Key].Find(x => x.Name == "Hammer"));
        general.AddItem(general.database.Items[(int)ItemType.Key].Find(x => x.Name == "Oar"));
        general.AddItem(general.database.Items[(int)ItemType.Key].Find(x => x.Name == "Planks"));

        Character newChar = general.database.Characters[1];
        general.database.Player.Characters.Add(new Character(newChar.Id, newChar.Name, newChar.FrontRow, newChar.HPMax, newChar.HPCurrent, newChar.Attack, newChar.MagicATK, newChar.Defense, 
            newChar.MagicDEF, newChar.Speed, Skills: newChar.Skills));
        general.database.Player.Characters[1].Equipments[(int)ItemType.Weapon] = general.database.Items[(int)ItemType.Weapon][0];
        general.database.Player.Characters[1].Equipments[(int)ItemType.Support] = general.database.Items[(int)ItemType.Support][0];
        general.LevelUpCharacter(general.database.Player.Characters[1], targetLevel: 10);

        newChar = general.database.Characters[2];
        general.database.Player.Characters.Add(new Character(newChar.Id, newChar.Name, newChar.FrontRow, newChar.HPMax, newChar.HPCurrent, newChar.Attack, newChar.MagicATK, newChar.Defense, 
            newChar.MagicDEF, newChar.Speed, Skills: newChar.Skills));
        general.database.Player.Characters[2].Equipments[(int)ItemType.Weapon] = general.database.Items[(int)ItemType.Weapon][0];
        general.database.Player.Characters[2].Equipments[(int)ItemType.Support] = general.database.Items[(int)ItemType.Support][0];*/
        ///////////////////////////////////// TESTES /////////////////////////////////////



        general.eventSequence = null;
        general.audioController.enableButtonSound = false;
        StartCoroutine(general.LoadScene(currentScene, 3, 1.5f));
        //StartCoroutine(general.LoadScene(currentScene, 0.1f, 0.1f));
    }

    public void LanguageButton(){

        GameSettings.Language = (GameSettings.Language == Application.systemLanguage) ? SystemLanguage.English : Application.systemLanguage;
        general.localization.LocalizeUI();
    }

    public void QuitGameButton(){
        
        Application.Quit();
    }
}