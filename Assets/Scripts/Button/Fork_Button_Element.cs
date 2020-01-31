using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fork_Button_Element : MonoBehaviour
{
    public StagePlay m_StagePlay;
    private int num = 2;

    // Start is called before the first frame update
    void Start()
    {
        m_StagePlay = FindObjectOfType<StagePlay>();
        this.GetComponent<Button>().onClick.AddListener(delegate {this.ButtonDown(); });        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ButtonDown()
    {
        if (num == XML_Reader.Instance.scenarioToDict.SelectItemEventDictionary[XML_Reader.Instance.scenarioToDict.StageSetDictionary[m_StagePlay.sceneLoader.currentStage].PageList[m_StagePlay.Index].EventID].Item)
        {
            Debug.Log("정답이다. 연금술사!");
            PlayerInfo.Instance.isComplite = true;
            m_StagePlay.forwardDown();
        }
        else
        {
            this.GetComponent<Button>().interactable = false;
            this.transform.GetChild(0).gameObject.SetActive(true);
        }
    }
}
