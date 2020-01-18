using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Dossamer.PanZoom
{
    [CustomEditor(typeof(PanZoomBehavior))]
    public class PanZoomBehaviorEditor : Editor
    {
        SerializedProperty inputMultiplierKeys;
        SerializedProperty inputMultiplierValues;

        SerializedProperty mouseButton;
        
        // [Tooltip("Defaults to attached Camera component or Camera.main if left null. Camera used to convert screen space to world space.")]
        SerializedProperty referenceCamera;

        // [Tooltip("Minimum distance for user to drag pointer for pan to be noticeable")]
        SerializedProperty precision;
        
        SerializedProperty axesValues;

        // [Tooltip("Object you want to move at a magnitude equal to the pan amount")]
        SerializedProperty focusTarget;

        // [Tooltip("Used for zoom clamping")]
        SerializedProperty minDistFromTarget;

        // [Tooltip("Speed along axes will be greater when distance from focus is greater if enabled")]
        SerializedProperty isContributionProportionalToDistFromTarget;

        // [Tooltip("Object with attached trigger collider that constrains camera position")]
        SerializedProperty triggerBoundaryObject;

        // [Tooltip("Optional--if left blank behavior will raycast against all colliders in scene to check for boundary.")]
        SerializedProperty triggerBoundaryLayerName;

        SerializedProperty minViewPortSize;
        SerializedProperty maxViewPortSize;

        // editor controls
        [SerializeField]
        bool areMultipliersShown = false;
        [SerializeField]
        bool areAxesShown = true;

        bool isEditingAxes = false;
        Dictionary<Axis, AxisProfile> axisProfilesMap = new Dictionary<Axis, AxisProfile>();

        struct AxisProfile
        {
            public AxisData data;
            public bool isBeingEdited; 
        }

        protected virtual void OnEnable()
        {
            // Setup the SerializedProperties
            inputMultiplierKeys = serializedObject.FindProperty("inputMultipliers").FindPropertyRelative("keys");
            inputMultiplierValues = serializedObject.FindProperty("inputMultipliers").FindPropertyRelative("values");

            axesValues = serializedObject.FindProperty("axes").FindPropertyRelative("values");

            mouseButton = serializedObject.FindProperty("mouseButton");
            referenceCamera = serializedObject.FindProperty("referenceCamera");
            precision = serializedObject.FindProperty("precision");
            focusTarget = serializedObject.FindProperty("focusTarget");
            minDistFromTarget = serializedObject.FindProperty("minForwardDistanceFromFocusTarget");
            isContributionProportionalToDistFromTarget = serializedObject.FindProperty("isContributionProportionalToDistFromTarget");

            triggerBoundaryObject = serializedObject.FindProperty("triggerBoundaryObject");
            triggerBoundaryLayerName = serializedObject.FindProperty("triggerBoundaryLayerName");

            minViewPortSize = serializedObject.FindProperty("minViewPortSize");
            maxViewPortSize = serializedObject.FindProperty("maxViewPortSize");
        }

        public void OnSceneGUI()
        {
            PanZoomBehavior panZoom = (PanZoomBehavior)target;

            panZoom.UpdateAxes();
            panZoom.DrawDebugRays();

            // edit individual axes orientations visually
            if (isEditingAxes)
            {
                serializedObject.Update();

                // update list of axes being targeted
                for (int i = 0; i < axesValues.arraySize; i++)
                {
                    SerializedProperty sp = axesValues.GetArrayElementAtIndex(i);

                    AxisData tempData = new AxisData
                    {
                        key = (Axis)sp.FindPropertyRelative("key").enumValueIndex,
                        orientation = sp.FindPropertyRelative("orientation").quaternionValue,
                        IsOverriden = sp.FindPropertyRelative("IsOverriden").boolValue,
                        IsEnabled = sp.FindPropertyRelative("IsEnabled").boolValue,
                        inputMethod = (InputMethod)sp.FindPropertyRelative("inputMethod").enumValueIndex
                    };

                    if (tempData.IsEnabled && tempData.IsOverriden)
                    {
                        if (!axisProfilesMap.ContainsKey(tempData.key))
                        {
                            // Debug.Log("added property");
                            axisProfilesMap.Add(tempData.key, new AxisProfile { data = tempData, isBeingEdited = false });
                        }
                        else
                        {
                            // update orientation of old entry,
                            // but preserve isBeingEdited
                            axisProfilesMap[tempData.key] = new AxisProfile { data = tempData, isBeingEdited = axisProfilesMap[tempData.key].isBeingEdited };
                        }
                    }
                    else if (axisProfilesMap.ContainsKey(tempData.key))
                    {
                        axisProfilesMap.Remove(tempData.key);
                    }
                }
                
                

            // window with axis selection
            // see https://forum.unity.com/threads/unexplained-guilayout-mismatched-issue-is-it-a-unity-bug-or-a-miss-understanding.158375/
            //if (Event.current.type == EventType.Layout)
            //{ ^^ seems to block drawing UI in OnSceneGUI
                Handles.BeginGUI();

                GUILayout.BeginVertical("Box", GUILayout.MaxWidth(Screen.width / 3));

                GUILayout.Label("Axes to edit");

                if (axisProfilesMap.Count > 0)
                {
                    // deep copy of keys to keep dictionary in sync
                    List<Axis> keysDeepCopy = new List<Axis>();
                    foreach (Axis key in axisProfilesMap.Keys)
                    {
                        keysDeepCopy.Add(key);
                    }

                    foreach (var key in keysDeepCopy)
                    {
                        GUILayout.BeginHorizontal();
                        bool flag = GUILayout.Toggle(axisProfilesMap[key].isBeingEdited, "Axis " + axisProfilesMap[key].data.key.ToString());
                        GUILayout.Label(axisProfilesMap[key].data.orientation.eulerAngles.ToString());
                        GUILayout.EndHorizontal();

                        axisProfilesMap[key] = new AxisProfile { data = axisProfilesMap[key].data, isBeingEdited = flag };
                    }
                }
                else
                {
                    GUILayout.Label("Override an enabled axis in the inspector to edit here.");
                }

                if (GUILayout.Button("Close <esc>") || Event.current.keyCode == KeyCode.Escape)
                {
                    isEditingAxes = false;
                }

                GUILayout.EndVertical();

                Handles.EndGUI();

                // handle actual rotation

                Vector3 pos = HandleUtility.GUIPointToWorldRay(new Vector2(Screen.width * 0.25f, Screen.height * 0.67f)).GetPoint(1f);

                // get any value from the dictionary
                var e = axisProfilesMap.GetEnumerator();
                e.MoveNext();
                var orientation = e.Current.Value.data.orientation;

                EditorGUI.BeginChangeCheck();
                Quaternion rot = Handles.RotationHandle(orientation, pos);
                if (EditorGUI.EndChangeCheck())
                {
                    // see https://stackoverflow.com/questions/22157435/difference-between-the-two-quaternions
                    // take the rotation delta and apply to all active axes

                    Quaternion diff = rot * Quaternion.Inverse(orientation);

                    foreach (var kvp in axisProfilesMap)
                    {
                        if (kvp.Value.isBeingEdited)
                        {
                            for (int i = 0; i < axesValues.arraySize; i++)
                            {
                                SerializedProperty sp = axesValues.GetArrayElementAtIndex(i);

                                if ((Axis)sp.FindPropertyRelative("key").enumValueIndex == kvp.Key)
                                {
                                    sp.FindPropertyRelative("orientation").quaternionValue = diff * kvp.Value.data.orientation;
                                }
                            }
                        }
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        public override void OnInspectorGUI()
        {
            // Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
            serializedObject.Update();

            EditorGUILayout.Space();

            // Show the custom GUI controls.
            EditorGUILayout.PropertyField(mouseButton, new GUIContent("Mouse Button", "Button that triggers pointer drag"));
            EditorGUILayout.PropertyField(precision, new GUIContent("Mouse Precision", "Minimum distance for user to drag pointer for pan to be noticeable"));

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(referenceCamera, new GUIContent("Reference Camera", "Defaults to attached Camera component or Camera.main if left null. Camera used to convert screen space to world space."));
            EditorGUILayout.PropertyField(focusTarget, new GUIContent("Focus Target", "Object you want to move at a magnitude equal to the pan amount"));
            EditorGUILayout.PropertyField(minDistFromTarget, new GUIContent("Min Distance From Focus", "Used for zoom clamping"));
            EditorGUILayout.PropertyField(isContributionProportionalToDistFromTarget, new GUIContent("Proportional Move Speed", "Speed along axes will be greater when distance from focus is greater if enabled"));

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(triggerBoundaryObject, new GUIContent("Boundary Object", "Object with attached trigger collider that constrains camera position"));
            EditorGUILayout.PropertyField(triggerBoundaryLayerName, new GUIContent("Boundary Layer Name", "Optional--if left blank behavior will raycast against all colliders in scene to check for boundary."));

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Orthographic Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(minViewPortSize, new GUIContent("Min Viewport Size"));
            EditorGUILayout.PropertyField(maxViewPortSize, new GUIContent("Max Viewport Size"));

            PanZoomBehavior panZoom = (PanZoomBehavior)target;

            EditorGUILayout.Space();

            areMultipliersShown = EditorGUILayout.Foldout(areMultipliersShown, areMultipliersShown ? "Input Axis Multipliers" : "Show Input Multipliers");

            if (areMultipliersShown)
            {
                EditorGUILayout.LabelField(new GUIContent("[Hover here for note]", "Note that multipliers configured for a perspective camera may need to be reconfigured for use with an orthographic camera."));

                for (int i = 0; i < inputMultiplierKeys.arraySize; i++)
                {
                    InputMethod method = (InputMethod)inputMultiplierKeys.GetArrayElementAtIndex(i).enumValueIndex;
                    float floatValue = inputMultiplierValues.GetArrayElementAtIndex(i).floatValue;
                    inputMultiplierValues.GetArrayElementAtIndex(i).floatValue = EditorGUILayout.FloatField(new GUIContent(method.ToString()), floatValue);
                }
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Edit Overriden Axes"))
            {
                isEditingAxes = true;
                SceneView.RepaintAll();
            }

            EditorGUILayout.Space();

            areAxesShown = EditorGUILayout.Foldout(areAxesShown, areAxesShown ? "Configure Axes" : "Show Axes");

            if (areAxesShown)
            {
                for (int i = 0; i < axesValues.arraySize; i ++)
                {
                    SerializedProperty sp = axesValues.GetArrayElementAtIndex(i);
                    
                    AxisData tempData = new AxisData {
                        key = (Axis)sp.FindPropertyRelative("key").enumValueIndex,
                        orientation = sp.FindPropertyRelative("orientation").quaternionValue,
                        IsOverriden = sp.FindPropertyRelative("IsOverriden").boolValue,
                        IsEnabled = sp.FindPropertyRelative("IsEnabled").boolValue,
                        inputMethod = (InputMethod)sp.FindPropertyRelative("inputMethod").enumValueIndex,
                        secondaryInputMethod = (InputMethod)sp.FindPropertyRelative("secondaryInputMethod").enumValueIndex,
                        IsSecondaryInputEnabled = sp.FindPropertyRelative("IsSecondaryInputEnabled").boolValue,
                        DoesSecondaryNeedTrigger = sp.FindPropertyRelative("DoesSecondaryNeedTrigger").boolValue,
                        secondaryInputTrigger = sp.FindPropertyRelative("secondaryInputTrigger").stringValue,
                        customInputMethod = sp.FindPropertyRelative("customInputMethod").stringValue,
                        customSecondaryInputMethod = sp.FindPropertyRelative("customSecondaryInputMethod").stringValue
                    };

                    EditorGUILayout.Space();

                    GUILayout.Label("Axis " + tempData.key.ToString(), EditorStyles.boldLabel);

                    tempData.IsEnabled = GUILayout.Toggle(tempData.IsEnabled, new GUIContent("Enabled?"));

                    if (tempData.IsEnabled)
                    {
                        tempData.IsOverriden = GUILayout.Toggle(tempData.IsOverriden, new GUIContent("Override axis value?"));
                        if (tempData.IsOverriden)
                        {
                            tempData.orientation = Quaternion.Euler(EditorGUILayout.Vector3Field(new GUIContent("Orientation"), tempData.orientation.eulerAngles));
                        }

                        tempData.inputMethod = (InputMethod)EditorGUILayout.EnumPopup(new GUIContent("Input Method"), tempData.inputMethod);

                        if (tempData.inputMethod == InputMethod.CustomAxis)
                        {
                            tempData.customInputMethod = EditorGUILayout.TextField(new GUIContent("Custom Axis Name"), tempData.customInputMethod);
                        }

                        tempData.IsSecondaryInputEnabled = GUILayout.Toggle(tempData.IsSecondaryInputEnabled, new GUIContent("Enable secondary input?"));

                        if (tempData.IsSecondaryInputEnabled)
                        {
                            tempData.secondaryInputMethod = (InputMethod)EditorGUILayout.EnumPopup(new GUIContent("Secondary Input Method"), tempData.secondaryInputMethod);

                            if (tempData.secondaryInputMethod == InputMethod.CustomAxis)
                            {
                                tempData.customSecondaryInputMethod = EditorGUILayout.TextField(new GUIContent("Custom Axis Name"), tempData.customSecondaryInputMethod);
                            }

                            tempData.DoesSecondaryNeedTrigger = GUILayout.Toggle(tempData.DoesSecondaryNeedTrigger, new GUIContent("Is secondary triggered by button?"));

                            if (tempData.DoesSecondaryNeedTrigger)
                            {
                                tempData.secondaryInputTrigger = EditorGUILayout.TextField(new GUIContent("Trigger Button Name"), tempData.secondaryInputTrigger);
                            }
                        }
                    }

                    sp.FindPropertyRelative("IsEnabled").boolValue = tempData.IsEnabled;
                    sp.FindPropertyRelative("IsOverriden").boolValue = tempData.IsOverriden;
                    sp.FindPropertyRelative("orientation").quaternionValue = tempData.orientation;
                    sp.FindPropertyRelative("inputMethod").enumValueIndex = (int)tempData.inputMethod;
                    sp.FindPropertyRelative("secondaryInputMethod").enumValueIndex = (int)tempData.secondaryInputMethod;
                    sp.FindPropertyRelative("IsSecondaryInputEnabled").boolValue = tempData.IsSecondaryInputEnabled;
                    sp.FindPropertyRelative("DoesSecondaryNeedTrigger").boolValue = tempData.DoesSecondaryNeedTrigger;
                    sp.FindPropertyRelative("secondaryInputTrigger").stringValue = tempData.secondaryInputTrigger;
                    sp.FindPropertyRelative("customInputMethod").stringValue = tempData.customInputMethod;
                    sp.FindPropertyRelative("customSecondaryInputMethod").stringValue = tempData.customSecondaryInputMethod;
                }

                EditorGUILayout.Space();
            }
            
            // Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
            serializedObject.ApplyModifiedProperties();
        }
    }
}