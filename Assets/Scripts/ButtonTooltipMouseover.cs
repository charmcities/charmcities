using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonTooltipMouseover : MonoBehaviour
{
    public ButtonTooltip tooltipUI;
    [TextArea(3,10)]
    public string text;

    private EventTrigger trigger { get { return GetComponent<EventTrigger>(); } }

    private void Awake()
    {
        gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener((data) => { OnPointerEnter((PointerEventData)data); });
        trigger.triggers.Add(entry);

        EventTrigger.Entry exit = new EventTrigger.Entry();
        exit.eventID = EventTriggerType.PointerExit;
        exit.callback.AddListener((data) => { OnPointerExit((PointerEventData)data); });
        trigger.triggers.Add(exit);
    }

    public void OnPointerEnter(PointerEventData data)
    {
        tooltipUI.Show(text);
    }

    public void OnPointerExit(PointerEventData data)
    {
        tooltipUI.Hide();
    }
}
