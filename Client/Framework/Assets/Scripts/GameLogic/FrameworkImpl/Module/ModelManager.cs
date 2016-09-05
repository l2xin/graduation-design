/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: ModelManager.cs
			// Describle: 模块管理
			// Created By:  hsu
			// Date&Time:  2016/3/3 19:29:32
            // Modify History:
            //
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Air2000;

namespace GameLogic
{
    public enum ModelType
    {
        MT_App,
        /// <summary>
        /// 登录模块.
        /// </summary>
        MT_Login,
        /// <summary>
        /// 主界面.
        /// </summary>
        MT_Main,

        /// <summary>
        /// 匹配模块.
        /// </summary>
        MT_Match,

        /// <summary>
        /// 战斗模块.
        /// </summary>
        MT_Battle,

        /// <summary>
        /// 战斗结算模块.
        /// </summary>
        MT_BattleEnd,

        /// <summary>
        /// 商城模块
        /// </summary>
        MT_Shop,

        /// <summary>
        /// 系统设置.
        /// </summary>
        MT_Setting,

		/// <summary>
		/// 邮件模块
		/// </summary>
		MT_Email,

		/// <summary>
		/// 公告
		/// </summary>
		MT_Notice,

		/// <summary>
		/// 角色详情
		/// </summary>
		MT_RoleDetail,

		///每日签到;
		MT_SignatureBook,
        ///任务
        MT_Mission,
        /// 聊天
        MT_Chat,
        /// <summary>
        /// 体力
        /// </summary>
        MT_TiLi,
        /// <summary>
        /// 数据模块总数.
        /// </summary>
        Count,
    };
    public class ModelManager : GTModelManager/*, IModelManagerListener*/
    {
        private static ModelManager mInstance;
        public Player pSelfPlayer
        {
            get { return mSelfPlayer as Player; }
            set
            {
                mSelfPlayer = value;
            }
        }

        private ModelManager()
        {
        }

        public static ModelManager GetSingleton()
        {
            if (mInstance == null)
            {
                mInstance = new ModelManager();
            }
            return mInstance;
        }

        public override void InitModel()
        {
            AddModel(new LoginModel());
        }

        public BaseModel GetModelByType(ModelType varModelType)
        {
            GTModel model = null;
            mModels.TryGetValue((int)varModelType, out model);
            return model as BaseModel;
        }

        public bool AddModel(BaseModel varModel)
        {
            if (varModel == null)
            {
                return false;
            }
            bool bExist = mModels.ContainsKey(varModel.pModelID);
            if (!bExist)
            {
                mModels.Add(varModel.pModelID, varModel);
                return true;
            }
            return false;
        }

        public void DestroyModel(BaseModel varModel)
        {
            if (varModel == null)
            {
                return;
            }
            varModel.Destroy();
        }

        /// <summary>
        /// 创建ListenerControl实例
        /// </summary>
        /// <param name="varType"></param>
        /// <param name="varModel"></param>
        /// <returns></returns>
        public BaseListenerControl CreateListenerControl(Type varType, BaseModel varModel)
        {
            if (varType == typeof(LoginListenerControl))
            {
                return new LoginListenerControl(varModel as LoginModel);
            }
            else if (varType == typeof(AppListenerControl))
            {
                return new AppListenerControl(varModel as AppModel);
            }
            else
            {
                return null;
            }
        }
    }
}
