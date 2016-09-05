using UnityEngine;
using System.Collections;
using System;

public enum TxtOutlineColor
{
	None,//无描边;
	Black,//黑色描边;
	Blue,//蓝色描边;
	Red,//红色描边;
}

public enum TxtColor
{
	Default,
	White,
	Blue1,
	Blue2,
	Blue3,
	Purple,
	Pink,
	Orange,
	Green,
	Yellow1,
	Yellow2,
	Gray,
	Red,
}
public enum TxtSize
{
	Default,
	size14=14,
	size16=16,
	size18=18,
	size22=22,
	size26=26,
	size35=35,
	size50=50,
}
public class ColorSet {

	//Label的字体颜色;
	public static string [] colorArr= new string[]
	{
		"",
		"ffffff",//白色;
		"2fd3e1",//蓝色1;
		"21435e",//蓝色2;
		"0e5d9f",//蓝色3;
		"f233d8",//紫色;
		"ff8686",//粉色;
		"ffc600",//橙色;
		"1aff00",//绿色;
		"ffd568",//黄色1;
		"fef42d",//黄色2
		"afb3b2",//灰色;
		"f31010",//红色;
	};
	//Label的字体描边颜色;
	public static string [] OutlineColorArr= new string[]
	{
		"",
		"000000",//黑色;
		"2e4e67",//蓝色;
		"920f0f",//红色;
	};
	/// <summary>
	/// Gets the color string.
	/// 获取颜色值;
	/// </summary>
	/// <returns>The color string.</returns>
	/// <param name="type">Type.</param>
	public static string getColorStr(TxtColor type)
	{
		int index = (int)type;
		string color = "b9b2b2";
		color = colorArr[index];
		return color;
	}

	/// <summary>
	/// Gets the color.
	/// 获取颜色;
	/// </summary>
	/// <returns>The color.</returns>
	/// <param name="type">Type.</param>
	public static Color getColor(TxtColor type)
	{
		int index = (int)type;
		string colorStr = "b9b2b2";
		if (index >= 0 && index < colorArr.Length)
		{
			colorStr = colorArr [index];
		}

		int r = Convert .ToInt16 ( "0x"+ colorStr.Substring(0, 2).ToString(),16);
		int g = Convert.ToInt16("0x" + colorStr.Substring(2, 2).ToString(), 16);
		int b = Convert.ToInt16("0x" + colorStr.Substring(4, 2).ToString(), 16);

//		int r = int.Parse(colorStr.Substring (0, 2));
//		int g = int.Parse(colorStr.Substring (2, 4));
//		int b = int.Parse(colorStr.Substring (4, 6));
		Color color = new Color (r/255f,g/255f,b/255f);

		return color;
	}
	/// <summary>
	/// Gets the color.
	/// 获取字体描边颜色;
	/// </summary>
	/// <returns>The color.</returns>
	/// <param name="type">Type.</param>
	public static Color getOutlineColor(TxtOutlineColor type)
	{
		int index = (int)type;
		string colorStr = "b9b2b2";
		if (index >= 0 && index < OutlineColorArr.Length) 
		{
			colorStr = OutlineColorArr [index];
		}
		
		int r = Convert .ToInt16 ( "0x"+ colorStr.Substring(0, 2).ToString(),16);
		int g = Convert.ToInt16("0x" + colorStr.Substring(2, 2).ToString(), 16);
		int b = Convert.ToInt16("0x" + colorStr.Substring(4, 2).ToString(), 16);

		Color color = new Color (r/255f,g/255f,b/255f);
		
		return color;
	}
	public static Color getColor1(TxtColor type)
	{
		int index = (int)type;
		string colorStr = "ffd568";
		colorStr = colorArr[index];
		
		int r = Convert .ToInt16 ( "0x"+ colorStr.Substring(0, 2).ToString(),16);
		int g = Convert.ToInt16("0x" + colorStr.Substring(2, 2).ToString(), 16);
		int b = Convert.ToInt16("0x" + colorStr.Substring(4, 2).ToString(), 16);
		
		//		int r = int.Parse(colorStr.Substring (0, 2));
		//		int g = int.Parse(colorStr.Substring (2, 4));
		//		int b = int.Parse(colorStr.Substring (4, 6));
		Color color = new Color (r/255f,g/255f,b/255f);
		
		return color;
	}
}
