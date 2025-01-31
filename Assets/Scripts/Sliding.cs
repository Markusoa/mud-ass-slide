using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Sliding : MonoBehaviour
{
    Rigidbody rb;
    private PlayerController pc;

    [Header("Sliding")]
    public float slideForce;
    public bool sliding = false;

    float horizontal;
    float vertical;

    float currentSlope;
    float lastSlope;

    void Start() {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        pc = GetComponent<PlayerController>();
    }

    void FixedUpdate() {
        if(sliding) { 
            slidingMovement();
        }
        pc.anim.SetBool("isWalking", !sliding);
        pc.anim.SetBool("isSliding", sliding);
    }

    void Update() {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        if(Input.GetKeyDown(KeyCode.Space)) {
            sliding = true;
        }
    }

    void slidingMovement() {
        // Vector3 inputDirection = pc.orientation.forward * vertical + pc.orientation.right * horizontal;

        sliding = true;

        if(pc.OnSlope()) {
            if(pc.isGrounded) {
                currentSlope = Vector3.Angle(Vector3.up, pc.slopeHit.normal);
                lastSlope = currentSlope;
            }

            if(lastSlope > 55.0f) {
                rb.AddForce(Vector3.down * lastSlope/8, ForceMode.Impulse);     // When the player comes across a very large slope we need to apply more gravity
                                                                                // for the player to stay on the ground and not start flying/jumping in air.
            } else if(lastSlope < 55.0f) {
                rb.AddForce(Vector3.down * 1.5f, ForceMode.Impulse);            // otherwise apply normal gravity
            }
        } else {
            rb.AddForce(Vector3.down * 1.5f, ForceMode.Impulse);                // should not be needed as the player is always on a slope while sliding, but
                                                                                // just in case this isn't the situation we still want to apply gravity to the player.
        }
        // rb.AddForce(pc.GetSlopeMoveDirection(pc.orientation.forward) * slideForce, ForceMode.Force);
    }

    private void OnTriggerEnter(Collider collider) {
        sliding = false;
    }
}