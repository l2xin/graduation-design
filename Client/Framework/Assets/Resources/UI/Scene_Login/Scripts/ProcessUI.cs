using UnityEngine;
using System.Collections;
using Air2000;
using GameLogic;

public class ProcessUI : MonoBehaviour
{
    public UISprite sliderProcess;
    public GameObject loading;
    #region UpdateCode
    public void Start()
    {
    }
    public void SetLodingShow(bool var)
    {
        if (loading == null)
        {
            return;
        }
        loading.SetActive(var);
    }
    public void OpenProcess()
    {
        UIHelper.SetActiveState(transform, "Process", true);
        InvokeRepeating("UpdateProcess", 0, 0.1f);
    }
    void UpdateProcess()
    {
        //double var = (UpdateVersionInfo.GetSigle().pDownloadProcess / UpdateVersionInfo.GetSigle().pTotalDwonloadSize) ;
        //string process = (var * 100).ToString("0");
        //if (UpdateVersionInfo.GetSigle().pDownloadProcess <= 0)
        //{
        //    process = "0";
        //}
        //string text = string.Format(Localization.Get("Update Text 3"), process);
        //Log.Info("DownloadProcess0-1：" + var.ToString());
        //Helper.SetLabelText(transform, "Process/processLabel", text);
        //if (sliderProcess != null)
        //{
        //    sliderProcess.fillAmount = (float)var;
        //}
        //if (UpdateVersionInfo.GetSigle().pIsDownLoadOver)
        //{
        //    WindowManager.GetSingleton().OpenTipDialog(UpdateVersionInfo.GetSigle().pStrWindowInfoTitle, UpdateVersionInfo.GetSigle().pStrWindowInfo, OpenTip);
        //    Helper.SetActiveState(transform, "Process", false);
        //    CancelInvoke("UpdateProcess");
        //}
        //float tempVar = (float)(UpdateVersionInfo.GetSigle().pTotalDwonloadSize / 1024) / 1024;
        //if (tempVar == 0)
        //{
        //    UIHelper.SetLabelText(transform, "Process/Total", string.Format(Localization.Get("Update Text 2"), tempVar.ToString("0")));
        //}
        //else
        //{
        //    UIHelper.SetLabelText(transform, "Process/Total", string.Format(Localization.Get("Update Text 2"), tempVar.ToString("0.00")));

        //}
    }

    private void OpenTip(DialogTip.OKYES keys)
    {
        if (keys == DialogTip.OKYES.Yes)
        {
            WindowManager.GetSingleton().CloseDialog(DialogTip.Path);
#if UNITY_ANDROID
            TSDKService.ConfirmInstallApkByTSDK();
#endif
        }
        else
        {
            Application.Quit();
        }
    }
    #endregion

}
