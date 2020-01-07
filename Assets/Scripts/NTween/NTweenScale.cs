using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class NTweenScale : MonoBehaviour
{
    public Vector3 From;
    public Vector3 To;

    [Header("Tweening")]
    public float duration = 1f;    
    public bool loop = false;

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
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayForward()
    {
        this.transform.localScale = From;
        myTween = this.transform.DOScale(To, duration).OnStart(StartEvents.Invoke).OnComplete(CompleteEvents.Invoke).SetEase(Ease.OutBounce);
    }
    public void PlayReverse()
    {
        this.transform.localScale = To;
        myTween = this.transform.DOScale(From, duration).OnStart(StartEvents.Invoke).OnComplete(CompleteEvents.Invoke).SetEase(Ease.OutBounce);
    }

    public void StopTween()
    {
        if (true == myTween.IsPlaying())
            myTween.Kill();
    }

}
