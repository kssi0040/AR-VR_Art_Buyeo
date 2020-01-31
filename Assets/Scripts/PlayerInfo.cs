using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class PlayerInfo : MonoBehaviour
{
    private const string ACCESS_FINE_LOCATION = "android.permission.ACCESS_FINE_LOCATION";
    private const string WRITE_EXTERNAL_STORAGE = "android.permission.WRITE_EXTERNAL_STORAGE";
    private const string CAMERA = "android.permission.CAMERA";

    private static PlayerInfo instance = null;
    private static readonly object padlock = new object();

    private PlayerInfo()
    {

    }

    public static PlayerInfo Instance
    {
        get
        {
            lock(padlock)
            {
                if(instance==null)
                {
                    instance = new PlayerInfo();
                }
                return instance;
            }
        }
    }

    public List<int> GetMedal2D = new List<int>();
    public List<int> GetMedal3D = new List<int>();
        
    public bool isComplite;   
    public bool isClicked;
    public bool ClearQuiz;

    //public int Gender = -1; //0=여성 1=남성
    public int Gender = 0; //0=여성 1=남성

    // 추가됨...
    private const string appID = "art_AR";
    private string userID = "12345";


    private void Awake()
    {
        StartCoroutine(PermissionCheck());
    }
    // 안드로이드 허가 체크....
    IEnumerator PermissionCheck()
    {
        if (Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite) == false)
        {
            Permission.RequestUserPermission(Permission.ExternalStorageWrite);
            yield return new WaitUntil(() => Application.isFocused == true);
        }
        if (Permission.HasUserAuthorizedPermission(Permission.Camera) == false)
        {
            Permission.RequestUserPermission(Permission.Camera);
            yield return new WaitUntil(() => Application.isFocused == true);
        }
        if (Permission.HasUserAuthorizedPermission(Permission.FineLocation) == false)
        {
            Permission.RequestUserPermission(Permission.FineLocation);
            yield return new WaitUntil(() => Application.isFocused == true);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if(instance!=null)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 추가됨....
    public void SetUserID(string _userID) { userID = _userID; }
    public string GetUserID() { return userID; }
    public string GetAppID() { return appID; }
}
