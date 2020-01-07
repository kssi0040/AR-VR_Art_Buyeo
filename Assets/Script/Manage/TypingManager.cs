using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class TypingManager : MonoBehaviour 
{
    [SerializeField]
    Text dialogue;

    private string text = "Q 2. 대한민국의 주권은 국민에게 있습니다. 주권의 의미를 알아볼까요?&#xA; &#xA;주인된 권리로 우리나라의 국민이라면 누구나 가진다.";

    StringBuilder sb1 = new StringBuilder();
    StringBuilder sb2 = new StringBuilder();

    //WaitForSeconds SpellingDelay = new WaitForSeconds(0.03f);
    public float Delay;
    WaitForSeconds SpellingDelay;
    //20191122 추가사항
    public Scrollbar m_Scrollbar;
    public StagePlay m_StagePlay;


    // Use this for initialization
    void Start () 
    {
        //TypingText(text, dialogue);
        SpellingDelay = new WaitForSeconds(Delay);
        m_StagePlay = FindObjectOfType<StagePlay>();
    }
	
	// Update is called once per frame
	void Update () {	}

    public void TypingText(string contents, Text uiText)
    {
        sb1.Append(contents);

        TypeSentence(uiText, m_Scrollbar);
    }
    private void TypeSentence(Text uiText, Scrollbar m_scrollbar)
    {
        StartCoroutine(ITypeSentence(uiText, m_scrollbar));
    }

    private IEnumerator ITypeSentence(Text uiText, Scrollbar uiScrollbar)
    {
        foreach (char letter in sb1.ToString().ToCharArray())
        {
            //_dialogueText.text += letter;
            sb2.Append(letter);
            uiText.text = sb2.ToString();

            yield return SpellingDelay;

            if (PlayerInfo.Instance.isClicked)
            {
                uiText.text = string.Empty;
                uiText.text = sb1.ToString();
                if (m_StagePlay != null)
                {
                    if (m_StagePlay.Narration != null)
                    {
                        m_StagePlay.Narration.Stop();
                        m_StagePlay.Narration.time = 0.0f;
                    }
                }
                break;
            }
        }
        PlayerInfo.Instance.isComplite = true;
        PlayerInfo.Instance.isClicked = false;
        //20191122 추가내용
        if (uiScrollbar != null)
        {
            if (uiScrollbar.IsActive() == true)
            {
                uiScrollbar.value = 0.0f;
            }
        }
        sb1.Length = 0;
        sb2.Length = 0;
        Debug.Log("Completed!");
    }
}
