/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: ResLoadExample.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/7/4 14:48:37
            // Modify History:
            //
//----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GTools.Res
{
    public class ResLoadExample : MonoBehaviour
    {
        void Start()
        {
            ResourceManager.ListenInitializeFinish += OnResManagerInitFinish;

            ResourceManager.Initialize();
        }
        void OnResManagerInitFinish()
        {

        }
        void OnLoadedShuaiShuai(UnityEngine.Object obj, ResourceLoadParam param)
        {
            GameObject.Instantiate(obj);
            int a = 1;
        }
        void OnLoadedJoe(UnityEngine.Object obj, ResourceLoadParam param)
        {
            GameObject.Instantiate(obj);
            int a = 1;
        }
        void OnLoadedLittleSister(UnityEngine.Object obj, ResourceLoadParam param)
        {
            GameObject.Instantiate(obj);
            int a = 1;
        }
        void OnLoadedMage(UnityEngine.Object obj, ResourceLoadParam param)
        {
            GameObject.Instantiate(obj);
            int a = 1;
        }
        void OnGUI()
        {
            if (GUI.Button(new Rect(10, 10, 150, 30), "Generate exception"))
            {

            }
            if (GUI.Button(new Rect(10, 50, 150, 30), "Load Shuaishuai"))
            {
                ResourceManager.LoadAssetAsync("role/shuaishuai", "ShuaiShuai(Merged)", typeof(GameObject), OnLoadedShuaiShuai);
            }
            if (GUI.Button(new Rect(10, 90, 150, 30), "Load Joe"))
            {
                ResourceManager.LoadAssetAsync("role/joe", "Joe(Merged)", typeof(GameObject), OnLoadedJoe);
            }
            if (GUI.Button(new Rect(10, 130, 150, 30), "Load LittleGirl"))
            {
                ResourceManager.LoadAssetAsync("role/littlesister", "LittleSister(Merged)", typeof(GameObject), OnLoadedLittleSister);
            }
            if (GUI.Button(new Rect(10, 170, 150, 30), "Load Mage"))
            {
                ResourceManager.LoadAssetAsync("role/mage", "Mage(Merged)", typeof(GameObject), OnLoadedMage);
            }
            if (GUI.Button(new Rect(10, 210, 150, 30), "Open Login"))
            {
                ResourceManager.LoadAssetAsync("ui/login", "Login", typeof(GameObject), OnLoadedMage);
            }
            if (GUI.Button(new Rect(10, 250, 150, 30), " "))
            {

            }
            if (GUI.Button(new Rect(10, 290, 150, 30), " "))
            {

            }
        }
    }
}
