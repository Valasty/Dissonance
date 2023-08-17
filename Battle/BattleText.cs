using UnityEngine;
using UnityEngine.UI;

public class BattleText : MonoBehaviour {

    Text battleText;
    bool animateText = false;
    int textDuration = 1;

    void Awake(){

        battleText = GetComponent<Text>();
    }

    void Update(){

        if (animateText)
            gameObject.transform.Translate(Vector2.up * Time.deltaTime);
    }

    public void ProcessText(string text, Vector2 position = new Vector2(), float elementalMultiplier = 1){
        
        if (position == Vector2.zero)
            battleText.rectTransform.position = new Vector2(0, 3);         
        else{
            animateText = true;
            //textDuration = 1;
            battleText.rectTransform.position = position;
            if (text.Substring(0, 1) == "-"){
                text = text.Remove(0, 1);
                battleText.color = Color.green;
            }
            else if (elementalMultiplier < 1)
                battleText.color = Color.yellow;
            else if (elementalMultiplier > 1)
                battleText.color = Color.red;
        }

        battleText.text = text;
        Destroy(gameObject, textDuration);
    }
}