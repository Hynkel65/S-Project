using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundDetection : MonoBehaviour
{
    public ContactFilter2D castFilter;
    public float groundDistance = 0.1f;
    public float wallDistance = 0.01f;
    CapsuleCollider2D touchingCol;
    Animator animator;

    //Vector2 colliderSize;
    //[SerializeField] float slopeCheckDist;
    //public LayerMask layerMask;
    //float slopeDownAngle;
    //public Vector2 slopeNormalPerp;
    //public bool isOnSlope;
    //float slopeDownAngleOld;
    //float slopeSideAngle;


    RaycastHit2D[] groundHits = new RaycastHit2D[5];
    //RaycastHit2D[] wallHits = new RaycastHit2D[5];

    [SerializeField]
    private bool _isGrounded;

    public bool isGrounded 
    { 
        get 
        {
            return _isGrounded;
        } 
        private set
        {
            _isGrounded = value;
            animator.SetBool("isGrounded", _isGrounded);
        } 
    }

    //[SerializeField]
    //private bool _isOnWall;
    //private Vector2 wallCheckDirection => gameObject.transform.localScale.x > 0 ? Vector2.right : Vector2.left;

    //public bool isOnWall
    //{
    //    get
    //    {
    //        return _isOnWall;
    //    }
    //    private set
    //    {
    //        _isOnWall = value;
    //        animator.SetBool("isOnWall", _isOnWall);
    //    }
    //}


    void Awake()
    {
        touchingCol = GetComponent<CapsuleCollider2D>();
        animator = GetComponent<Animator>();
    }

    //private void Start()
    //{
    //    colliderSize = touchingCol.size;
    //}

    void FixedUpdate()
    {
        isGrounded = touchingCol.Cast(Vector2.down, castFilter, groundHits, groundDistance) > 0;
        //SlopeCheck();
        //isOnWall = touchingCol.Cast(wallCheckDirection, castFilter, wallHits, wallDistance) > 0;
    }

    //private void SlopeCheck()
    //{
    //    Vector2 checkPos = transform.position - new Vector3(0.0f, colliderSize.y / 2);

    //    SlopeCheckHorizontal(checkPos);
    //    SlopeCheckVertical(checkPos);
    //}

    //private void SlopeCheckHorizontal(Vector2 checkPos)
    //{
    //    RaycastHit2D slopeHitFront = Physics2D.Raycast(checkPos, transform.right, slopeCheckDist, layerMask);
    //    RaycastHit2D slopeHitBack = Physics2D.Raycast(checkPos, -transform.right, slopeCheckDist, layerMask);

    //    if (slopeHitFront)
    //    {
    //        isOnSlope = true;
    //        slopeSideAngle = Vector2.Angle(slopeHitFront.normal, Vector2.up);
    //    }
    //    else if (slopeHitBack)
    //    {
    //        isOnSlope = true;
    //        slopeSideAngle = Vector2.Angle(slopeHitBack.normal, Vector2.up);
    //    }
    //    else
    //    {
    //        isOnSlope = false;
    //        slopeSideAngle = 0.0f;
    //    }
    //}

    //private void SlopeCheckVertical(Vector2 checkPos)
    //{
    //    RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, slopeCheckDist, layerMask);

    //    if (hit)
    //    {
    //        slopeNormalPerp = Vector2.Perpendicular(hit.normal);

    //        slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);

    //        if (slopeDownAngle != slopeDownAngleOld)
    //        {
    //            isOnSlope = true;
    //        }

    //        slopeDownAngleOld = slopeDownAngle;

    //        Debug.DrawRay(hit.point, slopeNormalPerp, Color.red);
    //        Debug.DrawRay(hit.point, hit.normal, Color.green);
    //    }
    //}
}
