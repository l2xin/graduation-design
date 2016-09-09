/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: GTModelManager.cs
			// Describle: 模块管理
			// Created By:  hsu
			// Date&Time:  2016/3/3 19:02:56
            // Modify History:
            //
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Air2000
{
    public abstract class GTModelManager
    {
        protected Dictionary<int, GTModel> mModels;
        protected GTPlayer mSelfPlayer;//玩家自身
        protected List<IModelManagerListener> mListeners;

        public GTModelManager()
        {
            mModels = new Dictionary<int, GTModel>();
            mListeners = new List<IModelManagerListener>();

        }
        public virtual void InitModel() { }

        public GTModel GetModelByID(int varModelID)
        {
            GTModel model = null;
            mModels.TryGetValue(varModelID, out model);
            return model;
        }

        public void AddModel(GTModel model)
        {
            return;
        }

        public void AddListener(IModelManagerListener varListener)
        {
            if (varListener == null)
            {
                return;
            }
            if (mListeners.Contains(varListener))
            {
                return;
            }
            mListeners.Add(varListener);
        }
        public void RemoveListener(IModelManagerListener varListener)
        {
            if (varListener == null)
            {
                return;
            }
            mListeners.Contains(varListener);
        }
        public void RemoveAllListener()
        {
            mListeners.Clear();
        }
    }
}
