using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Meat_Button_Element : MonoBehaviour
{
    public StagePlay m_StagePlay;
    // Start is called before the first frame update
    void Awake()
    {
        m_StagePlay = FindObjectOfType<StagePlay>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        for(int k=0;k<XML_Reader.Instance.scenarioToDict.CreateObjType2DDictionary[m_StagePlay.sceneLoader.currentStage].Count;k++)
        {
            for(int j=0;j<m_StagePlay.sceneLoader.object2DDict[k].Count;j++)
            {
                if(m_StagePlay.sceneLoader.object2DDict[k][j].name== "forward")
                {
                    if(m_StagePlay.sceneLoader.object2DDict[k][j].activeSelf==false)
                    {
                        this.GetComponent<Button>().interactable = true;
                        this.GetComponent<Button>().onClick.AddListener(delegate { m_StagePlay.forwardDown(); });
                    }
                    else
                    {
                        this.GetComponent<Button>().interactable=false;
                    }
                }
            }
        }
    }
}
