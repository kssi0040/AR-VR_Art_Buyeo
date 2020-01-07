using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LinkButtonElement : MonoBehaviour
{
    public WebManage m_webManage;
    // Start is called before the first frame update
    void Start()
    {
        m_webManage = FindObjectOfType<WebManage>();
        this.GetComponent<Button>().onClick.AddListener(delegate { m_webManage.OpenWebView(); });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
