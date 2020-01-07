using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum TABLE_SET
{
    NONE = 0,
    TABLE,
    CURRY,
    RICE_NOODLE,
    CHINA_NOODLE,
    MISO_SOUP
}

public class UIManager : MonoBehaviour {

    // Use this for initialization
    public Text noticeText;
            

    

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetTextNotice(TABLE_SET _eFood)
    {
        if(TABLE_SET.NONE == _eFood)
        {
            noticeText.text = "";
        }
        else if (TABLE_SET.TABLE == _eFood)
        {
            noticeText.text = "설치 가능한 곳에 테이블을 설치해주세요.";
        }
        else if (TABLE_SET.CURRY == _eFood)
        {
            noticeText.text = "인도 카레를 식탁에 차리세요.";
        }
        else if (TABLE_SET.RICE_NOODLE == _eFood)
        {
            noticeText.text = "베트남 쌀국수를 식탁에 차리세요.";
        }
        else if (TABLE_SET.CHINA_NOODLE == _eFood)
        {
            noticeText.text = "중국 장수면을 식탁에 차리세요.";
        }
        else if (TABLE_SET.MISO_SOUP == _eFood)
        {
            noticeText.text = "한국 청국장을 식탁에 차리세요.";
        }
    }

    public void SetTestText(string _text)
    {
        noticeText.text = _text;
    }

}
