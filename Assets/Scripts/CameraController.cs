using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]

public class CameraController : MonoBehaviour
{
    BaseControls controls;

    public GameObject uiPanel;
    public float screenBorder = 15;

    public float panSpeed = 100;
    public float orbitSpeed = 30;
    Vector2 moveInput = Vector2.zero;
    Vector2 mousePos = Vector2.zero;
    Vector2 mouseDelta = Vector2.zero;

    private Vector3 pan;
    private bool orbitMode = false;
    private Vector3 orbitTarget;


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
        if (!orbitMode)
        {
            float panX = moveInput.x * panSpeed * Time.deltaTime;
            float panZ = moveInput.y * panSpeed * Time.deltaTime;
            pan = new Vector3(panX, 0, panZ);

            transform.Translate(pan, Space.World);
        }
        else
        {
            transform.RotateAround(orbitTarget, Vector3.up, moveInput.x * orbitSpeed * Time.deltaTime);
            transform.RotateAround(orbitTarget, Vector3.right, moveInput.y * orbitSpeed * Time.deltaTime);
        }
        MouseHandler();
    }

    void ToggleOrbit()
    {
        if (orbitMode)
        {
            orbitMode = false;
        }
        else
        {
            orbitMode = true;
            RaycastHit hit;
            Physics.Raycast(transform.position, transform.forward, out hit);
            orbitTarget = hit.point;
        }
    }

    void MouseHandler()
    {
        if (!orbitMode)
        {
            Vector3 pan = Vector3.zero;
            if (mousePos.y >= Screen.height - screenBorder)
            {
                pan += Vector3.forward * panSpeed * Time.deltaTime;
            }
            else if (mousePos.y <= screenBorder)
            {
                pan -= Vector3.forward * panSpeed * Time.deltaTime;
            }
            else if (mousePos.x <= screenBorder)
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
            transform.RotateAround(orbitTarget, Vector3.up, mouseDelta.x * orbitSpeed * Time.deltaTime);
            transform.RotateAround(orbitTarget, Vector3.right, mouseDelta.y * orbitSpeed * Time.deltaTime);
        }
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
