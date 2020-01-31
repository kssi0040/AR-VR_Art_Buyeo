using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScrollRectSnap : MonoBehaviour
{
    public RectTransform panel;                             // to hold the scroll panel;
    public Button[] aButtons;

    public GameObject Sphere;
    public Texture[] Textures;
    public string[] Names;
    public new Text name;
    public new Camera camera;
    public int Temp;

    public RectTransform center;                           // center to compare the distance for each button

    private float[] aDistances;                               // all button's distance to the center..
    private bool bDragging = false;                          // will be true,  while we drag the panel
    private int iBtnDistance;                                   // will hold the distance between the buttons
    private int iMinButtonNum;                               // to hold the number of the button, with smallest distance to center


    public GameObject NoticePopup;


    // Start is called before the first frame update
    void Start()
    {
        int iBtnLength = aButtons.Length;
        aDistances = new float[iBtnLength];
        // get distance between buttons
        iBtnDistance = (int)Mathf.Abs(aButtons[1].GetComponent<RectTransform>().anchoredPosition.x - aButtons[0].GetComponent<RectTransform>().anchoredPosition.x);        
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < aButtons.Length; ++i)
        {
            aDistances[i] = Mathf.Abs(center.transform.position.x - aButtons[i].transform.position.x);            
        }

        float minDistance = Mathf.Min(aDistances);                              // Get the min Distance        
        for (int a = 0; a < aButtons.Length; ++a)
        {
            if(minDistance == aDistances[a])
            {
                /*
                Temp = iMinButtonNum;
                iMinButtonNum = a;   
                if(Temp!=iMinButtonNum)
                {
                    if(camera!=null)
                    {
                        camera.transform.rotation = Quaternion.Euler(Vector3.zero);
                    }
                }
                */
                if (iMinButtonNum < Textures.Length)
                {
                    Sphere.GetComponent<Renderer>().material.mainTexture = Textures[iMinButtonNum];
                }
                if (iMinButtonNum < Names.Length)
                    name.text = Names[iMinButtonNum];
            }
        }

        if(!bDragging)
        {
            LerpToButton(iMinButtonNum * -iBtnDistance);
        }
    }


    void LerpToButton(int position)
    {
        float fPosX = Mathf.Lerp(panel.anchoredPosition.x, position, Time.deltaTime * 10f);
        Vector2 newPosition = new Vector2(fPosX, panel.anchoredPosition.y);
        panel.anchoredPosition = newPosition;
    }

    //================================================ EVENT TRIGGER ==================================================//
    // Event Trigger 에서 call...
    public void StartDrag()
    {
        bDragging = true;
    }

    public void EndDrag()
    {
        bDragging = false;
    }


    public void LeftButtonDown()
    {
        iMinButtonNum--;
        if (iMinButtonNum < 0)
        {
            iMinButtonNum = 0;
        }
    }

    public void RightButtonDown()
    {
        iMinButtonNum++;
        if (iMinButtonNum >= Names.Length - 1)
        {
            iMinButtonNum = Names.Length - 1;
        }
    }

    public void NoticeButtonEvent()
    {
        NoticePopup.SetActive(false);
    }

    // stage button....
    public void StageButtonEvent()
    {
        if (0 == iMinButtonNum)
        {
            // stage 1 load...
            if (Quiz_XML_Reader.Instance.readCompleted == true && XML_Reader.Instance.readCompleted == true)
            {
                SceneManager.LoadScene("Stage1");
            }
        }
        else if (1 == iMinButtonNum)
        {
            // stage 1 load...
            if (Quiz_XML_Reader.Instance.readCompleted == true && XML_Reader.Instance.readCompleted == true)
            {
                SceneManager.LoadScene("Stage2");
            }
        }
        else if (2 == iMinButtonNum)
        {
            // stage 1 load...
            if (Quiz_XML_Reader.Instance.readCompleted == true && XML_Reader.Instance.readCompleted == true)
            {
                SceneManager.LoadScene("GPS_Scene");
            }
        }
        else if (3 == iMinButtonNum)
        {
            // stage 1 load...
            if (Quiz_XML_Reader.Instance.readCompleted == true && XML_Reader.Instance.readCompleted == true)
            {
                SceneManager.LoadScene("Record");
            }
        }
    }

    public void HomeButtonEvent()
    {
        SceneManager.LoadScene("Start");
    }

    public void MapButtonEvent()
    {
        if (0 == iMinButtonNum)
        {
            //36.895005, 126.206617
            string strUrl = "https://www.google.co.kr/maps/place/%EC%A0%95%EB%A6%BC%EC%82%AC%EC%A7%80+%EC%98%A4%EC%B8%B5%EC%84%9D%ED%83%91/@36.2793082,126.9111489,17z/data=!3m1!4b1!4m5!3m4!1s0x357a9fd5024a7c53:0x6df12603cbc183f7!8m2!3d36.2793039!4d126.9133376?hl=ko";
            Application.OpenURL(strUrl);
        }
        else if (1 == iMinButtonNum)
        {            
            string strUrl = "https://www.google.co.kr/maps/place/%EB%8A%A5%EC%82%B0%EB%A6%AC%EA%B3%A0%EB%B6%84%EA%B5%B0/@36.2784444,126.9418341,17z/data=!3m1!4b1!4m5!3m4!1s0x357a9ff1dab2fcbb:0xd238ff4752d666dc!8m2!3d36.2784401!4d126.9440228?hl=ko";
            Application.OpenURL(strUrl);
        }
        else if (2 == iMinButtonNum)
        {
            //36.845262, 126.196726            
            //string strUrl = "https://www.google.com/maps/place/36.835972,126.195911";
            //Application.OpenURL(strUrl);
        }
    }
}
