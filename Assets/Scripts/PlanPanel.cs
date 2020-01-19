using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanPanel : MonoBehaviour
{
    BaseControls controls;
    LayerMask ground = 1 << 8;
    LayerMask illegal = 1 << 10 | 1 << 11;

    bool showBox = false;
    bool boxLegal;

    public Material legalPlan;
    public Material illegalPlan;

    public GameObject road;
    public GameObject powerPlant;
    public GameObject residentialZone;

    Vector2 mousePos;

    GameObject currentObject;
    
    private void Awake()
    {
        controls = new BaseControls();
        controls.Planning.Position.performed += ctx => mousePos = ctx.ReadValue<Vector2>();
        controls.Planning.Place.performed += ctx => PlaceCurrent();
    }

    private void Update()
    {
        if (showBox)
        {
            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            RaycastHit groundHit;
            bool hitDetect = Physics.BoxCast(
                ray.origin, 
                currentObject.transform.localScale, 
                ray.direction, 
                out groundHit
            );
            if (hitDetect)
            {
                // Check for legal placement
                currentObject.transform.position = new Vector3(
                    groundHit.point.x, currentObject.transform.position.y, groundHit.point.z
                );
            }
        }
    }

    public void PlanNetwork()
    {
        
    }

    public void PlanPloppable()
    {
        showBox = true;
        currentObject = Instantiate(powerPlant);
    }

    public void PlanGrowable()
    {

    }

    void PlaceCurrent()
    {
        showBox = false;
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
