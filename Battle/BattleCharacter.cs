using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleCharacter : MonoBehaviour {

    BattleController battleController;

    public int Level;
    public bool FrontRow;
    public int HPCurrent;
    public int HPMax;
    public int HP { get { return HPCurrent; } set { HPCurrent = Mathf.Clamp(value, 0, HPMax); HPBar.localScale = new Vector2((float)HPCurrent * 100 / (float)HPMax / 100, 1); } }
    public int Attack;
    public int MagicATK;
    public int Defense;
    public int MagicDEF;
    public int Speed;
    public int[] Elemental;
    public int Accuracy;
    public int Evasion;
    public List<Skill> ActiveSkills = new List<Skill>();
    public List<Skill> PassiveSkills = new List<Skill>();

    //COMBAT ONLY, NO INSTATIATION
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public Transform conditionsCanvas;
    public Vector2 startPosition;
    public RectTransform ATBBar;
    public RectTransform HPBar;
    float ATBCurrent;
    public float ATB { get { return ATBCurrent; } set { ATBCurrent = value; ATBBar.localScale = new Vector2(Mathf.Clamp(ATBCurrent, 0, 10) / 10, 1); } }
    public string latestSkillName;

    public List<Condition> conditionsList = new List<Condition>();

    public void InstantiateCharacter(Character character, string charTag){

        battleController = FindObjectOfType<BattleController>();

        tag = charTag; ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// PRECISA DA TAG!?!?!?!?
        animator.runtimeAnimatorController = FindObjectOfType<Database>().Animators.Find(x => x.Name == character.Name).Animator;
        name = character.Name;
        Level = character.Level;
        FrontRow = character.FrontRow;
        HPMax = character.HPMax;
        HP = character.HPCurrent;
        Attack = character.Attack;
        MagicATK = character.MagicATK;
        Defense = character.Defense;
        MagicDEF = character.MagicDEF;
        Speed = character.Speed;
        Elemental = character.Elemental;
        Accuracy = character.Accuracy;
        Evasion = character.Evasion;

        foreach (Skill skill in character.Skills){
            if (skill.Status != SkillStatus.Active)
                continue;

            if (skill.Type == SkillType.Active)
                ActiveSkills.Add(skill);
            else{
                PassiveSkills.Add(skill);
                if (skill.Name == "Cover")
                    battleController.charWithCover = this;
                if (skill.SpecialEffectTrigger == SpecialEffectTrigger.CounterGlobal)                
                    battleController.globalCounterSkills.Add(new GlobalSkill(skill, this));
            }
        }
        /*Resistance = character.Resistance;
        Weakness = character.Weakness;*/
        
        if (tag == "Ally"){
            Item equipmentAttributes = character.GetEquipmentsAttributes();
            Attack += equipmentAttributes.Attack;
            MagicATK += equipmentAttributes.MagicATK;
            Defense += equipmentAttributes.Defense;
            MagicDEF += equipmentAttributes.MagicDEF;
            Speed += equipmentAttributes.Speed;
            Accuracy += equipmentAttributes.Accuracy;
            Evasion += equipmentAttributes.Evasion;
            Elemental[0] = Mathf.Clamp(Elemental[0] + equipmentAttributes.Elemental[0], -100, 100);
            Elemental[1] = Mathf.Clamp(Elemental[1] + equipmentAttributes.Elemental[1], -100, 100);
            Elemental[2] = Mathf.Clamp(Elemental[2] + equipmentAttributes.Elemental[2], -100, 100);
            Elemental[3] = Mathf.Clamp(Elemental[3] + equipmentAttributes.Elemental[3], -100, 100);
            Elemental[4] = Mathf.Clamp(Elemental[4] + equipmentAttributes.Elemental[4], -100, 100);
            Elemental[5] = Mathf.Clamp(Elemental[5] + equipmentAttributes.Elemental[5], -100, 100);
        }

        if (HP > 0)
            ATB = Random.Range(0, 5);
            //ATB = 9;

        //referenceMonster = monster;
        if (!FrontRow){
            int positionModifier = (tag == "Ally") ? 1 : -1;
            transform.position = new Vector2(transform.position.x + positionModifier, transform.position.y);
        }
        startPosition = transform.position;
    }
}