using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Air2000;
using Air2000.ImageEffect;

public class WindowTransitions : MonoBehaviour
{
    public delegate void Callback();


    TweenAlpha mTween0;
    TweenAlpha mTween1;
    EventDelegate TweenEndCallback;


    public UISprite mSprite;
    public UIPanel mPanel;
    public GetBlurBackground mBlur;

    public Callback mCallback;



    // Use this for initialization

    void Awake()
    {
        if (mBlur != null)
        {
            mBlur.GetBlurImg();
        }

        if (mPanel != null)
        {
            mPanel.transform.gameObject.SetActive(false);
        }
    }

    void Start()
    {
        StartCoroutine("OpenWindow");
    }

    public IEnumerator OpenWindow()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        if (transform == null || mSprite == null)
        {
            yield return null;
        }

        if (mTween0 == null || mTween1 == null)
        {
            mTween0 = null;
            mTween1 = null;

            mTween0 = mSprite.gameObject.AddComponent<TweenAlpha>();
            mTween0.from = 0f;
            mTween0.to = 1f;
            mTween0.duration = 0.2f;

            TweenEndCallback = new EventDelegate(CallbackOpenWindow);

            mTween0.SetOnFinished(TweenEndCallback);


            mTween1 = mSprite.gameObject.AddComponent<TweenAlpha>();
            mTween1.from = 1f;
            mTween1.to = 0f;
            mTween1.duration = 0.2f;
            mTween1.delay = 0.2f;
        }
        else
        {
            mTween0.ResetToBeginning();
            mTween0.Toggle();
            mTween1.ResetToBeginning();
            mTween1.Toggle();
        }
    }

    void CallbackOpenWindow()
    {
        if (mPanel != null)
        {
            mPanel.transform.gameObject.SetActive(true);
        }
    }





    public void CloseWindow(Callback varCallback)
    {
        mCallback = varCallback;

        if (mTween0 != null)
        {
            mTween0.RemoveOnFinished(TweenEndCallback);
            mTween0.ResetToBeginning();

            TweenEndCallback = new EventDelegate(CallbackColseWindow0);
            mTween0.SetOnFinished(TweenEndCallback);
            mTween0.Toggle();
        }
        else
        {
            CallbackColseWindow();
        }
    }
    void CallbackColseWindow0()
    {
        if (mPanel != null)
        {
            mPanel.transform.gameObject.SetActive(false);
        }
        if (mTween1 != null)
        {
            mTween1.delay = 0f;
            mTween1.ResetToBeginning();
            mTween1.SetOnFinished(CallbackColseWindow);
            mTween1.Toggle();
        }

    }

    void CallbackColseWindow()
    {
        if (mCallback != null)
        {
            mCallback();
        }
    }
}
