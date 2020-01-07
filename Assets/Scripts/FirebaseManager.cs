using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.IO;

public class FirebaseManager : MonoBehaviour
{    
    public AuthUIMgr lobbyUI;                                                           // 삭제 예정...
    Firebase.Auth.FirebaseAuth auth;
    Firebase.Auth.FirebaseUser user;

    [Header("Login")]
    public InputField userId;
    public InputField userPassword;

    [Header("Sign Up")]
    public InputField signupEmail;
    public InputField signupPassword;
    public InputField signupConfirmPassword;

    [Header("DownLoad")]
    public GameObject mainPart;
    public GameObject downloadPart;

    [Header("Video Play")]
    public GameObject videoPlayer;

    [Header("Etc")]
    public Text noticeText;
    public RawImage rawImage;
    
    private string fileName = "group-1920-1080_ori.png";
    private string assetBundleName = "mobile_ver";
    private bool bLoading = false;

    private bool bInit = false;
    private bool bXmlFile = false;
    private bool bXmlFileQuiz = false;


    // 수정될 수 있음...
    private static FirebaseManager instance = null;
    private static readonly object padlock = new object();

    public static FirebaseManager Instance
    {
        get
        {
            lock (padlock)
            {
                if (instance == null)
                {
                    instance = new FirebaseManager();
                }
                return instance;
            }
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);
        InitializeFirebase();        
    }
    
    void OnDestroy()
    {
        auth.StateChanged -= AuthStateChanged;
    }

    //==================================================================================================//
    //=========================================== FIREBASE AUTH ===========================================//
    //==================================================================================================//    
    void InitializeFirebase()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
        bInit = true;
    }
    // 유저의 상태 변화...
    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;
            if (!signedIn && user != null)
            {
                Debug.Log("Signed out " + user.UserId);
                lobbyUI.FireBasePartButtonEvent();
            }
            user = auth.CurrentUser;
            if (signedIn)
            {
                Debug.Log("Signed in " + user.UserId);
                //displayName = user.DisplayName ?? "";
                //emailAddress = user.Email ?? "";
                //photoUrl = user.PhotoUrl ?? "";
                Debug.Log("display Name: " + user.DisplayName);

                // 성공시에...
                lobbyUI.ShowFirebaseLogginPanel();
                lobbyUI.firebaseLoggedinID.text = user.Email;
                lobbyUI.firebaseLoggedinText.text = "Log in Success";
                //lobbyUI.ShowNotice("Log In Complete..");
            }
        }
    }        

    public bool GetInit()
    {
        return bInit;
    }
    //================================================================================================//
    //============================================= LOG IN ==============================================//
    //================================================================================================//
    public void FirebaseLoginButtonEvent()
    {                        
        TryLoginWithFirebaseAuth(userId.text, userPassword.text);
    }
    void TryLoginWithFirebaseAuth(string email, string password)
    {
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                //Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                Debug.Log("SignInWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.Log("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }
            // log in 성공시...
            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})", newUser.DisplayName, newUser.UserId);
        });
    }
    //================================================================================================//
    //============================================ SIGN UP ==============================================//
    //================================================================================================//
    public void FirebaseSignUpButtonEvent()
    {        
        lobbyUI.ShowFirebaseSignUpPanel();
    }
    public void FirebaseCreateAccountButtonEvent()
    {        
        TrySignupWifhFirebaseAuth(signupEmail.text, signupPassword.text, signupConfirmPassword.text);
    }
    void TrySignupWifhFirebaseAuth(string email, string password, string confirmPassword)
    {
        if (password != confirmPassword)                                            // 비번과 비번확인 일치하지 않음...
        {
            Debug.Log("pass word not match!");
            return;
        }
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }
            // Firebase user has been created.      // 계정 생성 완료...            
            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("Firebase user created successfully: {0} ({1})", newUser.DisplayName, newUser.UserId);
        });
    }
    
    //============================================ LOG OUT ==============================================//    
    public void FirebaseLogOutButtonEvent()
    {        
        auth.SignOut();        
    }


    //==================================================================================================//
    //==================================================================================================//
    //=============================================  STORAGE ==============================================//
    //==================================================================================================//
    //==================================================================================================//

    //==================================================================================================//
    //==============================================  UPLOAD =============================================//
    //==================================================================================================//
    public void FirebaseUploadLocalButtonEvent()
    {
        Debug.Log("=== upload begin ===");

        StartCoroutine(UploadProcessLocal());                       
        //StartCoroutine(UploadProcessByteBufferPNGFormat());
    }
    
    // 로컬 방식 업로드 부분... 로컬 방식이기 때문에 모든 포멧이든 다 된다.
    IEnumerator UploadProcessLocal()
    {
        bool bError = false;
        string bMsg = "";
        PrintState("upload begin...");

        // Get a reference to the storage service, using the default Firebase App
        Firebase.Storage.FirebaseStorage storage = Firebase.Storage.FirebaseStorage.DefaultInstance;
        // Create a storage reference from our storage service
        Firebase.Storage.StorageReference storage_ref = storage.GetReferenceFromUrl("gs://eduplatform-97d55.appspot.com/");                          // FireaBase 계정의 address 적는 곳.
        // Create a reference to 'images/mountains.jpg'        
        // While the file names are the same, the references point to different files
        //mountains_ref.Name == mountain_images_ref.Name; // true
        //mountains_ref.Path == mountain_images_ref.Path; // false

        string local_file = "";
        if (Application.platform == RuntimePlatform.Android)
        {
            local_file = "file:// " + Application.persistentDataPath + "/" + fileName;                        // android 휴대폰의 접근 가능한 로컬 주소
        }
        else
        {
            local_file = "C:/Users/Gana/Downloads/project_stuff/" + fileName;                               // pc 의 로컬 주소..
        }
        Firebase.Storage.StorageReference rivers_ref = storage_ref.Child("stuff/" + fileName);                      // 스토리지에 레퍼런스 먼저 만들기..
        // metadata.. 추가할 것 이 있으면 추가,,, 필요 없을 시에 null 로..
        //var new_metadata = new Firebase.Storage.MetadataChange();
        //new_metadata.ContentType = "lyrics/text";       
        // 메타데이터 추가시에....
        // Task TmpTask = rivers_ref.PutFileAsync(local_file, null, new Firebase.Storage.StorageProgress<Firebase.Storage.UploadState>(state =>         

        Task TmpTask = rivers_ref.PutFileAsync(local_file, null, new Firebase.Storage.StorageProgress<Firebase.Storage.UploadState>(state =>
        {
            // Process 의 진행율을 보여주는 부분.. 
            Debug.Log(string.Format("Progress: {0} of {1} bytes transferred.", state.BytesTransferred, state.TotalByteCount));
            PercentView(state.BytesTransferred, state.TotalByteCount);
        }), System.Threading.CancellationToken.None, null).ContinueWith(task =>
        {
            Debug.Log(string.Format("OnClickUpload::IsCompleted:{0} IsCanceled:{1} IsFaulted:{2}", task.IsCompleted, task.IsCanceled, task.IsFaulted));
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log(task.Exception.ToString());
                // Uh-oh, an error occurred!                       
                bError = true;
                bMsg = "error: " + task.Exception.ToString();
            }
            else if (task.IsCompleted)
            {
                // Metadata contains file metadata such as size, content-type, and download URL.
                Firebase.Storage.StorageMetadata metadata = task.Result;
                Debug.Log("Finished uploading...");
                bError = false;
            }
        });        
        yield return new WaitUntil(() => TmpTask.IsCompleted);              // 업로드가 완전히 완료된 후에 나머지를 처리한다.
        if (false == bError)
            PrintState("upload complete...");
        else if (true == bError)
            PrintState(bMsg);         
    }    
    // png 이미지 파일을 byte buffer 형식으로 업로드...
    IEnumerator UploadProcessByteBufferPNGFormat()
    {
        yield return new WaitForEndOfFrame();

        Firebase.Storage.FirebaseStorage storage = Firebase.Storage.FirebaseStorage.DefaultInstance;
        // Create a storage reference from our storage service
        Firebase.Storage.StorageReference storage_ref = storage.GetReferenceFromUrl("gs://eduplatform-97d55.appspot.com/");

        string local_file = "";
        if (Application.platform == RuntimePlatform.Android)
        {
            local_file = Application.persistentDataPath + "/" + fileName;
        }
        else
        {
            local_file = "C:/Users/Gana/Downloads/Test_imgs/" + fileName;
        }

        Firebase.Storage.StorageReference rivers_ref = storage_ref.Child("stuff/" + fileName);
        WWW www = new WWW("file:///" + local_file);
        while (!www.isDone)
            yield return null;

        Texture2D tmpTexture = new Texture2D(www.texture.width, www.texture.height, TextureFormat.RGB24, false);
        tmpTexture.SetPixels(www.texture.GetPixels());
        tmpTexture.Apply();
        byte[] bytes = tmpTexture.EncodeToPNG();

        var TmpTask = rivers_ref.PutBytesAsync(bytes, null, new Firebase.Storage.StorageProgress<Firebase.Storage.UploadState>(state =>
        {
            Debug.Log(string.Format("Progress: {0} of {1} bytes transferred.", state.BytesTransferred, state.TotalByteCount));
            PercentView(state.BytesTransferred, state.TotalByteCount);
        }), System.Threading.CancellationToken.None, null).ContinueWith(task =>
        {
            Debug.Log(string.Format("OnClickUpload::IsCompleted:{0} IsCanceled:{1} IsFaulted:{2}", task.IsCompleted, task.IsCanceled, task.IsFaulted));
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log(task.Exception.ToString());
                // Uh-oh, an error occurred!
            }
            else
            {
                // Metadata contains file metadata such as size, content-type, and download URL.
                Firebase.Storage.StorageMetadata metadata = task.Result;
                Debug.Log("Finished uploading...");
                //foreach (var entry in metadata.DownloadUrls)
                //    Debug.Log("download url = " + entry);
            }
        });
        yield return new WaitUntil(() => TmpTask.IsCompleted);
        PrintState("uploading complete..");
    }    
    // mp4 movie upload...  테스트 안 해봤음. 작업해야 함....
    IEnumerator UploadProcessByteBufferMP4Format()
    {
        yield return new WaitForEndOfFrame();

        Firebase.Storage.FirebaseStorage storage = Firebase.Storage.FirebaseStorage.DefaultInstance;
        // Create a storage reference from our storage service
        Firebase.Storage.StorageReference storage_ref = storage.GetReferenceFromUrl("gs://eduplatform-97d55.appspot.com/");

        string local_file = "";
        if (Application.platform == RuntimePlatform.Android)
        {
            local_file = Application.persistentDataPath + "/" + fileName;
        }
        else
        {
            local_file = "C:/Users/Gana/Downloads/Test_imgs/" + fileName;
        }

        Firebase.Storage.StorageReference rivers_ref = storage_ref.Child("stuff/" + fileName);
        WWW www = new WWW("file:///" + local_file);
        while (!www.isDone)
            yield return null;

        Texture2D tmpTexture = new Texture2D(www.texture.width, www.texture.height, TextureFormat.RGB24, false);
        tmpTexture.SetPixels(www.texture.GetPixels());
        tmpTexture.Apply();
        byte[] bytes = tmpTexture.EncodeToPNG();

        var TmpTask = rivers_ref.PutBytesAsync(bytes, null, new Firebase.Storage.StorageProgress<Firebase.Storage.UploadState>(state =>
        {
            Debug.Log(string.Format("Progress: {0} of {1} bytes transferred.", state.BytesTransferred, state.TotalByteCount));
            //PercentView(state.BytesTransferred, state.TotalByteCount);

        }), System.Threading.CancellationToken.None, null).ContinueWith(task =>
        {
            Debug.Log(string.Format("OnClickUpload::IsCompleted:{0} IsCanceled:{1} IsFaulted:{2}", task.IsCompleted, task.IsCanceled, task.IsFaulted));

            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log(task.Exception.ToString());
                // Uh-oh, an error occurred!
            }
            else
            {
                // Metadata contains file metadata such as size, content-type, and download URL.
                Firebase.Storage.StorageMetadata metadata = task.Result;
                Debug.Log("Finished uploading...");
                //foreach (var entry in metadata.DownloadUrls)
                //    Debug.Log("download url = " + entry);
            }
        });

        yield return new WaitUntil(() => TmpTask.IsCompleted);
        //PrintState("uploading complete..");
    }
    //====================================================================================================//
    //==============================================  DOWNLOAD =============================================//    
    //====================================================================================================//


    //===============================================  BUTTON ==============================================//    
    public void FirebaseDownloadButtonEvent()
    {
        mainPart.SetActive(false);
        downloadPart.SetActive(true);
    }
    public void FirebaseLocalDownloadButtonEvent()
    {        
        StartCoroutine(LocalDownloadStart());
    }
    public void FirebaseCacheImgDownloadButtonEvent()
    {        
        StartCoroutine(DownloadCacheImgByBuffer());
    }
    public void FirebaseCacheTextDownloadButtonEvent()
    {        
        StartCoroutine(DownloadByBufferTxt());
    }

    //===========================================================================================================//        
    //============================================== DOWNLOAD FIREBASE =============================================//        
    //==================================================== XML ===================================================//        
    //===========================================================================================================//        

    // xml....
    // first xml file...
    public void XmlFileDownload(string _fileName)
    {
        bXmlFile = false;
        StartCoroutine(LocalDownloadXmlFile(_fileName));
    }

    IEnumerator LocalDownloadXmlFile(string _fileName)                                                                                                            // 로컬 다운로드 방식
    {
        // test....
        fileName = _fileName;
        Firebase.Storage.FirebaseStorage storage = Firebase.Storage.FirebaseStorage.DefaultInstance;
        // Create a storage reference from our storage service        
        Firebase.Storage.StorageReference storage_ref = storage.GetReferenceFromUrl("gs://eduplatform-97d55.appspot.com/");                                         // 파이어베이스 계정에 할당된 주소...
        // Create a reference to the file you want to upload        
        Firebase.Storage.StorageReference rivers_ref = storage_ref.Child("XmlFile_Art_Buyeo/" + fileName);                                                                                               // 하위 폴더...

        string local_file = "";
        // 경로 지정....
        if (Application.platform == RuntimePlatform.Android)
            local_file = Application.persistentDataPath + "/" + fileName;                                           // android 의 접근 가능한 주소,, 폴더 추가 가능..        
        else
            local_file = Application.streamingAssetsPath + "/" + fileName;

        Task TmpTask = rivers_ref.GetFileAsync(local_file, new Firebase.Storage.StorageProgress<Firebase.Storage.DownloadState>(state =>
        {
            // 다운로드 진행율 표시...
            Debug.Log(string.Format("Progress: {0} of {1} bytes transferred.", state.BytesTransferred, state.TotalByteCount));
            //PercentView(state.BytesTransferred, state.TotalByteCount);
        })).ContinueWith(task => {
            Debug.Log(string.Format("OnClickDownload::IsCompleted:{0} IsCanceled:{1} IsFaulted:{2}", task.IsCompleted, task.IsCanceled, task.IsFaulted));
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log(task.Exception.ToString());
                // Uh-oh, an error occurred!                
                Debug.Log("Oops,, Error..");
            }
            else
            {
                Debug.Log("Finished downloading...");
            }
        });
        yield return new WaitUntil(() => TmpTask.IsCompleted);
        bXmlFile = true;
    }
    public bool GetXmlFile()
    {
        return bXmlFile;
    }
    // second xml file...

    public void XmlQuizDownload(string _fileName)
    {
        bXmlFileQuiz = false;
        StartCoroutine(LocalDownloadXmlQuiz(_fileName));
    }

    IEnumerator LocalDownloadXmlQuiz(string _fileName)
    {
        // test....
        fileName = _fileName;
        Firebase.Storage.FirebaseStorage storage = Firebase.Storage.FirebaseStorage.DefaultInstance;
        // Create a storage reference from our storage service        
        Firebase.Storage.StorageReference storage_ref = storage.GetReferenceFromUrl("gs://eduplatform-97d55.appspot.com/");                                         // 파이어베이스 계정에 할당된 주소...
        // Create a reference to the file you want to upload        
        Firebase.Storage.StorageReference rivers_ref = storage_ref.Child("XmlFile_Art_Buyeo/" + fileName);                                                                                               // 하위 폴더...

        string local_file = "";
        // 경로 지정....
        if (Application.platform == RuntimePlatform.Android)
            local_file = Application.persistentDataPath + "/" + fileName;                                           // android 의 접근 가능한 주소,, 폴더 추가 가능..        
        else
            local_file = Application.streamingAssetsPath + "/" + fileName;

        Task TmpTask = rivers_ref.GetFileAsync(local_file, new Firebase.Storage.StorageProgress<Firebase.Storage.DownloadState>(state =>
        {
            // 다운로드 진행율 표시...
            Debug.Log(string.Format("Progress: {0} of {1} bytes transferred.", state.BytesTransferred, state.TotalByteCount));
            //PercentView(state.BytesTransferred, state.TotalByteCount);
        })).ContinueWith(task => {
            Debug.Log(string.Format("OnClickDownload::IsCompleted:{0} IsCanceled:{1} IsFaulted:{2}", task.IsCompleted, task.IsCanceled, task.IsFaulted));
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log(task.Exception.ToString());
                // Uh-oh, an error occurred!                
                Debug.Log("Oops,, Error..");
            }
            else
            {
                Debug.Log("Finished downloading...");
            }
        });
        yield return new WaitUntil(() => TmpTask.IsCompleted);
        bXmlFileQuiz = true;
    }

    public bool GetXmlFileQuiz()
    {
        return bXmlFileQuiz;
    }

    //===========================================================================================================//        






    IEnumerator LocalDownloadStart()                                                                                                            // 로컬 다운로드 방식
    {
        // test....
        fileName = "assetbundle_1.0";        
        Firebase.Storage.FirebaseStorage storage = Firebase.Storage.FirebaseStorage.DefaultInstance;
        // Create a storage reference from our storage service        
        Firebase.Storage.StorageReference storage_ref = storage.GetReferenceFromUrl("gs://eduplatform-97d55.appspot.com/");                                         // 파이어베이스 계정에 할당된 주소...
        // Create a reference to the file you want to upload        
        Firebase.Storage.StorageReference rivers_ref = storage_ref.Child("AssetBundle/" + fileName);                                                                                               // 하위 폴더...

        string local_file = "";        
        // 경로 지정....
        if (Application.platform == RuntimePlatform.Android)        
            local_file = Application.persistentDataPath + "/" + fileName;                                           // android 의 접근 가능한 주소,, 폴더 추가 가능..        
        else        
            local_file = "C:/Users/Gana/Downloads/" + fileName;
        
        Task TmpTask = rivers_ref.GetFileAsync(local_file, new Firebase.Storage.StorageProgress<Firebase.Storage.DownloadState>(state =>
        {
            // 다운로드 진행율 표시...
            Debug.Log(string.Format("Progress: {0} of {1} bytes transferred.", state.BytesTransferred, state.TotalByteCount));                              
            //PercentView(state.BytesTransferred, state.TotalByteCount);
        })).ContinueWith(task => {
            Debug.Log(string.Format("OnClickDownload::IsCompleted:{0} IsCanceled:{1} IsFaulted:{2}", task.IsCompleted, task.IsCanceled, task.IsFaulted));
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log(task.Exception.ToString());
                // Uh-oh, an error occurred!                
                Debug.Log("Oops,, Error..");
            }
            else
            {
                Debug.Log("Finished downloading...");
            }
        });
        yield return new WaitUntil(() => TmpTask.IsCompleted);        
    }    


    IEnumerator DownloadCacheImgByBuffer()                                                                      // 이미지를 로컬에 저장하지 않고, cache 메모리에 저장 후 화면에 표시..
    {
        bool bError = false;
        byte[] fileContents2 = null;

        Firebase.Storage.FirebaseStorage storage = Firebase.Storage.FirebaseStorage.DefaultInstance;
        // Create a storage reference from our storage service
        //Firebase.Storage.StorageReference storage_ref = storage.GetReferenceFromUrl("gs://fir-authtest22.appspot.com");
        Firebase.Storage.StorageReference storage_ref = storage.GetReferenceFromUrl("gs://eduplatform-97d55.appspot.com/");
        // Create a reference to the file you want to upload
        Firebase.Storage.StorageReference rivers_ref = storage_ref.Child("ccc/" + fileName);

        // 최대 사이즈 지정할 수 있음..
        const long maxAllowedSize = 5 * 1024 * 1024;                                    // 최대사이즈 일단 5mb
        var TmpTask = rivers_ref.GetBytesAsync(maxAllowedSize, new Firebase.Storage.StorageProgress<Firebase.Storage.DownloadState>(state =>
        {
            // 다운로드 진행율 표시...
            Debug.Log(string.Format("Progress: {0} of {1} bytes transferred.", state.BytesTransferred, state.TotalByteCount));
            PercentView(state.BytesTransferred, state.TotalByteCount);

        })).ContinueWith((System.Threading.Tasks.Task<byte[]> task) => {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log(task.Exception.ToString());
                // Uh-oh, an error occurred!
                bError = true;
            }
            else if (task.IsCompleted)
            {
                Debug.Log("Finished downloading!");
                fileContents2 = task.Result;
                bError = false;
            }
        });        
        yield return new WaitUntil(() => TmpTask.IsCompleted);

        if (false == bError)
        {
            Texture2D tmpTexture = new Texture2D(16, 16, TextureFormat.RGB24, false);
            bool isLoaded = tmpTexture.LoadImage(fileContents2);
            while (!isLoaded)
                yield return null;

            tmpTexture.name = "tmpTexutre";
            rawImage.gameObject.SetActive(true);
            rawImage.texture = tmpTexture;            
            PrintState("image view");
        }
        else if (true == bError)
        {
            PrintState("Error...");
        }
    }    
    IEnumerator DownloadByBufferTxt()                                                                                  // 텍스트를 로컬에 저장하지 않고, cache 메모리에 저장 후 화면에 표시.. 한글은 UTF-8 만 가능.
    {
        bool bError = false;
        byte[] fileContents = null;

        Firebase.Storage.FirebaseStorage storage = Firebase.Storage.FirebaseStorage.DefaultInstance;
        // Create a storage reference from our storage service
        Firebase.Storage.StorageReference storage_ref = storage.GetReferenceFromUrl("gs://eduplatform-97d55.appspot.com/");
        // Create a reference to the file you want to upload
        Firebase.Storage.StorageReference rivers_ref = storage_ref.Child("ccc/text_1.txt");

        // 최대 사이즈....
        const long maxAllowedSize = 2 * 1024 * 1024;    // 2mb
        var TmpTask = rivers_ref.GetBytesAsync(maxAllowedSize, new Firebase.Storage.StorageProgress<Firebase.Storage.DownloadState>(state =>
        {
            // 다운로드 진행율 표시...
            Debug.Log(string.Format("Progress: {0} of {1} bytes transferred.", state.BytesTransferred, state.TotalByteCount));
            PercentView(state.BytesTransferred, state.TotalByteCount);

        })).ContinueWith((System.Threading.Tasks.Task<byte[]> task) => {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log(task.Exception.ToString());
                // Uh-oh, an error occurred!
                bError = true;
            }
            else if (task.IsCompleted)
            {
                Debug.Log("Finished downloading!");
                fileContents = task.Result;
                bError = false;
            }
        });
        yield return new WaitUntil(() => TmpTask.IsCompleted);

        if (false == bError)
        {
            string testText = System.Text.Encoding.UTF8.GetString(fileContents);                                       // UTF8로 인코딩하여서 변환... 
            PrintState("lyrics: " + testText);
        }
        else if (true == bError)
        {
            PrintState("Error...");
        }
    }
    //=======================================================================================================//
    //=================================================  DELETE ================================================//    
    //=======================================================================================================//
    public void FirebaseDelectFileButtonEvent()                                                                     // button event..
    {        
        StartCoroutine(DeleteProcess());
    }
    IEnumerator DeleteProcess()
    {
        yield return new WaitForEndOfFrame();

        Firebase.Storage.FirebaseStorage storage = Firebase.Storage.FirebaseStorage.DefaultInstance;
        // Create a storage reference from our storage service
        Firebase.Storage.StorageReference storage_ref = storage.GetReferenceFromUrl("gs://eduplatform-97d55.appspot.com/");
        // Create a reference to the file you want to upload
        Firebase.Storage.StorageReference rivers_ref = storage_ref.Child("ccc/" + fileName);                                                                         // 삭제하려는 파일의 레퍼런스...

        // Delete the file
        Task tmpTask = rivers_ref.DeleteAsync().ContinueWith(task => {
            if (task.IsCompleted)
            {
                Debug.Log("File deleted successfully.");
            }
            else
            {
                Debug.Log("error...");
            }
        });

        yield return new WaitUntil(() => tmpTask.IsCompleted);
        PrintState("delete file complete");
    }

    //=========================================================================================================//
    //=============================================  ASSET BUNDLE LOAD ============================================//    
    //=========================================================================================================//
    // 흠....

    public void FirebaseAssetBundleLoadButtonEvent()                                                            // button event...
    {        
        StartCoroutine(AssetBudleDownloadLocal());
    }
    IEnumerator AssetBudleDownloadLocal()                                                                               // 로컬 방식 다운로드..
    {
        bLoading = true;        
        Firebase.Storage.FirebaseStorage storage = Firebase.Storage.FirebaseStorage.DefaultInstance;
        // Create a storage reference from our storage service
        //Firebase.Storage.StorageReference storage_ref = storage.GetReferenceFromUrl("gs://fir-authtest22.appspot.com");
        Firebase.Storage.StorageReference storage_ref = storage.GetReferenceFromUrl("gs://eduplatform-97d55.appspot.com/");
        // Create a reference to the file you want to upload
        Firebase.Storage.StorageReference rivers_ref = storage_ref.Child("AssetBundleTest1/" + assetBundleName);                            // 업로드 확인해야 함...

        string local_file = "";
        if (Application.platform == RuntimePlatform.Android)
        {
            if (!Directory.Exists(Application.persistentDataPath + "/" + "AssetBundle"))                                                    //폴더가 있는지 체크하고 없으면 만든다.
            {
                Directory.CreateDirectory(Application.persistentDataPath + "/" + "AssetBundle");                                    
            }
            if (File.Exists(Application.persistentDataPath + "/" + "AssetBundle" + "/" + assetBundleName))
            {
                StartCoroutine(AssetBundleLoadFromLocal());                                                                                                 // 에셋번들 로드...
                yield break;
            }
            local_file = Application.persistentDataPath + "/" + "AssetBundle" + "/" + assetBundleName;
        }
        else
        {
            if (File.Exists("C:/Users/Gana/Downloads/AssetBundle/AssetBundle_PC" + "/" + assetBundleName))
            {
                StartCoroutine(AssetBundleLoadFromLocal());
                yield break;
            }
            local_file = "C:/Users/Gana/Downloads/AssetBundle/AssetBundle_PC" + "/" + assetBundleName;
        }
        
        Task TmpTask = rivers_ref.GetFileAsync(local_file, new Firebase.Storage.StorageProgress<Firebase.Storage.DownloadState>(state =>
        {
            // 다운로드 진행률....
            Debug.Log(string.Format("Progress: {0} of {1} bytes transferred.", state.BytesTransferred, state.TotalByteCount));
            PercentView(state.BytesTransferred, state.TotalByteCount);

        })).ContinueWith(task => {
            Debug.Log(string.Format("OnClickDownload::IsCompleted:{0} IsCanceled:{1} IsFaulted:{2}", task.IsCompleted, task.IsCanceled, task.IsFaulted));
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log(task.Exception.ToString());                                
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
        var myLoadedAssetBundle2 = AssetBundle.LoadFromFileAsync(Path.Combine(assetBundleDirectory, assetBundleName));
        if (!myLoadedAssetBundle2.isDone)
        {
            PrintState("plaese wait...");
            yield return myLoadedAssetBundle2;
        }

        // 아래처럼 WWW 를 활용해도 OK... progress 로 진행율 표시할 수 있음...
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
            PrintState("Fail Load");                            // 로드 실패...
            bLoading = false;
            yield break;
        }
        else
        {
            //Debug.Log("Successed to load AssetBundle");
            PrintState("Successed to load AssetBundle");
        }

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
        child.GetComponent<RectTransform>().sizeDelta = new Vector2(900.0f, 600.0f);        
        child.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0f, 0.0f, 0.0f);
        child.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
        child.GetComponent<RectTransform>().localEulerAngles = new Vector3(0f, 0f, 0f);        
        myLoadedAssetBundle.Unload(false);

        bLoading = false;
        PrintState("Loading Complete");
    }





















    //=========================================================================================================//
    //===============================================  VIDEO PLAY ================================================//    
    //=========================================================================================================//
    /*
    public void VideoPlayButtonEvent()                                                                                                      // button event...
    {        
        StartCoroutine(PlayVideoPlayer());
    }
    IEnumerator PlayVideoPlayer()
    {
        WaitForSeconds waitTime = new WaitForSeconds(0.1f);
        if (Application.platform == RuntimePlatform.Android)
        {            
            //videPlayer.url = Application.persistentDataPath +"/SYJ.mp4";            
            // firebase 에서 받아온 url 주소...
            videoPlayerSc.url = "https://firebasestorage.googleapis.com/v0/b/fir-authtest22.appspot.com/o/Test1%2FBeach%20Sunrise%20-%20360%C2%B0%20Video_1080.mp4?alt=media&token=598d19f3-56cc-4894-af08-9f008553f482";
        }
        else
        {
            //videoPlayerSc.url = System.IO.Path.Combine("C:/Users/Gana/Downloads/project_stuff", "SYJ.mp4");            
            videoPlayerSc.url = "https://firebasestorage.googleapis.com/v0/b/fir-authtest22.appspot.com/o/Test1%2FBeach%20Sunrise%20-%20360%C2%B0%20Video_1440.mp4?alt=media&token=a09b2edf-a6b5-48ab-89fa-e308c313abab";
        }

        while (!videoPlayerSc.isPrepared)
        {            
            PrintState("preparing video");
            yield return waitTime;
            break;
        }                
        videoPlayerSc.Play();        
        PrintState("playing video");
        mainPart.SetActive(false);
    }
    */
    //=============================================  EXTRA FUNCTION =============================================//    
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
