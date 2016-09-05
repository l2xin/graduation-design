/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: GTEditorTimer.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/3/30 18:29:51
            // Modify History:
            //
//----------------------------------------------------------------*/

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Air2000
{
    public class GTEditorTimer
    {
        public delegate void ClockDel();

        public delegate void ClockDelEx(object varParam);

        public class Clock
        {
            public GTEditorTimer Timer { get; set; }
            public float mCurrentTime;
            public float mClockTime;
            public ClockDel mTimeDel;
            public ClockDelEx mTimeDelEx;
            public object mUserData;
            public bool mIsRepeat;


            public Clock(float varClockTime, ClockDel varDel)
            {
                mCurrentTime = 0.0f;
                mClockTime = varClockTime;
                mTimeDel = varDel;
                mIsRepeat = true;
            }
            public Clock(float varClockTime, ClockDel varDel, bool isRepeat)
            {
                mCurrentTime = 0.0f;
                mClockTime = varClockTime;
                mTimeDel = varDel;
                mIsRepeat = isRepeat;
            }
            public Clock(float varClockTime, ClockDelEx varDel, object varParam)
            {
                mCurrentTime = 0.0f;
                mClockTime = varClockTime;
                mTimeDelEx = varDel;
                mUserData = varParam;
                mIsRepeat = true;
            }
            public Clock(float varClockTime, ClockDelEx varDel, object varParam, bool isRepeat)
            {
                mCurrentTime = 0.0f;
                mClockTime = varClockTime;
                mTimeDelEx = varDel;
                mUserData = varParam;
                mIsRepeat = isRepeat;
            }
            public bool ChangeTime(float varTime)
            {
                mCurrentTime += varTime;
                if (mCurrentTime > mClockTime)
                {
                    mCurrentTime -= mClockTime;
                    if (mTimeDel != null)
                    {
                        mTimeDel();
                    }
                    if (mTimeDelEx != null)
                    {
                        mTimeDelEx(mUserData);
                    }
                    if (mIsRepeat == false && Timer != null)
                    {
                        Timer.RemoveClock(this);
                    }
                    return true;
                }
                return false;
            }
        }


        private List<Clock> mRegisterClocks;
        private float LastTime;
        public GTEditorTimer()
        {
            mRegisterClocks = new List<Clock>();
            LastTime = Time.realtimeSinceStartup;
        }
        private void UpdateClockTime()
        {
            float currentTime = Time.realtimeSinceStartup;
            float deltaTime = currentTime - LastTime;
            if (deltaTime > 0.5)
            {
                LastTime = currentTime;
                return;
            }
            for (int i = 0; i < mRegisterClocks.Count; i++)
            {
                Clock Clock = mRegisterClocks[i];
                if (Clock == null)
                {
                    continue;
                }
                Clock.ChangeTime(deltaTime);
            }
            LastTime = currentTime;
        }
        #region define public method
        public void Update()
        {
            UpdateClockTime();
        }

        public bool AddClock(Clock clock)
        {
            if (clock == null)
            {
                return false;
            }
            clock.Timer = this;
            if (mRegisterClocks == null)
            {
                mRegisterClocks = new List<Clock>();
            }
            else
            {
                if (mRegisterClocks.Contains(clock))
                {
                    return false;
                }
            }
            mRegisterClocks.Add(clock);
            return true;
        }

        public bool RemoveClock(Clock clock)
        {
            if (clock == null)
            {
                return false;
            }
            if (mRegisterClocks == null || mRegisterClocks.Count == 0)
            {
                return false;
            }
            return mRegisterClocks.Remove(clock);
        }
        #endregion
    }
}
#endif