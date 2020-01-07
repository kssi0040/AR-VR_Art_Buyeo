using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchControl : MonoBehaviour
{
    public Camera uiCamera;
    private bool bHold = false;
    private Vector3 currentPos = new Vector3();
    private Vector3 priviousPos = new Vector3();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            bHold = true;           
            priviousPos.x = Input.mousePosition.x;
            priviousPos.y = Input.mousePosition.y;
            priviousPos.z = 0f;
        }
        if (Input.GetMouseButtonUp(0))
        {
            bHold = false;
        }

        if (bHold == true)
        {
            // horizon 축...
            currentPos = Input.mousePosition;            
            float horizonRate = priviousPos.x - currentPos.x;
            float tmpGap = horizonRate / 25;            
            float yAngle = uiCamera.transform.rotation.eulerAngles.y;
            float horizonPos = yAngle + tmpGap;
            
            // vertical 축
            float verticalRate = currentPos.y - priviousPos.y;            
            float tmpGap2 = verticalRate / 40;
            float verticalPos = uiCamera.transform.rotation.eulerAngles.x;
            float verticalPos2 = verticalPos + tmpGap2;

            float VerticalRot = 0f;
            //Debug.Log("haha: " + verticalPos2);
            if (-10f < verticalPos2 && 60f > verticalPos2)
            {
                VerticalRot = Mathf.Clamp(verticalPos2, -10f, 45.0f);
            }
            else if (60f < verticalPos2 && 365f > verticalPos2)
            {
                VerticalRot = Mathf.Clamp(verticalPos2, 320f, 365f);
            }
            //uiCamera.transform.rotation = Quaternion.Euler(verticalPos2, horizonPos, 0f);
            uiCamera.transform.rotation = Quaternion.Euler(VerticalRot, horizonPos, 0f);

            priviousPos = currentPos;
        }
    }
}
