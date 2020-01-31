using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class AnsElement : MonoBehaviour
{
    public Button ansBut;
    public RectTransform lrPos;
    UILineConnector m_UILineConnector;

    // Start is called before the first frame update
    void Start()
    {
        m_UILineConnector = FindObjectOfType<UILineConnector>();
        ansBut.onClick.AddListener(delegate { m_UILineConnector.AnsButtonCallBack(ansBut, lrPos); });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
