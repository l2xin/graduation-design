using UnityEngine;
using System.Collections;
using Air2000;
using Air2000.UI;
using GameLogic;

public class NetDialog : MonoBehaviour
{
#if ASSETBUNDLE_MODE
    public static string mDialogName = "NetDialog";
#else
    public static string mDialogName = "UI/Dialog/NetDialog";
#endif

    // Use this for initialization
    void Start()
    {
        //if (Game.GetCurrentScene().pStateName == FXQSceneType.SimpleScene.ToString())
        {
            Invoke("BrokenNetwork", 10f);
        }
    }

    void OnDestroy()
    {
        WindowManager.GetSingleton().CloseDialog(ConfirmDialog.mDialogName);
    }

    void BrokenNetwork()
    {
        transform.gameObject.SetActive(false);
        WindowManager.GetSingleton().OpenConfirmDialog("Title", Localization.Get("Network text 2"), Localization.Get("Network text 3"), Localization.Get("Network text 4"), Delegate, null, true);
    }

    void Delegate(DialogAction varAction, object varParam)
    {
        //重新登录;
        if (varAction == DialogAction.OK)
        {
            GameContext.GetInstance().GotoScene(SceneType.LoginScene);
            CloseDialog();

        }
        //继续重连;
        else if (varAction == DialogAction.Cancel)
        {
            Invoke("BrokenNetwork", 10f);
            transform.gameObject.SetActive(true);
        }
    }

    public void CloseDialog()
    {
        WindowManager.GetSingleton().CloseDialog(mDialogName);
    }
}



