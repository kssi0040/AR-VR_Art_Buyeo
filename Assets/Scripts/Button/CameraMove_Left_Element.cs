using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class CameraMove_Left_Element : MonoBehaviour
{
    public CameraMove m_CameraMove;
	// Use this for initialization
	void Start ()
    {
        m_CameraMove = FindObjectOfType<CameraMove>();
        EventTrigger.Entry PointerDown = new EventTrigger.Entry();
        PointerDown.eventID = EventTriggerType.PointerDown;
        PointerDown.callback.AddListener((data) => { OnPointerDown((PointerEventData)data); });
        this.GetComponent<EventTrigger>().triggers.Add(PointerDown);
        EventTrigger.Entry PointerUp = new EventTrigger.Entry();
        PointerUp.eventID = EventTriggerType.PointerUp;
        PointerUp.callback.AddListener((data) => { OnPointerUp((PointerEventData)data); });
        this.GetComponent<EventTrigger>().triggers.Add(PointerUp);
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void OnPointerDown(PointerEventData data)
    {
        m_CameraMove.LeftButtonDown();
    }
    public void OnPointerUp(PointerEventData data)
    {
        m_CameraMove.LeftButtonUp();
    }
}
