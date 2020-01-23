﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]

public class CameraController : MonoBehaviour
{
    BaseControls controls;

    public float screenBorder = 15;

    public float panSpeed = 100;
    public float defaultOrbitSpeed = 50;
    public float mouseOrbitSpeed = 10;
    Vector2 moveInput = Vector2.zero;
    Vector2 mousePos = Vector2.zero;
    Vector2 mouseDelta = Vector2.zero;

    private Vector3 pan;

    private bool orbitMode = false;
    private Vector3 orbitTarget;
    [Range(0,90)]
    public float orbitYMax = 90f;
    [Range(0, 90)]
    public float orbitYMin = 5f;
    Vector3 orbitVector;
    float cameraXRotation;
    float cameraYRotation;
    float orbitDistance;


    void Awake()
    {
        controls = new BaseControls();
        controls.Navigation.OrbitShift.performed += ctx => ToggleOrbit();
        controls.Navigation.Movement.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Navigation.PointerLocation.performed += ctx => mousePos = ctx.ReadValue<Vector2>();
        controls.Navigation.PointerDelta.performed += ctx => mouseDelta = ctx.ReadValue<Vector2>();
    }

    // Update is called once per frame
    void Update()
    {
        DefaultHandler();
        MouseHandler();
    }

    // DefaultHandler processes keyboard and gamepad input.
    void DefaultHandler()
    {
        if (!orbitMode)
        {
            // Outside of orbit mode, translate movement input directly into panning.
float panX = moveInput.x * panSpeed * Time.deltaTime;
            float panZ = moveInput.y * panSpeed * Time.deltaTime;
            pan = new Vector3(panX, 0, panZ);

            transform.Translate(pan, Space.World);
        }
        else
        {
            // In orbit mode, movement input translates directly into orbiting.
            OrbitMode(moveInput, defaultOrbitSpeed);
        }
    }

    // MouseHandler processes mouse pointer input.
    void MouseHandler()
    {
        if (!orbitMode)
        {
            // Outside of orbit mode, translate movement input into panning only when the cursor position is near the window borders.
            Vector3 pan = Vector3.zero;
            if (mousePos.y >= Screen.height - screenBorder)
            {
                pan += Vector3.forward * panSpeed * Time.deltaTime;
            }
            else if (mousePos.y <= screenBorder)
            {
                pan -= Vector3.forward * panSpeed * Time.deltaTime;
            }
            
            if (mousePos.x <= screenBorder)
            {
                pan += Vector3.left * panSpeed * Time.deltaTime;
            }
            else if (mousePos.x >= Screen.width - screenBorder)
            {
                pan += Vector3.right * panSpeed * Time.deltaTime;
            }

            transform.Translate(pan, Space.World);
        }
        else
        {
            // In orbit mode, mouse movement translates directly into orbiting.
            OrbitMode(mouseDelta, mouseOrbitSpeed);
        }
    }

    // When the orbit mode button is pressed or released, enter or leave orbit mode.
    void ToggleOrbit()
    {
        if (orbitMode)
        {
            orbitMode = false;
        }
        else
        {
            // On entering orbit mode, find a target directly in front of the camera as the point to orbit around.
            orbitMode = true;
            RaycastHit hit;
            Physics.Raycast(transform.position, transform.forward, out hit);
            orbitTarget = hit.point;

            // Get a vector to that point and a distance.
            orbitVector = transform.position - orbitTarget;
            orbitDistance = Vector3.Magnitude(orbitVector);

            // Get the current Euler angles of the camera.
            cameraXRotation = transform.eulerAngles.x;
            cameraYRotation = transform.eulerAngles.y;
        }
    }

    // When in orbiting mode, the input (either keyboard/gamepad or mouse delta) moves the camera around the current target.
    void OrbitMode(Vector2 input, float orbitSpeed)
    {
        float orbitX = input.x * orbitSpeed * Time.deltaTime;
        float orbitY = input.y * orbitSpeed * Time.deltaTime;

        // Note the inversion: the X rotation is altered by Y input and vice versa.
        cameraXRotation += orbitY;
        cameraYRotation += orbitX;

        // The maximum X rotation is clamped. An orbitYMax of 90 will allow a top-down view.
        cameraXRotation = Mathf.Clamp(cameraXRotation, orbitYMin, orbitYMax);

        // The camera rotation is adjusted to the new values.
        Quaternion rotation = Quaternion.Euler(cameraXRotation, cameraYRotation, 0);

        // The camera is positioned orbitDistance units away from the target at the new rotation angle.
        Vector3 negativeDistance = new Vector3(0, 0, -orbitDistance);
        Vector3 position = rotation * negativeDistance + orbitTarget;

        // The position and rotation are applied.
        transform.rotation = rotation;
        transform.position = position;
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
}
