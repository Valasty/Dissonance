using UnityEngine;

public class PlayerController : MonoBehaviour {

    public GeneralFunctions general;
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public Collider2D collider2d;
    public LayerMask toolLayer;
    public GameObject menuCanvas;
    
    const float speed = 4;
    Vector2 movement;
    public Vector2 lastDirection;

    public bool eventHappening = false;

    Collider2D areaCollider;
    public GameObject exclamationBubble;    

    void FixedUpdate () {
        
        if (eventHappening)
            return;

        ///////////////////////////////////
        /////// MOVEMENT CONTROLLER ///////
        ///////////////////////////////////
        movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        animator.SetFloat("Velocity", movement.magnitude);

        if (movement.x != 0){
            lastDirection = new Vector2(movement.x, 0);
            animator.SetFloat("Horizontal Direction", movement.x);
            animator.SetFloat("Vertical Direction", lastDirection.y);
        }
        if (movement.y != 0){
            lastDirection = new Vector2(0, movement.y);
            animator.SetFloat("Horizontal Direction", lastDirection.x);
            animator.SetFloat("Vertical Direction", movement.y);
        }
        transform.Translate(movement * speed * Time.deltaTime);        
    }

    void Update(){

        if (eventHappening) //QUALQUER REGRA QUE POE AQUI TEM QUE POR ALI EMBAIXO NO ONTRIGGERENTER
            return;

        //////////////////////////////////
        //// GENERAL INPUT CONTROLLER ////
        //////////////////////////////////
        if (Input.GetButtonDown("Confirm")){
            if (areaCollider != null){

                //look at
                Vector2 normalized = (areaCollider.transform.position - transform.position).normalized;
                if (Mathf.Abs(normalized.x) > Mathf.Abs(normalized.y)){
                    animator.SetFloat("Horizontal Direction", normalized.x);
                    animator.SetFloat("Vertical Direction", 0);
                }
                else{
                    animator.SetFloat("Horizontal Direction", 0);
                    animator.SetFloat("Vertical Direction", normalized.y);
                }

                //start event
                eventHappening = true;
                StartCoroutine(areaCollider.GetComponentInParent<IInteractable>().Interact());
            }
        }

        ////////////////// TOOL //////////////////
        if (Input.GetButtonDown("Tool")){
            RaycastHit2D hit = Physics2D.Raycast(collider2d.bounds.center, lastDirection, 0.4f, toolLayer);
            if (hit.collider != null)
                StartCoroutine(hit.collider.GetComponent<PushableObject>().Push(lastDirection, this));
        }

        ////////////////// MENU //////////////////
        if (Input.GetButtonDown("Menu")){
            general.audioController.PlayMenuOpenSound();
            general.audioController.enableButtonSound = true;
            eventHappening = true;
            menuCanvas.SetActive(true);
        }
    }

    //all but interactables
    void OnTriggerEnter2D(Collider2D collider){

        switch (collider.tag){
            case "Event":
                general.playerController.eventHappening = true;
                StartCoroutine(collider.GetComponent<IInteractable>().Interact());
                break;
            case "Transition":
                StartCoroutine(general.LoadScene(collider.name));
                break;
            /*case "Event Transition":
                general.eventSequence = collider.transform.parent.name;
                StartCoroutine(general.LoadScene(collider.name));
                break;*/
            /*case "Big Event Transition":
                general.eventSequence = collider.transform.parent.name;
                //StartCoroutine(general.LoadScene(collider.name, fadeOutTime: 3));
                break;*/
        }
    }

    //interactables only
    void OnTriggerStay2D(Collider2D collision){

        if (collision.tag != "Interactable")
            return;

        if (eventHappening){
            if (exclamationBubble.activeInHierarchy)
                exclamationBubble.SetActive(false);            
        }
        else{
            if (!exclamationBubble.activeInHierarchy){
                areaCollider = collision;
                exclamationBubble.SetActive(true);
            }
        }        
    }

    //interactables only
    void OnTriggerExit2D(Collider2D collider){

        if (collider.tag == "Interactable"){
            areaCollider = null;
            exclamationBubble.SetActive(false);
        }
    }
}