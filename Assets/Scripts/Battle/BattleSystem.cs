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

    private BattleState state;
    private int currentAction;
    private int currentMove;

    public void StartBattle() {
        StartCoroutine(SetupBattle());
    }

    public void HandleUpdate() {
        if (state == BattleState.PlayerAction) {
            HandleActionSelection();
        } else if (state == BattleState.PlayerMove) {
            HandleMoveSelection();
        }
    }

    private IEnumerator SetupBattle() {
        playerUnit.Setup();
        enemyUnit.Setup();
        playerHUD.SetData(playerUnit.Pokemon);
        enemyHUD.SetData(enemyUnit.Pokemon);
        dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);

        yield return dialogBox.TypeDialog($"A wild {enemyUnit.Pokemon.Base.Name} appeared!");

        PlayerAction();
    }

    private void PlayerAction() {
        state = BattleState.PlayerAction;
        StartCoroutine(dialogBox.TypeDialog("Choose an action"));
        dialogBox.EnableActionSelector(true);
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
            OnBattleOver(false);
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
        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            if (currentAction < 1)
                currentAction += 1;
        } else if (Input.GetKeyDown(KeyCode.UpArrow)) {
            if (currentAction > 0)
                currentAction -= 1;
        }

        dialogBox.UpdateActionSelection(currentAction);

        if (Input.GetKeyDown(KeyCode.Z)) {
            if (currentAction == 0) { // Fight
                PlayerMove();
            } else if (currentAction == 1) { // Run
                //run
            }
        }
    }

    private void HandleMoveSelection() {
        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            if (currentMove < playerUnit.Pokemon.Moves.Count - 1)
                currentMove += 1;
        } else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            if (currentMove > 0)
                currentMove -= 1;
        } else if (Input.GetKeyDown(KeyCode.DownArrow)) {
            if (currentMove < playerUnit.Pokemon.Moves.Count - 2)
                currentMove += 2;
        } else if (Input.GetKeyDown(KeyCode.UpArrow)) {
            if (currentMove > 1)
                currentMove -= 2;
        }

        dialogBox.UpdateMoveSelection(currentMove, playerUnit.Pokemon.Moves[currentMove]);

        if (Input.GetKeyDown(KeyCode.Z)) {
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            StartCoroutine(PerformPlayerMove());
        }
    }
}