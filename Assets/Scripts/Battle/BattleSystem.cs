using System;
using System.Collections;
using UnityEngine;

public enum BattleState {
    Start,
    ActionSelection,
    MoveSelection,
    PerformMove,
    Busy,
    PartyScreen,
    BattleOver
}

public class BattleSystem : MonoBehaviour {
    
    public event Action<bool> OnBattleOver; 
    
    [SerializeField] private BattleUnit playerUnit;
    [SerializeField] private BattleUnit enemyUnit;
    [SerializeField] private BattleDialog dialogBox;
    [SerializeField] private PartyScreen partyScreen;

    private BattleState state;
    private int currentAction;
    private int currentMove;
    private int currentMember;

    private PokemonParty playerParty;
    private Pokemon wildPokemon;
    
    public void StartBattle(PokemonParty playerParty, Pokemon wildPokemon) {
        this.playerParty = playerParty;
        this.wildPokemon = wildPokemon;
        
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
        playerUnit.Setup(playerParty.GetHealthyPokemon());
        enemyUnit.Setup(wildPokemon);
        dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);
        partyScreen.Init();

        yield return dialogBox.TypeDialog($"A wild {enemyUnit.Pokemon.Base.Name} appeared!");

        ChooseFirstTurn();
    }

    private void ChooseFirstTurn() {
        if (playerUnit.Pokemon.Speed >= enemyUnit.Pokemon.Speed) {
            ActionSelection();
        } else {
            StartCoroutine(EnemyMove());
        }
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

    private IEnumerator PlayerMove() {
        state = BattleState.PerformMove;

        var move = playerUnit.Pokemon.Moves[currentMove];
        yield return RunMove(playerUnit, enemyUnit, move);

        if (state == BattleState.PerformMove) {
            StartCoroutine(EnemyMove());
        }
    }

    private IEnumerator EnemyMove() {
        state = BattleState.PerformMove;

        var move = enemyUnit.Pokemon.GetRandomMove();
        yield return RunMove(enemyUnit, playerUnit, move);

        if (state == BattleState.PerformMove) {
            ActionSelection();
        }
    }

    private IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move) {
        move.PP -= 1;
        yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.Name} used {move.Base.Name}");

        sourceUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(1f);
        targetUnit.PlayHitAnimation();

        if (move.Base.Category == MoveCategory.Status) {
            yield return RunMoveEffects(move, sourceUnit.Pokemon, targetUnit.Pokemon);
        } else {
            var damageDetails = targetUnit.Pokemon.TakeDamage(move, sourceUnit.Pokemon);
            yield return targetUnit.HUD.UpdateHP();
            yield return ShowDamageDetails(damageDetails);
        }

        if (targetUnit.Pokemon.HP <= 0) {
            yield return dialogBox.TypeDialog($"{targetUnit.Pokemon.Base.Name} fainted");
            targetUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(2f);
            
            CheckForBattleOver(targetUnit);
        }
        
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

    private IEnumerator RunMoveEffects(Move move, Pokemon source, Pokemon target) {
        var effects = move.Base.Effects;
        
        // Stat Boosting
        if (effects.Boosts != null) {
            if (move.Base.Target == MoveTarget.Self) {
                source.ApplyBoosts(effects.Boosts);
            } else {
                target.ApplyBoosts(effects.Boosts);
            }
        }

        // Status Conditions
        if (effects.Status != ConditionID.None) {
            target.SetStatus(effects.Status);
        }

        yield return ShowStatusChanges(source);
        yield return ShowStatusChanges(target);
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
            BattleOver(true);
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
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            StartCoroutine(PlayerMove());
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
                state = BattleState.Busy;
                StartCoroutine(SwitchPokemon(selectedMember));
            }
        } else if (Input.GetKeyDown(KeyCode.X)) {
            partyScreen.gameObject.SetActive(false);
            ActionSelection();
        }
    }

    private IEnumerator SwitchPokemon(Pokemon newPokemon) {
        bool currentPokemonFainted = true;
        if (playerUnit.Pokemon.HP > 0) {
            currentPokemonFainted = false;
            yield return dialogBox.TypeDialog($"Come back {playerUnit.Pokemon.Base.Name}");
            playerUnit.PlayLeaveAnimation();
            yield return new WaitForSeconds(2f);
        }

        playerUnit.Setup(newPokemon);
        dialogBox.SetMoveNames(newPokemon.Moves);

        yield return dialogBox.TypeDialog($"Go {newPokemon.Base.Name}!");

        if (currentPokemonFainted) {
            ChooseFirstTurn();
        } else {
            StartCoroutine(EnemyMove());
        }
    }
}