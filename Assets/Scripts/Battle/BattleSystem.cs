using System;
using System.Collections;
using UnityEngine;

public enum BattleState {
    Start,
    PlayerAction,
    PlayerMove,
    EnemyMove,
    Busy
}

public class BattleSystem : MonoBehaviour {
    
    public event Action<bool> OnBattleOver; 
    
    [SerializeField] private BattleUnit playerUnit;
    [SerializeField] private BattleUnit enemyUnit;
    [SerializeField] private BattleHUD playerHUD;
    [SerializeField] private BattleHUD enemyHUD;
    [SerializeField] private BattleDialog dialogBox;
    [SerializeField] private PartyScreen partyScreen;

    private BattleState state;
    private int currentAction;
    private int currentMove;

    private PokemonParty playerParty;
    private Pokemon wildPokemon;
    
    public void StartBattle(PokemonParty playerParty, Pokemon wildPokemon) {
        this.playerParty = playerParty;
        this.wildPokemon = wildPokemon;
        
        StartCoroutine(SetupBattle());
    }

    public void HandleUpdate() {
        switch (state) {
            case BattleState.PlayerAction:
                HandleActionSelection();
                break;
            case BattleState.PlayerMove:
                HandleMoveSelection();
                break;
        }
    }

    private IEnumerator SetupBattle() {
        playerUnit.Setup(playerParty.GetHealthyPokemon());
        enemyUnit.Setup(wildPokemon);
        playerHUD.SetData(playerUnit.Pokemon);
        enemyHUD.SetData(enemyUnit.Pokemon);
        dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);
        partyScreen.Init();

        yield return dialogBox.TypeDialog($"A wild {enemyUnit.Pokemon.Base.Name} appeared!");

        PlayerAction();
    }

    private void PlayerAction() {
        state = BattleState.PlayerAction;
        dialogBox.SetDialog("Choose an action");
        dialogBox.EnableActionSelector(true);
    }

    private void OpenPartyScreen() {
        partyScreen.SetPartyData(playerParty.Pokemons);
        partyScreen.gameObject.SetActive(true);
    }

    private void PlayerMove() {
        state = BattleState.PlayerMove;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableMoveSelector(true);
    }

    private IEnumerator PerformPlayerMove() {
        state = BattleState.Busy;

        var move = playerUnit.Pokemon.Moves[currentMove];
        move.PP -= 1;
        yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} used {move.Base.Name}");

        playerUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(1f);

        enemyUnit.PlayHitAnimation();
        var damageDetails = enemyUnit.Pokemon.TakeDamage(move, playerUnit.Pokemon);
        yield return enemyHUD.UpdateHP();
        yield return ShowDamageDetails(damageDetails);

        if (damageDetails.Fainted) {
            yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.Name} fainted");
            enemyUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(2f);
            OnBattleOver(true);
        } else {
            StartCoroutine(PerformEnemyMove());
        }
    }

    private IEnumerator PerformEnemyMove() {
        state = BattleState.EnemyMove;

        var move = enemyUnit.Pokemon.GetRandomMove();
        move.PP -= 1;
        yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.Name} used {move.Base.Name}");
        
        enemyUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(1f);

        playerUnit.PlayHitAnimation();
        var damageDetails = playerUnit.Pokemon.TakeDamage(move, enemyUnit.Pokemon);
        yield return playerHUD.UpdateHP();
        yield return ShowDamageDetails(damageDetails);

        if (damageDetails.Fainted) {
            yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} fainted");
            playerUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(2f);

            var nextPokemon = playerParty.GetHealthyPokemon();
            if (nextPokemon != null) {
                playerUnit.Setup(nextPokemon);
                playerHUD.SetData(nextPokemon);
                dialogBox.SetMoveNames(nextPokemon.Moves);

                yield return dialogBox.TypeDialog($"Go {nextPokemon.Base.Name}!");

                PlayerAction();
            } else {
                OnBattleOver(false);
            }
        } else {
            PlayerAction();
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
                PlayerMove();
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
            StartCoroutine(PerformPlayerMove());
        } else if (Input.GetKeyDown(KeyCode.X)) {
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            PlayerAction();
        }
    }
}