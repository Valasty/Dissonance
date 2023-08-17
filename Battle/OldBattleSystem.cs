using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OldBattleSystem : MonoBehaviour{

    /*public EnemyAI enemyAI;
    Database database;
    GeneralFunctions generalFunctions;
    LocalizationController localization;

    public Transform[] allySpawnLocations;
    public Transform[] enemySpawnLocations;
    public Text skillDescriptionText;
    public Text battleEndingText;
    public Text[] charNameText;
    public Text[] charHPText;
    public Text[] charSPText;
    public RawImage[] elementList = new RawImage[5];
    public GameObject mainCanvas;
    public GameObject actionCanvas;
    public GameObject skillCanvas;
    public GameObject targetObject;
    public Transform battleCanvas;
    public GameObject battleTextPrefab;
    public GameObject characterPrefab;
    public GameObject ATBBarPrefab;
    public GameObject buttonPrefab;
    public GameObject conditionPrefab;
    
    int maximumSpeed;
    const int baseMissChance = 10; //%
    const float missChancePerSpeed = 0.5f; //each Speed difference adds/removes 5% chance    
    int Experience;

    List<List<BattleCharacter>> charactersInBattle = new List<List<BattleCharacter>>();
    List<BattleCharacter> actionSelectQueue = new List<BattleCharacter>();
    BattleText skillText;
    BattleCharacter currentCharacter;
    ActionType actionType = ActionType.None;
    Skill actionSkill;
    int currentTargetIndex;
    public int enemyTarget;
    const float charMoveSpeed = 0.2f;

    bool enableTargetInput = false;
    bool actionHappening = false;
    bool inputCooldown;

    void Awake(){

        database = FindObjectOfType<Database>();
        generalFunctions = FindObjectOfType<GeneralFunctions>();
        localization = FindObjectOfType<LocalizationController>();
        battleCanvas.GetComponent<Canvas>().worldCamera = Camera.main;
    }

    void Start(){

        //UI LOAD
        generalFunctions.ResetCamera();
        GameObject[] battleButtons = GameObject.FindGameObjectsWithTag("UI Button");
        for (int i = 0; i < battleButtons.Length; i++){
            battleButtons[i].GetComponentInChildren<Text>().text = localization.buttonTexts[i];
        }
        actionCanvas.SetActive(false);
        InitialElementSetup();
        SpawnCharacters();
    }

    void SpawnCharacters(){

        ///////// SPAWNS ALLIES /////////
        charactersInBattle.Add(new List<BattleCharacter>());
        for (int i = 0; i < database.Player.Characters.Count; i++) {
            charactersInBattle[0].Add((Instantiate(characterPrefab, allySpawnLocations[i].position, Quaternion.identity).GetComponent<BattleCharacter>()));
            charactersInBattle[0][i].InstantiateCharacter(database.Player.Characters[i], "Ally");
            if (charactersInBattle[0][i].Speed > maximumSpeed)
                maximumSpeed = charactersInBattle[0][i].Speed;
        }

        ///////// SPAWNS ENEMIES /////////
        charactersInBattle.Add(new List<BattleCharacter>());
        for (int i = 0; i < generalFunctions.battleEnemies.Count; i++){
            charactersInBattle[1].Add((Instantiate(characterPrefab, enemySpawnLocations[i].position, Quaternion.identity).GetComponent<BattleCharacter>()));
            charactersInBattle[1][i].InstantiateCharacter(generalFunctions.battleEnemies[i], "Enemy");
            if (charactersInBattle[1][i].Speed > maximumSpeed)
                maximumSpeed = charactersInBattle[1][i].Speed;
        }
    }

    void InitialElementSetup(){

        for (int i = 0; i < elementList.Length; i++){
            elementList[i].color = generalFunctions.sceneElements[i];
        }
    }

    void Update(){
        
        CharacterController();
        ///////// ACTIONS CONTROLLER /////////
        if (actionSelectQueue.Count > 0 && !actionHappening){
            actionHappening = true;
            if (actionSelectQueue[0].tag == "Ally")
                AllyActionSelect(actionSelectQueue[0]);
            else
                EnemyActionSelect(actionSelectQueue[0]);
            actionSelectQueue.RemoveAt(0);
        }

        if (skillCanvas.activeInHierarchy && Input.GetButtonDown("Cancel"))
            CancelSkill();
        
        ///////// TARGETING LOGIC STARTS /////////
        if (!enableTargetInput || charactersInBattle[enemyTarget].Count == 0)
            return;

        if (actionSkill.AreaTarget){
            currentTargetIndex = (currentTargetIndex  >= charactersInBattle[enemyTarget].Count - 1) ? 0 : currentTargetIndex + 1;
            targetObject.transform.position = charactersInBattle[enemyTarget][currentTargetIndex].transform.position;
        }

        ///////// TARGET INPUT /////////
        if (Input.GetButtonDown("Confirm") && InputCooldown())
            SubmitTarget();
        if (Input.GetButtonDown("Cancel"))        
            CancelTarget();
        if (Input.GetButtonDown("Horizontal") && !actionSkill.SelfTarget)
            ChangeTarget(Input.GetAxisRaw("Horizontal"));
        if (Input.GetButtonDown("Vertical") && !actionSkill.SelfTarget)
            ChangeTarget(-Input.GetAxisRaw("Vertical"));
    }

    bool InputCooldown(){

        ///////// MAKES SURE SUBMIT TARGET IS NOT PRESSED RIGHT AFTER THE BUTTONS /////////
        if (!inputCooldown)
            return true;
        inputCooldown = false;
        return false;
    }

    void CharacterController(){

        ///////// CALLED BY UPDATE EVERY FRAME /////////

        if (actionHappening)
            return;

        ///////// ALLIES /////////
        foreach (BattleCharacter character in charactersInBattle[0]){
            
            //atb - WHEN ATB REACHES 10, BECOMES 11 AND GOES TO THE QUEUE
            if (character.HP > 0 && !character.conditionsList.Exists(x => x.Id == ConditionID.Stun)){
                character.ATB += Time.deltaTime * (character.Speed / (maximumSpeed / 2));
                character.ATBBar.transform.localScale = new Vector2((Mathf.Clamp(character.ATB, 0, 10) / 10), 1);
                if (character.ATB > 10 && character.ATB < 11){
                    actionSelectQueue.Add(character);
                    character.ATB = 11;
                }
            }

            //conditions
            if (character.conditionsList.Count == 0)
                continue;
            for (int i = 0; i < character.conditionsList.Count; i++){
                character.conditionsList[i].Timer += Time.deltaTime;
                if (character.conditionsList[i].Timer > character.conditionsList[i].TimerLimit)
                    RemoveCondition(character.conditionsList[i], character);
            }
        }

        ///////// ENEMIES /////////
        foreach (BattleCharacter character in charactersInBattle[1]){
            
            //atb - WHEN ATB REACHES 10, BECOMES 11 AND GOES TO THE QUEUE
            if (!character.conditionsList.Exists(x => x.Id == ConditionID.Stun)){
                character.ATB += Time.deltaTime * (character.Speed / (maximumSpeed / 2));
                character.ATBBar.transform.localScale = new Vector2((Mathf.Clamp(character.ATB, 0, 10) / 10), 1);
                if (character.ATB > 10 && character.ATB < 11){
                    actionSelectQueue.Add(character);
                    character.ATB = 11;
                }
            }

            //conditions
            if (character.conditionsList.Count == 0)
                continue;
            for (int i = 0; i < character.conditionsList.Count; i++){
                character.conditionsList[i].Timer += Time.deltaTime;
                if (character.conditionsList[i].Timer > character.conditionsList[i].TimerLimit)
                    RemoveCondition(character.conditionsList[i], character);
            }
        }
    }
    
    void AllyActionSelect(BattleCharacter character){

        ///////// PROMPTS THE PANEL FOR ACTION SELECTION WHEN ATB BECOMES 11 /////////

        //removes 1 turn conditions
        for (int i = 0; i < character.conditionsList.Count; i++){
            if (character.conditionsList[i].Id == ConditionID.Counter || character.conditionsList[i].Id == ConditionID.Protect)
                RemoveCondition(character.conditionsList[i], character);
        }

        //prompts for action
        //character.ATBBar.color = Color.green;
        currentCharacter = character;
        actionCanvas.SetActive(true);
        EventSystem.current.SetSelectedGameObject(EventSystem.current.firstSelectedGameObject);
    }

    void EnemyActionSelect(BattleCharacter enemy){

        ///////// AUTOMATICALLY SELECTS AND EXECUTES ENEMY ACTION WHEN ATB BECOMES 11 /////////
        //enemy.ATBBar.color = Color.green;
        currentCharacter = enemy;
        
        actionSkill = enemyAI.DefineAction(enemy);
        enemyTarget = (actionSkill.EnemyTarget == 0) ? 1 : 0; //enemy target has to be inverted to support the same skills as the allies
        currentTargetIndex = Random.Range(0, charactersInBattle[enemyTarget].Count);
        while ((!actionSkill.DeadTarget && charactersInBattle[enemyTarget][currentTargetIndex].HP == 0) || (actionSkill.DeadTarget && charactersInBattle[enemyTarget][currentTargetIndex].HP > 0))
            currentTargetIndex = Random.Range(0, charactersInBattle[enemyTarget].Count);
        SubmitTarget();
        //ENEMIES NEED TO UTILIZE SP TO CAST SPELLS!!!




        //DESCOMENTE ISSO E COMENTE O RESTO PRA PULAR A ENEMY ACTION!!!
        /*enemy.ATB = 0;
        actionHappening = false;
        return;
    }

    public void AttackButton(){

        ///////// CALLED ON ATTACK BUTTON PRESS /////////
        actionSkill = database.Skills[0];
        enemyTarget = 1;
        currentTargetIndex = 0;
        inputCooldown = true;
        targetObject.SetActive(true);
        actionCanvas.SetActive(false);
        targetObject.transform.position = charactersInBattle[enemyTarget][currentTargetIndex].transform.position;
        enableTargetInput = true;
    }
    
    public void SkillButton(){

        ///////// CALLED ON SKILL BUTTON PRESS TO OPEN THE SKILL CANVAS /////////
        mainCanvas.SetActive(false);
        Text lastButtonText = null;
        if (EventSystem.current.currentSelectedGameObject.GetComponentInChildren<Text>().text == "Item"){
            List<Item> usablePlayerItems = database.Items.FindAll(x => (x.Type == ItemType.Healing || x.Type == ItemType.Offensive)); //gets only usable items
            string lastItem = null;
            int quantity = 1;
            foreach (Item item in usablePlayerItems){
                if (item.Name != lastItem){
                    quantity = 1;
                    lastButtonText = (lastButtonText == null) ? buttonPrefab.GetComponentInChildren<Text>() : Instantiate(buttonPrefab, skillCanvas.transform).GetComponentInChildren<Text>();
                    lastButtonText.text = item.Name + " x" + quantity.ToString("00");                    
                }
                else{
                    quantity++;
                    lastButtonText.text = item.Name + " x" + quantity.ToString("00");                    
                }
                if (skillCanvas.transform.childCount == 9)
                    break;
                lastItem = item.Name;
            }
            actionType = ActionType.Item;
        }
        else{
            List<Skill> activeSkillsList = currentCharacter.SkillList.FindAll(x => x.ActionType == ActionType.Skill);
            foreach (Skill skill in activeSkillsList){
                lastButtonText = (lastButtonText == null) ? buttonPrefab.GetComponentInChildren<Text>() : Instantiate(buttonPrefab, skillCanvas.transform).GetComponentInChildren<Text>();
                lastButtonText.text = skill.LocalizedName;
                if (skillCanvas.transform.childCount == 9)
                    break;
            }
            actionType = ActionType.Skill;        
        }
        skillCanvas.SetActive(true);
        EventSystem.current.SetSelectedGameObject(skillCanvas.transform.GetChild(0).gameObject);
    }

    public void SkillButtonSelect(){

        ///////// CALLED ON SKILL BUTTON SELECTION TO DISPLAY IT'S DESCRIPTION /////////
        Text buttonText = EventSystem.current.currentSelectedGameObject.GetComponentInChildren<Text>();
        if (actionType == ActionType.Item)
            skillDescriptionText.text = database.Items.Find(x => x.LocalizedName == buttonText.text.Substring(0, buttonText.text.Length - 4)).Description;
        else
            skillDescriptionText.text = database.Skills.Find(x => x.LocalizedName == buttonText.text).Description;
    }

    public void SkillButtonClick() {

        ///////// CALLED ON SKILL BUTTON PRESS /////////
        Text buttonText = EventSystem.current.currentSelectedGameObject.GetComponentInChildren<Text>();
        if (actionType == ActionType.Skill){
            // checks if there's SP to cast skill
            actionSkill = database.Skills.Find(x => x.LocalizedName == buttonText.text);
            if (actionSkill.SPCost > currentCharacter.SP){
                print("Not enough SP!");
                return;
            }
        }
        else
            actionSkill = database.Skills.Find(x => x.LocalizedName == buttonText.text.Substring(0, buttonText.text.Length - 4));
        enemyTarget = actionSkill.EnemyTarget;
        currentTargetIndex = (actionSkill.SelfTarget) ? charactersInBattle[enemyTarget].IndexOf(currentCharacter) : 0;
        inputCooldown = true;
        targetObject.SetActive(true);
        skillCanvas.SetActive(false);
        targetObject.transform.position = charactersInBattle[enemyTarget][currentTargetIndex].transform.position;
        enableTargetInput = true;
    }

    public void DefendButton(){

        ///////// CALLED ON DEFEND BUTTON PRESS /////////
        actionSkill = database.Skills[1];
        enemyTarget = 0;
        currentTargetIndex = charactersInBattle[enemyTarget].IndexOf(currentCharacter);
        inputCooldown = true;
        targetObject.SetActive(true);
        actionCanvas.SetActive(false);
        targetObject.transform.position = charactersInBattle[enemyTarget][currentTargetIndex].transform.position;
        enableTargetInput = true;
    }

    void CancelSkill() {

        ///////// CALLED ON CANCEL BUTTON PRESS ON SELECTING SKILL /////////
        actionSkill = null;
        for (int i = 1; i < skillCanvas.transform.childCount; i++)
            GameObject.Destroy(skillCanvas.transform.GetChild(i).gameObject);
        skillCanvas.SetActive(false);
        mainCanvas.SetActive(true);
        skillDescriptionText.text = null;
        EventSystem.current.SetSelectedGameObject(EventSystem.current.firstSelectedGameObject);
    }
    
    void CancelTarget(){

        ///////// CALLED ON CANCEL BUTTON PRESS ON SELECTING TARGET /////////
        targetObject.SetActive(false);
        if (actionSkill.ActionType == ActionType.Attack || actionSkill.ActionType == ActionType.Defend){
            actionCanvas.SetActive(true);
            EventSystem.current.SetSelectedGameObject(EventSystem.current.firstSelectedGameObject);
        }
        else{
            skillCanvas.SetActive(true);
            EventSystem.current.SetSelectedGameObject(skillCanvas.transform.GetChild(0).gameObject);
        }
        enableTargetInput = false;
    }
        
    void SubmitTarget(){

        ///////// CALLED ON CONFIRM BUTTON PRESS ON SELECTING TARGET /////////
        if (!actionSkill.AreaTarget
            && (!actionSkill.DeadTarget && charactersInBattle[enemyTarget][currentTargetIndex].HP == 0
            || (actionSkill.DeadTarget && charactersInBattle[enemyTarget][currentTargetIndex].HP > 0)
            || (actionSkill.Damage < 0 && charactersInBattle[enemyTarget][currentTargetIndex].HP == charactersInBattle[enemyTarget][currentTargetIndex].HPMax))){
                print("Invalid target!!!");
                return;
        }

        //removes the item from the inventory, or the mana from the character
        if (actionSkill.ActionType == ActionType.Item)
            database.Player.Items.Remove(database.Items.Find(x => x.Name == actionSkill.Id));
        else if (actionSkill.ActionType == ActionType.Skill)
            currentCharacter.SP -= actionSkill.SPCost;

        enableTargetInput = false;
        targetObject.SetActive(false);
        actionCanvas.SetActive(false);
        mainCanvas.SetActive(true);
        for (int i = 1; i < skillCanvas.transform.childCount; i++)
            GameObject.Destroy(skillCanvas.transform.GetChild(i).gameObject);        
        skillDescriptionText.text = null;
        ExecuteAction();
    }
    
    void ChangeTarget(float AxisValue){

        ///////// CALLED ON DIRECTIONALS BUTTONS PRESS ON SELECTING TARGET /////////
        int playerInput = (int)AxisValue;

        if (currentTargetIndex == 0 && playerInput == -1)
            currentTargetIndex = charactersInBattle[enemyTarget].Count - 1;
        else if (currentTargetIndex == charactersInBattle[enemyTarget].Count - 1 && playerInput == 1)
            currentTargetIndex = 0;
        else
            currentTargetIndex += playerInput;

        targetObject.transform.position = charactersInBattle[enemyTarget][currentTargetIndex].transform.position;
    }

    void ExecuteAction(){

        currentCharacter.latestSkill = actionSkill;
        currentCharacter.defending = false;
        currentCharacter.animator.SetBool("Attack", true);
        
        if (actionSkill.Id == "Defend")
            StartCoroutine(DefendAction());
        else
            StartCoroutine(SkillAction());
    }

    IEnumerator DefendAction(){ ///////////////////////////////////////////////////////////// TRANSFORMAR DEFEND EM CONDITION????

        //attack pre effects
        yield return new WaitForSeconds(1f);

        if (actionSkill.Id != "Defend"){
            skillText = Instantiate(battleTextPrefab, battleCanvas).GetComponent<BattleText>();
            skillText.ProcessText("Defend");
        }
        currentCharacter.defending = true; //this is reverted on the next action execution of this target

        yield return StartCoroutine(TargetBlink(currentCharacter));

        //attack post effects
        yield return new WaitForSeconds(1f);
        EndAction(currentCharacter);
    }
       
    IEnumerator SkillAction(){

        //texts controller
        if (actionSkill.Id != "Attack"){
            skillText = Instantiate(battleTextPrefab, battleCanvas).GetComponent<BattleText>();
            skillText.ProcessText(actionSkill.LocalizedName);
        }

        yield return StartCoroutine(CharacterMovement());

        List<BattleCharacter> targetList = new List<BattleCharacter>();
        if (actionSkill.AreaTarget)
            targetList.AddRange(charactersInBattle[enemyTarget]);
        else
            targetList.Add(charactersInBattle[enemyTarget][currentTargetIndex]);

        //damage calculations and defender interactions
        foreach (BattleCharacter defender in targetList){
            //if invalid target, continues to the next
            if ((actionSkill.DeadTarget && defender.HP > 0) || (!actionSkill.DeadTarget && defender.HP == 0))
                continue;

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //COUNTER LOGIC
            if (defender.conditionsList.Exists(x => x.Id == ConditionID.Counter)
                && defender.tag != currentCharacter.tag
                && !actionSkill.AreaTarget
                && actionSkill.PhysicalDamage){
                    actionSkill = database.Skills.Find(x => x.Id == "Counter Action");
                    skillText = Instantiate(battleTextPrefab, battleCanvas).GetComponent<BattleText>();
                    skillText.ProcessText(actionSkill.LocalizedName);
                    yield return new WaitForSeconds(1);
                    DamageApplication(defender, currentCharacter);
                    RemoveCondition(defender.conditionsList.Find(x => x.Id == ConditionID.Counter), defender);
                    StartCoroutine(TargetBlink(currentCharacter));
                    CheckDeath(currentCharacter);
                    continue;
            }            
            //PROTECT LOGIC
            if (defender.conditionsList.Exists(x => x.Id == ConditionID.Protect) && defender.tag != currentCharacter.tag){
                //finds the highest def char
                BattleCharacter newDefender = new BattleCharacter();
                foreach (BattleCharacter character in charactersInBattle[enemyTarget]){
                    if (character.HP > 0 && character.Defense > newDefender.Defense && character != defender)
                        newDefender = character;
                }
                //if there's an available defender, adjusts movements and applies damages accordingly
                if (newDefender.Defense != 0){
                    Vector3 targetPosition = new Vector2();
                    targetPosition = (enemyTarget == 0) ? new Vector2(defender.transform.position.x - 1, defender.transform.position.y) : new Vector2(defender.transform.position.x + 1, defender.transform.position.y);
                    while (newDefender.transform.position != targetPosition){
                        newDefender.transform.position = Vector2.MoveTowards(newDefender.transform.position, targetPosition, charMoveSpeed * 2);
                        yield return null;
                    }
                    bool previousDefending = newDefender.defending;
                    newDefender.defending = true;
                    if (actionSkill.Damage != 0)
                        DamageApplication(currentCharacter, newDefender);
                    ApplyConditions(newDefender);
                    StartCoroutine(TargetBlink(newDefender));
                    newDefender.defending = previousDefending;
                    while (newDefender.startPosition != (Vector2)newDefender.transform.position){
                        newDefender.transform.position = Vector2.MoveTowards(newDefender.transform.position, targetPosition, charMoveSpeed);
                        yield return null;
                    }
                    continue;
                }
            }
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            //if physical damage checks for miss
            if (actionSkill.ActionType != ActionType.Item && actionSkill.PhysicalDamage && actionSkill.Damage > 0
                && (defender.Speed - currentCharacter.Speed) * missChancePerSpeed + baseMissChance >= Random.Range(0, 100)){
                    BattleText battleText = Instantiate(battleTextPrefab, battleCanvas).GetComponent<BattleText>();
                    battleText.ProcessText("Miss!", defender.transform.position);
                    continue;
            }

            //applies attacks effects
                if (actionSkill.Damage != 0) //SPELL CAUSES DAMAGE OR HEALS
                DamageApplication(currentCharacter, defender);
            ApplyConditions(defender);
            StartCoroutine(TargetBlink(defender));
            CheckDeath(defender);
            yield return new WaitForSeconds(0.5f);
        }
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(CharacterMovement(false));
        EndAction(currentCharacter);
    }

    IEnumerator CharacterMovement(bool preAttack = true){
                              
        if (preAttack){
            Vector2 direction = (currentCharacter.tag == "Enemy") ? Vector2.right : Vector2.left;
            while (Vector2.Distance(currentCharacter.startPosition, currentCharacter.transform.position) < 1.5f){
                currentCharacter.transform.Translate(direction * charMoveSpeed);
                yield return null;
            }
        }
        else{
            while (currentCharacter.startPosition != (Vector2)currentCharacter.transform.position){
                currentCharacter.transform.position = Vector2.MoveTowards(currentCharacter.transform.position, currentCharacter.startPosition, charMoveSpeed / 2);
                yield return null;
            }
        }        
    }
    
    void DamageApplication(BattleCharacter attacker, BattleCharacter defender){
        
        bool physicalDamage = actionSkill.PhysicalDamage;
        int attackerDamage = (physicalDamage) ? attacker.Attack : attacker.MagicATK;
        int defenderDefense = (physicalDamage) ? defender.Defense : defender.MagicDEF;
        int weaknessDamage = 0; //0 = regular damage // 1 = resistance damage // 2 = weakness damage
        float totalDamage = 1;

        //ATTACK ATTRIBUTE (if not item)
        if (actionSkill.ActionType != ActionType.Item)
            totalDamage = (attackerDamage * 1.5f) + Random.Range(attacker.Level / 2, attacker.Level);

        //DEFEND ATTRIBUTE (if not healing)
        if (actionSkill.Damage >= 0)
            totalDamage = Mathf.Clamp(totalDamage - defenderDefense + Random.Range(defender.Level / 2, defender.Level), 1, 999);

        //SKILL MULTIPLIER
        totalDamage *= actionSkill.Damage;

        //ELEMENTAL MULTIPLIERS
        if (actionSkill.Element != Element.None){            
            totalDamage *= ElementsController();            
            if (actionSkill.Element == defender.Resistance){
                weaknessDamage = 1;
                totalDamage /= 2;
            }
            else if (actionSkill.Element == defender.Weakness){
                weaknessDamage = 2;
                totalDamage *= 2;
            }
        }

        //DEFEND REDUCTION
        if (defender.defending && actionSkill.Damage >= 0)
            totalDamage *= 0.2f;

        //CLAMP BETWEEN 1 AND 999
        totalDamage = (actionSkill.Damage > 0) ? Mathf.Clamp(totalDamage, 1, 999) : Mathf.Clamp(totalDamage, -999, -1);

        //SPECIAL EFFECTS
        if (defender.conditionsList.Exists(x => x.Id == ConditionID.WaterShield) && actionSkill.Element == Element.Water)
            totalDamage = 0;
        
        //DAMAGE APPLICATION
        defender.HP -= (int)totalDamage;
        BattleText battleText = Instantiate(battleTextPrefab, battleCanvas).GetComponent<BattleText>();
        battleText.ProcessText(((int)totalDamage).ToString(), defender.transform.position, weaknessDamage);
    }

    void ApplyConditions(BattleCharacter defender){

        if (actionSkill.ConditionID == ConditionID.None)
            return;

        //if condition already exists, moves it to the past list before adding a new one with new timer
        Condition existingCondition = defender.conditionsList.Find(x => x.Id == actionSkill.ConditionID);
        if (existingCondition != null){
            if (existingCondition.Timer > existingCondition.TimerLimit - 2)
                RemoveCondition(existingCondition, defender);
        }

        //adds the condition and instantiates the icons
        Condition condition = database.Conditions.Find(x => x.Id == actionSkill.ConditionID);
        defender.conditionsList.Add(new Condition(condition.Id));
        defender.conditionsList[defender.conditionsList.Count - 1].IconGameObject = Instantiate(conditionPrefab, new Vector2 (defender.transform.position.x, defender.transform.position.y + 0.8f), Quaternion.identity);
        defender.conditionsList[defender.conditionsList.Count - 1].IconGameObject.GetComponent<SpriteRenderer>().sprite = condition.Icon;
        
        //for each time the defender has already been affected by the same condition, it reduces 2 seconds the duration
        defender.conditionsList[defender.conditionsList.Count - 1].TimerLimit = 5 - (defender.pastConditionsList.FindAll(x => x == condition.Id).Count * 2);

        //applies specific condition effects and sets their timers
        switch (condition.Id){
            case ConditionID.Stun:
                if (defender.ATB >= 10){
                    defender.ATB = 9.9f;
                    actionSelectQueue.Remove(defender);
                }
                break;
            case ConditionID.WaterShield:
                defender.conditionsList[defender.conditionsList.Count - 1].Timer = 20;
                break;
            default: //cover / protect
                defender.conditionsList[defender.conditionsList.Count - 1].Timer = -99999;
                break;*/
        /*case ConditionID.TargetLock:
            defender.conditionsList[defender.conditionsList.Count - 1].Timer = -99999;
            defender.conditionsList[defender.conditionsList.Count - 1].OptionalCharacter = currentCharacter;
            break;
        //}
    }

    void RemoveCondition(Condition condition, BattleCharacter character){

        ///////// CALLED BY CharacterController UPDATE WHEN TIMERS RUNS OUT /////////
        Destroy(character.transform.Find(condition.Name).gameObject);
        character.conditionsList.Remove(condition);

        //reorganize condition icons
        for (int i = 0; i < character.conditionsList.Count; i++)
            character.transform.Find(character.conditionsList[i].Name).position = new Vector3(character.transform.position.x + i * 0.25f, character.transform.position.y + 0.8f);
    }

    float ElementsController(){

        float elementalMultiplier = 1;
        ElementDetails actionElement = database.Elements.Find(x => x.Element == actionSkill.Element);

        Color tempColor1;
        Color tempColor2;
        tempColor1 = actionElement.Color;
        for (int i = 0; i < elementList.Length; i++){
            tempColor2 = elementList[i].color ;
            elementList[i].color = tempColor1;
            tempColor1 = tempColor2;
        }        

        for (int i = 0; i < elementList.Length; i++){
            if (elementList[i].color == actionElement.Color)
                elementalMultiplier += (actionSkill.PhysicalDamage) ? 0.2f : 0.4f;
            else if (elementList[i].color == actionElement.OpposingColor)
                elementalMultiplier -= (actionSkill.PhysicalDamage) ? 0.2f : 0.4f;
        }
        return elementalMultiplier;
    }

    void CheckDeath(BattleCharacter defender){

        if (defender.HP != 0)
            return;

        foreach (Condition condition in defender.conditionsList)
            Destroy(condition.IconGameObject);
        actionSelectQueue.Remove(defender);

        ///////// REMOVES DEAD ENEMIES FROM BATTLE /////////
        if (defender.tag != "Ally"){
            Experience += defender.Level;
            charactersInBattle[1].Remove(defender);
            Destroy(defender.gameObject);
        }

        ///////// RESETS DEAD ALLY VARIABLES - ATB AND CONDITIONS /////////
        else{
            defender.ATB = 0;
            defender.ATBBar.transform.localScale = new Vector2((Mathf.Clamp(defender.ATB, 0, 10) / 10), 1);
            defender.conditionsList.Clear();
            //defender.pastConditionsList.Clear();
        }

        ///////// CHECKS TO END BATTLE /////////
        List<BattleCharacter> tempCharList = charactersInBattle[0].FindAll(x => x.HP == 0);
        if (charactersInBattle[1].Count == 0 || charactersInBattle[0].FindAll(x => x.HP == 0).Count == charactersInBattle[0].Count)
            StartCoroutine(EndBattle(charactersInBattle[1].Count));

        ///////// ADJUSTS THE CURRENT MAXIMUMSPEED ///////// 
        if (defender.Speed == maximumSpeed){
            maximumSpeed = 0;
            foreach (BattleCharacter character in charactersInBattle[0]){
                if (character.Speed > maximumSpeed)
                    maximumSpeed = character.Speed;
            }
            foreach (BattleCharacter character in charactersInBattle[1]){
                if (character.Speed > maximumSpeed)
                    maximumSpeed = character.Speed;
            }
        }
    }

    void EndAction(BattleCharacter attacker){

        actionType = ActionType.None;
        actionSkill = null;
        attacker.animator.SetBool("Attack", false);
        attacker.ATB = 0;
        //attacker.ATBBar.color = Color.blue;
        actionHappening = false;
    }

    IEnumerator TargetBlink(BattleCharacter defender){
        
        defender.spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        if (defender != null)
            defender.spriteRenderer.color = Color.white;
    }
    
    IEnumerator EndBattle(int enemiesAlive){

        actionSelectQueue.Clear();        
        yield return new WaitForSeconds(1);
        if (enemiesAlive == 0){
            battleEndingText.text = Experience + localization.battleEndingText;
            yield return new WaitForSeconds(2);
            foreach (BattleCharacter character in charactersInBattle[0]){                
                //dead characters wont get xp
                if (character.HP == 0)
                    continue;
                Character dbCharacter = database.Player.Characters.Find(x => x.Name == character.name);
                if (generalFunctions.LevelUpCharacter(dbCharacter, Experience)){
                    battleEndingText.text = dbCharacter.Name + localization.levelUpText + dbCharacter.Level;
                    yield return new WaitForSeconds(2);
                }
            }
            StartCoroutine(generalFunctions.LoadScene(generalFunctions.previousScene));
        }
        else { 
            battleEndingText.text = "GAME OVER";
            StartCoroutine(generalFunctions.LoadScene(generalFunctions.previousScene)); //////////////////////////////////////////////// COMENTAR!!!
        }
    }*/
}