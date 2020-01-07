using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GoogleMapApi : MonoBehaviour {

    // 정리해서 라이브러리 화해야 할 듯....
    public RawImage img;    
    private string g_strBaseURL = "https://maps.googleapis.com/maps/api/staticmap?center=";
    public double g_dLat = 37.572816;      // 위도(광화문광장 세종대왕 동상)
    public double g_dLon = 126.976848;    // 경도(광화문광장 세종대왕 동상)
    public int g_MapZoom = 17;
    public int g_MapWidth = 320;
    public int g_MapHeight = 240;
    public enum mapType { roadmap, satellite, hybrid, terrain }
    public mapType mapSelected;
    public int scale;
    private string g_strPath = "weight:3%7Ccolor:blue%7Cenc:{coaHnetiVjM??_SkM??~R";
    private string g_strGoogleAPIKey = "AIzaSyDjTS2CkLFSC1aBR635zk3jrHo20OsWQ2I";                       // google map api key !! 중요 !!!

    IEnumerator MapDraw()
    {
        // 베이스 + 센터 지정 + 줌 + 맵 사이즈 width + height + 스케일 + 맵 타입 + 마커(여러개 가능...)
        string url = g_strBaseURL + g_dLat.ToString() + "," + g_dLon.ToString() +
       "&zoom=" + g_MapZoom.ToString() +
       "&size=" + g_MapWidth.ToString() + "x" + g_MapHeight.ToString() +
       "&path=" + g_strPath +
       "&key=" + g_strGoogleAPIKey;
        //Debug.Log("URL: " + url);
        WWW www = new WWW(url);
        yield return www;
        img.texture = www.texture;
        img.SetNativeSize();        
    }
    
    // test... marker...
    // 수정해야 함....
    IEnumerator MapDrawMakrer1()
    {
        // 베이스 + 센터 지정 + 줌 + 맵 사이즈 width + height + 스케일 + 맵 타입 + 마커(여러개 가능...)
        string url = g_strBaseURL + g_dLat.ToString() + "," + g_dLon.ToString() +
       "&zoom=" + g_MapZoom.ToString() +
       "&size=" + g_MapWidth.ToString() + "x" + g_MapHeight.ToString() +
       // marker color = { black, brown, green, purple, yellow, blue, gray, orange, red, white } 에서 고를 수 있음...
       // 사이즈는 최대 4
       "&markers=size:4%7Ccolor:blue%7Clabel:test%7C37.572816,126.976848" +                     //marker 1...
       "&markers=size:4%7Ccolor:red%7Clabel:test%7C37.573917,126.978111" +                     //marker 2... // 계속 늘릴 수 있음....       
       "&markers=size:4%7Ccolor:red%7Clabel:test%7C37.572171,126.975245" +                     //marker 3...
       "&path=" + g_strPath +       
       "&key=" + g_strGoogleAPIKey;
        //Debug.Log("URL: " + url);
        WWW www = new WWW(url);
        yield return www;
        img.texture = www.texture;
        img.SetNativeSize();
    }


    // Use this for initialization
    void Start () {
        img = gameObject.GetComponent<RawImage>();
        //StartCoroutine("Map");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //======================================================================================================//
    //================================================= BUTTONS =============================================//
    //======================================================================================================//

    public void MapOn()
    {
        StartCoroutine("MapDraw");
    }
    public void MarkerMaker1()
    {
        //MapDrawMakrer1
        StartCoroutine("MapDrawMakrer1");
    }
    public void MarkerMaker2()
    {

    }

    //zoom in func test...
    public void ZoomInMap()
    {
        g_MapZoom++;
        StartCoroutine("MapDrawMakrer1");
    }

    public void ZoomOutMap()
    {
        g_MapZoom--;
        StartCoroutine("MapDrawMakrer1");
    }

    public void NavigationOn()
    {        
        StartCoroutine("NaviTester");
    }

    // navi test...
    IEnumerator NaviTester()
    {        
        string url = "https://maps.googleapis.com/maps/api/directions/json?origin=Disneyland&destination=Universal+Studios+Hollywood" + 
         g_dLat.ToString() + "," + g_dLon.ToString() +
        "&zoom=" + g_MapZoom.ToString() +
        "&size=" + g_MapWidth.ToString() + "x" + g_MapHeight.ToString() +
        
        // marker color = { black, brown, green, purple, yellow, blue, gray, orange, red, white } 에서 고를 수 있음...
        // 사이즈는 최대 4
        //"&markers=size:4%7Ccolor:blue%7Clabel:test%7C37.572816,126.976848" +                   //marker 1...
        //"&markers=size:4%7Ccolor:red%7Clabel:test%7C37.573917,126.978111" +                     //marker 2... 
        //"&markers=size:4%7Ccolor:red%7Clabel:test%7C37.572171,126.975245" +                     //marker 3...
        // 계속 늘릴 수 있음....       

        "&path=" + g_strPath +
        "&key=" + g_strGoogleAPIKey;

        Debug.Log("URL: " + url);
        WWW www = new WWW(url);
        yield return www;
        img.texture = www.texture;
        img.SetNativeSize();       
    }
}
