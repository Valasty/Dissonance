using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillController : MonoBehaviour{

    public BattleController battleController;
    GeneralFunctions general;

    void Start(){

        general = FindObjectOfType<GeneralFunctions>();
    }

    public Skill GetCounterSkill(Skill passiveSkill, Skill attackerSkill, BattleCharacter defender){

        Skill CounterSkill = null;

        switch (passiveSkill.Name){
            case "Last Stand":
                if (!defender.conditionsList.Exists(x => x.Name == passiveSkill.Name)){
                    CounterSkill = new Skill(0, passiveSkill.Name, Range: SkillRange.Self, EnemyTarget: 0, DeadTarget: true, Damage: -0.001f, SpecialEffectTrigger: SpecialEffectTrigger.After,
                        LocalizedName: passiveSkill.LocalizedName);
                }
                break;
            case "Counter":
                if (!attackerSkill.AreaTarget && attackerSkill.Range != SkillRange.Ranged)
                    CounterSkill = new Skill(0, passiveSkill.Name, Range: SkillRange.Melee, Accuracy: 0, Damage: 1f, LocalizedName: passiveSkill.LocalizedName);
                break;
            case "Revenge":
                if (!defender.conditionsList.Exists(x => x.Name == passiveSkill.Name) && attackerSkill.Damage > 0 && battleController.lastDamageCaused >= defender.HPMax / 2)
                    CounterSkill = new Skill(0, passiveSkill.Name, Range: SkillRange.Self, EnemyTarget: 0, SpecialEffectTrigger: SpecialEffectTrigger.After, LocalizedName: passiveSkill.LocalizedName);
                break;
            case "Nature's Will":
                if (defender.conditionsList.FindAll(x => x.Positive == false).Count > 2)
                    CounterSkill = new Skill(0, passiveSkill.Name, Range: SkillRange.Self, Element: Element.Earth, EnemyTarget: 0, SpecialEffectTrigger: SpecialEffectTrigger.After, LocalizedName: passiveSkill.LocalizedName);
                break;
        }
        return CounterSkill;
    }

    public Skill GetGlobalCounterSkill(GlobalSkill globalSkill, BattleCharacter defender){

        Skill CounterSkill = null;

        switch (globalSkill.Skill.Name){            
            case "Avenge":
                if (defender != globalSkill.Character && defender.HP > 0 && defender.HP <= defender.HPMax / 5)
                    CounterSkill = new Skill(0, globalSkill.Skill.Name, Range: SkillRange.Self, EnemyTarget: 0, SpecialEffectTrigger: SpecialEffectTrigger.After, LocalizedName: globalSkill.Skill.LocalizedName);
                break;
        }
        return CounterSkill;
    }

    //RUNS FOR EACH TARGET
    public void ExecuteDuringSkillEffect(Skill currentSkill, BattleCharacter attacker, BattleCharacter defender){

        switch (currentSkill.Name){
            //CAESAR
            case "Tiger Claw":
                battleController.ApplyCondition(new Condition(currentSkill.Name, false, ConditionType.DMGTakenMod, 0.5f, 2, true), defender);
                break;
            case "Monkey King Paw": //WILL NOT WORK IF CAST BY AN ENEMY!!!
                battleController.ApplyCondition(new Condition(currentSkill.Name, false, ConditionType.Taunt, battleController.charactersInBattle[0].IndexOf(attacker), 1), defender);
                break;
            case "Seth's Dust":
                battleController.ApplyCondition(new Condition(currentSkill.Name, false, ConditionType.HitCheck, -25 * battleController.ElementalModifier(), 3), defender);
                break;
            case "Danu's Blessing":                
                battleController.ApplyCondition(new Condition(currentSkill.Name, true, ConditionType.DoT, battleController.GetAttackerDamage(attacker, -0.5f), 3), defender);
                break;
            case "Combo":
                if (battleController.lastDamageCaused > 0)
                    battleController.ApplyCondition(new Condition(attacker.name + "Combo", false, ConditionType.Combo, 0.05f, 1, Cumulative: true), defender);
                break;
            case "Heavy Hits":
                if (battleController.lastDamageCaused > 0){
                    if (defender.ATB >= 10)
                        battleController.actionSelectQueue.Remove(defender);
                    defender.ATB = Mathf.Clamp(defender.ATB, 2, 10) - 2;
                }
                break;

            //MONSTER
            case "Poison Sting":
                battleController.ApplyCondition(new Condition(currentSkill.Name, false, ConditionType.DoT, battleController.GetAttackerDamage(attacker, 1f), 3), defender);
                break;
            case "Slime Tackle":
                if (defender.ATB >= 10)
                    battleController.actionSelectQueue.Remove(defender);
                defender.ATB = Mathf.Clamp(defender.ATB, 2, 10) - 2;                
                break;

            //ITEMS
            case "Antidote":                    
                battleController.RemoveCondition(defender.conditionsList.Find(x => x.Name == "Poison Sting"), defender);
                break;
        }
    }

    //RUNS A SINGLE TIME AFTER ACTION
    public void ExecuteAfterSkillSelfEffect(Skill currentSkill, BattleCharacter attacker){
        
        switch (currentSkill.Name){
            //GENERAL
            case "Relocate":
                int moveDistance = attacker.FrontRow ? 1 : -1;
                if (attacker.tag == "Enemy")
                    moveDistance *= -1;
                attacker.FrontRow = !attacker.FrontRow;
                attacker.startPosition = new Vector2(attacker.transform.position.x + moveDistance, attacker.transform.position.y);
                battleController.CharacterMovement(attacker, "Retreat");
                break;
            case "Swap":
                break;
            case "Escape":
                break;

            //CAESAR
            case "Defend":
                battleController.ApplyCondition(new Condition(currentSkill.Name, true, ConditionType.DMGTakenMod, -0.5f, 2), attacker);
                break;
            case "Crane Strike":
                battleController.ApplyCondition(new Condition(currentSkill.Name, true, ConditionType.DMGTakenMod, -0.2f, 2), attacker);
                break;
            case "Jord's Hammer":
                battleController.ApplyCondition(new Condition(currentSkill.Name, false, ConditionType.DMGTakenMod, 0.5f, 2, true), attacker);
                break;
            case "Titan":
                battleController.ApplyCondition(new Condition(currentSkill.Name, true, ConditionType.Titan, battleController.elements[(int)currentSkill.Element] * 0.1f, 6), attacker);
                break;
            case "Last Stand":
                battleController.ApplyCondition(new Condition(currentSkill.Name, false, ConditionType.Marker, 0, 6), attacker);
                attacker.HP = 1;
                break;
            case "Revenge":
                battleController.ApplyCondition(new Condition(currentSkill.Name, true, ConditionType.DMGDealtMod, 0.5f, 1, true, false), attacker);
                break;
            case "Avenge":
                battleController.ApplyCondition(new Condition(currentSkill.Name, true, ConditionType.AreaTargetMod, 0, 1, true, false), attacker);
                break;
            case "Nature's Will":
                for (int i = attacker.conditionsList.Count - 1; i >= 0; i--)
                    battleController.RemoveCondition(attacker.conditionsList[i], attacker);
                break;

            //MONSTERS
            case "Slime Rush":
                battleController.ApplyCondition(new Condition(currentSkill.Name, true, ConditionType.HitCheck, 25, 2), attacker);
                break;
        }
    }
}