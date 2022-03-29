using UnityEngine;

public enum GameState {
    FreeRoam, Battle
}

public class GameController : MonoBehaviour {

    [SerializeField] private PlayerController playerController;
    [SerializeField] private BattleSystem battleSystem;
    [SerializeField] private Camera worldCamera;
    
    private GameState state;

    private void Start() {
        playerController.OnEncountered += StartBattle;
        battleSystem.OnBattleOver += EndBattle;
    }
    
    private void Update() {
        switch (state) {
            case GameState.FreeRoam:
                playerController.HandleUpdate();
                break;
            case GameState.Battle:
                battleSystem.HandleUpdate();
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
