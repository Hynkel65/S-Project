using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    [Header("Movement")]
    public float walkAcceleration = 3f;
    public float maxSpeed = 3f;
    public float walkStopRate = 0.05f;

    [SerializeField] float baseCastDist;
    [SerializeField] Transform DetectionCastPoint;

    [Header("Agro")]
    public float agroRange = 4f;
    public float maxAgroMoveSpeed = 8f;
    [SerializeField] bool isAgro;
    [SerializeField] bool isSearching;
    public Transform player;

    [Header("Detection")]
    public DetectionZone attackZone;
    public Transform castPoint;


    Rigidbody2D rb;
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
        animator = GetComponent<Animator>();
        damageable = GetComponent<Damageable>();
        isAgro = false;
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
            if (isAgro)
            {
                ChasePlayer();

                if (!isSearching)
                {
                    StartCoroutine(StopChasingPlayer());
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
            if (!damageable.LockMovement)
            {
                Move();
            }
        }

        if (IsHittingWall() || IsNearEdge())
        {
            FlipDirection();
        }
    }

    private void Move()
    {
        if (canMove)
            rb.velocity = new Vector2(MathF.Min(Math.Max(rb.velocity.x + (walkAcceleration * walkDirectionVector.x * Time.deltaTime), -maxSpeed), maxSpeed), rb.velocity.y);
        else
            rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, 0, walkStopRate), rb.velocity.y);
    }

    bool IsHittingWall()
    {
        bool value = false;

        float castDist = baseCastDist;

        if (walkDirectionVector == Vector2.left)
        {
            castDist = -baseCastDist;
        }

        Vector3 targetPos = DetectionCastPoint.position;
        targetPos.x += castDist;

        Debug.DrawLine(DetectionCastPoint.position, targetPos, Color.blue);

        if (Physics2D.Linecast(DetectionCastPoint.position, targetPos, 1 << LayerMask.NameToLayer("Ground")))
        {
            value = true;
        }
        else
        {
            value = false;
        }

        return value;
    }

    bool IsNearEdge()
    {
        bool value = false;

        float castDist = baseCastDist;

        Vector3 targetPos = DetectionCastPoint.position;
        targetPos.y -= (castDist + 3);

        Debug.DrawLine(DetectionCastPoint.position, targetPos, Color.red);

        if (Physics2D.Linecast(DetectionCastPoint.position, targetPos, 1 << LayerMask.NameToLayer("Ground")))
        {
            value = false;
        }
        else
        {
            value = true;
        }

        return value;
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
            if (!IsNearEdge())
            {
                //enemy is to the left of the player, then move right
                if (canMove)
                    rb.velocity = new Vector2(MathF.Min(Math.Max(rb.velocity.x + (walkAcceleration * walkDirectionVector.x * Time.deltaTime), -maxAgroMoveSpeed), maxAgroMoveSpeed), rb.velocity.y);
                else
                    rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, 0, walkStopRate), rb.velocity.y);

                WalkDirection = WalkableDirection.Right;
            }
            else
            {
                rb.velocity = new Vector2(0, 0);
            }
        }
        else if (transform.position.x > player.position.x)
        {
            if (!IsNearEdge())
            {
                //enemy is to the right of the player, then move left
                if (canMove)
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
        } else
        {
            WalkDirection = WalkableDirection.Right;
        }
    }

    public void OnHit(int damage, Vector2 knockback)
    {
        rb.velocity = new Vector2(knockback.x, rb.velocity.y + knockback.y);
    }

    IEnumerator StopChasingPlayer()
    {
        isSearching = true;
        yield return new WaitForSeconds(5);
        isAgro = false;
        isSearching = false;
    }
}
