using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ButtonStates {
    Unpressed,
    Pressed
}

public class Button : DoorActivator
{
    MeshRenderer mesh;
    ButtonStates state = ButtonStates.Unpressed;
    [SerializeField]
    Material pressedMaterial;
    [SerializeField]
    Material unpressedMaterial;
    void Start()
    {
        mesh = GetComponent<MeshRenderer>();
        mesh.material = unpressedMaterial;
    }

    int collisionsCount = 0;

    void OnCollisionEnter(Collision collision) {
        collisionsCount++;
        if (state == ButtonStates.Unpressed) {
            state = ButtonStates.Pressed;
            mesh.material = pressedMaterial;
            onActivate.Invoke();
        }
    }
    void OnCollisionExit(Collision collision) {
        collisionsCount--;
        if (collisionsCount <= 0 && state == ButtonStates.Pressed) {
            state = ButtonStates.Unpressed;
            mesh.material = unpressedMaterial;
            onDeactivate.Invoke();
        }
    }
}
