/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: CharacterPoolController.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/6/11 15:29:23
            // Modify History:
            //
//----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Air2000.Character;
using Air2000;
using Air2000.Res;
namespace GameLogic
{
    public class CharacterPoolController : MonoBehaviour
    {
        public static CharacterPoolController Instance;
        public NewGameCharacter ShuaiShuai;
        public NewGameCharacter Joe;
        public NewGameCharacter LittleSister;
        public NewGameCharacter Mage;
        void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            ResContext.ListenInitializeFinish += OnResManagerInited;
        }
        void OnResManagerInited()
        {
            string shuaishuai_prefab_path;
#if ASSETBUNDLE_MODE
            shuaishuai_prefab_path="role/shuaishuai";
#else
            shuaishuai_prefab_path = "Model/Role/Boy_shuai/Shuaishuai(Merged)";
#endif
            ResContext.LoadAssetAsync(shuaishuai_prefab_path, "Shuaishuai(Merged)", typeof(GameObject), OnLoadShuaishuai);

            string Joe_prefab_path;
#if ASSETBUNDLE_MODE
            Joe_prefab_path="role/joe";
#else
            Joe_prefab_path = "Model/Role/Joe/Joe(Merged)";
#endif
            ResContext.LoadAssetAsync(Joe_prefab_path, "Joe(Merged)", typeof(GameObject), OnLoadJoe);

            string Mage_prefab_path = "role/mage";
#if ASSETBUNDLE_MODE
            Mage_prefab_path="role/mage";
#else
            Mage_prefab_path = "Model/Role/Mage/Mage(Merged)";
#endif
            ResContext.LoadAssetAsync(Mage_prefab_path, "Mage(Merged)", typeof(GameObject), OnLoadMage);

            string LittleSister_prefab_path = "role/littlesister";
#if ASSETBUNDLE_MODE
            LittleSister_prefab_path= "role/littlesister";
#else
            LittleSister_prefab_path = "Model/Role/Little_girl/LittleSister(Merged)";
#endif
            ResContext.LoadAssetAsync(LittleSister_prefab_path, "LittleSister(Merged)", typeof(GameObject), OnLoadLittleSister);
        }
        void Start()
        {
        }
        void OnLoadShuaishuai(UnityEngine.Object obj, ResourceLoadParam param)
        {
            if (obj == null) return;
            GameObject newObj = GameObject.Instantiate(obj) as GameObject;
            if (newObj == null) return;
            newObj.name = "Shuaishuai(Merged)";
            ShuaiShuai = newObj.GetComponent<NewGameCharacter>();
            newObj.SetActive(false);
            newObj.transform.SetParent(transform);
        }
        void OnLoadJoe(UnityEngine.Object obj, ResourceLoadParam param)
        {
            if (obj == null) return;
            GameObject newObj = GameObject.Instantiate(obj) as GameObject;
            if (newObj == null) return;
            newObj.name = "Joe(Merged)";
            Joe = newObj.GetComponent<NewGameCharacter>();
            newObj.SetActive(false);
            newObj.transform.SetParent(transform);
        }
        void OnLoadMage(UnityEngine.Object obj, ResourceLoadParam param)
        {
            if (obj == null) return;
            GameObject newObj = GameObject.Instantiate(obj) as GameObject;
            if (newObj == null) return;
            newObj.name = "Mage(Merged)";
            Mage = newObj.GetComponent<NewGameCharacter>();
            newObj.SetActive(false);
            newObj.transform.SetParent(transform);
        }
        void OnLoadLittleSister(UnityEngine.Object obj, ResourceLoadParam param)
        {
            if (obj == null) return;
            GameObject newObj = GameObject.Instantiate(obj) as GameObject;
            if (newObj == null) return;
            newObj.name = "LittleSister(Merged)";
            LittleSister = newObj.GetComponent<NewGameCharacter>();
            newObj.SetActive(false);
            newObj.transform.SetParent(transform);
        }
        void OnDestroy()
        {

        }
#if UNITY_EDITOR
        private NewGameCharacter CurrentCharacter;
        void OnGUI()
        {
            return;
            if (GUI.Button(new Rect(300, 10, 150, 30), "GetCharacter"))
            {
                CurrentCharacter = GetCharacter(Profession.ShuaiShuai, Color.red);
                CurrentCharacter.Active(false);
            }
            if (GUI.Button(new Rect(300, 50, 150, 30), "PoolCharacter"))
            {
                Pool(CurrentCharacter);
            }
            if (GUI.Button(new Rect(300, 90, 150, 30), "Character Appear"))
            {
                CurrentCharacter.Appear(new Vector3(1.2f, 0.6f, 0.5f), Air2000.Character.CharacterCommand.CC_Appear);
            }
        }
#endif
        public static NewGameCharacter Get(string config)
        {
            return null;
            //return GetCharacter("", Color.black);
        }
        public static NewGameCharacter Get(string config, Color color)
        {
            return null;
            //return GetCharacter(NewGameCharacter.StrToProfession(config), color);
        }
        public static NewGameCharacter GetCharacter(Profession prefession, Color color)
        {
            NewGameCharacter ch = null;
            switch (prefession)
            {
                case Profession.ShuaiShuai:
                    ch = Instance.ShuaiShuai;
                    break;
                case Profession.Joe:
                    ch = Instance.Joe;
                    break;
                case Profession.LittleSister:
                    ch = Instance.LittleSister;
                    break;
                case Profession.Mage:
                    ch = Instance.Mage;
                    break;
                default:
                    break;
            }
            if (ch == null) return null;
            GameObject newChObj = ObjPoolController.Instantiate(ch.gameObject);
            if (newChObj == null) return null;
            ch = newChObj.GetComponent<NewGameCharacter>();
            if (ch == null) return null;
            ch.Init(color);
            return ch;
        }
        public static bool GonnaQuit = false;
        void OnApplicationQuit()
        {
            GonnaQuit = true;
        }
        public static void Pool(GameObject obj)
        {
            if (GonnaQuit) return;
            if (obj != null)
            {
                NewGameCharacter character = obj.GetComponent<NewGameCharacter>();
                if (character != null)
                {
                    character.CleanUp();
                }
                ObjPoolController.DestroyImmediate(obj);
            }
        }
        public static void Pool(NewGameCharacter character)
        {
            if (GonnaQuit) return;
            if (character != null)
            {
                character.CleanUp();
                ObjPoolController.DestroyImmediate(character.gameObject);
            }
        }
    }
}
