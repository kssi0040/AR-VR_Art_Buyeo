using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore.HelloAR;
using UnityEngine.UI;

public class ARManager : MonoBehaviour {

    //============================================ SINGLETON DECLARE ================================================//        
    private static ARManager _instance = null;
    private static GameObject ArMgrObject;
    public static ARManager Instance
    {
        get
        {
            if (null == _instance)
            {
                _instance = FindObjectOfType(typeof(ARManager)) as ARManager;
                if (null == _instance)
                {
                    ArMgrObject = new GameObject();
                    ArMgrObject.name = "ARManager";
                    _instance = ArMgrObject.AddComponent<ARManager>();
                    _instance.Initialize();
                }
            }
            return _instance;
        }
    }

    //============================================================================================================//
    public HelloARController ArController = null;

    public GameObject orderText = null;
    public GameObject tableButton = null;
    public GameObject curryButton = null;
    public GameObject riceNoodleButton = null;
    public GameObject chinaNoodleButton = null;
    public GameObject misoSoupButton = null;
    public GameObject resetButton = null;


    //============================================================================================================//
    public GameObject testPopup = null;

    public GameObject ARCamera = null;
    public GameObject mainCamera = null;



    private Text orderTextSc;
    private Button tableButtonSc;
    private Button CurryButtonSc;
    private Button RiceNoodleButtonSc;
    private Button ChinaNoodleButtonSc;
    private Button MisoSoupButtonSc;
    private Button ResetButtonSc;

    private bool bArMode = true;

    void Awake() {
        Initialize();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // On / Off 기능도 추가해야 할 듯....
    // 기타 대사 및 이펙트, 팝업 등등도 여기서 관리한다.
    public void TableStoryActive(bool _bValue)
    {
        if(true == _bValue)
        {
            orderText.SetActive(true);
            tableButton.SetActive(true);
            curryButton.SetActive(true);
            riceNoodleButton.SetActive(true);
            chinaNoodleButton.SetActive(true);
            misoSoupButton.SetActive(true);
            resetButton.SetActive(true);
        }
        else if(false == _bValue)
        {
            orderText.SetActive(false);
            tableButton.SetActive(false);
            curryButton.SetActive(false);
            riceNoodleButton.SetActive(false);
            chinaNoodleButton.SetActive(false);
            misoSoupButton.SetActive(false);
            resetButton.SetActive(false);
        }
        
    }

    //============================================================================================================//
    //================================================== BUTTONS ===================================================//
    //============================================================================================================//
    // test...
    public void PopupOnButtonEvent()
    {        
        if(true == bArMode)
        {
            ARCamera.SetActive(false);
            mainCamera.SetActive(true);
            bArMode = false;
        }
        else
        {
            ARCamera.SetActive(true);
            mainCamera.SetActive(false);
            bArMode = true;
        }        
    }
    public void PopupOffButtonEvent()
    {
        testPopup.SetActive(false);
    }

    void Initialize()
    {
        if (null == ArController)
        {
            ArController = FindObjectOfType(typeof(HelloARController)) as HelloARController;
        }        
        if(null == orderText)
        {            
            orderText = GameObject.Find("Canvas").transform.Find("Text_order").gameObject;
            orderTextSc = orderText.transform.GetComponent<Text>();
        }
        if (null == tableButton)
        {
            tableButton = GameObject.Find("Canvas").transform.Find("Button_Table").gameObject;
            tableButtonSc = tableButton.transform.GetComponent<Button>();
        }
        if (null == curryButton)
        {
            //curryButton = GameObject.Find("Button_Curry") as GameObject;
            curryButton = GameObject.Find("Canvas").transform.Find("Button_Curry").gameObject;
            CurryButtonSc = curryButton.transform.GetComponent<Button>();
        }
        if (null == riceNoodleButton)
        {         
            riceNoodleButton = GameObject.Find("Canvas").transform.Find("Button_Rice_Noodle").gameObject;
            RiceNoodleButtonSc = riceNoodleButton.transform.GetComponent<Button>();
        }
        if (null == ChinaNoodleButtonSc)
        {            
            chinaNoodleButton = GameObject.Find("Canvas").transform.Find("Button_China_Noodle").gameObject;
            ChinaNoodleButtonSc = chinaNoodleButton.transform.GetComponent<Button>();
        }
        if (null == misoSoupButton)
        {            
            misoSoupButton = GameObject.Find("Canvas").transform.Find("Button_Miso_Soup").gameObject;
            MisoSoupButtonSc = misoSoupButton.transform.GetComponent<Button>();
        }
        if (null == resetButton)
        {            
            resetButton = GameObject.Find("Canvas").transform.Find("Button_Reset").gameObject;
            ResetButtonSc = resetButton.transform.GetComponent<Button>();
        }
    }

}




