using UnityEngine;
using System.Collections;

public class AnchorAdjust : MonoBehaviour
{
    public int mWidth = 1224;
    public int mHeight = 720;


    public UITexture mUITexture;

    float mRelative_x;
    float mRelative_y;

    void Awake()
    {
        mRelative_x = transform.localPosition.x / (mWidth*0.5f);

        mRelative_y = transform.localPosition.y / mHeight;
    }

    void Update()
    {
        Vector3 tempVec3 = transform.localPosition;

        if (mUITexture == null)
        {
            return;
        }

        float width = mUITexture.width;
        float height = mUITexture.height;

        tempVec3.x = width*0.5f * mRelative_x;
        tempVec3.y = height * mRelative_y;

       transform.localPosition = tempVec3;
    }
}
