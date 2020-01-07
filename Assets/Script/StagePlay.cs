using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine.EventSystems;

public class StagePlay : MonoBehaviour
{
    public int Prev;
    public int Index;
    public int Next;
    public SceneLoader sceneLoader;
    public Image BackGroundImg;
    public AudioSource BGM;
    public AudioSource Effect;
    public AudioSource Narration;
    public int QuizCount = 0;
    public bool QuizClear = false;
    public float Timer = 0.0f;    
    public int Minute = 0;
    public int GameCount = 0;
    Text text;
    public Text TimeText;
    public QuizManager QuizManager;
    public VR_Manager m_VR_Manager;
    public GameObject ARCore;
    public GameObject Light;
    public GameObject PlaneGenerator;
    public GameObject PointCloud;
    public GameObject PlaneDiscovery;
    public GameObject ARController;
    public GameObject Guide;
    public Camera m_Camera;
    public VideoPlayer player;
    public Dictionary<int, GameObject> MiniGameObj3DDict=new Dictionary<int, GameObject>();
    private const float SWIPE_THREHOLD = 500;
    private bool tap,swipeLeft,swipeRight,swipeUp,swipeDown;
    private bool isDragging = false;
    private Vector2 startTouch, swipeDelta;
    public int SwipeCount = 0;
    public bool BlurClear = false;

    //20191122 추가사항
    public Scrollbar m_Scrollbar;

    // Start is called before the first frame update
    void Start()
    {        
        QuizManager = FindObjectOfType<QuizManager>();
        Prev = XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[0].Prev;
        Index = XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[0].Index;
        Next = XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[0].Next;

        BackGroundImg = GameObject.Find("BackGround").GetComponent<Image>();        
        BGM = GameObject.FindGameObjectWithTag("BGM").GetComponent<AudioSource>();
        Effect = GameObject.FindGameObjectWithTag("Effect").GetComponent<AudioSource>();
        Narration = GameObject.FindGameObjectWithTag("Narration").GetComponent<AudioSource>();

        player = FindObjectOfType<VideoPlayer>();
        StageSet();
    }

    // Update is called once per frame
    void Update()
    {
        //현재 이벤트가 퀴즈일 경우
        if (XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventType == "Quiz")
        {
            //퀴즈 중 시간을 기록하는 퀴즈일 경우
            if (XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].QuizInfoList.Type == "QuizTimeRec")
            {
                if (QuizClear == false)
                {
                    Timer += Time.deltaTime;

                    if(Timer>=60)
                    {
                        Minute++;
                        Timer -= 60;
                    }

                    TimeText.text = string.Format("{0:D1} : {1:D2}", Minute, (int)Timer);
                }
            }
            //퀴즈 중 타임어택 일 경우
            else if(XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].QuizInfoList.Type== "QuizTimeAtk")
            {
                if(QuizClear==false)
                {                    
                    Timer -= Time.deltaTime;
                    if (Timer <= 0)
                    {
                        Minute--;
                        if (Minute <= 0)
                        {
                            Minute = 0;
                            Index = XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].Prev;
                            Prev = XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].Prev;
                            Next = XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].Next;
                            QuizCount = 0;
                            StageSet();
                            PlayerInfo.Instance.isComplite = false;
                        }                        
                        else
                        {
                            Timer += 60;
                        }
                    }

                    TimeText.text = string.Format("{0:D1} : {1:D2}", Minute, (int)Timer);                    
                }
            }
        }

        //터치게임 중 일 경우
        if(XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventType== "TouchGame")
        {
            //터치 인식하고 지울꺼 지우자..
            if(Input.touchCount>0)
            {
                Touch touch = Input.GetTouch(0);

                if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                {
                    return;
                }

                ObjectTouch();
            }

            if(GameCount<=0)
            {
                forwardDown();
            }
        }

        if (XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventType == "DeleteBlur")
        {
            tap = swipeLeft = swipeRight = swipeUp = swipeDown = false;
            #region Standalone Inputs
            if (Input.GetMouseButtonDown(0))
            {
                tap = true;
                isDragging = true;
                startTouch = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
            }
            #endregion

            #region Mobile Inputs
            if (Input.touches.Length != 0)
            {
                if (Input.touches[0].phase == TouchPhase.Began)
                {
                    tap = true;
                    isDragging = true;
                    startTouch = Input.touches[0].position;
                }
                else if (Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled)
                {
                    isDragging = false;
                }
            }
            #endregion

            swipeDelta = Vector2.zero;
            if (isDragging)
            {
                if(Input.touches.Length!=0)
                {
                    swipeDelta = Input.touches[0].position - startTouch;
                }
                else if(Input.GetMouseButton(0))
                {
                    swipeDelta = (Vector2)Input.mousePosition - startTouch;
                }
            }

            if(swipeDelta.magnitude>SWIPE_THREHOLD)
            {
                float x = swipeDelta.x;
                float y = swipeDelta.y;

                if(Mathf.Abs(x)>Mathf.Abs(y))
                {
                    if (x < 0)
                    {
                        swipeLeft = true;
                        SwipeCount++;
                    }
                    else
                    {
                        swipeRight = true;
                        SwipeCount++;
                    }
                }
                else
                {
                    if (y > 0)
                    {
                        swipeDown = true;
                        SwipeCount++;
                    }
                    else
                    {
                        swipeUp = true;
                        SwipeCount++;
                    }
                }
            }
            if(swipeLeft==true||swipeRight==true||swipeUp==true||swipeDown==true)
            {
                EraseBlur();
            }            
        }
    }

    public void StageSet()
    {
        if(Next>-1)
        {
            m_Camera.gameObject.SetActive(true);

            m_Camera.transform.rotation = Quaternion.Euler(Vector3.zero);
            //AR 관련 오브젝트 시작
            if (ARCore != null)
            {
                ARCore.SetActive(false);
            }

            if(ARController!=null)
            {
                ARController.SetActive(false);
            }

            if (Light != null)
            {
                Light.SetActive(false);
            }

            if (PlaneGenerator != null)
            {
                PlaneGenerator.SetActive(false);
            }

            if (PointCloud != null)
            {
                PointCloud.SetActive(false);
            }

            if (PlaneDiscovery != null)
            {
                PlaneDiscovery.SetActive(false);
            }
           
            //여기까지            
            
            //BackGroundImg.sprite = sceneLoader.myLoadedAssetBundle.LoadAsset<Sprite>(XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].BG.image);
            if (XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].BG.image == null)
            {
                BackGroundImg.transform.gameObject.SetActive(false);
            }
            else
            {
                BackGroundImg.sprite = Resources.Load<Sprite>(XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].BG.image);
                BackGroundImg.transform.gameObject.SetActive(true);
            }

            if (XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].BG.BGM.OnOff==1)
            {
                if(XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].BG.BGM.Src!="null")
                {
                    BGM.clip = sceneLoader.myLoadedAssetBundle.LoadAsset<AudioClip>(XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].BG.BGM.Src);
                    BGM.time = 0.0f;
                    BGM.Play();
                }
            }

            if (XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].BG.effect.OnOff == 1)
            {
                if (XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].BG.effect.Src != "null")
                {
                    Effect.clip = sceneLoader.myLoadedAssetBundle.LoadAsset<AudioClip>(XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].BG.effect.Src);
                    Effect.time = 0.0f;
                    Effect.Play();
                }
            }

            if (XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].BG.narration.OnOff == 1)
            {
                if (PlayerInfo.Instance.Gender == 0)
                {
                    if (XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].BG.narration.Src1 != "null")
                    {
                        //Narration.clip = sceneLoader.myLoadedAssetBundle.LoadAsset<AudioClip>(XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].BG.narration.Src);
                        Narration.clip = Resources.Load<AudioClip>(XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].BG.narration.Src1);
                        Narration.time = 0.0f;
                        Narration.Play();
                    }
                }
                else
                {
                    if (XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].BG.narration.Src2 != "null")
                    {
                        //Narration.clip = sceneLoader.myLoadedAssetBundle.LoadAsset<AudioClip>(XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].BG.narration.Src);
                        Narration.clip = Resources.Load<AudioClip>(XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].BG.narration.Src2);
                        Narration.time = 0.0f;
                        Narration.Play();
                    }
                }
            }

            switch (XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventType)
            {
                case "Text":
                    Debug.Log("111");
                    TextEvent();
                    break;
                case "PopUp":
                    Debug.Log("222");
                    PopUpEvent();
                    break;
                case "SphereImageOn":
                    Debug.Log("333");
                    SphereImageOnEvent();
                    break;
                case "SpeechBubbleOn":
                    Debug.Log("444");
                    SpeechBubbleOnEvent();
                    break;
                case "Quarter":
                    Debug.Log("555");
                    QuarterEvent();
                    break;
                case "LoadScene":
                    Debug.Log("666");
                    LoadSceneEvent();
                    break;
                case "Quiz":
                    Debug.Log("777");
                    QuizEvent();
                    break;
                case "TakeCapture":
                    Debug.Log("888");
                    TakeCaptureEvent();
                    break;
                case "PanelPop":
                    Debug.Log("999");
                    PanelPopEvent();
                    break;
                case "TableSetting":
                    Debug.Log("10");
                    TableSettingEvent();
                    break;
                case "SelectItem":
                    Debug.Log("11");
                    SelectItemEvent();
                    break;
                case "QuarterSelect":
                    Debug.Log("12");
                    QuarterSelectEvent();
                    break;
                case "CardPopUp":
                    Debug.Log("13");
                    CardPopUpEvent();
                    break;
                case "InputField":
                    Debug.Log("14");
                    InputFieldEvent();
                    break;
                case "VideoPlay":
                    Debug.Log("15");
                    VideoPlayEvent();
                    break;
                case "TouchGame":
                    Debug.Log("16");
                    TouchGameEvent();
                    break;
                case "TakeShot":
                    Debug.Log("17");
                    ShotEvent();
                    break;
                case "DeleteBlur":
                    Debug.Log("18");
                    DeleteBlurEvent();
                    break;
                case "Paint":
                    Debug.Log("19");
                    PaintEvent();
                    break;
                case "FindGame":
                    Debug.Log("20");
                    FindGameEvent();
                    break;
                default:
                    Debug.Log("000");
                    break;
            }
        }
    }

    public void forwardDown()
    {
        int temp = -1;
        if (PlayerInfo.Instance.isComplite)
        {
            if (Narration.isPlaying == false)
            {
                Camera.main.transform.rotation = Quaternion.Euler(Vector3.zero);
                //분기 체크
                if (XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventType == "Quarter")
                {
                    //branchList[Group] 개수만큼 돌면서 모두 true 일 경우 Node 값으로 index 변경
                    for (int k = 0; k < XML_Reader.Instance.scenarioToDict.BranchDictionary[sceneLoader.currentStage][XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].indexList.Group].Count; k++)
                    {

                        if (XML_Reader.Instance.scenarioToDict.BranchDictionary[sceneLoader.currentStage][XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].indexList.Group][k] == false)
                        {
                            temp = Next;
                            break;
                        }
                        else
                        {
                            temp = XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].Node;
                        }
                    }
                    Index = temp;
                    Prev = XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].Prev;
                    Next = XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].Next;
                }
                //퀴즈일때 체크 사항
                else if (XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventType == "Quiz")
                {
                    //시간 기록하는 퀴즈일 경우
                    if (XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].QuizInfoList.Type == "QuizTimeRec")
                    {
                        Debug.Log("QuizCount : " + QuizCount);
                        //if (QuizCount >= Quiz_XML_Reader.Instance.quizToDict.QuizTimeRecDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].ID].Count)
                        //다 풀었을때만 다음 이벤트로 넘어가는 스크립트
                        if (QuizClear == true)
                        {
                            Index = Next;
                            Prev = XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].Prev;
                            Next = XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].Next;
                            QuizCount = 0;
                            QuizClear = false;
                            Timer = 0.0f;
                            Minute = 0;
                        }
                        //else
                        //{
                        //    StageSet();
                        //    PlayerInfo.Instance.isComplite = false;                        
                        //}
                    }
                    //타임 어택 퀴즈일 경우
                    else if (XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].QuizInfoList.Type == "QuizTimeAtk")
                    {
                        Debug.Log("QuizCount : " + QuizCount);
                        //다 풀었을때만 다음 이벤트로 넘어가는 스크립트
                        if (QuizClear == true)
                        {
                            Index = Quiz_XML_Reader.Instance.quizToDict.QuizTimeAtkDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].ID].node.Clear;
                            Prev = XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].Prev;
                            Next = XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].Next;
                            QuizCount = 0;
                            QuizClear = false;
                            Timer = 0.0f;
                            Minute = 0;
                        }
                        //else
                        //{
                        //    StageSet();
                        //    PlayerInfo.Instance.isComplite = false;
                        //}
                    }
                    //그 외
                    else
                    {
                        Index = Next;
                        Prev = XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].Prev;
                        Next = XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].Next;
                    }
                }
                //입력 이벤트일때 체크 사항
                else if (XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventType == "InputField")
                {
                    for (int k = 0; k < XML_Reader.Instance.scenarioToDict.StageInfoDictionary[sceneLoader.currentStage].objectList.ObjType2D.Length; k++)
                    {
                        for (int j = 0; j < sceneLoader.object2DDict[k].Count; j++)
                        {
                            if (sceneLoader.object2DDict[k][j].tag == "InputFieldText")
                            {
                                Debug.Log(sceneLoader.object2DDict[k][j].transform.GetChild(0).GetChild(1).name);
                                if (sceneLoader.object2DDict[k][j].transform.GetChild(0).GetChild(1).GetComponent<Text>().text == "")
                                {
                                    return;
                                }
                                else
                                {
                                    Index = Next;
                                    Prev = XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].Prev;
                                    Next = XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].Next;
                                }
                            }
                        }
                    }
                }
                //블러 지우기 이벤트일때 체크 사항
                else if (XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventType == "DeleteBlur")
                {
                    if (BlurClear == true)
                    {
                        Index = Next;
                        Prev = XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].Prev;
                        Next = XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].Next;
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    Index = Next;
                    Prev = XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].Prev;
                    Next = XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].Next;
                }
                if (XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventType != "VideoPlay")
                {
                    if (player != null)
                    {
                        player.time = 0.0f;
                        player.Stop();
                    }
                }
                StageSet();
                if (XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventType != "TakeCapture" || XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventType != "TableSetting" || XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventType != "FindGame")
                {
                    PlayerInfo.Instance.isComplite = false;
                }
            }
            else
            {
                return;
            }
        }
        else
        {
            PlayerInfo.Instance.isClicked = true;
        }
    }

    public void TextEvent()
    {
        for(int k=0;k<sceneLoader.CanvasList.Count;k++)
        {
            for(int j=0;j<XML_Reader.Instance.scenarioToDict.StageInfoDictionary[sceneLoader.currentStage].CanvasCount;j++)
            {
                if(k==j)
                {
                    sceneLoader.CanvasList[k].transform.gameObject.SetActive(true);
                    break;
                }
                sceneLoader.CanvasList[k].transform.gameObject.SetActive(false);
            }
        }
        
        if (XML_Reader.Instance.scenarioToDict.CreateCharType2DDictionary[sceneLoader.currentStage] != null)
        {
            for (int k = 0; k < XML_Reader.Instance.scenarioToDict.CreateCharType2DDictionary[sceneLoader.currentStage].Count; k++)
            {
                for (int j = 0; j < sceneLoader.character2DDict[k].Count; j++)
                {
                    sceneLoader.character2DDict[k][j].SetActive(false);
                }
            }
            if (XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D != null)
            {
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D.Length; k++)
                {
                    for (int j = 0; j < XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList.Length; j++)
                    {
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().anchorMin = XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].anchorMin.GetAnchorMin();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().anchorMax = XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].anchorMax.GetAnchorMax();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().pivot = XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].pivot.GetPivot();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().offsetMin = XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].offsetMin.GetOffsetMin();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().offsetMax = XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].offsetMax.GetOffsetMax();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().anchoredPosition3D = new Vector3(sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().anchoredPosition3D.x, sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().anchoredPosition3D.y, 0.0f);
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().localRotation = XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].Rotation.GetRot();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().localScale = XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].scale.GetScale();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].SetActive(true);
                        if (sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<Animator>() != null)
                        {
                            sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<Animator>().SetTrigger(XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].Anim);
                        }
                    }
                }
            }
        }        
        if (XML_Reader.Instance.scenarioToDict.CreateChar3DDictionary[sceneLoader.currentStage] != null)
        {
            for(int k=0;k<sceneLoader.character3DDict.Count;k++)
            {
                sceneLoader.character3DDict[k].SetActive(false);
            }
            if (XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList != null)
            {
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList.Length; k++)
                {
                    sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].transform.position = XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].Position.GetPos();
                    sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].transform.localRotation = XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].Rotation.GetRot();
                    sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].transform.localScale = XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].scale.GetScale();
                    sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].SetActive(true);
                    if (sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].GetComponent<Animator>() != null)
                    {
                        sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].GetComponent<Animator>().SetTrigger(XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].Anim);
                    }
                }
            }
        }
        if (XML_Reader.Instance.scenarioToDict.CreateObjType2DDictionary[sceneLoader.currentStage] != null)
        {
            for (int k=0;k<XML_Reader.Instance.scenarioToDict.CreateObjType2DDictionary[sceneLoader.currentStage].Count;k++)
            {
                for(int j=0;j<sceneLoader.object2DDict[k].Count;j++)
                {
                    sceneLoader.object2DDict[k][j].SetActive(false);
                }
            }
            if (XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D != null)
            {
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D.Length; k++)
                {
                    for (int j = 0; j < XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList.Length; j++)
                    {
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().anchorMin = XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].anchorMin.GetAnchorMin();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().anchorMax = XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].anchorMax.GetAnchorMax();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().pivot = XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].pivot.GetPivot();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().offsetMin = XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].offsetMin.GetOffsetMin();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().offsetMax = XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].offsetMax.GetOffsetMax();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().anchoredPosition3D = new Vector3(sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().anchoredPosition3D.x, sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().anchoredPosition3D.y, 0.0f);                        
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().localRotation = XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].Rotation.GetRot();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().localScale = XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].scale.GetScale();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].SetActive(true);
                        if (sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<Animator>() != null)
                        {
                            sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<Animator>().SetTrigger(XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].Anim);
                        }
                    }
                }
            }
        }
        if (XML_Reader.Instance.scenarioToDict.CreateObj3DDictionary[sceneLoader.currentStage]!=null)
        {
            for (int k = 0; k < sceneLoader.object3DDict.Count; k++)
            {
                sceneLoader.object3DDict[k].SetActive(false);
            }
            if (XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList != null)
            {
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList.Length; k++)
                {
                    sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].transform.position = XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].Position.GetPos();
                    sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].transform.localRotation = XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].Rotation.GetRot();
                    sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].transform.localScale = XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].scale.GetScale();
                    sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].SetActive(true);
                    if (sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].GetComponent<Animator>() != null)
                    {
                        sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].GetComponent<Animator>().SetTrigger(XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].Anim);
                    }
                }
            }
        }

        if (PlayerInfo.Instance.Gender == 0)
        {
            Debug.Log("check 111");
            if (XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D != null)
            {
                Debug.Log("check 222");
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D.Length; k++)
                {
                    for (int j = 0; j < sceneLoader.character2DDict[k].Count; j++)
                    {
                        Debug.Log("check 333");
                        if (sceneLoader.character2DDict[k][j].name == "CH_02")
                        {
                            Debug.Log("check 444");
                            sceneLoader.character2DDict[k][j].SetActive(false);
                        }
                    }
                }
            }
        }
        else if (PlayerInfo.Instance.Gender == 1)
        {
            if (XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D != null)
            {
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D.Length; k++)
                {
                    for (int j = 0; j < sceneLoader.character2DDict[k].Count; j++)
                    {
                        if (sceneLoader.character2DDict[k][j].name == "CH_01")
                        {
                            sceneLoader.character2DDict[k][j].SetActive(false);
                        }
                    }
                }
            }
        }
        TextOutPut();
    }

    public void PopUpEvent()
    {
        for (int k = 0; k < sceneLoader.CanvasList.Count; k++)
        {
            for (int j = 0; j < XML_Reader.Instance.scenarioToDict.StageInfoDictionary[sceneLoader.currentStage].CanvasCount; j++)
            {
                if (k == j)
                {
                    sceneLoader.CanvasList[k].transform.gameObject.SetActive(true);
                    break;
                }
                sceneLoader.CanvasList[k].transform.gameObject.SetActive(false);
            }
        }

        if (XML_Reader.Instance.scenarioToDict.CreateCharType2DDictionary[sceneLoader.currentStage] != null)
        {
            for (int k = 0; k < XML_Reader.Instance.scenarioToDict.CreateCharType2DDictionary[sceneLoader.currentStage].Count; k++)
            {
                for (int j = 0; j < sceneLoader.character2DDict[k].Count; j++)
                {
                    sceneLoader.character2DDict[k][j].SetActive(false);
                }
            }
            if(XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D!=null)
            {
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D.Length; k++)
                {
                    for (int j = 0; j < XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList.Length; j++)
                    {
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().anchorMin = XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].anchorMin.GetAnchorMin();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().anchorMax = XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].anchorMax.GetAnchorMax();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().pivot = XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].pivot.GetPivot();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().offsetMin = XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].offsetMin.GetOffsetMin();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().offsetMax = XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].offsetMax.GetOffsetMax();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().anchoredPosition3D = new Vector3(sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().anchoredPosition3D.x, sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().anchoredPosition3D.y, 0.0f);
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().localRotation = XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].Rotation.GetRot();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().localScale = XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].scale.GetScale();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].SetActive(true);
                        if (sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<Animator>() != null)
                        {
                            sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<Animator>().SetTrigger(XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].Anim);
                        }
                    }
                }
            }
        }
        if (XML_Reader.Instance.scenarioToDict.CreateChar3DDictionary[sceneLoader.currentStage] != null)
        {
            for (int k = 0; k < sceneLoader.character3DDict.Count; k++)
            {
                sceneLoader.character3DDict[k].SetActive(false);
            }
            if (XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList != null)
            {
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList.Length; k++)
                {
                    sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].transform.position = XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].Position.GetPos();
                    sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].transform.localRotation = XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].Rotation.GetRot();
                    sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].transform.localScale = XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].scale.GetScale();
                    sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].SetActive(true);
                    if (sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].GetComponent<Animator>() != null)
                    {
                        sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].GetComponent<Animator>().SetTrigger(XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].Anim);
                    }
                }
            }
        }
        if (XML_Reader.Instance.scenarioToDict.CreateObjType2DDictionary[sceneLoader.currentStage] != null)
        {
            for (int k = 0; k < XML_Reader.Instance.scenarioToDict.CreateObjType2DDictionary[sceneLoader.currentStage].Count; k++)
            {
                for (int j = 0; j < sceneLoader.object2DDict[k].Count; j++)
                {
                    sceneLoader.object2DDict[k][j].SetActive(false);
                }
            }
            if (XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D!=null)
            {
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D.Length; k++)
                {
                    for (int j = 0; j < XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList.Length; j++)
                    {
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().anchorMin = XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].anchorMin.GetAnchorMin();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().anchorMax = XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].anchorMax.GetAnchorMax();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().pivot = XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].pivot.GetPivot();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().offsetMin = XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].offsetMin.GetOffsetMin();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().offsetMax = XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].offsetMax.GetOffsetMax();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().anchoredPosition3D = new Vector3(sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().anchoredPosition3D.x, sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().anchoredPosition3D.y, 0.0f);
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().localRotation = XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].Rotation.GetRot();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().localScale = XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].scale.GetScale();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].SetActive(true);
                        if (sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<Animator>() != null)
                        {
                            sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<Animator>().SetTrigger(XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].Anim);
                        }
                    }
                }
            }
        }
        if (XML_Reader.Instance.scenarioToDict.CreateObj3DDictionary[sceneLoader.currentStage] != null)
        {
            for (int k = 0; k < sceneLoader.object3DDict.Count; k++)
            {
                sceneLoader.object3DDict[k].SetActive(false);
            }
            if (XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList!=null)
            {
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList.Length; k++)
                {
                    sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].transform.position = XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].Position.GetPos();
                    sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].transform.localRotation = XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].Rotation.GetRot();
                    sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].transform.localScale = XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].scale.GetScale();
                    sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].SetActive(true);
                    if (sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].GetComponent<Animator>() != null)
                    {
                        sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].GetComponent<Animator>().SetTrigger(XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].Anim);
                    }
                }
            }
        }

        //링크 넣을 것...
        if (XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].link.OnOff==1)
        {
            if (XML_Reader.Instance.scenarioToDict.CreateObjType2DDictionary[sceneLoader.currentStage] != null)
            {
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.StageInfoDictionary[sceneLoader.currentStage].CanvasCount; k++)
                {
                    for (int j = 0; j < sceneLoader.object2DDict[k].Count; j++)
                    {
                        if (sceneLoader.object2DDict[k][j].name == "Link")
                        {
                            //sceneLoader.object2DDict[k][j].transform.GetChild(0).GetComponent<Text>().text = XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].link.Name;
                            GameObject.Find("WebManager").SendMessage("initURL", XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].link.URL);
                            continue;
                        }
                    }
                }
            }
        }
        if (PlayerInfo.Instance.Gender == 0)
        {
            if (XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D != null)
            {
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D.Length; k++)
                {
                    for (int j = 0; j < sceneLoader.character2DDict[k].Count; j++)
                    {
                        if (sceneLoader.character2DDict[k][j].name == "CH_02")
                        {
                            sceneLoader.character2DDict[k][j].SetActive(false);
                        }
                    }
                }
            }
        }
        else if (PlayerInfo.Instance.Gender == 1)
        {
            if (XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D != null)
            {
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D.Length; k++)
                {
                    for (int j = 0; j < sceneLoader.character2DDict[k].Count; j++)
                    {
                        if (sceneLoader.character2DDict[k][j].name == "CH_01")
                        {
                            sceneLoader.character2DDict[k][j].SetActive(false);
                        }
                    }
                }
            }
        }

        TextOutPut();
    }

    public void CardPopUpEvent()
    {
        for (int k = 0; k < sceneLoader.CanvasList.Count; k++)
        {
            for (int j = 0; j < XML_Reader.Instance.scenarioToDict.StageInfoDictionary[sceneLoader.currentStage].CanvasCount; j++)
            {
                if (k == j)
                {
                    sceneLoader.CanvasList[k].transform.gameObject.SetActive(true);
                    break;
                }
                sceneLoader.CanvasList[k].transform.gameObject.SetActive(false);
            }
        }

        if (XML_Reader.Instance.scenarioToDict.CreateCharType2DDictionary[sceneLoader.currentStage] != null)
        {
            for (int k = 0; k < XML_Reader.Instance.scenarioToDict.CreateCharType2DDictionary[sceneLoader.currentStage].Count; k++)
            {
                for (int j = 0; j < sceneLoader.character2DDict[k].Count; j++)
                {
                    sceneLoader.character2DDict[k][j].SetActive(false);
                }
            }
            if (XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D != null)
            {
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D.Length; k++)
                {
                    for (int j = 0; j < XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList.Length; j++)
                    {
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().anchorMin = XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].anchorMin.GetAnchorMin();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().anchorMax = XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].anchorMax.GetAnchorMax();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().pivot = XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].pivot.GetPivot();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().offsetMin = XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].offsetMin.GetOffsetMin();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().offsetMax = XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].offsetMax.GetOffsetMax();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().anchoredPosition3D = new Vector3(sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().anchoredPosition3D.x, sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().anchoredPosition3D.y, 0.0f);
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().localRotation = XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].Rotation.GetRot();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().localScale = XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].scale.GetScale();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].SetActive(true);
                        if (sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<Animator>() != null)
                        {
                            sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<Animator>().SetTrigger(XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].Anim);
                        }
                    }
                }
            }
        }
        if (XML_Reader.Instance.scenarioToDict.CreateChar3DDictionary[sceneLoader.currentStage] != null)
        {
            for (int k = 0; k < sceneLoader.character3DDict.Count; k++)
            {
                sceneLoader.character3DDict[k].SetActive(false);
            }
            if (XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList != null)
            {
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList.Length; k++)
                {
                    sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].transform.position = XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].Position.GetPos();
                    sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].transform.localRotation = XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].Rotation.GetRot();
                    sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].transform.localScale = XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].scale.GetScale();
                    sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].SetActive(true);
                    if (sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].GetComponent<Animator>() != null)
                    {
                        sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].GetComponent<Animator>().SetTrigger(XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].Anim);
                    }
                }
            }
        }
        if (XML_Reader.Instance.scenarioToDict.CreateObjType2DDictionary[sceneLoader.currentStage] != null)
        {
            for (int k = 0; k < XML_Reader.Instance.scenarioToDict.CreateObjType2DDictionary[sceneLoader.currentStage].Count; k++)
            {
                for (int j = 0; j < sceneLoader.object2DDict[k].Count; j++)
                {
                    sceneLoader.object2DDict[k][j].SetActive(false);
                }
            }
            if (XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D != null)
            {
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D.Length; k++)
                {
                    for (int j = 0; j < XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList.Length; j++)
                    {
                        Debug.Log(XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID);
                        Debug.Log(XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID);
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().anchorMin = XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].anchorMin.GetAnchorMin();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().anchorMax = XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].anchorMax.GetAnchorMax();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().pivot = XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].pivot.GetPivot();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().offsetMin = XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].offsetMin.GetOffsetMin();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().offsetMax = XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].offsetMax.GetOffsetMax();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().anchoredPosition3D = new Vector3(sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().anchoredPosition3D.x, sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().anchoredPosition3D.y, 0.0f);
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().localRotation = XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].Rotation.GetRot();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().localScale = XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].scale.GetScale();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].SetActive(true);
                        if (sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<Animator>() != null)
                        {
                            sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<Animator>().SetTrigger(XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].Anim);
                        }
                    }
                }
            }
        }
        if (XML_Reader.Instance.scenarioToDict.CreateObj3DDictionary[sceneLoader.currentStage] != null)
        {
            for (int k = 0; k < sceneLoader.object3DDict.Count; k++)
            {
                sceneLoader.object3DDict[k].SetActive(false);
            }
            if (XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList != null)
            {
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList.Length; k++)
                {
                    sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].transform.position = XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].Position.GetPos();
                    sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].transform.localRotation = XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].Rotation.GetRot();
                    sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].transform.localScale = XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].scale.GetScale();
                    sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].SetActive(true);
                    if (sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].GetComponent<Animator>() != null)
                    {
                        sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].GetComponent<Animator>().SetTrigger(XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].Anim);
                    }
                }
            }
        }

        GameObject.FindGameObjectWithTag("Title").GetComponent<Text>().text = XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].Title;

        if (PlayerInfo.Instance.Gender == 0)
        {
            if (XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D != null)
            {
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D.Length; k++)
                {
                    for (int j = 0; j < sceneLoader.character2DDict[k].Count; j++)
                    {
                        if (sceneLoader.character2DDict[k][j].name == "CH_02")
                        {
                            sceneLoader.character2DDict[k][j].SetActive(false);
                        }
                    }
                }
            }
        }
        else if (PlayerInfo.Instance.Gender == 1)
        {
            if (XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D != null)
            {
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D.Length; k++)
                {
                    for (int j = 0; j < sceneLoader.character2DDict[k].Count; j++)
                    {
                        if (sceneLoader.character2DDict[k][j].name == "CH_01")
                        {
                            sceneLoader.character2DDict[k][j].SetActive(false);
                        }
                    }
                }
            }
        }
        TextOutPut();
    }

    public void SphereImageOnEvent()
    {
        for(int k=0;k<sceneLoader.CanvasList.Count;k++)
        {
            for(int j=0;j<XML_Reader.Instance.scenarioToDict.StageInfoDictionary[sceneLoader.currentStage].CanvasCount;j++)
            {
                if(k==j)
                {
                    sceneLoader.CanvasList[k].transform.gameObject.SetActive(true);
                    break;
                }
                sceneLoader.CanvasList[k].transform.gameObject.SetActive(false);
            }
        }

        if (XML_Reader.Instance.scenarioToDict.CreateCharType2DDictionary[sceneLoader.currentStage] != null)
        {
            for (int k = 0; k < XML_Reader.Instance.scenarioToDict.CreateCharType2DDictionary[sceneLoader.currentStage].Count; k++)
            {
                for (int j = 0; j < sceneLoader.character2DDict[k].Count; j++)
                {
                    sceneLoader.character2DDict[k][j].SetActive(false);
                }
            }
            if (XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D != null)
            {
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D.Length; k++)
                {
                    for (int j = 0; j < XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList.Length; j++)
                    {
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().anchorMin = XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].anchorMin.GetAnchorMin();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().anchorMax = XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].anchorMax.GetAnchorMax();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().pivot = XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].pivot.GetPivot();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().offsetMin = XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].offsetMin.GetOffsetMin();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().offsetMax = XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].offsetMax.GetOffsetMax();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().anchoredPosition3D = new Vector3(sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().anchoredPosition3D.x, sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().anchoredPosition3D.y, 0.0f);
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().localRotation = XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].Rotation.GetRot();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().localScale = XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].scale.GetScale();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].SetActive(true);
                        if (sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<Animator>() != null)
                        {
                            sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<Animator>().SetTrigger(XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].Anim);
                        }
                    }
                }
            }
        }
        if (XML_Reader.Instance.scenarioToDict.CreateChar3DDictionary[sceneLoader.currentStage] != null)
        {
            for (int k = 0; k < sceneLoader.character3DDict.Count; k++)
            {
                sceneLoader.character3DDict[k].SetActive(false);
            }
            if (XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList != null)
            {
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList.Length; k++)
                {
                    sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].transform.position = XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].Position.GetPos();
                    sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].transform.localRotation = XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].Rotation.GetRot();
                    sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].transform.localScale = XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].scale.GetScale();
                    sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].SetActive(true);
                    if (sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].GetComponent<Animator>() != null)
                    {
                        sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].GetComponent<Animator>().SetTrigger(XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].Anim);
                    }
                }
            }
        }
        if (XML_Reader.Instance.scenarioToDict.CreateObjType2DDictionary[sceneLoader.currentStage] != null)
        {
            for (int k = 0; k < XML_Reader.Instance.scenarioToDict.CreateObjType2DDictionary[sceneLoader.currentStage].Count; k++)
            {
                for (int j = 0; j < sceneLoader.object2DDict[k].Count; j++)
                {
                    sceneLoader.object2DDict[k][j].SetActive(false);
                }
            }
            if (XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D != null)
            {
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D.Length; k++)
                {
                    for (int j = 0; j < XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList.Length; j++)
                    {
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().anchorMin = XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].anchorMin.GetAnchorMin();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().anchorMax = XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].anchorMax.GetAnchorMax();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().pivot = XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].pivot.GetPivot();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().offsetMin = XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].offsetMin.GetOffsetMin();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().offsetMax = XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].offsetMax.GetOffsetMax();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().anchoredPosition3D = new Vector3(sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().anchoredPosition3D.x, sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().anchoredPosition3D.y, 0.0f);
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().localRotation = XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].Rotation.GetRot();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().localScale = XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].scale.GetScale();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].SetActive(true);
                        if (sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<Animator>() != null)
                        {
                            sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<Animator>().SetTrigger(XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].Anim);
                        }
                    }
                }
            }
        }
        if (XML_Reader.Instance.scenarioToDict.CreateObj3DDictionary[sceneLoader.currentStage] != null)
        {
            for (int k = 0; k < sceneLoader.object3DDict.Count; k++)
            {
                sceneLoader.object3DDict[k].SetActive(false);
            }
            if (XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList != null)
            {
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList.Length; k++)
                {
                    sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].transform.position = XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].Position.GetPos();
                    sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].transform.localRotation = XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].Rotation.GetRot();
                    sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].transform.localScale = XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].scale.GetScale();
                    sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].SetActive(true);
                    if (sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].GetComponent<Animator>() != null)
                    {
                        sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].GetComponent<Animator>().SetTrigger(XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].Anim);
                    }
                }
            }
        }

        //텍스처 변경하기
        for (int k=0;k<sceneLoader.object3DDict.Count;k++)
        {
            if(sceneLoader.object3DDict[k].name=="Sphere")
            {
                GameObject Sphere = sceneLoader.object3DDict[k];
                //Sphere.GetComponent<Renderer>().material.mainTexture = sceneLoader.myLoadedAssetBundle.LoadAsset<Texture>(XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].SphereImage);
                Sphere.GetComponent<Renderer>().material.mainTexture = Resources.Load<Texture>(XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].SphereImage);
                break;
            }
        }

        if (PlayerInfo.Instance.Gender == 0)
        {
            if (XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D != null)
            {
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D.Length; k++)
                {
                    for (int j = 0; j < sceneLoader.character2DDict[k].Count; j++)
                    {
                        if (sceneLoader.character2DDict[k][j].name == "CH_02")
                        {
                            sceneLoader.character2DDict[k][j].SetActive(false);
                        }
                    }
                }
            }
        }
        else if (PlayerInfo.Instance.Gender == 1)
        {
            if (XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D != null)
            {
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D.Length; k++)
                {
                    for (int j = 0; j < sceneLoader.character2DDict[k].Count; j++)
                    {
                        if (sceneLoader.character2DDict[k][j].name == "CH_01")
                        {
                            sceneLoader.character2DDict[k][j].SetActive(false);
                        }
                    }
                }
            }
        }
        TextOutPut();
    }

    public void SpeechBubbleOnEvent()
    {
        for (int k = 0; k < sceneLoader.CanvasList.Count; k++)
        {
            for (int j = 0; j < XML_Reader.Instance.scenarioToDict.StageInfoDictionary[sceneLoader.currentStage].CanvasCount; j++)
            {
                if (k == j)
                {
                    sceneLoader.CanvasList[k].transform.gameObject.SetActive(true);
                    break;
                }
                sceneLoader.CanvasList[k].transform.gameObject.SetActive(false);
            }
        }

        if (XML_Reader.Instance.scenarioToDict.CreateCharType2DDictionary[sceneLoader.currentStage] != null)
        {
            for (int k = 0; k < XML_Reader.Instance.scenarioToDict.CreateCharType2DDictionary[sceneLoader.currentStage].Count; k++)
            {
                for (int j = 0; j < sceneLoader.character2DDict[k].Count; j++)
                {
                    sceneLoader.character2DDict[k][j].SetActive(false);
                }
            }
            if (XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D != null)
            {
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D.Length; k++)
                {
                    for (int j = 0; j < XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList.Length; j++)
                    {
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().anchorMin = XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].anchorMin.GetAnchorMin();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().anchorMax = XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].anchorMax.GetAnchorMax();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().pivot = XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].pivot.GetPivot();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().offsetMin = XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].offsetMin.GetOffsetMin();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().offsetMax = XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].offsetMax.GetOffsetMax();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().anchoredPosition3D = new Vector3(sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().anchoredPosition3D.x, sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().anchoredPosition3D.y, 0.0f);
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().localRotation = XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].Rotation.GetRot();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().localScale = XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].scale.GetScale();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].SetActive(true);
                        if (sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<Animator>() != null)
                        {
                            sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<Animator>().SetTrigger(XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].Anim);
                        }
                    }
                }
            }
        }
        if (XML_Reader.Instance.scenarioToDict.CreateChar3DDictionary[sceneLoader.currentStage] != null)
        {
            for (int k = 0; k < sceneLoader.character3DDict.Count; k++)
            {
                sceneLoader.character3DDict[k].SetActive(false);
            }
            if (XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList != null)
            {
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList.Length; k++)
                {
                    sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].transform.position = XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].Position.GetPos();
                    sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].transform.localRotation = XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].Rotation.GetRot();
                    sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].transform.localScale = XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].scale.GetScale();
                    sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].SetActive(true);
                    if (sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].GetComponent<Animator>() != null)
                    {
                        sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].GetComponent<Animator>().SetTrigger(XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].Anim);
                    }
                }
            }
        }
        if (XML_Reader.Instance.scenarioToDict.CreateObjType2DDictionary[sceneLoader.currentStage] != null)
        {
            for (int k = 0; k < XML_Reader.Instance.scenarioToDict.CreateObjType2DDictionary[sceneLoader.currentStage].Count; k++)
            {
                for (int j = 0; j < sceneLoader.object2DDict[k].Count; j++)
                {
                    sceneLoader.object2DDict[k][j].SetActive(false);
                }
            }
            if (XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D != null)
            {
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D.Length; k++)
                {
                    for (int j = 0; j < XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList.Length; j++)
                    {
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().anchorMin = XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].anchorMin.GetAnchorMin();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().anchorMax = XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].anchorMax.GetAnchorMax();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().pivot = XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].pivot.GetPivot();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().offsetMin = XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].offsetMin.GetOffsetMin();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().offsetMax = XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].offsetMax.GetOffsetMax();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().anchoredPosition3D = new Vector3(sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().anchoredPosition3D.x, sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().anchoredPosition3D.y, 0.0f);
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().localRotation = XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].Rotation.GetRot();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().localScale = XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].scale.GetScale();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].SetActive(true);
                        if (sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<Animator>() != null)
                        {
                            sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<Animator>().SetTrigger(XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].Anim);
                        }
                    }
                }
            }
        }
        if (XML_Reader.Instance.scenarioToDict.CreateObj3DDictionary[sceneLoader.currentStage] != null)
        {
            for (int k = 0; k < sceneLoader.object3DDict.Count; k++)
            {
                sceneLoader.object3DDict[k].SetActive(false);
            }
            if (XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList != null)
            {
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList.Length; k++)
                {
                    sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].transform.position = XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].Position.GetPos();
                    sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].transform.localRotation = XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].Rotation.GetRot();
                    sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].transform.localScale = XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].scale.GetScale();
                    sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].SetActive(true);
                    if (sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].GetComponent<Animator>() != null)
                    {
                        sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].GetComponent<Animator>().SetTrigger(XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].Anim);
                    }
                }
            }
        }
        if (PlayerInfo.Instance.Gender == 0)
        {
            if (XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D != null)
            {
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D.Length; k++)
                {
                    for (int j = 0; j < sceneLoader.character2DDict[k].Count; j++)
                    {
                        if (sceneLoader.character2DDict[k][j].name == "CH_02")
                        {
                            sceneLoader.character2DDict[k][j].SetActive(false);
                        }
                    }
                }
            }
        }
        else if (PlayerInfo.Instance.Gender == 1)
        {
            if (XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D != null)
            {
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D.Length; k++)
                {
                    for (int j = 0; j < sceneLoader.character2DDict[k].Count; j++)
                    {
                        if (sceneLoader.character2DDict[k][j].name == "CH_01")
                        {
                            sceneLoader.character2DDict[k][j].SetActive(false);
                        }
                    }
                }
            }
        }
        TextOutPut();
    }

    public void QuarterSelectEvent()
    {
        for (int k = 0; k < sceneLoader.CanvasList.Count; k++)
        {
            for (int j = 0; j < XML_Reader.Instance.scenarioToDict.TableSettingEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D.Length; j++)
            {
                sceneLoader.CanvasList[k].transform.gameObject.SetActive(false);
            }
        }

        if (XML_Reader.Instance.scenarioToDict.CreateCharType2DDictionary[sceneLoader.currentStage] != null)
        {
            for (int k = 0; k < XML_Reader.Instance.scenarioToDict.CreateCharType2DDictionary[sceneLoader.currentStage].Count; k++)
            {
                for (int j = 0; j < sceneLoader.character2DDict[k].Count; j++)
                {
                    sceneLoader.character2DDict[k][j].SetActive(false);
                }
            }
        }

        if (XML_Reader.Instance.scenarioToDict.CreateChar3DDictionary[sceneLoader.currentStage] != null)
        {
            for (int k = 0; k < sceneLoader.character3DDict.Count; k++)
            {
                sceneLoader.character3DDict[k].SetActive(false);
            }
        }

        if (XML_Reader.Instance.scenarioToDict.CreateObjType2DDictionary[sceneLoader.currentStage] != null)
        {
            for (int k = 0; k < XML_Reader.Instance.scenarioToDict.CreateObjType2DDictionary[sceneLoader.currentStage].Count; k++)
            {
                for (int j = 0; j < sceneLoader.object2DDict[k].Count; j++)
                {
                    sceneLoader.object2DDict[k][j].SetActive(false);
                }
            }
        }

        if (XML_Reader.Instance.scenarioToDict.CreateObj3DDictionary != null)
        {
            for (int k = 0; k < sceneLoader.object3DDict.Count; k++)
            {
                sceneLoader.object3DDict[k].SetActive(false);
            }
        }

        if (XML_Reader.Instance.scenarioToDict.QuarterSelectEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D != null)
        {
            //2D 오브젝트가 있다면...
        }

        if (XML_Reader.Instance.scenarioToDict.QuarterSelectEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.ActiveObjList != null)
        {
            for (int k = 0; k < XML_Reader.Instance.scenarioToDict.QuarterSelectEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.ActiveObjList.Length; k++)
            {
                for (int j = 0; j < sceneLoader.object3DDict.Count; j++)
                {
                    if (XML_Reader.Instance.scenarioToDict.QuarterSelectEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.ActiveObjList[k].Name == sceneLoader.object3DDict[j].name)
                    {
                        Debug.Log(sceneLoader.object3DDict[j].name);
                        sceneLoader.object3DDict[j].SetActive(true);
                    }
                }
            }
        }
        m_Camera.gameObject.SetActive(false);
        Debug.Log("1");
        ARCore.SetActive(true);
        Debug.Log("2");
        ARController.SetActive(true);
        Debug.Log("3");
        Light.SetActive(true);
    }

    public void QuarterEvent()
    {
        for (int k = 0; k < sceneLoader.CanvasList.Count; k++)
        {
            for (int j = 0; j < XML_Reader.Instance.scenarioToDict.StageInfoDictionary[sceneLoader.currentStage].CanvasCount; j++)
            {
                if (k == j)
                {
                    sceneLoader.CanvasList[k].transform.gameObject.SetActive(true);
                    break;
                }
                sceneLoader.CanvasList[k].transform.gameObject.SetActive(false);
            }
        }
        if (XML_Reader.Instance.scenarioToDict.CreateCharType2DDictionary[sceneLoader.currentStage] != null)
        {
            for (int k = 0; k < XML_Reader.Instance.scenarioToDict.CreateCharType2DDictionary[sceneLoader.currentStage].Count; k++)
            {
                for (int j = 0; j < sceneLoader.character2DDict[k].Count; j++)
                {
                    sceneLoader.character2DDict[k][j].SetActive(false);
                }
            }
            if (XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D != null)
            {
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D.Length; k++)
                {
                    for (int j = 0; j < XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList.Length; j++)
                    {
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().anchorMin = XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].anchorMin.GetAnchorMin();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().anchorMax = XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].anchorMax.GetAnchorMax();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().pivot = XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].pivot.GetPivot();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().offsetMin = XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].offsetMin.GetOffsetMin();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().offsetMax = XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].offsetMax.GetOffsetMax();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().anchoredPosition3D = new Vector3(sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().anchoredPosition3D.x, sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().anchoredPosition3D.y, 0.0f);
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().localRotation = XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].Rotation.GetRot();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().localScale = XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].scale.GetScale();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].SetActive(true);
                        if (sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<Animator>() != null)
                        {
                            sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<Animator>().SetTrigger(XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].Anim);
                        }
                    }
                }
            }
        }
        if (XML_Reader.Instance.scenarioToDict.CreateChar3DDictionary[sceneLoader.currentStage] != null)
        {
            for (int k = 0; k < sceneLoader.character3DDict.Count; k++)
            {
                sceneLoader.character3DDict[k].SetActive(false);
            }
            if (XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList != null)
            {
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList.Length; k++)
                {
                    sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].transform.position = XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].Position.GetPos();
                    sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].transform.localRotation = XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].Rotation.GetRot();
                    sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].transform.localScale = XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].scale.GetScale();
                    sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].SetActive(true);
                    if (sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].GetComponent<Animator>() != null)
                    {
                        sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].GetComponent<Animator>().SetTrigger(XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].Anim);
                    }
                }
            }
        }
        if (XML_Reader.Instance.scenarioToDict.CreateObjType2DDictionary[sceneLoader.currentStage] != null)
        {
            for (int k = 0; k < XML_Reader.Instance.scenarioToDict.CreateObjType2DDictionary[sceneLoader.currentStage].Count; k++)
            {
                for (int j = 0; j < sceneLoader.object2DDict[k].Count; j++)
                {
                    sceneLoader.object2DDict[k][j].SetActive(false);
                }
            }
            if (XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D!=null)
            {
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D.Length; k++)
                {
                    for (int j = 0; j < XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList.Length; j++)
                    {
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().anchorMin = XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].anchorMin.GetAnchorMin();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().anchorMax = XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].anchorMax.GetAnchorMax();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().pivot = XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].pivot.GetPivot();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().offsetMin = XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].offsetMin.GetOffsetMin();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().offsetMax = XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].offsetMax.GetOffsetMax();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().anchoredPosition3D = new Vector3(sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().anchoredPosition3D.x, sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().anchoredPosition3D.y, 0.0f);
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().localRotation = XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].Rotation.GetRot();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().localScale = XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].scale.GetScale();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].SetActive(true);
                        if (sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<Animator>() != null)
                        {
                            sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<Animator>().SetTrigger(XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].Anim);
                        }
                    }
                }
            }
        }
        if (XML_Reader.Instance.scenarioToDict.CreateObj3DDictionary[sceneLoader.currentStage] != null)
        {
            for (int k = 0; k < sceneLoader.object3DDict.Count; k++)
            {
                sceneLoader.object3DDict[k].SetActive(false);
            }
            if (XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList!=null)
            {
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList.Length; k++)
                {
                    sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].transform.position = XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].Position.GetPos();
                    sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].transform.localRotation = XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].Rotation.GetRot();
                    sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].transform.localScale = XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].scale.GetScale();
                    sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].SetActive(true);
                    if (sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].GetComponent<Animator>() != null)
                    {
                        sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].GetComponent<Animator>().SetTrigger(XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].Anim);
                    }
                }
            }
        }

        //진행할때 분기를 클리어함을 체크
        XML_Reader.Instance.scenarioToDict.BranchDictionary[sceneLoader.currentStage][XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].indexList.Group][XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].indexList.ID] = true;

        if (PlayerInfo.Instance.Gender == 0)
        {
            if (XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D != null)
            {
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D.Length; k++)
                {
                    for (int j = 0; j < sceneLoader.character2DDict[k].Count; j++)
                    {
                        if (sceneLoader.character2DDict[k][j].name == "CH_02")
                        {
                            sceneLoader.character2DDict[k][j].SetActive(false);
                        }
                    }
                }
            }
        }
        else if (PlayerInfo.Instance.Gender == 1)
        {
            if (XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D != null)
            {
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D.Length; k++)
                {
                    for (int j = 0; j < sceneLoader.character2DDict[k].Count; j++)
                    {
                        if (sceneLoader.character2DDict[k][j].name == "CH_01")
                        {
                            sceneLoader.character2DDict[k][j].SetActive(false);
                        }
                    }
                }
            }
        }
        TextOutPut();
    }

    public void LoadSceneEvent()
    {
        //sceneLoader.myLoadedAssetBundle.Unload(false);
        SceneManager.LoadScene(XML_Reader.Instance.scenarioToDict.LoadSceneEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].SceneName);
    }

    public void QuizEvent()
    {
        for (int k = 0; k < sceneLoader.CanvasList.Count; k++)
        {
            sceneLoader.CanvasList[k].transform.gameObject.SetActive(false);
        }

        for (int k = 0; k < XML_Reader.Instance.scenarioToDict.StageInfoDictionary[sceneLoader.currentStage].CanvasCount; k++)
        {
            sceneLoader.CanvasList[k].transform.gameObject.SetActive(true);
        }

        if (XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].QuizInfoList.Type == "QuizTimeRec")
        {
            QuizTimeRec(XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].QuizInfoList.ID);
        }
        else if(XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].QuizInfoList.Type =="QuizTimeAtk")
        {
            QuizTimeAtk(XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].QuizInfoList.ID);
        }
        else
        {
            for (int k = 0; k < sceneLoader.CanvasList.Count; k++)
            {
                if (sceneLoader.CanvasList[k].name == XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].QuizInfoList.Type+"Canvas")
                {
                    sceneLoader.CanvasList[k].transform.gameObject.SetActive(true);
                }
            }
            switch (XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].QuizInfoList.Type)
            {
                case "Link":                    
                    QuizManager.GenerateLinkQuiz(XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].QuizInfoList.ID);
                    break;
                case "Opt":
                    QuizManager.GenerateOptQuiz(XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].QuizInfoList.ID);
                    break;
                case "ImgOpt":
                    QuizManager.GenerateImgOptQuiz(XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].QuizInfoList.ID);
                    break;
                case "Typing1":
                    QuizManager.GenerateTyping1Quiz(XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].QuizInfoList.ID);
                    break;
                case "Typing2":
                    QuizManager.GenerateTyping2Quiz(XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].QuizInfoList.ID);
                    break;
                default:
                    break;
            }
           
        }
        if (XML_Reader.Instance.scenarioToDict.CreateCharType2DDictionary[sceneLoader.currentStage] != null)
        {
            for (int k = 0; k < XML_Reader.Instance.scenarioToDict.CreateCharType2DDictionary[sceneLoader.currentStage].Count; k++)
            {
                for (int j = 0; j < sceneLoader.character2DDict[k].Count; j++)
                {
                    sceneLoader.character2DDict[k][j].SetActive(false);
                }
            }
            if (XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D != null)
            {
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D.Length; k++)
                {
                    for (int j = 0; j < XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList.Length; j++)
                    {
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().anchorMin = XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].anchorMin.GetAnchorMin();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().anchorMax = XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].anchorMax.GetAnchorMax();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().pivot = XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].pivot.GetPivot();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().offsetMin = XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].offsetMin.GetOffsetMin();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().offsetMax = XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].offsetMax.GetOffsetMax();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().anchoredPosition3D = new Vector3(sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().anchoredPosition3D.x, sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().anchoredPosition3D.y, 0.0f);
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().localRotation = XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].Rotation.GetRot();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().localScale = XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].scale.GetScale();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].SetActive(true);
                        if (sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<Animator>() != null)
                        {
                            sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<Animator>().SetTrigger(XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].Anim);
                        }
                    }
                }
            }
        }
        if (XML_Reader.Instance.scenarioToDict.CreateChar3DDictionary[sceneLoader.currentStage] != null)
        {
            for (int k = 0; k < sceneLoader.character3DDict.Count; k++)
            {
                sceneLoader.character3DDict[k].SetActive(false);
            }
            if (XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList!=null)
            {
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList.Length; k++)
                {
                    sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].transform.position = XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].Position.GetPos();
                    sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].transform.localRotation = XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].Rotation.GetRot();
                    sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].transform.localScale = XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].scale.GetScale();
                    sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].SetActive(true);
                    if (sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].GetComponent<Animator>() != null)
                    {
                        sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].GetComponent<Animator>().SetTrigger(XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].Anim);
                    }
                }
            }
        }
        if (XML_Reader.Instance.scenarioToDict.CreateObjType2DDictionary[sceneLoader.currentStage] != null)
        {
            for (int k = 0; k < XML_Reader.Instance.scenarioToDict.CreateObjType2DDictionary[sceneLoader.currentStage].Count; k++)
            {
                for (int j = 0; j < sceneLoader.object2DDict[k].Count; j++)
                {
                    sceneLoader.object2DDict[k][j].SetActive(false);
                }
            }
            if (XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D != null)
            {
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D.Length; k++)
                {
                    for (int j = 0; j < XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList.Length; j++)
                    {
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().anchorMin = XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].anchorMin.GetAnchorMin();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().anchorMax = XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].anchorMax.GetAnchorMax();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().pivot = XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].pivot.GetPivot();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().offsetMin = XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].offsetMin.GetOffsetMin();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().offsetMax = XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].offsetMax.GetOffsetMax();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().anchoredPosition3D = new Vector3(sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().anchoredPosition3D.x, sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().anchoredPosition3D.y, 0.0f);
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().localRotation = XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].Rotation.GetRot();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().localScale = XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].scale.GetScale();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].SetActive(true);
                        if (sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<Animator>() != null)
                        {
                            sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<Animator>().SetTrigger(XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].Anim);
                        }
                    }
                }
            }
        }
        if (XML_Reader.Instance.scenarioToDict.CreateObj3DDictionary[sceneLoader.currentStage] != null)
        {
            for (int k = 0; k < sceneLoader.object3DDict.Count; k++)
            {
                sceneLoader.object3DDict[k].SetActive(false);
            }
            if (XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList != null)
            {
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList.Length; k++)
                {
                    sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].transform.position = XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].Position.GetPos();
                    sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].transform.localRotation = XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].Rotation.GetRot();
                    sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].transform.localScale = XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].scale.GetScale();
                    sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].SetActive(true);
                    if (sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].GetComponent<Animator>() != null)
                    {
                        sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].GetComponent<Animator>().SetTrigger(XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].Anim);
                    }
                }
            }
        }

        if (PlayerInfo.Instance.Gender == 0)
        {
            if (XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D != null)
            {
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D.Length; k++)
                {
                    for (int j = 0; j < sceneLoader.character2DDict[k].Count; j++)
                    {
                        if (sceneLoader.character2DDict[k][j].name == "CH_02")
                        {
                            sceneLoader.character2DDict[k][j].SetActive(false);
                        }
                    }
                }
            }
        }
        else if (PlayerInfo.Instance.Gender == 1)
        {
            if (XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D != null)
            {
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D.Length; k++)
                {
                    for (int j = 0; j < sceneLoader.character2DDict[k].Count; j++)
                    {
                        if (sceneLoader.character2DDict[k][j].name == "CH_01")
                        {
                            sceneLoader.character2DDict[k][j].SetActive(false);
                        }
                    }
                }
            }
        }
    }

    //시간 기록 퀴즈 생성
    public void QuizTimeRec(int id)
    {
        
        if (QuizCount == 0)
        {
            Minute = 0;
            Timer = 0.0f;
            QuizCount = 0;
            QuizClear = false;                        
        }
        
        for (int k = 0; k < sceneLoader.CanvasList.Count; k++)
        {
            if (sceneLoader.CanvasList[k].name == Quiz_XML_Reader.Instance.quizToDict.QuizTimeRecDictionary[id].InitQuizList[QuizCount].Type + "Canvas")
            {
                sceneLoader.CanvasList[k].transform.gameObject.SetActive(true);
            }
            else if(sceneLoader.CanvasList[k].name=="TimerCanvas")
            {
                sceneLoader.CanvasList[k].transform.gameObject.SetActive(true);
            }
        }

        switch (Quiz_XML_Reader.Instance.quizToDict.QuizTimeRecDictionary[id].InitQuizList[QuizCount].Type)
        {
            case "Link":
                QuizManager.GenerateLinkQuizTimeRec(Quiz_XML_Reader.Instance.quizToDict.QuizTimeRecDictionary[id].InitQuizList[QuizCount].ID);
                break;
            case "Typing1":
                QuizManager.GenerateTyping1QuizTimeRec(Quiz_XML_Reader.Instance.quizToDict.QuizTimeRecDictionary[id].InitQuizList[QuizCount].ID);
                break;
            case "Typing2":
                QuizManager.GenerateTyping2QuizTimeRec(Quiz_XML_Reader.Instance.quizToDict.QuizTimeRecDictionary[id].InitQuizList[QuizCount].ID);
                break;
            case "Opt":
                QuizManager.GenerateOptQuizTimeRec(Quiz_XML_Reader.Instance.quizToDict.QuizTimeRecDictionary[id].InitQuizList[QuizCount].ID);
                break;
            case "ImgOpt":
                QuizManager.GenerateImgOptQuizTimeRec(Quiz_XML_Reader.Instance.quizToDict.QuizTimeRecDictionary[id].InitQuizList[QuizCount].ID);
                break;
            default:
                break;
        }        
    }

    //타임 어택 퀴즈 생성
    public void QuizTimeAtk(int id)
    {
        
        if (QuizCount == 0)
        {
            Minute = 0;
            Timer = Quiz_XML_Reader.Instance.quizToDict.QuizTimeAtkDictionary[id].Time;
            float temp = Timer / 60.0f;
            for (int k = 0; k < temp; k++)
            {
                if (Timer >= 60)
                {
                    Minute++;
                    Timer -= 60;
                }
            }
            if(Timer<=0)
            {
                Timer = 0.0f;
            }
            QuizClear = false;
        }
        
        for (int k=0;k<sceneLoader.CanvasList.Count;k++)
        {
            if(sceneLoader.CanvasList[k].name==Quiz_XML_Reader.Instance.quizToDict.QuizTimeAtkDictionary[id].InitQuizList[QuizCount].Type+"Canvas")
            {
                sceneLoader.CanvasList[k].transform.gameObject.SetActive(true);
            }
            else if(sceneLoader.CanvasList[k].name=="TimerCanvas")
            {
                sceneLoader.CanvasList[k].transform.gameObject.SetActive(true);
            }
        }

        switch(Quiz_XML_Reader.Instance.quizToDict.QuizTimeAtkDictionary[id].InitQuizList[QuizCount].Type)
        {
            case "Link":
                QuizManager.GenerateLinkQuizTimeAtk(Quiz_XML_Reader.Instance.quizToDict.QuizTimeAtkDictionary[id].InitQuizList[QuizCount].ID);
                break;
            case "Typing1":
                QuizManager.GenerateTyping1QuizTimeAtk(Quiz_XML_Reader.Instance.quizToDict.QuizTimeAtkDictionary[id].InitQuizList[QuizCount].ID);
                break;
            case "Typing2":
                QuizManager.GenerateTyping2QuizTimeAtk(Quiz_XML_Reader.Instance.quizToDict.QuizTimeAtkDictionary[id].InitQuizList[QuizCount].ID);
                break;
            case "Opt":
                QuizManager.GenerateOptQuizTimeAtk(Quiz_XML_Reader.Instance.quizToDict.QuizTimeAtkDictionary[id].InitQuizList[QuizCount].ID);
                break;
            case "ImgOpt":
                QuizManager.GenerateImgOptQuizTimeAtk(Quiz_XML_Reader.Instance.quizToDict.QuizTimeAtkDictionary[id].InitQuizList[QuizCount].ID);
                break;
            default:
                break;
        }        
    }

    //사진 찍고 저장
    public void TakeCaptureEvent()
    {
        if (ARCore != null)
        {            
            ARCore.SetActive(true);

            for(int k=0;k<sceneLoader.CanvasList.Count;k++)
            {
                if(sceneLoader.CanvasList[k].name=="CaptureCanvas"||sceneLoader.CanvasList[k].name=="PhotoCanvas")
                {
                    sceneLoader.CanvasList[k].transform.gameObject.SetActive(true);
                }
                else
                {
                    sceneLoader.CanvasList[k].transform.gameObject.SetActive(false);
                }
            }

            if (XML_Reader.Instance.scenarioToDict.CreateCharType2DDictionary[sceneLoader.currentStage] != null)
            {
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.CreateCharType2DDictionary[sceneLoader.currentStage].Count; k++)
                {
                    for (int j = 0; j < sceneLoader.character2DDict[k].Count; j++)
                    {
                        sceneLoader.character2DDict[k][j].SetActive(false);
                    }
                }
                if (XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D != null)
                {
                    for (int k = 0; k < XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D.Length; k++)
                    {
                        for (int j = 0; j < XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList.Length; j++)
                        {
                            sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().anchorMin = XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].anchorMin.GetAnchorMin();
                            sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().anchorMax = XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].anchorMax.GetAnchorMax();
                            sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().pivot = XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].pivot.GetPivot();
                            sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().offsetMin = XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].offsetMin.GetOffsetMin();
                            sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().offsetMax = XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].offsetMax.GetOffsetMax();
                            sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().anchoredPosition3D = new Vector3(sceneLoader.character2DDict[k][XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().anchoredPosition3D.x, sceneLoader.character2DDict[k][XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().anchoredPosition3D.y, 0.0f);
                            sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().localRotation = XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].Rotation.GetRot();
                            sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().localScale = XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].scale.GetScale();
                            sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].SetActive(true);
                            if (sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<Animator>() != null)
                            {
                                sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<Animator>().SetTrigger(XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].Anim);
                            }
                        }
                    }
                }
            }
            if (XML_Reader.Instance.scenarioToDict.CreateChar3DDictionary[sceneLoader.currentStage] != null)
            {
                for (int k = 0; k < sceneLoader.character3DDict.Count; k++)
                {
                    sceneLoader.character3DDict[k].SetActive(false);
                }
                if (XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList != null)
                {
                    for (int k = 0; k < XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList.Length; k++)
                    {
                        sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].transform.position = XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].Position.GetPos();
                        sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].transform.localRotation = XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].Rotation.GetRot();
                        sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].transform.localScale = XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].scale.GetScale();
                        sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].SetActive(true);
                        if (sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].GetComponent<Animator>() != null)
                        {
                            sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].GetComponent<Animator>().SetTrigger(XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].Anim);
                        }
                    }
                }
            }
            if (XML_Reader.Instance.scenarioToDict.CreateObjType2DDictionary[sceneLoader.currentStage] != null)
            {
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.CreateObjType2DDictionary[sceneLoader.currentStage].Count; k++)
                {
                    for (int j = 0; j < sceneLoader.object2DDict[k].Count; j++)
                    {
                        sceneLoader.object2DDict[k][j].SetActive(false);
                    }
                }
                if (XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D != null)
                {
                    for (int k = 0; k < XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D.Length; k++)
                    {
                        for (int j = 0; j < XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList.Length; j++)
                        {
                            sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().anchorMin = XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].anchorMin.GetAnchorMin();
                            sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().anchorMax = XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].anchorMax.GetAnchorMax();
                            sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().pivot = XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].pivot.GetPivot();
                            sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().offsetMin = XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].offsetMin.GetOffsetMin();
                            sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().offsetMax = XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].offsetMax.GetOffsetMax();
                            sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().anchoredPosition3D = new Vector3(sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().anchoredPosition3D.x, sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().anchoredPosition3D.y, 0.0f);
                            sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().localRotation = XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].Rotation.GetRot();
                            sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().localScale = XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].scale.GetScale();
                            sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].SetActive(true);
                            if (sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<Animator>() != null)
                            {
                                sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<Animator>().SetTrigger(XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].Anim);
                            }
                        }
                    }
                }
            }
            if (XML_Reader.Instance.scenarioToDict.CreateObj3DDictionary[sceneLoader.currentStage] != null)
            {
                for (int k = 0; k < sceneLoader.object3DDict.Count; k++)
                {
                    sceneLoader.object3DDict[k].SetActive(false);
                }
                if (XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList != null)
                {
                    for (int k = 0; k < XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList.Length; k++)
                    {
                        sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].transform.position = XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].Position.GetPos();
                        sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].transform.localRotation = XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].Rotation.GetRot();
                        sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].transform.localScale = XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].scale.GetScale();
                        sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].SetActive(true);
                        if (sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].GetComponent<Animator>() != null)
                        {
                            sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].GetComponent<Animator>().SetTrigger(XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].Anim);
                        }
                    }
                }
            }

            if (PlayerInfo.Instance.Gender == 0)
            {
                if (XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D != null)
                {
                    for (int k = 0; k < XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D.Length; k++)
                    {
                        for (int j = 0; j < sceneLoader.character2DDict[k].Count; j++)
                        {
                            if (sceneLoader.character2DDict[k][j].name == "CH_02")
                            {
                                sceneLoader.character2DDict[k][j].SetActive(false);
                            }
                        }
                    }
                }
            }
            else if (PlayerInfo.Instance.Gender == 1)
            {
                if (XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D != null)
                {
                    for (int k = 0; k < XML_Reader.Instance.scenarioToDict.CaptureEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D.Length; k++)
                    {
                        for (int j = 0; j < sceneLoader.character2DDict[k].Count; j++)
                        {
                            if (sceneLoader.character2DDict[k][j].name == "CH_01")
                            {
                                sceneLoader.character2DDict[k][j].SetActive(false);
                            }
                        }
                    }
                }
            }

            GameObject.Find("CaptureManager").GetComponent<TakeCapture>().TakeShotWithKids(GameObject.Find("CaptureManager").GetComponent<TakeCapture>().Kids, true);
        }
    }

    //사진 찍기 
    public void ShotEvent()
    {
        if(ARCore!=null)
        {
            ARCore.SetActive(true);

            for (int k = 0; k < sceneLoader.CanvasList.Count; k++)
            {
                if (sceneLoader.CanvasList[k].name == "CaptureCanvas" || sceneLoader.CanvasList[k].name == "PhotoCanvas")
                {
                    sceneLoader.CanvasList[k].transform.gameObject.SetActive(true);
                }
                else
                {
                    sceneLoader.CanvasList[k].transform.gameObject.SetActive(false);
                }
            }

            if (XML_Reader.Instance.scenarioToDict.CreateCharType2DDictionary[sceneLoader.currentStage] != null)
            {
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.CreateCharType2DDictionary[sceneLoader.currentStage].Count; k++)
                {
                    for (int j = 0; j < sceneLoader.character2DDict[k].Count; j++)
                    {
                        sceneLoader.character2DDict[k][j].SetActive(false);
                    }
                }
                if (XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D != null)
                {
                    for (int k = 0; k < XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D.Length; k++)
                    {
                        for (int j = 0; j < XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList.Length; j++)
                        {
                            sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().anchorMin = XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].anchorMin.GetAnchorMin();
                            sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().anchorMax = XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].anchorMax.GetAnchorMax();
                            sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().pivot = XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].pivot.GetPivot();
                            sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().offsetMin = XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].offsetMin.GetOffsetMin();
                            sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().offsetMax = XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].offsetMax.GetOffsetMax();
                            sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().anchoredPosition3D = new Vector3(sceneLoader.character2DDict[k][XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().anchoredPosition3D.x, sceneLoader.character2DDict[k][XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().anchoredPosition3D.y, 0.0f);
                            sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().localRotation = XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].Rotation.GetRot();
                            sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().localScale = XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].scale.GetScale();
                            sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].SetActive(true);
                            if (sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<Animator>() != null)
                            {
                                sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<Animator>().SetTrigger(XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].Anim);
                            }
                        }
                    }
                }
            }
            if (XML_Reader.Instance.scenarioToDict.CreateChar3DDictionary[sceneLoader.currentStage] != null)
            {
                for (int k = 0; k < sceneLoader.character3DDict.Count; k++)
                {
                    sceneLoader.character3DDict[k].SetActive(false);
                }
                if (XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList != null)
                {
                    for (int k = 0; k < XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList.Length; k++)
                    {
                        sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].transform.position = XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].Position.GetPos();
                        sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].transform.localRotation = XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].Rotation.GetRot();
                        sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].transform.localScale = XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].scale.GetScale();
                        sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].SetActive(true);
                        if (sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].GetComponent<Animator>() != null)
                        {
                            sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].GetComponent<Animator>().SetTrigger(XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].Anim);
                        }
                    }
                }
            }
            if (XML_Reader.Instance.scenarioToDict.CreateObjType2DDictionary[sceneLoader.currentStage] != null)
            {
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.CreateObjType2DDictionary[sceneLoader.currentStage].Count; k++)
                {
                    for (int j = 0; j < sceneLoader.object2DDict[k].Count; j++)
                    {
                        sceneLoader.object2DDict[k][j].SetActive(false);
                    }
                }
                if (XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D != null)
                {
                    for (int k = 0; k < XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D.Length; k++)
                    {
                        for (int j = 0; j < XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList.Length; j++)
                        {
                            sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().anchorMin = XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].anchorMin.GetAnchorMin();
                            sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().anchorMax = XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].anchorMax.GetAnchorMax();
                            sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().pivot = XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].pivot.GetPivot();
                            sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().offsetMin = XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].offsetMin.GetOffsetMin();
                            sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().offsetMax = XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].offsetMax.GetOffsetMax();
                            sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().anchoredPosition3D = new Vector3(sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().anchoredPosition3D.x, sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().anchoredPosition3D.y, 0.0f);
                            sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().localRotation = XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].Rotation.GetRot();
                            sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().localScale = XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].scale.GetScale();
                            sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].SetActive(true);
                            if (sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<Animator>() != null)
                            {
                                sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<Animator>().SetTrigger(XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].Anim);
                            }
                        }
                    }
                }
            }
            if (XML_Reader.Instance.scenarioToDict.CreateObj3DDictionary[sceneLoader.currentStage] != null)
            {
                for (int k = 0; k < sceneLoader.object3DDict.Count; k++)
                {
                    sceneLoader.object3DDict[k].SetActive(false);
                }
                if (XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList != null)
                {
                    for (int k = 0; k < XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList.Length; k++)
                    {
                        sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].transform.position = XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].Position.GetPos();
                        sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].transform.localRotation = XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].Rotation.GetRot();
                        sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].transform.localScale = XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].scale.GetScale();
                        sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].SetActive(true);
                        if (sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].GetComponent<Animator>() != null)
                        {
                            sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].GetComponent<Animator>().SetTrigger(XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].Anim);
                        }
                    }
                }
            }

            if (PlayerInfo.Instance.Gender == 0)
            {
                if (XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D != null)
                {
                    for (int k = 0; k < XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D.Length; k++)
                    {
                        for (int j = 0; j < sceneLoader.character2DDict[k].Count; j++)
                        {
                            if (sceneLoader.character2DDict[k][j].name == "CH_02")
                            {
                                sceneLoader.character2DDict[k][j].SetActive(false);
                            }
                        }
                    }
                }
            }
            else if (PlayerInfo.Instance.Gender == 1)
            {
                if (XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D != null)
                {
                    for (int k = 0; k < XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D.Length; k++)
                    {
                        for (int j = 0; j < sceneLoader.character2DDict[k].Count; j++)
                        {
                            if (sceneLoader.character2DDict[k][j].name == "CH_01")
                            {
                                sceneLoader.character2DDict[k][j].SetActive(false);
                            }
                        }
                    }
                }
            }

            GameObject.Find("CaptureManager").GetComponent<TakeCapture>().TakeShotWithKids(GameObject.Find("CaptureManager").GetComponent<TakeCapture>().Kids, true);
            if (Guide != null)
            {
                Guide.SetActive(true);
                Guide.GetComponent<Text>().text = XML_Reader.Instance.scenarioToDict.ShotEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].text;
            }
        }
    }

    public void PanelPopEvent()
    {
        for (int k = 0; k < sceneLoader.CanvasList.Count; k++)
        {
            for (int j = 0; j < XML_Reader.Instance.scenarioToDict.StageInfoDictionary[sceneLoader.currentStage].CanvasCount; j++)
            {
                if (k == j)
                {
                    sceneLoader.CanvasList[k].transform.gameObject.SetActive(true);
                    break;
                }
                sceneLoader.CanvasList[k].transform.gameObject.SetActive(false);
            }
        }

        if (XML_Reader.Instance.scenarioToDict.CreateCharType2DDictionary[sceneLoader.currentStage] != null)
        {
            for (int k = 0; k < XML_Reader.Instance.scenarioToDict.CreateCharType2DDictionary[sceneLoader.currentStage].Count; k++)
            {
                for (int j = 0; j < sceneLoader.character2DDict[k].Count; j++)
                {
                    sceneLoader.character2DDict[k][j].SetActive(false);
                }
            }
            if (XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D != null)
            {
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D.Length; k++)
                {
                    for (int j = 0; j < XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList.Length; j++)
                    {
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().anchorMin = XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].anchorMin.GetAnchorMin();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().anchorMax = XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].anchorMax.GetAnchorMax();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().pivot = XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].pivot.GetPivot();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().offsetMin = XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].offsetMin.GetOffsetMin();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().offsetMax = XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].offsetMax.GetOffsetMax();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().anchoredPosition3D = new Vector3(sceneLoader.character2DDict[k][XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().anchoredPosition3D.x, sceneLoader.character2DDict[k][XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().anchoredPosition3D.y, 0.0f);
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().localRotation = XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].Rotation.GetRot();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().localScale = XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].scale.GetScale();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].SetActive(true);
                        if (sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<Animator>() != null)
                        {
                            sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<Animator>().SetTrigger(XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].Anim);
                        }
                    }
                }
            }
        }
        if (XML_Reader.Instance.scenarioToDict.CreateChar3DDictionary[sceneLoader.currentStage] != null)
        {
            for (int k = 0; k < sceneLoader.character3DDict.Count; k++)
            {
                sceneLoader.character3DDict[k].SetActive(false);
            }
            if (XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList != null)
            {
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList.Length; k++)
                {
                    sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].transform.position = XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].Position.GetPos();
                    sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].transform.localRotation = XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].Rotation.GetRot();
                    sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].transform.localScale = XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].scale.GetScale();
                    sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].SetActive(true);
                    if (sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].GetComponent<Animator>() != null)
                    {
                        sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].GetComponent<Animator>().SetTrigger(XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].Anim);
                    }
                }
            }
        }
        if (XML_Reader.Instance.scenarioToDict.CreateObjType2DDictionary[sceneLoader.currentStage] != null)
        {
            for (int k = 0; k < XML_Reader.Instance.scenarioToDict.CreateObjType2DDictionary[sceneLoader.currentStage].Count; k++)
            {
                for (int j = 0; j < sceneLoader.object2DDict[k].Count; j++)
                {
                    sceneLoader.object2DDict[k][j].SetActive(false);
                }
            }
            if (XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D != null)
            {
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D.Length; k++)
                {
                    for (int j = 0; j < XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList.Length; j++)
                    {
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().anchorMin = XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].anchorMin.GetAnchorMin();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().anchorMax = XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].anchorMax.GetAnchorMax();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().pivot = XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].pivot.GetPivot();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().offsetMin = XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].offsetMin.GetOffsetMin();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().offsetMax = XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].offsetMax.GetOffsetMax();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().anchoredPosition3D = new Vector3(sceneLoader.object2DDict[k][XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().anchoredPosition3D.x, sceneLoader.object2DDict[k][XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().anchoredPosition3D.y, 0.0f);
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().localRotation = XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].Rotation.GetRot();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().localScale = XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].scale.GetScale();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].SetActive(true);
                        if (sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<Animator>() != null)
                        {
                            sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<Animator>().SetTrigger(XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].Anim);
                        }
                    }
                }
            }
        }
        if (XML_Reader.Instance.scenarioToDict.CreateObj3DDictionary[sceneLoader.currentStage] != null)
        {
            for (int k = 0; k < sceneLoader.object3DDict.Count; k++)
            {
                sceneLoader.object3DDict[k].SetActive(false);
            }
            if (XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList != null)
            {
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList.Length; k++)
                {
                    sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].transform.position = XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].Position.GetPos();
                    sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].transform.localRotation = XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].Rotation.GetRot();
                    sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].transform.localScale = XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].scale.GetScale();
                    sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].SetActive(true);
                    if (sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].GetComponent<Animator>() != null)
                    {
                        sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].GetComponent<Animator>().SetTrigger(XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].Anim);
                    }
                }
            }
        }

        if (PlayerInfo.Instance.Gender == 0)
        {
            if (XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D != null)
            {
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D.Length; k++)
                {
                    for (int j = 0; j < sceneLoader.character2DDict[k].Count; j++)
                    {
                        if (sceneLoader.character2DDict[k][j].name == "CH_02")
                        {
                            sceneLoader.character2DDict[k][j].SetActive(false);
                        }
                    }
                }
            }
        }
        else if (PlayerInfo.Instance.Gender == 1)
        {
            if (XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D != null)
            {
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D.Length; k++)
                {
                    for (int j = 0; j < sceneLoader.character2DDict[k].Count; j++)
                    {
                        if (sceneLoader.character2DDict[k][j].name == "CH_01")
                        {
                            sceneLoader.character2DDict[k][j].SetActive(false);
                        }
                    }
                }
            }
        }
        TextOutPut();
    }

    public void TableSettingEvent()
    {
        if (ARCore == null)
        {
            return;
        }

        for (int k = 0; k < sceneLoader.CanvasList.Count; k++)
        {
            for (int j = 0; j < XML_Reader.Instance.scenarioToDict.TableSettingEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D.Length; j++)
            {
                if (k == XML_Reader.Instance.scenarioToDict.TableSettingEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[j].ID)
                {
                    sceneLoader.CanvasList[k].transform.gameObject.SetActive(true);
                    break;
                }
                sceneLoader.CanvasList[k].transform.gameObject.SetActive(false);
            }
        }

        for (int k = 0; k < sceneLoader.CanvasList.Count; k++)
        {
            if (sceneLoader.CanvasList[k].name == "TableSettingCanvas")
            {
                sceneLoader.CanvasList[k].transform.gameObject.SetActive(true);
            }
        }

        m_Camera.gameObject.SetActive(false);
        
        ARCore.SetActive(true);
        
        ARController.SetActive(true);

        Light.SetActive(true);

        PlaneGenerator.SetActive(true);

        PointCloud.SetActive(true);

        PlaneDiscovery.SetActive(true);

        GameObject.FindGameObjectWithTag("4").SetActive(true);

    }

    public void SelectItemEvent()
    {
        Debug.Log("111");
        for (int k = 0; k < sceneLoader.CanvasList.Count; k++)
        {
            for (int j = 0; j < XML_Reader.Instance.scenarioToDict.StageInfoDictionary[sceneLoader.currentStage].CanvasCount; j++)
            {
                if (k == j)
                {
                    sceneLoader.CanvasList[k].transform.gameObject.SetActive(true);
                    break;
                }
                sceneLoader.CanvasList[k].transform.gameObject.SetActive(false);
            }
        }
        for(int k=0;k<sceneLoader.CanvasList.Count;k++)
        {
            if(sceneLoader.CanvasList[k].name==XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventType+"Canvas")
            {
                sceneLoader.CanvasList[k].gameObject.SetActive(true);
                for(int j=0;j<sceneLoader.CanvasList[k].transform.GetChild(0).childCount;j++)
                {
                    if (sceneLoader.CanvasList[k].transform.GetChild(0).GetChild(j).gameObject.GetComponent<Button>() != null)
                    {
                        sceneLoader.CanvasList[k].transform.GetChild(0).GetChild(j).gameObject.GetComponent<Button>().interactable = true;
                        if (sceneLoader.CanvasList[k].transform.GetChild(0).GetChild(j).childCount > 0)
                        {
                            sceneLoader.CanvasList[k].transform.GetChild(0).GetChild(j).GetChild(0).gameObject.SetActive(false);
                        }
                    }
                }
            }
        }
        
        if (XML_Reader.Instance.scenarioToDict.CreateCharType2DDictionary[sceneLoader.currentStage] != null)
        {
            for (int k = 0; k < XML_Reader.Instance.scenarioToDict.CreateCharType2DDictionary[sceneLoader.currentStage].Count; k++)
            {
                for (int j = 0; j < sceneLoader.character2DDict[k].Count; j++)
                {
                    sceneLoader.character2DDict[k][j].SetActive(false);
                }
            }
            if (XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D != null)
            {
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D.Length; k++)
                {
                    for (int j = 0; j < XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList.Length; j++)
                    {
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().anchorMin = XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].anchorMin.GetAnchorMin();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().anchorMax = XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].anchorMax.GetAnchorMax();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().pivot = XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].pivot.GetPivot();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().offsetMin = XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].offsetMin.GetOffsetMin();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().offsetMax = XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].offsetMax.GetOffsetMax();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().anchoredPosition3D = new Vector3(sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().anchoredPosition3D.x, sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().anchoredPosition3D.y, 0.0f);
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().localRotation = XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].Rotation.GetRot();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().localScale = XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].scale.GetScale();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].SetActive(true);
                        if (sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<Animator>() != null)
                        {
                            sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<Animator>().SetTrigger(XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].Anim);
                        }
                    }
                }
            }
        }
        if (XML_Reader.Instance.scenarioToDict.CreateChar3DDictionary[sceneLoader.currentStage] != null)
        {
            for (int k = 0; k < sceneLoader.character3DDict.Count; k++)
            {
                sceneLoader.character3DDict[k].SetActive(false);
            }
            if (XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList != null)
            {
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList.Length; k++)
                {
                    sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].transform.position = XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].Position.GetPos();
                    sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].transform.localRotation = XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].Rotation.GetRot();
                    sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].transform.localScale = XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].scale.GetScale();
                    sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].SetActive(true);
                    if (sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].GetComponent<Animator>() != null)
                    {
                        sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].GetComponent<Animator>().SetTrigger(XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].Anim);
                    }
                }
            }
        }
        if (XML_Reader.Instance.scenarioToDict.CreateObjType2DDictionary[sceneLoader.currentStage] != null)
        {
            for (int k = 0; k < XML_Reader.Instance.scenarioToDict.CreateObjType2DDictionary[sceneLoader.currentStage].Count; k++)
            {
                for (int j = 0; j < sceneLoader.object2DDict[k].Count; j++)
                {
                    sceneLoader.object2DDict[k][j].SetActive(false);
                }
            }
            if (XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D != null)
            {
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D.Length; k++)
                {
                    for (int j = 0; j < XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList.Length; j++)
                    {
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().anchorMin = XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].anchorMin.GetAnchorMin();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().anchorMax = XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].anchorMax.GetAnchorMax();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().pivot = XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].pivot.GetPivot();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().offsetMin = XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].offsetMin.GetOffsetMin();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().offsetMax = XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].offsetMax.GetOffsetMax();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().anchoredPosition3D = new Vector3(sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().anchoredPosition3D.x, sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().anchoredPosition3D.y, 0.0f);
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().localRotation = XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].Rotation.GetRot();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().localScale = XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].scale.GetScale();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].SetActive(true);
                        if (sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<Animator>() != null)
                        {
                            sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<Animator>().SetTrigger(XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].Anim);
                        }
                    }
                }
            }
        }
        if (XML_Reader.Instance.scenarioToDict.CreateObj3DDictionary[sceneLoader.currentStage] != null)
        {
            for (int k = 0; k < sceneLoader.object3DDict.Count; k++)
            {
                sceneLoader.object3DDict[k].SetActive(false);
            }
            if (XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList != null)
            {
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList.Length; k++)
                {
                    sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].transform.position = XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].Position.GetPos();
                    sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].transform.localRotation = XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].Rotation.GetRot();
                    sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].transform.localScale = XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].scale.GetScale();
                    sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].SetActive(true);
                    if (sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].GetComponent<Animator>() != null)
                    {
                        sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].GetComponent<Animator>().SetTrigger(XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].Anim);
                    }
                }
            }
        }

        TextOutPut();
    }

    //입력 이벤트
    public void InputFieldEvent()
    {
        for(int k=0;k<sceneLoader.CanvasList.Count;k++)
        {
            for(int j=0;j<XML_Reader.Instance.scenarioToDict.StageInfoDictionary[sceneLoader.currentStage].CanvasCount;j++)
            {
                if(k==j)
                {
                    sceneLoader.CanvasList[k].transform.gameObject.SetActive(true);
                    break;
                }
                sceneLoader.CanvasList[k].transform.gameObject.SetActive(false);
            }
        }

        if(XML_Reader.Instance.scenarioToDict.CreateCharType2DDictionary[sceneLoader.currentStage]!=null)
        {
            for(int k=0;k<XML_Reader.Instance.scenarioToDict.CreateCharType2DDictionary[sceneLoader.currentStage].Count;k++)
            {
                for(int j=0;j<sceneLoader.character2DDict[k].Count;j++)
                {
                    sceneLoader.character2DDict[k][j].SetActive(false);
                }
            }
            if(XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D!=null)
            {
                for(int k=0;k<XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D.Length;k++)
                {
                    for(int j=0;j<XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList.Length;j++)
                    {
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().anchorMin = XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].anchorMin.GetAnchorMin();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().anchorMax = XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].anchorMax.GetAnchorMax();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().pivot = XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].pivot.GetPivot();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().offsetMin = XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].offsetMin.GetOffsetMin();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().offsetMax = XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].offsetMax.GetOffsetMax();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().anchoredPosition3D = new Vector3(sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().anchoredPosition3D.x, sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().anchoredPosition3D.y, 0);
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().localRotation = XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].Rotation.GetRot();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().localScale = XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].scale.GetScale();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].SetActive(true);
                        if (sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<Animator>()!=null)
                        {
                            sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<Animator>().SetTrigger(XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].Anim);
                        }
                    }
                }
            }
        }
        if(XML_Reader.Instance.scenarioToDict.CreateChar3DDictionary[sceneLoader.currentStage]!=null)
        {
            for(int k=0;k<sceneLoader.character3DDict.Count;k++)
            {
                sceneLoader.character3DDict[k].SetActive(false);
            }
            if (XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList!=null)
            {
                for(int k=0;k<XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList.Length;k++)
                {
                    sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].transform.position = XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].Position.GetPos();
                    sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].transform.localRotation = XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].Rotation.GetRot();
                    sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].transform.localScale = XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].scale.GetScale();
                    sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].SetActive(true);
                    if(sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].GetComponent<Animator>()!=null)
                    {
                        sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].GetComponent<Animator>().SetTrigger(XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].Anim);
                    }
                }
            }
        }
        if(XML_Reader.Instance.scenarioToDict.CreateObjType2DDictionary[sceneLoader.currentStage]!=null)
        {
            for (int k = 0; k < XML_Reader.Instance.scenarioToDict.CreateObjType2DDictionary[sceneLoader.currentStage].Count; k++)
            {
                for (int j = 0; j < sceneLoader.object2DDict[k].Count; j++)
                {
                    sceneLoader.object2DDict[k][j].SetActive(false);
                }
            }
            if (XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D != null)
            {
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D.Length; k++)
                {
                    for (int j = 0; j < XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList.Length; j++)
                    {
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().anchorMin = XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].anchorMin.GetAnchorMin();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().anchorMax = XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].anchorMax.GetAnchorMax();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().pivot = XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].pivot.GetPivot();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().offsetMin = XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].offsetMin.GetOffsetMin();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().offsetMax = XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].offsetMax.GetOffsetMax();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().anchoredPosition3D = new Vector3(sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().anchoredPosition3D.x, sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().anchoredPosition3D.y, 0);
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().localRotation = XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].Rotation.GetRot();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().localScale = XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].scale.GetScale();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].SetActive(true);
                        if (sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<Animator>() != null)
                        {
                            sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<Animator>().SetTrigger(XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].Anim);
                        }
                    }
                }
            }
        }
        if(XML_Reader.Instance.scenarioToDict.CreateObj3DDictionary[sceneLoader.currentStage]!=null)
        {
            for (int k = 0; k < sceneLoader.object3DDict.Count; k++)
            {
                sceneLoader.object3DDict[k].SetActive(false);
            }
            if (XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList != null)
            {
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList.Length; k++)
                {
                    sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].transform.position = XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].Position.GetPos();
                    sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].transform.localRotation = XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].Rotation.GetRot();
                    sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].transform.localScale = XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].scale.GetScale();
                    sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].SetActive(true);
                    if (sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].GetComponent<Animator>() != null)
                    {
                        sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].GetComponent<Animator>().SetTrigger(XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].Anim);
                    }
                }
            }
        }

        if (PlayerInfo.Instance.Gender == 0)
        {
            if (XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D != null)
            {
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D.Length; k++)
                {
                    for (int j = 0; j < sceneLoader.character2DDict[k].Count; j++)
                    {
                        if (sceneLoader.character2DDict[k][j].name == "CH_02")
                        {
                            sceneLoader.character2DDict[k][j].SetActive(false);
                        }
                    }
                }
            }
        }
        else if (PlayerInfo.Instance.Gender == 1)
        {
            if (XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D != null)
            {
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D.Length; k++)
                {
                    for (int j = 0; j < sceneLoader.character2DDict[k].Count; j++)
                    {
                        if (sceneLoader.character2DDict[k][j].name == "CH_01")
                        {
                            sceneLoader.character2DDict[k][j].SetActive(false);
                        }
                    }
                }
            }
        }

        TextOutPut();
    }

    //동영상 재생
    public void VideoPlayEvent()
    {
        for (int k = 0; k < sceneLoader.CanvasList.Count; k++)
        {
            for (int j = 0; j < XML_Reader.Instance.scenarioToDict.StageInfoDictionary[sceneLoader.currentStage].CanvasCount; j++)
            {
                if (k == j)
                {
                    sceneLoader.CanvasList[k].transform.gameObject.SetActive(true);
                    break;
                }
                sceneLoader.CanvasList[k].transform.gameObject.SetActive(false);
            }
        }

        if (XML_Reader.Instance.scenarioToDict.CreateCharType2DDictionary[sceneLoader.currentStage] != null)
        {
            for (int k = 0; k < XML_Reader.Instance.scenarioToDict.CreateCharType2DDictionary[sceneLoader.currentStage].Count; k++)
            {
                for (int j = 0; j < sceneLoader.character2DDict[k].Count; j++)
                {
                    sceneLoader.character2DDict[k][j].SetActive(false);
                }
            }
            if (XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D != null)
            {
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D.Length; k++)
                {
                    for (int j = 0; j < XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList.Length; j++)
                    {
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().anchorMin = XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].anchorMin.GetAnchorMin();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().anchorMax = XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].anchorMax.GetAnchorMax();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().pivot = XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].pivot.GetPivot();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().offsetMin = XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].offsetMin.GetOffsetMin();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().offsetMax = XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].offsetMax.GetOffsetMax();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().anchoredPosition3D = new Vector3(sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().anchoredPosition3D.x, sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().anchoredPosition3D.y, 0);
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().localRotation = XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].Rotation.GetRot();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().localScale = XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].scale.GetScale();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].SetActive(true);
                        if (sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<Animator>() != null)
                        {
                            sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<Animator>().SetTrigger(XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].Anim);
                        }
                    }
                }
            }
        }
        if (XML_Reader.Instance.scenarioToDict.CreateChar3DDictionary[sceneLoader.currentStage] != null)
        {
            for (int k = 0; k < sceneLoader.character3DDict.Count; k++)
            {
                sceneLoader.character3DDict[k].SetActive(false);
            }
            if (XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList != null)
            {
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList.Length; k++)
                {
                    sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].transform.position = XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].Position.GetPos();
                    sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].transform.localRotation = XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].Rotation.GetRot();
                    sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].transform.localScale = XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].scale.GetScale();
                    sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].SetActive(true);
                    if (sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].GetComponent<Animator>() != null)
                    {
                        sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].GetComponent<Animator>().SetTrigger(XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].Anim);
                    }
                }
            }
        }
        if (XML_Reader.Instance.scenarioToDict.CreateObjType2DDictionary[sceneLoader.currentStage] != null)
        {
            for (int k = 0; k < XML_Reader.Instance.scenarioToDict.CreateObjType2DDictionary[sceneLoader.currentStage].Count; k++)
            {
                for (int j = 0; j < sceneLoader.object2DDict[k].Count; j++)
                {
                    sceneLoader.object2DDict[k][j].SetActive(false);
                }
            }
            if (XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D != null)
            {
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D.Length; k++)
                {
                    for (int j = 0; j < XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList.Length; j++)
                    {
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().anchorMin = XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].anchorMin.GetAnchorMin();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().anchorMax = XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].anchorMax.GetAnchorMax();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().pivot = XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].pivot.GetPivot();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().offsetMin = XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].offsetMin.GetOffsetMin();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().offsetMax = XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].offsetMax.GetOffsetMax();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().anchoredPosition3D = new Vector3(sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().anchoredPosition3D.x, sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().anchoredPosition3D.y, 0);
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().localRotation = XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].Rotation.GetRot();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().localScale = XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].scale.GetScale();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].SetActive(true);
                        if (sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<Animator>() != null)
                        {
                            sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<Animator>().SetTrigger(XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].Anim);
                        }
                    }
                }
            }
        }
        if (XML_Reader.Instance.scenarioToDict.CreateObj3DDictionary[sceneLoader.currentStage] != null)
        {
            for (int k = 0; k < sceneLoader.object3DDict.Count; k++)
            {
                sceneLoader.object3DDict[k].SetActive(false);
            }
            if (XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList != null)
            {
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList.Length; k++)
                {
                    sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].transform.position = XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].Position.GetPos();
                    sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].transform.localRotation = XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].Rotation.GetRot();
                    sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].transform.localScale = XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].scale.GetScale();
                    sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].SetActive(true);
                    if (sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].GetComponent<Animator>() != null)
                    {
                        sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].GetComponent<Animator>().SetTrigger(XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].Anim);
                    }
                }
            }
        }

        //VideoPlayer player = FindObjectOfType<VideoPlayer>();
        if (player != null)
        {
            for (int k = 0; k < XML_Reader.Instance.scenarioToDict.StageInfoDictionary[sceneLoader.currentStage].objectList.ObjType2D.Length; k++)
            {
                for (int j = 0; j < sceneLoader.object2DDict[k].Count; j++)
                {
                    if (sceneLoader.object2DDict[k][j].tag == "VideoTexture")
                    {
                        if (sceneLoader.object2DDict[k][j].activeSelf == true)
                        {
                            player.targetTexture = (RenderTexture)sceneLoader.object2DDict[k][j].GetComponent<RawImage>().texture;
                            player.url = XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].Clip;
                            player.time = 0.0f;
                            player.Play();
                        }
                    }
                }
            }
        }

        if (PlayerInfo.Instance.Gender == 0)
        {
            if (XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D != null)
            {
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D.Length; k++)
                {
                    for (int j = 0; j < sceneLoader.character2DDict[k].Count; j++)
                    {
                        if (sceneLoader.character2DDict[k][j].name == "CH_02")
                        {
                            sceneLoader.character2DDict[k][j].SetActive(false);
                        }
                    }
                }
            }
        }
        else if (PlayerInfo.Instance.Gender == 1)
        {
            if (XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D != null)
            {
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D.Length; k++)
                {
                    for (int j = 0; j < sceneLoader.character2DDict[k].Count; j++)
                    {
                        if (sceneLoader.character2DDict[k][j].name == "CH_01")
                        {
                            sceneLoader.character2DDict[k][j].SetActive(false);
                        }
                    }
                }
            }
        }
        TextOutPut();
    }

    //쓰레기 치우기 미니게임
    public void TouchGameEvent()
    {
        if(MiniGameObj3DDict.Count!=0)
        {
            MiniGameObj3DDict.Clear();
        }
        GameCount = 0;
        int temp = 0;
        float PosX = 0;
        float PosY = 0;
        float PosZ = -5;
        float RotX = 0;
        float RotY = 0;
        float RotZ = 0;

        GameCount = Random.Range(XML_Reader.Instance.scenarioToDict.TouchGameEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].count.min, XML_Reader.Instance.scenarioToDict.TouchGameEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].count.max);
        for(int k=0;k<sceneLoader.CanvasList.Count;k++)
        {
            for(int j=0;j<XML_Reader.Instance.scenarioToDict.StageInfoDictionary[sceneLoader.currentStage].CanvasCount;j++)
            {
                if(k==j)
                {
                    sceneLoader.CanvasList[k].transform.gameObject.SetActive(true);
                    break;
                }
                sceneLoader.CanvasList[k].transform.gameObject.SetActive(false);
            }
        }

        if (XML_Reader.Instance.scenarioToDict.CreateCharType2DDictionary[sceneLoader.currentStage] != null)
        {
            for (int k = 0; k < XML_Reader.Instance.scenarioToDict.CreateCharType2DDictionary[sceneLoader.currentStage].Count; k++)
            {
                for (int j = 0; j < sceneLoader.character2DDict[k].Count; j++)
                {
                    sceneLoader.character2DDict[k][j].SetActive(false);
                }
            }            
        }
        if (XML_Reader.Instance.scenarioToDict.CreateChar3DDictionary[sceneLoader.currentStage] != null)
        {
            for (int k = 0; k < sceneLoader.character3DDict.Count; k++)
            {
                sceneLoader.character3DDict[k].SetActive(false);
            }            
        }
        if (XML_Reader.Instance.scenarioToDict.CreateObjType2DDictionary[sceneLoader.currentStage]!=null)
        {
            for(int k=0;k<XML_Reader.Instance.scenarioToDict.CreateObjType2DDictionary[sceneLoader.currentStage].Count;k++)
            {
                for(int j=0;j<sceneLoader.object2DDict[k].Count;j++)
                {
                    sceneLoader.object2DDict[k][j].SetActive(false);
                }
            }

            if (XML_Reader.Instance.scenarioToDict.TouchGameEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D!=null)
            {
                for(int k=0;k<XML_Reader.Instance.scenarioToDict.TouchGameEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D.Length;k++)
                {
                    for(int j=0; j<XML_Reader.Instance.scenarioToDict.TouchGameEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList.Length;j++)
                    {
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.TouchGameEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.TouchGameEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().anchorMin = XML_Reader.Instance.scenarioToDict.TouchGameEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].anchorMin.GetAnchorMin();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.TouchGameEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.TouchGameEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().anchorMax = XML_Reader.Instance.scenarioToDict.TouchGameEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].anchorMax.GetAnchorMax();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.TouchGameEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.TouchGameEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().pivot = XML_Reader.Instance.scenarioToDict.TouchGameEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].pivot.GetPivot();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.TouchGameEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.TouchGameEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().offsetMin = XML_Reader.Instance.scenarioToDict.TouchGameEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].offsetMin.GetOffsetMin();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.TouchGameEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.TouchGameEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().offsetMax = XML_Reader.Instance.scenarioToDict.TouchGameEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].offsetMax.GetOffsetMax();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.TouchGameEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.TouchGameEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().anchoredPosition3D = new Vector3(sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.TouchGameEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.TouchGameEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().anchoredPosition3D.x, sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.TouchGameEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.TouchGameEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().anchoredPosition3D.y, 0);
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.TouchGameEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.TouchGameEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().localRotation = XML_Reader.Instance.scenarioToDict.TouchGameEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].Rotation.GetRot();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.TouchGameEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.TouchGameEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().localScale = XML_Reader.Instance.scenarioToDict.TouchGameEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].scale.GetScale();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.TouchGameEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.TouchGameEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].SetActive(true);
                        if (sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.TouchGameEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.TouchGameEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<Animator>() != null)
                        {
                            sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.TouchGameEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.TouchGameEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<Animator>().SetTrigger(XML_Reader.Instance.scenarioToDict.TouchGameEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].Anim);
                        }
                    }
                }
            }
        }
        if (XML_Reader.Instance.scenarioToDict.CreateObj3DDictionary[sceneLoader.currentStage] != null)
        {
            for (int k = 0; k < sceneLoader.object3DDict.Count; k++)
            {
                sceneLoader.object3DDict[k].SetActive(false);
            }            
        }
        //3D 쓰레기
        if (XML_Reader.Instance.scenarioToDict.TouchGameEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].CreateObjList.ObjType3D.CreateObjList.Length>0)
        {
            for(int k=0;k<GameCount;k++)
            {
                //쓰레기 종류 결정
                temp = Random.Range(0, XML_Reader.Instance.scenarioToDict.TouchGameEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].CreateObjList.ObjType3D.CreateObjList.Length);
                //좌표 잡기
                PosX = Random.Range(-4.0f, 4.0f);
                PosY = Random.Range(-1.0f, 3.0f);
                RotX = Random.Range(0.0f, 360.0f);
                RotY = Random.Range(0.0f, 360.0f);
                RotZ = Random.Range(0.0f, 360.0f);

                GameObject TrashObj = Resources.Load(XML_Reader.Instance.scenarioToDict.TouchGameEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].CreateObjList.ObjType3D.CreateObjList[temp].Name) as GameObject;

                GameObject prefab = null;

                prefab = TrashObj;

                if(prefab!=null)
                {
                    GameObject obj = Instantiate(prefab) as GameObject;

                    obj.name = XML_Reader.Instance.scenarioToDict.TouchGameEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].CreateObjList.ObjType3D.CreateObjList[temp].Name+k;

                    obj.transform.position = new Vector3(PosX, PosY, PosZ);
                    obj.transform.localRotation = Quaternion.Euler(new Vector3(RotX, RotY, RotZ));
                    obj.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

                    MiniGameObj3DDict.Add(k, obj);
                }
            }
        }

        TextOutPut();
    }

    public void DeleteBlurEvent()
    {
        for (int k = 0; k < sceneLoader.CanvasList.Count; k++)
        {
            for (int j = 0; j < XML_Reader.Instance.scenarioToDict.StageInfoDictionary[sceneLoader.currentStage].CanvasCount; j++)
            {
                if (k == j)
                {
                    sceneLoader.CanvasList[k].transform.gameObject.SetActive(true);
                    break;
                }
                sceneLoader.CanvasList[k].transform.gameObject.SetActive(false);
            }
        }

        if (XML_Reader.Instance.scenarioToDict.CreateCharType2DDictionary[sceneLoader.currentStage] != null)
        {
            for (int k = 0; k < XML_Reader.Instance.scenarioToDict.CreateCharType2DDictionary[sceneLoader.currentStage].Count; k++)
            {
                for (int j = 0; j < sceneLoader.character2DDict[k].Count; j++)
                {
                    sceneLoader.character2DDict[k][j].SetActive(false);
                }
            }
            if (XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D != null)
            {
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D.Length; k++)
                {
                    for (int j = 0; j < XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList.Length; j++)
                    {
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().anchorMin = XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].anchorMin.GetAnchorMin();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().anchorMax = XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].anchorMax.GetAnchorMax();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().pivot = XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].pivot.GetPivot();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().offsetMin = XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].offsetMin.GetOffsetMin();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().offsetMax = XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].offsetMax.GetOffsetMax();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().anchoredPosition3D = new Vector3(sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().anchoredPosition3D.x, sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().anchoredPosition3D.y, 0.0f);
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().localRotation = XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].Rotation.GetRot();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<RectTransform>().localScale = XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].scale.GetScale();
                        sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].SetActive(true);
                        if (sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<Animator>() != null)
                        {
                            sceneLoader.character2DDict[XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].ID][XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].ID].GetComponent<Animator>().SetTrigger(XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D[k].InitCharList[j].Anim);
                        }
                    }
                }
            }
        }
        if (XML_Reader.Instance.scenarioToDict.CreateChar3DDictionary[sceneLoader.currentStage] != null)
        {
            for (int k = 0; k < sceneLoader.character3DDict.Count; k++)
            {
                sceneLoader.character3DDict[k].SetActive(false);
            }
            if (XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList != null)
            {
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList.Length; k++)
                {
                    sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].transform.position = XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].Position.GetPos();
                    sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].transform.localRotation = XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].Rotation.GetRot();
                    sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].transform.localScale = XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].scale.GetScale();
                    sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].SetActive(true);
                    if (sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].GetComponent<Animator>() != null)
                    {
                        sceneLoader.character3DDict[XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].ID].GetComponent<Animator>().SetTrigger(XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType3D.InitCharList[k].Anim);
                    }
                }
            }
        }
        if (XML_Reader.Instance.scenarioToDict.CreateObjType2DDictionary[sceneLoader.currentStage] != null)
        {
            for (int k = 0; k < XML_Reader.Instance.scenarioToDict.CreateObjType2DDictionary[sceneLoader.currentStage].Count; k++)
            {
                for (int j = 0; j < sceneLoader.object2DDict[k].Count; j++)
                {
                    sceneLoader.object2DDict[k][j].SetActive(false);
                }
            }
            if (XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D != null)
            {
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D.Length; k++)
                {
                    for (int j = 0; j < XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList.Length; j++)
                    {
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().anchorMin = XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].anchorMin.GetAnchorMin();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().anchorMax = XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].anchorMax.GetAnchorMax();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().pivot = XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].pivot.GetPivot();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().offsetMin = XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].offsetMin.GetOffsetMin();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().offsetMax = XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].offsetMax.GetOffsetMax();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().anchoredPosition3D = new Vector3(sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().anchoredPosition3D.x, sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().anchoredPosition3D.y, 0.0f);
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().localRotation = XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].Rotation.GetRot();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<RectTransform>().localScale = XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].scale.GetScale();
                        sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].SetActive(true);
                        if (sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<Animator>() != null)
                        {
                            sceneLoader.object2DDict[XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].ID][XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].ID].GetComponent<Animator>().SetTrigger(XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType2D[k].InitObjList[j].Anim);
                        }
                    }
                }
            }
        }
        if (XML_Reader.Instance.scenarioToDict.CreateObj3DDictionary[sceneLoader.currentStage] != null)
        {
            for (int k = 0; k < sceneLoader.object3DDict.Count; k++)
            {
                sceneLoader.object3DDict[k].SetActive(false);
            }
            if (XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList != null)
            {
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList.Length; k++)
                {
                    sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].transform.position = XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].Position.GetPos();
                    sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].transform.localRotation = XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].Rotation.GetRot();
                    sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].transform.localScale = XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].scale.GetScale();
                    sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].SetActive(true);
                    if (sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].GetComponent<Animator>() != null)
                    {
                        sceneLoader.object3DDict[XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].ID].GetComponent<Animator>().SetTrigger(XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].objList.ObjType3D.InitObjList[k].Anim);
                    }
                }
            }
        }

        if (PlayerInfo.Instance.Gender == 0)
        {
            if (XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D != null)
            {
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D.Length; k++)
                {
                    for (int j = 0; j < sceneLoader.character2DDict[k].Count; j++)
                    {
                        if (sceneLoader.character2DDict[k][j].name == "CH_02")
                        {
                            sceneLoader.character2DDict[k][j].SetActive(false);
                        }
                    }
                }
            }
        }
        else if (PlayerInfo.Instance.Gender == 1)
        {
            if (XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D != null)
            {
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].charList.CharType2D.Length; k++)
                {
                    for (int j = 0; j < sceneLoader.character2DDict[k].Count; j++)
                    {
                        if (sceneLoader.character2DDict[k][j].name == "CH_01")
                        {
                            sceneLoader.character2DDict[k][j].SetActive(false);
                        }
                    }
                }
            }
        }

        TextOutPut();
    }

    public void PaintEvent()
    {
        for(int k=0;k<sceneLoader.CanvasList.Count;k++)
        {
            sceneLoader.CanvasList[k].transform.gameObject.SetActive(false);
        }

        for(int k=0;k<sceneLoader.CanvasList.Count;k++)
        {
            if(sceneLoader.CanvasList[k].name==XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventType+"UICanvas")
            {
                sceneLoader.CanvasList[k].gameObject.SetActive(true);
            }            
        }

        if (XML_Reader.Instance.scenarioToDict.CreateCharType2DDictionary[sceneLoader.currentStage] != null)
        {
            for (int k = 0; k < XML_Reader.Instance.scenarioToDict.CreateCharType2DDictionary[sceneLoader.currentStage].Count; k++)
            {
                for (int j = 0; j < sceneLoader.character2DDict[k].Count; j++)
                {
                    sceneLoader.character2DDict[k][j].SetActive(false);
                }
            }          
        }
        if (XML_Reader.Instance.scenarioToDict.CreateChar3DDictionary[sceneLoader.currentStage] != null)
        {
            for (int k = 0; k < sceneLoader.character3DDict.Count; k++)
            {
                sceneLoader.character3DDict[k].SetActive(false);
            }           
        }
        if (XML_Reader.Instance.scenarioToDict.CreateObjType2DDictionary[sceneLoader.currentStage] != null)
        {
            for (int k = 0; k < XML_Reader.Instance.scenarioToDict.CreateObjType2DDictionary[sceneLoader.currentStage].Count; k++)
            {
                for (int j = 0; j < sceneLoader.object2DDict[k].Count; j++)
                {
                    sceneLoader.object2DDict[k][j].SetActive(false);
                }
            }            
        }
        if (XML_Reader.Instance.scenarioToDict.CreateObj3DDictionary[sceneLoader.currentStage] != null)
        {
            for (int k = 0; k < sceneLoader.object3DDict.Count; k++)
            {
                sceneLoader.object3DDict[k].SetActive(false);
            }            
        }

        for (int k=0;k<sceneLoader.object3DDict.Count;k++)
        {
            if(sceneLoader.object3DDict[k].name==XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventType+ "DrawingPlane")
            {
                sceneLoader.object3DDict[k].SetActive(true);
            }
        }
    }

    public void FindGameEvent()
    {
        if(ARCore!=null)
        {
            ARCore.SetActive(true);

            for(int k=0;k<sceneLoader.CanvasList.Count;k++)
            {
                if(sceneLoader.CanvasList[k].name== "FindGameCanvas")
                {
                    sceneLoader.CanvasList[k].gameObject.SetActive(true);
                }
                else
                {
                    sceneLoader.CanvasList[k].gameObject.SetActive(false);
                }
            }
            m_VR_Manager.VRModeStart();
        }
    }
    
    public void TextOutPut()
    {
        string Contents = string.Empty;
        bool trigger = false;
        switch (XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventType)
        {
            case "Text":
                Contents = XML_Reader.Instance.scenarioToDict.TextEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].text;
                break;
            case "PopUp":
                Contents = XML_Reader.Instance.scenarioToDict.PopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].text;
                break;
            case "CardPopUp":
                Contents = XML_Reader.Instance.scenarioToDict.CardPopUpEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].text;
                break;
            case "SphereImageOn":
                Contents = XML_Reader.Instance.scenarioToDict.SphereImageOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].text;
                break;
            case "SpeechBubbleOn":
                Contents = XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].text;
                break;
            case "Quarter":
                Contents = XML_Reader.Instance.scenarioToDict.QuarterEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].text;
                break;
            case "PanelPop":
                Contents = XML_Reader.Instance.scenarioToDict.PanelPopEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].text;
                break;
            case "SelectItem":
                Contents = XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].text;
                break;
            case "InputField":
                Contents=XML_Reader.Instance.scenarioToDict.InputFieldEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].text;
                break;
            case "VideoPlay":
                Contents = XML_Reader.Instance.scenarioToDict.VideoPlayEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].text;
                break;
            case "TouchGame":
                Contents = XML_Reader.Instance.scenarioToDict.TouchGameEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].Text;
                break;
            case "DeleteBlur":
                Contents = XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].Text;
                break;
            default:
                break;
        }

        //텍스트 위치 지정
        switch (XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventType)
        {            
            case "SphereImageOn":
            case "InputField":
            case "VideoPlay":
            case "Text":
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.CreateObjType2DDictionary[sceneLoader.currentStage].Count; k++)
                {
                    for (int j = 0; j < sceneLoader.object2DDict[k].Count; j++)
                    {
                        if (sceneLoader.object2DDict[k][j].tag == "TextBox")
                        {
                            if (sceneLoader.object2DDict[k][j].activeSelf == true)
                            {
                                text = sceneLoader.object2DDict[k][j].transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
                                //20191122 추가사항
                                for (int q = 0; q < sceneLoader.object2DDict[k][j].transform.childCount; q++)
                                {
                                    for (int w = 0; w < sceneLoader.object2DDict[k][j].transform.GetChild(q).transform.childCount; w++)
                                    {
                                        if (sceneLoader.object2DDict[k][j].transform.GetChild(q).GetChild(w).GetComponent<Scrollbar>() != null)
                                        {
                                            m_Scrollbar = sceneLoader.object2DDict[k][j].transform.GetChild(q).GetChild(w).GetComponent<Scrollbar>();
                                        }
                                    }
                                }
                            }                            
                        }
                    }
                }
                break;
            case "PopUp":
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.CreateObjType2DDictionary[sceneLoader.currentStage].Count; k++)
                {
                    for (int j = 0; j < sceneLoader.object2DDict[k].Count; j++)
                    {
                        if (sceneLoader.object2DDict[k][j].tag == "PopUp")
                        {
                            if (sceneLoader.object2DDict[k][j].activeSelf == true)
                            {
                                text = sceneLoader.object2DDict[k][j].transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
                                //20191122 추가사항
                                for (int q = 0; q < sceneLoader.object2DDict[k][j].transform.childCount; q++)
                                {
                                    for (int w = 0; w < sceneLoader.object2DDict[k][j].transform.GetChild(q).transform.childCount; w++)
                                    {
                                        if (sceneLoader.object2DDict[k][j].transform.GetChild(q).GetChild(w).GetComponent<Scrollbar>() != null)
                                        {
                                            m_Scrollbar = sceneLoader.object2DDict[k][j].transform.GetChild(q).GetChild(w).GetComponent<Scrollbar>();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                break;
            case "CardPopUp":
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.CreateObjType2DDictionary[sceneLoader.currentStage].Count; k++)
                {
                    for (int j = 0; j < sceneLoader.object2DDict[k].Count; j++)
                    {
                        if (sceneLoader.object2DDict[k][j].tag == "CardTextBox")
                        {
                            if (sceneLoader.object2DDict[k][j].activeSelf == true)
                            {
                                text = sceneLoader.object2DDict[k][j].transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
                                //20191122 추가사항
                                for (int q = 0; q < sceneLoader.object2DDict[k][j].transform.childCount; q++)
                                {
                                    for (int w = 0; w < sceneLoader.object2DDict[k][j].transform.GetChild(q).transform.childCount; w++)
                                    {
                                        if (sceneLoader.object2DDict[k][j].transform.GetChild(q).GetChild(w).GetComponent<Scrollbar>() != null)
                                        {
                                            m_Scrollbar = sceneLoader.object2DDict[k][j].transform.GetChild(q).GetChild(w).GetComponent<Scrollbar>();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                break;
            case "SpeechBubbleOn":
                switch (XML_Reader.Instance.scenarioToDict.SpeechBubbleOnEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].Pos)
                {
                    case "Left":
                        for (int k = 0; k < XML_Reader.Instance.scenarioToDict.CreateObjType2DDictionary[sceneLoader.currentStage].Count; k++)
                        {
                            for (int j = 0; j < sceneLoader.object2DDict[k].Count; j++)
                            {
                                if (sceneLoader.object2DDict[k][j].tag == "Left")
                                {
                                    if (sceneLoader.object2DDict[k][j].activeSelf == true)
                                    {
                                        text = sceneLoader.object2DDict[k][j].transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
                                        //20191122 추가사항
                                        for (int q = 0; q < sceneLoader.object2DDict[k][j].transform.childCount; q++)
                                        {
                                            for (int w = 0; w < sceneLoader.object2DDict[k][j].transform.GetChild(q).transform.childCount; w++)
                                            {
                                                if (sceneLoader.object2DDict[k][j].transform.GetChild(q).GetChild(w).GetComponent<Scrollbar>() != null)
                                                {
                                                    m_Scrollbar = sceneLoader.object2DDict[k][j].transform.GetChild(q).GetChild(w).GetComponent<Scrollbar>();
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case "Right":
                        for (int k = 0; k < XML_Reader.Instance.scenarioToDict.CreateObjType2DDictionary[sceneLoader.currentStage].Count; k++)
                        {
                            for (int j = 0; j < sceneLoader.object2DDict[k].Count; j++)
                            {
                                if (sceneLoader.object2DDict[k][j].tag == "Right")
                                {
                                    if (sceneLoader.object2DDict[k][j].activeSelf == true)
                                    {
                                        text = sceneLoader.object2DDict[k][j].transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
                                        //20191122 추가사항
                                        for (int q = 0; q < sceneLoader.object2DDict[k][j].transform.childCount; q++)
                                        {
                                            for (int w = 0; w < sceneLoader.object2DDict[k][j].transform.GetChild(q).transform.childCount; w++)
                                            {
                                                if (sceneLoader.object2DDict[k][j].transform.GetChild(q).GetChild(w).GetComponent<Scrollbar>() != null)
                                                {
                                                    m_Scrollbar = sceneLoader.object2DDict[k][j].transform.GetChild(q).GetChild(w).GetComponent<Scrollbar>();
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;                    
                }
                break;
            case "PanelPop":
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.CreateObjType2DDictionary[sceneLoader.currentStage].Count; k++)
                {
                    for (int j = 0; j < sceneLoader.object2DDict[k].Count; j++)
                    {
                        if (sceneLoader.object2DDict[k][j].tag == "System_PopUp")
                        {
                            if (sceneLoader.object2DDict[k][j].activeSelf == true)
                            {
                                text = sceneLoader.object2DDict[k][j].transform.GetChild(0).GetComponent<Text>();
                                //20191122 추가사항
                                for (int q = 0; q < sceneLoader.object2DDict[k][j].transform.childCount; q++)
                                {
                                    for (int w = 0; w < sceneLoader.object2DDict[k][j].transform.GetChild(q).transform.childCount; w++)
                                    {
                                        if (sceneLoader.object2DDict[k][j].transform.GetChild(q).GetChild(w).GetComponent<Scrollbar>() != null)
                                        {
                                            m_Scrollbar = sceneLoader.object2DDict[k][j].transform.GetChild(q).GetChild(w).GetComponent<Scrollbar>();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                break;
            case "SelectItem":
                for(int k=0;k<XML_Reader.Instance.scenarioToDict.CreateObjType2DDictionary[sceneLoader.currentStage].Count;k++)
                {
                    for(int j=0;j<sceneLoader.object2DDict[k].Count;j++)
                    {
                        if (sceneLoader.object2DDict[k][j].tag == "Order")
                        {
                            if (sceneLoader.object2DDict[k][j].activeSelf == true)
                            {
                                text = sceneLoader.object2DDict[k][j].transform.GetChild(0).GetComponent<Text>();
                                //20191122 추가사항
                                for (int q = 0; q < sceneLoader.object2DDict[k][j].transform.childCount; q++)
                                {
                                    for (int w = 0; w < sceneLoader.object2DDict[k][j].transform.GetChild(q).transform.childCount; w++)
                                    {
                                        if (sceneLoader.object2DDict[k][j].transform.GetChild(q).GetChild(w).GetComponent<Scrollbar>() != null)
                                        {
                                            m_Scrollbar = sceneLoader.object2DDict[k][j].transform.GetChild(q).GetChild(w).GetComponent<Scrollbar>();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                break;
            case "TouchGame":
            case "DeleteBlur":
                for(int k=0;k<XML_Reader.Instance.scenarioToDict.CreateObjType2DDictionary[sceneLoader.currentStage].Count;k++)
                {
                    for(int j=0;j<sceneLoader.object2DDict[k].Count;j++)
                    {
                        if(sceneLoader.object2DDict[k][j].tag=="Rule")
                        {
                            if(sceneLoader.object2DDict[k][j].activeSelf==true)
                            {
                                text = sceneLoader.object2DDict[k][j].GetComponent<Text>();
                                //20191122 추가사항
                                for (int q = 0; q < sceneLoader.object2DDict[k][j].transform.childCount; q++)
                                {
                                    for (int w = 0; w < sceneLoader.object2DDict[k][j].transform.GetChild(q).transform.childCount; w++)
                                    {
                                        if (sceneLoader.object2DDict[k][j].transform.GetChild(q).GetChild(w).GetComponent<Scrollbar>() != null)
                                        {
                                            m_Scrollbar = sceneLoader.object2DDict[k][j].transform.GetChild(q).GetChild(w).GetComponent<Scrollbar>();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                break;
            case "Quarter":
                for (int k = 0; k < XML_Reader.Instance.scenarioToDict.CreateObjType2DDictionary[sceneLoader.currentStage].Count; k++)
                {
                    for (int j = 0; j < sceneLoader.object2DDict[k].Count; j++)
                    {
                        if (sceneLoader.object2DDict[k][j].tag == "TextBox")
                        {
                            if (sceneLoader.object2DDict[k][j].activeSelf == true)
                            {
                                text = sceneLoader.object2DDict[k][j].transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
                                trigger = true;
                                break;
                            }
                        }
                        else if (sceneLoader.object2DDict[k][j].tag == "PopUp")
                        {
                            if (sceneLoader.object2DDict[k][j].activeSelf == true)
                            {
                                text = sceneLoader.object2DDict[k][j].transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
                                trigger = true;
                                break;
                            }
                        }
                    }    
                    if(trigger==true)
                    {
                        break;
                    }
                }
                break;
            default:
                break;
        }
        //if(Contents.Contains("###"))
        //{
        //    //뒤엔 계정 또는 계정에 저장된 이름
        //    //string temp = Contents.Replace("###",);
        //    //타이핑 소스
        //    //GameObject.Find("TypingManager").GetComponent<TypingManager>().TypingText(temp, text);
        //}
        //else
        //{
        //    GameObject.Find("TypingManager").GetComponent<TypingManager>().TypingText(Contents, text);                
        //}
        if(XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventType=="PanelPop"|| XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventType == "SelectItem")
        {
            text.text = Contents;
            PlayerInfo.Instance.isComplite = true;
        }
        else
        {
            GameObject.Find("TypingManager").GetComponent<TypingManager>().m_Scrollbar = m_Scrollbar;
            GameObject.Find("TypingManager").GetComponent<TypingManager>().TypingText(Contents, text);
        }
    }

    //쓰레기 터치 판정
    public void ObjectTouch()
    {
        if(Input.touchCount==1)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            bool bCheck = Physics.Raycast(ray, out hit, 30.0f);
            if(bCheck==true)
            {
                string name = hit.collider.gameObject.name;


                foreach (KeyValuePair<int, GameObject> pair in new Dictionary<int, GameObject>(MiniGameObj3DDict))
                {
                    if (pair.Value.transform.gameObject.name == name)
                    {
                        MiniGameObj3DDict.Remove(pair.Key);
                        Destroy(pair.Value.transform.gameObject);
                        GameCount--;
                    }
                    else
                    {
                        continue;
                    }
                }
            }
        }
    }

    public void TouchTest()
    {
        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);        
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero, 0f);
        Debug.DrawRay(pos, transform.forward * 10, Color.red, 0.3f);
        bool bCheck = hit;
        if (bCheck)
        {
            if (hit.collider != null)
            {
                Debug.Log(hit.collider.name);
            }
        }
    }

    public void EraseBlur()
    {
        if (XML_Reader.Instance.scenarioToDict.CreateObjType2DDictionary[sceneLoader.currentStage] != null)
        {            
           for(int k=0;k<XML_Reader.Instance.scenarioToDict.CreateCharType2DDictionary[sceneLoader.currentStage].Count;k++)
            {
                for(int j=0;j<sceneLoader.object2DDict[k].Count;j++)
                {
                    if(sceneLoader.object2DDict[k][j].name=="Blur")
                    {
                        //Debug.Log(1.0f-((1.0f / XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].MaxCount)*SwipeCount));
                        float a= 1.0f - ((1.0f / XML_Reader.Instance.scenarioToDict.DeleteBlurEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[sceneLoader.currentStage].PageList[Index].EventID].MaxCount) * SwipeCount);
                        Debug.Log(a);
                        sceneLoader.object2DDict[k][j].GetComponent<Image>().color = new Color(sceneLoader.object2DDict[k][j].GetComponent<Image>().color.r, sceneLoader.object2DDict[k][j].GetComponent<Image>().color.g, sceneLoader.object2DDict[k][j].GetComponent<Image>().color.b, a);
                        if(a<=0)
                        {
                            BlurClear = true;
                        }
                    }
                }
            }
        }

        startTouch = swipeDelta = Vector2.zero;
        isDragging = false;
    }
   
}
