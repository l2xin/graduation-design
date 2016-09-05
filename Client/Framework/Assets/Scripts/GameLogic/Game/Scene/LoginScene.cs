/*----------------------------------------------------------------
            // Copyright (C) 2015 南昌光速科技有限公司
            // 版权所有。 
            //
            // 文件名： LoginScene
            // 文件功能描述：LoginScene
            //
            // 
            // 创建标识：hsu-20160113
            // 修改描述：
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using Air2000;

namespace GameLogic
{
    public class LoginScene : GameScene
    {
        public LoginScene()
            : base(SceneType.LoginScene.ToString())
        {

        }
        public override void Begin()
        {
            base.Begin();

            Application.LoadLevel(SceneType.LoginScene.ToString());
            WindowManager.GetSingleton().DestroyAllWin(true);
            WindowManager.GetSingleton().OpenWindow(LoginView.VIEW_KEY);

			LoginModel tempModel = ModelManager.GetSingleton().GetModelByType(ModelType.MT_Login) as LoginModel;
			if(tempModel != null)
			{
				tempModel.RegisterSceneEvent();
			}
        }

        public override void Update()
        {
            base.Update();
        }

        public override void End()
        {
            base.End();
        }
    }
}

