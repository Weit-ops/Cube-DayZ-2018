﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Require a character controller to be attached to the same game object
[RequireComponent(typeof(CharacterMotor))]
[AddComponentMenu("Character/FPS Input Controller")]
public class FPSInputController : MonoBehaviour
{
    private CharacterMotor motor;

    // Use this for initialization
    void Awake()
    {
        motor = GetComponent<CharacterMotor>();
    }

    // Update is called once per frame
    void Update()
    {
        // Get the input vector from kayboard or analog stick
        //Debug.Log(string.Format("{0} {1}", Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));
        Vector3 directionVector = new Vector3(ControlFreak2.CF2Input.GetAxis("Horizontal"), 0, ControlFreak2.CF2Input.GetAxis("Vertical"));

        //Debug.Log(string.Format("{0} {1}", MobileInput.HorizontalAxis, MobileInput.VerticalAxis));
        //Vector3 directionVector = new Vector3(MobileInput.HorizontalAxis, 0, MobileInput.VerticalAxis);

        if (directionVector != Vector3.zero)
        {
            // Get the length of the directon vector and then normalize it
            // Dividing by the length is cheaper than normalizing when we already have the length anyway
            float directionLength = directionVector.magnitude;
            directionVector = directionVector / directionLength;

            // Make sure the length is no bigger than 1
            directionLength = Mathf.Min(1.0f, directionLength);

            // Make the input vector more sensitive towards the extremes and less sensitive in the middle
            // This makes it easier to control slow speeds when using analog sticks
            directionLength = directionLength * directionLength;

            // Multiply the normalized direction vector by the modified length
            directionVector = directionVector * directionLength;
        }

        // Apply the direction to the CharacterMotor
        motor.inputMoveDirection = transform.rotation * directionVector;
        motor.inputJump = ControlFreak2.CF2Input.GetButton("Jump");
    }
}