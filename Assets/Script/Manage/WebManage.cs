using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WebManage : MonoBehaviour
{
    public GameObject WebPrefab;
    public string strURL = string.Empty;
	// Use this for initialization
	void Start () {
        //OpenWebView();
	}
	
	// Update is called once per frame
	void Update ()
    {

	}

    public void initURL(string _strURL)
    {
        strURL = _strURL;
    }
    public void OpenWebView()
    {
        GameObject newViewer = Instantiate(WebPrefab) as GameObject;
        newViewer.transform.SetParent(this.transform);
        newViewer.transform.localScale = new Vector3(1, 1, 1);
        newViewer.transform.localPosition = new Vector3(1, 1, 1);
        newViewer.transform.GetComponent<WebView>().initURL(strURL);
        newViewer.transform.GetComponent<WebView>().StartWebView();
    }
}
