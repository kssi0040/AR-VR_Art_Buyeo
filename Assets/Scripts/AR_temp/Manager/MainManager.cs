using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using PublicDefine;

public class MainManager : MonoBehaviour {

    //============================================ SINGLETON DECLARE ================================================//        
    private static MainManager _instance = null;
    private static GameObject MainMgrObject;
    public static MainManager Instance
    {
        get
        {
            if(null == _instance)
            {
                _instance = FindObjectOfType(typeof(MainManager)) as MainManager;
                if(null == _instance)
                {
                    MainMgrObject = new GameObject();
                    MainMgrObject.name = "MainManager";
                    _instance = MainMgrObject.AddComponent<MainManager>();
                }
            }
            return _instance;
        }
    }

    //============================================================================================================//
    
    private AR_MODE eARMode = AR_MODE.TRACKING;

    // 사진 찍기...
    //WebCamTexture webCamTex;
    


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    //============================================================================================================//


    public void TrackingModeButtonEvent()
    {
        eARMode = AR_MODE.TRACKING;
        //Debug.Log("AR Mode: " + eARMode);       
    }
    public void PickingModeButtonEvent()
    {
        eARMode = AR_MODE.PICKING;
        //Debug.Log("AR Mode: " + eARMode);
    }

    public void TableButtonEvent()
    {
        Debug.Log("table check");
    }

    public void Food1ButtonEvent()
    {
        Debug.Log("food 1");
    }

    public void Food2ButtonEvent()
    {
        Debug.Log("food 2");
    }

    public void Food3ButtonEvent()
    {
        Debug.Log("food 3");
    }
    public void Food4ButtonEvent()
    {
        Debug.Log("food 4");
    }



    public void TakePhotoButtonEvent()
    {        
        Debug.Log("Take Photo");
        //webCamTex = new WebCamTexture();
        //webCamTex.Play();        
        //string captureFIle = Application.persistentDataPath + "/Test_ScreenShot.png";        
        //ScreenCapture.CaptureScreenshot(captureFIle);
        // 해봐야할 듯....
        StartCoroutine("SaveScreenShot");



    }
    IEnumerator SaveScreenShot()
    {
        yield return new WaitForEndOfFrame();

        string myFileName = "ScreenShot" + System.DateTime.Now.Hour + System.DateTime.Now.Minute + System.DateTime.Now.Second + ".png";
        string myDefaultLocation = Application.persistentDataPath + "/" + myFileName;
        string myFolderLocation = "/storage/emulated/0/DCIM/Camera/";
        string myScreenShotLocation = myFolderLocation + myFileName;

        if(!System.IO.Directory.Exists(myFolderLocation))
        {
            System.IO.Directory.CreateDirectory(myFolderLocation);
        }

        ScreenCapture.CaptureScreenshot(myFileName);

        yield return new WaitForSeconds(1);

        //System.IO.File.Move(myDefaultLocation, myScreenShotLocation);
        AndroidJavaClass classPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject objActivity = classPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaClass classUri = new AndroidJavaClass("android.net.Uri");
        AndroidJavaObject objIntent = new AndroidJavaObject("android.content.Intent", new object[2] { "android.intent.action.MEDIA_MOUNTED", classUri.CallStatic<AndroidJavaObject>("parse", "file://" + myScreenShotLocation) });
        objActivity.Call("sendBroadcast", objIntent);
    }



    //=======================================================================================================//
    //================================================= SETTER ================================================//
    //=======================================================================================================//
    public void SetARMode(AR_MODE _eMode)
    {
        eARMode = _eMode;
    }

    //=======================================================================================================//
    //================================================= GETTER ================================================//
    //=======================================================================================================//
    public AR_MODE GetARMode()
    {
        return eARMode;
    }
}
