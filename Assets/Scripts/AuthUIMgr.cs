using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AuthUIMgr : MonoBehaviour {
    [Header("Panel")]

    public GameObject lobbyPanel;
    public GameObject databaseLoginPanel;
    public GameObject firebaseLoginPanel;

    public GameObject databaseSignupPanel;
    public GameObject firebaseSignupPanel;


    public GameObject loginPanel;
    public GameObject signupPanel;

    public GameObject loggedinPanel;

    public GameObject databaseoggedinPanel;
    public GameObject firebaseLoggedinPanel;


    [Header("Login")]
    public InputField loginEmail;
    public InputField loginPassword;
    [Header("Sign Up")]
    public InputField signupEmail;
    public InputField signupPassword;
    public InputField signupConfirmPassword;

    public Text loggedinUserEmail;

    public Text noticeText;

    // 추가됨..
    public InputField signupID;
    public Dropdown typeDropdown;
    public Dropdown genderDropdown;
    public InputField signupPhone;

    public Text databaseLoggedinID;
    public Text databaseLoggedinText;

    public Text firebaseLoggedinID;
    public Text firebaseLoggedinText;


    // Use this for initialization
    void Awake()
    {
        /*
        Debug.Log("awake");
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                //   app = Firebase.FirebaseApp.DefaultInstance;

                // Set a flag here to indicate whether Firebase is ready to use by your app.
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
        */
    }
    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;                              // 화면 안 꺼지게 하기...
        ShowPanel(lobbyPanel);
        //ShowPanel(databaseoggedinPanel);
    }
    // Update is called once per frame
    void Update()
    {

    }
	
    public void ShowFirstPanel()
    {
        ShowPanel(lobbyPanel);
    }
    public void ShowLoginPanel()
    {
        ShowPanel(loginPanel);
    }    
    public void ShowSignUpPanel()
    {
        ShowPanel(signupPanel);
    }

    // 계정 생성...
    public void ShowDatabaseSignUpPanel()
    {
        ShowPanel(databaseSignupPanel);
    }
    public void ShowFirebaseSignUpPanel()
    {
        ShowPanel(firebaseSignupPanel);
    }

    // 로그인 성공...
    public void ShowFirebaseLogginPanel()
    {
        ShowPanel(firebaseLoggedinPanel);
    }
    public void ShowDatabaseLoggedinPanel()
    {
        ShowPanel(databaseoggedinPanel);
    }

    public void ShowPanel(GameObject panel)
    {        
        loginPanel.SetActive(false);
        signupPanel.SetActive(false);
        loggedinPanel.SetActive(false);

        // 추가됨...
        lobbyPanel.SetActive(false);
        databaseLoginPanel.SetActive(false);
        firebaseLoginPanel.SetActive(false);
        databaseoggedinPanel.SetActive(false);
        firebaseLoggedinPanel.SetActive(false);

        firebaseSignupPanel.SetActive(false);
        databaseSignupPanel.SetActive(false);

        panel.SetActive(true);
    }


    public void ShowNotice(string _text)
    {
        noticeText.text = _text;
    }

    //=========================================================================================================//
    public void DataBasePartButtonEvent()
    {
        ShowPanel(databaseLoginPanel);
    }
    public void FireBasePartButtonEvent()
    {        
        //ShowPanel(firebaseLoginPanel);
        ShowPanel(loggedinPanel);
    }
    //=========================================================================================================//
    //============================================= DATABASE PART ================================================//
    //=========================================================================================================//




    //=============================================== USELESS =====================================================//
    public void ShowLoggedinPanel()
    {
        ShowPanel(loggedinPanel);        
    }

}
