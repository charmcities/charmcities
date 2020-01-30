using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class ButtonTooltip : MonoBehaviour
{
    TextMeshProUGUI tooltipTextbox;
    RectTransform background;

    BaseControls controls;
    Vector2 mousePos;
    [SerializeField]
    private Camera uiCamera;

    readonly float textPadding = 4f;
    float cursorXOffset = 10f;
    float cursorYOffset = -25f;

    private void Awake()
    {
        background = transform.Find("Background").GetComponent<RectTransform>();
        tooltipTextbox = transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
        
        Hide();

        // Unity New Input System actions
        controls = new BaseControls();
        // Track mouse position
        controls.Navigation.PointerLocation.performed += ctx => mousePos = ctx.ReadValue<Vector2>();
    }

    private void Update()
    {
        // Keep tooltip fully contained within screen bounds.
        if (mousePos.x + cursorXOffset < 0)
        {
            mousePos.x = -cursorXOffset;
        }
        else if (mousePos.x + cursorXOffset + background.sizeDelta.x > Screen.width)
        {
            mousePos.x = Screen.width - (cursorXOffset + background.sizeDelta.x);
        }
        if (mousePos.y + cursorYOffset - background.sizeDelta.y < 0)
        {
            mousePos.y = -cursorYOffset + background.sizeDelta.y;
        }
        else if (mousePos.y + cursorYOffset > Screen.height)
        {
            mousePos.y = Screen.height - cursorYOffset;
        }

        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponent<RectTransform>(), mousePos, uiCamera, out localPos);
        localPos.x += cursorXOffset;
        localPos.y += cursorYOffset;

        transform.localPosition = localPos;
    }

    public void Show(string tooltipString)
    {
        gameObject.SetActive(true);

        tooltipTextbox.SetText(tooltipString);

        Vector2 backgroundSize = new Vector2(tooltipTextbox.preferredWidth + (textPadding * 2), tooltipTextbox.preferredHeight + (textPadding * 2));
        background.sizeDelta = backgroundSize;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        if (controls != null)
        {
            controls.Disable();
        }
    }
}
