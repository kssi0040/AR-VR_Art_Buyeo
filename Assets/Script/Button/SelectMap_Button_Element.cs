using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectMap_Button_Element : MonoBehaviour
{
    public CommonButton m_CommonButton;
    // Start is called before the first frame update
    void Start()
    {
        m_CommonButton = GameObject.Find("ButtonManager").GetComponent<CommonButton>();
        this.GetComponent<Button>().onClick.AddListener(delegate { m_CommonButton.SelectMapButtonDown(); });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
