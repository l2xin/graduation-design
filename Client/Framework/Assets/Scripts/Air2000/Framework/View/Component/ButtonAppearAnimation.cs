using UnityEngine;
using System.Collections;

public class ButtonAppearAnimation : MonoBehaviour
{

    TweenScale mTween;


    // Use this for initialization

    void Awake()
    {
        Resrt();
    }

    void OnEnable()
    {
        Play();
    }

    void Start()
    {

    }


    void OnDisable()
    {
        Resrt();
    }

    void Resrt()
    {
        if (mTween == null)
        {
            mTween = transform.GetComponent<TweenScale>();
        }
        if (mTween == null)
        {
            return;
        }
        transform.localScale = mTween.from;
    }

    void Play()
    {
        if (mTween == null)
        {
            mTween = transform.GetComponent<TweenScale>();
        }
        if (mTween == null)
        {
            return;
        }

        mTween.ResetToBeginning();
        mTween.PlayForward();
    }
}
