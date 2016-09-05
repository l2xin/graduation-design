using UnityEngine;
using System.Collections;
using Air2000;

namespace Air2000.UI
{
    /// <summary>
    /// 二次确认操作的枚举.
    /// </summary>
    public enum DialogAction
    {
        OK,
        Cancel,
    }
    /// <summary>
    /// 用户二次确认操作的代理.
    /// </summary>
    public delegate void DialogActionDelegate(DialogAction varAction, object varParam);

    public class ConfirmDialog : MonoBehaviour
    {
        DialogActionDelegate mActionDel;
        object mParam;
#if ASSETBUNDLE_MODE
        public static string mDialogName = "ConfirmDialog";
#else
        public static string mDialogName = "UI/Dialog/ConfirmDialog";
#endif
        //防止多次点击;
        bool IsClick;
        void Start()
        {
            InitButtonEvent();
        }
        /// <summary>
        /// 设置弹框信息.
        /// </summary>
        /// <param name="varContents">显示的内容.</param>
        /// /// <param name="varBtnName">确认按钮的名字.</param>
        /// <param name="varCallBack">用户操作的代理.</param>
        public void ShowDiaogInfo(string varTitle, string varContents, string varBtnOKName, string varBtnNameCancel, DialogActionDelegate varCallBack, object varParam, bool varIsShowCancle)
        {
            IsClick = false;
            mActionDel = varCallBack;
            mParam = varParam;
            Helper.SetLabelText(transform, "Title", varTitle);
            Helper.SetLabelText(transform, "Label", varContents);
            Helper.SetLabelText(transform, "OK/Label", varBtnOKName);
            Helper.SetLabelText(transform, "Cancel/Label", varBtnNameCancel);

            Helper.SetActiveState(transform, "Cancel", varIsShowCancle);

            if (varIsShowCancle == false)
            {
                Transform tempTra = this.transform.Find("OK");
                if (tempTra != null)
                {
                    tempTra.localPosition = new Vector3(0, tempTra.localPosition.y, tempTra.localPosition.z);
                }
            }
        }
        /// <summary>
        /// 初始化按钮事件响应.
        /// </summary>
        void InitButtonEvent()
        {
            Transform tempTran = transform.Find(DialogAction.OK.ToString());
            if (tempTran != null)
            {
                UIEventListener.Get(tempTran.gameObject).onClick = ButtonSureOnClicked;
            }
            tempTran = transform.Find(DialogAction.Cancel.ToString());
            if (tempTran != null)
            {
                UIEventListener.Get(tempTran.gameObject).onClick = ButtonCancelOnClicked;
            }
        }
        /// <summary>
        /// 确定按钮点击.
        /// </summary>
        /// <param name="varObj">Variable object.</param>
        void ButtonSureOnClicked(GameObject varObj)
        {
            if (IsClick)
            {
                return;
            }
            IsClick = true;

            if (mActionDel != null)
            {
                mActionDel(DialogAction.OK, mParam);
            }
            CloseDialog();
        }
        /// <summary>
        /// 取消按钮点击.
        /// </summary>
        /// <param name="varObj">Variable object.</param>
        void ButtonCancelOnClicked(GameObject varObj)
        {
            if (IsClick)
            {
                return;
            }
            IsClick = true;

            if (mActionDel != null)
            {
                mActionDel(DialogAction.Cancel, mParam);
            }
            CloseDialog();
        }
        /// <summary>
        /// 关闭窗口.
        /// </summary>
        public void CloseDialog()
        {
            WindowAnimation Win = UIHelper.GetComponent<WindowAnimation>(transform.parent);
            if (Win != null)
            {
                Win.ColseWindow(CloseWindow);
            }

        }

        void CloseWindow()
        {
            WindowManager.GetSingleton().CloseDialog(mDialogName);

        }



    }
}


