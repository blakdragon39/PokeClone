using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum BattleState {
    Start,
    ActionSelection,
    MoveSelection,
    RunningTurn,
    Busy,
    PartyScreen,
    BattleOver
}

public enum BattleAction {
    Move,
    SwitchPokemon,
    UseItem,
    Run
}

public class BattleSystem : MonoBehaviour {
    
    public event Action<bool> OnBattleOver; 
    
    [SerializeField] private BattleUnit playerUnit;
    [SerializeField] private BattleUnit enemyUnit;
    [SerializeField] private BattleDialog dialogBox;
    [SerializeField] private PartyScreen partyScreen;
    [SerializeField] private Image playerImage;
    [SerializeField] private Image trainerImage;

    private BattleState state;
    private BattleState? prevState;
    private int currentAction;
    private int currentMove;
    private int currentMember;

    private PokemonParty playerParty;
    private PokemonParty trainerParty;
    private Pokemon wildPokemon;

    private bool isTrainerBattle = false;
    private PlayerController player;
    private TrainerController trainer;

    public void StartBattle(PokemonParty playerParty, Pokemon wildPokemon) {
        this.playerParty = playerParty;
        this.wildPokemon = wildPokemon;
        
        StartCoroutine(SetupBattle());
    }
    
    public void StartTrainerBattle(PokemonParty playerParty, PokemonParty trainerParty) {
        this.playerParty = playerParty;
        this.trainerParty = trainerParty;

        isTrainerBattle = true;
        player = playerParty.GetComponent<PlayerController>();
        trainer = trainerParty.GetComponent<TrainerController>();
        
        StartCoroutine(SetupBattle());
    }

    public void HandleUpdate() {
        switch (state) {
            case BattleState.ActionSelection:
                HandleActionSelection();
                break;
            case BattleState.MoveSelection:
                HandleMoveSelection();
                break;
            case BattleState.PartyScreen:
                HandlePartyScreenSelection();
                break;
        }
    }

    private IEnumerator SetupBattle() {
        playerUnit.Clear();
        enemyUnit.Clear();
        
        if (!isTrainerBattle) {
            // Wild Pokemon Battle
            playerUnit.Setup(playerParty.GetHealthyPokemon());
            enemyUnit.Setup(wildPokemon);
        
            dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);
            yield return dialogBox.TypeDialog($"A wild {enemyUnit.Pokemon.Base.Name} appeared!");
        } else {
            // Trainer Battle
            playerUnit.gameObject.SetActive(false);
            enemyUnit.gameObject.SetActive(false);
            playerImage.gameObject.SetActive(true);
            trainerImage.gameObject.SetActive(true);

            playerImage.sprite = player.Sprite;
            trainerImage.sprite = trainer.Sprite;

            yield return dialogBox.TypeDialog($"{trainer.Name} wants to battle.");
            
            // Send out first pokemon of trainer
            trainerImage.gameObject.SetActive(false);
            enemyUnit.gameObject.SetActive(true);
            var enemyPokemon = trainerParty.GetHealthyPokemon();
            enemyUnit.Setup(enemyPokemon);
            yield return dialogBox.TypeDialog($"{trainer.Name} sent out {enemyPokemon.Base.Name}.");

            // Send out first pokemon of player
            playerImage.gameObject.SetActive(false);
            playerUnit.gameObject.SetActive(true);
            var playerPokemon = playerParty.GetHealthyPokemon();
            playerUnit.Setup(playerPokemon);
            yield return dialogBox.TypeDialog($"Go {playerPokemon.Base.Name}!");
            dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);
        }

        partyScreen.Init();
        ActionSelection();
    }

    private void BattleOver(bool won) {
        state = BattleState.BattleOver;
        
        playerParty.Pokemons.ForEach(p => p.OnBattleOver());
        
        OnBattleOver(won);
    }

    private void ActionSelection() {
        state = BattleState.ActionSelection;
        dialogBox.SetDialog("Choose an action");
        dialogBox.EnableActionSelector(true);
    }

    private void OpenPartyScreen() {
        state = BattleState.PartyScreen;
        currentMember = 0;
        partyScreen.SetPartyData(playerParty.Pokemons);
        partyScreen.gameObject.SetActive(true);
    }

    private void MoveSelection() {
        state = BattleState.MoveSelection;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableMoveSelector(true);
    }

    private IEnumerator RunTurns(BattleAction playerAction) {
        state = BattleState.RunningTurn;

        if (playerAction == BattleAction.Move) {
            playerUnit.Pokemon.CurrentMove = playerUnit.Pokemon.Moves[currentMove];
            enemyUnit.Pokemon.CurrentMove = enemyUnit.Pokemon.GetRandomMove();

            int playerMovePriority = playerUnit.Pokemon.CurrentMove.Base.Priority;
            int enemyMovePriority = enemyUnit.Pokemon.CurrentMove.Base.Priority;
            
            // Check who goes first 
            bool playerGoesFirst = true;
            if (enemyMovePriority > playerMovePriority) 
                playerGoesFirst = false;
            else if (enemyMovePriority == playerMovePriority)
                playerGoesFirst = playerUnit.Pokemon.Speed > enemyUnit.Pokemon.Speed;
            
            var firstUnit = playerGoesFirst ? playerUnit : enemyUnit;
            var secondUnit = playerGoesFirst ? enemyUnit : playerUnit;

            var secondPokemon = secondUnit.Pokemon;
            
            // First turn
            yield return RunMove(firstUnit, secondUnit, firstUnit.Pokemon.CurrentMove);
            yield return RunAfterTurn(firstUnit);
            if (state == BattleState.BattleOver) yield break;

            if (secondPokemon.HP > 0) {
                // Second turn
                yield return RunMove(secondUnit, firstUnit, secondUnit.Pokemon.CurrentMove);
                yield return RunAfterTurn(secondUnit);
                if (state == BattleState.BattleOver) yield break;
            }
        } else {
            if (playerAction == BattleAction.SwitchPokemon) {
                var selectedPokemon = playerParty.Pokemons[currentMember];
                state = BattleState.Busy;
                yield return SwitchPokemon(selectedPokemon);
            }
            
            //Enemy Turn
            var enemyMove = enemyUnit.Pokemon.GetRandomMove();
            yield return RunMove(enemyUnit, playerUnit, enemyMove);
            yield return RunAfterTurn(enemyUnit);
            if (state == BattleState.BattleOver) yield break;
        }

        if (state != BattleState.BattleOver) state = BattleState.ActionSelection;
    }

    private IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move) {
        bool canRunMove = sourceUnit.Pokemon.OnBeforeMove();
        if (!canRunMove) {
            yield return ShowStatusChanges(sourceUnit.Pokemon);
            yield return sourceUnit.HUD.UpdateHP();
            yield break;
        }

        yield return ShowStatusChanges(sourceUnit.Pokemon);
        move.PP -= 1;
        yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.Name} used {move.Base.Name}");

        if (CheckIfMoveHits(move, sourceUnit.Pokemon, targetUnit.Pokemon)) {

            sourceUnit.PlayAttackAnimation();
            yield return new WaitForSeconds(1f);
            targetUnit.PlayHitAnimation();

            if (move.Base.Category == MoveCategory.Status) {
                yield return RunMoveEffects(move.Base.Effects, sourceUnit.Pokemon, targetUnit.Pokemon, move.Base.Target);
            } else {
                var damageDetails = targetUnit.Pokemon.TakeDamage(move, sourceUnit.Pokemon);
                yield return targetUnit.HUD.UpdateHP();
                yield return ShowDamageDetails(damageDetails);
            }

            if (move.Base.Secondaries != null && move.Base.Secondaries.Count > 0 && targetUnit.Pokemon.HP > 0) {
                foreach (var secondary in move.Base.Secondaries) {
                    var rnd = Random.Range(1, 101);
                    if (rnd < secondary.Chance) {
                        yield return RunMoveEffects(secondary, sourceUnit.Pokemon, targetUnit.Pokemon, secondary.Target);
                    }
                }
            }

            if (targetUnit.Pokemon.HP <= 0) {
                yield return dialogBox.TypeDialog($"{targetUnit.Pokemon.Base.Name} fainted");
                targetUnit.PlayFaintAnimation();
                yield return new WaitForSeconds(2f);

                CheckForBattleOver(targetUnit);
            }
        } else {
            yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.Name}'s attack missed.");
        }
    }

    private IEnumerator RunMoveEffects(MoveEffects effects, Pokemon source, Pokemon target, MoveTarget moveTarget) {
        // Stat Boosting
        if (effects.Boosts != null) {
            if (moveTarget == MoveTarget.Self) {
                source.ApplyBoosts(effects.Boosts);
            } else {
                target.ApplyBoosts(effects.Boosts);
            }
        }

        // Status Conditions
        if (effects.Status != ConditionID.None) {
            target.SetStatus(effects.Status);
        }
        
        // Status Conditions
        if (effects.VolatileStatus != ConditionID.None) {
            target.SetVolatileStatus(effects.VolatileStatus);
        }

        yield return ShowStatusChanges(source);
        yield return ShowStatusChanges(target);
    }

    private IEnumerator RunAfterTurn(BattleUnit sourceUnit) {
        if (state == BattleState.BattleOver) yield break;
        yield return new WaitUntil(() => state == BattleState.RunningTurn);
        
        sourceUnit.Pokemon.OnAfterTurn();
        yield return ShowStatusChanges(sourceUnit.Pokemon);
        yield return sourceUnit.HUD.UpdateHP();

        // Checking for KO after burn/poison/etc
        if (sourceUnit.Pokemon.HP <= 0) {
            yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.Name} fainted");
            sourceUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(2f);

            CheckForBattleOver(sourceUnit);
        }
    }

    private bool CheckIfMoveHits(Move move, Pokemon source, Pokemon target) {
        if (move.Base.AlwaysHits) return true;
        
        float moveAccuracy = move.Base.Accuracy;
        int accuracy = source.StatBoosts[Stat.Accuracy];
        int evasion = target.StatBoosts[Stat.Evasion];
        var boostValues = new [] { 1f, 4f/3f, 5f/3f, 2f, 7f/3f, 8f/3f, 3f };

        if (accuracy > 0) moveAccuracy *= boostValues[accuracy];
        else if (accuracy < 0) moveAccuracy /= boostValues[-accuracy];
        
        if (evasion > 0) moveAccuracy /= boostValues[evasion];
        else if (evasion < 0) moveAccuracy *= boostValues[-evasion];
        
        return Random.Range(1, 101) <= moveAccuracy;
    }

    private IEnumerator ShowStatusChanges(Pokemon pokemon) {
        while (pokemon.StatusChanges.Count > 0) {
            var message = pokemon.StatusChanges.Dequeue();
            yield return dialogBox.TypeDialog(message);
        }
    }

    private void CheckForBattleOver(BattleUnit faintedUnit) {
        if (faintedUnit.IsPlayerUnit) {
            var nextPokemon = playerParty.GetHealthyPokemon();
            if (nextPokemon != null) {
                OpenPartyScreen();
            } else {
                BattleOver(false);
            }
        } else {
            if (!isTrainerBattle) {
                BattleOver(true);
            } else {
                var nextPokemon = trainerParty.GetHealthyPokemon();
                if (nextPokemon != null) {
                    StartCoroutine(SendNextTrainerPokemon(nextPokemon));
                } else {
                    BattleOver(true);
                }
            }
        }
    }
    
    private IEnumerator ShowDamageDetails(DamageDetails details) {
        if (details.Critical) {
            yield return dialogBox.TypeDialog("A critical hit!");
        }

        if (details.TypeEffectiveness > 1f) {
            yield return dialogBox.TypeDialog("It's super effective!");
        } else if (details.TypeEffectiveness < 1f) { //todo what about 0f?
            yield return dialogBox.TypeDialog("It's not very effective.");
        }
    }

    private void HandleActionSelection() {
        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            currentAction += 1;
        } else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            currentAction -= 1;
        } else if (Input.GetKeyDown(KeyCode.DownArrow)) {
            currentAction += 2;
        } else if (Input.GetKeyDown(KeyCode.UpArrow)) {
            currentAction -= 2;
        }

        currentAction = Mathf.Clamp(currentAction, 0, 3);

        dialogBox.UpdateActionSelection(currentAction);

        if (Input.GetKeyDown(KeyCode.Z)) {
            if (currentAction == 0) { // Fight
                MoveSelection();
            } else if (currentAction == 1) { // Bag
                // Bag
            } else if (currentAction == 2) { // Pokemon
                prevState = state;
                OpenPartyScreen();
            } else if (currentAction == 3) { // Run
                // Run
            }
        }
    }

    private void HandleMoveSelection() {
        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            currentMove += 1;
        } else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            currentMove -= 1;
        } else if (Input.GetKeyDown(KeyCode.DownArrow)) {
            currentMove += 2;
        } else if (Input.GetKeyDown(KeyCode.UpArrow)) {
            currentMove -= 2;
        }

        currentMove = Mathf.Clamp(currentMove, 0, playerUnit.Pokemon.Moves.Count - 1);

        dialogBox.UpdateMoveSelection(currentMove, playerUnit.Pokemon.Moves[currentMove]);

        if (Input.GetKeyDown(KeyCode.Z)) {
            var move = playerUnit.Pokemon.Moves[currentMove];
            if (move.PP == 0) return;
            
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            StartCoroutine(RunTurns(BattleAction.Move));
        } else if (Input.GetKeyDown(KeyCode.X)) {
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            ActionSelection();
        }
    }

    private void HandlePartyScreenSelection() {
        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            currentMember += 1;
        } else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            currentMember -= 1;
        } else if (Input.GetKeyDown(KeyCode.DownArrow)) {
            currentMember += 2;
        } else if (Input.GetKeyDown(KeyCode.UpArrow)) {
            currentMember -= 2;
        }

        currentMember = Mathf.Clamp(currentMember, 0, playerParty.Pokemons.Count - 1);
        
        partyScreen.UpdateMemberSelection(currentMember);

        if (Input.GetKeyDown(KeyCode.Z)) {
            var selectedMember = playerParty.Pokemons[currentMember];
            if (selectedMember.HP <= 0) {
                partyScreen.SetMessageText("You can't send out a fainted pokemon");
            } else if (selectedMember == playerUnit.Pokemon) {
                partyScreen.SetMessageText("You can't switch with the same pokemon");
            } else {
                partyScreen.gameObject.SetActive(false);

                if (prevState == BattleState.ActionSelection) {
                    prevState = null;
                    StartCoroutine(RunTurns(BattleAction.SwitchPokemon));
                } else {
                    state = BattleState.Busy;
                    StartCoroutine(SwitchPokemon(selectedMember));    
                }
            }
        } else if (Input.GetKeyDown(KeyCode.X)) {
            partyScreen.gameObject.SetActive(false);
            ActionSelection();
        }
    }

    private IEnumerator SwitchPokemon(Pokemon newPokemon) {
        bool currentPokemonFainted = true;
        if (playerUnit.Pokemon.HP > 0) {
            yield return dialogBox.TypeDialog($"Come back {playerUnit.Pokemon.Base.Name}");
            playerUnit.PlayLeaveAnimation();
            yield return new WaitForSeconds(2f);
        }

        playerUnit.Setup(newPokemon);
        dialogBox.SetMoveNames(newPokemon.Moves);

        yield return dialogBox.TypeDialog($"Go {newPokemon.Base.Name}!");

        state = BattleState.RunningTurn;
    }

    private IEnumerator SendNextTrainerPokemon(Pokemon nextPokemon) {
        state = BattleState.Busy;
        
        enemyUnit.Setup(nextPokemon);
        yield return dialogBox.TypeDialog($"{trainer.Name} sent out {nextPokemon.Base.Name}.");

        state = BattleState.RunningTurn;
    }
}