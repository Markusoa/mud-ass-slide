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

        rb.AddForce(Vector3.down * 1.25f, ForceMode.Impulse);
        rb.AddForce(pc.GetSlopeMoveDirection(pc.orientation.forward) * slideForce, ForceMode.Force);
    }

    private void OnTriggerEnter(Collider collider) {
        sliding = false;
    }
}