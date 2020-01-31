using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Button1_Element : MonoBehaviour
{
    public StagePlay m_StagePlay;
    // Start is called before the first frame update
    void Start()
    {
        m_StagePlay = FindObjectOfType<StagePlay>();        
        this.GetComponent<Button>().onClick.AddListener(delegate { this.OnbuttonDown(); });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnbuttonDown()
    {
        if (PlayerInfo.Instance.isComplite)
        {
            m_StagePlay.Index = 9;
            m_StagePlay.StageSet();
            PlayerInfo.Instance.isComplite = false;
        }
    }
}
