using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(GroundDetection))]
public class Slime : MonoBehaviour
{
    public float walkSpeed = 3f;
    public float wallDistance = 0.2f;
    public float walkStopRate = 0.05f;

    public ContactFilter2D castFilter;
    CapsuleCollider2D touchingCol;
    public DetectionZone attackZone;
    public DetectionZone cliffZone;
    
    
    Rigidbody2D rb;
    GroundDetection touchingDirections;
    Animator animator;
    Damageable damageable;

    public enum WalkableDirection { Right, Left }
    private Vector2 walkDirectionVector = Vector2.right;
    private WalkableDirection _walkDirection;

    RaycastHit2D[] wallHits = new RaycastHit2D[5];
    public WalkableDirection WalkDirection
    {
        get { return _walkDirection; }
        set { 
            if(_walkDirection != value)
            {
                //Direction flipped
                gameObject.transform.localScale = new Vector2(gameObject.transform.localScale.x * -1, gameObject.transform.localScale.y);

                if(value == WalkableDirection.Right)
                {
                    walkDirectionVector = Vector2.right;
                } else if(value == WalkableDirection.Left)
                {
                    walkDirectionVector = Vector2.left;
                }
            }
            
            _walkDirection = value; }
    }

    public bool _hasTarget = false;

    public bool HasTarget { get {return _hasTarget;} private set
        {
            _hasTarget = value;
            animator.SetBool("hasTarget", value);
        } 
    }

    public bool canMove
    {
        get
        {
            return animator.GetBool("canMove");
        }
    }

    [SerializeField]
    private bool _isOnWall;

    private Vector2 wallCheckDirection => gameObject.transform.localScale.x > 0 ? Vector2.right : Vector2.left;

    public bool isOnWall 
    { 
        get 
        {
            return _isOnWall;
        } 
        private set
        {
            _isOnWall = value;
            animator.SetBool("isOnWall", _isOnWall);
        } 
    }

    public float attackCooldown 
    {
        get
        {
            return animator.GetFloat("attackCooldown");
        }
        private set
        {
            animator.SetFloat("attackCooldown", Mathf.Max(value, 0));
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        touchingDirections = GetComponent<GroundDetection>();
        animator = GetComponent<Animator>();
        touchingCol = GetComponent<CapsuleCollider2D>();
        damageable = GetComponent<Damageable>();
    }

    private void Update()
    {
        HasTarget = attackZone.detectedColliders.Count > 0;

        if(attackCooldown > 0)
            attackCooldown -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        isOnWall = touchingCol.Cast(wallCheckDirection, castFilter, wallHits, wallDistance) > 0;

        if (touchingDirections.isGrounded && isOnWall)
        {
            FlipDirection();
        }

        if (!damageable.LockMovement)
        {
            if (canMove /*&& touchingDirections.isGrounded*/)
                rb.velocity = new Vector2(walkSpeed * walkDirectionVector.x, rb.velocity.y);
            else
                rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, 0, walkStopRate), rb.velocity.y);
        }
    }

    private void FlipDirection()
    {
        if(WalkDirection == WalkableDirection.Right)
        {
            WalkDirection = WalkableDirection.Left;
        } else //if (WalkDirection == WalkableDirection.Left)
        {
            WalkDirection = WalkableDirection.Right;
        }
    }

    public void OnHit(int damage, Vector2 knockback)
    {
        rb.velocity = new Vector2(knockback.x, rb.velocity.y + knockback.y);
    }

    public void OncliffDetected()
    {
        if (touchingDirections.isGrounded)
            FlipDirection();
    }
}
