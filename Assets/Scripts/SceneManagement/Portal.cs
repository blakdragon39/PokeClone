using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour, IPlayerTriggerable {

    [SerializeField] private int sceneToLoad = -1;
    [SerializeField] private Transform spawnPoint;

    public Transform SpawnPoint => spawnPoint;
    
    public void OnPlayerTriggered(PlayerController player) {
        StartCoroutine(SwitchScene(player));
    }

    private IEnumerator SwitchScene(PlayerController player) {
        DontDestroyOnLoad(gameObject);
        yield return SceneManager.LoadSceneAsync(sceneToLoad);
        
        // find first portal in next scene
        var destPortal = FindObjectsOfType<Portal>().First(portal => portal != this);
        player.Character.SetPositionAndSnapToTile(destPortal.SpawnPoint.position);
        
        Destroy(gameObject);
    }
}