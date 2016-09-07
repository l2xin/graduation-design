using UnityEngine;
using System.Collections;
using System.IO;
using ImageHelper;
using System;

namespace Air2000
{
    /// <summary>
    /// UI辅助类.
    /// </summary>
    public class UIHelper
    {
        /// <summary>
        /// 设置按钮委托事件.
        /// </summary>
        /// <param name="varBtn"></param>
        /// <param name="varDelegate"></param>
        public static void SetButtonEvent(Transform varBtn, UIEventListener.VoidDelegate varDelegate)
        {
            if (varBtn != null)
            {
                UIEventListener.Get(varBtn.gameObject).onClick = varDelegate;
            }
        }

        /// <summary>
        /// 设置按钮委托事件.
        /// </summary>
        /// <param name="varParent"></param>
        /// <param name="varBtnPath"></param>
        /// <param name="varDelegate"></param>
        public static void SetButtonEvent(Transform varParent, string varBtnPath, UIEventListener.VoidDelegate varDelegate)
        {
            if (varParent != null)
            {
                Transform tempBtn = varParent.Find(varBtnPath);
                if (tempBtn == null)
                {
#if UNITY_EDITOR

                    Debug.LogError("!!ERROR SetButtonEvent Not attach Delegate Named : " + varDelegate.Method.Name);
#endif
                    return;
                }
                SetButtonEvent(tempBtn, varDelegate);
            }
        }

        /// <summary>
        /// 设置某Label的内容.
        /// </summary>
        /// <param name="varParent"></param>
        /// <param name="varPath"></param>
        /// <param name="varContent"></param>
        /// <returns></returns>
        public static UILabel SetLabelText(Transform varParent, string varPath, string varContent)
        {
            UILabel tempLabel = GetComponent<UILabel>(varParent, varPath);
            if (tempLabel == null)
            {
                return null;
            }

            tempLabel.text = varContent;

            return tempLabel;
        }

        /// <summary>
        /// 设置某Label的内容.
        /// </summary>
        /// <param name="varTran"></param>
        /// <param name="varContent"></param>
        /// <returns></returns>
        public static UILabel SetLabelText(Transform varTran, string varContent)
        {
            if (varTran == null)
            {
                return null;
            }
            UILabel tempLabel = varTran.GetComponent<UILabel>();
            if (tempLabel != null)
            {
                tempLabel.text = varContent;
                return tempLabel;
            }
            return null;
        }

        public static SpringPosition ResetSpringPosition(GameObject varObj, Vector3 varVec, float varStrength)
        {
            if (varObj == null)
            {
                return null;
            }

            SpringPosition tempSp = varObj.GetComponent<SpringPosition>();
            if (tempSp != null)
            {
                GameObject.Destroy(tempSp);
            }
            tempSp = varObj.AddComponent<SpringPosition>();

            tempSp.target = varVec;
            tempSp.strength = varStrength;
            tempSp.onFinished = null;

            return tempSp;
        }

        /// <summary>
        /// 设置某个对象的激活状态.
        /// </summary>
        /// <param name="varParent"></param>
        /// <param name="varState"></param>
        /// <returns></returns>
        public static bool SetActiveState(Transform varParent, bool varState)
        {
            if (varParent == null)
            {
                return false;
            }
            varParent.gameObject.SetActive(varState);
            return true;
        }

        /// <summary>
        /// 设置某个对象的激活状态.
        /// </summary>
        /// <param name="varParent"></param>
        /// <param name="varPath"></param>
        /// <param name="varState"></param>
        /// <returns></returns>
        public static bool SetActiveState(Transform varParent, string varPath, bool varState)
        {
            if (varParent == null)
            {
                return false;
            }
            Transform find = varParent.Find(varPath);
            if (find == null)
            {
                return false;
            }
            find.gameObject.SetActive(varState);
            return true;
        }

        /// <summary>
        /// 设置某个Sprite对象的图片名字.
        /// </summary>
        /// <param name="varParent"></param>
        /// <param name="varPath"></param>
        /// <param name="varName"></param>
        /// <returns></returns>
        public static UISprite SetSpriteName(Transform varParent, string varPath, string varName)
        {
            if (varParent == null)
            {
                return null;
            }
            Transform tempTran = varParent.Find(varPath);
            return SetSpriteName(tempTran, varName);
        }
        /// <summary>
        /// 设置某个Sprite对象的图片名字.
        /// </summary>
        /// <param name="varTran"></param>
        /// <param name="varName"></param>
        /// <returns></returns>
        public static UISprite SetSpriteName(Transform varTran, string varName)
        {
            if (varTran == null)
            {
                return null;
            }
            UISprite temp = varTran.GetComponent<UISprite>();
            if (temp != null)
            {
                temp.spriteName = varName;
                temp.MakePixelPerfect();
                return temp;
            }
            return null;
        }

        public static UISprite SetSpriteSize(Transform varParent, string varPath, int varX, int varY = -1)
        {
            if (varParent == null)
            {
                return null;
            }
            Transform tempTran = varParent.Find(varPath);
            return SetSpriteSize(tempTran, varX, varY);
        }

        public static UISprite SetSpriteSize(Transform varTran, int varX, int varY = -1)
        {
            if (varTran == null)
            {
                return null;
            }
            UISprite temp = varTran.GetComponent<UISprite>();
            if (temp != null)
            {
                if (varX == 0)
                {
                    varTran.gameObject.SetActive(false);
                }

                temp.width = varX;
                if (varY != -1)
                {
                    temp.width = varY;
                }
                return temp;
            }
            return null;
        }

        public static UISprite SetSpriteAlpha(Transform varTran, string varPath, int varAlpha)
        {
            if (varTran == null)
            {
                return null;
            }
            Transform tempTran = varTran.Find(varPath);
            return SetSpriteAlpha(tempTran, varAlpha);
        }

        public static UISprite SetSpriteAlpha(Transform varTran, int varAlpha)
        {
            if (varTran == null)
            {
                return null;
            }
            UISprite temp = varTran.GetComponent<UISprite>();
            if (temp != null)
            {
                temp.alpha = (float)varAlpha / 255;
                return temp;
            }
            return null;
        }

        public static UILabel SetLabelAlpha(Transform varTran, string varPath, int varAlpha)
        {
            if (varTran == null)
            {
                return null;
            }
            Transform tempTran = varTran.Find(varPath);
            return SetLabelAlpha(tempTran, varAlpha);
        }

        public static UILabel SetLabelAlpha(Transform varTran, int varAlpha)
        {
            if (varTran == null)
            {
                return null;
            }
            UILabel temp = varTran.GetComponent<UILabel>();
            if (temp != null)
            {
                temp.alpha = (float)varAlpha / 255;
                return temp;
            }
            return null;
        }



        /// <summary>
        /// 获得指定路径对应的组件.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="varTrans"></param>
        /// <param name="varPath"></param>
        /// <returns></returns>
        public static T GetComponent<T>(Transform varTrans, string varPath) where T : Component
        {
            if (varTrans == null)
            {
                return null;
            }

            Transform tempTrans = varTrans.Find(varPath);
            if (tempTrans == null)
            {
                return null;
            }

            return tempTrans.GetComponent<T>();
        }

        /// <summary>
        /// 获得对应的组件.
        /// </summary>
        /// <returns>The component.</returns>
        /// <param name="varTrans">Variable trans.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T GetComponent<T>(Transform varTrans) where T : Component
        {
            if (varTrans == null)
            {
                return null;
            }
            return varTrans.GetComponent<T>();
        }

        public static void SetTextureToUrl(Transform varTrans, string varPath, string Url)
        {
            if (varTrans != null)
            {
                Transform tempTra = varTrans.Find(varPath);

                SetTextureToUrl(tempTra, Url);
            }

        }

        public static void SetTextureToUrl(Transform varTrans, string url)
        {
            if (varTrans != null)
            {
                UITexture tempUITexture = varTrans.GetComponent<UITexture>();
                GameLogic.GameContext.GetInstance().StartCoroutine(SetTextureToUrl(url, tempUITexture));
            }
        }




        /// <summary>
        /// 设置UITexture.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="UITexture"></param>
        /// <returns></returns>
        public static IEnumerator SetTextureToUrl(string url, UITexture varTexture)
        {
            using (WWW www = new WWW(url))
            {
                //挂起程序段，等资源下载完成后，继续执行下去;
                yield return www;
                if (string.IsNullOrEmpty(www.error))
                {
                    if (varTexture != null && www.texture != null)
                    {
                        GifReader.GetImage(varTexture, www.bytes, string.Empty);
                    }
                }

                www.Dispose();
            }
        }

        //public static IEnumerator SetTextureToUrl(string url, UITexture varUITexture)
        //{
        //    if (string.IsNullOrEmpty(url)) yield break;
        //    if (varUITexture == null) yield break;
        //    WWW www = new WWW(url);
        //    yield return www;
        //    if (string.IsNullOrEmpty(www.error))
        //    {
        //        string contentType = www.responseHeaders["CONTENT-TYPE"];
        //        var contents = contentType.Split('/');
        //        if (contents[0] == "image")
        //        {
        //            if (contents[1].ToLower() == "png" || contents[1].ToLower() == "jpeg")
        //            {
        //                varUITexture.mainTexture = www.textureNonReadable;
        //            }
        //            else
        //            {
        //                //写到文件中
        //                string fileName = Helper.GenerateId();
        //                string directory = Application.persistentDataPath + "/Resources/";
        //                if (!Directory.Exists(directory))
        //                {
        //                    Directory.CreateDirectory(directory);
        //                }
        //                string filePath = directory + fileName + "." + contents[1];
        //                File.WriteAllBytes(filePath, www.bytes);
        //                //从文件加载
        //                Texture2D t = Resources.Load<Texture2D>(fileName);
        //                varUITexture.mainTexture = t;
        //            }
        //        }
        //    }
        //    www.Dispose();
        //}


        public static void SetTextureToUrl(Transform varTrans, string varPath1, string varPath2, string Url)
        {
            if (varTrans != null)
            {
                Transform tempTra1 = varTrans.Find(varPath1);
                Transform tempTra2 = varTrans.Find(varPath2);
                if (tempTra1 != null && tempTra2 != null)
                {
                    UITexture tempTexture1 = tempTra1.GetComponent<UITexture>();
                    UniGifTexture tempTexture2 = tempTra2.GetComponent<UniGifTexture>();
                    if (tempTra1 != null && tempTra2 != null)
                    {
                        GameLogic.GameContext.GetInstance().StartCoroutine(SetTextureToUrl(Url, tempTexture1, tempTexture2));
                    }
                }

            }

        }



        /// <summary>
        /// 设置UITexture.
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="varTexture"></param>
        /// <returns></returns>
        public static IEnumerator SetTextureToUrl(string Url, UITexture varTexture, UniGifTexture varTextureRJB)
        {
            Helper.Log(" Url = http://q.qlogo.cn/qqapp/1104827980/E866B8F7799C8F036AE3E85584B809A7/100 ");

            using (WWW www = new WWW(Url))
            {
                //挂起程序段，等资源下载完成后，继续执行下去;
                yield return www;

                if (string.IsNullOrEmpty(www.error))
                {

                    if (www.bytes[0] != 'G' || www.bytes[1] != 'I' || www.bytes[2] != 'F')
                    {
                        if (varTexture != null)
                        {
                            varTexture.mainTexture = www.texture;
                        }
                    }
                    else
                    {
                        //if (varTextureRJB != null)
                        //{
                        //    yield return FXQ.FXQMainService.GetSingleton().StartCoroutine(varTextureRJB.SetGifFromUrlCoroutine(Url));
                        //}
                    }

                }
                else
                {
                    Helper.Log("ZQ www.error = " + www.error);
                }

                www.Dispose();
            }
        }


        /// <summary>
        /// 获得字符串.
        /// </summary>
        /// <param name="varByte"></param>
        /// <returns></returns>
        public static string GetString(byte[] varByte)
        {
            try
            {
                return System.Text.Encoding.UTF8.GetString(varByte).TrimEnd('\0');
            }
            catch (System.Exception e)
            {
                return string.Empty;
                Helper.Log("UIHelper.cs GetString Error varByte : " + varByte + " System.Exception : " + e.Message);
                throw;
            }
        }

        /// <summary>
        /// 获得带符号的数字字符串.
        /// </summary>
        /// <param name="varNumber"></param>
        /// <returns></returns>
        public static string GetNumberString(int varNumber)
        {
            if (varNumber > 0)
            {
                return "+" + varNumber.ToString();
            }

            return varNumber.ToString();
        }

        public static string GetNumberString(long varNumber)
        {
            if (varNumber > 0)
            {
                return "+" + varNumber.ToString();
            }

            return varNumber.ToString();
        }

        /// <summary>
        /// 获取字符串中指定字符后面的字符串,并转成整型返回.
        /// </summary>
        /// <param name="varName"></param>
        /// <param name="varSpilt"></param>
        /// <returns></returns>
        public static int GetCutNumber(string varName, char varSpilt)
        {
            int tempNum = -1;

            int tempIndex = varName.IndexOf(varSpilt) + 1;

            string tempsub = varName.Substring(tempIndex, varName.Length - tempIndex);

            int.TryParse(tempsub, out tempNum);

            return tempNum;
        }


        /// <summary>
        /// 获取金额（千位分隔）字符.
        /// </summary>
        /// <param name="varNumber"></param>
        /// <returns></returns>
        public static string GetSeparatorNumber(int varNumber)
        {
            if (varNumber > 0)
            {
                return GetSeparatorNumber(varNumber.ToString());
            }
            else
            {
                varNumber = System.Math.Abs(varNumber);

                return "-" + GetSeparatorNumber(varNumber.ToString());
            }
        }
        /// 获取金额（千位分隔）字符.
        public static string GetSeparatorNumber(string varNumberStr)
        {
            int tempLength = varNumberStr.Length;

            int temp = 3;

            while (tempLength > temp)
            {
                tempLength -= temp;

                varNumberStr = varNumberStr.Insert(tempLength, ",");
            }

            return varNumberStr;
        }


        /// <summary>  
        /// 截取字节数组  
        /// </summary>  
        /// <param name="srcBytes">要截取的字节数组</param>  
        /// <param name="startIndex">开始截取位置的索引</param>  
        /// <param name="length">要截取的字节长度</param>  
        /// <returns>截取后的字节数组</returns>  
        public static byte[] SubByte(byte[] srcBytes, int startIndex, int length)
        {
            System.IO.MemoryStream bufferStream = new System.IO.MemoryStream();
            byte[] returnByte = new byte[] { };
            if (srcBytes == null) { return returnByte; }
            if (startIndex < 0) { startIndex = 0; }
            if (startIndex < srcBytes.Length)
            {
                if (length < 1 || length > srcBytes.Length - startIndex) { length = srcBytes.Length - startIndex; }
                bufferStream.Write(srcBytes, startIndex, length);
                returnByte = bufferStream.ToArray();
                bufferStream.SetLength(0);
                bufferStream.Position = 0;
            }
            bufferStream.Close();
            bufferStream.Dispose();
            return returnByte;
        }

        /// <summary>
        /// 头像的截取借口，（显示4个字节，超过的显示。。。）
        /// </summary>
        /// <param name="varNickName"></param>
        /// <returns></returns>
        public static string GetPlayerName(string varNickName)
        {
            if (String.IsNullOrEmpty(varNickName) == false && varNickName.Length > 4)
            {
                varNickName = varNickName.Substring(0, 4) + "...";
            }

            return varNickName;
        }



        //把总秒数转成 倒计时00:00格式;
        public static string GetTextBySeconds(int seconds)
        {
            int minutes = seconds / 60;
            int second = seconds - minutes * 60;

            string str_time = string.Empty;

            if (minutes < 10)
            {
                str_time += ("0" + minutes + ":");
            }
            else
            {
                str_time += (minutes + ":");
            }
            if (second < 10)
            {
                str_time += ("0" + second);
            }
            else
            {
                str_time += (second);
            }

            return str_time;
        }



        public static bool SetTraPosition(Transform varTra, Vector3 varVec3)
        {
            if (varTra == null)
            {
                return false;
            }
            varTra.localPosition = varVec3;
            return true;
        }

        public static bool SetTraPosition(Transform varTra, string varPath, Vector3 varVec3)
        {
            if (varTra == null)
            {
                return false;
            }

            Transform tempTra = varTra.Find(varPath);
            if (SetTraPosition(tempTra,varVec3))
            {
                return true;
            }
            return false;
        }

    }
}
