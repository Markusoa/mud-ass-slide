using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Sliding : MonoBehaviour
{
    InputAction slide;

    Rigidbody rb;

    bool sliding = false;

    void Start() {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        slide = new InputAction(
            type: InputActionType.Button,
            binding: "<Keyboard>/space",
            interactions: "press(behavior=1)"
        );
        slide = new InputAction(
            type: InputActionType.Button,
            binding: "<Gamepad>/buttonSouth"
        );

        slide.Enable();
    }

    void Update()
    {
        
    }

    public void startSlide() {
        sliding = true;
    }

}
