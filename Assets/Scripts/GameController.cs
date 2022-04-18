using UnityEngine;

public enum GameState {
    FreeRoam, Battle, Dialog
}

public class GameController : MonoBehaviour {

    [SerializeField] private PlayerController playerController;
    [SerializeField] private BattleSystem battleSystem;
    [SerializeField] private Camera worldCamera;
    
    private GameState state;

    private void Start() {
        playerController.OnEncountered += StartBattle;
        battleSystem.OnBattleOver += EndBattle;
        
        DialogManager.Instance.OnShowDialog += () => state = GameState.Dialog;
        DialogManager.Instance.OnCloseDialog += () => {
            if (state == GameState.Dialog)
                state = GameState.FreeRoam;
        };
    }
    
    private void Update() {
        switch (state) {
            case GameState.FreeRoam:
                playerController.HandleUpdate();
                break;
            case GameState.Battle:
                battleSystem.HandleUpdate();
                break;
            case GameState.Dialog:
                DialogManager.Instance.HandleUpdate();
                break;
        }
    }

    private void StartBattle() {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        var playerParty = playerController.GetComponent<PokemonParty>();
        var wildPokemon = FindObjectOfType<MapArea>().GetComponent<MapArea>().GetRandomWildPokemon();
        
        battleSystem.StartBattle(playerParty, wildPokemon);
    }

    private void EndBattle(bool won) {
        state = GameState.FreeRoam;
        battleSystem.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);
    }
}
