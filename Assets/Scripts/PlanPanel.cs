using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanPanel : MonoBehaviour
{
    BaseControls controls;
    public LayerMask groundMask = 1 << 8;
    public LayerMask illegalMask = 1 << 10 | 1 << 11;

    bool showBox = false;
    public bool boxLegal;

    public Material legalPlan;
    public Material illegalPlan;

    public GameObject road;
    public GameObject powerPlant;
    public GameObject residentialZone;

    public Terrain terrain;
    ConstructionSite construction;

    Vector2 mousePos;

    GameObject currentObject;
    
    private void Awake()
    {
        controls = new BaseControls();
        controls.Planning.Position.performed += ctx => mousePos = ctx.ReadValue<Vector2>();
        controls.Planning.Place.performed += ctx => PlaceCurrent();

        construction = terrain.GetComponent<ConstructionSite>();
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
                out groundHit,
                currentObject.transform.rotation,
                10000,
                groundMask
            );
            if (hitDetect)
            {
                boxLegal = !Physics.BoxCast(
                    ray.origin,
                    currentObject.transform.localScale / 2,
                    ray.direction,
                    currentObject.transform.rotation,
                    10000,
                    illegalMask
                );
                currentObject.transform.position = new Vector3(
                    groundHit.point.x, currentObject.transform.position.y, groundHit.point.z
                );
                currentObject.GetComponent<MeshRenderer>().material = boxLegal ? legalPlan : illegalPlan;
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
        if (!boxLegal)
        {
            return;
        }
        currentObject.layer = 10;
        showBox = false;
        construction.CreatePloppableSite(
            currentObject.transform.position,
            currentObject.transform.lossyScale.x / 8,
            currentObject.transform.lossyScale.z / 8
        );
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
