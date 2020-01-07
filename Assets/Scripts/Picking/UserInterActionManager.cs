using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PublicDefine;

public class UserInterActionManager : MonoBehaviour {

    public Camera ARCamera;
    public GameObject demoObjectParent;
    public GameObject demoObject1;
    public Text posText;
    public Text posText2;

    private bool bMakeDelay = true;
    private AR_MODE eARMode = AR_MODE.NONE;


    private GameObject holdingTarget = null;
    private float fZoomSpeed = 0.5f;


    private bool bHolding = false;
    private bool bCheckClick = false;


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        // make object...
        //TouchToMake();

        // picking...
        // 일단 ray 를 이용하여 완성....
        //eARMode = MainManager.Instance.GetARMode();
        /*
        if(AR_MODE.PICKING == eARMode)
        {
            //TouchToPicking();
            //TouchToPicking2();
        }  
        */
    }

    //=====================================================================================================//
    //============================================= MAKE OBJECT =============================================//
    //=====================================================================================================//

    void TouchToMake()
    {        
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            if (Input.touchCount > 0)
            {
                Vector3 touchPos = Input.GetTouch(0).position;
                touchPos.z = 0.8f;

                Vector3 worldPos = Camera.main.ScreenToWorldPoint(touchPos);
                Debug.Log("worldPos x: " + worldPos.x + " y: " + worldPos.y + " z:" + worldPos.z);
                //worldPos.z = 2f;
                // test...
                PlaceObject(worldPos, Quaternion.Euler(0f, 0f, 0f));
            }
        }
        else
        {            
            if (Input.GetMouseButton(0))
            {
                Vector3 touchPos = Input.mousePosition;
                //touchPos.z = 1f;                
                touchPos.z = 0.8f;
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(touchPos);
                Debug.Log("worldPos x: " + worldPos.x + " y: " + worldPos.y + " z:" + worldPos.z);
                //worldPos.z = 2f;
                // test...
                PlaceObject(worldPos, Quaternion.Euler(0f, 0f, 0f));
            }
        }
    }
    void PlaceObject(Vector3 _pos, Quaternion _rot)
    {
        // 여기서 생성한다....
        // 하지만 2 ~ 3초에 한 번씩....
        if(true == bMakeDelay)
        {
            Instantiate(demoObject1, _pos, _rot, demoObjectParent.transform);
            bMakeDelay = false;
            Invoke("ResetToMake", 1.5f);
        }        
    }
    void ResetToMake()
    {
        bMakeDelay = true;
    }

    //=====================================================================================================//
    //=============================================== PICKING ===============================================//
    //=====================================================================================================//
    void TouchToPicking2()
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            if (Input.touchCount == 1)
            {
                // hmm....
                if(true == bHolding)
                {
                    RaycastHit hit;
                    Ray ray = ARCamera.ScreenPointToRay(Input.GetTouch(0).position);
                    if(Physics.Raycast(ray, out hit, 30f))
                    {
                        //posText.text = "touch x: " + holdingTarget.transform.localPosition.x + " y: " + holdingTarget.transform.localPosition.y + " z: " + holdingTarget.transform.localPosition.z;
                        //posText.text = "touch x: " + holdingTarget.transform.localPosition.x + " y: " + holdingTarget.transform.localPosition.y + " z: " + holdingTarget.transform.localPosition.z;
                        holdingTarget.transform.localPosition = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                        posText.text = "touch x: " + holdingTarget.transform.localPosition.x + " y: " + holdingTarget.transform.localPosition.y + " z: " + holdingTarget.transform.localPosition.z;
                    }
                }
                
                if(Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    RaycastHit hit;
                    Ray ray = ARCamera.ScreenPointToRay(Input.GetTouch(0).position);
                    bool bCheck = Physics.Raycast(ray, out hit, 30f);

                    if (true == bCheck)
                    {
                        holdingTarget = hit.collider.gameObject;
                        bHolding = true;
                    }
                }
                else if(Input.GetTouch(0).phase == TouchPhase.Ended)
                {
                    bHolding = false;
                    holdingTarget = null;
                }
            }
        }
        else
        {
            if (Input.GetMouseButton(0))
            {
                RaycastHit hit;
                Ray ray = ARCamera.ScreenPointToRay(Input.mousePosition);
                bool bCheck = Physics.Raycast(ray, out hit, 30f);                                
                if(true == bCheck)
                {
                    //holdingTarget = hit.collider.gameObject;
                    //Debug.Log("hit x: " + hit.point.x + " y: " + hit.point.y + "z: "+hit.point.z);
                    holdingTarget = hit.collider.gameObject;

                    // 결국 그 만큼 더하거나 해야 한다는 것인데....
                    holdingTarget.transform.localPosition = new Vector3(hit.point.x, hit.point.y, holdingTarget.transform.localPosition.z);
                    Debug.Log("hit x: " + holdingTarget.transform.localPosition.x + "  y: " + holdingTarget.transform.localPosition.y + "  z: " + holdingTarget.transform.localPosition.z);
                    //holdingTarget.transform.localPosition = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                }
            }
        }
    }


    void TouchToPicking()
    {        
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            if(Input.touchCount == 0)
            {
                if (true == bHolding)
                {
                    RaycastHit hit;
                    Ray ray = ARCamera.ScreenPointToRay(Input.GetTouch(0).position);

                    if(Physics.Raycast(ray, out hit, 30f))
                    {
                        holdingTarget = hit.collider.gameObject;                        
                    }
                    

                }
            }
            

            /*
            
            if (Input.touchCount > 0)
            {
                Vector3 screenCenter = ARCamera.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));                
                Vector3 touchPos = Input.GetTouch(0).position;                
                touchPos.z = 10f;                
                Vector3 worldPos = ARCamera.ScreenToWorldPoint(touchPos);
                Vector3 dir = worldPos - this.transform.position;
                dir = dir.normalized;

                RaycastHit hit;
                bool bCheckObject = Physics.Raycast(this.transform.position, dir, out hit);
                if (true == bCheckObject)
                {                               
                    holdingTarget = hit.collider.gameObject;
                    touchPos.z = holdingTarget.transform.localPosition.z;                    
                    worldPos = ARCamera.ScreenToWorldPoint(touchPos);

                    dir = worldPos - this.transform.position;
                    dir = dir.normalized;
                    bCheckObject = Physics.Raycast(this.transform.position, dir, out hit);

                    if (true == bCheckObject)
                    {
                        holdingTarget = hit.collider.gameObject;
                        //float originPosZ = holdingTarget.transform.localPosition.z;
                        //originPosZ = Mathf.Clamp(originPosZ, 0.5f, 3f);

                        posText.text = "touch x: " + holdingTarget.transform.localPosition.x + " y: " + holdingTarget.transform.localPosition.y + " z: " + holdingTarget.transform.localPosition.z;
                        posText2.text = "worldPos x: " + worldPos.x + " y: " + worldPos.y + " z: " + worldPos.z;

                        

                        holdingTarget.transform.localPosition = new Vector3(worldPos.x, worldPos.y, worldPos.z);
                        //holdingTarget.transform.localPosition = new Vector3(worldPos.x, holdingTarget.transform.localPosition.y, worldPos.z);                        
                        //posText.text = "touch x: " + holdingTarget.transform.localPosition.x + " y: " + holdingTarget.transform.localPosition.y + " z: " + holdingTarget.transform.localPosition.z;                      
                    }
                }

                if(Input.touchCount == 2) {
                    // 투 터치 일 경우...
                    if(null != holdingTarget)
                    {
                        Touch touchZero = Input.GetTouch(0);
                        Touch touchOne = Input.GetTouch(1);

                        Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                        Vector2 touchOnePrevPos = touchZero.position - touchZero.deltaPosition;

                        float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                        float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

                        float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

                        // 위에서 길이가 나옴....                        
                        //posText2.text = deltaMagnitudeDiff.ToString();
                        deltaMagnitudeDiff = Mathf.Abs(deltaMagnitudeDiff);

                        //float fOriginScaleX = holdingTarget.transform.localScale.x;
                        //float fOriginScaleY = holdingTarget.transform.localScale.y;
                        //float fOriginScaleZ = holdingTarget.transform.localScale.z;
                        // 일단 -80 ~ -700 까지 찍힘...
                        // 0.2 ~ 0.6 까지 제한...
                        float fChangeScale = Mathf.Clamp((deltaMagnitudeDiff / 1000), 0.2f, 0.6f);                                                            
                        holdingTarget.transform.localScale = new Vector3(fChangeScale, fChangeScale, fChangeScale);                       
                    }
                }
            }
        }
        else
        {
            if (Input.GetMouseButton(0))
            {                                
                Vector3 screenCenter = ARCamera.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
                Vector3 touchPos = Input.mousePosition;                
                touchPos.z = 10f;                
                Vector3 worldPos = ARCamera.ScreenToWorldPoint(touchPos);
                //Debug.Log("world x: " + worldPos.x + " y: " + worldPos.y + " z: "+ worldPos.z);                                
                Vector3 dir = worldPos - this.transform.position;
                dir = dir.normalized;

                RaycastHit hit;                                
                bool bCheckObject = Physics.Raycast(this.transform.position, dir, out hit);                
                if (true == bCheckObject)
                {                    
                    GameObject target = hit.collider.gameObject;
                    //Debug.Log("pos x: " + target.transform.localPosition.x + " y: " + target.transform.localPosition.y + " z: " + target.transform.localPosition.z);
                    touchPos.z = target.transform.localPosition.z;
                    worldPos = ARCamera.ScreenToWorldPoint(touchPos);
                    dir = worldPos - this.transform.position;
                    dir = dir.normalized;                    
                    bCheckObject = Physics.Raycast(this.transform.position, dir, out hit);

                    if (true == bCheckObject)
                    {
                        target = hit.collider.gameObject;
                        //Debug.Log("pos2 x: " + target.transform.localPosition.x + " y: " + target.transform.localPosition.y + " z: " + target.transform.localPosition.z);
                        target.transform.localPosition = new Vector3(worldPos.x, worldPos.y, worldPos.z);
                    }
                } 
            }




        */
        }
    }
}
