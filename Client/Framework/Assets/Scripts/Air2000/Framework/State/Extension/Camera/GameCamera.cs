/*----------------------------------------------------------------
            // Copyright (C) 2015 南昌光速科技有限公司
            // 版权所有。 
            //
            // 文件名： GameCamera
            // 文件功能描述：游戏中各种摄像机定义。普通。主城，或者特殊情况
            //
            // 
            // 创建标识：Neil_20150407

//----------------------------------------------------------------*/

//using UnityEngine;
//using System.Collections;

//namespace Air2000
//{
    //public delegate void GameCameraDel(object varParams);

    //public class GameCamera : State
    //{

    //    private Camera mCamera;
    //    protected GameObject mObj;


    //    public GameCamera(string name, GameCameraManager varManager)
    //        : base(name)
    //    {

    //        mObj = new GameObject();

    //        mCamera = mObj.AddComponent<Camera>();
    //        // Camera看到的层级不包括UI;
    //        mCamera.cullingMask = Helper.EverythingBut
    //        (
    //             LayerMask.NameToLayer("UI"),
    //             LayerMask.NameToLayer("QiumoUI"),
    //             LayerMask.NameToLayer("QiumoDialog"),
    //             LayerMask.NameToLayer("Default"),
    //             LayerMask.NameToLayer("ThrModel")
    //        );
    //        //mCamera.cullingMask = Helper.OnlyIncluding()

    //        mCamera.fieldOfView = 35.0f;
    //        mObj.name = name;
    //        mObj.transform.SetParent(varManager.pObject.transform);
    //        mCamera.clearFlags = CameraClearFlags.Color;
    //        mCamera.backgroundColor = Color.black;
    //        mObj.SetActive(false);
    //    }


    //    public Camera pCamera
    //    {
    //        get { return mCamera; }
    //        set { mCamera = value; }
    //    }

    //    /// update every frame;
    //    override public void Update()
    //    {

    //    }

    //    /// state begin .
    //    override public void Begin()
    //    {
    //        if (pCamera != null)
    //        {
    //            mObj.SetActive(true);
    //        }
    //    }

    //    /// state end.
    //    override public void End()
    //    {
    //        if (pCamera != null)
    //        {
    //            mObj.SetActive(false);
    //        }
    //    }


    //}





    //public class PointCamera : GameCamera
    //{

    //    private Vector3 mPoition;
    //    private Vector3 mRation;


    //    public Vector3 pPostion
    //    {
    //        get
    //        {
    //            return mPoition;
    //        }
    //        set
    //        {
    //            mPoition = value;
    //            SetObjPosition(mPoition);
    //        }
    //    }

    //    public Vector3 pRotation
    //    {
    //        get
    //        {
    //            return mRation;
    //        }
    //        set
    //        {
    //            mRation = value;
    //            SetObjRotation(mRation);
    //        }
    //    }


    //    public PointCamera(GameCameraManager pManager)
    //        : base(typeof(PointCamera).ToString(), pManager)
    //    {
    //        mPoition = Vector3.zero;
    //        mRation = Vector3.zero;
    //    }

    //    /// state begin .
    //    override public void Begin()
    //    {
    //        base.Begin();
    //        SetObjPosition(mPoition);
    //        SetObjRotation(mRation);

    //    }

    //    void SetObjPosition(Vector3 varPos)
    //    {
    //        if (mObj != null)
    //        {
    //            mObj.transform.position = varPos;
    //        }
    //    }

    //    void SetObjRotation(Vector3 varRot)
    //    {
    //        if (mObj != null)
    //        {
    //            mObj.transform.rotation = Quaternion.Euler(varRot);
    //        }
    //    }


    //}



    ///// <summary>
    ///// 平滑移动相机.
    ///// </summary>
    //public class MoveCamera : GameCamera
    //{
    //    private GameCameraDel mCallBack;

    //    private object mParam;

    //    private Vector3 mBeginPosition;

    //    private Vector3 mEndPosition;

    //    private Quaternion mBeginQuternion;

    //    private Quaternion mEndQuternion;

    //    private float mSpeed;

    //    private bool mFrag;

    //    private float mLasttime;

    //    private float mDurtation;

    //    public MoveCamera(GameCameraManager pManager)
    //        : base(typeof(MoveCamera).ToString(), pManager)
    //    {
    //        mBeginPosition = pCamera.transform.position;
    //        mBeginQuternion = pCamera.transform.rotation;
    //        mSpeed = 0.1f;
    //    }

    //    public void SetEndCallBack(GameCameraDel varCallBack, object varParam)
    //    {
    //        mCallBack = varCallBack;
    //        mParam = varParam;
    //    }

    //    public float pSpeed
    //    {
    //        get { return mSpeed; }
    //        set { mSpeed = value; }
    //    }
    //    public Vector3 pBeginPosition
    //    {
    //        get { return mBeginPosition; }
    //        set { mBeginPosition = value; }
    //    }
    //    public Vector3 pEndPosition
    //    {
    //        get { return mEndPosition; }
    //        set { mEndPosition = value; }
    //    }

    //    public Quaternion pBeginQuternion
    //    {
    //        get { return mBeginQuternion; }
    //        set { mBeginQuternion = value; }
    //    }
    //    public Quaternion pEndQuternion
    //    {
    //        get { return mEndQuternion; }
    //        set { mEndQuternion = value; }
    //    }

    //    private void SetCameraBeginPosition(Vector3 varPosition)
    //    {
    //        mObj.transform.position = varPosition;
    //    }
    //    private void SetCameraEndPosition(Vector3 varPosition)
    //    {
    //        mEndPosition = varPosition;
    //    }
    //    private void SetBeginQuternion(Quaternion varQu)
    //    {
    //        mBeginQuternion = varQu;
    //    }
    //    private void SetEndQuternion(Quaternion varQu)
    //    {
    //        mEndQuternion = varQu;
    //    }

    //    private void CallBackFun()
    //    {
    //        if (mCallBack != null)
    //        {
    //            mCallBack(mParam);
    //            mCallBack = null;
    //        }
    //    }


    //    override public void Begin()
    //    {
    //        base.Begin();
    //        mLasttime = 0.0f;
    //        if (Helper.FlotEqualZero(mSpeed))
    //        {
    //            mDurtation = 1;
    //        }
    //        mDurtation = 1 / mSpeed;
    //        mFrag = true;
    //    }

    //    override public void Update()
    //    {
    //        if (mEndPosition == null || mEndQuternion == null || mFrag == false)
    //        {
    //            CallBackFun();
    //            return;
    //        }

    //        mLasttime += Time.deltaTime;
    //        float pre = mLasttime / mDurtation;
    //        if (pre >= 1.0f)
    //        {
    //            pre = 1.0f;
    //            if (mFrag == true)
    //            {
    //                CallBackFun();
    //                mFrag = false;
    //            }

    //            return;
    //        }

    //        if (pre <= 0.0f)
    //        {
    //            pre = 0.0f;
    //        }

    //        pCamera.transform.rotation = Quaternion.Slerp(mBeginQuternion, mEndQuternion, pre);

    //        pCamera.transform.position = mBeginPosition + (mEndPosition - mBeginPosition) * pre;
    //    }


    //    override public void End()
    //    {
    //        pCamera.transform.rotation = mEndQuternion;

    //        pCamera.transform.position = mEndPosition;

    //        base.End();
    //    }

    //}





    //public class NormalCamera : GameCamera
    //{
    //    private GameObject mTargetObj;

    //    private float mDistance;

    //    private float mXangle;

    //    private float mYangle;

    //    /// <summary>
    //    /// 角色的高度
    //    /// </summary>
    //    private Vector3 mTargetHight;

    //    private Transform mFollowCameraTran;


    //    public NormalCamera(GameCameraManager pManager)
    //        : base(typeof(NormalCamera).ToString(), pManager)
    //    {
    //        mDistance = 8.0f;

    //        mXangle = 20.8f;
    //        mYangle = 20.7f;

    //        mTargetHight = new Vector3(0f, 1.0f, 0f);
    //        mFollowCameraTran = GameCameraManager.pFollowTransform;
    //        mObj.transform.SetParent(mFollowCameraTran);
    //    }



    //    public GameObject pTarget
    //    {
    //        get { return mTargetObj; }
    //        set { mTargetObj = value; }
    //    }


    //    public float pDistance
    //    {
    //        get { return mDistance; }
    //        set { mDistance = value; }
    //    }


    //    public float pXangle
    //    {
    //        get { return mXangle; }
    //        set { mXangle = value; }
    //    }

    //    public float pYangle
    //    {
    //        get { return mYangle; }
    //        set { mYangle = value; }
    //    }




    //    public void SetOrige(Vector3 varEuler)
    //    {
    //        //pCamera.transform.localEulerAngles = varEuler;

    //        mFollowCameraTran.localEulerAngles = varEuler;

    //        return;
    //    }

    //    public override void Update()
    //    {
    //        if (mFollowCameraTran == null || pTarget == null)
    //        {
    //            return;
    //        }
    //        Vector3 oriPosition = pTarget.transform.position;
    //        oriPosition += mTargetHight;
    //        Vector3 dir = mFollowCameraTran.forward;
    //        dir.Normalize();
    //        mFollowCameraTran.position = oriPosition + dir * mDistance * (-1.0f);
    //        return;


    //        //if (pTarget == null)
    //        //{
    //        //    return;
    //        //}



    //        ////*
    //        //Vector3 oriPosition = pTarget.transform.position;

    //        //oriPosition += mTargetHight;

    //        //Vector3 dir = pCamera.transform.forward;


    //        //dir.Normalize();

    //        //pCamera.transform.position = oriPosition + dir * mDistance * (-1.0f);






    //        //return;


    //        //*/
    //        Vector3 newpos = new Vector3(Mathf.Cos(pYangle), Mathf.Sin(pXangle), Mathf.Sin(pYangle));

    //        newpos.Normalize();

    //        newpos *= pDistance;
    //        //newpos = pTarget.transform.TransformDirection(newpos);

    //        if (pCamera != null)
    //        {
    //            pCamera.transform.position = pTarget.transform.position + newpos;
    //            pCamera.transform.LookAt(pTarget.transform.position);

    //            //pCamera.transform.RotateAround(pTarget.transform.position,Vector3.up,pYangle);
    //        }

    //    }

    //    public override void End()
    //    {
    //        if (pCamera != null)
    //        {
    //            mObj.SetActive(false);
    //        }
    //    }
    //}

    ///// <summary>
    ///// 用于相机的抖动等
    ///// </summary>
    //public class AnimateCamera : GameCamera
    //{
    //    private GameObject mTargetObj;

    //    private float mDistance;

    //    private float mXangle;

    //    private float mYangle;

    //    /// <summary>
    //    /// 角色的高度
    //    /// </summary>
    //    private Vector3 mTargetHight;

    //    private Transform mFollowCameraTran;


    //    public void Cross(GameCamera varCamera)
    //    {
    //        if (varCamera == null)
    //        {
    //            return;
    //        }
    //        pCamera.transform.position = new Vector3(varCamera.pCamera.transform.position.x, varCamera.pCamera.transform.position.y, varCamera.pCamera.transform.position.z);
    //        pCamera.transform.rotation = varCamera.pCamera.transform.rotation;
    //    }


    //    public AnimateCamera(GameCameraManager pManager)
    //        : base(typeof(AnimateCamera).ToString(), pManager)
    //    {
    //        mDistance = 8.0f;
    //        mXangle = 20.8f;
    //        mYangle = 20.7f;
    //        mTargetHight = new Vector3(0f, 1.0f, 0f);
    //        pCamera.gameObject.AddComponent<Animation>();

    //        mFollowCameraTran = GameCameraManager.pFollowTransform;
    //        mObj.transform.SetParent(mFollowCameraTran);
    //    }
    //    public GameObject pTarget
    //    {
    //        get { return mTargetObj; }
    //        set { mTargetObj = value; }
    //    }
    //    public void SetOrige(Vector3 varEuler)
    //    {
    //        pCamera.transform.localEulerAngles = varEuler;

    //        return;
    //    }
    //    public override void Update()
    //    {
    //        if (pTarget == null)
    //        {
    //            return;
    //        }
    //        return;
    //    }
    //    public override void End()
    //    {
    //        if (pCamera != null)
    //        {
    //            mObj.SetActive(false);
    //        }
    //    }
    //}

//}


