using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioToDict
{
    private ScenarioContainer scenarioContainer;

    public Dictionary<int, StageInfo> StageInfoDictionary = new Dictionary<int, StageInfo>();

    public Dictionary<int, CanvasChar>[] CreateCharType2DDictionary;

    public Dictionary<int, CreateChar>[,] CreateChar2DDictionary;

    public Dictionary<int, CreateChar3D>[] CreateChar3DDictionary;

    public Dictionary<int, CanvasObj>[] CreateObjType2DDictionary;

    public Dictionary<int, CreateObj>[,] CreateObj2DDictionary;

    public Dictionary<int, CreateObj3D>[] CreateObj3DDictionary;

    //AR 스티커, 도장 관련
    public Dictionary<int, CanvasMedal> CreateMedalType2DDictionary = new Dictionary<int, CanvasMedal>();

    public Dictionary<int, CreateMedal>[] CreateMedal2DDictionary;

    public Dictionary<int, CreateMedal3D> CreateMedal3DDictionary = new Dictionary<int, CreateMedal3D>();

    //스테이지 관련
    public Dictionary<int, StageSet> StageSetDictionary = new Dictionary<int, StageSet>();

    public Dictionary<int, TextEvent> TextEventDictionary = new Dictionary<int, TextEvent>();

    public Dictionary<int, PopUpEvent> PopUpEventDictionary = new Dictionary<int, PopUpEvent>();

    public Dictionary<int, SphereImageOnEvent> SphereImageOnEventDictionary = new Dictionary<int, SphereImageOnEvent>();

    public Dictionary<int, SpeechBubbleOnEvent> SpeechBubbleOnEventDictionary = new Dictionary<int, SpeechBubbleOnEvent>();

    public Dictionary<int, QuarterSelectEvent> QuarterSelectEventDictionary = new Dictionary<int, QuarterSelectEvent>();

    public Dictionary<int, QuarterEvent> QuarterEventDictionary = new Dictionary<int, QuarterEvent>();

    public Dictionary<int, LoadSceneEvent> LoadSceneEventDictionary = new Dictionary<int, LoadSceneEvent>();

    public Dictionary<int, QuizEvent> QuizEventDictionary = new Dictionary<int, QuizEvent>();

    public Dictionary<int, CaptureEvent> CaptureEventDictionary = new Dictionary<int, CaptureEvent>();

    public Dictionary<int, ShotEvent> ShotEventDictionary = new Dictionary<int, ShotEvent>();

    public Dictionary<int, PanelPopEvent> PanelPopEventDictionary = new Dictionary<int, PanelPopEvent>();

    public Dictionary<int, TableSettingEvent> TableSettingEventDictionary = new Dictionary<int, TableSettingEvent>();

    public Dictionary<int, SelectItemEvent> SelectItemEventDictionary = new Dictionary<int, SelectItemEvent>();

    public Dictionary<int, CardPopUpEvent> CardPopUpEventDictionary = new Dictionary<int, CardPopUpEvent>();

    public Dictionary<int, InputFieldEvent> InputFieldEventDictionary = new Dictionary<int, InputFieldEvent>();

    public Dictionary<int, VideoPlayEvent> VideoPlayEventDictionary = new Dictionary<int, VideoPlayEvent>();

    public Dictionary<int, TouchGameEvent> TouchGameEventDictionary = new Dictionary<int, TouchGameEvent>();

    public Dictionary<int, DeleteBlurEvent> DeleteBlurEventDictionary = new Dictionary<int, DeleteBlurEvent>();

    public Dictionary<int, PaintEvent> PaintEventDictionary = new Dictionary<int, PaintEvent>();

    public Dictionary<int, FindGameEvent> FindGameEventDictionary = new Dictionary<int, FindGameEvent>();

    //분기 관련
    
    public Dictionary<int,bool>[] BranchOnOff;
    public Dictionary<int, Dictionary<int,bool>>[] BranchDictionary;

    public void Load(string file)
    {
        scenarioContainer = XmlIO.LoadXml<ScenarioContainer>(file);

        AddStageInfoToDict();

        AddCreateCharType2DToDict();

        AddCreateChar2DToDict();

        AddCreateChar3DToDict();

        AddCreateObjType2DToDict();

        AddCreateObj2DToDict();

        AddCreateObj3DToDict();

        AddStageSetToDict();

        AddTextEventToDict();

        AddPopUpEventToDict();

        AddCardPopUpEventToDict();

        AddSpeechBubbleOnEventToDict();

        AddSphereImageOnEventToDict();

        AddLoadSceneEventToDict();

        AddQuarterEventToDict();

        AddQuarterSelectEventToDict();

        AddQuizEventToDict();

        AddCaptureEventToDict();

        AddShotEventToDict();

        AddPanelPopEventToDict();

        AddTableSettingEventToDict();

        AddSelectItemEventToDict();

        AddBranchToDict();

        AddInputFieldEventToDict();

        AddVideoPlayEventToDict();

        AddTouchGameEventToDict();

        AddDeleteBlurEventToDict();

        AddPaintEventToDict();

        //AddFindGameEventToDict();
    }

    public int GetMaxStage()
    {
        return scenarioContainer.MaxStage;
    }

    public int GetMaxMedal2D()
    {
        return scenarioContainer.MaxMedal2D;
    }

    public int GetMaxMedal3D()
    {
        return scenarioContainer.MaxMedal3D;
    }

    public EventList GetEventList()
    {
        return scenarioContainer.EventSet;
    }

    public int GetCanvasCount(int index)
    {
        return scenarioContainer.StageInfoList[index].CanvasCount;
    }

    public CharacterList GetCharacterList(int index)
    {
        return scenarioContainer.StageInfoList[index].characterList;
    }

    public WorldChar GetCreateCharType3D(int index)
    {
        return scenarioContainer.StageInfoList[index].characterList.CharType3D;
    }

    public ObjectList GetObjectList(int index)
    {
        return scenarioContainer.StageInfoList[index].objectList;   
    }

    public int GetIndex(int StageSetID, int StageID)
    {
        return scenarioContainer.StageSetList[StageSetID].PageList[StageID].Index;
    }

    public int GetNext(int StageSetID, int StageID)
    {
        return scenarioContainer.StageSetList[StageSetID].PageList[StageID].Next;
    }

    public string GetEventType(int StageSetID, int StageID)
    {
        return scenarioContainer.StageSetList[StageSetID].PageList[StageID].EventType;
    }

    public int GetEventID(int StageSetID, int StageID)
    {
        return scenarioContainer.StageSetList[StageSetID].PageList[StageID].EventID;
    }

    public BackGround GetBackGround(int StageSetID, int StageID)
    {
        return scenarioContainer.StageSetList[StageSetID].PageList[StageID].BG;
    }

    private void AddStageInfoToDict()
    {
        if(StageInfoDictionary.Count!=0)
        {
            StageInfoDictionary.Clear();
        }

        if(scenarioContainer.StageInfoList==null)
        {
            Debug.Log("XML파일 StageInfo doesn't exist!");
            return;
        }

        for(int k=0;k<scenarioContainer.StageInfoList.Length;k++)
        {
            int key = scenarioContainer.StageInfoList[k].ID;
            StageInfo stageInfo = scenarioContainer.StageInfoList[k];

            if(!StageInfoDictionary.ContainsKey(key))
            {
                StageInfoDictionary.Add(key, stageInfo);
            }
        }    
    }

    private void AddCreateCharType2DToDict()
    {
        CreateCharType2DDictionary = new Dictionary<int, CanvasChar>[StageInfoDictionary.Count];
        for (int p = 0; p < StageInfoDictionary.Count; p++)
        {
            CreateCharType2DDictionary[p] = new Dictionary<int, CanvasChar>();
            CreateCharType2DDictionary[p].Clear();
        }

        for (int k = 0; k < scenarioContainer.StageInfoList.Length; k++)
        {
            if (scenarioContainer.StageInfoList[k].characterList.CharType2D == null)
            {
                Debug.Log("XML파일 StageInfo" + k + "번째 CharacterList의 CharType2D 없음");
                continue;
            }

            for (int j = 0; j < scenarioContainer.StageInfoList[k].characterList.CharType2D.Length; j++)
            {
                int key = scenarioContainer.StageInfoList[k].characterList.CharType2D[j].ID;
                CanvasChar CreateCharType2D = scenarioContainer.StageInfoList[k].characterList.CharType2D[j];
                if (!CreateCharType2DDictionary[k].ContainsKey(key))
                {
                    CreateCharType2DDictionary[k].Add(key, CreateCharType2D);
                }
            }
        }
    }

    private void AddCreateChar2DToDict()
    {
        CreateChar2DDictionary = new Dictionary<int, CreateChar>[StageInfoDictionary.Count, CreateCharType2DDictionary.Length];
        for(int k=0;k<StageInfoDictionary.Count;k++)
        {
            for(int j=0;j<CreateCharType2DDictionary[k].Count;j++)
            {
                if(CreateCharType2DDictionary[k][j].CreateCharList!=null)
                {
                    CreateChar2DDictionary[k, j] = new Dictionary<int, CreateChar>();
                    CreateChar2DDictionary[k, j].Clear();
                }
            }
        }

        for(int k=0;k<StageInfoDictionary.Count;k++)
        {
            for(int j=0;j<CreateCharType2DDictionary[k].Count;j++)
            {
                if(CreateCharType2DDictionary[k][j].CreateCharList==null)
                {
                    Debug.Log("XML파일 StageInfo " + k + "번째 CharacterList의 " + j + "번째의 CharType2D의 CreateCharList가 없음");
                    continue;
                }
                for(int p=0;p<CreateCharType2DDictionary[k][j].CreateCharList.Length;p++)
                {
                    int key = CreateCharType2DDictionary[k][j].CreateCharList[p].ID;
                    CreateChar CreateChar2DInfo = CreateCharType2DDictionary[k][j].CreateCharList[p];

                    if(!CreateChar2DDictionary[k,j].ContainsKey(key))
                    {
                        CreateChar2DDictionary[k, j].Add(key, CreateChar2DInfo);
                    }
                }
            }
        }        
    }

    private void AddCreateChar3DToDict()
    {
        CreateChar3DDictionary = new Dictionary<int, CreateChar3D>[StageInfoDictionary.Count];

        for (int k = 0; k < StageInfoDictionary.Count; k++)
        {
            if (StageInfoDictionary[k].characterList.CharType3D.CreateCharList != null)
            {
                CreateChar3DDictionary[k] = new Dictionary<int, CreateChar3D>();
                CreateChar3DDictionary[k].Clear();
            }
        }

        for (int k = 0; k < StageInfoDictionary.Count; k++)
        {
            if (StageInfoDictionary[k].characterList.CharType3D.CreateCharList == null)
            {
                Debug.Log("XML파일 StageInfo " + k + "번째 CharacterList의 CharType3D의 CreateCharList가 없음");
                continue;
            }
            for (int j = 0; j < StageInfoDictionary[k].characterList.CharType3D.CreateCharList.Length; j++)
            {
                int key = StageInfoDictionary[k].characterList.CharType3D.CreateCharList[j].ID;
                CreateChar3D CreateChar3DInfo = StageInfoDictionary[k].characterList.CharType3D.CreateCharList[j];
                if (!CreateChar3DDictionary[k].ContainsKey(key))
                {
                    CreateChar3DDictionary[k].Add(key, CreateChar3DInfo);
                }
            }
        }        
    }

    private void AddCreateObjType2DToDict()
    {
        CreateObjType2DDictionary = new Dictionary<int, CanvasObj>[StageInfoDictionary.Count];
        for(int k=0;k<StageInfoDictionary.Count;k++)
        {
            CreateObjType2DDictionary[k] = new Dictionary<int, CanvasObj>();
            CreateObjType2DDictionary[k].Clear();
        }

        for(int k=0;k<StageInfoDictionary.Count;k++)
        {
            if(StageInfoDictionary[k].objectList.ObjType2D==null)
            {
                Debug.Log("XML파일 StageInfo" + k + "번째 ObjectList의 ObjcetType2D 없음");
                continue;
            }

            for(int j=0;j<StageInfoDictionary[k].objectList.ObjType2D.Length;j++)
            {
                int key = StageInfoDictionary[k].objectList.ObjType2D[j].ID;
                CanvasObj CreateObjType2D = StageInfoDictionary[k].objectList.ObjType2D[j];

                if(!CreateObjType2DDictionary[k].ContainsKey(key))
                {
                    CreateObjType2DDictionary[k].Add(key, CreateObjType2D);
                }
            }            
        }
    }

    private void AddCreateObj2DToDict()
    {
        int temp = 0;
        for(int k=0;k<StageInfoDictionary.Count;k++)
        {
            if(temp<CreateObjType2DDictionary[k].Count)
            {
                temp = CreateObjType2DDictionary[k].Count;
            }
        }
        //CreateObj2DDictionary = new Dictionary<int, CreateObj>[StageInfoDictionary.Count, CreateObjType2DDictionary.Length];
        CreateObj2DDictionary = new Dictionary<int, CreateObj>[StageInfoDictionary.Count, temp];
        for (int k=0;k<StageInfoDictionary.Count;k++)
        {
            for(int j=0;j<CreateObjType2DDictionary[k].Count;j++)
            {
                if(CreateObjType2DDictionary[k][j].CreateObjList!=null)
                {
                    CreateObj2DDictionary[k, j] = new Dictionary<int, CreateObj>();
                    CreateObj2DDictionary[k, j].Clear();
                }
            }
        }

        for(int k=0;k<StageInfoDictionary.Count;k++)
        {
            for (int j = 0; j < CreateObjType2DDictionary[k].Count; j++)
            {
                if (CreateObjType2DDictionary[k][j].CreateObjList == null)
                {
                    Debug.Log("XML파일 StageInfo " + k + "번째 ObjectList의 " + j + "번째의 ObjectType2D의 CreateObjectList가 없음");
                    continue;
                }

                for (int p = 0; p < CreateObjType2DDictionary[k][j].CreateObjList.Length; p++)
                {
                    int key = CreateObjType2DDictionary[k][j].CreateObjList[p].ID;
                    CreateObj CreateObj2DInfo = CreateObjType2DDictionary[k][j].CreateObjList[p];

                    if(!CreateObj2DDictionary[k,j].ContainsKey(key))
                    {
                        CreateObj2DDictionary[k, j].Add(key, CreateObj2DInfo);
                    }
                }
            }            
        }
    }

    private void AddCreateObj3DToDict()
    {
        CreateObj3DDictionary = new Dictionary<int, CreateObj3D>[StageInfoDictionary.Count];

        for(int k=0;k<StageInfoDictionary.Count;k++)
        {
            if(StageInfoDictionary[k].objectList.ObjType3D.CreateObjList!=null)
            {
                CreateObj3DDictionary[k] = new Dictionary<int, CreateObj3D>();
                CreateObj3DDictionary[k].Clear();
            }
        }

        for(int k=0;k<StageInfoDictionary.Count;k++)
        {
            if(StageInfoDictionary[k].objectList.ObjType3D.CreateObjList==null)
            {
                Debug.Log("XML파일 StageInfo " + k + "번째 ObjectList의 ObjcetType3D의 CreateObjectList가 없음");
                continue;
            }
            for(int j=0;j<StageInfoDictionary[k].objectList.ObjType3D.CreateObjList.Length;j++)
            {
                int key = StageInfoDictionary[k].objectList.ObjType3D.CreateObjList[j].ID;
                CreateObj3D CreateObj3DInfo = StageInfoDictionary[k].objectList.ObjType3D.CreateObjList[j];

                if(!CreateObj3DDictionary[k].ContainsKey(key))
                {
                    CreateObj3DDictionary[k].Add(key, CreateObj3DInfo);
                }
            }           
        }        
    }

    private void AddStageSetToDict()
    {
        if(StageSetDictionary.Count!=0)
        {
            StageSetDictionary.Clear();
        }

        if(scenarioContainer.StageSetList==null)
        {
            Debug.Log("XML파일 StageSet doesn't exist!");
            return;
        }

        for(int k=0;k<scenarioContainer.StageSetList.Length;k++)
        {
            int key = scenarioContainer.StageSetList[k].ID;
            StageSet StageSetInfo = scenarioContainer.StageSetList[k];

            if(!StageSetDictionary.ContainsKey(key))
            {
                StageSetDictionary.Add(key, StageSetInfo);
            }
        }
    }

    private void AddTextEventToDict()
    {
        if(TextEventDictionary.Count!=0)
        {
            TextEventDictionary.Clear();
        }

        if (scenarioContainer.EventSet.TextEventList == null)
        {
            Debug.Log("XML파일 EventSet의 TextEventList가 없다.");
            return;
        }

        for(int k=0;k<scenarioContainer.EventSet.TextEventList.Length;k++)
        {
            int key = scenarioContainer.EventSet.TextEventList[k].ID;
            TextEvent TextEventInfo = scenarioContainer.EventSet.TextEventList[k];

            if(!TextEventDictionary.ContainsKey(key))
            {
                TextEventDictionary.Add(key, TextEventInfo);
            }
        }
    }

    private void AddPopUpEventToDict()
    {
        if(PopUpEventDictionary.Count!=0)
        {
            PopUpEventDictionary.Clear();
        }

        if (scenarioContainer.EventSet.PopUpEventList == null)
        {
            Debug.Log("XML파일 EventSet의 PopUpEventList가 없다.");
            return;
        }

        for(int k=0;k<scenarioContainer.EventSet.PopUpEventList.Length;k++)
        {
            int key = scenarioContainer.EventSet.PopUpEventList[k].ID;
            PopUpEvent PopUpEventInfo = scenarioContainer.EventSet.PopUpEventList[k];

            if(!PopUpEventDictionary.ContainsKey(key))
            {
                PopUpEventDictionary.Add(key, PopUpEventInfo);
            }
        }
    }

    private void AddSphereImageOnEventToDict()
    {
        if(SphereImageOnEventDictionary.Count!=0)
        {
            SphereImageOnEventDictionary.Clear();
        }

        if(scenarioContainer.EventSet.SphereImageOnEventList==null)
        {
            Debug.Log("XML파일 EventSet의 SphereImageOnEventList가 없다.");
            return;
        }

        for(int k=0;k<scenarioContainer.EventSet.SphereImageOnEventList.Length;k++)
        {
            int key = scenarioContainer.EventSet.SphereImageOnEventList[k].ID;
            SphereImageOnEvent SphereImageOnEventInfo = scenarioContainer.EventSet.SphereImageOnEventList[k];
            
            if(!SphereImageOnEventDictionary.ContainsKey(key))
            {
                SphereImageOnEventDictionary.Add(key, SphereImageOnEventInfo);
            }
        }
    }

    private void AddSpeechBubbleOnEventToDict()
    {
        if(SpeechBubbleOnEventDictionary.Count!=0)
        {
            SpeechBubbleOnEventDictionary.Clear();
        }

        if(scenarioContainer.EventSet.SpeechBubbleOnEventOnList==null)
        {
            Debug.Log("XML파일 EventSet의 SpeechBubbleOnEventOnList가 없다.");
            return;
        }

        for(int k=0;k<scenarioContainer.EventSet.SpeechBubbleOnEventOnList.Length;k++)
        {
            int key = scenarioContainer.EventSet.SpeechBubbleOnEventOnList[k].ID;
            SpeechBubbleOnEvent SpeechBubbleOnEventInfo = scenarioContainer.EventSet.SpeechBubbleOnEventOnList[k];
            
            if(!SpeechBubbleOnEventDictionary.ContainsKey(key))
            {
                SpeechBubbleOnEventDictionary.Add(key, SpeechBubbleOnEventInfo);
            }
        }
    }

    private void AddQuarterEventToDict()
    {
        if(QuarterEventDictionary.Count!=0)
        {
            QuarterEventDictionary.Clear();
        }

        if (scenarioContainer.EventSet.QuarterEventList == null)
        {
            Debug.Log("XML파일 EventSet의 QuarterEventList가 없다.");
            return;
        }

        for (int k = 0; k < scenarioContainer.EventSet.QuarterEventList.Length; k++)
        {
            int key = scenarioContainer.EventSet.QuarterEventList[k].ID;
            QuarterEvent QuarterEventInfo = scenarioContainer.EventSet.QuarterEventList[k];

            if (!QuarterEventDictionary.ContainsKey(key))
            {
                QuarterEventDictionary.Add(key, QuarterEventInfo);
            }
        }
    }

    private void AddLoadSceneEventToDict()
    {
        if(LoadSceneEventDictionary.Count!=0)
        {
            LoadSceneEventDictionary.Clear();
        }

        if(scenarioContainer.EventSet.LoadSceneEventList==null)
        {
            Debug.Log("XML파일 EventSet의 LoadSceneEventList가 없다.");
            return;
        }

        for(int k=0;k<scenarioContainer.EventSet.LoadSceneEventList.Length;k++)
        {
            int key = scenarioContainer.EventSet.LoadSceneEventList[k].ID;
            LoadSceneEvent LoadSceneEventInfo = scenarioContainer.EventSet.LoadSceneEventList[k];

            if(!LoadSceneEventDictionary.ContainsKey(key))
            {
                LoadSceneEventDictionary.Add(key, LoadSceneEventInfo);
            }
        }
    }

    private void AddQuizEventToDict()
    {
        if(QuizEventDictionary.Count!=0)
        {
            QuizEventDictionary.Clear();
        }

        if(scenarioContainer.EventSet.QuizEventList==null)
        {
            Debug.Log("XML파일 EventSet의 QuizEventList가 없다.");
            return;
        }

        for(int k=0;k<scenarioContainer.EventSet.QuizEventList.Length;k++)
        {
            int key = scenarioContainer.EventSet.QuizEventList[k].ID;
            QuizEvent QuizEventInfo = scenarioContainer.EventSet.QuizEventList[k];

            if(!QuizEventDictionary.ContainsKey(key))
            {
                QuizEventDictionary.Add(key, QuizEventInfo);
            }
        }
    }

    private void AddCaptureEventToDict()
    {
        if(CaptureEventDictionary.Count!=0)
        {
            CaptureEventDictionary.Clear();
        }

        if(scenarioContainer.EventSet.CaptureEventList==null)
        {
            Debug.Log("XML파일 EventSet의 CaptureEventList가 없다.");
            return;
        }

        for(int k=0;k<scenarioContainer.EventSet.CaptureEventList.Length;k++)
        {
            int key = scenarioContainer.EventSet.CaptureEventList[k].ID;
            CaptureEvent CaptureEventInfo = scenarioContainer.EventSet.CaptureEventList[k];

            if(!CaptureEventDictionary.ContainsKey(key))
            {
                CaptureEventDictionary.Add(key, CaptureEventInfo);
            }
        }
    }

    private void AddShotEventToDict()
    {
        if(ShotEventDictionary.Count!=0)
        {
            ShotEventDictionary.Clear();
        }

        if(scenarioContainer.EventSet.ShotEventList==null)
        {
            Debug.Log("XML파일 EventSet의 ShotEventList가 없다.");
            return;
        }

        for(int k=0;k<scenarioContainer.EventSet.ShotEventList.Length;k++)
        {
            int key = scenarioContainer.EventSet.ShotEventList[k].ID;
            ShotEvent ShotEventInfo = scenarioContainer.EventSet.ShotEventList[k];

            if(!ShotEventDictionary.ContainsKey(key))
            {
                ShotEventDictionary.Add(key, ShotEventInfo);
            }
        }
    }

    private void AddPanelPopEventToDict()
    {
        if(PanelPopEventDictionary.Count!=0)
        {
            PanelPopEventDictionary.Clear();
        }

        if(scenarioContainer.EventSet.PanelPopEventList==null)
        {
            Debug.Log("XML파일 EventSet의 PanelPopEventList가 없다.");
            return;
        }

        for(int k=0;k<scenarioContainer.EventSet.PanelPopEventList.Length;k++)
        {
            int key = scenarioContainer.EventSet.PanelPopEventList[k].ID;
            PanelPopEvent PanelPopEventInfo = scenarioContainer.EventSet.PanelPopEventList[k];

            if(!PanelPopEventDictionary.ContainsKey(key))
            {
                PanelPopEventDictionary.Add(key, PanelPopEventInfo);
            }
        }
    }

    private void AddTableSettingEventToDict()
    {
        if(TableSettingEventDictionary.Count!=0)
        {
            TableSettingEventDictionary.Clear();
        }

        if(scenarioContainer.EventSet.TableSettingEventList==null)
        {
            Debug.Log("XML파일 EventSet의 TableSettingEventList가 없다.");
            return;
        }

        for(int k=0;k<scenarioContainer.EventSet.TableSettingEventList.Length;k++)
        {
            int key = scenarioContainer.EventSet.TableSettingEventList[k].ID;
            TableSettingEvent TableSettingEventInfo = scenarioContainer.EventSet.TableSettingEventList[k];

            if(!TableSettingEventDictionary.ContainsKey(key))
            {
                TableSettingEventDictionary.Add(key, TableSettingEventInfo);
            }
        }
    }

    private void AddSelectItemEventToDict()
    {
        if(SelectItemEventDictionary.Count!=0)
        {
            SelectItemEventDictionary.Clear();
        }

        if(scenarioContainer.EventSet.SelectItemEventList==null)
        {
            Debug.Log("XML파일 EventSet의 SelectItemEventList가 없다.");
            return;
        }

        for(int k=0;k<scenarioContainer.EventSet.SelectItemEventList.Length;k++)
        {
            int key = scenarioContainer.EventSet.SelectItemEventList[k].ID;
            SelectItemEvent SelectItemEventInfo = scenarioContainer.EventSet.SelectItemEventList[k];

            if(!SelectItemEventDictionary.ContainsKey(key))
            {
                SelectItemEventDictionary.Add(key, SelectItemEventInfo);
            }
        }
    }

    private void AddQuarterSelectEventToDict()
    {
        if(QuarterSelectEventDictionary.Count!=0)
        {
            QuarterSelectEventDictionary.Clear();
        }

        if (scenarioContainer.EventSet.QuarterSelectEventList == null)
        {
            Debug.Log("XML파일 EventSet의 QuarterSelectEventList가 없다.");
            return;
        }

        for(int k=0;k<scenarioContainer.EventSet.QuarterSelectEventList.Length;k++)
        {
            int key = scenarioContainer.EventSet.QuarterSelectEventList[k].ID;
            QuarterSelectEvent QuarterSelectEventInfo = scenarioContainer.EventSet.QuarterSelectEventList[k];

            if(!QuarterSelectEventDictionary.ContainsKey(key))
            {
                QuarterSelectEventDictionary.Add(key, QuarterSelectEventInfo);
            }
        }
    }

    private void AddCardPopUpEventToDict()
    {
        if(CardPopUpEventDictionary.Count!=0)
        {
            CardPopUpEventDictionary.Clear();
        }

        if(scenarioContainer.EventSet.CardPopUpEventList==null)
        {
            Debug.Log("XML파일 EventSet의 CardPopUpEventList가 없다.");
            return;
        }

        for(int k=0;k<scenarioContainer.EventSet.CardPopUpEventList.Length;k++)
        {
            int key = scenarioContainer.EventSet.CardPopUpEventList[k].ID;
            CardPopUpEvent CardPopUpEventInfo = scenarioContainer.EventSet.CardPopUpEventList[k];
            
            if(!CardPopUpEventDictionary.ContainsKey(key))
            {
                CardPopUpEventDictionary.Add(key, CardPopUpEventInfo);
            }
        }
    }

    private void AddBranchToDict()
    {
        //Branch branch=new Branch();
        BranchDictionary = new Dictionary<int, Dictionary<int,bool>>[StageSetDictionary.Count];
        BranchOnOff = new Dictionary<int, bool>[StageSetDictionary.Count];
        for (int k = 0; k < XML_Reader.Instance.scenarioToDict.StageSetDictionary.Count; k++)
        {
            BranchDictionary[k] = new Dictionary<int, Dictionary<int, bool>>();
            BranchDictionary[k].Clear();
        }

        for(int k=0;k<XML_Reader.Instance.scenarioToDict.StageSetDictionary.Count;k++)
        {
            BranchOnOff[k] = new Dictionary<int, bool>();
            BranchOnOff[k].Clear();
        }

        for (int k = 0; k < XML_Reader.Instance.scenarioToDict.StageSetDictionary.Count; k++)
        {
            if (XML_Reader.Instance.scenarioToDict.StageSetDictionary[k].branchLists == null)
            {
                Debug.Log("XML파일 StageInfo" + k + "번째 branchLists 없음");
            }

            for (int j = 0; j < XML_Reader.Instance.scenarioToDict.StageSetDictionary[k].branchLists.Length; j++)
            {
                for (int q = 0; q < XML_Reader.Instance.scenarioToDict.StageSetDictionary[k].branchLists[j].Count; q++)
                {
                    int key1 = q;
                    bool temp = false;
                    if(!BranchOnOff[k].ContainsKey(key1))
                    {
                        BranchOnOff[k].Add(key1, temp);
                    }
                }
                int key2 = XML_Reader.Instance.scenarioToDict.StageSetDictionary[k].branchLists[j].ID;
                if (!BranchDictionary[k].ContainsKey(key2))
                {
                    BranchDictionary[k].Add(key2, BranchOnOff[k]);
                }               
            }
        }
    }

    private void AddInputFieldEventToDict()
    {
        if(InputFieldEventDictionary.Count!=0)
        {
            InputFieldEventDictionary.Clear();
        }

        if(scenarioContainer.EventSet.InputFieldEventList==null)
        {
            Debug.Log("XML파일 EventSet의 InputFieldEventList가 없다.");
            return;
        }

        for(int k=0;k<scenarioContainer.EventSet.InputFieldEventList.Length;k++)
        {
            int key = scenarioContainer.EventSet.InputFieldEventList[k].ID;
            InputFieldEvent InputFieldEventInfo = scenarioContainer.EventSet.InputFieldEventList[k];

            if(!InputFieldEventDictionary.ContainsKey(key))
            {
                InputFieldEventDictionary.Add(key, InputFieldEventInfo);
            }
        }
    }

    private void AddVideoPlayEventToDict()
    {
        if(VideoPlayEventDictionary.Count!=0)
        {
            VideoPlayEventDictionary.Clear();
        }

        if(scenarioContainer.EventSet.VideoPlayEventList==null)
        {
            Debug.Log("XML파일 EventSet의 VideoPlayEventList가 없다.");
            return;
        }

        for(int k=0;k<scenarioContainer.EventSet.VideoPlayEventList.Length;k++)
        {
            int key = scenarioContainer.EventSet.VideoPlayEventList[k].ID;
            VideoPlayEvent VideoPlayEventInfo = scenarioContainer.EventSet.VideoPlayEventList[k];

            if(!VideoPlayEventDictionary.ContainsKey(key))
            {
                VideoPlayEventDictionary.Add(key, VideoPlayEventInfo);
            }
        }
    }

    private void AddTouchGameEventToDict()
    {
        if(TouchGameEventDictionary.Count!=0)
        {
            TouchGameEventDictionary.Clear();
        }

        if(scenarioContainer.EventSet.TouchGameEventList==null)
        {
            Debug.Log("XML파일 EventSet의 TouchGameEventList가 없다.");
            return;
        }

        for(int k=0;k<scenarioContainer.EventSet.TouchGameEventList.Length;k++)
        {
            int key = scenarioContainer.EventSet.TouchGameEventList[k].ID;
            TouchGameEvent TouchGameEventInfo = scenarioContainer.EventSet.TouchGameEventList[k];

            if(!TouchGameEventDictionary.ContainsKey(key))
            {
                TouchGameEventDictionary.Add(key, TouchGameEventInfo);
            }
        }
    }

    private void AddDeleteBlurEventToDict()
    {
        if(DeleteBlurEventDictionary.Count!=0)
        {
            DeleteBlurEventDictionary.Clear();
        }

        if (scenarioContainer.EventSet.DeleteBlurEventList == null)
        {
            Debug.Log("XML파일 EventSet의 DeleteBlurEventList가 없다.");
            return;
        }

        for(int k=0;k<scenarioContainer.EventSet.DeleteBlurEventList.Length;k++)
        {
            int key = scenarioContainer.EventSet.DeleteBlurEventList[k].ID;
            DeleteBlurEvent DeleteBlurEventInfo = scenarioContainer.EventSet.DeleteBlurEventList[k];

            if(!DeleteBlurEventDictionary.ContainsKey(key))
            {
                DeleteBlurEventDictionary.Add(key, DeleteBlurEventInfo);
            }
        }
    }

    private void AddPaintEventToDict()
    {
        if(PaintEventDictionary.Count!=0)
        {
            PaintEventDictionary.Clear();
        }

        if(scenarioContainer.EventSet.PaintEventList==null)
        {
            Debug.Log("XML파일 EventSet의 PaintEventList가 없다.");
            return;
        }

        for(int k=0;k<scenarioContainer.EventSet.PaintEventList.Length;k++)
        {
            int key = scenarioContainer.EventSet.PaintEventList[k].ID;
            PaintEvent PaintEventInfo = scenarioContainer.EventSet.PaintEventList[k];

            if(!PaintEventDictionary.ContainsKey(key))
            {
                PaintEventDictionary.Add(key, PaintEventInfo);
            }
        }
    }

    //private void AddFindGameEventToDict()
    //{
    //    if (FindGameEventDictionary.Count != 0)
    //    {
    //        FindGameEventDictionary.Clear();
    //    }

    //    if (scenarioContainer.EventSet.FindGameEventList == null)
    //    {
    //        Debug.Log("XML파일 EventSet의 FindGameEventList가 없다.");
    //        return;
    //    }

    //    for (int k = 0; k < scenarioContainer.EventSet.FindGameEventList.Length; k++)
    //    {
    //        int key = scenarioContainer.EventSet.FindGameEventList[k].ID;
    //        FindGameEvent FindGameEventInfo = scenarioContainer.EventSet.FindGameEventList[k];

    //        if(!FindGameEventDictionary.ContainsKey(key))
    //        {
    //            FindGameEventDictionary.Add(key, FindGameEventInfo);
    //        }
    //    }
    //}
}


