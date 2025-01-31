using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody rb;

    [Header("Movement")]
    private float speed;                     // player current movement speed
    public float slideSpeed;
    public float walkSpeed;
    public float turnSmooth = 0.1f;
    public float height;
    public float rotationSpeed;             // the speed at which players body rotates

    public float drag;                      // if unset the player will slide uncontrollably

    public float speedIncreaseMultiplier;   // overtime speed increase multiplier
    public float slopeIncreaseMultiplier;   // steeper drop gives more speed

    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;

    Sliding slide;

    float turnSmoothVelocity;

    [Header("Jumping")]
    public float jumpForce = 5.0f;
    private bool readyToJump = true;
    public float jumpCooldown;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    [Header("Other stuff")]
    public Animator anim;
    public Transform cam;                   // set the player camera and not Cinemachine camera
    public Transform orientation;           // orientation object
    public Transform rotationTarget;

    bool isGrounded = true;
    
    float horizontal;
    float vertical;

    public MovementState state;
    public enum MovementState {
        walking,
        sliding,
        air
    }

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

        if(speed < 2f) {
            state = MovementState.walking;
            slide.sliding = false;
        }

        playerInput();
        speedControl();
        stateHandler();

        lastDesiredMoveSpeed = desiredMoveSpeed;
        rb.useGravity = !slide.sliding;     // if sliding disable gravity
    }

    private void FixedUpdate() {
        movePlayer();
    }

    private void playerInput() {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        if(Input.GetKeyDown(KeyCode.Space) && state == MovementState.sliding && isGrounded && readyToJump) {
            readyToJump = false;
            jump();

            Invoke(nameof(resetJump), jumpCooldown);
        }

    }

    private void stateHandler() {
        if(slide.sliding) {                 // mode = sliding
            state = MovementState.sliding;
            
            if(OnSlope() && rb.linearVelocity.y < 0.2f) {
                desiredMoveSpeed = slideSpeed;
            } else { desiredMoveSpeed = walkSpeed; }
        }

        else if(isGrounded) {               // assume that mode = walking unless sliding
            state = MovementState.walking;
            desiredMoveSpeed = walkSpeed;
        }

        if(Mathf.Abs(desiredMoveSpeed - lastDesiredMoveSpeed) > 4f) {
            StopAllCoroutines();
            StartCoroutine(SmoothlyLerpMoveSpeed());
        } else { speed = desiredMoveSpeed; }
    }

    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        // smoothly lerp movementSpeed to desired value
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - speed);
        float startValue = speed;

        while (time < difference)
        {
            speed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);

            if (OnSlope())
            {
                float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
                float slopeAngleIncrease = 1 + (slopeAngle / 90f);

                time += Time.deltaTime * speedIncreaseMultiplier * slopeIncreaseMultiplier * slopeAngleIncrease;
            }
            else
                time += Time.deltaTime * speedIncreaseMultiplier;

            yield return null;
        }

        speed = desiredMoveSpeed;
    }

    void movePlayer() {
        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

        if(direction.magnitude >= 0.1f) {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmooth);

            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            rb.AddForce(moveDir * speed * 2.5f, ForceMode.Force);

            anim.SetBool("isWalking", true);

            float singleStep = speed * Time.deltaTime;

            // Rotate the forward vector towards the target direction by one step
            Vector3 newDirection = Vector3.RotateTowards(orientation.forward, GetSlopeMoveDirection(moveDir), singleStep, 2.0f);
            transform.rotation = Quaternion.LookRotation(newDirection);
        }
        else {
            anim.SetBool("isWalking", false);
        }
    }

    void speedControl() {
        // limiting speed on slope
        if (OnSlope() && !exitingSlope)
        {
            if (rb.linearVelocity.magnitude > speed)
                rb.linearVelocity = rb.linearVelocity.normalized * speed;
        }
        // limiting speed on ground or in air
        else
        {
            Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

            // limit velocity if needed
            if (flatVel.magnitude > speed)
            {
                Vector3 limitedVel = flatVel.normalized * speed;
                rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
            }
        }
    }

    private void jump() {
        exitingSlope = true;

        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void resetJump() {
        exitingSlope = false;
        readyToJump = true;
    }

    public bool OnSlope() {
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, height * 0.5f + 0.3f)) {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }
        return false;
    }

    public Vector3 GetSlopeMoveDirection(Vector3 direction) {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }
}