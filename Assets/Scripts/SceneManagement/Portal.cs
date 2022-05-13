using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour, IPlayerTriggerable {

    [SerializeField] private int sceneToLoad = -1;
    [SerializeField] private DestinationId destinationPortal;
    [SerializeField] private Transform spawnPoint;

    public Transform SpawnPoint => spawnPoint;
    
    public void OnPlayerTriggered(PlayerController player) {
        StartCoroutine(SwitchScene(player));
    }

    private IEnumerator SwitchScene(PlayerController player) {
        DontDestroyOnLoad(gameObject);
        GameController.Instance.PauseGame(true);
        
        yield return SceneManager.LoadSceneAsync(sceneToLoad);
        
        // find first portal in next scene with the same destination ID
        var destPortal = FindObjectsOfType<Portal>()
            .First(portal => portal != this && portal.destinationPortal == destinationPortal);
        player.Character.SetPositionAndSnapToTile(destPortal.SpawnPoint.position);
        
        GameController.Instance.PauseGame(false);
        Destroy(gameObject);
    }
}

public enum DestinationId {
    A, B, C, D
}