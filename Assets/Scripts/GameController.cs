using UnityEngine;

public enum GameState {
    FreeRoam, Battle, Dialog, Cutscene, Paused
}

public class GameController : MonoBehaviour {

    public static GameController Instance { get; private set; }
    
    [SerializeField] private PlayerController playerController;
    [SerializeField] private BattleSystem battleSystem;
    [SerializeField] private Camera worldCamera;

    private GameState state;
    private GameState stateBeforePause;
    private TrainerController trainer;

    private void Awake() {
        Instance = this;
        ConditionsDB.Init();
    }
    
    private void Start() {
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

    public void PauseGame(bool pause) {
        if (pause) {
            stateBeforePause = state;
            state = GameState.Paused;
        } else {
            state = stateBeforePause;
        }
    }

    public void StartBattle() {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        var playerParty = playerController.GetComponent<PokemonParty>();
        var wildPokemon = FindObjectOfType<MapArea>().GetComponent<MapArea>().GetRandomWildPokemon();
        var wildPokemonCopy = new Pokemon(wildPokemon.Base, wildPokemon.Level);
        
        battleSystem.StartBattle(playerParty, wildPokemonCopy);
    }
    
    public void StartTrainerBattle(TrainerController trainer) {
        this.trainer = trainer;
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        var playerParty = playerController.GetComponent<PokemonParty>();
        var trainerParty = trainer.GetComponent<PokemonParty>();
        
        battleSystem.StartTrainerBattle(playerParty, trainerParty);
    }

    public void OnEnterTrainersView(TrainerController trainer) {
        state = GameState.Cutscene;
        StartCoroutine(trainer.TriggerTrainerBattle(playerController));
    }

    private void EndBattle(bool won) {
        if (won && trainer != null) {
            trainer.BattleLost();
            trainer = null;
        }
        
        state = GameState.FreeRoam;
        battleSystem.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);
    }
}
