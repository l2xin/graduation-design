/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: BaseListenerControl.cs
			// Describle:  监听
			// Created By:  hsu
			// Date&Time:  2016/3/3 20:26:25
            // Modify History:
            //
//----------------------------------------------------------------*/
#define OPEN_DEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Air2000;

namespace GameLogic
{
    public class BaseListenerControl : GTModuleListenerControl, IPlayerManagerListener
    {
        private PlayerManager mPlayerManager;
        public PlayerManager pPlayerManager
        {
            get { return mPlayerManager; }
            set { mPlayerManager = value; }
        }
        public BaseView pView
        {
            get { return mView as BaseView; }
            set { mView = value; }
        }
        public BaseModel pModel
        {
            get { return mModel as BaseModel; }
            set
            {
                if (mModel != null)
                {
                    mModel.RemoveListener(this);
                }
                if (value != null)
                {
                    value.AddListener(this);
                } mModel = value;
            }
        }

        public BaseListenerControl(BaseModel varModel)
            : base(varModel)
        {
			PlayerManager.GetSingleton().AddListener(this);
        }

        #region Implement for IPlayerManagerListener
        public virtual void OnPlayerLogin(Player varPlayer) { }
        public virtual void OnPlayerLogout(Player varPlayer) { }
        public virtual void OnPlayerOffline(Player varPlayer) { }
        #endregion

        #region Implement for IModelListener
        public override void OnModelInited()
        {
            base.OnModelInited();
            PrintMsg(GetType().ToString() + " OnModelInited");
        }
        public override void OnModelDestroyed()
        {
            base.OnModelDestroyed();
            PrintMsg(GetType().ToString() + " OnModelDestroyed");
        }
        public override void OnViewAssigned(GTView varView)
        {
            base.OnViewAssigned(varView);
            pView = varView as BaseView;
            if (pView != null)
            {
                pView.AddListener(this);
            }
            PrintMsg(GetType().ToString() + " OnViewAssigned");
        }
        public override void OnModelStateChanged(ModelState varModelState)
        {
            base.OnModelStateChanged(varModelState);
            PrintMsg(GetType().ToString() + " OnModelStateChanged: "+varModelState.ToString());
        }
        #endregion

        #region Implement for IViewListener
        public override void OnViewInited()
        {
            base.OnViewInited();
            PrintMsg(GetType().ToString() + " OnViewInited");
        }
        public override void OnViewStart()
        {
            base.OnViewStart();
            PrintMsg(GetType().ToString() + " OnViewStart");
        }
        public override void OnViewEnable()
        {
            base.OnViewEnable();
            PrintMsg(GetType().ToString() + " OnViewEnable");
        }
        public override void OnViewDisable()
        {
            base.OnViewDisable();
            PrintMsg(GetType().ToString() + " OnViewDisable");
            mModel.ChangeModelState(ModelState.Background);
        }
        public override void OnViewDestroy()
        {
            base.OnViewDestroy();
            if (mView != null)
            {
                mView.RemoveListener(this);
            }
            PrintMsg(GetType().ToString() + " OnViewDestroy");
        }
        #endregion

        #region for debug
        private void PrintMsg(string varMsg)
        {
#if UNITY_EDITOR && OPEN_DEBUG
            Helper.Log(varMsg);
#endif
        }
        #endregion
    }
}
