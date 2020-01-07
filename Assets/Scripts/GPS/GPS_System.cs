using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Android;
using UnityEngine.SceneManagement;
using System;

public class GPS_System : MonoBehaviour
{
    [Header("UI")]
    //public Text gpsNotice;    
    public GameObject mainCanvas;
    public GameObject ImageBundle;
    public Text describeText;

    //public GameObject CityButtons;
    public GameObject BackButton;
    //public GameObject Ansan;
    //public GameObject Buyeo;
    //public GameObject Chang_neong;
    //public GameObject Chun_chen;
    public string AppID = "";
    public string strEdu_type = null;
    public string strSpot1 = null;
    public string strSpot2 = null;
    public string strSpot3 = null;
    public string strSpot4 = null;

    private Text[] aAnsanSpotsDistance = new Text[4];
    private double[] aDistance = new double[4];
    
    private float latitude;
    private float longitude;
    private int iCount = 0;
    private string gpsText = "";    
    private int iStampCount = 0;
    private int iGPS_PosCount = 0;

    private double[] aSpotLatitude = new double[4];
    private double[] aSpotLongitude  = new double[4];
    private GameObject[] aStampImg = new GameObject[4];
    private GameObject[] aButtonImg = new GameObject[4];
    private GameObject[] aImageBundle = new GameObject[4];
    private int iTestCount = 0;
    private int iButtonIndex = -1;
    private int iArriveIndex = -1;

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < aStampImg.Length; ++i)
        {
            aStampImg[i] = mainCanvas.transform.GetChild(i + 1).gameObject.transform.GetChild(0).gameObject;
            aButtonImg[i] = mainCanvas.transform.GetChild(i + 1).gameObject.transform.GetChild(1).gameObject;
            aAnsanSpotsDistance[i] = mainCanvas.transform.GetChild(i + 1).gameObject.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.GetComponent<Text>();
            aImageBundle[i] = ImageBundle.transform.GetChild(i).gameObject;
            aImageBundle[i].SetActive(false);
        }

        //모바일 location 정보에 대한 권한 설정...
        if (Application.platform == RuntimePlatform.Android)
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            {
                Permission.RequestUserPermission(Permission.FineLocation);
            }            
        }
        ReStartGPS();
        //RequestGPS_Pos();
        //StampStatusUpdate();
    }

    // Update is called once per frame
    void Update()    {      }

    //================================================= GPS 정보 받기(경도, 위도) ==================================================//
    // GPS 정보 받기...    
    public void RequestGPS_Pos()
    {
        string structure = "";
        if (0 == iGPS_PosCount)
        {
            structure = strSpot1;
        }
        else if (1 == iGPS_PosCount)
        {
            structure = strSpot2;
        }
        else if (2 == iGPS_PosCount)
        {
            structure = strSpot3;
        }
        else if (3 == iGPS_PosCount)
        {
            structure = strSpot4;
        }
        else if (4 == iGPS_PosCount)
        {            
            StampCheck();            
            return;
        }

        //Debug.Log("check");
        DatabaseManager.Instance.GetGPS_Info(structure);
    }
    public void GetGPS_Pos(string _strAnswer)
    {
        //Debug.Log("answer: " + _strAnswer);
        string strAnswer = _strAnswer;
        if (strAnswer != "")
        {
            int location = strAnswer.IndexOf(",");
            int length = strAnswer.Length;
            int minusLength = length - location;
            string strLatitude = strAnswer.Substring(0, location);
            string strLongitute = strAnswer.Substring(location + 1, minusLength - 1);
            aSpotLatitude[iGPS_PosCount] = double.Parse(strLatitude);
            aSpotLongitude[iGPS_PosCount] = double.Parse(strLongitute);            
            
            // 계속 반복한다.
            Debug.Log("lati: " + aSpotLatitude[iGPS_PosCount]);
            Debug.Log("long: " + aSpotLongitude[iGPS_PosCount]);

            iGPS_PosCount++;
            RequestGPS_Pos();
        }
        else
        {
            // 에러....
        }
    }

    //====================================================================================================================//
    // 스탬프 획득 여부...
    void StampCheck()
    {
        //string appID = PlayerInfo.Instance.GetAppID();
        string appID = AppID;
        string userID = PlayerInfo.Instance.GetUserID();
        //string kind = "social_sticker_first";
        string kind = "";
        string get_flag = "";
        string edu_type = strEdu_type + "_live";
        string game_type = "AR";
        
        if (0 == iStampCount)
        {
            kind = strEdu_type + "_sticker_first";
            DatabaseManager.Instance.GetStamp(appID, userID, kind, edu_type);
        }
        else if(1 == iStampCount)
        {
            kind = strEdu_type + "_sticker_second";
            DatabaseManager.Instance.GetStamp(appID, userID, kind, edu_type);
        }
        else if (2 == iStampCount)
        {
            kind = strEdu_type + "_sticker_third";
            DatabaseManager.Instance.GetStamp(appID, userID, kind, edu_type);
        }
        else if (3 == iStampCount)
        {
            kind = strEdu_type + "_sticker_fourth";
            DatabaseManager.Instance.GetStamp(appID, userID, kind, edu_type);
        }
        else if (4 == iStampCount)
        {
            StartCoroutine(GPS_KeepUpdate());
            return;
        }        
    }



    public void CatchStampInfo(string _strAnswer)
    {
        Debug.Log("answer: " + _strAnswer);
        string strAnswer = _strAnswer;
        if ("Y" == strAnswer)
        {
            // 이미 획득한 Stamp..            
            aStampImg[iStampCount].SetActive(true);
            aButtonImg[iStampCount].SetActive(false);
        }
        else
        {            
            // 획득 안한 Stamp...
            aStampImg[iStampCount].SetActive(false);
            aButtonImg[iStampCount].SetActive(true);
        }        
        iStampCount++;
        StampCheck();
    }
    






    //======================================================= Stamp 획득 ======================================================//  
    // 획득시가 아닌,,,, 
    public void StampStatusUpdate()
    {
        //string appID = PlayerInfo.Instance.GetAppID();
        string appID = AppID;
        string userID = PlayerInfo.Instance.GetUserID();
        //string kind = "social_sticker_first";
        string kind = "";
        string get_flag = "";                
        string edu_type = strEdu_type + "_live";
        string game_type = "AR";
        int timeInfo = 0;

        if (0 == iStampCount)
        {
            // 수정 예정...
            get_flag = "Y";
            kind = strEdu_type + "_sticker_first";
            DatabaseManager.Instance.GpsInfoUpdate(appID, userID, kind, get_flag, edu_type, game_type, timeInfo);
        }
        else if (1 == iStampCount)
        {
            get_flag = "Y";
            kind = strEdu_type + "_sticker_second";
            DatabaseManager.Instance.GpsInfoUpdate(appID, userID, kind, get_flag, edu_type, game_type, timeInfo);
        }
        else if (2 == iStampCount)
        {
            get_flag = "Y";
            kind = strEdu_type + "_sticker_third";
            DatabaseManager.Instance.GpsInfoUpdate(appID, userID, kind, get_flag, edu_type, game_type, timeInfo);
        }
        else if (3 == iStampCount)
        {
            get_flag = "Y";
            kind = strEdu_type + "_sticker_fourth";
            DatabaseManager.Instance.GpsInfoUpdate(appID, userID, kind, get_flag, edu_type, game_type, timeInfo);
        }
        else if (4 == iStampCount)
        {
            ReStartGPS();
        }        
    }

    //==================================================== GPS INITIALIZE =====================================================//   
    void ReStartGPS()
    {

#if (UNITY_EDITOR || UNITY_STANDALONE_WIN)
        PcFuncStart();
#elif UNITY_ANDROID
        StartCoroutine(GpsStart());        
#endif
        //StartCoroutine(GpsStart());
    }

    // 안드로이드 용..
    IEnumerator GpsStart()
    {
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("User has not enabled GPS");
            gpsText = "User has not enabled GPS";            
            describeText.text = gpsText;
            Invoke("ReStartGPS", 4f);
            yield break;
        }
        Input.location.Start(5, 10);
        int maxWait = 20;

        gpsText = Input.location.status.ToString();
        describeText.text = gpsText;

        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            //Input.location.status
            gpsText = Input.location.status.ToString();
            describeText.text = gpsText;
            maxWait--;
        }

        RequestGPS_Pos();

        while (true)
        {
            latitude = Input.location.lastData.latitude;
            longitude = Input.location.lastData.longitude;            
            iCount++;            
            yield return new WaitForSeconds(0.5f);
        }
    }

    // pc용...
    void PcFuncStart()
    {        
        //latitude = (float)37.274092;
        //longitude = (float)126.839966;
        latitude = (float)37.484222;
        longitude = (float)126.899064;
        RequestGPS_Pos();
    }


    // 아래는 삭제 예정...
    // 아래는 삭제 예정...
    
    //================================================= GPS ===================================================//    

    IEnumerator GPS_KeepUpdate()
    {
        int iTmpCount = 0;

        while(true)
        {
            if (4 == iTmpCount)
                iTmpCount = 0;

            //Debug.Log("lati " + latitude);
            //Debug.Log("long " + longitude);

            if (false == aStampImg[iTmpCount].activeSelf)
            {                                
                // 모바일용...
                aDistance[iTmpCount] = GetDistance(latitude, longitude, aSpotLatitude[iTmpCount], aSpotLongitude[iTmpCount]);

                if (1000 <= aDistance[iTmpCount])                                                                            // 1000 미터 이상, 즉 kilo 로 표시...
                {
                    double dDist22 = aDistance[iTmpCount] / 1000;
                    string strDistance = dDist22.ToString("N1");
                    strDistance += "kilo";
                    aAnsanSpotsDistance[iTmpCount].text = strDistance;
                }
                else
                {
                    if (50 > aDistance[iTmpCount])                                                                              // 100 미터 이하일 경우 도착으로 표시...
                    {
                        // arrived...                
                        aAnsanSpotsDistance[iTmpCount].text = "";
                        // effect 가 있어야 할지도 있음....
                        aStampImg[iTmpCount].SetActive(true);
                        aButtonImg[iTmpCount].SetActive(false);

                        RequestCheckArrive(iTmpCount, latitude, longitude);
                    }
                    else
                    {
                        string strDistance = aDistance[iTmpCount].ToString("N0");
                        strDistance += "m";
                        aAnsanSpotsDistance[iTmpCount].text = strDistance;
                    }
                }
            }
            
            if (iButtonIndex == iTmpCount)
            {
                if (false == aStampImg[iTmpCount].activeSelf)
                {
                    describeText.text = aAnsanSpotsDistance[iTmpCount].text;
                }                
            }
                

            iTmpCount++;            
            yield return new WaitForSeconds(0.5f);
        }        
    }

    // 유저 id, 현재 gps, appID, 장소...
    void RequestCheckArrive(int _iTmeCount, double _latitude, double _longitute)
    {
        int iTmpCount = _iTmeCount;
        iArriveIndex = _iTmeCount;
        string dCurrentLatitude = _latitude.ToString();
        string dCurrentLongitude = _longitute.ToString();
        string dUserGPS = dCurrentLatitude + "," + dCurrentLongitude;

        //string appID = PlayerInfo.Instance.GetAppID();
        string appID = AppID;
        string userID = PlayerInfo.Instance.GetUserID();

        string structure = "";
        if (0 == iTmpCount)
            structure = strSpot1;
        else if (1 == iTmpCount)
            structure = strSpot2;
        else if (2 == iTmpCount)
            structure = strSpot3;
        else if (3 == iTmpCount)
            structure = strSpot4;
        else if (4 <= iTmpCount)
            return;

        DatabaseManager.Instance.GPS_ArriveUpdate(appID, userID, structure, dUserGPS);
    }

    //DatabaseManager.Instance.GpsInfoUpdate(appID, userID, kind, get_flag, edu_type, game_type, timeInfo);
    public void StampStickerUpdate()
    {
        //string appID = PlayerInfo.Instance.GetAppID();
        string appID = AppID;
        string userID = PlayerInfo.Instance.GetUserID();
        //string kind = "social_sticker_first";
        string kind = "";
        string get_flag = "";
        string edu_type = strEdu_type + "_live";
        string game_type = "AR";
        int timeInfo = 0;
        
        if (0 == iArriveIndex)
            kind = strEdu_type + "_sticker_first";
        else if (1 == iArriveIndex)
            kind = strEdu_type + "_sticker_second";
        else if (2 == iArriveIndex)
            kind = strEdu_type + "_sticker_third";
        else if (3 == iArriveIndex)
            kind = strEdu_type + "_sticker_fourth";
        else if (4 == iArriveIndex)
            return;

        get_flag = "Y";        
        // test 1 update...        
        DatabaseManager.Instance.GpsInfoUpdate(appID, userID, kind, get_flag, edu_type, game_type, timeInfo);
    }

    public void CatchGpsUpdate(string _strAnswer)
    {
        Debug.Log("state: " + _strAnswer);
    }

    //==========================================================================================================//
    //================================================= BUTTON ===================================================//
    //========================================================================================================//
    public void SpotButtonEvent1()
    {
        for(int i = 0; i < aImageBundle.Length; ++i)
        {
            aImageBundle[i].SetActive(false);
        }
        aImageBundle[0].SetActive(true);

        if (false == aStampImg[0].activeSelf)
        {
            describeText.text = aAnsanSpotsDistance[0].text;
        }
        iButtonIndex = 0;
    }
    public void SpotButtonEvent2()
    {
        for (int i = 0; i < aImageBundle.Length; ++i)
        {
            aImageBundle[i].SetActive(false);
        }
        aImageBundle[1].SetActive(true);

        if (false == aStampImg[1].activeSelf)
        {
            describeText.text = aAnsanSpotsDistance[1].text;
        }
        iButtonIndex = 1;
    }
    public void SpotButtonEvent3()
    {
        for (int i = 0; i < aImageBundle.Length; ++i)
        {
            aImageBundle[i].SetActive(false);
        }
        aImageBundle[2].SetActive(true);

        if (false == aStampImg[2].activeSelf)
        {
            describeText.text = aAnsanSpotsDistance[2].text;
        }
        iButtonIndex = 2;
    }
    public void SpotButtonEvent4()
    {
        for (int i = 0; i < aImageBundle.Length; ++i)
        {
            aImageBundle[i].SetActive(false);
        }
        aImageBundle[3].SetActive(true);

        if (false == aStampImg[3].activeSelf)
        {
            describeText.text = aAnsanSpotsDistance[3].text;
        }
        iButtonIndex = 3;
    }
    //========================================================================================================================//
    public void StampButtonEvent1()
    {
        for (int i = 0; i < aImageBundle.Length; ++i)
        {
            aImageBundle[i].SetActive(false);
        }
        aImageBundle[0].SetActive(true);
        iButtonIndex = 0;

        describeText.text = "만수산 기슭에 자리 잡은 무량사는 통일신라 문성왕때 법일국사가 창건하였다고 전해진다. 임진왜란 때 모두 불타고, 조선 인조때 진묵선사에 의해 중수되어 오늘에 이르고 있다. 무량사의 중심건물인 극란전은 보물 제356호로, 우리나라에서는 흔치 않은 2층 불전이다. 2층 불전은 외관상으로는 2층으로 보이나, 실제 내부는 위아래 층의 구분 없이 하나로 트여 있다.";
    }
    public void StampButtonEvent2()
    {
        for (int i = 0; i < aImageBundle.Length; ++i)
        {
            aImageBundle[i].SetActive(false);
        }
        aImageBundle[1].SetActive(true);
        iButtonIndex = 1;

        describeText.text = "대조사는 성흥산에 있는 사찰로, 이름이 지어지게 된 사건이 전설로 전해진다. 전설에 따르면 한 스님이 큰 바위 아래서 수도하다가 졸고 있었는데, 관음조(觀音鳥) 한 마리가 날아와 그 바위 위에 앉았다고 한다. 스님이 놀라 잠을 깨니, 바위가 미륵보살상으로 변해 있어서 절 이름을 '대조사'라고 하였다고 한다.";
    }
    public void StampButtonEvent3()
    {
        for (int i = 0; i < aImageBundle.Length; ++i)
        {
            aImageBundle[i].SetActive(false);
        }
        aImageBundle[2].SetActive(true);
        iButtonIndex = 2;

        describeText.text = "송국리 선사취락지는 1975년에 발굴ㆍ조사된 곳으로, 농경생활을 하던 청동기 문화인의 유적으로는 최대 규모이다. 당시 청동기 문화인의 주거지를 비롯해 민무늬토기·간석기·돌널무덤 등이 발굴되었으며, 탄화미도 발견되어 벼농사의 기원을 무문토기문화와 확실하게 연결 지을 수 있게 되었다. 뿐만 아니라, 이곳과 관련 있는 초촌면 산직리와 규암면·은산면·충화면·석성면 등지에는 고인돌을 비롯해 청동기시대의 많은 유적이 분포하고 있다.";
     }
    public void StampButtonEvent4()
    {
        for (int i = 0; i < aImageBundle.Length; ++i)
        {
            aImageBundle[i].SetActive(false);
        }
        aImageBundle[3].SetActive(true);
        iButtonIndex = 3;

        describeText.text = "국립부여박물관은 1929년 발족된 부여고적보존회를 시작으로 현재까지 약 80여 년에 이르는 깊은 역사를 간직하고 있다. 충남 서부지역의 선사문화를 비롯하여 특히 백제의 문화유산을 보존 관리하는 데에 중심 역할을 다하여 왔다. 이외에도 유적·유물의 전시와 조사연구, 문화교육, 국제교류 등 다양한 활동을 통해 백제의 역사와 문화를 널리 선양하는 데에 최선의 노력을 기울이고 있다. 최근에는 더욱 높아진 이용자들의 문화 향유 욕구와 수준에 발맞춰 쾌적한 관람환경을 제공하고자 전시 시설의 환경 개선과 다양한 전시유물의 확보에도 많은 노력을 기울이고 있다.";
    }
    public void BackButtonEvent()
    {
        SceneManager.LoadScene("SelectMap");
    }
    public void TestButtonEvent()
    {
        if (4 <= iTestCount)
            return;
        aStampImg[iTestCount].SetActive(true);
        aButtonImg[iTestCount].SetActive(false);
        iTestCount++;
    }

    public void MapButtonEvent()
    {
        //https://www.google.com/maps/dir/37.483782,126.9003409/37.5081441,126.8385434        

        //iButtonIndex
        //aSpotLatitude , aSpotLongitude        
        if (-1 == iButtonIndex)
            return;

        //string strUrl = "https://www.google.com/maps/dir/" + latitude + "," + longitude + "/";
        //string strUrl = "https://www.google.com/maps/dir/" + latitude + "," + longitude + "/" + 37.94849 + "," + 127.8147;
        string strUrl = "https://www.google.com/maps/dir/" + latitude + "," + longitude + "/" + aSpotLatitude[iButtonIndex] + "," + aSpotLongitude[iButtonIndex];
        Application.OpenURL(strUrl);
    }


    public void TestButtonEvent2()
    {
        //37.66771 128.7053
        latitude = (float)37.66771;
        longitude = (float)128.7053;
    }


    // 지도 열기...
    // 아래와 같은 양식으로 한다.
    //https://www.google.com/maps/dir/37.483782,126.9003409/37.5081441,126.8385434



    //=================================================================================================================//
    //===================================================== EXTRA FUNCTION ===============================================//
    //=================================================================================================================//    
    // 거리 계산 Function...
    double GetDistance(double lat1, double lon1, double lat2, double lon2)
    {
        double theta = lon1 - lon2;
        double dLat1 = deg2rad(lat1);
        double dLat2 = deg2rad(lat2);
        double dTheta = deg2rad(theta);

        double dist = Math.Sin(dLat1) * Math.Sin(dLat2) + Math.Cos(dLat1) * Math.Cos(dLat2) * Math.Cos(dTheta);
        dist = Math.Acos(dist);
        double dDistResult = rad2deg(dist);

        dDistResult = dDistResult * 60 * 1.1515;        
        dDistResult = dDistResult * 1.6093344;
        dDistResult = dDistResult * 1000.0;
        return dDistResult;
    }
    // 방향 각도 구하기....
    public short BearingP1toP2(double P1_latitude, double P1_longitude, double P2_latitude, double P2_longitude)
    {
        // 현재 위치 : 위도나 경도는 지구 중심을 기반으로 하는 각도이기 때문에 라디안 각도로 변환한다.
        double cur_Lat_radian = P1_latitude * (3.141592 / 180);
        double cur_Lon_radian = P1_longitude * (3.141592 / 180);
        // 목표 위치 : 위도나 경도는 지구 중심을 기반으로 하는 각도이기 때문에 라디안 각도로 변환한다.
        double Dest_Lat_radian = P2_latitude * (3.141592 / 180);
        double Dest_Lon_radian = P2_longitude * (3.141592 / 180);
        
        // radian distance               
        //radian_distance = Mathf.Acos(Mathf.Sin(DoubleToFloat(cur_Lat_radian)) * Mathf.Sin(DoubleToFloat(Dest_Lat_radian)) + Mathf.Cos(DoubleToFloat(cur_Lat_radian)) * Mathf.Cos(DoubleToFloat(Dest_Lat_radian)) * Mathf.Cos(DoubleToFloat(cur_Lon_radian - Dest_Lon_radian)));
        double radian_distance = Math.Acos(Math.Sin(cur_Lat_radian)) * Math.Sin(Dest_Lat_radian) + Math.Cos(cur_Lat_radian) * Math.Cos(Dest_Lat_radian) * Math.Cos(cur_Lon_radian - Dest_Lon_radian);

        // 목적지 이동 방향을 구한다.(현재 좌표에서 다음 좌표로 이동하기 위해서는 방향을 설정해야 한다. 라디안값이다.
        //double radian_bearing = Mathf.Acos((Mathf.Sin(DoubleToFloat(Dest_Lat_radian)) - Mathf.Sin(DoubleToFloat(cur_Lat_radian)) * Mathf.Cos(DoubleToFloat(radian_distance))) / (Mathf.Cos(DoubleToFloat(cur_Lat_radian)) * Mathf.Sin(DoubleToFloat(radian_distance))));
        double radian_bearing = Math.Acos(Math.Sin(Dest_Lat_radian)) - Math.Sin(cur_Lat_radian) * Math.Cos(radian_distance) / Math.Cos(cur_Lat_radian) * Math.Sin(radian_distance);

        // acos의 인수로 주어지는 x는 360분법의 각도가 아닌 radian(호도)값이다.       
        double true_bearing = 0;
        if (Math.Sin(Dest_Lon_radian - cur_Lon_radian) < 0)
        {
            true_bearing = radian_bearing * (180 / 3.141592);
            true_bearing = 360 - true_bearing;
        }
        else
        {
            true_bearing = radian_bearing * (180 / 3.141592);
        }
        return (short)true_bearing;
    }
    static double deg2rad(double _deg)
    {
        return (_deg * Mathf.PI / 180d);
    }
    static double rad2deg(double _rad)
    {
        return (_rad * 180d / Mathf.PI);
    }


    //===================================================== USELESS ===============================================//


    void AnsanCalculateDistance(int _iCount)
    {
        int iTempCount = _iCount;

        // test 용...        
        //double dMyLatitude = 37.274092;
        //double dMyLogitude = 126.839966;
        //pc 테스트용...        
        //aDistance[iTempCount] = GetDistance(dMyLatitude, dMyLogitude, aSpotLatitude[iTempCount], aSpotLongitude[iTempCount]);
        // 모바일용...
        aDistance[iTempCount] = GetDistance(latitude, longitude, aSpotLatitude[iTempCount], aSpotLongitude[iTempCount]);

        if (1000 <= aDistance[iTempCount])                                                                            // 1000 미터 이상, 즉 kilo 로 표시...
        {
            if (true == aStampImg[iTempCount].activeSelf)
                return;

            double dDist22 = aDistance[iTempCount] / 1000;
            string strDistance = dDist22.ToString("N1");
            strDistance += "kilo";

            aAnsanSpotsDistance[iTempCount].text = strDistance;
        }
        else
        {
            if (true == aStampImg[iTempCount].activeSelf)
                return;

            if (50 > aDistance[iTempCount])                                                                              // 100 미터 이하일 경우 도착으로 표시...
            {
                // arrived...                
                aAnsanSpotsDistance[iTempCount].text = "";
                //aAnsanSpotsStampBg[iTempCount].SetActive(true);
                //aAnsanSpotsStamp[iTempCount].SetActive(true);

                // 그리고 db 에 send 한다....
                // 도착...
                //RequestCheckArrive(iTempCount, dMyLatitude, dMyLogitude);
                RequestCheckArrive(iTempCount, latitude, longitude);
            }
            else
            {
                string strDistance = aDistance[iTempCount].ToString("N0");
                strDistance += "m";
                aAnsanSpotsDistance[iTempCount].text = strDistance;
            }
        }
    }


    // 안산시 버튼을 클리시에....
    public void AnsanButtonEvent()
    {
        // 총 4개를 받아온다.
        // 여기서 시작해서,,,
        // 아래처럼.....

        //string appID = PlayerInfo.Instance.GetAppID();
        string appID = AppID;
        string userID = PlayerInfo.Instance.GetUserID();
        //string kind = "social_sticker_first";
        string kind = "";
        string get_flag = "";
        string edu_type = "social_live";
        string game_type = "AR";
        int timeInfo = 0;

        /*
        if (0 == iStampCount)
            kind = "social_sticker_first";
        else if (1 == iStampCount)
            kind = "social_sticker_second";
        else if (2 == iStampCount)
            kind = "social_sticker_third";
        else if (3 == iStampCount)
            kind = "social_sticker_fourth";
        else if (4 == iStampCount)
            return;
        */

        get_flag = "Y";
        //kind = "social_sticker_first";
        kind = "social_sticker_second";


        Debug.Log("app ID: " + appID);
        Debug.Log("user ID: " + userID);

        // 여기서 먼저 체크를 해야 한다.
        // first ~ fourth 까지 각각 받아온다.
        // 총 4개의 코루틴 함수를 받아온다.

        // db 에서 stamp 결과 받아오기....
        // 성공...
        //DatabaseManager.Instance.GetStamp(appID, userID, kind, edu_type);

        // test 1 update...
        // 성공...
        DatabaseManager.Instance.GpsInfoUpdate(appID, userID, kind, get_flag, edu_type, game_type, timeInfo);

        /*
        CityButtons.SetActive(false);        
        Buyeo.SetActive(false);
        Chang_neong.SetActive(false);
        Chun_chen.SetActive(false);
        Ansan.SetActive(true);
        BackButton.SetActive(true); 
        
        eCity = GPS_CITY.ANSAN;
        */
    }

    /*
    public void CatchGpsUpdate(string _strAnswer)
    {
        Debug.Log("answer: " + _strAnswer);
        string strAnswer = _strAnswer;
        if ("Y" == strAnswer)
        {
            // 이미 획득한 Stamp..
            Debug.Log("already have");
            aStampImg[iStampCount].SetActive(true);
            aButtonImg[iStampCount].SetActive(false);
        }
        else
        {
            // 획득 안한 Stamp...
            aStampImg[iStampCount].SetActive(false);
            aButtonImg[iStampCount].SetActive(true);
        }

        // 여기서 call....
        iStampCount++;
        Invoke("StampStatusUpdate", 0.2f);
    }
    */
}
