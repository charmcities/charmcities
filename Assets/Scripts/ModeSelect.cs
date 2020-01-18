using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModeSelect : MonoBehaviour
{
    public GameObject planPanel;
    public Button planButton;
    public Button buildButton;
    public Button viewButton;

    private void Start()
    {
        PlanMode();
    }

    public void PlanMode()
    {
        planPanel.SetActive(true);
        SetActiveButton(planButton);
    }
    public void BuildMode()
    {
        planPanel.SetActive(false);
        SetActiveButton(buildButton);
    }

    public void ViewMode()
    {
        planPanel.SetActive(false);
        SetActiveButton(viewButton);
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
}
