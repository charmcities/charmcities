using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModeSelect : MonoBehaviour
{
    BaseControls controls;

    public GameObject planPanel;
    public Button planButton;
    public Button buildButton;
    public Button viewButton;

    enum Mode { Plan, Build, View };
    Mode currentMode;

    private void Start()
    {
        
    }

    void Awake()
    {
        currentMode = Mode.Plan;
        SelectCurrentMode();
        controls = new BaseControls();
        controls.Navigation.ModeShift.performed += ctx => ModeShift(ctx.ReadValue<float>());
    }

    void ModeShift(float direction)
    {
        if (currentMode == Mode.Plan)
        {
            currentMode = direction > 0 ? Mode.Build : Mode.View;
        }
        else if (currentMode == Mode.Build)
        {
            currentMode = direction > 0 ? Mode.View : Mode.Plan;
        }
        else
        {
            currentMode = direction > 0 ? Mode.Plan : Mode.Build;
        }

        SelectCurrentMode();
    }

    void SelectCurrentMode()
    {
        if (currentMode == Mode.Plan)
        {
            PlanMode();
        }
        else if (currentMode == Mode.Build)
        {
            BuildMode();
        }
        else
        {
            ViewMode();
        }
    }

    public void PlanMode()
    {
        planPanel.SetActive(true);
        SetActiveButton(planButton);
        currentMode = Mode.Plan;
    }
    public void BuildMode()
    {
        planPanel.SetActive(false);
        SetActiveButton(buildButton);
        currentMode = Mode.Build;
    }

    public void ViewMode()
    {
        planPanel.SetActive(false);
        SetActiveButton(viewButton);
        currentMode = Mode.View;
    }

    void SetActiveButton(Button activeButton)
    {
        SetButtonColor(planButton, false);
        SetButtonColor(buildButton, false);
        SetButtonColor(viewButton, false);
        SetButtonColor(activeButton, true);
    }

    void SetButtonColor(Button button, bool active)
    {
        Color activeColor = Color.white;
        Color inactiveColor = new Color32(138, 151, 176, 255);
        ColorBlock colors = button.colors;
        colors.normalColor = active ? activeColor : inactiveColor;
        colors.selectedColor = colors.normalColor;
        button.colors = colors;
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
