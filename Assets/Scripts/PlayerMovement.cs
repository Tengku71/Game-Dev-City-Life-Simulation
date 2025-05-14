using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Movement")]
    public float moveSpeed = 8f;

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sprite;
    private PlayerController playerController;

    private Vector2 moveInput;
    private Vector2 lastMoveDirection = Vector2.down;

    private enum MovementState { idle, idleSide, idleBack, run, runSide, runBack }

    private BoxCollider2D coll;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        coll = GetComponent<BoxCollider2D>();

        playerController = new PlayerController();
    }

    private void OnEnable()
    {
        playerController.Enable();
        playerController.Movement.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        playerController.Movement.Move.canceled += ctx => moveInput = Vector2.zero;
    }

    private void OnDisable()
    {
        playerController.Disable();
    }

    private void Update()
    {
        moveInput = playerController.Movement.Move.ReadValue<Vector2>();

        if (moveInput != Vector2.zero)
        {
            lastMoveDirection = moveInput;
        }
    }

    private void FixedUpdate()
    {
        if (moveInput != Vector2.zero)
        {
            rb.velocity = moveInput.normalized * moveSpeed;
        }
        else
        {
            rb.velocity = Vector2.zero;
        }

        UpdateAnimation();
    }

    private void UpdateAnimation()
    {
        MovementState state;

        if (moveInput.x != 0f)
        {
            state = MovementState.runSide;
            sprite.flipX = moveInput.x < 0;
        }
        else if (moveInput.y > 0f)
        {
            state = MovementState.runBack;
        }
        else if (moveInput.y < 0f)
        {
            state = MovementState.run;
        }
        else
        {
            // IDLE
            if (lastMoveDirection.x != 0)
            {
                state = MovementState.idleSide;
                sprite.flipX = lastMoveDirection.x < 0;
            }
            else if (lastMoveDirection.y > 0)
            {
                state = MovementState.idleBack;
            }
            else
            {
                state = MovementState.idle;
            }
        }

        // Make sure the animator transitions instantly with no delay
        anim.SetInteger("state", (int)state);
    }
}
