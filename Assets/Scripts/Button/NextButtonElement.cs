using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NextButtonElement : MonoBehaviour
{
    public StagePlay m_StagePlay;

    // Start is called before the first frame update
    void Start()
    {
        m_StagePlay = FindObjectOfType<StagePlay>();        
        this.GetComponent<Button>().onClick.AddListener(delegate { m_StagePlay.forwardDown(); });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
