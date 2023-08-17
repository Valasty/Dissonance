using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactionBubble : MonoBehaviour{

    float t;

    void Update(){

        t += Time.deltaTime;
        if (t > 0.7f)
            Destroy(gameObject);
    }
}