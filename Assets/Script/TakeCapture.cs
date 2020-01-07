using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TakeCapture : MonoBehaviour
{
    [SerializeField]
    GameObject blink;
    [SerializeField]
    Transform blParent;

    public List<GameObject> Kids;
    public StagePlay m_StagePlay;
    Texture2D texture;
    public RawImage image;
    public GoogleARCore.ARCoreSession ARCoreSession;

    private void Start()
    {
        m_StagePlay = FindObjectOfType<StagePlay>();
        blParent = m_StagePlay.sceneLoader.CanvasList[0].transform;

        blink = Resources.Load<GameObject>("Flash");

        for (int k = 0; k < m_StagePlay.sceneLoader.CanvasList.Count; k++)
        {        
            for (int j = 0; j < m_StagePlay.sceneLoader.CanvasList[k].transform.GetChild(0).childCount; j++)
            {
                Kids.Add(m_StagePlay.sceneLoader.CanvasList[k].transform.GetChild(0).GetChild(j).gameObject);
                if (m_StagePlay.sceneLoader.CanvasList[k].transform.GetChild(0).GetChild(j).tag == "RawImage")
                {
                    image = m_StagePlay.sceneLoader.CanvasList[k].transform.GetChild(0).GetChild(j).GetComponent<RawImage>();
                }
            }
        }
        
        for(int k=0;k<m_StagePlay.sceneLoader.character3DDict.Count;k++)
        {
            if(m_StagePlay.sceneLoader.character3DDict[k].tag=="Photo")
            {
                Kids.Add(m_StagePlay.sceneLoader.character3DDict[k]);
            }
        }

        for(int k=0;k<m_StagePlay.sceneLoader.object3DDict.Count;k++)
        {
            if(m_StagePlay.sceneLoader.object3DDict[k].tag=="Photo")
            {
                Kids.Add(m_StagePlay.sceneLoader.object3DDict[k]);
            }
        }        
    }
       
    public void TakeShot()
    {
        //StartCoroutine(CaptureIt());
        if (XML_Reader.Instance.scenarioToDict.StageSetDictionary[m_StagePlay.sceneLoader.currentStage].PageList[m_StagePlay.Index].EventType == "TakeCapture")
        {
            StartCoroutine(TakeScreenshotAndSave());
        }
        else if(XML_Reader.Instance.scenarioToDict.StageSetDictionary[m_StagePlay.sceneLoader.currentStage].PageList[m_StagePlay.Index].EventType == "TakeShot")
        {
            //Destroy(image);
            StartCoroutine(TakeScreenShot());
        }     
        else if(XML_Reader.Instance.scenarioToDict.StageSetDictionary[m_StagePlay.sceneLoader.currentStage].PageList[m_StagePlay.Index].EventType == "Paint")
        {            
            StartCoroutine(SavePaint());
        }
    }
    
    private IEnumerator TakeScreenshotAndSave()
    {
        GameObject.Find("CaptureCanvas").GetComponent<Canvas>().enabled = false;
        TakeShotWithKids(Kids, true);

        yield return new WaitForEndOfFrame();

        Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        ss.Apply();

        GameObject bl = Instantiate(blink) as GameObject;
        bl.transform.SetParent(blParent.transform, false);

        GameObject.Find("CaptureCanvas").GetComponent<Canvas>().enabled = true;

        TakeShotWithKids(Kids, true);

        string className = "EduApp_Social";
        string name = string.Format("{0}_Capture{1}_{2}.png", className, "{0}",System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
        Debug.Log("Permission result: " + NativeGallery.SaveImageToGallery(ss, className + " Captures", name));
        //Debug.Log("Permission result: " + NativeGallery.SaveImageToGallery(ss, "GalleryTest", "My img{0}.png"));
        Destroy(ss);
    }

    private IEnumerator TakeScreenShot()
    {
        if (image != null)
        {
            image.texture = null;
        }
        GameObject.Find("CaptureCanvas").GetComponent<Canvas>().enabled = false;        

        yield return new WaitForEndOfFrame();

        texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        texture.Apply();        
                
        GameObject bl = Instantiate(blink) as GameObject;
        bl.transform.SetParent(blParent.transform, false);

        GameObject.Find("CaptureCanvas").GetComponent<Canvas>().enabled = true;
    }

    private IEnumerator SavePaint()
    {
        if(image!=null)
        {
            image.texture = null;
        }

        GameObject.Find("PaintUICanvas").GetComponent<Canvas>().enabled = false;
        yield return new WaitForEndOfFrame();

        texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        texture.Apply();

        GameObject bl = Instantiate(blink) as GameObject;
        bl.transform.SetParent(blParent.transform, false);

        GameObject.Find("PaintUICanvas").GetComponent<Canvas>().enabled = true;
    }
    public void SelectTexture()
    {
        if (image != null)
        {
            if (ARCoreSession != null)
            {
                if (ARCoreSession.DeviceCameraDirection == GoogleARCore.DeviceCameraDirection.BackFacing)
                {
                    image.uvRect = new Rect(0, 0, 1, 1);
                }

                else
                {
                    image.uvRect = new Rect(0, 0, 1, -1);
                }
            }
            image.texture = texture;
            PlayerInfo.Instance.isComplite = true;
            m_StagePlay.forwardDown();
        }
        else
        {
            return;
        }
    }

    public void TakeShotWithKids(List<GameObject> _kidsList, bool isShow)
    {
        if (_kidsList == null)
            return;

        for (int i = 0; i < _kidsList.Count; i++)
        {
            _kidsList[i].SetActive(isShow);
        }

    }

    protected const string MEDIA_STORE_IMAGE_MEDIA = "android.provider.MediaStore$Images$Media";
    protected static AndroidJavaObject m_Activity;

    protected static string SaveImageToGallery(Texture2D a_Texture, string a_Title, string a_Description)
    {
        using (AndroidJavaClass mediaClass = new AndroidJavaClass(MEDIA_STORE_IMAGE_MEDIA))
        {
            using (AndroidJavaObject contentResolver = Activity.Call<AndroidJavaObject>("getContentResolver"))
            {
                AndroidJavaObject image = Texture2DToAndroidBitmap(a_Texture);
                return mediaClass.CallStatic<string>("insertImage", contentResolver, image, a_Title, a_Description);
            }
        }
    }

    protected static AndroidJavaObject Texture2DToAndroidBitmap(Texture2D a_Texture)
    {
        byte[] encodedTexture = a_Texture.EncodeToPNG();
        using (AndroidJavaClass bitmapFactory = new AndroidJavaClass("android.graphics.BitmapFactory"))
        {
            return bitmapFactory.CallStatic<AndroidJavaObject>("decodeByteArray", encodedTexture, 0, encodedTexture.Length);
        }
    }

    protected static AndroidJavaObject Activity
    {
        get
        {
            if (m_Activity == null)
            {
                AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                m_Activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            }
            return m_Activity;
        }
    }

    public void OpenAndroidGallery()
    {
        #region [ Intent intent = new Intent(); ]
        //instantiate the class Intent
        AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
        //instantiate the object Intent
        AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
        #endregion [ Intent intent = new Intent(); ]
        #region [ intent.setAction(Intent.ACTION_VIEW); ]
        //call setAction setting ACTION_SEND as parameter
        intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_VIEW"));
        #endregion [ intent.setAction(Intent.ACTION_VIEW); ]
        #region [ intent.setData(Uri.parse("content://media/internal/images/media")); ]
        //instantiate the class Uri
        AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
        //instantiate the object Uri with the parse of the url's file
        AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", "content://media/internal/images/media");
        //call putExtra with the uri object of the file
        intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), uriObject);
        #endregion [ intent.setData(Uri.parse("content://media/internal/images/media")); ]
        //set the type of file
        intentObject.Call<AndroidJavaObject>("setType", "image/jpeg");
        #region [ startActivity(intent); ]
        //instantiate the class UnityPlayer
        AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        //instantiate the object currentActivity
        AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
        //call the activity with our Intent
        currentActivity.Call("startActivity", intentObject);
        #endregion [ startActivity(intent); ]
    }
}
