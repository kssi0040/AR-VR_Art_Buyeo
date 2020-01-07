using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Video;
using System.Threading.Tasks;

public class AuthManager : MonoBehaviour {

    AuthUIMgr authUI;
    Firebase.Auth.FirebaseAuth auth;
    Firebase.Auth.FirebaseUser user;

    // storage 관련..
    public Text noticeText;
    //public string[] imgNames = new string[10];
    //private int iImgIndex = 0;
    

    // video player...
    //public VideoPlayer videoPlayerSc;
    public RawImage image;
    public VideoPlayer videPlayer;        
    public RawImage rawImage;
    //public VideoPlayer videoPlayer;
    private string fileName = "text_1.txt";

    // Use this for initialization
    void Start () {
        authUI = this.GetComponent<AuthUIMgr>();        
        //InitializeFirebase();
        //Application.runInBackground = true;                
        
    }

    // Update is called once per frame
    void Update()
    {

    }    
    void OnDestroy()
    {
        auth.StateChanged -= AuthStateChanged;
    }




    //================================================================================================//
    //============================================= MYSQL =============================================//
    //============================================= LOGIN ==============================================//
    //================================================================================================//        
    // 텍스트와 사진 올리기, 다운받기,
    // 텍스트와 동영상 올리기, 다운 받기 등등의 기능 필요..        
    // 로그인 작업해야 함...
    // firebase도 같이 로그인 작업해야 함...

    IEnumerator MySqlLogIn(string _strUserID, string _strUserPW)
    {
        WWWForm form = new WWWForm();
        form.AddField("userID", _strUserID);
        form.AddField("userPW", _strUserPW);
        
        // 아래에서 php 바꿔서 처리한다.
        using (UnityWebRequest www = UnityWebRequest.Post("http://192.168.1.183/ARVR/pages/login_admission/loginTest.php", form))
        {
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                //authUI.ShowNotice("Log In Fail");
            }
            else
            {                
                string strHandlerText = www.downloadHandler.text;                
                string resultText = strHandlerText.Trim();                              // 무조건 뒤에 공백이 삽입되어 받는다.
                //Debug.Log("text: " + resultText);
                if ("password ERROR" == resultText)
                {
                    Debug.Log("password error");
                }
                else
                {
                    if (_strUserID == resultText)
                    {
                        //Debug.Log("login...");
                        authUI.ShowDatabaseLoggedinPanel();
                        authUI.loggedinUserEmail.text = resultText;
                        authUI.ShowNotice("Log In Complete..");
                    }                    
                }
            }
        }
    }


    // 로그아웃 작업해야 함...
    IEnumerator MySqlLogOut(string userName, string userPW)
    {
        WWWForm form = new WWWForm();
        form.AddField("userID", userName);
        form.AddField("userPW", userPW);
        
        // 위의 방법과 아래의 방법 모두 가능....
        using (UnityWebRequest www = UnityWebRequest.Post("http://192.168.1.183/loginTest.php", form))
        {
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
            }
        }
    }    
    IEnumerator MySqlSignUp(string _strUserID, string _strUserPW, string _strConfirmPW, string _strType, string _strGender, string _strEmail, string _strPhone)
    {
        if (_strUserPW != _strConfirmPW)
        {
            // 비번과 비번확인 일치하지 않음...
            Debug.Log("pass word not match!");
            yield break;
        }

        bool bCheckExist = false;
        WWWForm form2 = new WWWForm();
        form2.AddField("userID", _strUserID);
        // 먼저 아이디를 중복검사한다.
        using (UnityWebRequest www2 = UnityWebRequest.Post("http://192.168.1.183/ARVR/pages/unity_php/checkid_unity.php", form2))
        {
            yield return www2.SendWebRequest();
            if (www2.isNetworkError || www2.isHttpError)
            {
                Debug.Log(www2.error);
            }
            else
            {
                Debug.Log(www2.downloadHandler.text);
                string strHandlerText = www2.downloadHandler.text;
                string resultText = strHandlerText.Trim();                

                if("exist" == resultText)
                {
                    bCheckExist = true;                    
                    Debug.Log("already exsit");
                }
                else 
                {
                    bCheckExist = false;
                    Debug.Log("ok to use");
                }
            }
        }

        if(true == bCheckExist)
        {
            // 팝업 오픈 등으로 notice 한다.
            yield break;
        }

        WWWForm form = new WWWForm();
        form.AddField("userID", _strUserID);
        form.AddField("userPW", _strUserPW);
        form.AddField("userType", _strType);
        form.AddField("userGender", _strGender);
        form.AddField("userEmail", _strEmail);
        form.AddField("userPhone", _strPhone);
                
        // 그리고 sign up 한다.
        using (UnityWebRequest www = UnityWebRequest.Post("http://192.168.1.183/ARVR/pages/unity_php/admission_unity.php", form))
        {
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                string strHandlerText = www.downloadHandler.text;                                
                string resultText = strHandlerText.Trim();

                if (_strUserID == resultText)
                {
                    //Debug.Log("login...");
                    authUI.ShowLoggedinPanel();
                    authUI.loggedinUserEmail.text = resultText;                    
                    authUI.ShowNotice("Log In Complete..");
                }
                else
                {
                    // 로그인 실패.... 일단 무조건 password error 라고 뜨는 듯...
                    //authUI.ShowNotice(resultText);
                }
            }            
        }                
    }

    //================================================================================================//
    //============================================= MYSQL =============================================//
    //============================================= UPDATE =============================================//
    //================================================================================================//        
    public void OnClickGetGeoStamp()
    {
        StartCoroutine(GetGeoStamp(authUI.loginEmail.text));
    }
    public void OnClickUpdateStamp()
    {
        // 일단 index로 정한다. 첫번째 스탬프는 0, 두번째는 1, 세번째는 2        
        StartCoroutine(UpdateGeoStamp(authUI.loginEmail.text, 1));
    }
    // 지리과 스탬프 전체 받아오기
    // 후에 수정해야 함... 00000 << 으로 초기화.    
    IEnumerator GetGeoStamp(string _strUserID)
    {
        WWWForm form = new WWWForm();
        form.AddField("userID", _strUserID);

        // 아래에서 php 바꿔서 처리한다.
        using (UnityWebRequest www = UnityWebRequest.Post("http://192.168.1.183/ARVR/pages/unity_php/getStamp.php", form))
        {
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                //authUI.ShowNotice("Log In Fail");
            }
            else
            {
                // 뒤에 공백이 삽입될 수 있음...
                Debug.Log("geo stamp: " + www.downloadHandler.text);
            }
        }
    }

    IEnumerator UpdateGeoStamp(string _strUserID, int _iIndex)
    {
        int iIndex = _iIndex;
        string strGeoStamp = "";

        WWWForm form = new WWWForm();
        form.AddField("userID", _strUserID);

        // 일단 스탬프를 받아온다.
        using (UnityWebRequest www = UnityWebRequest.Post("http://192.168.1.183/ARVR/pages/unity_php/getStamp.php", form))
        {
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);                       // 에러....
            }
            else
            {
                Debug.Log("geo stamp: " + www.downloadHandler.text);
                strGeoStamp = www.downloadHandler.text;               
            }
        }
        // 그리고 index에 따라서 바꾼다.
        char[] stampArr = strGeoStamp.ToCharArray();
        for (int i = 0; i < stampArr.Length; ++i)
        {
            if (i == iIndex)
            {
                char charIndex = '1';
                stampArr[i] = charIndex;
            }
            else            
                continue;            
        }
        string resultStamp = "";
        for (int i = 0; i < stampArr.Length; ++i)
        {
            resultStamp += stampArr[i];
        }
        Debug.Log("new stamp: " + resultStamp);

        // 그리고 result를 send 한다.        
        WWWForm form2 = new WWWForm();
        form2.AddField("userID", _strUserID);
        form2.AddField("geo_stamp", resultStamp);
        //form2.AddField("geo_stamp", "00000");
        //updateStamp.php
        using (UnityWebRequest www2 = UnityWebRequest.Post("http://192.168.1.183/ARVR/pages/unity_php/updateStamp.php", form2))
        {
            yield return www2.SendWebRequest();
            if (www2.isNetworkError || www2.isHttpError)
            {
                Debug.Log(www2.error);
            }
            else
            {
                Debug.Log("result: " + www2.downloadHandler.text);
                // 그리고 결과 받아온다.
            }
        }        
    }
    // 기타 외에.. 정보 받아오기... 예를 들면 스탬프, 타임어택 기록, 그림(프사), 동영상 주소 등등....    

    //==================================================================================================//
    //=========================================== FIREBASE AUTH ===========================================//
    //==================================================================================================//    

    void InitializeFirebase()                           
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
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
            }
            user = auth.CurrentUser;
            if (signedIn)
            {
                Debug.Log("Signed in " + user.UserId);                
                //displayName = user.DisplayName ?? "";
                //emailAddress = user.Email ?? "";
                //photoUrl = user.PhotoUrl ?? "";
                // 성공시에...
                authUI.ShowLoggedinPanel();                
                authUI.loggedinUserEmail.text = user.Email;
                authUI.ShowNotice("Log In Complete..");
            }
        }
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
    void TrySignupWifhFirebaseAuth(string email, string password, string confirmPassword)
    {
        if(password != confirmPassword)
        {
            // 비번과 비번확인 일치하지 않음...
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
            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);

            authUI.ShowLoginPanel();
        });
    }

    //================================================================================================//
    //============================================ FIREBASE =============================================//
    //============================================= LOGIN ==============================================//
    //================================================================================================//

    // 로그인...
    public void OnLoginButtonClick()
    {
        // firebase login...
        //TryLoginWithFirebaseAuth(authUI.loginEmail.text, authUI.loginPassword.text);        
        // mysql login...
        StartCoroutine(MySqlLogIn(authUI.loginEmail.text, authUI.loginPassword.text));        
    }


    // 로그 아웃
    public void OnSignOutButtonClick()
    {
        // hmm.....
        //auth.SignOut();
        authUI.ShowLoginPanel();
    }
    // 계정 생성...
    public void OnCreateAccountButtonClick()
    {
        // firebase sign up...
        //TrySignupWifhFirebaseAuth(authUI.signupEmail.text, authUI.signupPassword.text, authUI.signupConfirmPassword.text);
        // mysql...
        //StartCoroutine(MySqlSignUp(authUI.signupEmail.text, authUI.signupPassword.text, authUI.signupConfirmPassword.text));         // 입력받아서 넘긴다.        
        //StartCoroutine(MySqlSignUp(authUI.signupEmail.text, authUI.signupPassword.text, authUI.signupConfirmPassword.text));         // 입력받아서 넘긴다.        
        //authUI.genderDropdown.transform.GetChild(0).gameObject.GetComponent<Text>().text;
        //Debug.Log("type: " + authUI.typeDropdown.transform.GetChild(0).gameObject.GetComponent<Text>().text);

        string signupID = authUI.signupID.text;
        string signupPW = authUI.signupPassword.text;
        string signupConfirmPW = authUI.signupConfirmPassword.text;
        string signupEmail = authUI.signupEmail.text;
        string signupType = authUI.typeDropdown.transform.GetChild(0).gameObject.GetComponent<Text>().text;
        string signupGender = authUI.genderDropdown.transform.GetChild(0).gameObject.GetComponent<Text>().text;
        string signupPhone = authUI.signupPhone.text;

        StartCoroutine(MySqlSignUp(signupID, signupPW, signupConfirmPW, signupType, signupGender, signupEmail, signupPhone));
    }

    //==================================================================================================//
    //=============================================  STORAGE ==============================================//
    //==================================================================================================//

    //=============================================  UPLOAD =============================================//    
    public void OnClickUpload()
    {        
        Debug.Log("=== upload begin ===");
        StartCoroutine(UploadProcessLocal());        
        //StartCoroutine(UploadProcessByteBufferPNGFormat());
    }    
    
    // 로컬 업로드 부분...
    IEnumerator UploadProcessLocal()
    {
        bool bError = false;
        string bMsg = "";
        PrintState("upload begin...");                
        // Get a reference to the storage service, using the default Firebase App
        Firebase.Storage.FirebaseStorage storage = Firebase.Storage.FirebaseStorage.DefaultInstance;
        // Create a storage reference from our storage service
        Firebase.Storage.StorageReference storage_ref = storage.GetReferenceFromUrl("gs://fir-authtest22.appspot.com");                 // FireaBase 계정의 아이디 adress 적는 곳.
        // Create a reference to "mountains.jpg"
        //Firebase.Storage.StorageReference mountains_ref = storage_ref.Child("Test1/mountains.jpg");
        // Create a reference to 'images/mountains.jpg'
        //Firebase.Storage.StorageReference mountain_images_ref = storage_ref.Child("images/mountains.jpg");
        // While the file names are the same, the references point to different files
        //mountains_ref.Name == mountain_images_ref.Name; // true
        //mountains_ref.Path == mountain_images_ref.Path; // false

        string local_file = "";
        if (Application.platform == RuntimePlatform.Android)                                   
        {
            local_file = "file:// "+ Application.persistentDataPath + "/" + fileName;                        // android 휴대폰의 접근 가능한 로컬 주소
        }
        else
        {
            local_file = "C:/Users/Gana/Downloads/project_stuff/" + fileName;                               // pc 의 로컬 주소..
        }
        Firebase.Storage.StorageReference rivers_ref = storage_ref.Child("stuff/" + fileName);                      // 스토리지에 레퍼런스 먼저 만들기..
        // metadata.. 추가할 것 이 있으면 추가,,, 필요 없을 시에 null 로..
        var new_metadata = new Firebase.Storage.MetadataChange();
        new_metadata.ContentType = "lyrics/text";
        
        var TmpTask = rivers_ref.PutFileAsync(local_file, new_metadata, new Firebase.Storage.StorageProgress<Firebase.Storage.UploadState>(state =>
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
        // 업로드가 완전히 완료된 후에 나머지를 처리한다.
        yield return new WaitUntil(() => TmpTask.IsCompleted);        
        if (false == bError)
            PrintState("upload complete...");       
        else if(true == bError)        
            PrintState(bMsg);               
    }

    // 이미지 파일을 byte buffer 형식으로 업로드...
    IEnumerator UploadProcessByteBufferPNGFormat()
    {
        yield return new WaitForEndOfFrame();

        Firebase.Storage.FirebaseStorage storage = Firebase.Storage.FirebaseStorage.DefaultInstance;
        // Create a storage reference from our storage service
        Firebase.Storage.StorageReference storage_ref = storage.GetReferenceFromUrl("gs://fir-authtest22.appspot.com");

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
        WWW www = new WWW("file:///"+ local_file);        
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
    // mp4 movie upload...  테스트 안 해봤음.
    IEnumerator UploadProcessByteBufferMP4Format()
    {
        yield return new WaitForEndOfFrame();

        Firebase.Storage.FirebaseStorage storage = Firebase.Storage.FirebaseStorage.DefaultInstance;
        // Create a storage reference from our storage service
        Firebase.Storage.StorageReference storage_ref = storage.GetReferenceFromUrl("gs://fir-authtest22.appspot.com");

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

    //============================================= DOWNLOAD =============================================//    
    public void OnClickDonwload()
    {        
        PrintState("downloading begin...");                
        // 먼저 확인한다. 검증완료...        
        if (Application.platform == RuntimePlatform.Android)
        {
            StartCoroutine(DownloadByBufferTxt());
            /*
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(Application.persistentDataPath + "/" + fileName);            
            if (fileInfo.Exists)
            {
                // 파일 있음...
                //PrintState("asset bundle exist...");
            }
            else
            {
                //PrintState("no asset bundle...");
                //StartCoroutine(LocalDownloadStart());
            }  
            */
        }
        else
        {
            //StartCoroutine(LocalDownloadStart());
            //StartCoroutine(DownloadByBuffer());
            StartCoroutine(DownloadByBufferTxt());            
        }        
    }
    // 로컬 다운로드...
    IEnumerator LocalDownloadStart()
    {
        // 로컬 방식....
        Firebase.Storage.FirebaseStorage storage = Firebase.Storage.FirebaseStorage.DefaultInstance;
        // Create a storage reference from our storage service
        Firebase.Storage.StorageReference storage_ref = storage.GetReferenceFromUrl("gs://fir-authtest22.appspot.com");
        // Create a reference to the file you want to upload
        Firebase.Storage.StorageReference rivers_ref = storage_ref.Child("AssetBundleTest1/" + fileName);

        string local_file = "";
        if (Application.platform == RuntimePlatform.Android)
        {
            local_file = Application.persistentDataPath + "/" + fileName;                           // android 의 접근 가능한 주소,, 폴더 추가 가능..
        }
        else
        {
            local_file = "C:/Users/Gana/Downloads/AsssetBundle/" + fileName;
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
            }
            else
            {
                Debug.Log("Finished downloading...");
            }
        });

        yield return new WaitUntil(() => TmpTask.IsCompleted);
        PrintState("Finished downloading...");
    }    
    // 로컬에 저장하지 않고, 바로 읽어들여서 화면에 뿌리기.... png, jpg 파일 가능.
    IEnumerator DownloadByBuffer()
    {
        bool bError = false;
        byte[] fileContents2 = null;

        Firebase.Storage.FirebaseStorage storage = Firebase.Storage.FirebaseStorage.DefaultInstance;
        // Create a storage reference from our storage service
        Firebase.Storage.StorageReference storage_ref = storage.GetReferenceFromUrl("gs://fir-authtest22.appspot.com");
        // Create a reference to the file you want to upload
        Firebase.Storage.StorageReference rivers_ref = storage_ref.Child("Test1/test_arrow.png");
        
        // 최대 사이즈 지정할 수 있음..
        const long maxAllowedSize = 5 * 1024 * 1024;    // 최대사이즈 일단 5mb
        var TmpTask = rivers_ref.GetBytesAsync(maxAllowedSize, new Firebase.Storage.StorageProgress<Firebase.Storage.DownloadState>(state =>
        {
            Debug.Log(string.Format("Progress: {0} of {1} bytes transferred.", state.BytesTransferred, state.TotalByteCount));
            PercentView(state.BytesTransferred, state.TotalByteCount);

        })).ContinueWith((System.Threading.Tasks.Task<byte[]> task) => {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log(task.Exception.ToString());
                // Uh-oh, an error occurred!
                bError = true;
            }
            else if(task.IsCompleted)
            {
                Debug.Log("Finished downloading!");                
                fileContents2 = task.Result;
                bError = false;
            }
        });
        // 다운로드가 완료된 이후에 아래부분 call..
        yield return new WaitUntil(() => TmpTask.IsCompleted);
        if(false == bError)
        {
            Texture2D tmpTexture = new Texture2D(16, 16, TextureFormat.RGB24, false);
            bool isLoaded = tmpTexture.LoadImage(fileContents2);

            while (!isLoaded)
                yield return null;

            tmpTexture.name = "tmpTexutre";
            rawImage.texture = tmpTexture;
            PrintState("image view");
        }
        else if(true == bError)
        {
            PrintState("Error...");
        }        
    }
    // 로컬에 저장하지 않고, 바로 읽어들여서 화면에 뿌리기... txt 파일의 경우 UTF-8 인코딩 파일만 한글 표현 가능.
    IEnumerator DownloadByBufferTxt()
    {
        bool bError = false;
        byte[] fileContents = null;

        Firebase.Storage.FirebaseStorage storage = Firebase.Storage.FirebaseStorage.DefaultInstance;
        // Create a storage reference from our storage service
        Firebase.Storage.StorageReference storage_ref = storage.GetReferenceFromUrl("gs://fir-authtest22.appspot.com");
        // Create a reference to the file you want to upload
        Firebase.Storage.StorageReference rivers_ref = storage_ref.Child("stuff/text_1.txt");

        // 최대 사이즈....
        const long maxAllowedSize = 2 * 1024 * 1024;    // 2mb
        var TmpTask = rivers_ref.GetBytesAsync(maxAllowedSize, new Firebase.Storage.StorageProgress<Firebase.Storage.DownloadState>(state =>
        {
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

    //================================================== DELETE ==================================================//    
    public void DeleteFils()
    {
        Debug.Log("delete file on FireBase Storage");
        StartCoroutine(DeleteProcess());
    }

    IEnumerator DeleteProcess()
    {
        yield return new WaitForEndOfFrame();

        Firebase.Storage.FirebaseStorage storage = Firebase.Storage.FirebaseStorage.DefaultInstance;
        // Create a storage reference from our storage service
        Firebase.Storage.StorageReference storage_ref = storage.GetReferenceFromUrl("gs://fir-authtest22.appspot.com");
        // Create a reference to the file you want to upload
        Firebase.Storage.StorageReference rivers_ref = storage_ref.Child("stuff/" + fileName);              // 삭제하려는 파일의 레퍼런스...

        // Delete the file
        Task tmpTask = rivers_ref.DeleteAsync().ContinueWith(task => {
            if (task.IsCompleted)
            {
                Debug.Log("File deleted successfully.");
            }
            else
            {
                // Uh-oh, an error occurred!
            }
        });

        yield return new WaitUntil(() => tmpTask.IsCompleted);
        PrintState("delete file complete");
    }

    //================================================= METADATA  ==================================================//
    // Storage 상의 메타데이터 추가 및 교체..
    public void MetadataChange()
    {
        Firebase.Storage.FirebaseStorage storage = Firebase.Storage.FirebaseStorage.DefaultInstance;
        // Create a storage reference from our storage service
        Firebase.Storage.StorageReference storage_ref = storage.GetReferenceFromUrl("gs://fir-authtest22.appspot.com");
        // Create a reference to the file you want to upload
        Firebase.Storage.StorageReference rivers_ref = storage_ref.Child("Test1/test_arrow.png");
        
        var new_metadata = new Firebase.Storage.MetadataChange();
        //new_metadata.CacheControl = "public,max-age=300";
        new_metadata.ContentType = "image/png";
        // 아래처럼 Dictionary 활용하여서 추가할 수 있음..
        // Issue 사항은 추가되면 수정은 가능하지만 삭제가 불가능한 것 같음..
        /*        
        var new_metadata = new Firebase.Storage.MetadataChange
        {            
            CustomMetadata = new Dictionary<string, string>
            {                
                {"story", "story for test" },
                {"tag", "hmm...." },
            }
        };
        */
        // Update metadata properties
        Task tmpTask = rivers_ref.UpdateMetadataAsync(new_metadata).ContinueWith(task => {
            if (!task.IsFaulted && !task.IsCanceled)
            {
                // access the updated meta data
                Firebase.Storage.StorageMetadata meta = task.Result;
                Debug.Log("change..");
            }
        });
        //yield return new WaitUntil(() => tmpTask.IsCompleted);
        // do someting...
    }

    //=================================================== VIDEO ==================================================//
    public void PlayButtonEvent()
    {
        Debug.Log("play video");        
        StartCoroutine(PlayVideoPlayer());
    }
    IEnumerator PlayVideoPlayer()
    {
        WaitForSeconds waitTime = new WaitForSeconds(0.1f); 

        if (Application.platform == RuntimePlatform.Android)
        {
            videPlayer.url = "https://firebasestorage.googleapis.com/v0/b/fir-authtest22.appspot.com/o/Test1%2Ftest_movie_2.mp4?alt=media&token=04050b8f-ca56-4fd5-8b91-15ec1784755f";              // firebase 상의 주소...
            //videPlayer.url = Application.persistentDataPath +"/SYJ.mp4";            
        }
        else
        {
            videPlayer.url = System.IO.Path.Combine("C:/Users/Gana/Downloads/project_stuff", "SYJ.mp4");
            //videPlayer.url = System.IO.Path.Combine("C:/Users/Gana/Downloads/project_stuff", "Beach Sunrise - 360° Video_4k.mp4");            
            //videPlayer.url = "https://firebasestorage.googleapis.com/v0/b/fir-authtest22.appspot.com/o/Test1%2Ftest_movie_2.mp4?alt=media&token=04050b8f-ca56-4fd5-8b91-15ec1784755f";
        }            

        while (!videPlayer.isPrepared)
        {
            Debug.Log("preparing video");
            PrintState("preparing video");
            yield return waitTime;
            break;
        }
        Debug.Log("Done Preparing Video");
        PrintState("Done Preparing Video");

        videPlayer.Play();
        
        Debug.Log("playing video");
        PrintState("playing video");
    }


    //================================================= FUNCTION  ==================================================//
    // upload, download progress 수치를 % 로 변환...
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
