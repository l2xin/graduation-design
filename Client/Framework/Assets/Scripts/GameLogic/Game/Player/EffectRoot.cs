/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: EffectRoot.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/5/26 18:45:46
            // Modify History:
            //
//----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GameLogic
{
    public class EffectRoot : MonoBehaviour
    {
        public string PrefabRootPath;
        public string EffectKey;

        public Vector3 Position = Vector3.zero;
        public Vector3 EulerAngles = Vector3.zero;
        public Vector3 LocalScale = Vector3.one;
        public bool ActiveOnAwake = false;

        public GameObject Effect;
        void Awake()
        {
            if (Effect == null)
            {
                GameObject obj = GameObject.Find(PrefabRootPath);
                if (obj != null)
                {
                    Effect = ObjectPoolController.Instantiate(obj);
                    if (Effect != null)
                    {
                        Effect.transform.SetParent(transform);
                        Effect.transform.localPosition = Position;
                        Effect.transform.localEulerAngles = EulerAngles;
                        Effect.transform.localScale = LocalScale;
                        if (ActiveOnAwake)
                        {
                            Effect.SetActive(true);
                        }
                        else
                        {
                            Effect.SetActive(false);
                        }
                    }
                }
            }
        }

        void OnDestroy()
        {
            try
            {
               // ObjectPoolController.Destroy(Effect);
            }
            catch { }
        }
    }
}
