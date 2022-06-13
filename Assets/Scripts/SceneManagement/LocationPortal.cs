using System.Collections;
using System.Linq;
using UnityEngine;

// Teleports the player to a different position without switching scenes
public class LocationPortal : MonoBehaviour, IPlayerTriggerable {
    
    [SerializeField] private DestinationId destinationPortal;
    [SerializeField] private Transform spawnPoint;

    public Transform SpawnPoint => spawnPoint;

    private Fader fader;

    private void Start() {
        fader = FindObjectOfType<Fader>();
    }
    
    public void OnPlayerTriggered(PlayerController player) {
        player.Character.Animator.IsMoving = false;
        StartCoroutine(Teleport(player));
    }

    private IEnumerator Teleport(PlayerController player) {
        GameController.Instance.PauseGame(true);
        
        yield return fader.FadeIn(0.5f);
        
        // find first portal in next scene with the same destination ID
        var destPortal = FindObjectsOfType<LocationPortal>()
            .First(portal => portal != this && portal.destinationPortal == destinationPortal);
        player.Character.SetPositionAndSnapToTile(destPortal.SpawnPoint.position);

        yield return fader.FadeOut(0.5f);
        
        GameController.Instance.PauseGame(false);
    }
}