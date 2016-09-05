using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class OutLog : MonoBehaviour
{
    private static OutLog mInstance;


    static List<string> mLines = new List<string>();
    static List<string> mWriteTxt = new List<string>();
    private string outpath;

    private StreamWriter m_writer;

    private void Awake()
    {
        mInstance = this;
    }


    void OnDestroy()
    {
        if (m_writer != null)
        {

            m_writer.Close();
        }
    }

    void Start()
    {
        return;
        //Application.persistentDataPath Unity中只有这个路径是既可以读也可以写的。
        outpath = Application.persistentDataPath + "/outLog.txt";
        //每次启动客户端删除之前保存的Log
        if (System.IO.File.Exists(outpath))
        {
            //File.Delete(outpath);
        }
        //在这里做一个Log的监听
        //Application.RegisterLogCallback(HandleLog);
        //Application.logMessageReceived += HandleLog;
        //一个输出
        Log("111");


        //m_writer = new StreamWriter(outpath, true, Encoding.UTF8);
    }

    void Update()
    {
        return;
        if (m_writer == null)
        {
            Log("m_writer is null");
            return;
        }
        //因为写入文件的操作必须在主线程中完成，所以在Update中哦给你写入文件。
        if (mWriteTxt.Count > 0)
        {
            string[] temp = mWriteTxt.ToArray();
            foreach (string t in temp)
            {
                //   using (StreamWriter writer = new StreamWriter(outpath, true, Encoding.UTF8))
                {
                    m_writer.WriteLine(t);
                }
                //  mWriteTxt.Remove(t);
            }


        }


        mWriteTxt.Clear();


    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        mWriteTxt.Add(logString);
        if (type == LogType.Error || type == LogType.Exception)
        {
            Log(logString);
            Log(stackTrace);
        }
    }

    //这里我把错误的信息保存起来，用来输出在手机屏幕上
    static public void Log(params object[] objs)
    {
        if (mInstance == null)
        {
            return;
        }
        string text = System.DateTime.Now.ToString();
        for (int i = 0; i < objs.Length; ++i)
        {
            if (i == 0)
            {
                text += ":" + objs[i].ToString();
            }
            else
            {
                text += ", " + objs[i].ToString();
            }


        }
        Debug.Log(text);

        mWriteTxt.Add(text);

        //if (Application.isPlaying)
        //{
        //    if (mLines.Count > 20)
        //    {
        //        mLines.RemoveAt(0);
        //    }
        //    mLines.Add(text);

        //}
    }

    //void OnGUI()
    //{
    //    GUI.color = Color.red;
    //    for (int i = 0, imax = mLines.Count; i < imax; ++i)
    //    {
    //        GUILayout.Label(mLines[i]);
    //    }
    //}
}