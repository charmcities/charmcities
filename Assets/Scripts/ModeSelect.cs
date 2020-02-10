using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModeSelect : MonoBehaviour
{
    BaseControls controls;

    public GameObject[] planItems;
    public GameObject[] buildItems;
    public GameObject[] viewItems;

    public Button planButton;
    public Button buildButton;
    public Button viewButton;

    enum Mode { Plan, Build, View };
    Mode currentMode;

    void Awake()
    {
        currentMode = Mode.Plan;
        SelectCurrentMode();

        // Sets up input system.
        controls = new BaseControls();
        controls.Navigation.ModeShift.performed += ctx => ModeShift(ctx.ReadValue<float>());
    }

    // ModeShift takes user input to move either left or right (wrapping around) from one mode to the next.
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
            planButton.onClick.Invoke();
        }
        else if (currentMode == Mode.Build)
        {
            buildButton.onClick.Invoke();
        }
        else
        {
            viewButton.onClick.Invoke();
        }
    }

    private void SetItemsActive(GameObject[] itemList, bool status)
    {
        foreach (GameObject item in itemList)
        {
            item.SetActive(status);
        }

    }

    public void PlanMode()
    {
        SetItemsActive(planItems, true);
        SetItemsActive(buildItems, false);
        SetItemsActive(viewItems, false);
        SetActiveButton(planButton);
        MuteButtons(buildButton, viewButton);
        currentMode = Mode.Plan;
    }
    public void BuildMode()
    {
        SetItemsActive(planItems, false);
        SetItemsActive(buildItems, true);
        SetItemsActive(viewItems, false);
        SetActiveButton(buildButton);
        MuteButtons(planButton, viewButton);
        currentMode = Mode.Build;
    }

    public void ViewMode()
    {
        SetItemsActive(planItems, false);
        SetItemsActive(buildItems, false);
        SetItemsActive(viewItems, true);
        SetActiveButton(viewButton);
        MuteButtons(buildButton, planButton);
        currentMode = Mode.View;
    }

    void MuteButtons(Button button1, Button button2)
    {
        button1.GetComponent<ButtonSound>().StopSound();
        button2.GetComponent<ButtonSound>().StopSound();
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
