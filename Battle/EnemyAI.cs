using UnityEngine;

public class EnemyAI : MonoBehaviour {

    Database database;

    void Start(){

        database = FindObjectOfType<Database>();
    }

    public Skill DefineAction(BattleCharacter enemy){

        Skill skill = enemy.ActiveSkills[Random.Range(0, Mathf.Clamp(enemy.ActiveSkills.Count, 0, 3))];
        if (enemy.ActiveSkills.Count > 3 && Random.Range(0, 100) > 80)
            skill = enemy.ActiveSkills[Random.Range(3, enemy.ActiveSkills.Count)];

        if (enemy.latestSkillName == "Charging...")
            skill = enemy.PassiveSkills[0];
        
        Skill currentSkill = new Skill(skill.Id, skill.Name, skill.Type, skill.Range, skill.Element, skill.ConsumeElement, skill.EnemyTarget, skill.DeadTarget,
            skill.AreaTarget, skill.Accuracy, skill.PhysicalDamage, skill.Damage, skill.SpecialEffectTrigger, skill.Status, skill.LocalizedName, skill.Description);

        return currentSkill;
    }
}
