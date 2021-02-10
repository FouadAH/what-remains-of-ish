using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Controller class that handles collition logic, extends the raycast controller
/// </summary>
public class Controller_2D : RayCast_Controller
{
    public Collitioninfo collitions;

    public float maxSlopeAngle = 50f;
    
    [HideInInspector]
    public Vector2 playerInput;

    public override void Start()
    {
        base.Start();
        collitions.faceDirection = 1;
    }

    /// <summary>
    /// Method that moves the object by the given amount
    /// </summary>
    /// <param name="moveAmount">Amount to move</param>
    /// <param name="standingOnPlatform">Is the object standing a moving platform</param>
    public void Move(Vector2 moveAmount, bool standingOnPlatform = false)
    {
        Move(moveAmount, Vector2.zero, standingOnPlatform);
    }

    /// <summary>
    ///  Method that moves the object by the given amount
    /// </summary>
    /// <param name="moveAmount"></param>
    /// <param name="input"></param>
    /// <param name="standingOnPlatform"></param>
    public void Move(Vector2 moveAmount, Vector2 input, bool standingOnPlatform = false)
    {
        playerInput = input;
        collitions.Reset();
        UpdateRaycastOrigins();
        collitions.moveAmountOld = moveAmount;

        if (moveAmount.y < 0)
        {
            DescentSlope(ref moveAmount);
        }
        if (moveAmount.x != 0)
        {
            collitions.faceDirection = (int)Mathf.Sign(moveAmount.x);
        }
        HorizontalCollitions(ref moveAmount);

        if (moveAmount.y != 0)
        {
            VerticalCollitions(ref moveAmount);
        }
        if (standingOnPlatform == true)
        {
            collitions.below = true;
        }
        transform.Translate(moveAmount);
    }

    /// <summary>
    /// Method that handles horizontal collition logic 
    /// </summary>
    /// <param name="moveAmount">Reference to the amount the object is moved</param>
    void HorizontalCollitions(ref Vector2 moveAmount)
    {
        float directionX = collitions.faceDirection;
        float rayLength = Mathf.Abs(moveAmount.x) + skinWidth;

        if (Mathf.Abs(moveAmount.x) < skinWidth)
        {
            rayLength = 2 * skinWidth;
        }
        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? raycastorigins.bottomLeft : raycastorigins.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collitionMask);

            Debug.DrawRay(rayOrigin, Vector2.right * directionX, Color.red);

            if (hit)
            {
                if (hit.distance == 0)
                {
                    continue;
                }

                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                if (i == 0 && slopeAngle <= maxSlopeAngle)
                {

                    if (collitions.desendingSlope)
                    {
                        collitions.desendingSlope = false;
                        moveAmount = collitions.moveAmountOld;
                    }

                    float distanceToSlopeStart = 0;

                    if (slopeAngle != collitions.slopeAngleOld)
                    {
                        distanceToSlopeStart = hit.distance - skinWidth;
                        moveAmount.x -= distanceToSlopeStart * directionX;
                    }

                    ClimbSlope(ref moveAmount, slopeAngle, hit.normal);
                    moveAmount.x += distanceToSlopeStart * directionX;
                }

                if (!collitions.climbingSlope || slopeAngle > maxSlopeAngle)
                {
                    moveAmount.x = (hit.distance - skinWidth) * directionX;
                    rayLength = hit.distance;

                    if (collitions.climbingSlope)
                    {
                        moveAmount.y = Mathf.Tan(collitions.slopeAngle * Mathf.Deg2Rad * Mathf.Abs(moveAmount.x));
                    }

                    collitions.left = directionX == -1;
                    collitions.right = directionX == 1;
                }

            }
        }
    }

    /// <summary>
    /// Method that handles vertical collition logic 
    /// </summary>
    /// <param name="moveAmount">Reference to the amount the object is moved</param>
    void VerticalCollitions(ref Vector2 moveAmount)
    {
        float directionY = Mathf.Sign(moveAmount.y);
        float rayLength = Mathf.Abs(moveAmount.y) + skinWidth;

        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = (directionY == -1) ? raycastorigins.bottomLeft : raycastorigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySapacing * i + moveAmount.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collitionMask);


            Debug.DrawRay(rayOrigin, Vector2.up * directionY, Color.red);

            if (hit)
            {
                if (hit.collider.tag == "Through")
                {
                    if (directionY == 1 || hit.distance == 0)
                    {
                        continue;
                    }
                    if (collitions.fallingThroughtPlatform)
                    {
                        continue;
                    }
                    if (playerInput.y == -1)
                    {
                        collitions.fallingThroughtPlatform = true;
                        Invoke("resetFallingThrough", .5f);
                        continue;
                    }

                }
                

                moveAmount.y = (hit.distance - skinWidth) * directionY;
                rayLength = hit.distance;

                if (collitions.climbingSlope)
                {
                    moveAmount.x = moveAmount.y / Mathf.Tan(collitions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(moveAmount.x);
                }

                collitions.below = directionY == -1;
                collitions.above = directionY == 1;
            }
        }

        if (collitions.climbingSlope)
        {
            float directionX = Mathf.Sign(moveAmount.x);
            rayLength = Mathf.Abs(moveAmount.x + skinWidth);
            Vector2 rayOrigin = (directionX == 1) ? raycastorigins.bottomLeft : raycastorigins.bottomRight + Vector2.up * moveAmount.y;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collitionMask);

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != collitions.slopeAngle)
                {
                    moveAmount.x = (hit.distance + skinWidth) * directionX;
                    collitions.slopeAngle = slopeAngle;
                    collitions.slopeNormal = hit.normal;
                }
            }
        }
    }

    void resetFallingThrough()
    {
        collitions.fallingThroughtPlatform = false;
    }

    void ClimbSlope(ref Vector2 moveAmount, float slopeAngle, Vector2 slopeNormal)
    {
        float moveDistance = Mathf.Abs(moveAmount.x);
        float climbmoveAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

        if (moveAmount.y <= climbmoveAmountY)
        {
            moveAmount.y = climbmoveAmountY;
            moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
            collitions.below = true;
            collitions.climbingSlope = true;
            collitions.slopeNormal = slopeNormal;

        }
    }

    void DescentSlope(ref Vector2 moveAmount)
    {
        RaycastHit2D maxSlopeHitLeft = Physics2D.Raycast(raycastorigins.bottomLeft, Vector2.down, Mathf.Abs(moveAmount.y) + skinWidth, collitionMask);
        RaycastHit2D maxSlopeHitRight = Physics2D.Raycast(raycastorigins.bottomRight, Vector2.down, Mathf.Abs(moveAmount.y) + skinWidth, collitionMask);
        if (maxSlopeHitLeft ^ maxSlopeHitRight)
        {
            SlideDownMaxSlope(maxSlopeHitLeft, ref moveAmount);
            SlideDownMaxSlope(maxSlopeHitRight, ref moveAmount);
        }

        if (!collitions.slidingDownMaxSlope)
        {
            float directionX = Mathf.Sign(moveAmount.x);
            Vector2 rayOrigin = (directionX == -1) ? raycastorigins.bottomRight : raycastorigins.bottomLeft;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, collitionMask);

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != 0 && slopeAngle <= maxSlopeAngle)
                {
                    if (Mathf.Sign(hit.normal.x) == directionX)
                    {
                        if (hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x))
                        {
                            float moveDistance = Mathf.Abs(moveAmount.x);
                            float DescentmoveAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                            moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
                            moveAmount.y -= DescentmoveAmountY;

                            collitions.slopeAngle = slopeAngle;
                            collitions.desendingSlope = true;
                            collitions.below = true;
                            collitions.slopeNormal = hit.normal;

                        }
                    }
                }
            }
        }


    }

    void SlideDownMaxSlope(RaycastHit2D hit, ref Vector2 moveAmount)
    {
        if (hit)
        {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            if (slopeAngle > maxSlopeAngle)
            {
                moveAmount.x = Mathf.Sign(hit.normal.x) * (Mathf.Abs(moveAmount.y) - hit.distance) / Mathf.Tan(slopeAngle * Mathf.Deg2Rad);
                collitions.slopeAngle = slopeAngle;
                collitions.slidingDownMaxSlope = true;
                collitions.slopeNormal = hit.normal;

            }
        }
    }

    public struct Collitioninfo
    {
        public bool above, below, left, right, climbingSlope, desendingSlope;
        public bool slidingDownMaxSlope;
        public float slopeAngle, slopeAngleOld;

        public Vector2 moveAmountOld;
        public Vector2 slopeNormal;
        public bool fallingThroughtPlatform;

        public int faceDirection;

        public void Reset()
        {
            above = below = false;
            left = right = false;
            climbingSlope = false;
            slidingDownMaxSlope = false;
            slopeNormal = Vector2.zero;

            slopeAngleOld = slopeAngle;
            slopeAngle = 0;
        }
    }

}
