using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CloseQuarter : MonoBehaviour
{
    public StagePlay m_StagePlay;
    public int Node;
    public int Group;
    public int ID;    

    // Start is called before the first frame update
    void Start()
    {
        m_StagePlay = GameObject.Find("StagePlay").GetComponent<StagePlay>();
        this.GetComponent<Button>().onClick.AddListener(delegate { this.OnButtonDown(Node); });
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnButtonDown(int num)
    {
        int temp = -1;
        if (PlayerInfo.Instance.isComplite)
        {
            XML_Reader.Instance.scenarioToDict.BranchDictionary[m_StagePlay.sceneLoader.currentStage][Group][ID] = true;

            for (int k = 0; k < XML_Reader.Instance.scenarioToDict.BranchDictionary[m_StagePlay.sceneLoader.currentStage][Group].Count; k++)
            {
                if (XML_Reader.Instance.scenarioToDict.BranchDictionary[m_StagePlay.sceneLoader.currentStage][Group][k] == false)
                {
                    temp = m_StagePlay.Next;
                    break;
                }
                else
                {
                    temp = num;
                }
            }

            m_StagePlay.Index = temp;
            m_StagePlay.Prev = XML_Reader.Instance.scenarioToDict.StageSetDictionary[m_StagePlay.sceneLoader.currentStage].PageList[m_StagePlay.Index].Prev;
            m_StagePlay.Next = XML_Reader.Instance.scenarioToDict.StageSetDictionary[m_StagePlay.sceneLoader.currentStage].PageList[m_StagePlay.Index].Next;
            PlayerInfo.Instance.isComplite = false;
            m_StagePlay.StageSet();
        }
    }
}
