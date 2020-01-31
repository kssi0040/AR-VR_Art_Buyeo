using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VR_Manager : MonoBehaviour
{
    public Camera FirstPersonCamera;
    //20190905 수정사항
    public StagePlay m_StagePlay;    
	//public GameObject checkPopUp;
    public GameObject TextObject;
    public GameObject cardObject;

    private bool bStart = false;
    private bool bEnd = false;    

    private bool bTime = false;
    private float fTime = 0f;

    // Start is called before the first frame update
    void Start()
    {   //20190905 수정사항                   
		//checkPopUp.SetActive(false);
        TextObject.SetActive(false);
        cardObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (true == bEnd)
            return;

        if(true == bStart)
        {
            if(true == bTime)
            {
                fTime += Time.deltaTime;
                if (5f < fTime)
                {
                    cardObject.SetActive(true);
                    bTime = false;
                }
            }
            
            // pc 용....            
            if (Input.GetMouseButtonDown(0))
            {                
                RaycastHit hit;                
                Ray ray = FirstPersonCamera.ScreenPointToRay(Input.mousePosition);

                // 결국 아래 부분이 중요하다...
                bool bCheck = Physics.Raycast(ray, out hit, 30f);
                if (true == bCheck)
                {

                    Debug.Log("check");

                    GameObject gameObj = hit.collider.gameObject;
                    Destroy(gameObj);                    
                    CheckEnd();
                }
            }
            
            /*
            if (Input.touchCount == 1)
            {                
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    RaycastHit hit;
                    Ray ray = FirstPersonCamera.ScreenPointToRay(Input.GetTouch(0).position);

                    // 결국 아래 부분이 중요하다...
                    bool bCheck = Physics.Raycast(ray, out hit, 30f);
                    if (true == bCheck)
                    {                        
                        GameObject gameObj = hit.collider.gameObject;
                        Destroy(gameObj);                        
                        //CheckEnd();
                    }
                }
            }    
            */
        }
    }

    // 끝났는지 확인하는 함수.. 카운트로 정해야 할 듯...
    void CheckEnd()
    {        
        PlayerInfo.Instance.isComplite = true;
        //20190905 수정사항        
		//checkPopUp.SetActive(true);
        TextObject.SetActive(false);
        bEnd = true;
        //20190905 수정사항
        m_StagePlay.forwardDown();
    }


    public void VRModeStart()
    {
        bStart = true;
        bTime = true;
        fTime = 0f;


        TextObject.SetActive(true);
        //20190905 수정사항
        //checkPopUp.SetActive(false);
    }
    //안 씀
    //=============================================== BUTTON ===============================================//
    public void CheckButtonEvent()
    {   //20190905 수정사항
		//checkPopUp.SetActive(false);
        TextObject.SetActive(false);
        m_StagePlay.forwardDown();
    }
    


}
