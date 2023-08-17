using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISceneManager { void CutsceneSetup(bool start); }
public interface IInteractable { IEnumerator Interact(); }

public enum Element { Fire, Water, Wind, Earth, Light, Dark, None }
public enum ItemType { Weapon, Support, Helmet, Armor, Boots, Accessory, Usable, Key }
public enum SkillStatus { None, Learned, Active }
public enum SkillType { Active, Passive, Surge }
public enum SpecialEffectTrigger { None, /*Before, Attack*/ During, After, Counter, CounterOnDeath, CounterGlobal }
public enum SkillRange { Self, Melee, Ranged }
public enum ConditionType { DMGDealtMod, DMGTakenMod, Titan, DoT, Taunt, HitCheck, Marker, Combo, AreaTargetMod }
public enum ReactionBubbleType { Surprised }

public class Player{

    public List<Character> Characters;

    List<PlayerItem> InventoryWeapons;
    List<PlayerItem> InventorySupports;
    List<PlayerItem> InventoryHelmets;
    List<PlayerItem> InventoryArmors;
    List<PlayerItem> InventoryBoots;
    List<PlayerItem> InventoryAccessories;
    List<PlayerItem> InventoryUsableItems;
    List<PlayerItem> InventoryKeyItems;

    public List<List<PlayerItem>> Inventory = new List<List<PlayerItem>>();

    public Player(){

        Characters = new List<Character>();
        InventoryWeapons = new List<PlayerItem>();
        InventorySupports = new List<PlayerItem>();
        InventoryHelmets = new List<PlayerItem>();
        InventoryArmors = new List<PlayerItem>();
        InventoryBoots = new List<PlayerItem>();
        InventoryAccessories = new List<PlayerItem>();
        InventoryUsableItems = new List<PlayerItem>();
        InventoryKeyItems = new List<PlayerItem>();

        Inventory.Add(InventoryWeapons);
        Inventory.Add(InventorySupports);
        Inventory.Add(InventoryHelmets);
        Inventory.Add(InventoryArmors);
        Inventory.Add(InventoryBoots);
        Inventory.Add(InventoryAccessories);
        Inventory.Add(InventoryUsableItems);
        Inventory.Add(InventoryKeyItems);
    }
}

[Serializable]
public class SpritesList{

    public string Name;
    public Sprite Sprite;
}

[Serializable]
public class Animators{

    public string Name;
    public RuntimeAnimatorController Animator;
}

public class Character {

    public int Id;
    public string Name;
    public bool FrontRow;
    public int HPCurrent;
    public int HPMax;
    public int Attack;
    public int MagicATK;
    public int Defense;
    public int MagicDEF;
    public int Speed;
    public int[] Elemental;
    public int Accuracy = 100; /////////////////////////// used only by monsters
    public int Evasion; /////////////////////////// used only by monsters
    public List<Skill> Skills;
    public int Level;
    public int CurrentXP = 0;
    public int NextLevelXP = 5;

    public Item[] Equipments = new Item[7];

    public Character(int Id, string Name, bool FrontRow, int HPMax, int HPCurrent, int Attack, int MagicATK, int Defense, int MagicDEF, int Speed,
        int[] Elemental, int Evasion = 0, int Level = 0, int CurrentXP = 0, List<Skill> Skills = null){

        this.Id = Id;
        this.Name = Name;
        this.FrontRow = FrontRow;
        this.HPMax = HPMax;
        this.HPCurrent = HPCurrent;
        this.Attack = Attack;
        this.MagicATK = MagicATK;
        this.Defense = Defense;
        this.MagicDEF = MagicDEF;
        this.Speed = Speed;
        this.Elemental = Elemental;
        this.Evasion = Evasion;
        this.Level = Level;
        this.CurrentXP = CurrentXP;
        this.Skills = Skills;
    }

    //USED BY SAVE FUNCTION
    public int GetEquipmentId(Item equipment) {
        if (equipment != null)
            return equipment.Id;
        return 9999;
    }

    //USED TO AVOID NULL ISSUES
    public Item GetEquipmentsAttributes() {
        Item totalAttributes = new Item(9999, "temp", ItemType.Key);
        foreach (Item equipment in Equipments){
            if (equipment == null)
                continue;
            totalAttributes.Attack += equipment.Attack;
            totalAttributes.MagicATK += equipment.MagicATK;
            totalAttributes.Defense += equipment.Defense;
            totalAttributes.MagicDEF += equipment.MagicDEF;
            totalAttributes.Speed += equipment.Speed;
            totalAttributes.Accuracy += equipment.Accuracy;
            totalAttributes.Evasion += equipment.Evasion;
            for(int i = 0; i < totalAttributes.Elemental.Length; i++)
                totalAttributes.Elemental[i] += equipment.Elemental[i];
        }
        return totalAttributes;
    }
}

public class Skill{

    public int Id;
    public string Name;
    public SkillType Type;
    public SkillRange Range;
    public Element Element;
    public bool ConsumeElement;
    public int EnemyTarget; //0 pra aliados, 1 pra inimigos... usado diretamente no index
    public bool DeadTarget;
    public bool AreaTarget;
    public int Accuracy;
    public bool PhysicalDamage;
    public float Damage;
    public SpecialEffectTrigger SpecialEffectTrigger;
    
    public SkillStatus Status;
    public string LocalizedName;
    public string Description;

    public Skill(int Id, string Name, SkillType Type = SkillType.Active, SkillRange Range = SkillRange.Ranged, Element Element = Element.None, bool ConsumeElement = false, int EnemyTarget = 1,
        bool DeadTarget = false, bool AreaTarget = false, int Accuracy = 999, bool PhysicalDamage = true, float Damage = 0, SpecialEffectTrigger SpecialEffectTrigger = SpecialEffectTrigger.None,
        SkillStatus Status = SkillStatus.None, string LocalizedName = null, string Description = null){

        this.Id = Id;
        this.Name = Name;
        this.Type = Type;
        this.Range = Range;
        this.Element = Element;
        this.ConsumeElement = ConsumeElement;
        this.EnemyTarget = EnemyTarget;
        this.DeadTarget = DeadTarget;
        this.AreaTarget = AreaTarget;
        this.Accuracy = Accuracy;
        this.PhysicalDamage = PhysicalDamage;
        this.Damage = Damage;
        this.SpecialEffectTrigger = SpecialEffectTrigger;
        this.Status = Status;
        this.LocalizedName = LocalizedName;
        this.Description = Description;
    }
}

public class GlobalSkill{ //used by Battle

    public Skill Skill;
    public BattleCharacter Character;

    public GlobalSkill(Skill Skill, BattleCharacter Character){
        this.Skill = Skill;
        this.Character = Character;
    }
}

[Serializable]
public class SaveCharacter {

    public int Id;
    public int Level;
    public int CurrentXP;
    public int HPCurrent;
    public bool FrontRow;
    public int[] SkillsStatus = new int[21];
    public int[] Equipments = new int[7];

    public SaveCharacter(int Id, int Level, int CurrentXP, int HPCurrent, bool FrontRow, int[] SkillsStatus, int[] Equipments){

        this.Id = Id;
        this.Level = Level;
        this.CurrentXP = CurrentXP;
        this.HPCurrent = HPCurrent;
        this.FrontRow = FrontRow;
        for (int i = 0; i < SkillsStatus.Length; i++)
            this.SkillsStatus[i] = SkillsStatus[i];         
        for (int i = 0; i < Equipments.Length; i++)
            this.Equipments[i] = Equipments[i];
    }
}

[Serializable]
public class SaveItem{

    public int Id;
    public ItemType Type;
    public int Quantity;

    public SaveItem(int Id, ItemType Type, int Quantity){

        this.Id = Id;
        this.Type = Type;
        this.Quantity = Quantity;
    }
}

public class Item {

    public int Id;
    public string Name;
    public ItemType Type;
    public string LocalizedName;
    public string Description;

    public int Attack;
    public int MagicATK;
    public int Defense;
    public int MagicDEF;
    public int Speed;
    public int Accuracy;
    public int Evasion;
    public int CharId;
    public int[] Elemental;
    public Skill Skill;

    public Item(int Id, string Name, ItemType Type, int Attack = 0, int MagicATK = 0, int Defense = 0, int MagicDEF = 0, int Speed = 0, int Accuracy = 0, int Evasion = 0, int CharId = 999,
        int[] Elemental = null, Skill Skill = null){

        this.Id = Id;
        this.Name = Name;
        this.Type = Type;
        this.Attack = Attack;
        this.MagicATK = MagicATK;
        this.Defense = Defense;
        this.MagicDEF = MagicDEF;
        this.Speed = Speed;
        this.Accuracy = Accuracy;
        this.Evasion = Evasion;
        this.CharId = CharId;
        if (Elemental == null)
            this.Elemental = new int[6] { 0, 0, 0, 0, 0, 0 };
        else
            this.Elemental = Elemental;
        this.Skill = Skill;
    }
}

public class PlayerItem{

    public Item Item;
    public int Quantity;

    public PlayerItem(Item Item, int Quantity){

        this.Item = Item;
        this.Quantity = Quantity;
    }
}

public class ElementDetails{

    public Element Element;
    public Element OpposingElement;
    public string LocalizedName;

    public ElementDetails(Element Element, Element OpposingElement){
        this.Element = Element;
        this.OpposingElement = OpposingElement;
    }
}

public class Condition{

    public string Name;
    public bool Positive;
    public ConditionType Type;
    public float Modifier; //also used to point taunt target index!!!
    public int Duration;
    public bool ActiveOnCounter; //if it should activate on counters (usually used by Surges)
    public bool SingleInstance; //removed when triggered
    public bool Cumulative; //cummulative conditions dont expire and sum up the current values

    public Condition(string Name, bool Positive, ConditionType Type, float Modifier, int Duration, bool SingleInstance = false, bool ActiveOnCounter = true, bool Cumulative = false){

        this.Name = Name;
        this.Positive = Positive;
        this.Type = Type;
        this.Modifier = Modifier;
        this.Duration = Duration;
        this.ActiveOnCounter = ActiveOnCounter;
        this.SingleInstance = SingleInstance;
        this.Cumulative = Cumulative;
    }
}

[Serializable]
public class GameSave{

    public string GameVersion;
    public string Scene;
    public Switchs Switchs;
    public List<SaveCharacter> SaveCharacters;
    public List<SaveItem> SaveInventory;

    public GameSave(string GameVersion, string Scene, Switchs Switchs, List<SaveCharacter> SaveCharacters, List<SaveItem> SaveInventory){

        this.GameVersion = GameVersion;
        this.Scene = Scene;
        this.Switchs = Switchs;
        this.SaveCharacters = SaveCharacters;
        this.SaveInventory = SaveInventory;
    }
}