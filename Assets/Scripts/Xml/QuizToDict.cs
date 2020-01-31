using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizToDict
{
    private QuizContainer quizContainer;

    public Dictionary<int, QuizTimeRec> QuizTimeRecDictionary = new Dictionary<int, QuizTimeRec>();

    public Dictionary<int, QuizTimeAtk> QuizTimeAtkDictionary = new Dictionary<int, QuizTimeAtk>();

    public Dictionary<int, LinkQuiz> LinkQuizDictionary = new Dictionary<int, LinkQuiz>();

    public Dictionary<int, ImgOptQuiz> ImgOptQuizDictionary = new Dictionary<int, ImgOptQuiz>();

    public Dictionary<int, OptQuiz> OptQuizDictionary = new Dictionary<int, OptQuiz>();

    public Dictionary<int, Typing1Quiz> Typing1QuizDictionary = new Dictionary<int, Typing1Quiz>();

    public Dictionary<int, Typing2Quiz> Typing2QuizDictionary = new Dictionary<int, Typing2Quiz>();


    public void Load(string file)
    {
        quizContainer = XmlIO.LoadXml<QuizContainer>(file);

        AddQuizTimeRecToDict();

        AddQuizTimeAtkToDict();

        AddLinkQuizToDict();

        AddOptQuizToDict();

        AddImgOptQuizToDict();

        AddTyping1QuizToDict();

        AddTyping2QuizToDict();
    }

    private void AddQuizTimeRecToDict()
    {
        if(QuizTimeRecDictionary.Count!=0)
        {
            QuizTimeRecDictionary.Clear();
        }

        if(quizContainer.quizLists.QuizTimeRecList==null)
        {
            Debug.Log("XML파일 QuizList의 QuizTimeRecList가 없다.");
            return;
        }

        for(int k=0;k<quizContainer.quizLists.QuizTimeRecList.Length;k++)
        {
            int key = quizContainer.quizLists.QuizTimeRecList[k].ID;
            QuizTimeRec QuizTimeRecInfo = quizContainer.quizLists.QuizTimeRecList[k];

            if(!QuizTimeRecDictionary.ContainsKey(key))
            {
                QuizTimeRecDictionary.Add(key, QuizTimeRecInfo);
            }
        }
    }

    private void AddQuizTimeAtkToDict()
    {
        if(QuizTimeAtkDictionary.Count!=0)
        {
            QuizTimeAtkDictionary.Clear();
        }

        if(quizContainer.quizLists.QuizTimeAtkList==null)
        {
            Debug.Log("XML파일 QuizList의 QuizTimeAtkList가 없다.");
            return;
        }

        for(int k=0;k<quizContainer.quizLists.QuizTimeAtkList.Length;k++)
        {
            int key = quizContainer.quizLists.QuizTimeAtkList[k].ID;
            QuizTimeAtk QuizTimeAtkInfo = quizContainer.quizLists.QuizTimeAtkList[k];

            if(!QuizTimeAtkDictionary.ContainsKey(key))
            {
                QuizTimeAtkDictionary.Add(key, QuizTimeAtkInfo);
            }
        }
    }

    private void AddLinkQuizToDict()
    {
        if(LinkQuizDictionary.Count!=0)
        {
            LinkQuizDictionary.Clear();
        }

        if(quizContainer.quizLists.LinkQuizList==null)
        {
            Debug.Log("XML파일 QuizList의 LinkQuizList가 없다.");
            return;
        }

        for(int k=0;k<quizContainer.quizLists.LinkQuizList.Length;k++)
        {
            int key = quizContainer.quizLists.LinkQuizList[k].ID;
            LinkQuiz LinkQuizInfo = quizContainer.quizLists.LinkQuizList[k];

            if(!LinkQuizDictionary.ContainsKey(key))
            {
                LinkQuizDictionary.Add(key, LinkQuizInfo);
            }
        }
    }

    private void AddOptQuizToDict()
    {
        if(OptQuizDictionary.Count!=0)
        {
            OptQuizDictionary.Clear();
        }

        if(quizContainer.quizLists.OptionQuizList==null)
        {
            Debug.Log("XML파일 QuizList의 OptionQuizList가 없다.");
            return;
        }

        for(int k=0;k<quizContainer.quizLists.OptionQuizList.Length;k++)
        {
            int key = quizContainer.quizLists.OptionQuizList[k].ID;
            OptQuiz OptionQuizInfo = quizContainer.quizLists.OptionQuizList[k];

            if(!OptQuizDictionary.ContainsKey(key))
            {
                OptQuizDictionary.Add(key, OptionQuizInfo);
            }
        }
    }

    private void AddImgOptQuizToDict()
    {
        if(ImgOptQuizDictionary.Count!=0)
        {
            ImgOptQuizDictionary.Clear();
        }

        if(quizContainer.quizLists.ImgOptQuizList==null)
        {
            Debug.Log("XML파일 QuizList의 ImgOptQuizList가 없다.");
            return;
        }

        for(int k=0;k<quizContainer.quizLists.ImgOptQuizList.Length;k++)
        {
            int key = quizContainer.quizLists.ImgOptQuizList[k].ID;
            ImgOptQuiz ImgOptQuizInfo = quizContainer.quizLists.ImgOptQuizList[k];

            if(!ImgOptQuizDictionary.ContainsKey(key))
            {
                ImgOptQuizDictionary.Add(key, ImgOptQuizInfo);
            }
        }
    }

    private void AddTyping1QuizToDict()
    {
        if(Typing1QuizDictionary.Count!=0)
        {
            Typing1QuizDictionary.Clear();
        }

        if(quizContainer.quizLists.Typing1QuizList==null)
        {
            Debug.Log("XML파일 QuizList의 Typing1QuizList가 없다.");
            return;
        }

        for(int k=0;k<quizContainer.quizLists.Typing1QuizList.Length;k++)
        {
            int key = quizContainer.quizLists.Typing1QuizList[k].ID;
            Typing1Quiz Typing1QuizInfo = quizContainer.quizLists.Typing1QuizList[k];

            if(!Typing1QuizDictionary.ContainsKey(key))
            {
                Typing1QuizDictionary.Add(key, Typing1QuizInfo);
            }
        }
    }

    private void AddTyping2QuizToDict()
    {
        if(Typing2QuizDictionary.Count!=0)
        {
            Typing2QuizDictionary.Clear();
        }

        if(quizContainer.quizLists.Typing2QuizList==null)
        {
            Debug.Log("XML파일 QuizList의 Typing2QuizList가 없다.");
            return;
        }

        for(int k=0;k<quizContainer.quizLists.Typing2QuizList.Length;k++)
        {
            int key = quizContainer.quizLists.Typing2QuizList[k].ID;
            Typing2Quiz Typing2QuizInfo = quizContainer.quizLists.Typing2QuizList[k];

            if(!Typing2QuizDictionary.ContainsKey(key))
            {
                Typing2QuizDictionary.Add(key, Typing2QuizInfo);
            }
        }
    }
}