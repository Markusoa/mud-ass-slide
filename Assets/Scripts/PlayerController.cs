using UnityEngine;

public class PlayerController : MonoBehaviour
{
    CharacterController controller;
    public Animator anim;

    public float speed;
    public float turnSmooth = 0.1f;
    float turnSmoothVelocity;

    public Transform cam;
    
    void Start()
    {
        controller = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (!controller.isGrounded) {
            controller.Move(Vector3.down * 9.81f * Time.deltaTime);
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

        if(direction.magnitude >= 0.1f) {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmooth);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir * speed * Time.deltaTime);
            anim.SetBool("isWalking", true);
        }
        else {
            anim.SetBool("isWalking", false);
        }
    }
}
