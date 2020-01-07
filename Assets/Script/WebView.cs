using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System;
using System.Text;

public class WebView : MonoBehaviour
{
    public string strUrl;

   
    // Use this for initialization
    void Start ()
    {
        if (SceneManager.GetActiveScene().name == "Homepage")
        {
            StartWebView();
        }
	}

    public void initURL(string _strURL)
    {
        strUrl = _strURL;
    }

    public void StartWebView()
    {
        this.gameObject.AddComponent<WebViewObject>();
        this.GetComponent<WebViewObject>().Init((msg) => { Debug.Log(string.Format("CallFromJS[{0}]", msg)); });

        this.GetComponent<WebViewObject>().LoadURL(strUrl);
        this.GetComponent<WebViewObject>().SetVisibility(true);
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                if (this.GetComponent<WebViewObject>().CanGoBack() == true)
                {
                    this.GetComponent<WebViewObject>().GoBack();
                }
                else
                {
                    //if (SceneManager.GetActiveScene().name == "Homepage")
                    //{
                    //    PlayerInfo.Instance.isloading = true;
                    //    SceneManager.LoadScene("Start");
                    //}
                    //else
                    //{
                    //    PlayerInfo.Instance.isExit = false;
                    //    Destroy(this.gameObject);
                    //}
                    Destroy(this.gameObject);
                }
            }
        }
    }
}
