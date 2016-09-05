using UnityEngine;
using System.Collections;
using Air2000;


public class DialogTip : MonoBehaviour
{
#if ASSETBUNDLE_MODE
    public static string Path = "DialogTips";
#else
    public static string Path = "UI/Dialog/DialogTips";
#endif
    public enum OKYES
    {
        Yes,
        No
    }
    public delegate void BtnCallback(OKYES keys);

    public string tipTitle;
    public string context;

    public BtnCallback mCall;
    void Start()
    {
        RegiterBtn(transform, "Yes", OnYes);
        RegiterBtn(transform, "No", OnNO);
        Helper.SetLabelText(transform, "Title", tipTitle);
        Helper.SetLabelText(transform, "context", context);
    }

    private void OnNO(GameObject go)
    {
        WindowManager.GetSingleton().CloseDialog(Path);
        mCall(OKYES.No);
    }

    private void OnYes(GameObject go)
    {
        WindowManager.GetSingleton().CloseDialog(Path);
        mCall(OKYES.Yes);
    }
    public void RegiterBtn(Transform obj, string path, UIEventListener.VoidDelegate voidDelegate)
    {
        if (!string.IsNullOrEmpty(path) && obj != null)
        {
            obj = obj.Find(path);
        }
        if (obj == null)
        {
            return;
        }
        UIEventListener.Get(obj.gameObject).onClick = voidDelegate;
    }
}
