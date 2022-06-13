using UnityEngine;

public class EssentialObjectsSpawner : MonoBehaviour {

    [SerializeField] private GameObject essentialObjectsPrefab;

    private void Awake() {
        var existingObjects = FindObjectsOfType<EssentialObjects>();
        if (existingObjects.Length == 0) {
            // If there's a grid, spawn at its center
            var spawnPos = new Vector3(0, 0, 0);
            var grid = FindObjectOfType<Grid>();
            if (grid != null) {
                spawnPos = grid.transform.position;
            }
            Debug.Log($"Spawning at {spawnPos}");
            
            Instantiate(essentialObjectsPrefab, spawnPos, Quaternion.identity);
        }
    }
}