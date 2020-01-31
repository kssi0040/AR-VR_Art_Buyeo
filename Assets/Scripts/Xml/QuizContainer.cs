using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

[XmlRoot("Quiz")]
public class QuizContainer
{
    [XmlElement("List")]
    public QuizList quizLists;
}

public struct QuizList
{
    [XmlArray("QuizTimeRec"), XmlArrayItem("InitQuizInfo")]
    public QuizTimeRec[] QuizTimeRecList;

    [XmlArray("QuizTimeAtk"), XmlArrayItem("InitQuizInfo")]
    public QuizTimeAtk[] QuizTimeAtkList;

    [XmlArray("LinkQuiz"), XmlArrayItem("InitQuizInfo")]
    public LinkQuiz[] LinkQuizList;

    [XmlArray("Typing1Quiz"), XmlArrayItem("InitQuizInfo")]
    public Typing1Quiz[] Typing1QuizList;

    [XmlArray("Typing2Quiz"), XmlArrayItem("InitQuizInfo")]
    public Typing2Quiz[] Typing2QuizList;

    [XmlArray("ImgOptQuiz"), XmlArrayItem("InitQuizInfo")]
    public ImgOptQuiz[] ImgOptQuizList;

    [XmlArray("OptQuiz"), XmlArrayItem("InitQuizInfo")]
    public OptQuiz[] OptionQuizList;
}

public struct QuizTimeRec
{
    [XmlAttribute("ID")]
    public int ID;

    [XmlAttribute("Count")]
    public int Count;

    [XmlArray("QuizList"), XmlArrayItem("InitQuiz")]
    public InitQuiz[] InitQuizList;
}

public struct QuizTimeAtk
{
    [XmlAttribute("ID")]
    public int ID;

    [XmlAttribute("Count")]
    public int Count;

    [XmlAttribute("Time")]
    public float Time;

    [XmlArray("QuizList"), XmlArrayItem("InitQuiz")]
    public InitQuiz[] InitQuizList;

    //추가 스크립트 20190808
    [XmlElement("Node")]
    public Node node;
}

public struct InitQuiz
{
    [XmlAttribute("ID")]
    public int ID;

    [XmlAttribute("Type")]
    public string Type;

    //시간 기록하는 퀴즈 이벤트에서 퀴즈 종류들 여기에 다 때려박을 것
    [XmlElement("Typing1")]
    public Typing1Quiz Typing1QuizList;

    [XmlElement("Typing2")]
    public Typing2Quiz Typing2QuizList;

    [XmlElement("Opt")]
    public OptQuiz OptQuizList;

    [XmlElement("ImgOpt")]
    public ImgOptQuiz ImgOptQuizList;

    [XmlElement("Link")]
    public LinkQuiz LinkQuizList;
}

public struct Typing1Quiz
{
    [XmlAttribute("ID")]
    public int ID;

    [XmlElement("Question")]
    public string Quest;

    [XmlArray("AnswerList"), XmlArrayItem("InitAnswer")]
    public InitAnswer[] InitAnswerList;
}

public struct Typing2Quiz
{
    [XmlAttribute("ID")]
    public int ID;

    [XmlElement("Question")]
    public string Quest;

    [XmlArray("AnswerList"), XmlArrayItem("InitAnswer")]
    public InitAnswer[] InitAnswerList;
}

public struct OptQuiz
{
    [XmlAttribute("ID")]
    public int ID;

    [XmlElement("Question")]
    public string Quest;    

    [XmlArray("OrderPanel"), XmlArrayItem("InitItem")]
    public InitOptItem[] OrderList;

    [XmlArray("AnswerList"), XmlArrayItem("InitAnswer")]
    public InitAnswer[] InitAnswerList;
}

public struct ImgOptQuiz
{
    [XmlAttribute("ID")]
    public int ID;

    [XmlElement("Question")]
    public string Quest;

    [XmlArray("OrderPanel"), XmlArrayItem("InitItem")]
    public InitImgOptItem[] OrderList;

    [XmlArray("AnswerList"), XmlArrayItem("InitAnswer")]
    public InitAnswer[] InitAnswerList;
}

//왼쪽 패널의 Pair 값을 인덱스 삼아서 오른 패널에서 누른 버튼 이름과 오른쪽 패널 배열에 인덱스로 접근했을때 이름이랑 같으면 정답
public struct LinkQuiz
{
    [XmlAttribute("ID")]
    public int ID;

    [XmlElement("Text")]
    public string Text;

    [XmlArray("LeftPanel"), XmlArrayItem("InitItem")]
    public InitItem[] LeftItemList;

    [XmlArray("RightPanel"), XmlArrayItem("InitItem")]
    public InitItem[] RightItemList;
}

public struct InitItem
{
    [XmlAttribute("ID")]
    public int ID;

    [XmlAttribute("Name")]
    public string Name;

    [XmlElement("Sprite")]
    public string Sprite;

    [XmlElement("Text")]
    public string Text;

    [XmlElement("Pair")]
    public int Pair;
}

public struct InitOptItem
{
    [XmlAttribute("ID")]
    public int ID;

    [XmlAttribute("Name")]
    public string Name;

    [XmlElement("Text")]
    public string Text;
}

public struct InitImgOptItem
{
    [XmlAttribute("ID")]
    public int ID;

    [XmlAttribute("Name")]
    public string Name;

    [XmlElement("Sprite")]
    public string Sprite;

    [XmlElement("Text")]
    public string Text;
}

public struct InitAnswer
{
    [XmlAttribute("ID")]
    public int ID;

    [XmlAttribute("Answer")]
    public string Answer;
}
//추가 스크립트 20190808
public struct Node
{
    [XmlAttribute("Clear")]
    public int Clear;

    [XmlAttribute("fail")]
    public int fail;
}
