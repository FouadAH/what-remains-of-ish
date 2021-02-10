using UnityEngine;

/// <summary>
/// Controller class responsile for handling ray cast logic. Used to determine collitions. 
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class RayCast_Controller : MonoBehaviour {

    new Collider2D collider;
    public RaycastOrigins raycastorigins;
    
    public LayerMask collitionMask;


    public const float skinWidth = .015f;
    const float distBetweenRays = .25f;

    [HideInInspector]
    public int horizontalRayCount, 
        verticalRayCount;

    [HideInInspector]
    public float horizontalRaySpacing, 
        verticalRaySapacing;

    public virtual void Awake()
    {
        collider = GetComponent<Collider2D>();
    }

    public virtual void Start()
    {
        CalculateRaySpacing();
    }

    public struct RaycastOrigins
    {
        public Vector2 topLeft, topRight,
            bottomLeft, bottomRight;
    }

    public void CalculateRaySpacing()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);

        float boundsWidth = bounds.size.x;
        float boundsHeight = bounds.size.y;

        horizontalRayCount = Mathf.RoundToInt(boundsHeight/ distBetweenRays );
        verticalRayCount = Mathf.RoundToInt(boundsWidth / distBetweenRays);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySapacing = bounds.size.x / (verticalRayCount - 1);

    }

    public void UpdateRaycastOrigins()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);

        raycastorigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastorigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastorigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastorigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }
}
