using System.Collections.Generic;
using UnityEngine;

public class Database : MonoBehaviour {
    
    public List<Animators> Animators;
    public List<SpritesList> Portraits;
    public List<SpritesList> ConditionSprites/* = new List<SpritesList>()*/;
    //public List<SpritesList> ItemSpritesList;

    //public List<ElementDetails> Elements = new List<ElementDetails>();
    public List<Condition> Conditions = new List<Condition>();
    public List<List<Skill>> Skills = new List<List<Skill>>();
    List<Skill> Caesar = new List<Skill>();
    List<Skill> Arya = new List<Skill>();
    List<Skill> Serge = new List<Skill>();
    List<Skill> General = new List<Skill>();
    List<Skill> Enemy = new List<Skill>();
    public List<List<Item>> Items = new List<List<Item>>();
    List<Item> Weapons = new List<Item>();
    List<Item> Supports = new List<Item>();
    List<Item> Helmets = new List<Item>();
    List<Item> Armors = new List<Item>();
    List<Item> Boots = new List<Item>();
    List<Item> Accessories = new List<Item>();
    List<Item> Usableitems = new List<Item>();
    List<Item> KeyItems = new List<Item>();
    public List<Character> Characters = new List<Character>(); //this list is just the initial character stats!!!
    public List<Character> Enemies = new List<Character>();

    public Player Player; //instantiated on load by main menu

    void Awake(){

        Skills.Add(Caesar);
        Skills.Add(Arya);
        Skills.Add(Serge);
        Skills.Add(General);
        Skills.Add(Enemy);

        Items.Add(Weapons);
        Items.Add(Supports);
        Items.Add(Helmets);
        Items.Add(Armors);
        Items.Add(Boots);
        Items.Add(Accessories);
        Items.Add(Usableitems);
        Items.Add(KeyItems);


        /*
        IMPORTANT: counter skills must be in priority order!!!

        ACTIVE SKILL EFFECTS -- runs during active skill:
        - none = nothing happens
        - before = ???
        - during = executed for each target AFTER damage
        - post = executed once at the end of turn        

        PASSIVE SKILL EFFECTS -- runs passively on the backgroup:
        - none = nothing happens
        - attack = checked for each target AFTER damage
        - defend = 
        - counter = checked after being targeted (and it attends criterias) and enters a list to be executed after main action
        */

        ////////////////////////////////////////////////////////////////////////////// PLAYER SKILLS //////////////////////////////////////////////////////////////////////////////
        //////////////////////////// CAESAR
        Caesar.Add(new Skill(0, "Defend", SkillType.Active, SkillRange.Self, EnemyTarget: 0, SpecialEffectTrigger: SpecialEffectTrigger.After, Status: SkillStatus.Learned));
        Caesar.Add(new Skill(1, "Crane Strike", SkillType.Active, SkillRange.Melee, Accuracy: 20, Damage: 0.8f, SpecialEffectTrigger: SpecialEffectTrigger.After, Status: SkillStatus.Learned));
        Caesar.Add(new Skill(2, "Tiger Claw", SkillType.Active, SkillRange.Melee, Accuracy: 0, Damage: 0.5f, SpecialEffectTrigger: SpecialEffectTrigger.During, Status: SkillStatus.Learned));
        Caesar.Add(new Skill(3, "Monkey King Paw", SkillType.Active, SkillRange.Melee, Accuracy: 0, Damage: 1, SpecialEffectTrigger: SpecialEffectTrigger.During, Status: SkillStatus.Active));
        Caesar.Add(new Skill(4, "Dragon Assault", SkillType.Active, SkillRange.Melee, Accuracy: -20, Damage: 1.4f, Status: SkillStatus.Active));
        Caesar.Add(new Skill(5, "Tears of Gaia", SkillType.Active, Element: Element.Earth, Accuracy: 0, PhysicalDamage: false, Damage: 1.2f, Status: SkillStatus.None));
        Caesar.Add(new Skill(6, "Terra Blast", SkillType.Active, SkillRange.Melee, Element.Earth, Accuracy: 0, Damage: 0.8f, Status: SkillStatus.Active));
        Caesar.Add(new Skill(7, "Jord's Hammer", SkillType.Active, SkillRange.Melee, Element.Earth, Accuracy: 0, Damage: 1f, SpecialEffectTrigger: SpecialEffectTrigger.After, Status: SkillStatus.None));
        Caesar.Add(new Skill(8, "Seth's Dust", SkillType.Active, Element: Element.Earth, AreaTarget: true, Accuracy: 0, PhysicalDamage: false, SpecialEffectTrigger: SpecialEffectTrigger.During, Status: SkillStatus.None));
        Caesar.Add(new Skill(9, "Danu's Blessing", SkillType.Active, Element: Element.Earth, EnemyTarget: 0, PhysicalDamage: false, SpecialEffectTrigger: SpecialEffectTrigger.During, Status: SkillStatus.None));
        Caesar.Add(new Skill(10, "Atlas Wrath", SkillType.Active, SkillRange.Melee, Element.Earth, true, Accuracy: 20, Damage: 0.8f, Status: SkillStatus.None));
        Caesar.Add(new Skill(11, "Titan", SkillType.Active, SkillRange.Self, Element.Earth, true, 0, SpecialEffectTrigger: SpecialEffectTrigger.After, Status: SkillStatus.None));

        Caesar.Add(new Skill(12, "Counter", SkillType.Passive, SpecialEffectTrigger: SpecialEffectTrigger.Counter, Status: SkillStatus.Learned));
        Caesar.Add(new Skill(13, "Combo", SkillType.Passive, SpecialEffectTrigger: SpecialEffectTrigger.During, Status: SkillStatus.None));
        Caesar.Add(new Skill(14, "Heavy Hits", SkillType.Passive, SpecialEffectTrigger: SpecialEffectTrigger.During, Status: SkillStatus.Active));
        Caesar.Add(new Skill(15, "Cover", SkillType.Passive, Status: SkillStatus.None)); //logic inside ExecuteAction()
        Caesar.Add(new Skill(16, "Last Stand", SkillType.Passive, SpecialEffectTrigger: SpecialEffectTrigger.CounterOnDeath, Status: SkillStatus.Learned));
        Caesar.Add(new Skill(17, "Fortitude", SkillType.Passive, Status: SkillStatus.None)); //logic inside ApplyCondition()

        Caesar.Add(new Skill(18, "Revenge", SkillType.Surge, SpecialEffectTrigger: SpecialEffectTrigger.Counter, Status: SkillStatus.Active));
        Caesar.Add(new Skill(19, "Avenge", SkillType.Surge, SpecialEffectTrigger: SpecialEffectTrigger.CounterGlobal, Status: SkillStatus.None));
        Caesar.Add(new Skill(20, "Nature's Will", SkillType.Surge, SpecialEffectTrigger: SpecialEffectTrigger.Counter, Status: SkillStatus.None));
        

        //////////////////////////// ARYA
        Arya.Add(new Skill(0, "Arya Active 0", SkillType.Active, Damage: 1f, Status: SkillStatus.Active));
        Arya.Add(new Skill(1, "Arya Active 1", SkillType.Active, Status: SkillStatus.Active));
        Arya.Add(new Skill(2, "Arya Active 2", SkillType.Active, Status: SkillStatus.Active));
        Arya.Add(new Skill(3, "Arya Active 3", SkillType.Active, Status: SkillStatus.Active));
        Arya.Add(new Skill(4, "Arya Active 4", SkillType.Active, Status: SkillStatus.Learned));
        Arya.Add(new Skill(5, "Arya Active 5", SkillType.Active, Status: SkillStatus.Learned));
        Arya.Add(new Skill(6, "Arya Active 6", SkillType.Active, Status: SkillStatus.Learned));
        Arya.Add(new Skill(7, "Arya Active 7", SkillType.Active, Status: SkillStatus.Learned));
        Arya.Add(new Skill(8, "Arya Active 8", SkillType.Active));
        Arya.Add(new Skill(9, "Arya Active 9", SkillType.Active));
        Arya.Add(new Skill(10, "Arya Active 10", SkillType.Active));
        Arya.Add(new Skill(11, "Arya Active 11", SkillType.Active));

        Arya.Add(new Skill(16, "Arya Passive 0", SkillType.Passive, Status: SkillStatus.Active));
        Arya.Add(new Skill(17, "Arya Passive 1", SkillType.Passive, Status: SkillStatus.Learned));
        Arya.Add(new Skill(18, "Arya Passive 2", SkillType.Passive, Status: SkillStatus.Learned));
        Arya.Add(new Skill(19, "Arya Passive 3", SkillType.Passive, Status: SkillStatus.Learned));
        Arya.Add(new Skill(20, "Arya Passive 4", SkillType.Passive));
        Arya.Add(new Skill(21, "Arya Passive 5", SkillType.Passive));

        Arya.Add(new Skill(24, "Arya Surge 0", SkillType.Surge, Status: SkillStatus.Active));
        Arya.Add(new Skill(25, "Arya Surge 1", SkillType.Surge, Status: SkillStatus.Learned));
        Arya.Add(new Skill(26, "Arya Surge 2", SkillType.Surge));

        //////////////////////////// SERGE
        Serge.Add(new Skill(0, "Serge Active 0", SkillType.Active, Damage: 1f, Status: SkillStatus.Active));
        Serge.Add(new Skill(1, "Serge Active 1", SkillType.Active, Status: SkillStatus.Active));
        Serge.Add(new Skill(2, "Serge Active 2", SkillType.Active, Status: SkillStatus.Active));
        Serge.Add(new Skill(3, "Serge Active 3", SkillType.Active, Status: SkillStatus.Active));
        Serge.Add(new Skill(4, "Serge Active 4", SkillType.Active, Status: SkillStatus.Learned));
        Serge.Add(new Skill(5, "Serge Active 5", SkillType.Active, Status: SkillStatus.Learned));
        Serge.Add(new Skill(6, "Serge Active 6", SkillType.Active, Status: SkillStatus.Learned));
        Serge.Add(new Skill(7, "Serge Active 7", SkillType.Active, Status: SkillStatus.Learned));
        Serge.Add(new Skill(8, "Serge Active 8", SkillType.Active));
        Serge.Add(new Skill(9, "Serge Active 9", SkillType.Active));
        Serge.Add(new Skill(10, "Serge Active 10", SkillType.Active));
        Serge.Add(new Skill(11, "Serge Active 11", SkillType.Active));

        Serge.Add(new Skill(16, "Serge Passive 0", SkillType.Passive, Status: SkillStatus.Active));
        Serge.Add(new Skill(17, "Serge Passive 1", SkillType.Passive, Status: SkillStatus.Learned));
        Serge.Add(new Skill(18, "Serge Passive 2", SkillType.Passive, Status: SkillStatus.Learned));
        Serge.Add(new Skill(19, "Serge Passive 3", SkillType.Passive, Status: SkillStatus.Learned));
        Serge.Add(new Skill(20, "Serge Passive 4", SkillType.Passive));
        Serge.Add(new Skill(21, "Serge Passive 5", SkillType.Passive));

        Serge.Add(new Skill(24, "Serge Surge 0", SkillType.Surge, Status: SkillStatus.Active));
        Serge.Add(new Skill(25, "Serge Surge 1", SkillType.Surge, Status: SkillStatus.Learned));
        Serge.Add(new Skill(26, "Serge Surge 2", SkillType.Surge));

        //////////////////////////// GENERAL
        General.Add(new Skill(0, "Relocate", Range: SkillRange.Self, EnemyTarget: 0, SpecialEffectTrigger: SpecialEffectTrigger.After));
        General.Add(new Skill(1, "Swap", Range: SkillRange.Self, EnemyTarget: 0, SpecialEffectTrigger: SpecialEffectTrigger.After));
        General.Add(new Skill(2, "Escape", EnemyTarget: 0, SpecialEffectTrigger: SpecialEffectTrigger.After));

        //////////////////////////// ENEMIES
        Enemy.Add(new Skill(0, "Charging...", SkillType.Active, SkillRange.Self, EnemyTarget: 0, Status: SkillStatus.Active));
        Enemy.Add(new Skill(1, "Poison Sting", SkillType.Active, SkillRange.Melee, Accuracy: 0, Damage: 1f, SpecialEffectTrigger: SpecialEffectTrigger.During, Status: SkillStatus.Active));
        Enemy.Add(new Skill(2, "Water Bullet", SkillType.Active, SkillRange.Ranged, Element.Water, Accuracy: 0, PhysicalDamage: false, Damage: 1f, Status: SkillStatus.Active));
        Enemy.Add(new Skill(3, "Slime Tackle", SkillType.Active, SkillRange.Melee, Accuracy: -15, Damage: 1f, SpecialEffectTrigger: SpecialEffectTrigger.During, Status: SkillStatus.Active));
        Enemy.Add(new Skill(4, "Earth Bullet", SkillType.Active, SkillRange.Ranged, Element.Earth, Accuracy: -15, PhysicalDamage: false, Damage: 1f, Status: SkillStatus.Active));
        Enemy.Add(new Skill(5, "Slime Rush", SkillType.Active, SkillRange.Melee, Accuracy: 15, Damage: 1f, SpecialEffectTrigger: SpecialEffectTrigger.After, Status: SkillStatus.Active));
        Enemy.Add(new Skill(6, "Wind Bullet", SkillType.Active, SkillRange.Ranged, Element.Wind, Accuracy: 15, PhysicalDamage: false, Damage: 1f, Status: SkillStatus.Active));
        
        Enemy.Add(new Skill(7, "Water Cannon", SkillType.Passive, SkillRange.Ranged, Element.Water, Accuracy: 0, PhysicalDamage: false, Damage: 2f, Status: SkillStatus.Active)); //passiva pra não entrar no random do skill cast normal
        
        //////////////////////////// KID
        //Skills.Add(new Skill("Lightning Strike", Element.Light, 1));
        //Skills.Add(new Skill("Laser Blade", Element.Light, 1));
        //Skills.Add(new Skill("Take Cover")); //escolhe personagem pra bloquear ataques a ela?
        //Skills.Add(new Skill("Healing Touch", SPCost: 2, EnemyTarget: 0, PhysicalDamage: false, Damage: -1, Element: Element.Light));
        //Skills.Add(new Skill("Illusion Decoy", Element.Light, 1));

        //////////////////////////// INNKEEPER
        //Skills.Add(new Skill("Sonic Arrow", 1, 1)); //generates 1 wind
        //Skills.Add(new Skill("Supersonic Arrow", 1, 1));

        //////////////////////////// WATER DRUID
        //Skills.Add(new Skill("Water Shield", ActionType.Skill, SPCost: 2, EnemyTarget: 0, SelfTarget: true, Element: Element.Water, ConditionID: ConditionID.WaterShield));

        //////////////////////////// FIRE MAGE
        //Skills.Add(new Skill("Combust", Element.Fire, Action: SkillAction.Before)); //converts 1 water into 1 fire no damage
        //Skills.Add(new Skill("Lava Burst", Element.Fire, 1, false, Action: SkillAction.Before)); //converts 1 earth into 1 fire before damage
        //Skills.Add(new Skill("Red Giant", Element.Fire, 3, false, SpecialEffectTrigger: SpecialEffectTrigger.After)); //consumes all fire


        ///////////////////////////////////////////////////////////////////////////////// ITEMS /////////////////////////////////////////////////////////////////////////////////
        //////////////////////////// SPECIAL EFFECTS (they also use SKILL class)

        //////////////////////////// USABLE
        Usableitems.Add(new Item(0, "Potion", ItemType.Usable, Skill: new Skill(9999, "Potion", EnemyTarget: 0, Damage: -100)));
        Usableitems.Add(new Item(1, "Antidote", ItemType.Usable, Skill: new Skill(9998, "Antidote", EnemyTarget: 0, SpecialEffectTrigger: SpecialEffectTrigger.During)));
        //AREA TARGET NAO TA IMPLEMENTADO NO MENU
        //AREA TARGET NAO TA IMPLEMENTADO NO MENU
        //ENEMY TARGET = 1 NAO TA IMPLEMENTADO NO MENU
        //ENEMY TARGET = 1 NAO TA IMPLEMENTADO NO MENU

        //////////////////////////// KEYS
        KeyItems.Add(new Item(0, "Family Memento", ItemType.Key));
        KeyItems.Add(new Item(1, "Hammer", ItemType.Key));
        KeyItems.Add(new Item(2, "Planks", ItemType.Key));
        KeyItems.Add(new Item(3, "Oar", ItemType.Key));
        KeyItems.Add(new Item(4, "Father's Memento", ItemType.Key));
        KeyItems.Add(new Item(5, "Dark Orb A", ItemType.Key));
        KeyItems.Add(new Item(6, "Dark Orb B", ItemType.Key));
        KeyItems.Add(new Item(7, "Dark Orb C", ItemType.Key));

        //////////////////////////// WEAPONS
        Weapons.Add(new Item(0, "Old Gloves", ItemType.Weapon, Attack: 1, CharId: 0 /*, new int[6] { 60, -40, 0, 0, -60, 40 }*/));
        Weapons.Add(new Item(1, "Leather Gloves", ItemType.Weapon, Attack: 3, CharId: 0));
        /*Weapons.Add(new Item(2, "Old Dagger", ItemType.Weapon, 2, 0, 0, 0, 0, 0, 0, 1));
        Weapons.Add(new Item(3, "Iron Dagger", ItemType.Weapon, 4, 0, 0, 0, 0, 0, 0, 1));
        Weapons.Add(new Item(4, "Old Bow", ItemType.Weapon, 4, 0, 0, 0, 0, 0, -4, 2));
        Weapons.Add(new Item(5, "Iron Bow", ItemType.Weapon, 8, 0, 0, 0, 0, 0, -4, 2));*/

        //////////////////////////// SUPPORT
        Supports.Add(new Item(0, "Old Shield", ItemType.Support, Defense: 2, Evasion: -1, CharId: 0 /*, new int[6] { 60, -40, 0, 0, -60, 40 }*/));
        /*Supports.Add(new Item(1, "Old Off Dagger", ItemType.Support, 2, 0, 0, 0, 0, 0, 0, 1));
        Supports.Add(new Item(2, "Old Arrow", ItemType.Support, 1, 0, 0, 0, 0, 5, 0, 2));*/

        //////////////////////////// HELMET
        Helmets.Add(new Item(0, "Old Helmet", ItemType.Helmet, MagicDEF: 2, Evasion: -1, CharId: 0));

        //////////////////////////// ARMOR
        Armors.Add(new Item(0, "Old Armor", ItemType.Armor, Defense: 2, MagicDEF: 2, Evasion: -2, CharId: 0));

        //////////////////////////// BOOTS
        Boots.Add(new Item(0, "Old Boots", ItemType.Boots, Defense: 1, Speed: 1, CharId: 0));

        //////////////////////////// ACCESSORIES
        Accessories.Add(new Item(0, "Magic Ring", ItemType.Accessory, MagicDEF: 10, CharId: 0));

        ///////////////////////////////////////////////////////////////////////// CHARACTERS - 80 POINTS /////////////////////////////////////////////////////////////////////////
        Characters.Add(new Character(0, "Caesar", true, 200, 200, 20, 14, 20, 14, 12, new int[6] { 0, 0, -50, 50, 0, 0 }, Skills: Skills[0]));
        Characters.Add(new Character(1, "Arya", false, 100, 100, 6, 1, 1, 2, 1, new int[6] { 0, 0, 0, 0, 50, -50 }, Skills: Skills[1])); //DAGGERS - LIGHT
        Characters.Add(new Character(2, "Serge", false, 100, 100, 6, 1, 1, 2, 1, new int[6] { 0, 0, 50, -50, 0, 0 }, Skills: Skills[2])); //BOW - WIND
        /*Characters.Add(new Character("Water Druid Trapped in Forest", 10, 10, 6, 1, 1, 2, 8, Skills.FindAll(x => x.Name == "Protect Me" || x.Name == "Healing Touch"), Element.Light, Element.Dark)); WAND
        Characters.Add(new Character("Strongest Fire Mage in the World", 10, 10, 6, 1, 1, 2, 8, Skills.FindAll(x => x.Name == "Protect Me" || x.Name == "Healing Touch"), Element.Light, Element.Dark)); STAFF*/

        //////////////////////////////////////////////////////////////////////////////// ENEMIES ////////////////////////////////////////////////////////////////////////////////
        Enemies.Add(new Character(1, "Water Slime", false, 22, 22, /*42,*/ 16, 18, 16, 14, 16, new int[6] { -100, 100, 0, 0, 0, 0 }, Evasion: 10, Level: 6, Skills: new List<Skill> { Skills[4][1], Skills[4][2] }));
        Enemies.Add(new Character(2, "Earth Slime", true, 27, 27, /*14,*/ 18, 14, 24, 12, 12, new int[6] { 0, 0, -100, 100, 0, 0 }, Level: 6, Skills: new List<Skill> { Skills[4][3], Skills[4][4] }));
        Enemies.Add(new Character(3, "Wind Slime", true, 17, 17, /*28,*/ 14, 16, 14, 16, 20, new int[6] { 0, 0, 100, -100, 0, 0 }, Evasion: 20, Level: 6, Skills: new List<Skill> { Skills[4][5], Skills[4][6] }));

        Enemies.Add(new Character(4, "Water Lizard", true, 200, 200, /*50,*/ 10, 16, 16, 16, 12, new int[6] { -50, 50, 0, 0, 0, 0 }, Level: 12, Skills: new List<Skill> { Skills[4][1], Skills[4][2], Skills[4][3], Skills[4][7], Skills[4][0] }));
    }
}