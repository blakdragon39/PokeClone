using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour, IPlayerTriggerable {

    [SerializeField] private int sceneToLoad = -1;
    [SerializeField] private DestinationId destinationPortal;
    [SerializeField] private Transform spawnPoint;

    public Transform SpawnPoint => spawnPoint;

    private Fader fader;

    private void Start() {
        fader = FindObjectOfType<Fader>();
    }
    
    public void OnPlayerTriggered(PlayerController player) {
        StartCoroutine(SwitchScene(player));
    }

    private IEnumerator SwitchScene(PlayerController player) {
        DontDestroyOnLoad(gameObject);
        GameController.Instance.PauseGame(true);
        
        yield return fader.FadeIn(0.5f);
        yield return SceneManager.LoadSceneAsync(sceneToLoad);
        
        // find first portal in next scene with the same destination ID
        var destPortal = FindObjectsOfType<Portal>()
            .First(portal => portal != this && portal.destinationPortal == destinationPortal);
        player.Character.SetPositionAndSnapToTile(destPortal.SpawnPoint.position);

        yield return fader.FadeOut(0.5f);
        
        GameController.Instance.PauseGame(false);
        Destroy(gameObject);
    }
}

public enum DestinationId {
    A, B, C, D
}