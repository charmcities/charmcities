using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Dossamer.PanZoom
{
    [Serializable]
    public struct AxisData
    {
        public Axis key;
        public Quaternion orientation;
        public bool IsOverriden;
        public bool IsEnabled;
        public bool IsSecondaryInputEnabled;
        public bool DoesSecondaryNeedTrigger;
        public string secondaryInputTrigger;
        public string customInputMethod;
        public string customSecondaryInputMethod;
        public InputMethod inputMethod;
        public InputMethod secondaryInputMethod;
    }

    [Serializable]
    public enum Axis
    {
        RIGHT,
        UP,
        FORWARD
    }

    [Serializable]
    public enum InputMethod
    {
        PointerHorizontal,    // aka mouse, touch
        PointerVertical,
        HorizontalAxis,       // gamepad, keyboard
        VerticalAxis,
        Scrollwheel, // mouse-specific
        CustomAxis, // user-specified
        PinchZoom,
        Null
    }

    [Serializable]
    public enum MouseButton
    {
        Left = 0,
        Right = 1,
        Middle = 2
    }

    [Serializable]
    public class PanZoomBehavior : MonoBehaviour
    {
        [Serializable] public class InputMethodFloatDictionary : SerializableDictionary<InputMethod, float> { }
        [SerializeField]
        public InputMethodFloatDictionary inputMultipliers = new InputMethodFloatDictionary
        {
            { InputMethod.HorizontalAxis, 1f },
            { InputMethod.VerticalAxis, 1f },
            { InputMethod.PointerHorizontal, 1f },
            { InputMethod.PointerVertical, 1f },
            { InputMethod.Scrollwheel, 1f },
            { InputMethod.CustomAxis, 1f },
            { InputMethod.PinchZoom, 2f }
        };

        [SerializeField]
        protected MouseButton mouseButton = MouseButton.Left;

        [SerializeField]
        private float minViewPortSize = 0.3f;
        [SerializeField]
        private float maxViewPortSize = 50f; 

        private Vector3 startPosition;
        protected Vector3 mousePanStartPosition;
        protected Vector3? lastMouseDelta = null;

        protected float? lastPinchMagnitude = null;

        [SerializeField]
        protected Camera referenceCamera = null; 

        public Camera GetReferenceCamera()
        {
            return referenceCamera;
        }

        [SerializeField]
        protected float precision = 1f;

        public bool IsPanning { get; protected set; }

        [Serializable] public class AxisAxisDataDictionary : SerializableDictionary<Axis, AxisData> { }
        [SerializeField]
        public AxisAxisDataDictionary axes = new AxisAxisDataDictionary {
            {Axis.RIGHT, new AxisData
            {
                key = Axis.RIGHT,
                orientation = Quaternion.LookRotation(Vector3.right),
                IsOverriden = false,
                IsEnabled = true,
                inputMethod = InputMethod.PointerHorizontal,
                secondaryInputMethod = InputMethod.Null,
                IsSecondaryInputEnabled = false,
                DoesSecondaryNeedTrigger = false,
                secondaryInputTrigger = null,
                customInputMethod = null,
                customSecondaryInputMethod = null
            } },

            {Axis.UP, new AxisData
            {
                key = Axis.UP,
                orientation = Quaternion.LookRotation(Vector3.up),
                IsOverriden = false,
                IsEnabled = true,
                inputMethod = InputMethod.PointerVertical,
                secondaryInputMethod = InputMethod.Null,
                IsSecondaryInputEnabled = false,
                DoesSecondaryNeedTrigger = false,
                secondaryInputTrigger = null,
                customInputMethod = null,
                customSecondaryInputMethod = null
            } },

            {Axis.FORWARD, new AxisData
            {
                key = Axis.FORWARD,
                orientation = Quaternion.LookRotation(Vector3.forward),
                IsOverriden = false,
                IsEnabled = true,
                inputMethod = InputMethod.Scrollwheel,
                secondaryInputMethod = InputMethod.Null,
                IsSecondaryInputEnabled = false,
                DoesSecondaryNeedTrigger = false,
                secondaryInputTrigger = null,
                customInputMethod = null,
                customSecondaryInputMethod = null
            } }
        };


        [SerializeField]
        protected GameObject focusTarget = null;

        [SerializeField]
        private float minForwardDistanceFromFocusTarget = 0.5f;

        [SerializeField]
        private float minFieldOfView = 5f;
        [SerializeField]
        private float maxFieldOfView = 60f;

        [SerializeField]
        private GameObject triggerBoundaryObject = null;

        [SerializeField]
        private string triggerBoundaryLayerName = null; 

        private Vector3 lastTransformPosition;

        [SerializeField]
        protected bool isContributionProportionalToDistFromTarget = true; 

        public event EventHandler PanStarted;
        public event EventHandler PanEnded;

        protected void OnPanStarted(EventArgs e)
        {
            if (PanStarted != null)
            {
                PanStarted(this, e);
            }
        }

        protected void OnPanEnded(EventArgs e)
        {
            if (PanEnded != null)
            {
                PanEnded(this, e);
            }
        }

        public float GetMinForwardDistFromTarget()
        {
            return minForwardDistanceFromFocusTarget;
        }

        public GameObject GetFocusTarget()
        {
            return focusTarget;
        }

        public void ReassignTarget(GameObject target)
        {
            focusTarget = target;
        }

        protected Dictionary<InputMethod, bool> isInputMethodActive = new Dictionary<InputMethod, bool>()
        {
            { InputMethod.HorizontalAxis, false },
            { InputMethod.VerticalAxis, false },
            { InputMethod.PointerHorizontal, false },
            { InputMethod.PointerVertical, false },
            { InputMethod.Scrollwheel, false },
            { InputMethod.CustomAxis, false },
            { InputMethod.PinchZoom, false },
            { InputMethod.Null, false } // null is never true
        };

        protected virtual void Awake()
        {
            if (referenceCamera == null)
            {
                referenceCamera = GetComponent<Camera>();

                if (referenceCamera == null)
                {
                    referenceCamera = Camera.main;
                }
            }
            
            IsPanning = false;
            lastTransformPosition = transform.position;
        }

        void Update()
        {
            UpdateAxes();
            CheckIsPanning();
            UpdatePosition();

            // Handle raydrawing in editor OnSceneGUI
            // DrawDebugRays();
        }

        private void LateUpdate()
        {
            // if PanZoomEasingBehavior is a component, 
            // camera may have overshot its bounds because of tween--clamp it.

            if (focusTarget != null && GetForwardDotToFocusTarget() > 0)
            {
                ClampPerspectiveClose();
            }

            // clamp camera viewport bounds (bounds can be modified by other scripts)
            referenceCamera.orthographicSize = Mathf.Clamp(referenceCamera.orthographicSize, minViewPortSize, maxViewPortSize);

            // Debug.DrawLine(referenceCamera.transform.position, lastTransformPosition, Color.yellow, 10f);

            ClampPositionToTriggerBoundary();

            // Debug.DrawLine(referenceCamera.transform.position, lastTransformPosition, Color.grey, 10f);


            // call this after everything else
            lastTransformPosition = referenceCamera.transform.position;

            // DebugDrawPoint(lastTransformPosition, new Color(1f, 0.5f, 0.5f));
            

        }

        private bool IsCameraInBoundary()
        {
            return IsPositionInBoundary(referenceCamera.transform.position);
        }

        /// <summary>
        /// Returns false if boundary is null.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private bool IsPositionInBoundary(Vector3 position)
        {
            if (triggerBoundaryObject != null)
            {
                Collider[] colliders;

                Vector3 center = triggerBoundaryObject.GetComponent<Collider>().bounds.center;

                float radius = 0.001f;
                float fudgeFactor = 0f; // 0.01f; // give some tolerance to what qualifies as 'in'--rounding errors seem to be causing problems

                Vector3 rayDirection = (center - position).normalized;

                Vector3 castOrigin = position + rayDirection * (fudgeFactor + radius); 

                if (triggerBoundaryLayerName != null && triggerBoundaryLayerName != "")
                {
                    int collisionMask = 1 << LayerMask.NameToLayer(triggerBoundaryLayerName);

                    colliders = Physics.OverlapSphere(castOrigin, radius, collisionMask);
                }
                else
                {
                    colliders = Physics.OverlapSphere(castOrigin, radius);
                }

                foreach (Collider collider in colliders)
                {
                    if (collider.gameObject == triggerBoundaryObject)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private RaycastHit[] BoundaryCast(Vector3 start, Vector3 end)
        {
            RaycastHit[] hits;

            Vector3 difference = end - start;

            if (triggerBoundaryLayerName != null && triggerBoundaryLayerName != "")
            {
                int collisionMask = 1 << LayerMask.NameToLayer(triggerBoundaryLayerName);

                hits = Physics.RaycastAll(start, difference.normalized, difference.magnitude, collisionMask);
            }
            else
            {
                hits = Physics.RaycastAll(start, difference.normalized, difference.magnitude);
            }

            // Debug.DrawLine(referenceCamera.transform.position, referenceCamera.transform.position + difference, Color.green, 10f);

            return hits;
        }

        private float debugOffset = 0.1f;

        /// <summary>
        /// In orthographic mode don't want axis orthogonal to up and right to be affected
        /// </summary>
        private void UndoCameraOrthogonalAxisTranslation()
        {
            Vector3 camOrthogonal = GetCrossVector(); // referenceCamera.transform.rotation * Vector3.forward;

            Vector3 translation = referenceCamera.transform.position - lastTransformPosition;

            Vector3 projection = camOrthogonal * Vector3.Dot(camOrthogonal, translation);

            referenceCamera.transform.position -= projection;
        }

        private void ClampPositionToTriggerBoundary()
        {
            ClampPositionToTriggerBoundary(lastTransformPosition);
        }
        
        private void DebugDrawPoint(Vector3 position, Color color)
        {
            float mag = 0.05f;
            Debug.DrawLine(position - Vector3.forward * mag, position + Vector3.forward * mag, color, 10f);
            Debug.DrawLine(position - Vector3.right * mag, position + Vector3.right * mag, color, 10f);
            Debug.DrawLine(position - Vector3.up * mag, position + Vector3.up * mag, color, 10f);
        }

        private void ClampPositionToTriggerBoundary(Vector3 lastFinalPosition)
        {
            // Debug.Log(IsCameraInBoundary() ? "camera in boundary" : "camera not in boundary");

            if (triggerBoundaryObject != null && !IsCameraInBoundary())
            {
                RaycastHit[] hits;

                // Vector3 difference = referenceCamera.transform.position - lastFinalPosition;

                hits = BoundaryCast(referenceCamera.transform.position, lastFinalPosition);

                bool wasBoundaryIntersected = false;

                float offset = debugOffset; //0.01f;

                foreach (RaycastHit hit in hits)
                {
                    if (hit.collider.gameObject == triggerBoundaryObject)
                    {
                        // DebugDrawPoint(referenceCamera.transform.position, Color.blue);

                        // Debug.Log("adjusting by normal");

                        Vector3 surfaceNormal = hit.normal.normalized;
                        Vector3 vectorFromHitToCam = referenceCamera.transform.position - hit.point;

                        float dot = Vector3.Dot(surfaceNormal, vectorFromHitToCam); // * 1.5f; // because of what I'm assuming are rounding errors, sometimes dot magnitude falls just slightly short

                        Vector3 cache = referenceCamera.transform.position;

                        referenceCamera.transform.position -= dot * surfaceNormal; // (dot + offset) * surfaceNormal;

                        lastFinalPosition = referenceCamera.transform.position;

                        // Debug.DrawRay(hit.point, hit.normal, Color.magenta, 10f);

                        // Debug.DrawLine(cache, lastFinalPosition, Color.cyan, 10f);

                        // DebugDrawPoint(lastFinalPosition, Color.yellow);

                        int maxTries = 100; 

                        for (int i = 0; i < maxTries; i ++) {
                            if (IsCameraInBoundary())
                            {
                                break;
                            }

                            var newHits = BoundaryCast(referenceCamera.transform.position, triggerBoundaryObject.GetComponent<Collider>().bounds.center);

                            foreach (RaycastHit newHit in newHits)
                            {
                                if (newHit.collider.gameObject == triggerBoundaryObject)
                                {
                                    referenceCamera.transform.position = newHit.point; // - newHit.normal * 0.0001f; 
                                    // Debug.DrawRay(newHit.point, newHit.normal, Color.magenta, 10f);
                                    // Debug.Log("iterations: " + i);
                                    break;
                                }
                            }

                            if (i == maxTries - 1)
                            {
                                Debug.Log("couldn't move inside boundary");
                            }
                        }

                        lastFinalPosition = referenceCamera.transform.position;

                        // Debug.DrawLine(cache, lastFinalPosition, new Color(0, 0.3f, 0.6f), 10f);

                        // DebugDrawPoint(lastFinalPosition, Color.cyan);

                        // necessary b/c in cases where e.g. boundary collider is a sphere,
                        // don't want cam translating along it
                        // (b/c will end up clipping ground)

                        // if (referenceCamera.orthographic)
                        // {
                            UndoCameraOrthogonalAxisTranslation();
                        // }



                        // lastTransformPosition = referenceCamera.transform.position;

                        // Debug.Log("recursive call");

                        ClampPositionToTriggerBoundary(lastFinalPosition); // handles corner edge case

                        wasBoundaryIntersected = true;
                        break;
                    }
                }

                if (!wasBoundaryIntersected)
                {
                    // DebugDrawPoint(referenceCamera.transform.position, Color.red);

                    // Debug.Log("no boundary intersected");

                    // previous position wasn't within boundary either; 
                    // need to cast rays again, using boundary origin as endpoint
                    // offset by cross axis (trying to maintain vertical position)

                    Vector3 triggerCenter = triggerBoundaryObject.GetComponent<Collider>().bounds.center;
                    Vector3 camOrthogonal = GetCrossVector(); // referenceCamera.transform.rotation * Vector3.forward;

                    Vector3 difference = referenceCamera.transform.position - triggerCenter;

                    Vector3 projection = camOrthogonal * Vector3.Dot(camOrthogonal, difference);

                    Vector3 modifiedCenter = triggerCenter + projection;

                    int iterations = 30;
                    Vector3 step = (triggerCenter - modifiedCenter) / iterations; 

                    for (int i = 0; i < iterations; i ++)
                    {
                        hits = BoundaryCast(referenceCamera.transform.position, modifiedCenter + step * i);

                        // DebugDrawPoint(modifiedCenter + step * i, Color.blue);
                        // Debug.Log("point " + i); 

                        if (hits.Length > 0)
                        {
                            bool flag = false; 

                            foreach (var hit in hits)
                            {
                                if (hit.collider.gameObject == triggerBoundaryObject)
                                {
                                    referenceCamera.transform.position = hit.point; // - hit.normal * 0.0001f;
                                    // Debug.DrawRay(hit.point, hit.normal, Color.magenta, 10f);
                                    flag = true; 
                                    break;
                                }
                            }

                            if (flag)
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }

        public void DrawDebugRays()
        {
            Debug.DrawRay(transform.position, axes[Axis.RIGHT].orientation * Vector3.forward, axes[Axis.RIGHT].IsEnabled ? Color.green : Color.red);
            Debug.DrawRay(transform.position, axes[Axis.UP].orientation * Vector3.forward, axes[Axis.UP].IsEnabled ? Color.green : Color.red);
            Debug.DrawRay(transform.position, axes[Axis.FORWARD].orientation * Vector3.forward, axes[Axis.FORWARD].IsEnabled ? Color.green : Color.red);

            if (focusTarget != null && referenceCamera != null) {
                Debug.DrawLine(transform.position, focusTarget.transform.position, Color.cyan);

                Vector3 dir = axes[Axis.FORWARD].orientation * Vector3.forward;
                float dot = Vector3.Dot(referenceCamera.transform.position - focusTarget.transform.position, dir);
                Debug.DrawLine(referenceCamera.transform.position, referenceCamera.transform.position - dir * dot, Color.cyan);
                Debug.DrawLine(focusTarget.transform.position, referenceCamera.transform.position - dir * dot, Color.cyan);

                dir = Vector3.Cross(axes[Axis.RIGHT].orientation * Vector3.forward, axes[Axis.UP].orientation * Vector3.forward).normalized;
                dot = Vector3.Dot(referenceCamera.transform.position - focusTarget.transform.position, dir);
                Debug.DrawLine(referenceCamera.transform.position, referenceCamera.transform.position - dir * dot, Color.magenta);
                Debug.DrawLine(focusTarget.transform.position, referenceCamera.transform.position - dir * dot, Color.magenta);
            }
        }

        public float GetClampedCameraSize(float size)
        {
            return Mathf.Clamp(size, minViewPortSize, maxViewPortSize);
        }

        public float GetClampedCameraFOV(float size)
        {
            return Mathf.Clamp(size, minFieldOfView, maxFieldOfView);
        }

        public void UpdateAxes()
        {
            foreach (Axis axis in (Axis[])Enum.GetValues(typeof(Axis)))
            {
                AxisData axisData = axes[axis];

                if (!axisData.IsOverriden)
                {
                    Quaternion angle = Quaternion.LookRotation(Vector3.forward);
                    switch (axis)
                    {
                        case Axis.RIGHT:
                            angle = Quaternion.LookRotation(transform.rotation * Vector3.right);
                            break;
                        case Axis.UP:
                            angle = Quaternion.LookRotation(transform.rotation * Vector3.up);
                            break;
                        case Axis.FORWARD:
                            angle = Quaternion.LookRotation(transform.rotation * Vector3.forward);
                            break;
                    }

                    AxisData modifiedAxis = axisData;
                    modifiedAxis.orientation = angle;

                    axes[axis] = modifiedAxis;
                }
            }
        }

        protected virtual void CheckIsPanning()
        {

            // reset values
            var resetCopy = new SerializableDictionary<InputMethod, bool>();

            foreach (InputMethod method in isInputMethodActive.Keys)
            {
                resetCopy[method] = false;
            }

            isInputMethodActive = resetCopy;


            if (GetAxesOfType(InputMethod.HorizontalAxis).Count > 0 && Input.GetAxis("Horizontal") != 0f)
            {
                isInputMethodActive[InputMethod.HorizontalAxis] = true;
            }

            if (GetAxesOfType(InputMethod.VerticalAxis).Count > 0 && Input.GetAxis("Vertical") != 0f)
            {
                isInputMethodActive[InputMethod.VerticalAxis] = true;
            }

            if (GetAxesOfType(InputMethod.PointerHorizontal).Count > 0 && Input.GetMouseButton((int)mouseButton) && Input.touchCount <= 1)
            {
                isInputMethodActive[InputMethod.PointerHorizontal] = true;
            }

            if (GetAxesOfType(InputMethod.PointerVertical).Count > 0 && Input.GetMouseButton((int)mouseButton) && Input.touchCount <= 1)
            {
                isInputMethodActive[InputMethod.PointerVertical] = true;
            }

            if (GetAxesOfType(InputMethod.Scrollwheel).Count > 0 && Input.mouseScrollDelta.y != 0) {
                isInputMethodActive[InputMethod.Scrollwheel] = true;
            }

            List<AxisData> customAxes = GetAxesOfType(InputMethod.CustomAxis);
            if (customAxes.Count > 0) {
                foreach (AxisData axis in customAxes)
                {
                    if (axis.inputMethod == InputMethod.CustomAxis)
                    {
                        if (Input.GetAxis(axis.customInputMethod) != 0)
                        {
                            isInputMethodActive[InputMethod.CustomAxis] = true;
                        }
                    }

                    if (axis.secondaryInputMethod == InputMethod.CustomAxis)
                    {
                        if (Input.GetAxis(axis.customSecondaryInputMethod) != 0)
                        {
                            isInputMethodActive[InputMethod.CustomAxis] = true;
                        }
                    }
                }
            }

            if (GetAxesOfType(InputMethod.PinchZoom).Count > 0 && GetPinchZoomImmediateMagnitude() != 0)
            {
                isInputMethodActive[InputMethod.PinchZoom] = true;
            }

            bool wasPanning = IsPanning;

            IsPanning = false;

            foreach (bool status in isInputMethodActive.Values)
            {
                if (status)
                {
                    IsPanning = true;
                    break;
                }
            }

            if (IsPanning)
            {
                if (!wasPanning)
                {
                    OnPanStarted(EventArgs.Empty);
                }

                if (Input.GetMouseButtonDown((int)mouseButton))
                {
                    mousePanStartPosition = Input.mousePosition;
                    lastMouseDelta = null; 
                }

                if (Input.touchCount >= 2)
                {
                    if (Input.GetTouch(1).phase == TouchPhase.Began)
                    {
                        lastPinchMagnitude = null;
                    }
                }
            }
            else
            {
                if (wasPanning)
                {
                    OnPanEnded(EventArgs.Empty);
                }
            }
        }

        List<AxisData> GetAxesOfType(InputMethod method)
        {
            List<AxisData> axesList = new List<AxisData>();

            foreach (AxisData axis in axes.Values)
            {
                if (axis.IsEnabled)
                {
                    bool secondaryFlag = false;

                    if (axis.IsSecondaryInputEnabled)
                    {
                        // order matters--button check needs to come last
                        if (!axis.DoesSecondaryNeedTrigger || Input.GetButton(axis.secondaryInputTrigger))
                        {
                            if (axis.secondaryInputMethod == method)
                            {
                                secondaryFlag = true;
                            }
                        }
                    }
                    
                    if (axis.inputMethod == method || secondaryFlag)
                    {
                        axesList.Add(axis);
                    } 
                    
                }
            }

            return axesList;
        }

        public bool GetIsAxisEnabled(Axis axis)
        {
            return axes[axis].IsEnabled;
        }

        public void SetIsAxisEnabled(Axis axis, bool isEnabled)
        {
            AxisData axisData = axes[axis];
            axisData.IsEnabled = isEnabled;
            axes[axis] = axisData; 
        }

        void ClampPerspectiveClose()
        {
            float dotProduct = GetForwardDotToFocusTarget();

            Vector3 dir1 = GetCrossVector();
            Vector3 dir2 = axes[Axis.FORWARD].orientation * Vector3.forward;

            float cosine = Mathf.Cos(Vector3.Angle(dir1, dir2) * Mathf.Deg2Rad);

            float distance = dotProduct / cosine + referenceCamera.nearClipPlane + minForwardDistanceFromFocusTarget;

            referenceCamera.transform.position -= distance * dir2;
        }

        protected void AddAxisToContribution(AxisData axis, ref Vector3 contribution, float magnitude)
        {
            if (referenceCamera != null)
            {
                bool shouldContribute = true; 

                // zoom doesn't work via translation in orthographic mode
                if (axis.key == Axis.FORWARD)
                {
                    if (referenceCamera.orthographic)
                    {
                        referenceCamera.orthographicSize -= magnitude;
                        referenceCamera.orthographicSize = Mathf.Clamp(referenceCamera.orthographicSize, minViewPortSize, maxViewPortSize);
                        // Debug.Log(referenceCamera.orthographicSize);
                        shouldContribute = false;
                    }
                    else
                    {

                        if (focusTarget != null)
                        {

                            Vector3 potentialPosition = referenceCamera.transform.position + axis.orientation * Vector3.forward * magnitude * Time.deltaTime;

                            float dotProduct = GetForwardDotToFocusTarget();

                            if (GetForwardDotToFocusTarget(potentialPosition) > 0 && dotProduct <= 0)
                            {
                                // the proposed translation will overshoot the target (aka clip through the floor)
                                // so shorten it

                                shouldContribute = false;

                                // Debug.Log("adjusted position");

                                Vector3 dir1 = GetCrossVector();
                                Vector3 dir2 = axes[Axis.FORWARD].orientation * Vector3.forward;
                                float cosine = Mathf.Cos(Vector3.Angle(dir1, dir2) * Mathf.Deg2Rad);

                                float newMagnitude = dotProduct / cosine - minForwardDistanceFromFocusTarget - referenceCamera.nearClipPlane;

                                Vector3 translation = axis.orientation * Vector3.forward * newMagnitude;

                                // only add to the contribution if it's a non-zero change
                                if (referenceCamera.transform.position + translation != referenceCamera.transform.position)
                                {
                                    contribution += translation;
                                }
                            }
                            else if (dotProduct > 0)
                            {
                                // if we're already overshot, need to clamp
                                // Debug.Log("overshot, clamping");

                                shouldContribute = false;
                                ClampPerspectiveClose();

                            }

                            // zoomed in sufficiently, switch to FOV change
                            if (Mathf.Abs(dotProduct) <= minForwardDistanceFromFocusTarget)
                            {
                                referenceCamera.fieldOfView = Mathf.Clamp(referenceCamera.fieldOfView - magnitude, minFieldOfView, maxFieldOfView);

                                if (referenceCamera.fieldOfView < maxFieldOfView)
                                {
                                    shouldContribute = false;
                                }
                            }
                            else if (referenceCamera.fieldOfView < maxFieldOfView)
                            {
                                referenceCamera.fieldOfView = Mathf.Lerp(referenceCamera.fieldOfView, maxFieldOfView, 0.3f);
                            }
                        }
                    }
                }
                
                if (shouldContribute)
                {
                    contribution += axis.orientation * Vector3.forward * magnitude;
                }
            }
        }

        void AddMagnitudeToCamSize(float magnitude)
        {
            if (referenceCamera != null)
            {
                referenceCamera.orthographicSize += magnitude;
            }
        }

        protected virtual void UpdatePosition()
        {
            if (IsPanning)
            {
                Vector3 translation = Vector3.zero;

                if (isInputMethodActive[InputMethod.HorizontalAxis] ||
                    isInputMethodActive[InputMethod.VerticalAxis] ||
                    isInputMethodActive[InputMethod.Scrollwheel] ||
                    isInputMethodActive[InputMethod.CustomAxis])
                {
                    List<AxisData> horizontalAxes = GetAxesOfType(InputMethod.HorizontalAxis);
                    List<AxisData> verticalAxes = GetAxesOfType(InputMethod.VerticalAxis);
                    List<AxisData> scrollwheelAxes = GetAxesOfType(InputMethod.Scrollwheel);
                    List<AxisData> customAxes = GetAxesOfType(InputMethod.CustomAxis);

                    Vector3 contribution = Vector3.zero;

                    foreach (AxisData axis in horizontalAxes)
                    {
                        float magnitude = Input.GetAxis("Horizontal") * inputMultipliers[axis.inputMethod];
                        AddAxisToContribution(axis, ref contribution, magnitude);
                    }

                    foreach (AxisData axis in verticalAxes)
                    {
                        float magnitude = Input.GetAxis("Vertical") * inputMultipliers[axis.inputMethod];
                        AddAxisToContribution(axis, ref contribution, magnitude);
                    }

                    foreach (AxisData axis in scrollwheelAxes)
                    {
                        float magnitude = Input.mouseScrollDelta.y * inputMultipliers[axis.inputMethod];
                        AddAxisToContribution(axis, ref contribution, magnitude);
                    }

                    if (isInputMethodActive[InputMethod.CustomAxis])
                    {
                        foreach (AxisData axis in customAxes)
                        {
                            string axisString = "";

                            if (!axis.IsSecondaryInputEnabled)
                            {
                                axisString = axis.customInputMethod;
                            }
                            else if (isInputMethodActive[axis.inputMethod] && !isInputMethodActive[axis.secondaryInputMethod])
                            {
                                axisString = axis.customInputMethod;
                            }
                            else if (!isInputMethodActive[axis.inputMethod] && isInputMethodActive[axis.secondaryInputMethod])
                            {
                                axisString = axis.customSecondaryInputMethod;
                            }
                            else if (isInputMethodActive[axis.inputMethod] && isInputMethodActive[axis.secondaryInputMethod])
                            {
                                // both are active, go with the primary unless secondary's trigger is firing
                                if (!axis.DoesSecondaryNeedTrigger || !Input.GetButton(axis.secondaryInputTrigger))
                                {
                                    axisString = axis.customInputMethod;
                                }
                                else
                                {
                                    axisString = axis.customSecondaryInputMethod;
                                }
                            }

                            float magnitude = Input.GetAxis(axisString) * inputMultipliers[axis.inputMethod];
                            AddAxisToContribution(axis, ref contribution, magnitude);
                        }
                    }

                    // further adjust contribution proportionally to how far from focus target, if target isn't null
                    if (focusTarget != null && isContributionProportionalToDistFromTarget)
                    {
                        if (!referenceCamera.orthographic)
                        {
                            contribution *= Mathf.Abs(GetForwardDotToFocusTarget());
                        }
                        else
                        {
                            contribution *= referenceCamera.orthographicSize;
                        }
                    }

                    translation += contribution * Time.deltaTime;
                }

                if (isInputMethodActive[InputMethod.PointerHorizontal] || isInputMethodActive[InputMethod.PointerVertical])
                {
                    Vector3 mouseDelta = Input.mousePosition - mousePanStartPosition;

                    if (lastMouseDelta == null)
                    {
                        lastMouseDelta = mouseDelta;
                    }

                    if (mouseDelta != lastMouseDelta && mouseDelta.magnitude > precision)
                    {
                        float clipDistance = focusTarget == null ? referenceCamera.nearClipPlane : Mathf.Abs(GetForwardDotToFocusTarget());

                        Vector3 point = referenceCamera.ScreenToWorldPoint(new Vector3(mouseDelta.x - ((Vector3)lastMouseDelta).x, mouseDelta.y - ((Vector3)lastMouseDelta).y, clipDistance));
                        Vector3 origin = referenceCamera.ScreenToWorldPoint(new Vector3(0, 0, clipDistance));

                        // so transform component is on a plane parallel to the viewport
                        // but want it to be parallel towards plane that intersects (1, 0, 0) and (0, 1, 0)
                        Vector3 transformDelta = Quaternion.Inverse(referenceCamera.transform.rotation) * (point - origin);

                        // Debug.DrawLine(transformDelta, Vector3.zero, Color.green, 10f);

                        // Debug.Log("mouse x: " + ((mouseDelta.x - ((Vector3)lastMouseDelta).x)));
                        // Debug.Log("mouse y: " + ((mouseDelta.y - ((Vector3)lastMouseDelta).y)));

                        lastMouseDelta = mouseDelta;

                        List<AxisData> horizontalAxes = GetAxesOfType(InputMethod.PointerHorizontal);
                        List<AxisData> verticalAxes = GetAxesOfType(InputMethod.PointerVertical);

                        Vector3 contribution = Vector3.zero;

                        foreach (AxisData axis in horizontalAxes)
                        {
                            float magnitude = transformDelta.x * inputMultipliers[axis.inputMethod];
                            AddAxisToContribution(axis, ref contribution, magnitude);
                        }

                        foreach (AxisData axis in verticalAxes)
                        {
                            float magnitude = transformDelta.y * inputMultipliers[axis.inputMethod];
                            AddAxisToContribution(axis, ref contribution, magnitude);
                        }

                        // Debug.Log("x: " + transformDelta.x);
                        // Debug.Log("y: " + transformDelta.y);


                        translation += -1 * contribution; // flip the mouse contribution
                    }
                }

                if (isInputMethodActive[InputMethod.PinchZoom])
                { 
                    float pinchMagnitude = GetPinchZoomImmediateMagnitude();
                    // Debug.Log(pinchMagnitude + ", " + lastPinchMagnitude);

                    if (lastPinchMagnitude == null)
                    {
                        // Debug.Log("did a reset");
                        lastPinchMagnitude = pinchMagnitude;
                    }

                    if (pinchMagnitude != lastPinchMagnitude)
                    {
                        float deltaMagnitude = pinchMagnitude - (float)lastPinchMagnitude;

                        // Debug.Log(deltaMagnitude);

                        lastPinchMagnitude = pinchMagnitude;

                        List<AxisData> pinchZoomAxes = GetAxesOfType(InputMethod.PinchZoom);

                        Vector3 contribution = Vector3.zero;

                        foreach (AxisData axis in pinchZoomAxes)
                        {
                            float magnitude = deltaMagnitude * inputMultipliers[axis.inputMethod];

                            if (isContributionProportionalToDistFromTarget && referenceCamera.orthographic)
                            {
                                magnitude *= referenceCamera.orthographicSize;
                            }

                            AddAxisToContribution(axis, ref contribution, magnitude);
                        }

                        // further adjust contribution proportionally to how far from focus target, if target isn't null
                        if (focusTarget != null && isContributionProportionalToDistFromTarget)
                        {
                            if (!referenceCamera.orthographic)
                            {
                                contribution *= Mathf.Abs(GetForwardDotToFocusTarget());
                            }
                            else
                            {
                                contribution *= referenceCamera.orthographicSize;
                            }
                        }

                        translation += contribution; // * Time.deltaTime;
                    }
                }

                transform.position += translation;
            }
        }

        /// <summary>
        /// Returns magnitude of projected vector from focus to camera onto normal vector of plane intersecting UP & RIGHT axes
        /// </summary>
        /// <returns></returns>
        public float GetForwardDotToFocusTarget()
        {
            return GetForwardDotToFocusTarget(referenceCamera.transform.position);
        }

        /// <summary>
        /// Returns magnitude of projected vector from focus to passed position onto normal vector of plane intersecting UP & RIGHT axes
        /// </summary>
        /// <returns></returns>
        public float GetForwardDotToFocusTarget(Vector3 position)
        {
            if (focusTarget != null)
            {
                Vector3 dir = Vector3.Cross(axes[Axis.RIGHT].orientation * Vector3.forward, axes[Axis.UP].orientation * Vector3.forward).normalized;

                float mag = Vector3.Dot(position - focusTarget.transform.position, dir);

                return Mathf.Abs(mag) < referenceCamera.nearClipPlane ? Mathf.Sign(mag) * referenceCamera.nearClipPlane : mag;
            }

            Debug.LogError("[PanZoomBehavior] focusTarget is null");
            return Mathf.Infinity;
        }

        /// <summary>
        /// Returns normalized cross product of Right & Up axes
        /// </summary>
        /// <returns></returns>
        Vector3 GetCrossVector()
        {
            return Vector3.Cross(axes[Axis.RIGHT].orientation * Vector3.forward, axes[Axis.UP].orientation * Vector3.forward).normalized;
        }

        /// <summary>
        /// Returns a value between 0 and 1.
        /// </summary>
        /// <returns></returns>
        protected float GetPinchZoomImmediateMagnitude()
        {
            if (Input.touchCount >= 2)
            {
                Vector2 touch0, touch1;
                float absoluteDistance, proportionalDistance;
                touch0 = Input.GetTouch(0).position;
                touch1 = Input.GetTouch(1).position;

                absoluteDistance = Vector2.Distance(touch0, touch1);
                proportionalDistance = absoluteDistance / new Vector2(Screen.width, Screen.height).magnitude;

                return proportionalDistance;
            }

            return 0f;
        }
    }
}
