using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class NTweenPosition : MonoBehaviour
{
    public Vector3 From;
    public Vector3 To;

    [Header("Tweening")]
    public float duration = 1f;
    public bool loop = false;
    public LoopType loopType = LoopType.Restart;

    [Header("Actions")]
    public UnityEvent StartEvents = null;
    public UnityEvent CompleteEvents = null;

    private Tween myTween;

    private void OnEnable()
    {
        PlayForward();
    }
    private void OnDisable()
    {
        StopTween();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //==============================================================================================================//
    //=================================================== PLAY TWEEN =================================================//
    //==============================================================================================================//

    public void PlayForward()
    {
        this.transform.GetComponent<RectTransform>().anchoredPosition = From;
        if (false == loop)
            myTween = this.transform.GetComponent<RectTransform>().DOAnchorPos(To, duration).OnStart(StartEvents.Invoke).OnComplete(CompleteEvents.Invoke);
        else if (true == loop)
            myTween = this.transform.GetComponent<RectTransform>().DOAnchorPos(To, duration).OnStart(StartEvents.Invoke).OnComplete(CompleteEvents.Invoke).SetLoops(-1, loopType);        
    }

    public void PlayReverse()
    {
        this.transform.GetComponent<RectTransform>().anchoredPosition = To;
        if (false == loop)
            myTween = this.transform.GetComponent<RectTransform>().DOAnchorPos(From, duration).OnStart(StartEvents.Invoke).OnComplete(CompleteEvents.Invoke);
        else if (true == loop)
            myTween = this.transform.GetComponent<RectTransform>().DOAnchorPos(From, duration).OnStart(StartEvents.Invoke).OnComplete(CompleteEvents.Invoke).SetLoops(-1, loopType);
    }
    public void StopTween()
    {
        if(true == myTween.IsPlaying())
            myTween.Kill();            
    }
}
