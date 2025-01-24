using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Sliding : MonoBehaviour
{
    [Header("Player stuff")]
    Rigidbody rb;
    public Transform orientation;

    [Header("Sliding")]
    public float slideForce;
    public bool sliding = false;

    void Start() {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    void FixedUpdate() {
        if(sliding) { slide(); }
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)) {
            sliding = true;
        }
    }

    public void slide() {
        sliding = true;

        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        slidingMovement();
    }

    void slidingMovement() {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 inputDirection = orientation.forward * vertical + orientation.right * horizontal;

        rb.AddForce(orientation.forward * slideForce, ForceMode.Force);
    }

}
