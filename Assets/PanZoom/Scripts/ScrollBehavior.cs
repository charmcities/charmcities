using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * MIT license.
 * Copyright 2018 Alex Rupp-Coppi
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 *
 */

public class ScrollBehavior : MonoBehaviour {

    private Vector3 startPosition;
    private Vector3 scrollStartPosition;
    private Vector3? lastDelta = null;

    [SerializeField]
    [Tooltip("Defaults to attached Camera component or Camera.main if left null. Camera used to convert screen space to world space.")]
    private Camera referenceCamera = null; 

    [SerializeField]
    [Tooltip("Enable/Disable horizontal scrolling")]
    private bool isXAxisEnabled = false;

    [SerializeField]
    [Tooltip("Enable/Disable vertical scrolling")]
    private bool isYAxisEnabled = true;

    [SerializeField]
    [Tooltip("Minimum distance for user to drag pointer for scroll to be noticeable")]
    private readonly float precision = 1f;

    public bool IsScrolling { get; private set; }

    private void Awake()
    {
        if (referenceCamera == null)
        {
            referenceCamera = GetComponent<Camera>();

            if (referenceCamera == null)
            {
                referenceCamera = Camera.main;
            }
        }
    }
	
	void Update () {
        CheckIsScrolling();
        UpdatePosition();
	}

    void CheckIsScrolling()
    {
        int mouseButton = 0; 

        if (Input.GetMouseButtonDown(mouseButton))
        {
            IsScrolling = true;
            startPosition = transform.position;
            scrollStartPosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(mouseButton) || !Input.GetMouseButton(mouseButton))
        {
            IsScrolling = false;
        }
    }

    void UpdatePosition()
    {
        if (IsScrolling)
        {
            Vector3 delta = Input.mousePosition - scrollStartPosition;

            if (delta != lastDelta && delta.magnitude > precision)
            {
                lastDelta = delta;

                float clipDistance = 0.3f;

                Vector3 point = referenceCamera.ScreenToWorldPoint(new Vector3(delta.x, delta.y, clipDistance));
                Vector3 origin = referenceCamera.ScreenToWorldPoint(new Vector3(0, 0, clipDistance));

                Vector3 transformDelta = point - origin;

                // Debug.Log(transformDelta + ", " + delta);

                float x, y;

                x = isXAxisEnabled ? transformDelta.x : 0f;
                y = isYAxisEnabled ? transformDelta.y : 0f;

                transform.position = startPosition - new Vector3(x, y, 0f);
            }
        }
    }
}
