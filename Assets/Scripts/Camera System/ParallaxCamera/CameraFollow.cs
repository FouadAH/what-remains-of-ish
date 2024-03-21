using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
    /*
    public Controller_2D target;
    public BoxCollider2D boundBox;

    public float verticalOffset;

    float lookAheadX;
    public float lookAheadDist;
    public float lookSmoothTimeX;
    public float lookSmoothTimeY;
    
    public Vector2 focusAreaSize;
    FocusArea focusArea;
    private Vector3 minBounds;
    private Vector3 maxBounds;
    private Camera theCamera;
    private float halfHeight;
    private float halfWidth;
    float currentLookAheadX;
    float targetLookAheadX;
    float lookAheadDirX;
    float smoothLookVelocitX;
    float smoothLookVelocityY;

    bool lookAheadStopped;

    #region Singleton

    public static CameraFollow instance;

    private GameManager gm;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
        gm = FindObjectOfType<GameManager>();
    }
    #endregion

    void Start()
    {
        target = gm.player.GetComponent<Controller_2D>();
        focusArea = new FocusArea(target.GetComponent<Collider2D>().bounds, focusAreaSize);
        minBounds = boundBox.bounds.min;
        maxBounds = boundBox.bounds.max;
        theCamera = GetComponent<Camera>();
        halfHeight = theCamera.orthographicSize;
        halfWidth = halfHeight * Screen.width /Screen.height;
    }

    void LateUpdate()
    {
        focusArea.Update(target.GetComponent<Collider2D>().bounds);
        Vector2 focusPosition = focusArea.center + Vector2.up * verticalOffset;

        if(focusArea.velocity.x != 0)
        {
            lookAheadDirX = Mathf.Sign(focusArea.velocity.x);
            if(target.playerInput.x != 0)
            {
                lookAheadStopped = false;
                targetLookAheadX = lookAheadDirX * lookAheadDist;

            }
            else
            {
                if (!lookAheadStopped)
                {
                    lookAheadStopped = true;
                    targetLookAheadX = currentLookAheadX + (lookAheadDirX * lookAheadDist - currentLookAheadX)/4f;
                }
            }
        }
        
        currentLookAheadX = Mathf.SmoothDamp(currentLookAheadX, targetLookAheadX, ref smoothLookVelocitX, lookSmoothTimeX);

        focusPosition.y = Mathf.SmoothDamp(transform.position.y, focusPosition.y, ref smoothLookVelocityY, lookSmoothTimeY);
        focusPosition += Vector2.right * currentLookAheadX;
        transform.position = (Vector3)focusPosition + Vector3.forward *-10;

        transform.position = new Vector3(
             Mathf.Clamp(transform.position.x, minBounds.x + halfWidth, maxBounds.x - halfWidth),
             Mathf.Clamp(transform.position.y, minBounds.y + halfHeight, maxBounds.y - halfHeight),
             transform.position.z
            );

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, .5f);
        Gizmos.DrawCube(focusArea.center, focusAreaSize);
    }
    struct FocusArea {
        public Vector2 center;
        float left, right, top, bottom;
        public Vector2 velocity;

        public FocusArea(Bounds targetBounds, Vector2 size)
        {
            left = targetBounds.center.x - size.x / 2;
            right = targetBounds.center.x + size.x / 2;
            bottom = targetBounds.min.y;
            top = targetBounds.min.y + size.y;

            velocity = Vector2.zero;

            center = new Vector2((left + right) / 2, (top + bottom) / 2);
        }
        
        public void Update(Bounds targetBounds)
        {
            float shiftX = 0;
            if (targetBounds.min.x < left)
            {
                shiftX = targetBounds.min.x - left;
            }
            else if (targetBounds.max.x > right)
            {
                shiftX = targetBounds.max.x - right;
            }
            left += shiftX;
            right += shiftX;

            float shiftY = 0;
            if (targetBounds.min.y < bottom)
            {
                shiftY = targetBounds.min.y - bottom;
            }
            else if (targetBounds.max.y > top)
            {
                shiftY = targetBounds.max.y - top;
            }

            bottom += shiftY;
            top += shiftY;

            center = new Vector2((left + right) / 2, (top + bottom) / 2);
            velocity = new Vector2(shiftX, shiftY);
        }

    }
    */
}
