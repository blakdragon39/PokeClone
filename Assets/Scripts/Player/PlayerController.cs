using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    public LayerMask solidObjectsLayer;
    
    private bool isMoving;
    private Vector2 input;

    private Animator animator;
    private int moveXId;
    private int moveYId;
    private int isMovingId;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        moveXId = Animator.StringToHash("moveX");
        moveYId = Animator.StringToHash("moveY");
        isMovingId = Animator.StringToHash("isMoving");
    }

    public void Update()
    {
        if (!isMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");
            
            if (input.x != 0) input.y = 0; //disables diagonal movement  
            
            if (input != Vector2.zero)
            {
                animator.SetFloat(moveXId, input.x);
                animator.SetFloat(moveYId, input.y);
                
                var targetPos = transform.position;
                targetPos.x += input.x;
                targetPos.y += input.y;

                if (IsWalkable(targetPos))
                {
                    StartCoroutine(Move(targetPos));
                }
            }
        }
        
        animator.SetBool(isMovingId, isMoving);
    }

    private IEnumerator Move(Vector3 targetPos)
    {
        isMoving = true;
        
        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPos;
        isMoving = false;
    }

    private bool IsWalkable(Vector3 targetPos)
    {
        return Physics2D.OverlapCircle(targetPos, 0.2f, solidObjectsLayer) == null;
    }
}
