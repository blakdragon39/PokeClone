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
    [SerializeField] private BattleUnit playerUnit;
    [SerializeField] private BattleUnit enemyUnit;
    [SerializeField] private BattleHUD playerHUD;
    [SerializeField] private BattleHUD enemyHUD;
    [SerializeField] private BattleDialog dialogBox;

    private BattleState state;
    private int currentAction;
    private int currentMove;

    private void Start() {
        StartCoroutine(SetupBattle());
    }

    private void Update() {
        if (state == BattleState.PlayerAction) {
            HandleActionSelection();
        }
        else if (state == BattleState.PlayerMove) {
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
        yield return new WaitForSeconds(1f);

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
        yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} used {move.Base.Name}");
        yield return new WaitForSeconds(1f);

        bool fainted = enemyUnit.Pokemon.TakeDamage(move, playerUnit.Pokemon);
        yield return enemyHUD.UpdateHP();

        if (fainted) {
            yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.Name} fainted");
        } else {
            StartCoroutine(PerformEnemyMove());
        }
    }

    private IEnumerator PerformEnemyMove() {
        state = BattleState.EnemyMove;

        var move = enemyUnit.Pokemon.GetRandomMove();
        yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.Name} used {move.Base.Name}");
        yield return new WaitForSeconds(1f);

        bool fainted = playerUnit.Pokemon.TakeDamage(move, enemyUnit.Pokemon);
        playerHUD.UpdateHP();

        if (fainted) {
            yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} fainted");
        } else {
            PlayerAction();
        }
    }

    private void HandleActionSelection() {
        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            if (currentAction < 1)
                currentAction += 1;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow)) {
            if (currentAction > 0)
                currentAction -= 1;
        }

        dialogBox.UpdateActionSelection(currentAction);

        if (Input.GetKeyDown(KeyCode.Z)) {
            if (currentAction == 0) // Fight
            {
                PlayerMove();
            }
            else if (currentAction == 1) // Run
            {
                //run
            }
        }
    }

    private void HandleMoveSelection() {
        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            if (currentMove < playerUnit.Pokemon.Moves.Count - 1)
                currentMove += 1;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            if (currentMove > 0)
                currentMove -= 1;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow)) {
            if (currentMove < playerUnit.Pokemon.Moves.Count - 2)
                currentMove += 2;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow)) {
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