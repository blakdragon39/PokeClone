using UnityEngine;

public enum GameState {
    FreeRoam, Battle, Dialog, Cutscene
}

public class GameController : MonoBehaviour {

    public static GameController Instance { get; private set; }
    
    [SerializeField] private PlayerController playerController;
    [SerializeField] private BattleSystem battleSystem;
    [SerializeField] private Camera worldCamera;
    
    private GameState state;

    private void Awake() {
        Instance = this;
        ConditionsDB.Init();
    }
    
    private void Start() {
        playerController.OnEncountered += StartBattle;
        battleSystem.OnBattleOver += EndBattle;
        playerController.OnEnterTrainersView += TriggerTrainerBattle;
        
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
    
    public void StartTrainerBattle(TrainerController trainer) {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        var playerParty = playerController.GetComponent<PokemonParty>();
        var trainerParty = trainer.GetComponent<PokemonParty>();
        
        battleSystem.StartTrainerBattle(playerParty, trainerParty);
    }

    private void EndBattle(bool won) {
        state = GameState.FreeRoam;
        battleSystem.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);
    }

    private void TriggerTrainerBattle(Collider2D trainerCollider) {
        var trainer = trainerCollider.GetComponentInParent<TrainerController>();
        if (trainer != null) {
            state = GameState.Cutscene;
            StartCoroutine(trainer.TriggerTrainerBattle(playerController));
        }
    }
}
