namespace UnityEngine.UI.Extensions
{
    using System.Collections.Generic;
    using System.Collections;

    [AddComponentMenu("UI/Extensions/UI Line Connector")]
    [ExecuteInEditMode]
    public class UILineConnector : MonoBehaviour
    {        
        // The elements between which line segments should be drawn
        private RectTransform[] transforms;
        private RectTransform[] lrTransforms;

        public bool isDrawn;
        private bool isQuesClicked;

        public UILineRenderer[] m_LineRenderer;
        [SerializeField]
        private Image notifyMsg;

        public int index = -1;

        Button quizBut;
        Button ansBut;

        int ansIndex;

        public bool isInitialized;

        public StagePlay stagePlay;
     
        void Start()
        {
            stagePlay = FindObjectOfType<StagePlay>();
            transforms = new RectTransform[2];
            lrTransforms = new RectTransform[2];
            notifyMsg.enabled = false;
        }

        // Update is called once per frame
        void Update()
        {

            //Debug.Log("UILineConnector Update" + " index: " + index + " m_LineRenderer.Length: " + m_LineRenderer.Length
            //    + " isDrawn: " + isDrawn);

            if (isInitialized) //if (m_LineRenderer.Length == 0)
            {
                //m_LineRenderer = new UILineRenderer[transform.childCount];

                //         for (int i = 0; i < m_LineRenderer.Length; i++)
                //{
                //    m_LineRenderer[i] = transform.GetChild(i).GetComponent<UILineRenderer>();

                //}
                //notifyMsg.text = string.Empty;
                isInitialized = false;         
            }

            if (!isDrawn || index >= m_LineRenderer.Length)
                return;

            /*
            if (transforms == null || transforms.Length < 1)
            {
                return;
            }
            */

            RectTransform rt = GetComponent<RectTransform>();
            RectTransform canvas = GetComponentInParent<RectTransform>().GetParentCanvas().GetComponent<RectTransform>();

            // Get the pivot points
            Vector2 thisPivot = rt.pivot;
            Vector2 canvasPivot = canvas.pivot;

            // Set up some arrays of coordinates in various reference systems
            Vector3[] worldSpaces = new Vector3[transforms.Length];
            Vector3[] canvasSpaces = new Vector3[transforms.Length];
            Vector2[] points = new Vector2[transforms.Length];

            // First, convert the pivot to worldspace
            for (int i = 0; i < lrTransforms.Length; i++)
            {
                worldSpaces[i] = lrTransforms[i].TransformPoint(thisPivot);
            }

            // Then, convert to canvas space
            for (int i = 0; i < lrTransforms.Length; i++)
            {
                canvasSpaces[i] = canvas.InverseTransformPoint(worldSpaces[i]);
            }

            // Calculate delta from the canvas pivot point
            for (int i = 0; i < lrTransforms.Length; i++)
            {
                points[i] = new Vector2(canvasSpaces[i].x, canvasSpaces[i].y);
            }

            Debug.Log("index: " + index);

            // And assign the converted points to the line renderer
            m_LineRenderer[index].Points = points;
            m_LineRenderer[index].RelativeSize = false;

            
            
        }
        /// <summary>
        /// Quiz 앞 버튼 눌렀을 때
        /// </summary>
        /// <param name="but"></param>
        public void QuesButtonCallBack(Button but, RectTransform _lrPos)
        {
            if (isQuesClicked == false)
            {
                quizBut = but;
                transforms[0] = quizBut.GetComponent<RectTransform>();

                lrTransforms[0] = _lrPos;
                //lrTransforms[0].position = new Vector2(lrTransforms[0].position.x + 45, lrTransforms[0].position.y);

                isQuesClicked = true;
                isDrawn = false;
                
                quizBut.interactable = false;
                //시간 기록 퀴즈중 하나일 경우
                if (XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].QuizInfoList.Type == "QuizTimeRec")
                {
                    for (int k = 0; k < Quiz_XML_Reader.Instance.quizToDict.QuizTimeRecDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].QuizInfoList.ID].InitQuizList[stagePlay.QuizCount].LinkQuizList.LeftItemList.Length; k++)
                    {
                        if (quizBut.transform.GetChild(0).transform.name == Quiz_XML_Reader.Instance.quizToDict.QuizTimeRecDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].QuizInfoList.ID].InitQuizList[stagePlay.QuizCount].LinkQuizList.LeftItemList[k].Name)
                        {
                            ansIndex = Quiz_XML_Reader.Instance.quizToDict.QuizTimeRecDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].QuizInfoList.ID].InitQuizList[stagePlay.QuizCount].LinkQuizList.LeftItemList[k].Pair;
                            break;
                        }
                    }
                }
                //타임 어택 퀴즈중 하나일 경우
                else if(XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].QuizInfoList.Type=="QuizTimeAtk")
                { 
                    for(int k=0;k<Quiz_XML_Reader.Instance.quizToDict.QuizTimeAtkDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].QuizInfoList.ID].InitQuizList[stagePlay.QuizCount].LinkQuizList.LeftItemList.Length;k++)
                    {
                        if(quizBut.transform.GetChild(0).transform.name==Quiz_XML_Reader.Instance.quizToDict.QuizTimeAtkDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].QuizInfoList.ID].InitQuizList[stagePlay.QuizCount].LinkQuizList.LeftItemList[k].Name)
                        {
                            ansIndex = Quiz_XML_Reader.Instance.quizToDict.QuizTimeAtkDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].QuizInfoList.ID].InitQuizList[stagePlay.QuizCount].LinkQuizList.LeftItemList[k].Pair;
                            break;
                        }
                    }
                }
                //그 외
                else
                {
                    for (int k = 0; k < Quiz_XML_Reader.Instance.quizToDict.LinkQuizDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].QuizInfoList.ID].LeftItemList.Length; k++)
                    {
                        if (quizBut.transform.GetChild(0).transform.name == Quiz_XML_Reader.Instance.quizToDict.LinkQuizDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].QuizInfoList.ID].LeftItemList[k].Name)
                        {
                            ansIndex = Quiz_XML_Reader.Instance.quizToDict.LinkQuizDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].QuizInfoList.ID].LeftItemList[k].Pair;
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Quiz 뒤 버튼 눌렸을 때
        /// </summary>
        /// <param name = "but" ></ param >
        public void AnsButtonCallBack(Button but, RectTransform _lrPos)
        {
            if (!isQuesClicked)
                return;

            ansBut = but;
            //시간 기록 퀴즈중 하나일 경우
            if (XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].QuizInfoList.Type == "QuizTimeRec")
            {
                //수정된 부분...if문 수정  20190808
                if (ansBut.transform.GetChild(0).transform.name==Quiz_XML_Reader.Instance.quizToDict.QuizTimeRecDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].ID].InitQuizList[stagePlay.QuizCount].LinkQuizList.RightItemList[ansIndex].Name)
                {
                    transforms[1] = ansBut.GetComponent<RectTransform>();

                    lrTransforms[1] = _lrPos;
                    lrTransforms[0].position = new Vector2(lrTransforms[0].position.x, lrTransforms[0].position.y);
                    lrTransforms[1].position = new Vector2(lrTransforms[1].position.x, lrTransforms[1].position.y);

                    index++;

                    isDrawn = true;
                    isQuesClicked = false;

                    ansBut.interactable = false;

                    StartCoroutine(INotifyMsg("O"));

                    if(index>0&&index==m_LineRenderer.Length-1)
                    {
                        GameObject.Find("QuizManager").GetComponent<QuizManager>().LinkClear = true;
                        stagePlay.QuizCount++;
                        if(stagePlay.QuizCount==Quiz_XML_Reader.Instance.quizToDict.QuizTimeRecDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].ID].Count)
                        {
                            stagePlay.QuizClear = true;
                        }
                        StartCoroutine(NextQuiz());
                    }
                }
                else
                {
                    isQuesClicked = false;
                    quizBut.interactable = true;

                    StartCoroutine(INotifyMsg("X"));
                }
            }
            //타임 어택 퀴즈중 하나일 경우
            else if (XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].QuizInfoList.Type == "QuizTimeAtk")
            {
                //수정된 부분...if문 수정  20190808
                if (ansBut.transform.GetChild(0).transform.name==Quiz_XML_Reader.Instance.quizToDict.QuizTimeAtkDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].ID].InitQuizList[stagePlay.QuizCount].LinkQuizList.RightItemList[ansIndex].Name)
                {
                    transforms[1] = ansBut.GetComponent<RectTransform>();

                    lrTransforms[1] = _lrPos;
                    lrTransforms[0].position = new Vector2(lrTransforms[0].position.x, lrTransforms[0].position.y);
                    lrTransforms[1].position = new Vector2(lrTransforms[1].position.x, lrTransforms[1].position.y);

                    index++;

                    isDrawn = true;
                    isQuesClicked = false;

                    ansBut.interactable = false;

                    StartCoroutine(INotifyMsg("O"));

                    if(index>0&&index==m_LineRenderer.Length-1)
                    {
                        GameObject.Find("QuizManager").GetComponent<QuizManager>().LinkClear = true;
                        stagePlay.QuizCount++;
                        if(stagePlay.QuizCount==Quiz_XML_Reader.Instance.quizToDict.QuizTimeAtkDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].ID].Count)
                        {
                            stagePlay.QuizClear = true;
                        }
                        StartCoroutine(NextQuiz());
                    }
                }
                else
                {
                    isQuesClicked = false;
                    quizBut.interactable = true;
                    StartCoroutine(INotifyMsg("X"));
                    //추가 스크립트 20190808
                    StartCoroutine(PrevStage());
                }
            }
            //그 외
            else
            {   //수정된 부분...if문 수정  20190808             
                if (ansBut.transform.GetChild(0).transform.name == Quiz_XML_Reader.Instance.quizToDict.LinkQuizDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].QuizInfoList.ID].RightItemList[ansIndex].Name)
                {
                    transforms[1] = ansBut.GetComponent<RectTransform>();

                    lrTransforms[1] = _lrPos;
                    //lrTransforms[0].position = new Vector2(lrTransforms[0].position.x + 45 , lrTransforms[0].position.y);
                    //lrTransforms[1].position = new Vector2(lrTransforms[1].position.x - 45 , lrTransforms[1].position.y);
                    lrTransforms[0].position = new Vector2(lrTransforms[0].position.x, lrTransforms[0].position.y);
                    lrTransforms[1].position = new Vector2(lrTransforms[1].position.x, lrTransforms[1].position.y);
                    //lrTransforms[0].position = new Vector2(lrTransforms[0].position.x + (45 * (Screen.width / 2560)), lrTransforms[0].position.y);
                    //lrTransforms[1].position = new Vector2(lrTransforms[1].position.x - (45 * (Screen.width / 2560)), lrTransforms[1].position.y);

                    index++;

                    isDrawn = true;
                    isQuesClicked = false;

                    ansBut.interactable = false;

                    StartCoroutine(INotifyMsg("O"));
                    if (index > 0 && index == m_LineRenderer.Length - 1)
                    {
                        GameObject.Find("QuizManager").GetComponent<QuizManager>().LinkClear = true;
                        StartCoroutine(NextStage());
                    }
                }
                else
                {
                    isQuesClicked = false;
                    quizBut.interactable = true;

                    StartCoroutine(INotifyMsg("X"));
                }
            }
        }

        IEnumerator INotifyMsg(string msg)
        {
            notifyMsg.sprite = Resources.Load<Sprite>(msg);
            
            notifyMsg.enabled = true;
            yield return new WaitForSeconds(1f);
            notifyMsg.enabled = false;
        }

        IEnumerator NextStage()
        {
            yield return new WaitForSeconds(1.0f);
            PlayerInfo.Instance.isComplite = true;
            stagePlay.forwardDown();
        }

        IEnumerator NextQuiz()
        {
            yield return new WaitForEndOfFrame();
            PlayerInfo.Instance.isComplite = true;
            stagePlay.forwardDown();
        }
        //추가 스크립트 20190808
        IEnumerator PrevStage()
        {
            if (XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].QuizInfoList.Type == "QuizTimeAtk")
            {
                yield return new WaitForSeconds(1.0f);
                stagePlay.Index = Quiz_XML_Reader.Instance.quizToDict.QuizTimeAtkDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].ID].node.fail;
                stagePlay.Prev = XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].Prev;
                stagePlay.Next = XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].Next;
                stagePlay.StageSet();
            }
        }
    }
}