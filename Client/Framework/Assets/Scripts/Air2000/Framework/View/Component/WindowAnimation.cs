using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Air2000;

public class WindowAnimation : MonoBehaviour
{
    public delegate void Callback();

    public Callback mCallback;


    void Awake()
    {
        transform.localScale = new Vector3(0.2f, 0.2f, 1f);
    }

    public void ColseWindow(Callback varCallback)
    {
        mCallback = varCallback;

        TweenScale[] TweenScale = transform.GetComponents<TweenScale>();

        if (TweenScale != null)
        {
            for (int i = 0; i < TweenScale.Length; i++)
            {
                TweenScale Tween = TweenScale[i];
                if (Tween.from == Vector3.one)
                {
                    Tween.SetOnFinished(Notify);

                    Tween.Toggle();

                    break;
                }
            }
        }
    }

    void Notify()
    {
        if (mCallback != null)
        {
            mCallback();
        }
    }

}
