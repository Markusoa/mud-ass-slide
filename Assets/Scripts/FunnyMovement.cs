using UnityEngine;

public class FunnyMovement : MonoBehaviour
{
    Rigidbody rb;

    [Header("Player Variables")]
    public float speed;
    public float turnSmooth = 0.1f;
    public float height;
    public float rotationSpeed;

    float turnSmoothVelocity;

    [Header("Other stuff")]
    public Transform player;
    public Transform playerObj;
    public Animator anim;
    public Transform cam;
    public Transform orientation;

    bool isGrounded = true;
    
    float horizontal;
    float vertical;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (Physics.Raycast(transform.position, Vector3.down, height)) {
            isGrounded = true;
        }
        else {
            isGrounded = false;
        }
    }

    private void FixedUpdate() {
        movePlayer();
    }

    void movePlayer() {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

        if(direction.magnitude >= 0.1f) {
            Vector3 moveDir = orientation.forward * vertical + orientation.right * horizontal;

            rb.AddForce(moveDir * speed * 10f, ForceMode.Force);

            anim.SetBool("isWalking", true);
        }
        else {
            anim.SetBool("isWalking", false);
        }
    }

    void updateCamera() {
        // rotate orientation object
        Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        orientation.forward = viewDir.normalized;

        // rotate player object
        Vector3 inputDir = orientation.forward * vertical + orientation.right * horizontal;

        if(inputDir != Vector3.zero) {
            playerObj.forward = Vector3.Slerp(playerObj.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
        }
    }
}
