using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(GroundDetection), typeof(Damageable))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float movementSpeed = 8f;
    [SerializeField] bool _isMoving = false;
    public PhysicsMaterial2D noFric;
    public PhysicsMaterial2D fullFric;

    [Header("Jump")]
    public float jumpPower = 10f;
    public float fallMultiplier = 3f;
    public float jumpMultiplier = 1.2f;
    private int extraJumps;
    public int extraJumpsValue = 1;
    Vector2 vecGravity;

    [Header("Turn")]
    public bool _isFacingRight;
    public Transform firePoint;

    [Header("Dash")]
    public float dashPower = 200f;
    public float dashTime = 0.2f;
    public float dashCooldown = 2f;
    TrailRenderer dashTrail;
    [SerializeField] bool canDash = true;
    public bool _isDashing = false;

    [Header("Others")]
    public GameObject enemies;

    Vector2 moveInput;
    Rigidbody2D rb;
    Animator animator;
    GroundDetection detection;
    Damageable damageable;

    public float CurrentMoveSpeed
    {
        get
        {
            if (IsMoving && canMove)
            {
                if (IsDashing)
                {
                    return dashPower;
                }
                else
                {
                    return movementSpeed;
                }
            }
            else
            {
                //idle speed is 0
                return 0;
            }
        }
    }

    public bool IsMoving
    {
        get
        {
            return _isMoving;
        }
        private set
        {
            _isMoving = value;
            animator.SetBool("isMoving", _isMoving);
        }
    }

    public bool IsDashing
    {
        get
        {
            return _isDashing;
        }
        private set
        {
            _isDashing = value;
            animator.SetBool("isDashing", value);
        }
    }

    public bool isFacingRight
    {
        get { return _isFacingRight; }
        private set
        {
            if (isFacingRight != value)
            {
                transform.localScale *= new Vector2(-1, 1);
            }

            _isFacingRight = value;
        }
    }

    public bool canMove
    {
        get
        {
            return animator.GetBool("canMove");
        }
    }

    public bool isAlive
    {
        get
        {
            return animator.GetBool("isAlive");
        }
    }

    public bool lockMovement
    {
        get
        {
            return animator.GetBool("lockMovement");
        }
        set
        {
            animator.SetBool("lockMovement", value);
        }
    }


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        detection = GetComponent<GroundDetection>();
        dashTrail = GetComponent<TrailRenderer>();
        damageable = GetComponent<Damageable>();
        _isFacingRight = true;
    }

    private void Start()
    {
        extraJumps = extraJumpsValue;
        vecGravity = new Vector2(0, -Physics2D.gravity.y);
    }

    private void Update()
    {
        if (_isDashing)
        {
            return;
        }

        if (rb.velocity.y < 0)
            rb.velocity -= vecGravity * fallMultiplier * Time.deltaTime;

        if (!isAlive)
        {
            Destroy(enemies.gameObject);
            rb.sharedMaterial = fullFric;
        }

        //if (detection.isOnSlope && !IsMoving)
        //{
        //    rb.sharedMaterial = fullFric;
        //}
        //else
        //{
        //    rb.sharedMaterial = noFric;
        //}
    }

    private void FixedUpdate()
    {
        if (_isDashing)
        {
            return;
        }

        ApplyMovement();
        animator.SetFloat("yVelocity", rb.velocity.y);
    }

    private void ApplyMovement()
    {
        rb.velocity = new Vector2(moveInput.x * CurrentMoveSpeed, rb.velocity.y);

        //if (!damageable.LockMovement)
        //{
        //    //rb.velocity = new Vector2(moveInput.x * CurrentMoveSpeed, rb.velocity.y);

        //    if (detection.isGrounded && !detection.isOnSlope)
        //    {
        //        rb.velocity = new Vector2(moveInput.x * CurrentMoveSpeed, 0.0f);
        //    }
        //    else if (detection.isGrounded && detection.isOnSlope)
        //    {
        //        rb.velocity = new Vector2(moveInput.x * CurrentMoveSpeed * detection.slopeNormalPerp.x, moveInput.x * CurrentMoveSpeed * detection.slopeNormalPerp.y);
        //    }
        //    else if (!detection.isGrounded)
        //    {
        //        rb.velocity = new Vector2(moveInput.x * CurrentMoveSpeed, rb.velocity.y);
        //    }
        //}
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        if (isAlive)
        {
            IsMoving = moveInput != Vector2.zero;

            Turn(moveInput);
        }
        else
        {
            IsMoving = false;
        }
    }

    private void Turn(Vector2 moveInput)
    {
        if (moveInput.x > 0 && !isFacingRight)
        {
            isFacingRight = true;
        }
        else if (moveInput.x < 0 && isFacingRight)
        {
            isFacingRight = false;
        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed && canDash)
        {
            StartCoroutine(Dash());
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (isAlive && !_isDashing)
        {
            if (detection.isGrounded)
            {
                extraJumps = extraJumpsValue;
            }

            if (context.started && extraJumps == 0 && detection.isGrounded)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpPower * jumpMultiplier);
            }
            else if (context.started && extraJumps > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpPower * jumpMultiplier);
                extraJumps--;
            }
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            animator.SetTrigger("attack");
        }
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            animator.SetTrigger("shoot");
        }
    }

    public void OnHit(int damage, Vector2 knockback)
    {
        rb.velocity = new Vector2(knockback.x, rb.velocity.y + knockback.y);
    }

    IEnumerator Dash()
    {
        animator.SetTrigger("dash");
        canDash = false;
        IsDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0;
        rb.velocity = new Vector2(transform.localScale.x * CurrentMoveSpeed, 0);
        dashTrail.emitting = true;
        yield return new WaitForSeconds(dashTime);
        dashTrail.emitting = false;
        rb.gravityScale = originalGravity;
        IsDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}