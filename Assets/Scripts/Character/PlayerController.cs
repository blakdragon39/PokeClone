using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour {
    
    public float moveSpeed;
    public LayerMask solidObjectsLayer;
    public LayerMask interactableLayer;
    public LayerMask grassLayer;

    public event Action OnEncountered;

    private bool isMoving;
    private Vector2 input;

    private CharacterAnimator animator;

    private void Awake() {
        animator = GetComponent<CharacterAnimator>();
    }

    public void HandleUpdate() {
        if (!isMoving) {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            if (input.x != 0) input.y = 0; //disables diagonal movement  

            if (input != Vector2.zero) {
                animator.MoveX = input.x;
                animator.MoveY = input.y;

                var targetPos = transform.position;
                targetPos.x += input.x;
                targetPos.y += input.y;

                if (IsWalkable(targetPos)) {
                    StartCoroutine(Move(targetPos));
                }
            }
        }

        animator.IsMoving = isMoving;

        if (Input.GetKeyDown(KeyCode.Z)) {
            Interact();
        }
    }

    private void Interact() {
        var facingDir = new Vector3(animator.MoveX, animator.MoveY);
        var interactPos = transform.position + facingDir;
        // Debug.DrawLine(transform.position, interactPos, Color.green, 0.5f);

        var collider = Physics2D.OverlapCircle(interactPos, 0.3f, interactableLayer);

        if (collider != null) {
            collider.GetComponent<Interactable>()?.Interact();
        }
    }
    
    private IEnumerator Move(Vector3 targetPos) {
        isMoving = true;

        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon) {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPos;
        isMoving = false;

        CheckForEncounters();
    }

    private bool IsWalkable(Vector3 targetPos) {
        return Physics2D.OverlapCircle(targetPos, 0.2f, solidObjectsLayer | interactableLayer) == null;
    }

    private void CheckForEncounters() {
        if (Physics2D.OverlapCircle(transform.position, 0.2f, grassLayer) != null) {
            if (Random.Range(1, 101) <= 10) {
                animator.IsMoving = false;
                OnEncountered();
            }
        }
    }
}