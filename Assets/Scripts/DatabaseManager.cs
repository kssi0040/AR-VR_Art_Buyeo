using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class DatabaseManager : MonoBehaviour
{
    // gps 를 어떻게 받아올것인가...?
    private static DatabaseManager instance = null;
    private static readonly object padlock = new object();


    // 아래도 전부 삭제 예정....
    public AuthUIMgr lobbyUI;
    // login..
    [Header("Login")]
    public InputField userId;
    public InputField userPassword;
    public Text tempText;
    // sign up..
    [Header("Sign Up")]
    public InputField signupID;
    public InputField signupEmail;
    public InputField signupPassword;
    public InputField signupConfirmPassword;
    public InputField signupPhone;
    public Dropdown typeDropdown;
    public Dropdown genderDropdown;



     //========================================================================================================//
    public static DatabaseManager Instance
    {
        get
        {
            lock (padlock)
            {
                if (instance == null)
                {
                    instance = new DatabaseManager();
                }
                return instance;
            }
        }
    }

    private void Start()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;

        DontDestroyOnLoad(this.gameObject);
    }
    //========================================================================================================//


    //============================================= MYSQL =============================================//
    
    //================================================================================================//        
    //============================================= LOGIN ==============================================//
    //================================================================================================//        
    public void LogInSystemEvent(string _strUserID, string _strUserPW)
    {
        StartCoroutine(MySqlLogIn(_strUserID, _strUserPW));        
    }
    IEnumerator MySqlLogIn(string _strUserID, string _strUserPW)
    {
        WWWForm form = new WWWForm();
        form.AddField("userID", _strUserID);
        form.AddField("userPW", _strUserPW);

        // 아래에서 php 바꿔서 처리한다.
        //using (UnityWebRequest www = UnityWebRequest.Post("http://192.168.1.183/pages/unity_php/loginTest.php", form))
        using (UnityWebRequest www = UnityWebRequest.Post("http://eduarvr.dlinkddns.com/pages/unity_php/loginTest.php", form))
        {
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);                
            }
            else
            {
                string strHandlerText = www.downloadHandler.text;
                string resultText = strHandlerText.Trim();
                if ("password ERROR" == resultText)
                {
                    Debug.Log("password error");
                }
                else
                {
                    if (_strUserID == resultText)
                    {
                        Debug.Log("login... sucesss");                        
                        PlayerInfo.Instance.SetUserID(resultText);
                        //SceneManager.LoadScene("GPS_Scene");
                        GameObject.Find("ButtonManager").SendMessage("LoginEventCallBack", resultText);
                    }
                }
            }
        }
    }
    //================================================================================================//        
    //============================================== GPS ===============================================//
    //================================================================================================//        
    
        
    // 스탬프 정보 Update or Insert...    
    public void GpsInfoUpdate(string _strAppID, string _strUserID, string _strBadgeType, string _strFlag, string _strEduType, string _strGameType, int _strTime)
    {
        StartCoroutine(GpsResultUpdate(_strAppID, _strUserID, _strBadgeType, _strFlag, _strEduType, _strGameType, _strTime));        
    }
    // 스탬프 정보 Update or Insert...
    IEnumerator GpsResultUpdate(string _strAppID,  string _strUserID, string _strBadgeType, string _strFlag, string _strEduType, string _strGameType, int _strTime)
    {
        WWWForm form = new WWWForm();
        form.AddField("appID", _strAppID);
        form.AddField("userID", _strUserID);

        form.AddField("userBadgeType", _strBadgeType);
        form.AddField("userBadgeFlag", _strFlag);
        form.AddField("userEduType", _strEduType);
        form.AddField("userGameType", _strGameType);
        form.AddField("userTime", _strTime);

        using (UnityWebRequest www = UnityWebRequest.Post("http://eduarvr.dlinkddns.com/pages/unity_php/unity_sticker.php", form))
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
                // 수정 예정...
                //GameObject.Find("GPS_System").SendMessage("CatchGpsUpdate", resultText);
            }
        }
    }



    //============================================== 도착시에 ===============================================//
    public void GPS_ArriveUpdate(string _strAppID, string _strUserID, string _strLocation, string _strGPSinfo)
    {
        StartCoroutine(ArriveUpdate(_strAppID, _strUserID, _strLocation, _strGPSinfo));
    }
    IEnumerator ArriveUpdate(string _strAppID, string _strUserID, string _strLocation, string _strGPSinfo)
    {
        WWWForm form = new WWWForm();
        // 아래부터 작업해야 함....
        form.AddField("appID", _strAppID);
        form.AddField("userID", _strUserID);
        form.AddField("structure", _strLocation);
        form.AddField("GPSinfo", _strGPSinfo);

        // 아래에서 php 바꿔서 처리한다.
        //using (UnityWebRequest www = UnityWebRequest.Post("http://192.168.1.183/pages/unity_php/unity_GPS_log.php", form))
        using (UnityWebRequest www = UnityWebRequest.Post("http://eduarvr.dlinkddns.com/pages/unity_php/unity_GPS_log.php", form))            
        {
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                //Debug.Log(www.downloadHandler.text);
                string strHandlerText = www.downloadHandler.text;
                string resultText = strHandlerText.Trim();
                // 결과를 보내는 것 처리해야 함...
                //SendGpsResult(resultText);
                GameObject.Find("GPS_System").SendMessage("StampStickerUpdate", resultText);
                Debug.Log(resultText);
            }
        }
    }

    //============================================== 스탬프 정보 받기 ===============================================//
    // 새로 만듬. 스탬프 정보를 받아오는 함수....
    public void GetStamp(string _strAppID, string _strUserID, string _strBadgeType, string _strEduType)
    {
        StartCoroutine(GetStameInfo(_strAppID, _strUserID, _strBadgeType, _strEduType));
    }
    // 새로만듬. 스탬프 정보를 받아오는 함수....
    IEnumerator GetStameInfo(string _strAppID, string _strUserID, string _strBadgeType, string _strEduType)
    {
        WWWForm form = new WWWForm();
        form.AddField("appID", _strAppID);
        form.AddField("userID", _strUserID);
        form.AddField("userBadgeType", _strBadgeType); 
        form.AddField("userEduType", _strEduType);
        
        using (UnityWebRequest www = UnityWebRequest.Post("http://eduarvr.dlinkddns.com//pages/unity_php/sticker_check.php", form))
        {
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                //tempText.text = www.error;
            }
            else
            {
                //Debug.Log(www.downloadHandler.text);
                string strHandlerText = www.downloadHandler.text;
                string resultText = strHandlerText.Trim();
                GameObject.Find("GPS_System").SendMessage("CatchStampInfo", resultText);                
            }
        }
    }    

    //============================================== GPS 위도, 경도 받아오기 ===============================================//
    // gps 값 받아오기...
    public void GetGPS_Info(string _structure)
    {
        StartCoroutine(GetGPS_Pos(_structure));
    }
    IEnumerator GetGPS_Pos(string _structure)
    {
        WWWForm form = new WWWForm();
        form.AddField("structure", _structure);
        //Debug.Log("spot: " + _structure);
        using (UnityWebRequest www = UnityWebRequest.Post("http://eduarvr.dlinkddns.com/pages/unity_php/GPS_state_check.php", form))
        {
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                //Debug.Log(www.downloadHandler.text);
                string strHandlerText = www.downloadHandler.text;
                string resultText = strHandlerText.Trim();
                GameObject.Find("GPS_System").SendMessage("GetGPS_Pos", resultText);
            }
        }
    }


    //================================================================================================//        
    //========================================== ASSET BUNDLE ===========================================//
    //================================================================================================//            ..
    // 에셋번들 버전 보내기...
    public void RequestAssetBundleVersionCheck(string _appVersion, string _resVersion, string _appID)
    {
        StartCoroutine(AssetBundleVersionCheck(_appVersion, _resVersion, _appID));
    }
    IEnumerator AssetBundleVersionCheck(string _appVersion, string _resVersion, string _appID)
    {
        WWWForm form = new WWWForm();
        form.AddField("appID", _appID);
        form.AddField("appVersion", _appVersion);
        form.AddField("resVersion", _resVersion);

        using (UnityWebRequest www = UnityWebRequest.Post("http://eduarvr.dlinkddns.com/pages/unity_php/unity_app_check.php", form))
        {
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                //Debug.Log(www.downloadHandler.text);
                string strHandlerText = www.downloadHandler.text;
                string resultText = strHandlerText.Trim();
                //ResponseAssetBundleVersion(resultText);                
                GameObject.Find("AssetBundleManager").SendMessage("GetAssetBundleMsg", resultText);
            }
        }
    }
    
    // 에셋번들 URL 받아오기...
    public void RequestAssetBundleVersionURL(string _appVersion, string _resVersion, string _appID)
    {
        StartCoroutine(AssetBundleVersionURL(_appVersion, _resVersion, _appID));
    }
    IEnumerator AssetBundleVersionURL(string _appVersion, string _resVersion, string _appID)
    {
        WWWForm form = new WWWForm();
        form.AddField("appID", _appID);
        form.AddField("appVersion", _appVersion);
        form.AddField("resVersion", _resVersion);

        using (UnityWebRequest www = UnityWebRequest.Post("http://eduarvr.dlinkddns.com/pages/unity_php/unity_app_check.php", form))
        {
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                //Debug.Log(www.downloadHandler.text);
                string strHandlerText = www.downloadHandler.text;
                string resultText = strHandlerText.Trim();
                GameObject.Find("AssetBundleManager").SendMessage("GetAssetBundleVersionURL", resultText);                
            }
        }
    }
    












    //========================================================================================================//
    //========================================= 아래는 삭제 예정.. or 수정 예정 ===========================================//
    //========================================================================================================//










    /*
    //================================================================================================//        
    //============================================ SIGN UP ==============================================//
    //================================================================================================//        
    public void DatabaseSignUpButtonEvent()
    {        
        string tmpId = signupID.text;
        string tmpType = typeDropdown.transform.GetChild(0).gameObject.GetComponent<Text>().text;
        string tmpGender = genderDropdown.transform.GetChild(0).gameObject.GetComponent<Text>().text;
        string tmpPw = signupPassword.text;
        string tmpConirmId = signupConfirmPassword.text;
        string tmpEmail = signupEmail.text;        
        string tmpPhone = signupPhone.text;

        StartCoroutine(MySqlSignUp(tmpId, tmpPw, tmpConirmId, tmpType, tmpGender, tmpEmail, tmpPhone));
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

                if ("exist" == resultText)                                                              // 해당 아이디 이미 존재함...
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

        if (true == bCheckExist)
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
                    // 아이디 생성 후에 로그인...
                    Debug.Log("login...");                    
                    lobbyUI.ShowDatabaseLoggedinPanel();
                    lobbyUI.databaseLoggedinID.text = resultText;
                    lobbyUI.databaseLoggedinText.text = "Log in Success";
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
    //============================================ UPDATE ==============================================//
    //================================================================================================//            
    // 지리과,,, Stamp 관련... 예시....
    public void GetGeoStampButtonEvent()
    {
        //Debug.Log("get geo");
        lobbyUI.databaseLoggedinID.text = "12345";
        StartCoroutine(GetGeoStamp(lobbyUI.databaseLoggedinID.text));
    }
    IEnumerator GetGeoStamp(string _strUserID)
    {
        WWWForm form = new WWWForm();
        form.AddField("userID", _strUserID);
        
        using (UnityWebRequest www = UnityWebRequest.Post("http://192.168.1.183/ARVR/pages/unity_php/getStamp.php", form))
        {
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                lobbyUI.databaseLoggedinText.text = www.error;
            }
            else
            {
                Debug.Log("geo stamp: " + www.downloadHandler.text);
                lobbyUI.databaseLoggedinText.text = www.downloadHandler.text;
            }
        }
    }

    // 각각의 stamp update... 그리고 reset...
    public void GetGeoStamp1()
    {        
        StartCoroutine(UpdateGeoStamp(lobbyUI.databaseLoggedinID.text, 0));
    }
    public void GetGeoStamp2()
    {        
        StartCoroutine(UpdateGeoStamp(lobbyUI.databaseLoggedinID.text, 1));
    }
    public void GetGeoStamp3()
    {        
        StartCoroutine(UpdateGeoStamp(lobbyUI.databaseLoggedinID.text, 2));
    }
    public void GetGeoStamp4()
    {        
        StartCoroutine(UpdateGeoStamp(lobbyUI.databaseLoggedinID.text, 3));
    }
    public void GeoStampReset()
    {                
        StartCoroutine(ResetGeoStamp(lobbyUI.databaseLoggedinID.text));
    }

    // Reset Database 00000 으로 리셋...
    IEnumerator ResetGeoStamp(string _strUserID)
    {
        WWWForm form = new WWWForm();
        string resultStamp = "00000";

        form.AddField("userID", _strUserID);
        form.AddField("geo_stamp", resultStamp);

        using (UnityWebRequest www2 = UnityWebRequest.Post("http://192.168.1.183/ARVR/pages/unity_php/updateStamp.php", form))
        {
            yield return www2.SendWebRequest();
            if (www2.isNetworkError || www2.isHttpError)
            {
                Debug.Log(www2.error);
                lobbyUI.databaseLoggedinText.text = www2.error;
            }
            else
            {
                Debug.Log("result: " + www2.downloadHandler.text);
                lobbyUI.databaseLoggedinText.text = www2.downloadHandler.text;
                // 그리고 결과 받아온다.
            }
        }
    }

    // DB의 기록을 Update 한다...
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
                lobbyUI.databaseLoggedinText.text = www.error;
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

        using (UnityWebRequest www2 = UnityWebRequest.Post("http://192.168.1.183/ARVR/pages/unity_php/updateStamp.php", form2))
        {
            yield return www2.SendWebRequest();
            if (www2.isNetworkError || www2.isHttpError)
            {
                Debug.Log(www2.error);
                lobbyUI.databaseLoggedinText.text = www2.error;
            }
            else
            {
                Debug.Log("result: " + www2.downloadHandler.text);
                lobbyUI.databaseLoggedinText.text = www2.downloadHandler.text;
                // 그리고 결과 받아온다.
            }
        }
    }






    */



}
