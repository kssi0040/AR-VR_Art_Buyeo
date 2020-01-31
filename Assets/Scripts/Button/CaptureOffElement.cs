using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CaptureOffElement : MonoBehaviour
{
    public StagePlay m_StagePlay;
    public Camera main;
    // Start is called before the first frame update
    void Start()
    {
        m_StagePlay = FindObjectOfType<StagePlay>();
        main = GameObject.Find("Main Camera").GetComponent<Camera>();
        if (XML_Reader.Instance.scenarioToDict.StageSetDictionary[m_StagePlay.sceneLoader.currentStage].PageList[m_StagePlay.Index].EventType != "Paint")
        {
            main.gameObject.SetActive(false);
        }
        this.GetComponent<Button>().onClick.AddListener(delegate { this.buttonDown(); });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void buttonDown()
    {
        m_StagePlay.ARCore.transform.gameObject.SetActive(false);
        main.transform.gameObject.SetActive(true);
        main.transform.rotation = Quaternion.Euler(Vector3.zero);
        PlayerInfo.Instance.isComplite = true;
        m_StagePlay.forwardDown();
    }
}
