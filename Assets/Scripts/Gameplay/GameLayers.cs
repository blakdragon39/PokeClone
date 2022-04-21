using UnityEngine;

public class GameLayers : MonoBehaviour
{
    [SerializeField] private LayerMask solidObjectsLayer;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private LayerMask grassLayer;
    [SerializeField] private LayerMask playerLayer;

    public static GameLayers instance { get; set; }
    
    public LayerMask SolidObjectsLayer => solidObjectsLayer;
    public LayerMask InteractableLayer => interactableLayer;
    public LayerMask GrassLayer => grassLayer;
    public LayerMask PlayerLayer => playerLayer;

    private void Awake() {
        instance = this;
    }
}
