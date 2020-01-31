using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Threading.Tasks;

public class AssetBundleMgr : MonoBehaviour
{

    public Text noticeText;
    private bool bLoading = false;

    string fileName = "mobile_ver";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AssetBundleLoadButtonEvent()
    {
        Debug.Log("click load");

        if (true == bLoading)
            return;

        //StartCoroutine(AssetBundleLoadFromLocal());
        StartCoroutine(AssetBudleDownloadLocal());
    }
    // download from Firebase Storage..
    IEnumerator AssetBudleDownloadLocal()
    {
        bLoading = true;        

        // 로컬 방식....
        Firebase.Storage.FirebaseStorage storage = Firebase.Storage.FirebaseStorage.DefaultInstance;
        // Create a storage reference from our storage service
        Firebase.Storage.StorageReference storage_ref = storage.GetReferenceFromUrl("gs://fir-authtest22.appspot.com");
        // Create a reference to the file you want to upload
        Firebase.Storage.StorageReference rivers_ref = storage_ref.Child("AssetBundleTest1/" + fileName);

        string local_file = "";
        if (Application.platform == RuntimePlatform.Android)
        {
            if (!Directory.Exists(Application.persistentDataPath + "/" + "AssetBundle"))            //폴더가 있는지 체크하고 없으면 만듭니다.
            {
                Directory.CreateDirectory(Application.persistentDataPath + "/" + "AssetBundle");
            }
            if(File.Exists(Application.persistentDataPath + "/" + "AssetBundle" + "/" + fileName))
            {
                StartCoroutine(AssetBundleLoadFromLocal());
                yield break;
            }
            local_file = Application.persistentDataPath + "/" + "AssetBundle" + "/" + fileName;
        }
        else
        {
            if (File.Exists("C:/Users/Gana/Downloads/AssetBundle/AssetBundle_PC" + "/" + fileName))
            {
                StartCoroutine(AssetBundleLoadFromLocal());
                yield break;
            }
            local_file = "C:/Users/Gana/Downloads/AssetBundle/AssetBundle_PC" + "/" + fileName;
        }

        //var TmpTask = rivers_ref.GetFileAsync(local_file).ContinueWith(task => {
        var TmpTask = rivers_ref.GetFileAsync(local_file, new Firebase.Storage.StorageProgress<Firebase.Storage.DownloadState>(state =>
        {
            Debug.Log(string.Format("Progress: {0} of {1} bytes transferred.", state.BytesTransferred, state.TotalByteCount));
            PercentView(state.BytesTransferred, state.TotalByteCount);

        })).ContinueWith(task => {
            Debug.Log(string.Format("OnClickDownload::IsCompleted:{0} IsCanceled:{1} IsFaulted:{2}", task.IsCompleted, task.IsCanceled, task.IsFaulted));
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log(task.Exception.ToString());
                // Uh-oh, an error occurred!
                //authUI.ShowNotice("Error....");
                Debug.Log("Oops,, Error..");
                bLoading = false;
            }
            else
            {
                Debug.Log("Finished downloading...");
            }
        });

        yield return new WaitUntil(() => TmpTask.IsCompleted);
        PrintState("Downloading Complete");
        StartCoroutine(AssetBundleLoadFromLocal());
    }

    IEnumerator AssetBundleLoadFromLocal()
    {
        string assetBundleDirectory = "";
        if (Application.platform == RuntimePlatform.Android)
        {
            assetBundleDirectory = Application.persistentDataPath + "/" + "AssetBundle";            
        }
        else
        {
            assetBundleDirectory = "C:/Users/Gana/Downloads/AssetBundle/AssetBundle_PC";
        }
        //var myLoadedAssetBundle = AssetBundle.LoadFromFile(Path.Combine(assetBundleDirectory, "assetbundle_0"));
        var myLoadedAssetBundle2 = AssetBundle.LoadFromFileAsync(Path.Combine(assetBundleDirectory, fileName));
        if (!myLoadedAssetBundle2.isDone)
        {            
            PrintState("plaese wait...");
            yield return myLoadedAssetBundle2;
        }

        /*
        // 성공... pc and mobile...
        WWW www = WWW.LoadFromCacheOrDownload(new System.Uri(assetBundleDirectory + "/" + "mobile_ver").AbsoluteUri, 0);        
        if(!www.isDone)
        {
            Debug.Log("progress: " + www.progress);
            PrintState("progress: " + www.progress);
            yield return www;
        }        
        var myLoadedAssetBundle = www.assetBundle;
        yield return myLoadedAssetBundle;
        
        //AssetBundleRequest bundleRequest = myLoadedAssetBundle.LoadAllAssetsAsync(typeof(GameObject));
        //yield return bundleRequest;
        
        //var myLoadedAssetBundle = myLoadedAssetBundle2.assetBundle;
        if (myLoadedAssetBundle == null)
        {
            Debug.Log("Fail Load");
            PrintState("Fail Load");
            bLoading = false;
            yield break;
        }        
        else
        {
            Debug.Log("Successed to load AssetBundle");
        }
        */

        var myLoadedAssetBundle = myLoadedAssetBundle2.assetBundle;
        if (myLoadedAssetBundle == null)
        {
            Debug.Log("Fail Load");
            PrintState("Fail Load");
            bLoading = false;
            yield break;
        }
        else
        {
            Debug.Log("Successed to load AssetBundle");
            PrintState("Successed to load AssetBundle");
        }

        //var prefab = myLoadedAssetBundle.LoadAsset<GameObject>("Sphere");
        //Instantiate(prefab);        
        //var prefab2 = myLoadedAssetBundle.LoadAsset<GameObject>("Image");
        //Instantiate(prefab2.gameObject);

        
        //비디오 플레이어를 생성합니다.
        GameObject prefab3 = myLoadedAssetBundle.LoadAsset<GameObject>("Video Player");
        GameObject child = Instantiate(prefab3) as GameObject;
        child.name = "Video Player";
        child.GetComponent<VideoPlayer>().playOnAwake = false;
        //비디오 클립을 생성합니다.

        //"D:/Unity2019.1.0f2_Projects/FirebaseAuthTest4/Assets/Video/";
        //child.GetComponent<VideoPlayer>().clip = 
        child.GetComponent<VideoPlayer>().targetCamera = Camera.main;

        child.GetComponent<VideoPlayer>().source = VideoSource.Url;
        child.GetComponent<VideoPlayer>().url = "https://firebasestorage.googleapis.com/v0/b/fir-authtest22.appspot.com/o/Test1%2Ftest_movie_2.mp4?alt=media&token=04050b8f-ca56-4fd5-8b91-15ec1784755f";
        //child.GetComponent<VideoPlayer>().url = "https://firebasestorage.googleapis.com/v0/b/fir-authtest22.appspot.com/o/Test1%2FSYJ.mp4?alt=media&token=b9c752e3-5876-411d-a4b0-8d4d8a5321bb";
        
        /*
        child.GetComponent<VideoPlayer>().url = "D:/Unity2019.1.0f2_Projects/FirebaseAuthTest4/Assets/Video/test_movie_2.mp4";
        if (!child.GetComponent<VideoPlayer>().isPrepared)
            yield return null;

        child.GetComponent<VideoPlayer>().Play();
        */

        //AssetBundleRequest assetClip = myLoadedAssetBundle.LoadAssetAsync<VideoClip>("SYJ");
        //yield return assetClip;

        //VideoClip newVideoClip = assetClip.asset as VideoClip;

        //비디오를 설정하고 재생합니다.
        //child.GetComponent<VideoPlayer>().clip = newVideoClip;                  // 현재 Empty 에러 발생...
        if (!child.GetComponent<VideoPlayer>().isPrepared)
            yield return null;

        child.GetComponent<VideoPlayer>().Play();
        

        /*
        AssetBundleRequest assetImg = myLoadedAssetBundle.LoadAssetAsync<GameObject>("Image");
        yield return assetImg;
        
        GameObject rw = assetImg.asset as GameObject;
        GameObject child = Instantiate(rw) as GameObject;

        child.transform.parent = FindObjectOfType<Canvas>().transform;
        child.name = "Kei";
        child.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
        child.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
        child.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
        child.GetComponent<RectTransform>().anchoredPosition = new Vector2(-250.0f, -80.0f);
        //child.GetComponent<RectTransform>().sizeDelta = new Vector2(300.0f, 260.0f);
        child.GetComponent<RectTransform>().sizeDelta = new Vector2(900.0f, 600.0f);
        //child.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(-250.0f, -80.0f, 0.0f);
        child.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0f, 0.0f, 0.0f);
        child.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
        child.GetComponent<RectTransform>().localEulerAngles = new Vector3(0f, 0f, 0f);
        */
        //myLoadedAssetBundle.Unload(false);

        bLoading = false;
        PrintState("Loading Complete");
    }


    //================================================== VIDEO ===================================================//
    void PercentView(long _presentTransfer, long _transferCount)
    {
        double presentTransfer = _presentTransfer;
        double transferCount = _transferCount;
        double percent = (presentTransfer / transferCount) * 100;
        percent = Mathf.Round((float)percent);
        string strPercent = percent.ToString();
        string stringPercent = "Progress : " + strPercent + " %";
        PrintState(stringPercent);
    }

    void PrintState(string _strMsg)
    {
        noticeText.text = _strMsg;
    }

}
