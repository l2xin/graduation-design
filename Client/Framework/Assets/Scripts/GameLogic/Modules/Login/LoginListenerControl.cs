/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: LoginListenerControl.cs
			// Describle: 登录控制
			// Created By:  hsu
			// Date&Time:  2016/3/4 10:47:08
            // Modify History:
            //
//----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Air2000;
using UnityEngine;

namespace GameLogic
{
    public class LoginListenerControl : BaseListenerControl
    {
        public LoginModel pLoginModel
        {
            get { return mModel as LoginModel; }
        }
        public LoginListenerControl(LoginModel varModel) : base(varModel) { }

        #region inherit method
        public override void OnViewInited()
        {
            base.OnViewInited();
        }

        public override void OnModelInited()
        {
            base.OnModelInited();
        }

        public override void OnPlayerLogin(Player varPlayer)
        {
            base.OnPlayerLogin(varPlayer);
			pLoginModel.RegisterEvent();
        }
        #endregion

        #region for ui response
        #endregion
    }
}
