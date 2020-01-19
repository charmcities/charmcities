// GENERATED AUTOMATICALLY FROM 'Assets/Base Controls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @BaseControls : IInputActionCollection, IDisposable
{
    private InputActionAsset asset;
    public @BaseControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Base Controls"",
    ""maps"": [
        {
            ""name"": ""Navigation"",
            ""id"": ""d26b12bd-9fd4-41a1-9c11-a9c9d58afefe"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""PassThrough"",
                    ""id"": ""fae80b8c-2011-4601-8aaa-d26119a0ab98"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Pointer Location"",
                    ""type"": ""PassThrough"",
                    ""id"": ""41da19f4-ab61-4ff5-8f7d-26c450a54d34"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Pointer Delta"",
                    ""type"": ""PassThrough"",
                    ""id"": ""78be999b-4d64-421a-8da0-453a3bda976a"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Orbit Shift"",
                    ""type"": ""Button"",
                    ""id"": ""00e3f3ab-f75b-4b9b-9f91-d5c561bc2601"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press(behavior=2)""
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""Arrow Keys"",
                    ""id"": ""9ff5b249-69d8-496a-8aff-9639961b6b0f"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""7b10a8c1-6600-4308-9618-d922966f9da1"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard Only;Keyboard and Mouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""6d3b250a-6000-4bf3-b822-e196cc5fbccc"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard Only;Keyboard and Mouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""dcc55648-3ef7-492f-b88d-d8aec090ef39"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard Only;Keyboard and Mouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""f51b01ef-1397-4233-a5e3-0e1d77331a65"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard Only;Keyboard and Mouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Right Stick"",
                    ""id"": ""0aa8d82c-d5f5-47ec-8e83-95e8099b84a4"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""810f221e-85a3-473a-bf16-71081eb537a9"",
                    ""path"": ""<Gamepad>/rightStick/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""b0a40874-21cd-422f-9bb6-dd8f02f4c164"",
                    ""path"": ""<Gamepad>/rightStick/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""bcc2da51-26a2-4ea8-ad2e-0f80ead4cb49"",
                    ""path"": ""<Gamepad>/rightStick/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""56b6a7fe-c046-4d8e-8680-fe05e8d1da9c"",
                    ""path"": ""<Gamepad>/rightStick/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""590b116c-1944-4775-b257-38030d2984a1"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse only;Keyboard and Mouse"",
                    ""action"": ""Pointer Location"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b84e871e-22d2-440f-b701-a45dcc4189b9"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Pointer Location"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1e76e0b9-0f8f-4874-851b-6f9a99088bb0"",
                    ""path"": ""<Keyboard>/rightShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard Only;Keyboard and Mouse"",
                    ""action"": ""Orbit Shift"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d60f0ca8-4a2c-4484-ab36-a3ddff6cd470"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard Only;Keyboard and Mouse"",
                    ""action"": ""Orbit Shift"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b0796cb6-0994-44a5-b2dc-ceaefda4012e"",
                    ""path"": ""<Mouse>/middleButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse only;Keyboard and Mouse"",
                    ""action"": ""Orbit Shift"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2d545d78-664a-43b9-a18d-ee56134e94b4"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Orbit Shift"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""bae1e739-d365-4957-b089-cc49e9106525"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse only;Keyboard and Mouse"",
                    ""action"": ""Pointer Delta"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Planning"",
            ""id"": ""3f62c431-8b53-47ac-a633-ffa190dcb671"",
            ""actions"": [
                {
                    ""name"": ""Position"",
                    ""type"": ""PassThrough"",
                    ""id"": ""788ed286-cb57-458e-b124-338212dd35fb"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Place"",
                    ""type"": ""Button"",
                    ""id"": ""872b47c2-057f-404c-b3f3-8b9f489b0753"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": ""Press""
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""86b7ab5c-2d84-4177-82bd-aedd963c9661"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse only;Keyboard and Mouse"",
                    ""action"": ""Position"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f945c844-58b4-4f51-bab0-3a75458ebc96"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Position"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""140d2263-a6e5-46c1-8e26-62020a0e881b"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Place"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""369377e4-f788-4c12-8814-f0b1b4908f7c"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard Only"",
                    ""action"": ""Place"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2030615e-6c03-4e5b-b5e1-df985e7afd10"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse only;Keyboard and Mouse"",
                    ""action"": ""Place"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Keyboard Only"",
            ""bindingGroup"": ""Keyboard Only"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Mouse only"",
            ""bindingGroup"": ""Mouse only"",
            ""devices"": [
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Keyboard and Mouse"",
            ""bindingGroup"": ""Keyboard and Mouse"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Gamepad"",
            ""bindingGroup"": ""Gamepad"",
            ""devices"": [
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Navigation
        m_Navigation = asset.FindActionMap("Navigation", throwIfNotFound: true);
        m_Navigation_Movement = m_Navigation.FindAction("Movement", throwIfNotFound: true);
        m_Navigation_PointerLocation = m_Navigation.FindAction("Pointer Location", throwIfNotFound: true);
        m_Navigation_PointerDelta = m_Navigation.FindAction("Pointer Delta", throwIfNotFound: true);
        m_Navigation_OrbitShift = m_Navigation.FindAction("Orbit Shift", throwIfNotFound: true);
        // Planning
        m_Planning = asset.FindActionMap("Planning", throwIfNotFound: true);
        m_Planning_Position = m_Planning.FindAction("Position", throwIfNotFound: true);
        m_Planning_Place = m_Planning.FindAction("Place", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // Navigation
    private readonly InputActionMap m_Navigation;
    private INavigationActions m_NavigationActionsCallbackInterface;
    private readonly InputAction m_Navigation_Movement;
    private readonly InputAction m_Navigation_PointerLocation;
    private readonly InputAction m_Navigation_PointerDelta;
    private readonly InputAction m_Navigation_OrbitShift;
    public struct NavigationActions
    {
        private @BaseControls m_Wrapper;
        public NavigationActions(@BaseControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Movement => m_Wrapper.m_Navigation_Movement;
        public InputAction @PointerLocation => m_Wrapper.m_Navigation_PointerLocation;
        public InputAction @PointerDelta => m_Wrapper.m_Navigation_PointerDelta;
        public InputAction @OrbitShift => m_Wrapper.m_Navigation_OrbitShift;
        public InputActionMap Get() { return m_Wrapper.m_Navigation; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(NavigationActions set) { return set.Get(); }
        public void SetCallbacks(INavigationActions instance)
        {
            if (m_Wrapper.m_NavigationActionsCallbackInterface != null)
            {
                @Movement.started -= m_Wrapper.m_NavigationActionsCallbackInterface.OnMovement;
                @Movement.performed -= m_Wrapper.m_NavigationActionsCallbackInterface.OnMovement;
                @Movement.canceled -= m_Wrapper.m_NavigationActionsCallbackInterface.OnMovement;
                @PointerLocation.started -= m_Wrapper.m_NavigationActionsCallbackInterface.OnPointerLocation;
                @PointerLocation.performed -= m_Wrapper.m_NavigationActionsCallbackInterface.OnPointerLocation;
                @PointerLocation.canceled -= m_Wrapper.m_NavigationActionsCallbackInterface.OnPointerLocation;
                @PointerDelta.started -= m_Wrapper.m_NavigationActionsCallbackInterface.OnPointerDelta;
                @PointerDelta.performed -= m_Wrapper.m_NavigationActionsCallbackInterface.OnPointerDelta;
                @PointerDelta.canceled -= m_Wrapper.m_NavigationActionsCallbackInterface.OnPointerDelta;
                @OrbitShift.started -= m_Wrapper.m_NavigationActionsCallbackInterface.OnOrbitShift;
                @OrbitShift.performed -= m_Wrapper.m_NavigationActionsCallbackInterface.OnOrbitShift;
                @OrbitShift.canceled -= m_Wrapper.m_NavigationActionsCallbackInterface.OnOrbitShift;
            }
            m_Wrapper.m_NavigationActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Movement.started += instance.OnMovement;
                @Movement.performed += instance.OnMovement;
                @Movement.canceled += instance.OnMovement;
                @PointerLocation.started += instance.OnPointerLocation;
                @PointerLocation.performed += instance.OnPointerLocation;
                @PointerLocation.canceled += instance.OnPointerLocation;
                @PointerDelta.started += instance.OnPointerDelta;
                @PointerDelta.performed += instance.OnPointerDelta;
                @PointerDelta.canceled += instance.OnPointerDelta;
                @OrbitShift.started += instance.OnOrbitShift;
                @OrbitShift.performed += instance.OnOrbitShift;
                @OrbitShift.canceled += instance.OnOrbitShift;
            }
        }
    }
    public NavigationActions @Navigation => new NavigationActions(this);

    // Planning
    private readonly InputActionMap m_Planning;
    private IPlanningActions m_PlanningActionsCallbackInterface;
    private readonly InputAction m_Planning_Position;
    private readonly InputAction m_Planning_Place;
    public struct PlanningActions
    {
        private @BaseControls m_Wrapper;
        public PlanningActions(@BaseControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Position => m_Wrapper.m_Planning_Position;
        public InputAction @Place => m_Wrapper.m_Planning_Place;
        public InputActionMap Get() { return m_Wrapper.m_Planning; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlanningActions set) { return set.Get(); }
        public void SetCallbacks(IPlanningActions instance)
        {
            if (m_Wrapper.m_PlanningActionsCallbackInterface != null)
            {
                @Position.started -= m_Wrapper.m_PlanningActionsCallbackInterface.OnPosition;
                @Position.performed -= m_Wrapper.m_PlanningActionsCallbackInterface.OnPosition;
                @Position.canceled -= m_Wrapper.m_PlanningActionsCallbackInterface.OnPosition;
                @Place.started -= m_Wrapper.m_PlanningActionsCallbackInterface.OnPlace;
                @Place.performed -= m_Wrapper.m_PlanningActionsCallbackInterface.OnPlace;
                @Place.canceled -= m_Wrapper.m_PlanningActionsCallbackInterface.OnPlace;
            }
            m_Wrapper.m_PlanningActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Position.started += instance.OnPosition;
                @Position.performed += instance.OnPosition;
                @Position.canceled += instance.OnPosition;
                @Place.started += instance.OnPlace;
                @Place.performed += instance.OnPlace;
                @Place.canceled += instance.OnPlace;
            }
        }
    }
    public PlanningActions @Planning => new PlanningActions(this);
    private int m_KeyboardOnlySchemeIndex = -1;
    public InputControlScheme KeyboardOnlyScheme
    {
        get
        {
            if (m_KeyboardOnlySchemeIndex == -1) m_KeyboardOnlySchemeIndex = asset.FindControlSchemeIndex("Keyboard Only");
            return asset.controlSchemes[m_KeyboardOnlySchemeIndex];
        }
    }
    private int m_MouseonlySchemeIndex = -1;
    public InputControlScheme MouseonlyScheme
    {
        get
        {
            if (m_MouseonlySchemeIndex == -1) m_MouseonlySchemeIndex = asset.FindControlSchemeIndex("Mouse only");
            return asset.controlSchemes[m_MouseonlySchemeIndex];
        }
    }
    private int m_KeyboardandMouseSchemeIndex = -1;
    public InputControlScheme KeyboardandMouseScheme
    {
        get
        {
            if (m_KeyboardandMouseSchemeIndex == -1) m_KeyboardandMouseSchemeIndex = asset.FindControlSchemeIndex("Keyboard and Mouse");
            return asset.controlSchemes[m_KeyboardandMouseSchemeIndex];
        }
    }
    private int m_GamepadSchemeIndex = -1;
    public InputControlScheme GamepadScheme
    {
        get
        {
            if (m_GamepadSchemeIndex == -1) m_GamepadSchemeIndex = asset.FindControlSchemeIndex("Gamepad");
            return asset.controlSchemes[m_GamepadSchemeIndex];
        }
    }
    public interface INavigationActions
    {
        void OnMovement(InputAction.CallbackContext context);
        void OnPointerLocation(InputAction.CallbackContext context);
        void OnPointerDelta(InputAction.CallbackContext context);
        void OnOrbitShift(InputAction.CallbackContext context);
    }
    public interface IPlanningActions
    {
        void OnPosition(InputAction.CallbackContext context);
        void OnPlace(InputAction.CallbackContext context);
    }
}
