using System;
using System.Collections;
using UnityEngine;

public class Character : MonoBehaviour {

    public float moveSpeed;
    public bool IsMoving { get; set; }
    
    private CharacterAnimator animator;

    public CharacterAnimator Animator => animator;

    private void Awake() {
        animator = GetComponent<CharacterAnimator>();
    }
    
    public IEnumerator Move(Vector2 moveVector, Action OnMoveOver=null) {
        animator.MoveX = Mathf.Clamp(moveVector.x, -1f, 1f);
        animator.MoveY = Mathf.Clamp(moveVector.y, -1f, 1f);

        var targetPos = transform.position;
        targetPos.x += moveVector.x;
        targetPos.y += moveVector.y;

        if (!IsWalkable(targetPos))
            yield break;

        IsMoving = true;

        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon) {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPos;
        IsMoving = false;

        OnMoveOver?.Invoke();
    }

    public void HandleUpdate() {
        animator.IsMoving = IsMoving;
    }
    
    private bool IsWalkable(Vector3 targetPos) {
        return Physics2D.OverlapCircle(
            targetPos, 
            0.2f, 
            GameLayers.instance.SolidObjectsLayer | GameLayers.instance.InteractableLayer
        ) == null;
    }
}