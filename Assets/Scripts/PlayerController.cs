using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody rb;

    [Header("Player Variables")]
    public float speed;                     // player movement speed
    public float turnSmooth = 0.1f;
    public float height;
    public float rotationSpeed;             // the speed at which players body rotates

    public float drag;                      // if unset the player will slide uncontrollably

    Sliding slide;

    float turnSmoothVelocity;

    [Header("Other stuff")]
    public Animator anim;
    public Transform cam;                   // set the player camera and not Cinemachine camera
    public Transform orientation;           // orientation object

    bool isGrounded = true;
    
    float horizontal;
    float vertical;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;           // if unset the player will either fall or violently rotate

        slide = GetComponent<Sliding>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (Physics.Raycast(transform.position, Vector3.down, height)) {
            isGrounded = true;
        } else { isGrounded = false; }

        if(isGrounded) {
            rb.linearDamping = drag;        // apply player drag
        } else {
            rb.linearDamping = 0;           // if player is in air, disable drag
        }

        speedControl();
    }

    private void FixedUpdate() {
        movePlayer();
    }

    void movePlayer() {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

        if(direction.magnitude >= 0.1f) {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmooth);

            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            rb.AddForce(moveDir * speed * 2.5f, ForceMode.Force);

            anim.SetBool("isWalking", true);
        }
        else {
            anim.SetBool("isWalking", false);
        }
    }

    void speedControl() {
        Vector3 vel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        // If player velocity exceeds the target speed we need to change the velocity to the target speed.
        if(vel.magnitude > speed) {
            Vector3 targetVel = vel.normalized * speed;
            rb.linearVelocity = new Vector3(targetVel.x, rb.linearVelocity.y, targetVel.z);
        }
    }
}
