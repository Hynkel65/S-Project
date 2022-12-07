using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(GroundDetection))]
public class Enemy : MonoBehaviour
{
    [Header("Movement")]
    public float walkAcceleration = 3f;
    public float maxSpeed = 3f;
    public float wallDistance = 0.2f;
    public float walkStopRate = 0.05f;

    [Header("Agro")]
    public float agroRange = 4f;
    public float maxAgroMoveSpeed = 8f;
    [SerializeField] bool isAgro;
    [SerializeField] bool isSearching;
    [SerializeField] bool isChasing;
    public Transform player;

    [Header("Detection")]
    CapsuleCollider2D touchingCol;
    public DetectionZone attackZone;
    public DetectionZone cliffZone;
    public Transform castPoint;


    Rigidbody2D rb;
    GroundDetection touchingDirections;
    Animator animator;
    Damageable damageable;

    public enum WalkableDirection { Right, Left }
    private Vector2 walkDirectionVector = Vector2.right;
    private WalkableDirection _walkDirection;

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
        isAgro = false;
        isChasing = false;
    }

    private void Update()
    {
        if (CanSeePlayer(agroRange) && !isAgro)
        {
            //agro enemy
            isAgro = true;
        }
        else
        {
            if (isAgro && touchingDirections.isGrounded)
            {
                isChasing = true;
                ChasePlayer();

                if (!isSearching)
                {
                    StartCoroutine(StopChasingPlayer());
                }
            }
        }

        if (!isChasing)
        {
            if (cliffZone.detectedColliders.Count == 0)
            {
                if (!isAgro && !isSearching)
                {
                    FlipDirection();
                }
            }
        }
        HasTarget = attackZone.detectedColliders.Count > 0;

        if(attackCooldown > 0)
            attackCooldown -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        if (!isAgro && !isSearching)
        {
            if (touchingDirections.isGrounded && touchingDirections.isOnWall)
            {
                FlipDirection();
            }

            if (!damageable.LockMovement)
            {
                Move();
            }
        }
    }

    private void Move()
    {
        if (canMove && touchingDirections.isGrounded)
            //rb.velocity = new Vector2(maxSpeed, 0);
            rb.velocity = new Vector2(MathF.Min(Math.Max(rb.velocity.x + (walkAcceleration * walkDirectionVector.x * Time.deltaTime), -maxSpeed), maxSpeed), rb.velocity.y);
        else
            rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, 0, walkStopRate), rb.velocity.y);
    }

    bool CanSeePlayer(float distance)
    {
        bool value = false;
        float castDist = distance;

        if (_walkDirection == 0)
        {
            castDist = distance;
        }
        else
        {
            castDist = -distance;
        }
        
        Vector2 endPos = castPoint.position + Vector3.right * castDist;
        RaycastHit2D hit = Physics2D.Linecast(castPoint.position, endPos, 1 << LayerMask.NameToLayer("Player"));

        if (hit.collider != null)
        {
            if (hit.collider.gameObject.CompareTag("Player"))
            {
                value = true;
            }
            else
            {
                value = false;
            }

            Debug.DrawLine(castPoint.position, hit.point, Color.red);

        }
        else
            Debug.DrawLine(castPoint.position, endPos, Color.blue);

        return value;
    }

    private void ChasePlayer()
    {
        if (transform.position.x < player.position.x)
        {
            //enemy is to the left of the player, then move right
            if (cliffZone.detectedColliders.Count > 0)
            {
                if (canMove && touchingDirections.isGrounded)
                    //rb.velocity = new Vector2(maxAgroMoveSpeed, 0);
                    rb.velocity = new Vector2(MathF.Min(Math.Max(rb.velocity.x + (walkAcceleration * walkDirectionVector.x * Time.deltaTime), -maxAgroMoveSpeed), maxAgroMoveSpeed), rb.velocity.y);
                else
                    rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, 0, walkStopRate), rb.velocity.y);
                    
            }
            else
            {
                rb.velocity = new Vector2(0, 0);
            }
            WalkDirection = WalkableDirection.Right;
        }
        else if (transform.position.x > player.position.x)
        {
            //enemy is to the right of the player, then move left  
            if (cliffZone.detectedColliders.Count > 0)
            {
                if (canMove && touchingDirections.isGrounded)
                    //rb.velocity = new Vector2(-maxAgroMoveSpeed, 0);
                    rb.velocity = new Vector2(MathF.Min(Math.Max(rb.velocity.x + (walkAcceleration * walkDirectionVector.x * Time.deltaTime), maxAgroMoveSpeed), -maxAgroMoveSpeed), rb.velocity.y);
                else
                    rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, 0, walkStopRate), rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2(0, 0);
            }
            WalkDirection = WalkableDirection.Left;
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

    IEnumerator StopChasingPlayer()
    {
        isSearching = true;
        yield return new WaitForSeconds(5);
        isAgro = false;
        isSearching = false;
        isChasing = false;
    }
}
