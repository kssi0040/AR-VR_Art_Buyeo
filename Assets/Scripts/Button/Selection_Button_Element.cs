using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Selection_Button_Element : MonoBehaviour
{
    public StagePlay m_StagePlay;
    public int Node=0;
    // Start is called before the first frame update
    void Start()
    {
        m_StagePlay = GameObject.Find("StagePlay").GetComponent<StagePlay>();
        this.GetComponent<Button>().onClick.AddListener(delegate {this.OnButtonDown(Node); });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnButtonDown(int num)
    {
        if (PlayerInfo.Instance.isComplite)
        {
            m_StagePlay.Index = num;
            m_StagePlay.Next = XML_Reader.Instance.scenarioToDict.StageSetDictionary[m_StagePlay.sceneLoader.currentStage].PageList[m_StagePlay.Index].Next;
            m_StagePlay.Prev = XML_Reader.Instance.scenarioToDict.StageSetDictionary[m_StagePlay.sceneLoader.currentStage].PageList[m_StagePlay.Index].Prev;
            PlayerInfo.Instance.isComplite = false;
            m_StagePlay.StageSet();
        }
    }
}
