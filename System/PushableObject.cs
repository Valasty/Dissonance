using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushableObject : MonoBehaviour {

    public float distance; //set this on Inspector!!!
    public Rigidbody2D rigidBody;
    
    public IEnumerator Push(Vector2 lastDirection, PlayerController player){
        
        player.eventHappening = true;
        player.animator.SetFloat("Velocity", 0);

        rigidBody.AddForce(lastDirection * 10000);
        yield return new WaitForSeconds(0.1f); //necessary because the speed is not instantly computed

        while (Vector2.Distance(player.transform.position, transform.position) < distance && rigidBody.velocity != Vector2.zero)
            yield return null;
        rigidBody.velocity = Vector2.zero;

        player.eventHappening = false;
    }
}