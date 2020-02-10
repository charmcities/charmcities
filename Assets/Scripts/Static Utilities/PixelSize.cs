using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PixelSize
{
    /// <summary>
    /// Gets the size in pixels of a circle of a given diameter at a given distance from the camera.
    /// </summary>
    /// <param name="diameter">Diameter of circle, in meters</param>
    /// <param name="distance">Distance from camera, in meters</param>
    /// <returns>The dimension of the circle in pixels</returns>
    public static float GetPixelSize(float diameter, float distance)
    {
        return (diameter * Mathf.Rad2Deg * Screen.height) / (distance * Camera.main.fieldOfView);
    }

    /// <summary>
    /// Gets the size in pixels of a circle of a given diameter centered on a particular transform.
    /// </summary>
    /// <param name="diameter">Diameter of circle, in meters</param>
    /// <param name="target">Transform of the target</param>
    /// <returns>The dimension of the circle in pixels</returns>
    public static float GetPixelSize(float diameter, Transform target)
    {
        float distance = Vector3.Distance(target.position, Camera.main.transform.position);
        return GetPixelSize(diameter, distance);
    }

}
