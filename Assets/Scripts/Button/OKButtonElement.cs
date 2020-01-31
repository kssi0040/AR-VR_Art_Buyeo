using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OKButtonElement : MonoBehaviour
{
    public StagePlay m_StagePlay;

    // Start is called before the first frame update
    void Start()
    {
        m_StagePlay = FindObjectOfType<StagePlay>();
        this.GetComponent<Button>().onClick.AddListener(delegate { this.OnButtonDown(); });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnButtonDown()
    {
        PlayerInfo.Instance.isComplite = true;
        m_StagePlay.forwardDown();
    }
}
