//WATER RING TA BUGADO!!!
//WATER RING TA BUGADO!!!
//WATER RING TA BUGADO!!!
//WATER RING TA BUGADO!!!
//WATER RING TA BUGADO!!!
//WATER RING TA BUGADO!!!
//WATER RING TA BUGADO!!!
//WATER RING TA BUGADO!!!

//BEST FORMULA = (ATK + ATKLV)^2 / (ATK + DEF + DEFLV)  

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BattleController : MonoBehaviour {

    GeneralFunctions general;
    public EnemyAI enemyAI;
    public SkillController skillController;

    public Transform[] allySpawnLocations;
    public Transform[] enemySpawnLocations;
    public Transform[] elementsImages;
    public Text[] elementsTexts;
    public int[] elements;
    public Text descriptionText;
    public Text battleEndingText;
    public Transform mainCanvas;
    public GameObject targetObject;
    public GameObject turnObject;
    public GameObject battleTextPrefab;
    public GameObject characterPrefab;
    public GameObject conditionPrefab;
    public GameObject skillButtonPrefab;
    public GameObject itemButtonPrefab;

    int Experience;
    public int maximumSpeed; 
    
    public List<List<BattleCharacter>> charactersInBattle = new List<List<BattleCharacter>>(); //0 for ally, 1 for enemy
    List<BattleCharacter> battleCharacters = new List<BattleCharacter>(); //stores every character in battle, for performance usage in atb management
    public List<BattleCharacter> actionSelectQueue = new List<BattleCharacter>();
    List<BattleCharacter> counterActionQueue = new List<BattleCharacter>();
    List<Skill> counterSkillQueue = new List<Skill>();
    BattleText skillText;
    BattleCharacter currentCharacter;
    Skill currentSkill;
    UIButton currentItemButton;
    int currentTargetIndex;
    const float charMoveSpeed = 20;
    GameObject lastSelectedGameObject;
    
    bool enableTargetInput = false;
    bool actionHappening = false;
    bool endBattle = false;
    bool inputCooldown;
    
    //skill specific variables
    public int lastDamageCaused; //revenge and combo (caesar)
    public BattleCharacter charWithCover; //filled by char instantiation - cover (caesar)    
    public List<GlobalSkill> globalCounterSkills = new List<GlobalSkill>(); //filled by char instantiation - used by avenge (caesar)

    void Awake(){

        general = FindObjectOfType<GeneralFunctions>();
        mainCanvas.parent.GetComponent<Canvas>().worldCamera = Camera.main;
        Camera.main.orthographicSize = 4;

        general.audioController.battleMusicSource.Play();

        mainCanvas.GetChild(1).GetChild(0).GetComponentInChildren<Text>().text = general.localization.itemsButton;
        mainCanvas.GetChild(1).GetChild(1).GetComponentInChildren<Text>().text = general.database.Skills[general.database.Skills.Count - 2][0].LocalizedName;
        mainCanvas.GetChild(1).GetChild(2).GetComponentInChildren<Text>().text = general.database.Skills[general.database.Skills.Count - 2][1].LocalizedName;
        mainCanvas.GetChild(1).GetChild(3).GetComponentInChildren<Text>().text = general.database.Skills[general.database.Skills.Count - 2][2].LocalizedName;

        elements = general.sceneElements;
        ElementImageRefresh();
        InstantiateSkills();
        SpawnCharacters();
    }

    void InstantiateSkills(){
        
        ///////// INSTANTIATE SKILLS BUTTONS /////////
        for (int i = 1; i < 6; i++)
            Instantiate(skillButtonPrefab, mainCanvas.GetChild(0));
        
        ///////// INSTANTIATE ITEMS /////////
        GameObject itemButton = null;
        foreach (PlayerItem playerItem in general.database.Player.Inventory[(int)ItemType.Usable]){
            itemButton = (itemButton == null) ? itemButtonPrefab : Instantiate(itemButtonPrefab, mainCanvas.GetChild(2));
            UIButton uiButton = itemButton.GetComponent<UIButton>();
            uiButton.playerItem = playerItem;
        }
    }

    void ElementImageRefresh(){

        for (int i = 0; i < elements.Length; i++){
            elementsImages[i].localScale = new Vector2(1, (float)elements[i] / 5);
            elementsTexts[i].text = elements[i].ToString();
        }
    }

    void SpawnCharacters(){        

        ///////// SPAWNS ALLIES /////////
        charactersInBattle.Add(new List<BattleCharacter>());
        for (int i = 0; i < 4 && i < general.database.Player.Characters.Count; i++) {
            charactersInBattle[0].Add(Instantiate(characterPrefab, allySpawnLocations[i].position, Quaternion.identity).GetComponent<BattleCharacter>());
            charactersInBattle[0][i].InstantiateCharacter(general.database.Player.Characters[i], "Ally");
            if (general.database.Player.Characters[i].Speed > maximumSpeed)
                maximumSpeed = general.database.Player.Characters[i].Speed;
        }

        ///////// SPAWNS ENEMIES /////////
        charactersInBattle.Add(new List<BattleCharacter>());
        for (int i = 0; i < 4 && i < general.sceneEnemies.Count; i++){
            charactersInBattle[1].Add(Instantiate(characterPrefab, enemySpawnLocations[i].position, Quaternion.identity).GetComponent<BattleCharacter>());            
            charactersInBattle[1][i].InstantiateCharacter(general.sceneEnemies[i], "Enemy");
            //charactersInBattle[1][i].transform.rotation = new Quaternion(0, 180, 0, 0);
            charactersInBattle[1][i].spriteRenderer.flipX = true;


            LevelUpEnemy(charactersInBattle[1][i]);


            if (general.sceneEnemies[i].Speed > maximumSpeed)
                maximumSpeed = general.sceneEnemies[i].Speed;
        }
        
        ///////// ADDS EVERYONE TO A GENERAL LIST FOR PERFORMANCE IMPROVEMENT ON ATB MANAGEMENT /////////
        battleCharacters.AddRange(charactersInBattle[0]);
        battleCharacters.AddRange(charactersInBattle[1]);
    }  

    public void LevelUpEnemy(BattleCharacter character){

        character.HPMax = character.HPMax + (int)(character.Level * (character.HPMax / 20f));
        character.HP = character.HPMax;
        character.Attack = character.Attack + (int)(character.Level * (character.Attack / 20f));
        character.MagicATK = character.MagicATK + (int)(character.Level * (character.MagicATK / 20f));
        character.Defense = character.Defense + (int)(character.Level * (character.Defense / 20f));
        character.MagicDEF = character.MagicDEF + (int)(character.Level * (character.MagicDEF / 20f));
        character.Speed = character.Speed + (int)(character.Level * (character.Speed / 20f));
    }
    
    void Update(){
        
        CharacterController(); //controls ATB and conditions

        if (Input.GetButtonDown("Cancel"))
            CancelButtonPress();

        if (!enableTargetInput)
            return;

        ///////// TARGET INPUT /////////
        if (Input.GetButtonDown("Confirm") && InputCooldown())
            SubmitTarget();
        if (Input.GetButtonDown("Horizontal") && currentSkill.Range != SkillRange.Self)
            ChangeTarget(Input.GetAxisRaw("Horizontal"));
        if (Input.GetButtonDown("Vertical") && currentSkill.Range != SkillRange.Self)
            ChangeTarget(-Input.GetAxisRaw("Vertical"));
        
        if (enableTargetInput && currentSkill.AreaTarget){
            currentTargetIndex = (currentTargetIndex  >= charactersInBattle[currentSkill.EnemyTarget].Count - 1) ? 0 : currentTargetIndex + 1;
            targetObject.transform.position = charactersInBattle[currentSkill.EnemyTarget][currentTargetIndex].transform.position;
        }
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
        
        //atb - WHEN ATB REACHES 10, BECOMES 11 AND GOES TO THE QUEUE
        foreach (BattleCharacter character in battleCharacters){
            if (character.HP > 0){
                character.ATB += Time.deltaTime * ((float)character.Speed / (float)maximumSpeed) * 2;
                if (character.ATB >= 10 && character.ATB < 11){
                    actionSelectQueue.Add(character);
                    character.ATB = 11;
                    NextActionCheck();                    
                }
            }
        }
    }

    void NextActionCheck(){

        if (actionSelectQueue.Count > 0 && !actionHappening){
            actionHappening = true;
            turnObject.transform.SetParent(actionSelectQueue[0].transform);
            turnObject.transform.localPosition = Vector2.zero;
            turnObject.SetActive(true);
            if (actionSelectQueue[0].tag == "Ally")
                AllyActionSelect(actionSelectQueue[0]);
            else
                EnemyActionSelect(actionSelectQueue[0]);
            actionSelectQueue.RemoveAt(0);
        }
    }
    
    void AllyActionSelect(BattleCharacter character){

        ///////// PROMPTS THE PANEL FOR ACTION SELECTION WHEN ATB BECOMES 11 /////////
        
        //fills character skill buttons
        for (int i = 0; i < mainCanvas.GetChild(0).childCount; i++){
            if (i < character.ActiveSkills.Count){
                mainCanvas.GetChild(0).GetChild(i).gameObject.SetActive(true);
                mainCanvas.GetChild(0).GetChild(i).GetComponentInChildren<Text>().text = character.ActiveSkills[i].LocalizedName;
            }
            else
                mainCanvas.GetChild(0).GetChild(i).gameObject.SetActive(false);
        }

        //prompts for action
        currentCharacter = character;
        mainCanvas.gameObject.SetActive(true);
        general.audioController.enableButtonSound = true;
        EventSystem.current.SetSelectedGameObject(skillButtonPrefab);
    }

    void EnemyActionSelect(BattleCharacter enemy){

        ///////// AUTOMATICALLY SELECTS AND EXECUTES ENEMY ACTION WHEN ATB BECOMES 11 /////////        
        currentCharacter = enemy;        
        currentSkill = enemyAI.DefineAction(enemy);
        currentSkill.EnemyTarget = (currentSkill.EnemyTarget == 0) ? 1 : 0; //enemy target has to be inverted to support the same skills as the allies
        currentTargetIndex = Random.Range(0, charactersInBattle[currentSkill.EnemyTarget].Count);

        //TAUNT LOGIC
        if (!currentSkill.AreaTarget){
            Condition condition = currentCharacter.conditionsList.Find(x => x.Type == ConditionType.Taunt);
            if (condition != null)
                currentTargetIndex = (int)condition.Modifier;
            /*if (currentCharacter.conditionsList[i].SingleInstance)
                RemoveCondition(currentCharacter.conditionsList[i], attacker);
            }*/
        }

        while (charactersInBattle[currentSkill.EnemyTarget][currentTargetIndex].HP == 0)
            currentTargetIndex = Random.Range(0, charactersInBattle[currentSkill.EnemyTarget].Count);  
        StartCoroutine(ManageActions());


        //DESCOMENTE ISSO E COMENTE O RESTO PRA SIMULAR UMA ENEMY ACTION SIMPLES!!!
        /*currentCharacter = enemy;
        currentSkill = new Skill(99, "test", Range: SkillRange.Melee, EnemyTarget: 0, Damage: 1f);
        //enemyTarget = (currentSkill.EnemyTarget == 0) ? 1 : 0; //enemy target has to be inverted to support the same skills as the allies
        currentTargetIndex = 1;
        SubmitTarget();*/


        //DESCOMENTE ISSO E COMENTE O RESTO PRA PULAR A ENEMY ACTION!!! <<<<<<<<<<<<<<< as vez isso trava um inimigo, relaxa se isso acontecer!!!!!!!!!!!!!!!!!!!
        /*enemy.ATB = 0;
        actionHappening = false;
        return;*/
    }

    public void OtherButtonSelect(){

        currentSkill = general.database.Skills[general.database.Skills.Count - 2].Find(x => x.LocalizedName == EventSystem.current.currentSelectedGameObject.GetComponentInChildren<Text>().text);        
        descriptionText.text = (currentSkill == null) ? null : currentSkill.Description;
    }

    public void ItemButtonSelect(){

        currentItemButton = EventSystem.current.currentSelectedGameObject.GetComponent<UIButton>();
        descriptionText.text = currentItemButton.playerItem.Item.Description;
    }

    public void ItemButtonClick(){

        if (general.database.Player.Inventory[(int)ItemType.Usable].Count == 0){
            general.audioController.PlayInvalidNavigateSound();
            return;
        }

        mainCanvas.GetChild(0).gameObject.SetActive(false);
        mainCanvas.GetChild(1).gameObject.SetActive(false);
        mainCanvas.GetChild(2).gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(itemButtonPrefab);
    }

    public void SkillButtonSelect(){

        Skill skill = currentCharacter.ActiveSkills.Find(x => x.LocalizedName == EventSystem.current.currentSelectedGameObject.GetComponentInChildren<Text>().text);

        currentSkill = new Skill(skill.Id, skill.Name, skill.Type, skill.Range, skill.Element, skill.ConsumeElement, skill.EnemyTarget, skill.DeadTarget,
            skill.AreaTarget, skill.Accuracy, skill.PhysicalDamage, skill.Damage, skill.SpecialEffectTrigger, skill.Status, skill.LocalizedName, skill.Description);

        if (currentSkill.Accuracy != 999)
            descriptionText.text = "[" + (currentCharacter.Accuracy + currentSkill.Accuracy) + " Acc] " + currentSkill.Description;
        else
            descriptionText.text = currentSkill.Description;
    }
    
    public void SkillButtonClick() {

        //ISSO TA AQUI PORQUE NA DEMO NAO QUERO QUE ELES FUNCIONEM!!!
        //ISSO TA AQUI PORQUE NA DEMO NAO QUERO QUE ELES FUNCIONEM!!!
        //ISSO TA AQUI PORQUE NA DEMO NAO QUERO QUE ELES FUNCIONEM!!!
        if (currentSkill.Name == "Swap" || currentSkill.Name == "Escape"){
            general.audioController.PlayInvalidNavigateSound();
            return;
        }
        //ISSO TA AQUI PORQUE NA DEMO NAO QUERO QUE ELES FUNCIONEM!!!
        //ISSO TA AQUI PORQUE NA DEMO NAO QUERO QUE ELES FUNCIONEM!!!
        //ISSO TA AQUI PORQUE NA DEMO NAO QUERO QUE ELES FUNCIONEM!!!

        if (currentSkill.ConsumeElement && elements[(int)currentSkill.Element] == 0){
            general.audioController.PlayInvalidNavigateSound();
            return;
        }

        for (int i = currentCharacter.conditionsList.Count - 1; i >= 0; i--){
            if (currentCharacter.conditionsList[i].Type == ConditionType.AreaTargetMod && currentSkill.Range != SkillRange.Self)
                currentSkill.AreaTarget = true;            
            /*if (currentCharacter.conditionsList[i].SingleInstance)
                RemoveCondition(currentCharacter.conditionsList[i], attacker);
            }*/
        }

        //enemyTarget = currentSkill.EnemyTarget;
        currentTargetIndex = (currentSkill.Range == SkillRange.Self) ? charactersInBattle[currentSkill.EnemyTarget].IndexOf(currentCharacter) : 0;
        inputCooldown = true;
        mainCanvas.gameObject.SetActive(false);
        targetObject.SetActive(true);
        targetObject.transform.position = charactersInBattle[currentSkill.EnemyTarget][currentTargetIndex].transform.position;
        enableTargetInput = true;
        lastSelectedGameObject = EventSystem.current.currentSelectedGameObject; //used by CANCEL TARGET
        EventSystem.current.SetSelectedGameObject(null); //necessary to not bug the selection after action
    }

    public void UseItemButtonClick(){

        //enemyTarget = currentSkill.EnemyTarget;
        currentSkill = currentItemButton.playerItem.Item.Skill;
        currentTargetIndex = (currentSkill.Range == SkillRange.Self) ? charactersInBattle[currentSkill.EnemyTarget].IndexOf(currentCharacter) : 0;
        inputCooldown = true;
        mainCanvas.gameObject.SetActive(false);
        targetObject.SetActive(true);
        targetObject.transform.position = charactersInBattle[currentSkill.EnemyTarget][currentTargetIndex].transform.position;
        enableTargetInput = true;
        lastSelectedGameObject = EventSystem.current.currentSelectedGameObject; //used by CANCEL TARGET
        EventSystem.current.SetSelectedGameObject(null); //necessary to not bug the selection after action
    }

    void CancelButtonPress(){

        ///////// CALLED ON CANCEL BUTTON PRESS ON SELECTING TARGET /////////
        if (enableTargetInput){
            targetObject.SetActive(false);
            mainCanvas.gameObject.SetActive(true);
            enableTargetInput = false;
            EventSystem.current.SetSelectedGameObject(lastSelectedGameObject);
        }

        ///////// CALLED ON CANCEL BUTTON PRESS ON ITEM CANVAS /////////
        else if (currentItemButton != null){
            currentItemButton = null;
            mainCanvas.GetChild(0).gameObject.SetActive(true);
            mainCanvas.GetChild(1).gameObject.SetActive(true);
            mainCanvas.GetChild(2).gameObject.SetActive(false);
            EventSystem.current.SetSelectedGameObject(mainCanvas.GetChild(1).GetChild(0).gameObject);
        }
    }
        
    void SubmitTarget(){

        ///////// CALLED ON CONFIRM BUTTON PRESS ON SELECTING TARGET /////////
        if (!currentSkill.AreaTarget && (!currentSkill.DeadTarget && charactersInBattle[currentSkill.EnemyTarget][currentTargetIndex].HP == 0
        || currentSkill.DeadTarget && charactersInBattle[currentSkill.EnemyTarget][currentTargetIndex].HP > 0)){
            general.audioController.PlayInvalidNavigateSound();
            return;
        }

        ///////// HANDLES ITEMS USAGE /////////
        if (currentItemButton != null){
            mainCanvas.GetChild(0).gameObject.SetActive(true);
            mainCanvas.GetChild(1).gameObject.SetActive(true);
            mainCanvas.GetChild(2).gameObject.SetActive(false);
            general.RemoveItem(currentItemButton.playerItem.Item);
            if (currentItemButton.playerItem.Quantity > 0)
                currentItemButton.transform.GetChild(1).GetComponent<Text>().text = currentItemButton.playerItem.Quantity.ToString();
            else
                Destroy(currentItemButton.gameObject);
        }
        currentItemButton = null;

        general.audioController.enableButtonSound = false;
        enableTargetInput = false;
        targetObject.SetActive(false);
        mainCanvas.gameObject.SetActive(false);
        descriptionText.text = null;        
        StartCoroutine(ManageActions());
    }

    void ChangeTarget(float AxisValue){

        ///////// CALLED ON DIRECTIONALS BUTTONS PRESS ON SELECTING TARGET /////////
        int playerInput = (int)AxisValue;

        if (currentTargetIndex == 0 && playerInput == -1)
            currentTargetIndex = charactersInBattle[currentSkill.EnemyTarget].Count - 1;
        else if (currentTargetIndex == charactersInBattle[currentSkill.EnemyTarget].Count - 1 && playerInput == 1)
            currentTargetIndex = 0;
        else
            currentTargetIndex += playerInput;

        targetObject.transform.position = charactersInBattle[currentSkill.EnemyTarget][currentTargetIndex].transform.position;
    }
    
    IEnumerator ManageActions(){

        yield return new WaitForSeconds(0.2f);
        yield return StartCoroutine(ExecuteAction(currentCharacter, false));

        if (counterActionQueue.Count > 0){
            for (int i = 0; i < counterActionQueue.Count; i++){
                BattleCharacter characterToBeCountered = (counterSkillQueue[i].Range == SkillRange.Self) ? counterActionQueue[i] : currentCharacter;
                currentTargetIndex = charactersInBattle[counterSkillQueue[i].EnemyTarget].IndexOf(characterToBeCountered);
                if (currentTargetIndex == -1)
                    break;
                currentSkill = counterSkillQueue[i];
                yield return StartCoroutine(ExecuteAction(counterActionQueue[i], true));
                
            }
            counterActionQueue.Clear();
            counterSkillQueue.Clear();
        }

        if (!endBattle)
            EndAction();
    }

    IEnumerator ExecuteAction(BattleCharacter attacker, bool counterAction){

        //ATTACK PREPARATION
        turnObject.SetActive(false);
        lastDamageCaused = 0;
        attacker.latestSkillName = currentSkill.Name;
        skillText = Instantiate(battleTextPrefab, mainCanvas.parent).GetComponent<BattleText>();
        skillText.ProcessText(currentSkill.LocalizedName);
        StartCoroutine(CharacterMovement(attacker, "Attack", charactersInBattle[currentSkill.EnemyTarget][currentTargetIndex]));

        //ELEMENTAL ADDITION
        if (currentSkill.Element != Element.None && !currentSkill.ConsumeElement){
            elements[(int)currentSkill.Element] = Mathf.Clamp(elements[(int)currentSkill.Element] + 1, 0, 5);
            ElementImageRefresh();
        }

        //TARGET LIST FETCH
        List<BattleCharacter> targetList = new List<BattleCharacter>();
        if (currentSkill.AreaTarget)
            targetList.AddRange(charactersInBattle[currentSkill.EnemyTarget]);
        else
            targetList.Add(charactersInBattle[currentSkill.EnemyTarget][currentTargetIndex]);

        //COVER LOGIC
        if (!counterAction && charWithCover != null && charWithCover.tag == targetList[0].tag && targetList.Count == 1 && currentSkill.Damage > 0 && charWithCover.HPCurrent > targetList[0].HPCurrent){
            targetList[0] = charWithCover;
            StartCoroutine(CharacterMovement(charWithCover, "Cover", charactersInBattle[currentSkill.EnemyTarget][currentTargetIndex]));
        }

        yield return new WaitForSeconds(0.3f); //compensation for attacker movement

        //PROCESS ACTION FOR EACH TARGET
        foreach (BattleCharacter target in targetList){
            if (target.HP == 0 && !currentSkill.DeadTarget || CheckHit(attacker, target))
                continue;

            //ITEMS APPLIES DIRECT DAMAGE
            if (Mathf.Abs(currentSkill.Damage) > 20)
                ApplyDamage((int)currentSkill.Damage, target);

            //SKILLS ARE INVOLVED IN ATTRIBUTE CALCULATIONS
            else if (currentSkill.Damage != 0)
                DealDamageToTarget(attacker, target);
            
            //PROCESSES ATTACKER "DURING" AND "PASSIVE" SKILL EFFECTS
            if (currentSkill.SpecialEffectTrigger == SpecialEffectTrigger.During)
                skillController.ExecuteDuringSkillEffect(currentSkill, attacker, target);
            foreach (Skill passiveSkill in attacker.PassiveSkills){
                if (passiveSkill.SpecialEffectTrigger == SpecialEffectTrigger.During)
                    skillController.ExecuteDuringSkillEffect(passiveSkill, attacker, target);
            }

            StartCoroutine(TargetBlink(target));
                
            //ACTIVATES COUNTER SKILLS
            if (!counterAction){

                //checks for regular counters if the target survives
                if (target.HP > 0){
                    foreach (Skill passiveSkill in target.PassiveSkills){
                        if (passiveSkill.SpecialEffectTrigger == SpecialEffectTrigger.Counter){
                            Skill counterSkill = skillController.GetCounterSkill(passiveSkill, currentSkill, target);
                            if (counterSkill != null){
                                counterActionQueue.Add(target);
                                counterSkillQueue.Add(counterSkill);
                            }
                        }
                    }

                    //AVENGER LOGIC
                    foreach (GlobalSkill globalCounterSkill in globalCounterSkills){
                        Skill counterSkill = skillController.GetGlobalCounterSkill(globalCounterSkill, target);
                        if (counterSkill != null){
                            counterActionQueue.Add(globalCounterSkill.Character);
                            counterSkillQueue.Add(counterSkill);
                        }                        
                    }
                }

                //checks if there is any counters on death ------ ATUALMENTE SÓ FUNCIONA COM A PRIMEIRA E UNICA SKILL ON DEATH DO PERSONAGEM!!!
                else{
                    Skill counterOnDeathSkill = target.PassiveSkills.Find(x => x.SpecialEffectTrigger == SpecialEffectTrigger.CounterOnDeath);
                    if (counterOnDeathSkill != null){
                        Skill counterSkill = skillController.GetCounterSkill(counterOnDeathSkill, currentSkill, target);
                        if (counterSkill != null){
                            counterActionQueue.Add(target);
                            counterSkillQueue.Add(counterSkill);
                        }
                    }
                }
            }

            ProcessDeathAndRess(target);
        }

        if (currentSkill.SpecialEffectTrigger == SpecialEffectTrigger.After)
            skillController.ExecuteAfterSkillSelfEffect(currentSkill, attacker);

        if (currentSkill.Element != Element.None && currentSkill.ConsumeElement){
            elements[(int)currentSkill.Element] = 0;
            ElementImageRefresh();
        }

        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(CharacterMovement(attacker, "Retreat")); //WITHOUT YIELD RETURN, THE CURRENTCHARACTER WILL CHANGE AND MOVEMENT WILL BE STUCK WITHOUT COMING BACK
        yield return new WaitForSeconds(0.2f);
    }

    bool CheckHit(BattleCharacter attacker, BattleCharacter defender){

        //apply attacker conditions modifiers
        int accuracyModifier = 0;
        for (int i = attacker.conditionsList.Count - 1; i >= 0; i--){
            if (attacker.conditionsList[i].Type == ConditionType.HitCheck)
                accuracyModifier += (int)attacker.conditionsList[i].Modifier;
        }
        int attackerAccuracy = currentSkill.Accuracy + attacker.Accuracy + accuracyModifier;

        accuracyModifier = 0;
        for (int i = defender.conditionsList.Count - 1; i >= 0; i--){
            if (defender.conditionsList[i].Type == ConditionType.HitCheck)
                accuracyModifier += (int)defender.conditionsList[i].Modifier;
        }
        int defenderEvasion = defender.Evasion + accuracyModifier;

        if (attackerAccuracy - defenderEvasion < Random.Range(0, 100)){
            BattleText battleText = Instantiate(battleTextPrefab, mainCanvas.parent).GetComponent<BattleText>();
            battleText.ProcessText(general.localization.missAttackText, defender.transform.position);
            return true;
        }        
        return false;
    }
    
    public IEnumerator CharacterMovement(BattleCharacter movingChar, string moveType, BattleCharacter target = null){ //moveType = Attack / Retreat / Cover ----------- bool preAttack = true){
        
        switch (moveType){
            case "Attack":
                if (currentSkill.Range != SkillRange.Melee){ //if not melee, char doesn't need to move
                    yield return new WaitForSeconds(1);
                    yield break;
                }
                Vector2 targetPosition = (currentSkill.EnemyTarget == 1) ? new Vector2(-3.5f, 0.5f) : new Vector2(3.5f, 0.5f);
                if (!currentSkill.AreaTarget){
                    float distance = (currentSkill.EnemyTarget == 1) ? 1 : -1; //how close character gets to target
                    targetPosition = new Vector2(target.transform.position.x + distance, target.transform.position.y);
                }
                yield return StartCoroutine(BattleMoveTowards(movingChar, targetPosition));
                break;
            case "Retreat":
                targetPosition = movingChar.startPosition;
                yield return StartCoroutine(BattleMoveTowards(movingChar, targetPosition));
                break;
            case "Cover":
                float coverDistance = (currentSkill.EnemyTarget == 1) ? 0.5f : -0.5f; //how close character gets to target
                targetPosition = new Vector2(target.transform.position.x + coverDistance, target.transform.position.y);

                yield return new WaitForSeconds(0.2f);
                yield return StartCoroutine(BattleMoveTowards(movingChar, targetPosition));
                yield return new WaitForSeconds(0.7f);

                StartCoroutine(CharacterMovement(movingChar, "Retreat"));
                break;
        }
    }

    IEnumerator BattleMoveTowards(BattleCharacter movingChar, Vector2 targetPosition){
        
        //movingChar cant be convert to Vector2 or it messes up the variable assignment
        while (targetPosition != (Vector2)movingChar.transform.position){
            movingChar.transform.position = Vector2.MoveTowards(movingChar.transform.position, targetPosition, charMoveSpeed * Time.deltaTime);
            yield return null;
        }
    }

    public float GetAttackerDamage(BattleCharacter attacker, float baseModifier){

        float attackerDamage = currentSkill.PhysicalDamage ? attacker.Attack : attacker.MagicATK;
        float damage = attackerDamage + Random.Range(attacker.Level / 2, attacker.Level);
        damage *= baseModifier; //parameter necessary to be used with direct damage conditions
                
        //calculates attacker elemental bonuses
        if (currentSkill.Element != Element.None)
            damage *= ElementalModifier();

        //apply attacker conditions modifiers
        float attackerDamageMultiplier = 1;
        for (int i = attacker.conditionsList.Count - 1; i >= 0; i--){
            if (attacker.conditionsList[i].Type == ConditionType.DMGDealtMod || attacker.conditionsList[i].Type == ConditionType.Titan){
                if (currentSkill.Name == "Counter" && !attacker.conditionsList[i].ActiveOnCounter)
                    continue;
                attackerDamageMultiplier += attacker.conditionsList[i].Modifier;
                if (attacker.conditionsList[i].SingleInstance)
                    RemoveCondition(attacker.conditionsList[i], attacker);
            }
        }
        damage *= attackerDamageMultiplier;

        //cuts damage by half if melee on back roll
        if (!attacker.FrontRow && currentSkill.Range == SkillRange.Melee)
            damage /= 2;

        return damage;
    }

    void DealDamageToTarget(BattleCharacter attacker, BattleCharacter defender){

        //ATTACKER CALCULATION
        float damage = GetAttackerDamage(attacker, currentSkill.Damage);

        //DEFENDER CALCULATIONS (WILL BE IGNORED FOR HEALING SKILLS)
        float elementalMultiplier = 1;
        if (currentSkill.Damage > 0){
            float defenderDefense = currentSkill.PhysicalDamage ? defender.Defense : defender.MagicDEF;
            float finalDefense = defenderDefense + Random.Range(defender.Level / 2, defender.Level);
            damage = Mathf.Clamp(damage, 1, 999);
            
            //calculates defender elemental weakness
            if (currentSkill.Element != Element.None){
                if (defender.Elemental[(int)currentSkill.Element] == 100)
                    elementalMultiplier = -1;
                else if (defender.Elemental[(int)currentSkill.Element] > 0)
                    elementalMultiplier = Mathf.Abs((defender.Elemental[(int)currentSkill.Element] * 0.01f) - 1);
                else if (defender.Elemental[(int)currentSkill.Element] < 0)
                    elementalMultiplier = Mathf.Abs(defender.Elemental[(int)currentSkill.Element] * 0.01f) + 1;
            }
            damage *= elementalMultiplier;
            damage *= (100 / (100 + finalDefense));

            //apply defender conditions modifiers
            float defenderDamageMultiplier = 1;
            for (int i = defender.conditionsList.Count - 1; i >= 0; i--) {
                switch (defender.conditionsList[i].Type){
                    case ConditionType.DMGTakenMod:
                        defenderDamageMultiplier += defender.conditionsList[i].Modifier;
                        if (defender.conditionsList[i].SingleInstance)
                            RemoveCondition(defender.conditionsList[i], defender);
                        break;

                    case ConditionType.Titan:
                        defenderDamageMultiplier -= defender.conditionsList[i].Modifier;
                        break;
                    case ConditionType.Combo:
                        if (defender.conditionsList[i].Name.StartsWith(attacker.name))
                            defenderDamageMultiplier += defender.conditionsList[i].Modifier;
                        break;
                }
            }
            damage *= defenderDamageMultiplier;
        }
        damage = (damage > 0) ? Mathf.Clamp(damage, 1, 999) : Mathf.Clamp(damage, -999, -1);

        //cuts damage by half if melee on back roll
        if (!defender.FrontRow && currentSkill.Range == SkillRange.Melee)
            damage /= 2;

        lastDamageCaused = (int)damage;
        ApplyDamage((int)damage, defender, elementalMultiplier);
    }

    void ApplyDamage(int damage, BattleCharacter target, float damageType = 1, bool fatal = true){

        target.HP -= damage;
        if (!fatal && target.HP == 0)
            target.HP = 1;
        BattleText battleText = Instantiate(battleTextPrefab, mainCanvas.parent).GetComponent<BattleText>();
        battleText.ProcessText(damage.ToString(), target.transform.position, damageType);
    }

    public float ElementalModifier(){

        //elemental physical attacks can change damage from -50% to +50%
        //elemental magical attacks can change damage from -100% to +100%
                
        float elementalBonus = currentSkill.PhysicalDamage ? 0.1f : 0.2f;
        elementalBonus *= currentSkill.ConsumeElement ? 2 : 1;

        float elementalDrawback = currentSkill.PhysicalDamage ? 0.1f : 0.2f;
        int opposingElementIndex = (int)currentSkill.Element % 2 == 0 ? (int)currentSkill.Element + 1 : (int)currentSkill.Element - 1;

        float bonusElementalModifier = (elementalBonus * elements[(int)currentSkill.Element]) - (elementalDrawback * elements[opposingElementIndex]) + 1;

        return bonusElementalModifier;
    }
    
    public void ApplyCondition(Condition condition, BattleCharacter target){

        Condition existingCondition = target.conditionsList.Find(x => x.Name == condition.Name);
        GameObject conditionObject;

        if (existingCondition == null){
            target.conditionsList.Add(condition);
            existingCondition = target.conditionsList[target.conditionsList.Count - 1];
            conditionObject = Instantiate(conditionPrefab, target.conditionsCanvas);
            conditionObject.GetComponent<Image>().sprite = general.database.ConditionSprites.Find(x => x.Name == condition.Name).Sprite;
        }
        else{
            if (!existingCondition.Cumulative){
                existingCondition.Duration = condition.Duration;
                existingCondition.Modifier = condition.Modifier;
            }
            else{
                existingCondition.Duration += condition.Duration;
                existingCondition.Modifier += condition.Modifier;
            }
            conditionObject = target.conditionsCanvas.GetChild(target.conditionsList.IndexOf(existingCondition)).gameObject;
        }

        //FORTITUDE LOGIC
        if (target.PassiveSkills.Exists(x => x.Name == "Fortitude") && !existingCondition.Positive && existingCondition.Duration > 1)
            existingCondition.Duration--;

        conditionObject.GetComponentInChildren<Text>().text = existingCondition.Duration.ToString();
    }

    public void RemoveCondition(Condition condition, BattleCharacter target){

        if (condition == null)
            return;
        Destroy(target.conditionsCanvas.GetChild(target.conditionsList.IndexOf(condition)).gameObject);
        target.conditionsList.Remove(condition);
    }
    
    IEnumerator TargetBlink(BattleCharacter defender){

        defender.spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        if (defender != null)
            defender.spriteRenderer.color = Color.white;
    }

    void ProcessDeathAndRess(BattleCharacter defender){

        if (defender.HP > 0){
            if (currentSkill.DeadTarget) //doesnt works for monsters!!!
                defender.transform.rotation = Quaternion.identity;            
            return;
        }
        
        actionSelectQueue.Remove(defender);
        
        if (defender.tag == "Ally"){
            defender.ATB = 0;
            defender.transform.rotation = new Quaternion(0, 0, 0.5f/*-1*/, 0);
            for (int i = 0; i < defender.conditionsList.Count; i++)
                Destroy(defender.conditionsCanvas.GetChild(i).gameObject);
            defender.conditionsList.Clear();
            if (counterSkillQueue.Exists(x => x.Name == "Last Stand"))
                defender.HP = 1;
        }
        else{
            Experience += defender.Level * 3;
            battleCharacters.Remove(defender);
            charactersInBattle[currentSkill.EnemyTarget].Remove(defender);
            StartCoroutine(DeathFade(defender));
        }

        ///////// CHECKS TO END BATTLE /////////
        if (currentSkill.EnemyTarget == 1 && charactersInBattle[1].Count == 0)
            StartCoroutine(EndBattle());
        else if (currentSkill.EnemyTarget == 0 && charactersInBattle[0].FindAll(x => x.HP == 0).Count == charactersInBattle[0].Count)
            StartCoroutine(EndBattle(true)); //GAMEOVER!!!!!!!!!
    }
    
    IEnumerator DeathFade(BattleCharacter defender){
        
        yield return new WaitForSeconds(0.1f); //necessary so that target blinks before disappearing
        float time = 0.4f;
        while (defender.spriteRenderer.color.a > 0){
            time -= Time.deltaTime;
            defender.spriteRenderer.color = new Color(1, 1, 1, time);
            yield return null;
        }
        if (turnObject.transform.parent = defender.gameObject.transform)
            turnObject.transform.parent = null;
        Destroy(defender.gameObject);
    }
    
    void EndAction(){

        if (currentCharacter != null){

            currentCharacter.ATB = 0;

            //PROCESS CONDITIONS
            for (int i = currentCharacter.conditionsList.Count - 1; i >= 0; i--){
                
                //applies DoTs
                if (currentCharacter.conditionsList[i].Type == ConditionType.DoT)
                    ApplyDamage((int)currentCharacter.conditionsList[i].Modifier, currentCharacter, fatal: false);

                //durations countdown
                if (currentCharacter.conditionsList[i].Cumulative)
                    continue;
                currentCharacter.conditionsList[i].Duration--;
                if (currentCharacter.conditionsList[i].Duration == 0)
                    RemoveCondition(currentCharacter.conditionsList[i], currentCharacter);
                else
                    currentCharacter.conditionsCanvas.GetChild(i).GetComponentInChildren<Text>().text = currentCharacter.conditionsList[i].Duration.ToString();
            }
        }
        //currentSkill = null;
        actionHappening = false;
        NextActionCheck();
    }

    IEnumerator EndBattle(bool lostBattle = false){
                
        endBattle = true;
        if (!lostBattle){
            yield return StartCoroutine(general.audioController.VolumeFade("Music", false, 0));
            general.audioController.battleMusicSource.clip = general.audioController.battleVictoryClip;
            StartCoroutine(general.audioController.VolumeFade("Music", true, 1, 0.01f));
            general.audioController.battleMusicSource.Play();
            battleEndingText.text = Experience + general.localization.battleEndingText;
            yield return new WaitForSeconds(3);
            foreach (BattleCharacter character in charactersInBattle[0]){
                Character playerCharacter = general.database.Player.Characters.Find(x => x.Name == character.name);
                playerCharacter.HPCurrent = character.HP;
                //dead characters wont get xp
                if (character.HP == 0)
                    continue;
                general.CheckLevelUp(playerCharacter, Experience);
                if (playerCharacter.Level != character.Level){
                    battleEndingText.text = playerCharacter.Name + general.localization.levelUpText + playerCharacter.Level;
                    yield return new WaitForSeconds(1.5f);
                }
            }
            general.audioController.battleMusicSource.Stop();
            StartCoroutine(general.LoadBattle(enter: false));
        }
        else{
            StartCoroutine(general.audioController.VolumeFade("Music", false, 0, 3));
            yield return new WaitForSeconds(1);
            StartCoroutine(general.LoadScene("Game Over", 3, 3));            
        }
    }
}