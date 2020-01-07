using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

//Xml 파일 내부의 문자를 어떻게 구조화 할지 정의
[XmlRoot("ScenarioInfo")]
public class ScenarioContainer 
{
    [XmlElement("MaxStage")]
    public int MaxStage;

    [XmlElement("MaxMedal2D")]
    public int MaxMedal2D;

    [XmlElement("MaxMedal3D")]
    public int MaxMedal3D;

    [XmlArray("StageInfoList"), XmlArrayItem("StageInfo")]
    public StageInfo[] StageInfoList;

    [XmlElement("MedalList")]
    public Medal MedalList;

    [XmlArray("StageSetList"), XmlArrayItem("StageSet")]
    public StageSet[] StageSetList;

    [XmlElement("EventList")]
    public EventList EventSet;
}

public struct StageInfo
{
    [XmlAttribute("num")]
    public int ID;

    [XmlElement("CanvasCount")]
    public int CanvasCount;

    [XmlElement("CharacterList")]
    public CharacterList characterList;

    [XmlElement("ObjectList")]
    public ObjectList objectList;
}

public struct CharacterList
{
    [XmlArray("CharType2D"), XmlArrayItem("Canvas")]
    public CanvasChar[] CharType2D;

    [XmlElement("CharType3D")]
    public WorldChar CharType3D;
}

public struct CanvasChar
{
    [XmlAttribute("ID")]
    public int ID;

    [XmlArray("CreateCharList"), XmlArrayItem("CreateChar")]
    public CreateChar[] CreateCharList;
}

public struct WorldChar
{
    [XmlArray("CreateCharList"), XmlArrayItem("CreateChar")]
    public CreateChar3D[] CreateCharList;
}

public struct CreateChar
{
    [XmlAttribute("ID")]
    public int ID;

    [XmlAttribute("Name")]
    public string Name;

    [XmlAttribute("Desc")]
    public string Desc;
}

public struct CreateChar3D
{
    [XmlAttribute("ID")]
    public int ID;

    [XmlAttribute("Name")]
    public string Name;

    [XmlAttribute("Desc")]
    public string Desc;
}

public struct ObjectList
{
    [XmlArray("ObjType2D"), XmlArrayItem("Canvas")]
    public CanvasObj[] ObjType2D;

    [XmlElement("ObjType3D")]
    public WorldObj ObjType3D;
}

public struct CanvasObj
{
    [XmlAttribute("ID")]
    public int ID;

    [XmlArray("CreateObjList"), XmlArrayItem("CreateObj")]
    public CreateObj[] CreateObjList;
}

public struct WorldObj
{
    [XmlArray("CreateObjList"), XmlArrayItem("CreateObj")]
    public CreateObj3D[] CreateObjList;
}

public struct CreateObj
{
    [XmlAttribute("ID")]
    public int ID;

    [XmlAttribute("Name")]
    public string Name;

    [XmlAttribute("Desc")]
    public string Desc;
}

public struct CreateObj3D
{
    [XmlAttribute("ID")]
    public int ID;

    [XmlAttribute("Name")]
    public string Name;

    [XmlAttribute("Desc")]
    public string Desc;
}

public struct AnchorMin
{
    [XmlAttribute("x")]
    public float x;

    [XmlAttribute("y")]
    public float y;

    public Vector2 GetAnchorMin()
    {
        return new Vector2(x, y);
    }

    public void SetAnchorMin(Vector2 v)
    {
        x = v.x;
        y = v.y;  
    }
}

public struct AnchorMax
{
    [XmlAttribute("x")]
    public float x;

    [XmlAttribute("y")]
    public float y;

    public Vector2 GetAnchorMax()
    {
        return new Vector2(x, y);
    }

    public void SetAnchorMax(Vector2 v)
    {
        x = v.x;
        y = v.y;
    }
}

public struct Pivot
{
    [XmlAttribute("x")]
    public float x;

    [XmlAttribute("y")]
    public float y;

    public Vector2 GetPivot()
    {
        return new Vector2(x, y);
    }

    public void SetPivot(Vector2 v)
    {
        x = v.x;
        y = v.y;
    }
}

public struct OffsetMin
{
    [XmlAttribute("x")]
    public float x;

    [XmlAttribute("y")]
    public float y;

    public Vector2 GetOffsetMin()
    {
        return new Vector2(x, y);
    }

    public void SetOffsetMin(Vector2 v)
    {
        x = v.x;
        y = v.y;
    }
}

public struct OffsetMax
{
    [XmlAttribute("x")]
    public float x;

    [XmlAttribute("y")]
    public float y;

    public Vector2 GetOffsetMax()
    {
        return new Vector2(x, y);
    }

    public void SetOffsetMax(Vector2 v)
    {
        x = v.x;
        y = v.y;
    }
}

public struct Scale
{
    [XmlAttribute("x")]
    public float x;

    [XmlAttribute("y")]
    public float y;

    [XmlAttribute("z")]
    public float z;

    public Vector3 GetScale()
    {        
        return new Vector3(x, y, z);        
    }

    public void SetScale(Vector3 v)
    {
        x = v.x;
        y = v.y;
        z = v.z;
    }
}

public struct Pos
{
    [XmlAttribute("x")]
    public float x;

    [XmlAttribute("y")]
    public float y;

    [XmlAttribute("z")]
    public float z;

    public Vector3 GetPos()
    {
        return new Vector3(x, y, z);
    }

    public void SetPos(Vector3 v)
    {
        x = v.x;
        y = v.y;
        z = v.z;
    }
}

public struct Rot
{
    [XmlAttribute("x")]
    public float x;

    [XmlAttribute("y")]
    public float y;

    [XmlAttribute("z")]
    public float z;

    public Quaternion GetRot()
    {
        return Quaternion.Euler(x, y, z);
    }

    public void SetRot(Quaternion q)
    {
        var euler = q.eulerAngles;
        x = euler.x;
        y = euler.y;
        z = euler.z;
    }
}

public struct Medal
{
    [XmlArray("MedalType2D"), XmlArrayItem("Canvas")]
    public CanvasMedal[] MedalType2D;

    [XmlElement("MedalType3D")]
    public WorldMedal MedalType3D;
}

public struct CanvasMedal
{
    [XmlAttribute("ID")]
    public int ID;

    [XmlArray("CreateMedalList"), XmlArrayItem("CreateMedal")]
    public CreateMedal[] CreateMedalList;
}

public struct WorldMedal
{
    [XmlArray("CreateMedalList"), XmlArrayItem("CreateMedal")]
    public CreateMedal3D[] CreateMedalList;
}

public struct CreateMedal
{
    [XmlAttribute("ID")]
    public int ID;

    [XmlAttribute("Name")]
    public string Name;

    [XmlAttribute("Desc")]
    public string Desc;
}

public struct CreateMedal3D
{
    [XmlAttribute("ID")]
    public int ID;

    [XmlAttribute("Name")]
    public string Name;

    [XmlAttribute("Desc")]
    public string Desc;
}

public struct StageSet
{
    [XmlAttribute("num")]
    public int ID;

    [XmlArray("BranchList"), XmlArrayItem("BranchItem")]
    public BranchList[] branchLists;

    [XmlArray("PageList"), XmlArrayItem("Stage")]
    public Stage[] PageList;
}

public struct BranchList
{
    [XmlAttribute("ID")]
    public int ID;

    [XmlElement("Count")]
    public int Count;
}

public struct Stage
{
    [XmlAttribute("Prev")]
    public int Prev;

    [XmlAttribute("Index")]
    public int Index;

    [XmlAttribute("Next")]
    public int Next;

    [XmlAttribute("EventType")]
    public string EventType;

    [XmlAttribute("EventID")]
    public int EventID;

    [XmlElement("BackGround")]
    public BackGround BG;
}

public struct BackGround
{
    [XmlElement("Image")]
    public string image;

    [XmlElement("Music")]
    public Music BGM;

    [XmlElement("Effect")]
    public Effect effect;

    [XmlElement("Narration")]
    public Narration narration;

    [XmlElement("Navi")]
    public Navi navi;
}

public struct Music
{
    [XmlAttribute("On")]
    public int OnOff;

    [XmlAttribute("Src")]
    public string Src;
}

public struct Effect
{
    [XmlAttribute("On")]
    public int OnOff;

    [XmlAttribute("Src")]
    public string Src;
}

public struct Narration
{
    [XmlAttribute("On")]
    public int OnOff;

    [XmlAttribute("Src1")]
    public string Src1;

    [XmlAttribute("Src2")]
    public string Src2;
}

public struct Navi
{
    [XmlAttribute("On")]
    public int OnOff;

    [XmlAttribute("Index")]
    public int Index;
}

public struct EventList
{
    [XmlArray("TextEvent"), XmlArrayItem("InitEvent")]
    public TextEvent[] TextEventList;

    [XmlArray("PopUpEvent"), XmlArrayItem("InitEvent")]
    public PopUpEvent[] PopUpEventList;

    [XmlArray("CardPopUpEvent"), XmlArrayItem("InitEvent")]
    public CardPopUpEvent[] CardPopUpEventList;

    [XmlArray("SphereImageOnEvent"), XmlArrayItem("InitEvent")]
    public SphereImageOnEvent[] SphereImageOnEventList;

    [XmlArray("SpeechBubbleOnEvent"), XmlArrayItem("InitEvent")]
    public SpeechBubbleOnEvent[] SpeechBubbleOnEventOnList;

    [XmlArray("QuarterSelectEvent"), XmlArrayItem("InitEvent")]
    public QuarterSelectEvent[] QuarterSelectEventList;

    [XmlArray("QuarterEvent"), XmlArrayItem("InitEvent")]
    public QuarterEvent[] QuarterEventList;

    [XmlArray("LoadSceneEvent"), XmlArrayItem("InitEvent")]
    public LoadSceneEvent[] LoadSceneEventList;

    [XmlArray("QuizEvent"), XmlArrayItem("InitEvent")]
    public QuizEvent[] QuizEventList;

    [XmlArray("CaptureEvent"), XmlArrayItem("InitEvent")]
    public CaptureEvent[] CaptureEventList;

    [XmlArray("ShotEvent"), XmlArrayItem("InitEvent")]
    public ShotEvent[] ShotEventList;

    [XmlArray("PanelPopEvent"), XmlArrayItem("InitEvent")]
    public PanelPopEvent[] PanelPopEventList;

    [XmlArray("TableSettingEvent"), XmlArrayItem("InitEvent")]
    public TableSettingEvent[] TableSettingEventList;

    [XmlArray("SelectItemEvent"), XmlArrayItem("InitEvent")]
    public SelectItemEvent[] SelectItemEventList;

    [XmlArray("InputFieldEvent"), XmlArrayItem("InitEvent")]
    public InputFieldEvent[] InputFieldEventList;

    [XmlArray("VideoPlayEvent"), XmlArrayItem("InitEvent")]
    public VideoPlayEvent[] VideoPlayEventList;

    [XmlArray("TouchGameEvent"), XmlArrayItem("InitEvent")]
    public TouchGameEvent[] TouchGameEventList;

    [XmlArray("DeleteBlurEvent"), XmlArrayItem("InitEvent")]
    public DeleteBlurEvent[] DeleteBlurEventList;

    [XmlArray("PaintEvent"), XmlArrayItem("InitEvent")]
    public PaintEvent[] PaintEventList;

    [XmlArray("FindGameEvent"), XmlArrayItem("InitEvent")]
    public FindGameEvent[] FindGameEventList;
}

public struct TextEvent
{
    [XmlAttribute("ID")]
    public int ID;

    [XmlElement("InitCharList")]
    public InitCharList charList;

    [XmlElement("InitObjList")]
    public InitObjList objList;

    [XmlElement("Text")]
    public string text;    
}

public struct PopUpEvent
{
    [XmlAttribute("ID")]
    public int ID;

    [XmlElement("InitCharList")]
    public InitCharList charList;

    [XmlElement("InitObjList")]
    public InitObjList objList;

    [XmlElement("Text")]
    public string text;

    [XmlElement("Link")]
    public Link link;
}

public struct Link
{
    [XmlAttribute("On")]
    public int OnOff;

    [XmlAttribute("Name")]
    public string Name;

    [XmlAttribute("URL")]
    public string URL;
}

public struct CardPopUpEvent
{
    [XmlAttribute("ID")]
    public int ID;

    [XmlElement("InitCharList")]
    public InitCharList charList;

    [XmlElement("InitObjList")]
    public InitObjList objList;

    [XmlElement("Title")]
    public string Title;

    [XmlElement("Text")]
    public string text;
}

public struct SphereImageOnEvent
{
    [XmlAttribute("ID")]
    public int ID;

    [XmlElement("InitCharList")]
    public InitCharList charList;

    [XmlElement("InitObjList")]
    public InitObjList objList;

    [XmlElement("Text")]
    public string text;

    [XmlElement("SphereImage")]
    public string SphereImage;
}

public struct SpeechBubbleOnEvent
{
    [XmlAttribute("ID")]
    public int ID;

    [XmlElement("InitCharList")]
    public InitCharList charList;

    [XmlElement("InitObjList")]
    public InitObjList objList;

    [XmlElement("Text")]
    public string text;

    [XmlElement("Pos")]
    public string Pos;
}

public struct QuarterSelectEvent
{
    [XmlAttribute("ID")]
    public int ID;

    [XmlElement("ActiveObjList")]
    public ActiveObjList objList;
}

public struct QuarterEvent
{
    [XmlAttribute("ID")]
    public int ID;

    [XmlElement("InitCharList")]
    public InitCharList charList;

    [XmlElement("InitObjList")]
    public InitObjList objList;

    [XmlElement("Text")]
    public string text;

    [XmlElement("Index")]
    public Index indexList;

    [XmlElement("Node")]
    public int Node;
}

public struct LoadSceneEvent
{
    [XmlAttribute("ID")]
    public int ID;

    [XmlElement("Scene")]
    public string SceneName;
}

public struct QuizEvent
{
    [XmlAttribute("ID")]
    public int ID;

    [XmlElement("InitCharList")]
    public InitCharList charList;

    [XmlElement("InitObjList")]
    public InitObjList objList;

    [XmlElement("Quiz")]
    public QuizInfo QuizInfoList;
}

public struct CaptureEvent
{
    [XmlAttribute("ID")]
    public int ID;

    [XmlElement("InitCharList")]
    public InitCharList charList;

    [XmlElement("InitObjList")]
    public InitObjList objList;
}

public struct ShotEvent
{
    [XmlAttribute("ID")]
    public int ID;

    [XmlElement("InitCharList")]
    public InitCharList charList;

    [XmlElement("InitObjList")]
    public InitObjList objList;

    [XmlElement("Text")]
    public string text;
}

public struct PanelPopEvent
{
    [XmlAttribute("ID")]
    public int ID;

    [XmlElement("InitCharList")]
    public InitCharList charList;

    [XmlElement("InitObjList")]
    public InitObjList objList;

    [XmlElement("Text")]
    public string text;
}

public struct TableSettingEvent
{
    [XmlAttribute("ID")]
    public int ID;

    [XmlElement("InitObjList")]
    public InitObjList objList;    
}

public struct SelectItemEvent
{
    [XmlAttribute("ID")]
    public int ID;

    [XmlElement("InitCharList")]
    public InitCharList charList;

    [XmlElement("InitObjList")]
    public InitObjList objList;

    [XmlElement("Text")]
    public string text;

    [XmlElement("Item")]
    public int Item;
}

public struct InputFieldEvent
{
    [XmlAttribute("ID")]
    public int ID;

    [XmlElement("InitCharList")]
    public InitCharList charList;

    [XmlElement("InitObjList")]
    public InitObjList objList;

    [XmlElement("Text")]
    public string text;
}

public struct VideoPlayEvent
{
    [XmlAttribute("ID")]
    public int ID;

    [XmlElement("InitCharList")]
    public InitCharList charList;

    [XmlElement("InitObjList")]
    public InitObjList objList;

    [XmlElement("Text")]
    public string text;

    [XmlElement("Clip")]
    public string Clip;
}

public struct TouchGameEvent
{
    [XmlAttribute("ID")]
    public int ID;

    [XmlElement("Count")]
    public Count count;

    [XmlElement("ObjectList")]
    public ObjectList CreateObjList;

    [XmlElement("InitObjList")]
    public InitObjList objList;

    [XmlElement("Text")]
    public string Text;
}

public struct PaintEvent
{
    [XmlAttribute("ID")]
    public int ID;
}

public struct DeleteBlurEvent
{
    [XmlAttribute("ID")]
    public int ID;

    [XmlElement("InitCharList")]
    public InitCharList charList;

    [XmlElement("InitObjList")]
    public InitObjList objList;

    [XmlElement("Text")]
    public string Text;

    [XmlElement("MaxCount")]
    public int MaxCount;
}

public struct FindGameEvent
{
    [XmlAttribute("ID")]
    public int ID;

    [XmlElement("InitObjList")]
    public InitObjList objList;
}
public struct Index
{
    [XmlAttribute("Group")]
    public int Group;

    [XmlAttribute("ID")]
    public int ID;
}

public struct Count
{
    [XmlAttribute("min")]
    public int min;

    [XmlAttribute("max")]
    public int max;
}

public struct QuizInfo
{
    [XmlAttribute("Type")]
    public string Type;

    [XmlAttribute("ID")]
    public int ID;
}

public struct InitCharList
{
    [XmlArray("CharType2D"), XmlArrayItem("Canvas")]
    public CanvasInitChar[] CharType2D;

    [XmlElement("CharType3D")]
    public WorldInitChar CharType3D;
}

public struct CanvasInitChar
{
    [XmlAttribute("ID")]
    public int ID;

    [XmlArray("InitCharList"), XmlArrayItem("InitChar")]
    public InitChar[] InitCharList;
}

public struct WorldInitChar
{
    [XmlArray("InitCharList"), XmlArrayItem("InitChar")]
    public InitChar3D[] InitCharList;
}

public struct InitChar
{
    [XmlAttribute("ID")]
    public int ID;

    [XmlAttribute("Name")]
    public string Name;

    [XmlAttribute("Desc")]
    public string Desc;

    [XmlAttribute("Anim")]
    public string Anim;

    [XmlElement("AnchorMin")]
    public AnchorMin anchorMin;

    [XmlElement("AnchorMax")]
    public AnchorMax anchorMax;

    [XmlElement("Pivot")]
    public Pivot pivot;

    [XmlElement("OffsetMin")]
    public OffsetMin offsetMin;

    [XmlElement("OffsetMax")]
    public OffsetMax offsetMax;

    [XmlElement("Rot")]
    public Rot Rotation;

    [XmlElement("Scale")]
    public Scale scale;  
}

public struct InitChar3D
{
    [XmlAttribute("ID")]
    public int ID;

    [XmlAttribute("Name")]
    public string Name;

    [XmlAttribute("Desc")]
    public string Desc;

    [XmlAttribute("Anim")]
    public string Anim;

    [XmlElement("Pos")]
    public Pos Position;

    [XmlElement("Rot")]
    public Rot Rotation;

    [XmlElement("Scale")]
    public Scale scale;
}

public struct InitObjList
{
    [XmlArray("ObjType2D"), XmlArrayItem("Canvas")]
    public CanvasInitObj[] ObjType2D;

    [XmlElement("ObjType3D")]
    public WorldInitObj ObjType3D;
}

public struct CanvasInitObj
{
    [XmlAttribute("ID")]
    public int ID;

    [XmlArray("InitObjList"), XmlArrayItem("InitObj")]
    public InitObj[] InitObjList;
}

public struct WorldInitObj
{
    [XmlArray("InitObjList"), XmlArrayItem("InitObj")]
    public InitObj3D[] InitObjList;
}

public struct InitObj
{
    [XmlAttribute("ID")]
    public int ID;

    [XmlAttribute("Name")]
    public string Name;

    [XmlAttribute("Desc")]
    public string Desc;

    [XmlAttribute("Anim")]
    public string Anim;

    [XmlElement("AnchorMin")]
    public AnchorMin anchorMin;

    [XmlElement("AnchorMax")]
    public AnchorMax anchorMax;

    [XmlElement("Pivot")]
    public Pivot pivot;

    [XmlElement("OffsetMin")]
    public OffsetMin offsetMin;

    [XmlElement("OffsetMax")]
    public OffsetMax offsetMax;

    [XmlElement("Rot")]
    public Rot Rotation;

    [XmlElement("Scale")]
    public Scale scale;
}

public struct InitObj3D
{
    [XmlAttribute("ID")]
    public int ID;

    [XmlAttribute("Name")]
    public string Name;

    [XmlAttribute("Desc")]
    public string Desc;

    [XmlAttribute("Anim")]
    public string Anim;

    [XmlElement("Pos")]
    public Pos Position;

    [XmlElement("Rot")]
    public Rot Rotation;

    [XmlElement("Scale")]
    public Scale scale;
}

public struct ActiveObjList
{
    [XmlArray("ObjType2D"), XmlArrayItem("Canvas")]
    public CanvasActiveObj[] ObjType2D;

    [XmlElement("ObjType3D")]
    public WorldActiveObj ObjType3D;
}

public struct CanvasActiveObj
{
    [XmlAttribute("ID")]
    public int ID;

    [XmlArray("ActiveObjList"), XmlArrayItem("ActiveObj")]
    public ActiveObj[] ActiveObjList;
}

public struct WorldActiveObj
{
    [XmlArray("ActiveObjList"), XmlArrayItem("ActiveObj")]
    public ActiveObj3D[] ActiveObjList;
}

public struct ActiveObj
{
    [XmlAttribute("ID")]
    public int ID;

    [XmlAttribute("Name")]
    public string Name;

    [XmlAttribute("Desc")]
    public string Desc;

    [XmlAttribute("Anim")]
    public string Anim;
}

public struct ActiveObj3D
{
    [XmlAttribute("ID")]
    public int ID;

    [XmlAttribute("Name")]
    public string Name;

    [XmlAttribute("Desc")]
    public string Desc;

    [XmlAttribute("Anim")]
    public string Anim;
}