using UnityEngine;
using UnityEngine.UI;

public class UIButton : MonoBehaviour{
    
    public Text nameText; //used by all
    public Text quantityText; //used by items
    public Text hpText; //used by character
    public Text formationText; //used by character
    public Image buttonImage; //used by skill and character

    //ITEM
    PlayerItem PlayerItem;
    public PlayerItem playerItem {
        get{
            return PlayerItem;
        }
        set{
            PlayerItem = value;
            nameText.text = PlayerItem.Item.LocalizedName;
            quantityText.text = PlayerItem.Quantity.ToString();
        }
    }

    //SKILL
    Skill Skill;
    public Skill skill {
        get{
            return Skill;
        }
        set{
            Skill = value;
            buttonImage.color = Skill.Status == SkillStatus.Active ? Color.green : Color.white;
            nameText.text = Skill.LocalizedName;
        }
    }

    //CHARACTER
    Character Character;
    public Character character {
        get{
            return Character;
        }
        set{
            Character = value;
            nameText.text = Character.Name;
            buttonImage.sprite = FindObjectOfType<Database>().Portraits.Find(x => x.Name == Character.Name).Sprite;
            hpText.text = Character.HPCurrent.ToString("000") + " / " + Character.HPMax.ToString("000");
            formationText.text = Character.FrontRow ? "F" : "B";
        }
    }
}