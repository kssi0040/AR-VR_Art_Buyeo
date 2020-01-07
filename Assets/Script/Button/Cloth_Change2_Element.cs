using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cloth_Change2_Element : MonoBehaviour
{
    public StagePlay m_StagePlay;
    public Image Cloth;
    Canvas tempCanvas;
    // Start is called before the first frame update
    void Start()
    {
        m_StagePlay = FindObjectOfType<StagePlay>();
        this.GetComponent<Button>().onClick.AddListener(delegate { this.ButtonDown(); });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ButtonDown()
    {
        for(int k=0;k<m_StagePlay.sceneLoader.CanvasList.Count;k++)
        {
            if(m_StagePlay.sceneLoader.CanvasList[k].name=="PhotoCanvas")
            {
                tempCanvas = m_StagePlay.sceneLoader.CanvasList[k];
            }
        }

        for(int k=0;k<tempCanvas.transform.GetChild(0).childCount;k++)
        {
            if(tempCanvas.transform.GetChild(0).GetChild(k).name=="Cloth")
            {
                Cloth = tempCanvas.transform.GetChild(0).GetChild(k).GetComponent<Image>();
            }
        }
        Sprite[] sprites = Resources.LoadAll<Sprite>("고려");
        Cloth.sprite = sprites[0];
    }
}
