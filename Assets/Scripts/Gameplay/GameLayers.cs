using UnityEngine;

public class GameLayers : MonoBehaviour
{
    [SerializeField] private LayerMask solidObjectsLayer;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private LayerMask grassLayer;

    public static GameLayers instance { get; set; }
    
    public LayerMask SolidObjectsLayer => solidObjectsLayer;
    public LayerMask InteractableLayer => interactableLayer;
    public LayerMask GrassLayer => grassLayer;

    private void Awake() {
        instance = this;
    }
}
