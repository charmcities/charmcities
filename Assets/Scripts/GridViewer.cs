using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]

public class GridViewer : MonoBehaviour
{
    public float minPixels = 20;
    
    Renderer render;

    private void Start()
    {
        render = GetComponent<Renderer>();
    }

    private void LateUpdate()
    {
        // If the central grid squares are not, at minimum, minPixels in diameter, turn off the grid and be done.
        if (PixelSize.GetPixelSize(8, transform) < minPixels)
        {
            render.enabled = false;
            return;
        }

        // Otherwise, turn it on.
        render.enabled = true;

        // Find the center point where the camera is looking
        RaycastHit hit;
        Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit);
        Vector3 center = hit.point;

        // Round to the nearest 8 meter grid square and fix the Y height to one.
        center = GridUtilities.RoundToNearestSquare(center, 1);

        // Place the center of the grid at the center point.
        transform.position = center;
    }
}
