using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Add this behavior to objects you want to be able to click on to set as focus targets for a camera.
/// </summary>
public class EventOnClickBehavior : MonoBehaviour {

    public delegate void Del(GameObject obj);
    public static Del handler;

    private void OnMouseDown()
    {
        handler(this.gameObject);
    }
}
