using System;
using System.Collections;
using UnityEngine;

public class Character : MonoBehaviour {

    public float moveSpeed;
    public bool IsMoving { get; set; }
    public float OffsetY { get; private set; } = 0.3f;
    
    private CharacterAnimator animator;

    public CharacterAnimator Animator => animator;

    private void Awake() {
        animator = GetComponent<CharacterAnimator>();
        SetPositionAndSnapToTile(transform.position);
    }

    public void SetPositionAndSnapToTile(Vector2 pos) {
        pos.x = Mathf.Floor(pos.x) + 0.5f;
        pos.y = Mathf.Floor(pos.y) + 0.5f + OffsetY;
        transform.position = pos;
    }
    
    public IEnumerator Move(Vector2 moveVector, Action OnMoveOver=null) {
        animator.MoveX = Mathf.Clamp(moveVector.x, -1f, 1f);
        animator.MoveY = Mathf.Clamp(moveVector.y, -1f, 1f);

        var targetPos = transform.position;
        targetPos.x += moveVector.x;
        targetPos.y += moveVector.y;

        if (!IsPathClear(targetPos))
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

    private bool IsPathClear(Vector3 targetPos) {
        var diff = targetPos - transform.position;
        var dir = diff.normalized; //same direction as diff, but with length of 1
        return !Physics2D.BoxCast(
            transform.position + dir, 
            new Vector2(0.2f, 0.2f), 
            0f, 
            dir, 
            diff.magnitude - 1,
            GameLayers.instance.SolidObjectsLayer | GameLayers.instance.InteractableLayer | GameLayers.instance.PlayerLayer
        );
    }
    
    private bool IsWalkable(Vector3 targetPos) {
        return Physics2D.OverlapCircle(
            targetPos, 
            0.2f, 
            GameLayers.instance.SolidObjectsLayer | GameLayers.instance.InteractableLayer
        ) == null;
    }

    public void LookTowards(Vector3 targetPos) {
        var xDiff = Mathf.Floor(targetPos.x) - Mathf.Floor(transform.position.x);
        var yDiff = Mathf.Floor(targetPos.y - Mathf.Floor(transform.position.y));

        if (xDiff == 0 || yDiff == 0) {
            animator.MoveX = Mathf.Clamp(xDiff, -1f, 1f);
            animator.MoveY = Mathf.Clamp(yDiff, -1f, 1f);
        } else {
            Debug.LogError("Error in LookTowards: Character cannot look diaginally");
        }
    }
}