using UnityEngine;
using System.Collections;
using Air2000;

namespace Air2000.UI
{
    public class BattleSmallDialog : MonoBehaviour
    {
        /// <summary>
        /// 存放需要显示文字信息;.
        /// </summary>
        private string mContents = "";

        public float mTime = 1f;

        // Use this for initialization
        void Start()
        {
            ShowDialogContents();
            UIHelper.SetActiveState(transform, "Parent", true);

            Invoke("CloseDialog", mTime);
        }

        // Update is called once per frame
        void Update()
        {

        }

        /// <summary>
        /// 设置要显示的文字;.
        /// </summary>
        /// <param name="varContents">Variable contents.</param>
        public void SetContents(string varContents)
        {
            mContents = varContents;
        }
        void ShowDialogContents()
        {
            Helper.SetLabelText(transform, "Parent/Label", mContents);
        }

        /// <summary>
        /// 关闭窗口.
        /// </summary>
        void CloseDialog()
        {
#if ASSETBUNDLE_MODE
            WindowManager.GetSingleton().CloseDialog("BattleSmallDialog");
#else
            WindowManager.GetSingleton().CloseDialog("UI/Dialog/BattleSmallDialog");
#endif
        }
    }
}

