/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: ButtonSFXForNGUI.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/7/11 11:31:35
            // Modify History:
            //
//----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Air2000
{
    public class ButtonSFXForNGUI : MonoBehaviour
    {
        public UISoundEnum DefaultSound = UISoundEnum.Click;
        public UISoundEnum OverrideSound = UISoundEnum.None;
        void OnClick()
        {
            if (OverrideSound != UISoundEnum.None)
            {
                AudioAdapter.Instance.PlayUISound(OverrideSound);
            }
            else
            {
                AudioAdapter.Instance.PlayUISound(DefaultSound);
            }
        }
    }
}
