/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: EffectRoot.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/6/6 20:42:04
            // Modify History:
            //
//----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Air2000.Character
{
    [AddComponentMenu("GTools/Character/EffectRoot")]
    public class EffectRoot : MonoBehaviour
    {
        [HideInInspector]
        public string AssetBundle;
        [HideInInspector]
        public string AssetName;
        [HideInInspector]
        public Vector3 Position = Vector3.zero;
        [HideInInspector]
        public Vector3 EulerAngles = Vector3.zero;
        [HideInInspector]
        public Vector3 LocalScale = Vector3.one;
        [HideInInspector]
        public bool CacheImmediate = true;
        [HideInInspector]
        public bool ActiveOnAwake = false;
        [HideInInspector]
        public bool DontFollowParent = false;
        [HideInInspector]
        public Transform Parent;
        [HideInInspector]
        public GameObject Effect;
        [NonSerialized]
        public Character Character;
        void Awake()
        {
            if (Effect != null)
            {
                ObjPoolController.Destroy(Effect);
                Effect = null;
            }
            //if (Effect == null)
            //{
            //    GameObject obj = GameObject.Find(AssetBundle);
            //    if (obj != null)
            //    {
            //        Effect = ObjPoolController.Instantiate(obj);
            //        if (Effect != null)
            //        {
            //            Effect.transform.SetParent(transform);
            //            if (ActiveOnAwake)
            //            {
            //                Effect.transform.SetParent(transform);
            //                Effect.transform.localPosition = Position;
            //                Effect.transform.localEulerAngles = EulerAngles;
            //                Effect.transform.localScale = LocalScale;
            //                Effect.SetActive(true);
            //            }
            //            else
            //            {
            //                Effect.SetActive(false);
            //            }
            //        }
            //    }
            //}
            if (Character == null)
            {
                Character[] chs = GetComponentsInParent<Character>(true);
                if (chs != null && chs.Length > 0)
                {
                    Character = chs[0];
                }
            }
            if (ActiveOnAwake)
            {
                PlayEffect();
            }
        }
        bool GonnaQuit = false;
        void OnApplicationQuit()
        {
            GonnaQuit = true;
        }
        void OnDestroy()
        {
            if (Application.isPlaying == false) return;
            try
            {
                if (GonnaQuit == false)
                {
                    DestroyEffect();
                }
            }
            catch { }
            Character = null;
        }
        public GameObject GetEffect()
        {
            if (Effect == null)
            {
                //GameObject obj = CharacterEffectPackageController.Get(AssetBundle);
                //if (obj != null)
                //{
                //    Effect = ObjPoolController.Instantiate(obj);
                //}
            }
            return Effect;
        }
        public void DestroyEffect()
        {
            //CharacterEffectPackageController.Pool(Effect);
            Effect = null;
        }
        public bool PlayEffect()
        {
            if (Character == null) return false;
            if (GetEffect() == null)
            {
                return false;
            }
            if (DontFollowParent)
            {
                Effect.transform.SetParent(Parent);
                if (Parent == null)
                {
                    Vector3 newPos = Character.transform.TransformPoint(Position);
                    Effect.transform.position = newPos;
                    Vector3 newEulerAngles = Character.transform.TransformDirection(EulerAngles);
                    Effect.transform.eulerAngles = newEulerAngles;
                    Effect.transform.localScale = LocalScale;
                }
                else
                {
                    Effect.transform.localPosition = Position;
                    Effect.transform.localEulerAngles = EulerAngles;
                    Effect.transform.localScale = LocalScale;
                }
            }
            else
            {
                Effect.transform.SetParent(transform);
                Effect.transform.localPosition = Vector3.zero;
                Effect.transform.localEulerAngles = Vector3.zero;
                Effect.transform.localScale = Vector3.one;
            }
            Effect.SetActive(true);
            return true;
        }
        public bool StopEffect()
        {
            if (Character == null || Effect == null) return false;
            if (Effect.transform.parent != transform)
            {
                Effect.transform.SetParent(transform);
            }
            Effect.SetActive(false);
            if (CacheImmediate)
            {
                DestroyEffect();
            }
            return true;
        }
    }
}
