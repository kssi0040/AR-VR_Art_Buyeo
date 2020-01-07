using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class QuizManager : MonoBehaviour
{
    [SerializeField]
    Text QTextPrefab;
    [SerializeField]
    Scrollbar QScrollbarPrefab;

    [Header("Link Quiz")]
    [SerializeField]
    Transform LinkQPanel;
    [SerializeField]
    Transform quizParent;
    [SerializeField]
    Transform ansParent;
    [SerializeField]
    Transform UILineRendererParent;
    [SerializeField]
    GameObject qPanelPrefab;
    [SerializeField]
    GameObject aPanelPrefab;
    [SerializeField]
    GameObject UILineRendererPrefab;
    [SerializeField]
    public int LinkCount = 0;
    [SerializeField]
    public bool LinkClear = false;

    [Header("Typing1 Quiz")]
    [SerializeField]
    Transform Typing1QPanel;
    [SerializeField]
    private Image T1notifyMsg;
    [SerializeField]
    private InputField T1InputField;
    [SerializeField]
    private Button T1InputBtn;
    [SerializeField]
    public bool Typing1Clear = false;

    [Header("Typing2 Quiz")]
    [SerializeField]
    Transform Typing2QPanel;
    [SerializeField]
    private GameObject T2AnsPanelPrefab;
    [SerializeField]
    private GameObject T2InputButPrefab;
    [SerializeField]
    private Transform Typing2APanel;

    private List<GameObject> T2ifList = new List<GameObject>();

    [Header("Option Quiz")]
    [SerializeField]
    Transform OptionQPanel;
    [SerializeField]
    GameObject OAnsButPrefab;
    [SerializeField]
    Transform OptionApanel;
    [SerializeField]
    private Image OnotifyMsg;
    [SerializeField]
    public bool OptionClear = false;
    public bool OptFail = false;
    public int OptCount = 0;

    [Header("Img Option Quiz")]
    [SerializeField]
    Transform ImgOptionQPanel;
    [SerializeField]
    GameObject ImgOAnsButPrefab;
    [SerializeField]
    Transform ImgOptionAPanel;
    [SerializeField]
    private Image ImgOnotifyMsg;
    [SerializeField]
    public bool ImgOptClear = false;
    public bool ImgOptFail = false;
    public int ImgOptCount = 0;

    private List<GameObject> lineList = new List<GameObject>();
    public StagePlay stagePlay;
    // Start is called before the first frame update
    void Awake()
    {
        stagePlay = FindObjectOfType<StagePlay>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Quiz_XML_Reader.Instance.readCompleted)
        {
            //GenerateLinkQuiz(0);
            //GenerateTyping1Quiz(0);
            //GenerateOptionQuiz(0);
            //Quiz_XML_Reader.Instance.readCompleted = false;
        }
    }

    public void GenerateLinkQuiz(int id)
    {
        string Quiz = "";

        foreach(RectTransform child in quizParent.GetComponentsInChildren<RectTransform>())
        {
            if(child.transform.gameObject.name=="QuesPanel")
            {
                continue;
            }
            else
            {
                Destroy(child.transform.gameObject);
            }
        }

        foreach(RectTransform child in ansParent.GetComponentsInChildren<RectTransform>())
        {
            if(child.transform.gameObject.name=="AnsPanel")
            {
                continue;
            }
            else
            {
                Destroy(child.transform.gameObject);
            }
        }

        LinkCount = 0;

        if(lineList.Count!=0)
        {
            foreach(GameObject item in lineList)
            {
                Destroy(item);
            }

            lineList.Clear();
        }

        for(int k=0;k<Quiz_XML_Reader.Instance.quizToDict.LinkQuizDictionary[id].LeftItemList.Length;k++)
        {
            GameObject _qPrefab = Instantiate(qPanelPrefab);
            _qPrefab.transform.GetChild(1).GetChild(0).name = Quiz_XML_Reader.Instance.quizToDict.LinkQuizDictionary[id].LeftItemList[k].Name;
            _qPrefab.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = Quiz_XML_Reader.Instance.quizToDict.LinkQuizDictionary[id].LeftItemList[k].Text;
            _qPrefab.transform.SetParent(quizParent, false);

            GameObject _aPrefab = Instantiate(aPanelPrefab);
            _aPrefab.transform.GetChild(1).GetChild(0).name = Quiz_XML_Reader.Instance.quizToDict.LinkQuizDictionary[id].RightItemList[k].Name;
            _aPrefab.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = Quiz_XML_Reader.Instance.quizToDict.LinkQuizDictionary[id].RightItemList[k].Text;
            _aPrefab.transform.SetParent(ansParent, false);

            GameObject _lrPrefab = Instantiate(UILineRendererPrefab);
            _lrPrefab.transform.SetParent(UILineRendererParent, false);

            lineList.Add(_lrPrefab);

            LinkCount++;
        }

        Quiz = Quiz_XML_Reader.Instance.quizToDict.LinkQuizDictionary[id].Text;

        GameObject.Find("UILineConnector").GetComponent<UILineConnector>().m_LineRenderer = new UILineRenderer[LinkCount];

        for(int k=0;k<lineList.Count;k++)
        {
            GameObject.Find("UILineConnector").GetComponent<UILineConnector>().m_LineRenderer[k] = lineList[k].GetComponent<UILineRenderer>();
        }

        GameObject.Find("UILineConnector").GetComponent<UILineConnector>().isInitialized = true;
        GameObject.Find("UILineConnector").GetComponent<UILineConnector>().index = -1;
        GameObject.Find("UILineConnector").GetComponent<UILineConnector>().isDrawn = false;

        foreach(Transform child in LinkQPanel)
        {
            GameObject.Destroy(child.gameObject);
        }

        Text qText = Instantiate(QTextPrefab, LinkQPanel) as Text;
        qText.text = Quiz;
        Scrollbar qScrollbar = Instantiate(QScrollbarPrefab, LinkQPanel) as Scrollbar;

        LinkQPanel.GetComponent<ScrollRect>().content = qText.rectTransform;
        LinkQPanel.GetComponent<ScrollRect>().verticalScrollbar = qScrollbar;
    }

    public void GenerateTyping1Quiz(int id)
    {
        foreach(Transform child in Typing1QPanel)
        {
            GameObject.Destroy(child.gameObject);
        }
        T1InputBtn.onClick.RemoveAllListeners();
        Text qText = Instantiate(QTextPrefab, Typing1QPanel) as Text;
        Scrollbar qScrollbar = Instantiate(QScrollbarPrefab, Typing1QPanel) as Scrollbar;

        Typing1QPanel.GetComponent<ScrollRect>().content = qText.rectTransform;
        Typing1QPanel.GetComponent<ScrollRect>().verticalScrollbar = qScrollbar;

        T1InputField.text = string.Empty;
        T1notifyMsg.sprite = null;
        T1notifyMsg.enabled = false;

        qText.text = Quiz_XML_Reader.Instance.quizToDict.Typing1QuizDictionary[id].Quest;
        T1InputBtn.onClick.AddListener(delegate { Typing1AnswerSelectionEvent(id); });
    }

    public void GenerateTyping2Quiz(int id)
    {
        foreach(Transform child in Typing2QPanel)
        {
            GameObject.Destroy(child.gameObject);
        }
       
        Text qText = Instantiate(QTextPrefab, Typing2QPanel) as Text;
        Scrollbar qScrollbar = Instantiate(QScrollbarPrefab, Typing2QPanel) as Scrollbar;

        T2ifList.Clear();

        foreach(RectTransform child in Typing2APanel.GetComponentsInChildren<RectTransform>())
        {
            if(child.transform.gameObject.name=="Typing2Apanel")
            {
                continue;
            }
            else
            {
                Destroy(child.transform.gameObject);
            }
        }
    
        for(int k=0;k<Quiz_XML_Reader.Instance.quizToDict.Typing2QuizDictionary[id].InitAnswerList.Length;k++)
        {
            GameObject _AnsPrefab = Instantiate(T2AnsPanelPrefab);

            T2ifList.Add(_AnsPrefab);

            _AnsPrefab.transform.GetChild(0).GetComponent<Text>().text = k.ToString();
            _AnsPrefab.transform.SetParent(Typing2APanel, false);
        }

        qText.text = Quiz_XML_Reader.Instance.quizToDict.Typing2QuizDictionary[id].Quest;

        GameObject IButPrefab = Instantiate(T2InputButPrefab);
        IButPrefab.GetComponent<Button>().onClick.AddListener(delegate { Typing2AnswerSelectionEvent(id, T2ifList); });
        IButPrefab.transform.SetParent(Typing2APanel, false);
    }

    public void GenerateOptQuiz(int id)
    {        
        foreach(Transform child in OptionQPanel)
        {
            GameObject.Destroy(child.gameObject);
        }
        OptCount = 0;
        Text qText = Instantiate(QTextPrefab, OptionQPanel) as Text;
        Scrollbar qScrollbar = Instantiate(QScrollbarPrefab, OptionQPanel) as Scrollbar;

        OptionQPanel.GetComponent<ScrollRect>().content = qText.rectTransform;
        OptionQPanel.GetComponent<ScrollRect>().verticalScrollbar = qScrollbar;

        OnotifyMsg.sprite = null;
        OnotifyMsg.enabled = false;
        foreach(Button child in OptionApanel.GetComponentsInChildren<Button>())
        {
            Destroy(child.transform.gameObject);
        }

        qText.text = Quiz_XML_Reader.Instance.quizToDict.OptQuizDictionary[id].Quest;

        for (int k=0;k<Quiz_XML_Reader.Instance.quizToDict.OptQuizDictionary[id].OrderList.Length;k++)
        {            
            GameObject _aPrefab = Instantiate(OAnsButPrefab);
            _aPrefab.transform.GetChild(0).name = Quiz_XML_Reader.Instance.quizToDict.OptQuizDictionary[id].OrderList[k].Name;
            _aPrefab.transform.GetChild(0).GetComponent<Text>().text = Quiz_XML_Reader.Instance.quizToDict.OptQuizDictionary[id].OrderList[k].Text;
            _aPrefab.GetComponent<Button>().onClick.AddListener(delegate { OptAnswerSelectionEvent(id, _aPrefab.GetComponent<Button>()); });
            _aPrefab.transform.SetParent(OptionApanel, false);                     
        }
    }

    public void GenerateImgOptQuiz(int id)
    {
        foreach(Transform child in ImgOptionQPanel)
        {
            GameObject.Destroy(child.gameObject);
        }
        //20190905 추가사항
        ImgOptCount = 0;
        Text qText = Instantiate(QTextPrefab, ImgOptionQPanel) as Text;
        Scrollbar qScrollbar = Instantiate(QScrollbarPrefab, ImgOptionQPanel) as Scrollbar;

        ImgOptionQPanel.GetComponent<ScrollRect>().content = qText.rectTransform;
        ImgOptionQPanel.GetComponent<ScrollRect>().verticalScrollbar = qScrollbar;

        ImgOnotifyMsg.sprite = null;
        ImgOnotifyMsg.enabled = false;
        foreach(Button child in ImgOptionAPanel.GetComponentsInChildren<Button>())
        {
            Destroy(child.transform.gameObject);
        }

        qText.text = Quiz_XML_Reader.Instance.quizToDict.ImgOptQuizDictionary[id].Quest;

        for(int k=0;k<Quiz_XML_Reader.Instance.quizToDict.ImgOptQuizDictionary[id].OrderList.Length;k++)
        {
            GameObject ansPrefab = Instantiate(ImgOAnsButPrefab);
            ansPrefab.transform.GetChild(0).GetComponent<Text>().text = Quiz_XML_Reader.Instance.quizToDict.ImgOptQuizDictionary[id].OrderList[k].Text;
            ansPrefab.transform.GetChild(0).GetComponent<Text>().enabled = false;
            ansPrefab.GetComponent<Image>().sprite = Resources.Load<Sprite>(Quiz_XML_Reader.Instance.quizToDict.ImgOptQuizDictionary[id].OrderList[k].Sprite);
            ansPrefab.GetComponent<Button>().onClick.AddListener(delegate { ImgOptAnswerSelectionEvent(id, ansPrefab.GetComponent<Button>()); });
            ansPrefab.transform.SetParent(ImgOptionAPanel, false);
        }
    }

    public void GenerateLinkQuizTimeRec(int id)
    {
        string Quiz = "";

        foreach (RectTransform child in quizParent.GetComponentsInChildren<RectTransform>())
        {
            if (child.transform.gameObject.name == "QuesPanel")
            {
                continue;
            }
            else
            {
                Destroy(child.transform.gameObject);
            }
        }

        foreach (RectTransform child in ansParent.GetComponentsInChildren<RectTransform>())
        {
            if (child.transform.gameObject.name == "AnsPanel")
            {
                continue;
            }
            else
            {
                Destroy(child.transform.gameObject);
            }
        }

        LinkCount = 0;

        if (lineList.Count != 0)
        {
            foreach (GameObject item in lineList)
            {
                Destroy(item);
            }

            lineList.Clear();
        }

        for (int k = 0; k < Quiz_XML_Reader.Instance.quizToDict.QuizTimeRecDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].ID].InitQuizList[id].LinkQuizList.LeftItemList.Length; k++)            
        {
            GameObject _qPrefab = Instantiate(qPanelPrefab);
            _qPrefab.transform.GetChild(1).GetChild(0).name = Quiz_XML_Reader.Instance.quizToDict.QuizTimeRecDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].ID].InitQuizList[id].LinkQuizList.LeftItemList[k].Name;
            _qPrefab.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = Quiz_XML_Reader.Instance.quizToDict.QuizTimeRecDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].ID].InitQuizList[id].LinkQuizList.LeftItemList[k].Text;
            _qPrefab.transform.SetParent(quizParent, false);

            GameObject _aPrefab = Instantiate(aPanelPrefab);
            _aPrefab.transform.GetChild(1).GetChild(0).name = Quiz_XML_Reader.Instance.quizToDict.QuizTimeRecDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].ID].InitQuizList[id].LinkQuizList.RightItemList[k].Name;
            _aPrefab.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = Quiz_XML_Reader.Instance.quizToDict.QuizTimeRecDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].ID].InitQuizList[id].LinkQuizList.RightItemList[k].Text;
            _aPrefab.transform.SetParent(ansParent, false);

            GameObject _lrPrefab = Instantiate(UILineRendererPrefab);
            _lrPrefab.transform.SetParent(UILineRendererParent, false);

            lineList.Add(_lrPrefab);

            LinkCount++;
        }

        Quiz = Quiz_XML_Reader.Instance.quizToDict.QuizTimeRecDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].ID].InitQuizList[id].LinkQuizList.Text;

        GameObject.Find("UILineConnector").GetComponent<UILineConnector>().m_LineRenderer = new UILineRenderer[LinkCount];

        for (int k = 0; k < lineList.Count; k++)
        {
            GameObject.Find("UILineConnector").GetComponent<UILineConnector>().m_LineRenderer[k] = lineList[k].GetComponent<UILineRenderer>();
        }

        GameObject.Find("UILineConnector").GetComponent<UILineConnector>().isInitialized = true;
        GameObject.Find("UILineConnector").GetComponent<UILineConnector>().index = -1;
        GameObject.Find("UILineConnector").GetComponent<UILineConnector>().isDrawn = false;

        foreach (Transform child in LinkQPanel)
        {
            GameObject.Destroy(child.gameObject);
        }

        Text qText = Instantiate(QTextPrefab, LinkQPanel) as Text;
        qText.text = Quiz;
        Scrollbar qScrollbar = Instantiate(QScrollbarPrefab, LinkQPanel) as Scrollbar;

        LinkQPanel.GetComponent<ScrollRect>().content = qText.rectTransform;
        LinkQPanel.GetComponent<ScrollRect>().verticalScrollbar = qScrollbar;
    }

    public void GenerateTyping1QuizTimeRec(int id)
    {
        foreach (Transform child in Typing1QPanel)
        {
            GameObject.Destroy(child.gameObject);
        }
        T1InputBtn.onClick.RemoveAllListeners();
        Text qText = Instantiate(QTextPrefab, Typing1QPanel) as Text;
        Scrollbar qScrollbar = Instantiate(QScrollbarPrefab, Typing1QPanel) as Scrollbar;

        Typing1QPanel.GetComponent<ScrollRect>().content = qText.rectTransform;
        Typing1QPanel.GetComponent<ScrollRect>().verticalScrollbar = qScrollbar;

        T1InputField.text = string.Empty;
        T1notifyMsg.sprite = null;
        T1notifyMsg.enabled = false;
        qText.text = Quiz_XML_Reader.Instance.quizToDict.QuizTimeRecDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].ID].InitQuizList[id].Typing1QuizList.Quest;
        T1InputBtn.onClick.AddListener(delegate { Typing1AnswerSelectionTimeRecEvent(id); });
    }

    public void GenerateTyping2QuizTimeRec(int id)
    {
        foreach(Transform child in Typing2QPanel)
        {
            GameObject.Destroy(child.gameObject);
        }

        Text qText = Instantiate(QTextPrefab, Typing2QPanel) as Text;
        Scrollbar qScrollbar = Instantiate(QScrollbarPrefab, Typing2QPanel) as Scrollbar;

        Typing2QPanel.GetComponent<ScrollRect>().content = qText.rectTransform;
        Typing2QPanel.GetComponent<ScrollRect>().verticalScrollbar = qScrollbar;

        T2ifList.Clear();
        foreach(RectTransform child in Typing2APanel.GetComponentsInChildren<RectTransform>())
        {
            if(child.transform.gameObject.name=="Typing2APanel")
            {
                continue;
            }
            else
            {
                Destroy(child.transform.gameObject);
            }
        }

        for(int k=0;k<Quiz_XML_Reader.Instance.quizToDict.QuizTimeRecDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].ID].InitQuizList[id].Typing2QuizList.InitAnswerList.Length;k++)
        {
            GameObject _AnsPrefab = Instantiate(T2AnsPanelPrefab);

            T2ifList.Add(_AnsPrefab);

            _AnsPrefab.transform.GetChild(0).GetComponent<Text>().text = k.ToString();
            _AnsPrefab.transform.SetParent(Typing2APanel, false);
        }

        qText.text = Quiz_XML_Reader.Instance.quizToDict.QuizTimeRecDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].ID].InitQuizList[id].Typing2QuizList.Quest;

        GameObject IButPrefab = Instantiate(T2InputButPrefab);
        IButPrefab.GetComponent<Button>().onClick.AddListener(delegate { Typing2AnswerSelectionTimeRecEvent(id, T2ifList); });
        IButPrefab.transform.SetParent(Typing2APanel, false);
    }

    public void GenerateOptQuizTimeRec(int id)
    {
        foreach (Transform child in OptionQPanel)
        {
            GameObject.Destroy(child.gameObject);
        }
        OptCount = 0;
        Text qText = Instantiate(QTextPrefab, OptionQPanel) as Text;
        Scrollbar qScrollbar = Instantiate(QScrollbarPrefab, OptionQPanel) as Scrollbar;

        OptionQPanel.GetComponent<ScrollRect>().content = qText.rectTransform;
        OptionQPanel.GetComponent<ScrollRect>().verticalScrollbar = qScrollbar;

        OnotifyMsg.sprite = null;
        OnotifyMsg.enabled = false;
        foreach (Button child in OptionApanel.GetComponentsInChildren<Button>())
        {
            Destroy(child.transform.gameObject);
        }
        
        qText.text = Quiz_XML_Reader.Instance.quizToDict.QuizTimeRecDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].ID].InitQuizList[id].OptQuizList.Quest;

        for (int k = 0; k < Quiz_XML_Reader.Instance.quizToDict.QuizTimeRecDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].ID].InitQuizList[id].OptQuizList.OrderList.Length; k++)
        {
            GameObject _aPrefab = Instantiate(OAnsButPrefab);
            _aPrefab.transform.GetChild(0).name = Quiz_XML_Reader.Instance.quizToDict.QuizTimeRecDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].ID].InitQuizList[id].OptQuizList.OrderList[k].Name;
            _aPrefab.transform.GetChild(0).GetComponent<Text>().text = Quiz_XML_Reader.Instance.quizToDict.QuizTimeRecDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].ID].InitQuizList[id].OptQuizList.OrderList[k].Text;
            _aPrefab.GetComponent<Button>().onClick.AddListener(delegate { OptAnswerSelectionTimeRecEvent(id, _aPrefab.GetComponent<Button>()); });
            _aPrefab.transform.SetParent(OptionApanel, false);
        }
    }

    public void GenerateImgOptQuizTimeRec(int id)
    {
        foreach(Transform child in ImgOptionQPanel)
        {
            GameObject.Destroy(child.gameObject);
        }
        ImgOptCount = 0;
        Text qText = Instantiate(QTextPrefab, ImgOptionQPanel) as Text;
        Scrollbar qScrollbar = Instantiate(QScrollbarPrefab, ImgOptionQPanel) as Scrollbar;

        ImgOptionQPanel.GetComponent<ScrollRect>().content = qText.rectTransform;
        ImgOptionQPanel.GetComponent<ScrollRect>().verticalScrollbar = qScrollbar;

        ImgOnotifyMsg.sprite = null;
        ImgOnotifyMsg.enabled = false;
        foreach(Button child in ImgOptionAPanel.GetComponentsInChildren<Button>())
        {
            Destroy(child.transform.gameObject);
        }

        qText.text = Quiz_XML_Reader.Instance.quizToDict.QuizTimeRecDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].ID].InitQuizList[id].ImgOptQuizList.Quest;

        for(int k=0;k<Quiz_XML_Reader.Instance.quizToDict.QuizTimeRecDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].ID].InitQuizList[id].ImgOptQuizList.OrderList.Length;k++)
        {
            GameObject ansPrefab = Instantiate(ImgOAnsButPrefab);
            ansPrefab.transform.GetChild(0).GetComponent<Text>().text = Quiz_XML_Reader.Instance.quizToDict.QuizTimeRecDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].ID].InitQuizList[id].ImgOptQuizList.OrderList[k].Text;
            ansPrefab.transform.GetChild(0).GetComponent<Text>().enabled = false;
            ansPrefab.GetComponent<Image>().sprite = Resources.Load<Sprite>(Quiz_XML_Reader.Instance.quizToDict.QuizTimeRecDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].ID].InitQuizList[id].ImgOptQuizList.OrderList[k].Sprite);
            ansPrefab.GetComponent<Button>().onClick.AddListener(delegate { ImgOptAnswerSelectionTimeRecEvent(id, ansPrefab.GetComponent<Button>()); });
            ansPrefab.transform.SetParent(ImgOptionAPanel, false);
        }
    }

    public void GenerateLinkQuizTimeAtk(int id)
    {
        string Quiz = "";

        foreach(RectTransform child in quizParent.GetComponentsInChildren<RectTransform>())
        {
            if (child.transform.gameObject.name == "QuesPanel")
            {
                continue;
            }
            else
            {
                Destroy(child.transform.gameObject);
            }
        }

        foreach(RectTransform child in ansParent.GetComponentsInChildren<RectTransform>())
        {
            if (child.transform.gameObject.name=="AnsPanel")
            {
                continue;
            }
            else
            {
                Destroy(child.transform.gameObject);
            }
        }

        LinkCount = 0;

        if(lineList.Count!=0)
        {
            foreach(GameObject item in lineList)
            {
                Destroy(item);
            }

            lineList.Clear();
        }

        for(int k=0;k<Quiz_XML_Reader.Instance.quizToDict.QuizTimeAtkDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].ID].InitQuizList[id].LinkQuizList.LeftItemList.Length;k++)
        {
            GameObject _qPrefab = Instantiate(qPanelPrefab);
            _qPrefab.transform.GetChild(1).GetChild(0).name = Quiz_XML_Reader.Instance.quizToDict.QuizTimeAtkDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].ID].InitQuizList[id].LinkQuizList.LeftItemList[k].Name;
            _qPrefab.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = Quiz_XML_Reader.Instance.quizToDict.QuizTimeAtkDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].ID].InitQuizList[id].LinkQuizList.LeftItemList[k].Text;
            _qPrefab.transform.SetParent(quizParent, false);

            GameObject _aPrefab = Instantiate(aPanelPrefab);
            _aPrefab.transform.GetChild(1).GetChild(0).name = Quiz_XML_Reader.Instance.quizToDict.QuizTimeAtkDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].ID].InitQuizList[id].LinkQuizList.RightItemList[k].Name;
            _aPrefab.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = Quiz_XML_Reader.Instance.quizToDict.QuizTimeAtkDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].ID].InitQuizList[id].LinkQuizList.RightItemList[k].Text;
            _aPrefab.transform.SetParent(ansParent, false);

            GameObject _lrPrefab = Instantiate(UILineRendererPrefab);
            _lrPrefab.transform.SetParent(UILineRendererParent, false);

            lineList.Add(_lrPrefab);

            LinkCount++;
        }

        Quiz = Quiz_XML_Reader.Instance.quizToDict.QuizTimeAtkDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].ID].InitQuizList[id].LinkQuizList.Text;

        GameObject.Find("UILineConnector").GetComponent<UILineConnector>().m_LineRenderer = new UILineRenderer[LinkCount];

        for(int k=0;k<lineList.Count;k++)
        {
            GameObject.Find("UILineConnector").GetComponent<UILineConnector>().m_LineRenderer[k] = lineList[k].GetComponent<UILineRenderer>();
        }

        GameObject.Find("UILineConnector").GetComponent<UILineConnector>().isInitialized = true;
        GameObject.Find("UILineConnector").GetComponent<UILineConnector>().index = -1;
        GameObject.Find("UILineConnector").GetComponent<UILineConnector>().isDrawn = false;

        foreach(Transform child in LinkQPanel)
        {
            GameObject.Destroy(child.gameObject);
        }

        Text qText = Instantiate(QTextPrefab, LinkQPanel) as Text;
        qText.text = Quiz;
        Scrollbar qScrollbar = Instantiate(QScrollbarPrefab, LinkQPanel) as Scrollbar;

        LinkQPanel.GetComponent<ScrollRect>().content = qText.rectTransform;
        LinkQPanel.GetComponent<ScrollRect>().verticalScrollbar = qScrollbar;
    }

    public void GenerateOptQuizTimeAtk(int id)
    {
        foreach (Transform child in OptionQPanel)
        {
            GameObject.Destroy(child.gameObject);
        }
        OptCount = 0;
        OptFail = false;
        Text qText = Instantiate(QTextPrefab, OptionQPanel) as Text;
        Scrollbar qScrollbar = Instantiate(QScrollbarPrefab, OptionQPanel) as Scrollbar;

        OptionQPanel.GetComponent<ScrollRect>().content = qText.rectTransform;
        OptionQPanel.GetComponent<ScrollRect>().verticalScrollbar = qScrollbar;

        OnotifyMsg.sprite = null;
        OnotifyMsg.enabled = false;
        foreach(Button child in OptionApanel.GetComponentsInChildren<Button>())
        {
            Destroy(child.transform.gameObject);
        }

        qText.text = Quiz_XML_Reader.Instance.quizToDict.QuizTimeAtkDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].ID].InitQuizList[id].OptQuizList.Quest;

        for(int k=0;k<Quiz_XML_Reader.Instance.quizToDict.QuizTimeAtkDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].ID].InitQuizList[id].OptQuizList.OrderList.Length;k++)
        {
            GameObject _aPrefab = Instantiate(OAnsButPrefab);
            _aPrefab.transform.GetChild(0).name = Quiz_XML_Reader.Instance.quizToDict.QuizTimeAtkDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].ID].InitQuizList[id].OptQuizList.OrderList[k].Name;
            _aPrefab.transform.GetChild(0).GetComponent<Text>().text = Quiz_XML_Reader.Instance.quizToDict.QuizTimeAtkDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].ID].InitQuizList[id].OptQuizList.OrderList[k].Text;
            _aPrefab.GetComponent<Button>().onClick.AddListener(delegate { OptAnswerSelectionTimeAtkEvent(id, _aPrefab.GetComponent<Button>()); });
            _aPrefab.transform.SetParent(OptionApanel, false);
        }
    }

    public void GenerateImgOptQuizTimeAtk(int id)
    {
        foreach(Transform child in ImgOptionQPanel)
        {
            GameObject.Destroy(child.gameObject);
        }
        //20190905 추가사항
        ImgOptCount = 0;
        Text qText = Instantiate(QTextPrefab, ImgOptionQPanel) as Text;
        Scrollbar qScrollbar = Instantiate(QScrollbarPrefab, ImgOptionQPanel) as Scrollbar;

        ImgOptionQPanel.GetComponent<ScrollRect>().content = qText.rectTransform;
        ImgOptionQPanel.GetComponent<ScrollRect>().verticalScrollbar = qScrollbar;

        ImgOnotifyMsg.sprite = null;
        ImgOnotifyMsg.enabled = false;
        foreach(Button child in ImgOptionAPanel.GetComponentsInChildren<Button>())
        {
            Destroy(child.transform.gameObject);
        }

        qText.text = Quiz_XML_Reader.Instance.quizToDict.QuizTimeAtkDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].ID].InitQuizList[id].ImgOptQuizList.Quest;

        for(int k=0;k<Quiz_XML_Reader.Instance.quizToDict.QuizTimeAtkDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].ID].InitQuizList[id].ImgOptQuizList.OrderList.Length;k++)
        {
            GameObject ansPrefab = Instantiate(ImgOAnsButPrefab);
            ansPrefab.transform.GetChild(0).GetComponent<Text>().text = Quiz_XML_Reader.Instance.quizToDict.QuizTimeAtkDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].ID].InitQuizList[id].ImgOptQuizList.OrderList[k].Text;
            ansPrefab.transform.GetChild(0).GetComponent<Text>().enabled = false;
            ansPrefab.GetComponent<Image>().sprite = Resources.Load<Sprite>(Quiz_XML_Reader.Instance.quizToDict.QuizTimeAtkDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].ID].InitQuizList[id].ImgOptQuizList.OrderList[k].Sprite);
            ansPrefab.GetComponent<Button>().onClick.AddListener(delegate { ImgOptAnswerSelectionTimeAtkEvent(id, ansPrefab.GetComponent<Button>()); });
            ansPrefab.transform.SetParent(ImgOptionAPanel, false);
        }
    }

    public void GenerateTyping1QuizTimeAtk(int id)
    {
        foreach (Transform child in Typing1QPanel)
        {
            GameObject.Destroy(child.gameObject);
        }

        T1InputBtn.onClick.RemoveAllListeners();

        Text qText = Instantiate(QTextPrefab, Typing1QPanel) as Text;
        Scrollbar qScrollbar = Instantiate(QScrollbarPrefab, Typing1QPanel) as Scrollbar;

        Typing1QPanel.GetComponent<ScrollRect>().content = qText.rectTransform;
        Typing1QPanel.GetComponent<ScrollRect>().verticalScrollbar = qScrollbar;

        T1InputField.text = string.Empty;
        T1notifyMsg.sprite = null;
        T1notifyMsg.enabled = false;
        qText.text = Quiz_XML_Reader.Instance.quizToDict.QuizTimeAtkDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].ID].InitQuizList[id].Typing1QuizList.Quest;
        T1InputBtn.onClick.AddListener(delegate { Typing1AnswerSelectionTimeAtkEvent(id); });
    }

    public void GenerateTyping2QuizTimeAtk(int id)
    {
        foreach(Transform child in Typing2QPanel)
        {
            GameObject.Destroy(child.gameObject);
        }

        Text qText = Instantiate(QTextPrefab, Typing2QPanel) as Text;
        Scrollbar qScrollbar = Instantiate(QScrollbarPrefab, Typing2QPanel) as Scrollbar;

        Typing2QPanel.GetComponent<ScrollRect>().content = qText.rectTransform;
        Typing2QPanel.GetComponent<ScrollRect>().verticalScrollbar = qScrollbar;

        T2ifList.Clear();
        foreach(RectTransform child in Typing2APanel.GetComponentsInChildren<RectTransform>())
        {
            if(child.transform.gameObject.name=="Typing2APanel")
            {
                continue;
            }
            else
            {
                Destroy(child.transform.gameObject);
            }
        }

        for(int k=0;k<Quiz_XML_Reader.Instance.quizToDict.QuizTimeAtkDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].ID].InitQuizList[id].Typing2QuizList.InitAnswerList.Length;k++)
        {
            GameObject _AnsPrefab = Instantiate(T2AnsPanelPrefab);

            T2ifList.Add(_AnsPrefab);

            _AnsPrefab.transform.GetChild(0).GetComponent<Text>().text = k.ToString();
            _AnsPrefab.transform.SetParent(Typing2APanel, false);
        }

        qText.text = Quiz_XML_Reader.Instance.quizToDict.QuizTimeAtkDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].ID].InitQuizList[id].Typing2QuizList.Quest;

        GameObject IButPrefab = Instantiate(T2InputButPrefab);        
        IButPrefab.GetComponent<Button>().onClick.AddListener(delegate { Typing2AnswerSelectionTimeAtkEvent(id, T2ifList); });
        IButPrefab.transform.SetParent(Typing2APanel, false);
    }
    
    public void OptAnswerSelectionEvent(int id, Button but)
    {
        int count = 0;

        if (Quiz_XML_Reader.Instance.quizToDict.OptQuizDictionary[id].InitAnswerList.Length > 1)
        {
            for (int k = 0; k < Quiz_XML_Reader.Instance.quizToDict.OptQuizDictionary[id].InitAnswerList.Length; k++)
            {
                if (but.transform.GetChild(0).GetComponent<Text>().text == Quiz_XML_Reader.Instance.quizToDict.OptQuizDictionary[id].InitAnswerList[k].Answer)
                {
                    but.GetComponent<Image>().color = Color.green;

                    StartCoroutine(INotifyMsg(OnotifyMsg, "O"));

                    but.GetComponent<Button>().interactable = false;

                    count++;
                    if (Quiz_XML_Reader.Instance.quizToDict.OptQuizDictionary[id].InitAnswerList.Length == count)
                    {
                        OptionClear = true;
                        StartCoroutine(NextStage());
                    }
                    break;
                }
                else
                {
                    StartCoroutine(INotifyMsg(OnotifyMsg, "X"));
                }
            }
        }
        else
        {
            if (but.transform.GetChild(0).GetComponent<Text>().text == Quiz_XML_Reader.Instance.quizToDict.OptQuizDictionary[id].InitAnswerList[0].Answer)
            {
                but.GetComponent<Image>().color = Color.green;

                StartCoroutine(INotifyMsg(OnotifyMsg, "O"));

                but.GetComponent<Button>().interactable = false;

                StartCoroutine(NextStage());
            }
            else
            {
                StartCoroutine(INotifyMsg(OnotifyMsg, "X"));
            }
        }
    }

    public void OptAnswerSelectionTimeRecEvent(int id, Button but)
    {
        int count=0;

        if(Quiz_XML_Reader.Instance.quizToDict.QuizTimeRecDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].ID].InitQuizList[id].OptQuizList.InitAnswerList.Length>1)
        {
            for(int k=0;k<Quiz_XML_Reader.Instance.quizToDict.QuizTimeRecDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].ID].InitQuizList[id].OptQuizList.InitAnswerList.Length;k++)
            {
                if (but.transform.GetChild(0).GetComponent<Text>().text == Quiz_XML_Reader.Instance.quizToDict.QuizTimeRecDictionary[id].InitQuizList[stagePlay.QuizCount].OptQuizList.InitAnswerList[k].Answer)
                {
                    but.GetComponent<Image>().color = Color.green;

                    StartCoroutine(INotifyMsg(OnotifyMsg, "O"));

                    but.GetComponent<Button>().interactable = false;

                    count++;
                    if(Quiz_XML_Reader.Instance.quizToDict.QuizTimeRecDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].ID].InitQuizList[id].OptQuizList.InitAnswerList.Length==count)
                    {
                        OptionClear = true;
                        stagePlay.QuizCount++;
                        if(stagePlay.QuizCount==Quiz_XML_Reader.Instance.quizToDict.QuizTimeRecDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].ID].Count)
                        {
                            stagePlay.QuizClear = true;
                            //타임 보낼 것...
                        }
                        StartCoroutine(NextQuiz());
                    }                  
                    break;
                }
                else
                {
                    StartCoroutine(INotifyMsg(OnotifyMsg, "X"));
                }
            }
        }
        else
        {
            if (but.transform.GetChild(0).GetComponent<Text>().text == Quiz_XML_Reader.Instance.quizToDict.QuizTimeRecDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].ID].InitQuizList[id].OptQuizList.InitAnswerList[0].Answer)
            {
                but.GetComponent<Image>().color = Color.green;

                StartCoroutine(INotifyMsg(OnotifyMsg, "O"));

                but.GetComponent<Button>().interactable = false;

                stagePlay.QuizCount++;
                if (stagePlay.QuizCount == Quiz_XML_Reader.Instance.quizToDict.QuizTimeRecDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].ID].Count)
                {
                    stagePlay.QuizClear = true;
                }

                StartCoroutine(NextQuiz());
            }
            else
            {
                StartCoroutine(INotifyMsg(OnotifyMsg, "X"));
            }
        }
    }

    public void OptAnswerSelectionTimeAtkEvent(int id, Button but)
    {       
        if(Quiz_XML_Reader.Instance.quizToDict.QuizTimeAtkDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].ID].InitQuizList[id].OptQuizList.InitAnswerList.Length>1)
        {
            for(int k=0;k<Quiz_XML_Reader.Instance.quizToDict.QuizTimeAtkDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].ID].InitQuizList[id].OptQuizList.InitAnswerList.Length;k++)
            {
                if(but.transform.GetChild(0).GetComponent<Text>().text==Quiz_XML_Reader.Instance.quizToDict.QuizTimeAtkDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].ID].InitQuizList[id].OptQuizList.InitAnswerList[k].Answer)
                {
                    but.GetComponent<Image>().color = Color.green;

                    StartCoroutine(INotifyMsg(OnotifyMsg, "O"));

                    but.GetComponent<Button>().interactable = false;

                    OptCount++;
                    if(Quiz_XML_Reader.Instance.quizToDict.QuizTimeAtkDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].ID].InitQuizList[id].OptQuizList.InitAnswerList.Length== OptCount)
                    {
                        OptionClear = true;
                        stagePlay.QuizCount++;
                        if(stagePlay.QuizCount==Quiz_XML_Reader.Instance.quizToDict.QuizTimeAtkDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].ID].Count)
                        {
                            stagePlay.QuizClear = true;                            
                        }
                        StartCoroutine(NextQuiz());
                    }
                    OptFail = false;
                    break;
                }
                else
                {
                    StartCoroutine(INotifyMsg(OnotifyMsg, "X"));
                    OptFail = true;
                    //추가 스크립트 20190808
                    //StartCoroutine(PrevStage());
                }
            }
            if(OptFail==true)
            {
                StartCoroutine(PrevStage());
            }
        }
        else
        {
            if(but.transform.GetChild(0).GetComponent<Text>().text==Quiz_XML_Reader.Instance.quizToDict.QuizTimeAtkDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].ID].InitQuizList[id].OptQuizList.InitAnswerList[0].Answer)
            {
                but.GetComponent<Image>().color = Color.green;

                StartCoroutine(INotifyMsg(OnotifyMsg, "O"));

                but.GetComponent<Button>().interactable = false;

                stagePlay.QuizCount++;
                if(stagePlay.QuizCount==Quiz_XML_Reader.Instance.quizToDict.QuizTimeAtkDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].ID].Count)
                {
                    stagePlay.QuizClear = true;
                }

                StartCoroutine(NextQuiz());
            }
            else
            {
                StartCoroutine(INotifyMsg(OnotifyMsg, "X"));
                //추가 스크립트 20190808
                StartCoroutine(PrevStage());
            }
        }
    }

    public void ImgOptAnswerSelectionEvent(int id, Button but)
    {        
        int count = 0;
        
        if(Quiz_XML_Reader.Instance.quizToDict.ImgOptQuizDictionary[id].InitAnswerList.Length>1)
        {
            for(int k=0;k<Quiz_XML_Reader.Instance.quizToDict.ImgOptQuizDictionary[id].InitAnswerList.Length;k++)
            {
                if(but.transform.GetChild(0).GetComponent<Text>().text==Quiz_XML_Reader.Instance.quizToDict.ImgOptQuizDictionary[id].InitAnswerList[k].Answer)
                {
                    but.GetComponent<Image>().color = Color.green;

                    StartCoroutine(INotifyMsg(ImgOnotifyMsg, "O"));

                    but.GetComponent<Button>().interactable = false;
                    count++;
                    if(Quiz_XML_Reader.Instance.quizToDict.ImgOptQuizDictionary[id].InitAnswerList.Length==ImgOptCount)
                    {
                        ImgOptClear = true;
                        StartCoroutine(NextStage());                        
                    }
                    break;
                }
                else
                {
                    StartCoroutine(INotifyMsg(ImgOnotifyMsg, "X"));
                }
            }
        }
        else
        {
            if(but.transform.GetChild(0).GetComponent<Text>().text==Quiz_XML_Reader.Instance.quizToDict.ImgOptQuizDictionary[id].InitAnswerList[0].Answer)
            {
                but.GetComponent<Image>().color = Color.green;

                StartCoroutine(INotifyMsg(ImgOnotifyMsg, "O"));

                but.GetComponent<Button>().interactable = false;

                StartCoroutine(NextStage());
            }
            else
            {
                StartCoroutine(INotifyMsg(ImgOnotifyMsg, "X"));
            }
        }
    }

    public void ImgOptAnswerSelectionTimeRecEvent(int id, Button but)
    {
        int count = 0;

        if(Quiz_XML_Reader.Instance.quizToDict.QuizTimeRecDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].ID].InitQuizList[id].ImgOptQuizList.InitAnswerList.Length>1)
        {
            for(int k=0;k<Quiz_XML_Reader.Instance.quizToDict.QuizTimeRecDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].ID].InitQuizList[id].ImgOptQuizList.InitAnswerList.Length;k++)
            {
                if(but.transform.GetChild(0).GetComponent<Text>().text==Quiz_XML_Reader.Instance.quizToDict.QuizTimeRecDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].ID].InitQuizList[id].ImgOptQuizList.InitAnswerList[k].Answer)
                {
                    but.GetComponent<Image>().color = Color.green;
                    but.GetComponent<Button>().interactable = false;
                    count++;

                    if(Quiz_XML_Reader.Instance.quizToDict.QuizTimeRecDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].ID].InitQuizList[id].ImgOptQuizList.InitAnswerList.Length==count)
                    {
                        ImgOptClear = true;
                        stagePlay.QuizCount++;
                        if(stagePlay.QuizCount==Quiz_XML_Reader.Instance.quizToDict.QuizTimeRecDictionary[id].Count)
                        {
                            stagePlay.QuizClear = true;
                            //시간 보냅시다.
                        }
                        StartCoroutine(NextQuiz());
                    }
                    break;
                }
                else
                {
                    StartCoroutine(INotifyMsg(ImgOnotifyMsg, "X"));
                }
            }            
        }
        else
        {
            if(but.transform.GetChild(0).GetComponent<Text>().text==Quiz_XML_Reader.Instance.quizToDict.QuizTimeRecDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].ID].InitQuizList[id].ImgOptQuizList.InitAnswerList[0].Answer)
            {
                but.GetComponent<Image>().color = Color.green;
                but.GetComponent<Button>().interactable = false;

                stagePlay.QuizCount++;
                if(stagePlay.QuizCount==Quiz_XML_Reader.Instance.quizToDict.QuizTimeRecDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].ID].Count)
                {
                    stagePlay.QuizClear = true;
                    //시간 보낼것...
                }
                StartCoroutine(NextQuiz());
            }
            else
            {
                StartCoroutine(INotifyMsg(ImgOnotifyMsg, "X"));
            }
        }
    }

    public void ImgOptAnswerSelectionTimeAtkEvent(int id, Button but)
    {
        //20190905 수정사항
        //int count = 0;

        if (Quiz_XML_Reader.Instance.quizToDict.QuizTimeAtkDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].ID].InitQuizList[id].ImgOptQuizList.InitAnswerList.Length>1)
        {
            for(int k=0;k<Quiz_XML_Reader.Instance.quizToDict.QuizTimeAtkDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].ID].InitQuizList[id].ImgOptQuizList.InitAnswerList.Length;k++)
            {
                if(but.transform.GetChild(0).GetComponent<Text>().text==Quiz_XML_Reader.Instance.quizToDict.QuizTimeAtkDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].ID].InitQuizList[id].ImgOptQuizList.InitAnswerList[k].Answer)
                {
                    but.GetComponent<Image>().color = Color.green;
                    but.GetComponent<Button>().interactable = false;
                    //20190905 수정사항
                    ImgOptCount++;

                    if(Quiz_XML_Reader.Instance.quizToDict.QuizTimeAtkDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].ID].InitQuizList[id].ImgOptQuizList.InitAnswerList.Length==ImgOptCount)
                    {
                        ImgOptClear = true;
                        stagePlay.QuizCount++;
                        if(stagePlay.QuizCount==Quiz_XML_Reader.Instance.quizToDict.QuizTimeAtkDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].ID].Count)
                        {
                            stagePlay.QuizClear = true;
                        }
                        StartCoroutine(NextQuiz());
                    }
                    ImgOptFail = false;
                    break;
                }
                else
                {
                    StartCoroutine(INotifyMsg(ImgOnotifyMsg, "X"));
                    //20190905 수정사항
                    ImgOptFail = true;
                }
            }
            //20190905 수정사항
            if(ImgOptFail==true)
            {
                StartCoroutine(PrevStage());
            }
        }
        else
        {
            if (but.transform.GetChild(0).GetComponent<Text>().text == Quiz_XML_Reader.Instance.quizToDict.QuizTimeAtkDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].ID].InitQuizList[id].ImgOptQuizList.InitAnswerList[0].Answer)
            {
                but.GetComponent<Image>().color = Color.green;
                but.GetComponent<Button>().interactable = false;

                stagePlay.QuizCount++;
                if(stagePlay.QuizCount==Quiz_XML_Reader.Instance.quizToDict.QuizTimeAtkDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].ID].Count)
                {
                    stagePlay.QuizClear = true;
                }
                StartCoroutine(NextQuiz());
            }
            else
            {
                StartCoroutine(INotifyMsg(ImgOnotifyMsg, "X"));
                //추가 스크립트 20190808
                StartCoroutine(PrevStage());
            }
        }
    }

    public void Typing1AnswerSelectionEvent(int id)
    {
        if (T1InputField.text == Quiz_XML_Reader.Instance.quizToDict.Typing1QuizDictionary[id].InitAnswerList[0].Answer)            
        {
            //Typing1Clear = true;
            StartCoroutine(INotifyMsg(T1notifyMsg, "O"));
            StartCoroutine(NextStage());
        }
        else
        {
            StartCoroutine(INotifyMsg(T1notifyMsg, "X"));
        }
    }

    public void Typing1AnswerSelectionTimeRecEvent(int id)
    {        
        if(T1InputField.text==Quiz_XML_Reader.Instance.quizToDict.QuizTimeRecDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].ID].InitQuizList[id].Typing1QuizList.InitAnswerList[0].Answer)
        {
            //Typing1Clear = true;
            stagePlay.QuizCount++;
            Debug.Log("Count : " + stagePlay.QuizCount);
            if(stagePlay.QuizCount==Quiz_XML_Reader.Instance.quizToDict.QuizTimeRecDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].ID].Count)
            {
                stagePlay.QuizClear = true;
            }
            StartCoroutine(NextQuiz());
        }
        else
        {
            StartCoroutine(INotifyMsg(T1notifyMsg, "X"));
        }
    }

    public void Typing1AnswerSelectionTimeAtkEvent(int id)
    {
        if(T1InputField.text==Quiz_XML_Reader.Instance.quizToDict.QuizTimeAtkDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].ID].InitQuizList[id].Typing1QuizList.InitAnswerList[0].Answer)
        {
            //Typing1Clear = true;
            stagePlay.QuizCount++;
            if(stagePlay.QuizCount==Quiz_XML_Reader.Instance.quizToDict.QuizTimeAtkDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].ID].Count)
            {
                stagePlay.QuizClear = true;
            }
            StartCoroutine(NextQuiz());
        }
        else
        {
            StartCoroutine(INotifyMsg(T1notifyMsg, "X"));
            StartCoroutine(PrevStage());
        }
    }

    public void Typing2AnswerSelectionEvent(int id, List<GameObject> ifList)
    {
        int count = 0;

        for(int k=0;k<ifList.Count;k++)
        {
            if(ifList[k].transform.GetChild(2).GetComponent<InputField>().text==Quiz_XML_Reader.Instance.quizToDict.Typing2QuizDictionary[id].InitAnswerList[k].Answer)
            {
                ifList[k].transform.GetChild(3).GetComponent<Text>().text = "o";
                ifList[k].transform.GetChild(3).GetComponent<Text>().color = Color.green;
                count++;
            }
            else
            {
                ifList[k].transform.GetChild(3).GetComponent<Text>().text = "x";
                ifList[k].transform.GetChild(3).GetComponent<Text>().color = Color.red;
            }
        }

        if(count==Quiz_XML_Reader.Instance.quizToDict.Typing2QuizDictionary[id].InitAnswerList.Length)
        {
            StartCoroutine(NextStage());
        }
    }

    public void Typing2AnswerSelectionTimeRecEvent(int id, List<GameObject> ifList)
    {
        int count = 0;

        for(int k=0;k<ifList.Count;k++)
        {
            if(ifList[k].transform.GetChild(2).GetComponent<InputField>().text==Quiz_XML_Reader.Instance.quizToDict.QuizTimeRecDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].ID].InitQuizList[id].Typing2QuizList.InitAnswerList[k].Answer)
            {
                ifList[k].transform.GetChild(3).GetComponent<Text>().text = "o";
                ifList[k].transform.GetChild(3).GetComponent<Text>().color = Color.green;
                count++;
            }
            else
            {
                ifList[k].transform.GetChild(3).GetComponent<Text>().text = "x";
                ifList[k].transform.GetChild(3).GetComponent<Text>().color = Color.red;
            }
        }

        if(count==Quiz_XML_Reader.Instance.quizToDict.QuizTimeRecDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].ID].InitQuizList[id].Typing2QuizList.InitAnswerList.Length)
        {
            stagePlay.QuizCount++;
            if(stagePlay.QuizCount==Quiz_XML_Reader.Instance.quizToDict.QuizTimeRecDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].ID].Count)
            {
                stagePlay.QuizClear = true;
            }
            StartCoroutine(NextQuiz());
        }
    }

    public void Typing2AnswerSelectionTimeAtkEvent(int id, List<GameObject> ifList)
    {
        int count = 0;

        for(int k=0;k<ifList.Count;k++)
        {
            if(ifList[k].transform.GetChild(2).GetComponent<InputField>().text==Quiz_XML_Reader.Instance.quizToDict.QuizTimeAtkDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].ID].InitQuizList[id].Typing2QuizList.InitAnswerList[k].Answer)
            {
                ifList[k].transform.GetChild(3).GetComponent<Text>().text = "o";
                ifList[k].transform.GetChild(3).GetComponent<Text>().color = Color.green;
                count++;
            }
            else
            {
                ifList[k].transform.GetChild(3).GetComponent<Text>().text = "x";
                ifList[k].transform.GetChild(3).GetComponent<Text>().color = Color.red;
            }
        }
        Debug.Log("문제 맞춘 수: " + count);
        if(count==Quiz_XML_Reader.Instance.quizToDict.QuizTimeAtkDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].ID].InitQuizList[id].Typing2QuizList.InitAnswerList.Length)
        {
            stagePlay.QuizCount++;
            if(stagePlay.QuizCount==Quiz_XML_Reader.Instance.quizToDict.QuizTimeAtkDictionary[XML_Reader.Instance.scenarioToDict.QuizEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[stagePlay.sceneLoader.currentStage].PageList[stagePlay.Index].EventID].ID].Count)
            {
                stagePlay.QuizClear = true;
            }
            Debug.Log(stagePlay.QuizCount);
            StartCoroutine(NextQuiz());
        }
        //추가 스크립트 20190808
        else
        {
            StartCoroutine(PrevStage());
        }
    }

    IEnumerator INotifyMsg(Image uiImage, string msg)
    {
        uiImage.sprite = Resources.Load<Sprite>(msg);
        
        uiImage.enabled = true;
        yield return new WaitForSeconds(1f);
        uiImage.enabled = false;
    }
    
    IEnumerator NextQuiz()
    {
        yield return new WaitForEndOfFrame();
        PlayerInfo.Instance.isComplite = true;
        stagePlay.forwardDown();
    }

    IEnumerator NextStage()
    {
        yield return new WaitForSeconds(1.0f);
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
            stagePlay.QuizCount = 0;
            stagePlay.StageSet();            
        }
    }
}
