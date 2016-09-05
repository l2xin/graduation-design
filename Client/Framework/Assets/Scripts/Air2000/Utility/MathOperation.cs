using System;
using System.Text.RegularExpressions;

namespace Air2000
{
	public class MathOperation 
	{
		// 对浮点数四舍五入;
		public static float Round(float f, int decimals)
		{
			if(f < 0 || decimals < 0)  return f;
			int v = 1;
			for(int i = 0; i <= decimals; i++)
			{
				v *= 10;
			}
			int t = (int)((f * v + 5) / 10);
			v /= 10;
			f = (float)t / v;
			return f;
		}
		
		// 计算某年某月有多少天;
		public static int CalculateDay(int y, int m) 
		{
			// 日期格式有误;
			if (y <= 0 || m <= 0 || m > 12) return -1;
			int day;
			switch (m) 
			{
			case 1:
			case 3:
			case 5:
			case 7:
			case 8:
			case 10:
			case 12:
				day = 31;
				break;
			case 2:
			{
				// 是否是闰年;
				if ((y % 4 == 0 && y % 100 != 0) || y % 400 == 0)
				{
					day = 29;
				}
				else 
				{
					day = 28;
				}
			}
				break;
			default:
				day = 30;
				break;
			}
			return day;
		}
		
		// 计算某年某月某日是星期几（使用基姆拉尔森公式）;
		public static int CalculateWeekDay(int y, int m, int d) 
		{
			if (m == 1 || m == 2) 
			{
				m += 12;
				y--;  // 把一月和二月看成是上一年的十三月和十四月，例：如果是2004-1-10则换算成：2003-13-10来代入公式计算;
			}
			int week = (d + 2 * m + 3 * (m + 1) / 5 + y + y / 4 - y / 100 + y / 400) % 7;
			/*
         * week == 0, 星期一;
         * week == 1, 星期二;
         * week == 2, 星期三;
         * week == 3, 星期四;
         * week == 4, 星期五;
         * week == 5, 星期六;
         * week == 6, 星期日;
        */
			return week;
		}
		
		// 计算时间如小时，分钟，和秒
		public static string CalculateTime(int t)
		{
			if (t < 10)
			{
				return t.ToString().Insert(0, "0");
			}
			else
			{
				return t.ToString();
			}
		}
		
		/* 将秒数转换为时间;
	     * 时间格式为 天,时:分:秒
	     */
		public static TimeSpan SecondToTime(int seconds)
		{
			if(seconds <= 0) return TimeSpan.Zero;
			TimeSpan ts = new TimeSpan(0, 0 , 0, seconds);
			return ts;
		}
		
		// 计算结束时间与当前时间的间隔;
		public static TimeSpan  TimeInterval(DateTime start, DateTime end) 
		{
			TimeSpan ts = end.Subtract(start).Duration();  // Duration()获取timespan的绝对值;
			return ts;
		}
		
		// 将时间间隔转换为秒数;
		public static int TimeToSecond(TimeSpan ts) 
		{
			int seconds = ts.Days * 3600 * 24 + ts.Hours * 3600 + ts.Minutes * 60 + ts.Seconds;
			return seconds;
		}
		
		// (Unix)时间戳是自 1970 年 1 月 1 日（08:00:00 GMT）至当前时间的总秒数;
		// 将时间戳转换为DateTime;
		public static DateTime GetUnixTimestamp(string timeStamp)
		{
			DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
			long lTime = long.Parse("0000000".Insert(0, timeStamp));
			TimeSpan toNow = new TimeSpan(lTime);
			return dtStart.Add(toNow);
		}
		
		// 计算字符串出现在目标字符串的次数;
		public static int StringAppearCount(string origin, string target) 
		{
			if (origin == null || target == null) 
			{
				return 0;
			}
			
			return Regex.Matches(target, @origin).Count;
		}
		
		// 将某个字符串替换为指定字符串;
		public static string ReplaceStr(string origin, string oldStr, string newStr)
		{
			if(string.IsNullOrEmpty(origin))
			{
				return null;
			}
			
			return Regex.Replace(origin, oldStr, newStr);
		}
		
		// 计算字符串的字节长度;
		public static int GetStringByteLength(string str)
		{
			int lenTotal = 0;
			for(int i=0; i < str.Length; i++)
			{
				string strWord = str.Substring(i,1);    
				int asc = Convert.ToChar(strWord);
				if ( asc < 0 || asc > 127 )
					lenTotal += 2;
				else
					lenTotal += 1;
			}
			return lenTotal;
		}

		public static int GetUnicodeStrByteLength(string str)
		{
			if(string.IsNullOrEmpty(str))
			{
				return 0;
			}
			//使用Unicode编码的方式将字符串转换为字节数组,它将所有字符串(包括英文中文)全部以2个字节存储;
			byte[] bytestr = System.Text.Encoding.Unicode.GetBytes(str);
			int length = 0;
			for (int i = 0; i < bytestr.GetLength(0); i++)
			{
				//取余2是因为字节数组中所有的双数下标的元素都是unicode字符的第一个字节;
				if (i % 2 == 0)
				{
					length++;
				}
				else
				{
					//单数下标都是字符的第2个字节,如果一个字符第2个字节为0,则代表该Unicode字符是英文字符,否则为中文字符;
					if (bytestr[i] > 0) 
					{
						length++;
					}
				}
			}
			return length;
		}

		// 计算字符串的字节长度;
		public static int GetStrByteLength(string str)
		{
			if(string.IsNullOrEmpty(str))
			{
				return 0;
			}
			
			byte[] bytes = System.Text.Encoding.Default.GetBytes(str);
			return bytes.Length;
		}

		public static void InitStringBuilder(ref System.Text.StringBuilder builder, params System.Object[] array)
		{
			if(builder == null)
			{
				builder = new System.Text.StringBuilder();
			}
			else
			{
				builder.Remove(0, builder.Length);
			}
			
			if(array != null && array.Length > 0)
			{
				for(int i = 0; i < array.Length; i++)
				{
					builder.Append(array[i]);
				}
			}
		}
	}

}