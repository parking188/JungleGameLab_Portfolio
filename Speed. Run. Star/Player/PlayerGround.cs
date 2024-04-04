using UnityEngine;

public class PlayerGround : MonoBehaviour {
    private bool onGround = false;
    private bool onLandingDashGround = false;

    [Header("Collider Settings")]
    [SerializeField][Tooltip("Length of the ground-checking collider")] private float groundLength = 17f;
    [SerializeField][Tooltip("Distance between the ground-checking colliders")] private Vector3 colliderOffset;

    [Header("Lading Dash Settings")]
    [SerializeField][Tooltip("Length of the ground-checking for landing dash")] private float landingDashLength = 25f;
    [
        Header("Layer Masks")]
    [SerializeField][Tooltip("Which layers are read as the ground")] private LayerMask groundLayer;


    private void Update() {
        //Determine if the player is stood on objects on the ground layer, using a pair of raycasts
        onGround = Physics2D.Raycast(transform.position + colliderOffset, Vector2.down, groundLength, groundLayer) || Physics2D.Raycast(transform.position - colliderOffset, Vector2.down, groundLength, groundLayer);
        onLandingDashGround = Physics2D.Raycast(transform.position + colliderOffset, Vector2.down, landingDashLength, groundLayer) || Physics2D.Raycast(transform.position - colliderOffset, Vector2.down, landingDashLength, groundLayer);
    }

    private void OnDrawGizmos() {
        //Draw the ground colliders on screen for debug purposes
        if (onGround) { Gizmos.color = Color.green; } else { Gizmos.color = Color.red; }
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundLength);
        Gizmos.DrawLine(transform.position + colliderOffset, transform.position + colliderOffset + Vector3.down * groundLength);
        Gizmos.DrawLine(transform.position - colliderOffset, transform.position - colliderOffset + Vector3.down * groundLength);
        if (onLandingDashGround) { Gizmos.color = Color.green; } else { Gizmos.color = Color.red; }
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * landingDashLength);
    }

    //Send ground detection to other scripts
    public bool GetOnGround() { return onGround; }
    public bool GetOnLandingDashGround() { return onLandingDashGround; }
}