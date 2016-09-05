/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: GameCameraManager.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/1/19 9:50:33
            // Modify History:
            //
//----------------------------------------------------------------*/

//using UnityEngine;
//using System.Collections;

//namespace Air2000
//{
    //public class GameCameraManager : StateMachine
    //{
    //    private static GameCameraManager mInstance = null;//实例
    //    private static GameObject mObject;//GameCameraManager 的GameObject 实例
    //    private static GameObject mFollowCamera;//跟随主角的相机

    //    /// <summary>
    //    /// GameCameraManager 的GameObject 实例
    //    /// </summary>
    //    public GameObject pObject
    //    {
    //        get { return mObject; }
    //    }

    //    public static Transform pFollowTransform
    //    {
    //        get { return mFollowCamera.transform; }
    //    }

    //    private GameCameraManager()
    //    {
    //        GameCamera tmpCamera = new NormalCamera(this);

    //        RegisterState(tmpCamera);
    //        SetNextState(typeof(NormalCamera).ToString());

    //        tmpCamera = new PointCamera(this);
    //        RegisterState(tmpCamera);

    //        tmpCamera = new MoveCamera(this);
    //        RegisterState(tmpCamera);

    //        tmpCamera = new AnimateCamera(this);
    //        RegisterState(tmpCamera);
    //    }

    //    public static GameCameraManager GetSingleton()
    //    {
    //        if (mInstance == null)
    //        {
    //            if (mObject == null)
    //            {
    //                mObject = new GameObject("CameraManager");
    //                mObject.AddComponent<AudioListener>();
    //                GameObject.DontDestroyOnLoad(mObject);
    //            }
    //            if (mFollowCamera == null)
    //            {
    //                mFollowCamera = new GameObject("FollowCamera");
    //                mFollowCamera.transform.SetParent(mObject.transform);
    //            }
    //            mInstance = new GameCameraManager();
    //        }
    //        return mInstance;
    //    }

    //    public override bool SetNextState(string stateName)
    //    {
    //        base.SetNextState(stateName);
    //        return true;
    //    }
    //}
//}