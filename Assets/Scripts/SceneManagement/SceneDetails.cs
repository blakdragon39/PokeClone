using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDetails : MonoBehaviour {

    [SerializeField] private List<SceneDetails> connectedScenes;
    
    public bool IsLoaded { get; private set; }
    
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            LoadScene();
            GameController.Instance.SetCurrentScene(this);
            
            foreach (var scene in connectedScenes) {
                scene.LoadScene();
            }

            // Unload scenes that are no longer connected
            if (GameController.Instance.PrevScene != null) {
                var previouslyLoadedScenes = GameController.Instance.PrevScene.connectedScenes;

                foreach (var scene in previouslyLoadedScenes) {
                    if (!connectedScenes.Contains(scene) && scene != this) {
                        scene.UnloadScene();
                    }
                }
            }
        }
    }

    public void LoadScene() {
        if (!IsLoaded) {
            SceneManager.LoadSceneAsync(gameObject.name, LoadSceneMode.Additive);
            IsLoaded = true;
        }
    }
    
    public void UnloadScene() {
        if (IsLoaded) {
            SceneManager.UnloadSceneAsync(gameObject.name);
            IsLoaded = false;
        }
    }
}