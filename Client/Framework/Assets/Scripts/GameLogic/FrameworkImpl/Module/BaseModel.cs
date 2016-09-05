/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: BaseModel.cs
			// Describle: 模型
			// Created By:  hsu
			// Date&Time:  2016/3/3 19:27:20
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
    public class BaseModel : GTModel
    {
        public int pModelID
        {
            get { return mModelID; }
            set { mModelID = value; }
        }
        public ModelType pModelType
        {
            get { return (ModelType)mModelID; }
            set { mModelID = (int)value; }
        }
        public Player pSelfPlayer
        {
            get { return mSelfPlayer as Player; }
            set { mSelfPlayer = value; }
        }
        [Obsolete("Use 'pSelfPlayer' instead")]
        public Player mPlayer
        {
            get { return pSelfPlayer; }
            set { pSelfPlayer = value; }
        }
        public BaseView pView
        {
            get { return mView as BaseView; }
            set
            {
                mView = value;
                NotifyListener(IModelListenerEventType.OnViewAssigned, value);
            }
        }
        public BaseListenerControl pListenerControl
        {
            get { return mListenerControl as BaseListenerControl; }
            set { mListenerControl = value; }
        }
        public BaseModel(ModelType varModelType, Player varPlayer = null)
        {
            pModelType = varModelType;
            mSelfPlayer = varPlayer;
            OnModuleInit();
            NotifyListener(IModelListenerEventType.OnModelInited);
        }
        public BaseModel(ModelType varModelType, Type varTypeOfListenerControl, Player varPlayer = null)
        {
            pModelType = varModelType;
            mSelfPlayer = varPlayer;
            pListenerControl = ModelManager.GetSingleton().CreateListenerControl(varTypeOfListenerControl, this);
            OnModuleInit();
            NotifyListener(IModelListenerEventType.OnModelInited);
        }

        public BaseModel(ModelType varModelType, Type varTypeOfListenerControl, BaseView varView, Player varPlayer = null)
        {
            pModelType = varModelType;
            mView = varView;
            pListenerControl = ModelManager.GetSingleton().CreateListenerControl(varTypeOfListenerControl, this);
            OnModuleInit();
            NotifyListener(IModelListenerEventType.OnModelInited);
        }

        public override void Destroy()
        {
            base.Destroy();
        }
    }
}
