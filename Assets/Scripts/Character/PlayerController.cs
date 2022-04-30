using UnityEngine;

public class PlayerController : MonoBehaviour {

    [SerializeField] private string name;
    [SerializeField] private Sprite sprite;

    private const float offsetY = 0.3f;
    
    public string Name => name;
    public Sprite Sprite => sprite;

    private Vector2 input;

    private Character character;

    private void Awake() {
        character = GetComponent<Character>();
    }

    public void HandleUpdate() {
        if (!character.IsMoving) {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            if (input.x != 0) input.y = 0; //disables diagonal movement  

            if (input != Vector2.zero) {
                StartCoroutine(character.Move(input, OnMoveOver));
            }
        }
        
        character.HandleUpdate();

        if (Input.GetKeyDown(KeyCode.Z)) {
            Interact();
        }
    }

    private void Interact() {
        var facingDir = new Vector3(character.Animator.MoveX, character.Animator.MoveY);
        var interactPos = transform.position + facingDir;
        // Debug.DrawLine(transform.position, interactPos, Color.green, 0.5f);

        var collider = Physics2D.OverlapCircle(interactPos, 0.3f, GameLayers.instance.InteractableLayer);

        if (collider != null) {
            collider.GetComponent<Interactable>()?.Interact(transform);
        }
    }

    private void OnMoveOver() {
        var colliders = Physics2D.OverlapCircleAll(
            transform.position - new Vector3(0, offsetY), 
            0.2f, 
            GameLayers.instance.TriggerableLayers
        );

        foreach (var collider in colliders) {
            var triggerable = collider.GetComponent<IPlayerTriggerable>();

            if (triggerable != null) {
                character.Animator.IsMoving = false;
                triggerable.OnPlayerTriggered(this);
                break;
            }
        }
    }
}