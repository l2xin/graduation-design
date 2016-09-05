using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[ExecuteInEditMode]
public class LabelSkin : MonoBehaviour {
	
	bool mStarted = false;
	UILabel label;


	public enum TxtKind
	{
		None,
		Common,//通用文本 白色显示;
		Title,//标题文本 ;
		BigBtn,//大按钮文本;
		NormalBtn,//一般中等按钮文本;
		TabBtn,//选项卡文本;
		TabBtnSelect,//选项卡文本选中;
		ContentTitle,//文本标题;
		RankTitle,//排行标题;
		Content,//正文显示;

		NormalBtnYellow,//中等按钮底框为黄色的文本;
		NormalBtnBlue,//中等按钮底框为黄色的文本;
		SmallBtnBlue,//小按钮底框为蓝色的文本;
		SmallBtnRed,//小按钮底框为红色的文本;
		TabBtn1,
		TabBtnSelect1,

	}
	/// <summary>
	/// The kind.
	/// 显示文本的类型 不同种类不同配置;
	/// </summary>
	public TxtKind kind = TxtKind.None;
	/// <summary>
	/// The color of the current.
	/// 如果是默认将使用种类默认配置;
	/// </summary>
	public TxtColor showColor = TxtColor.Default;
	/// <summary>
	/// The size of the current.
	/// 如果不是默认的大小就用选择的;
	/// </summary>
	public TxtSize showSize = TxtSize.Default;
	/// <summary>
	/// The b show out line.
	/// 是否显示描边;
	/// </summary>
	public bool bShowOutLine = true;
	/// <summary>
	/// The color of the current.
	/// 如果是默认将使用种类默认配置 描边的颜色;
	/// </summary>
	public TxtOutlineColor OutlineColor = TxtOutlineColor.None;
	/// <summary>
	/// The refresh now.
	/// 更新配置显示;
	/// </summary>
	public bool refreshNow = false;
	/// <summary>
	/// 字体间距的配置信息.
	/// </summary>
	public static Dictionary<TxtSize,Vector2> mFontsSpace;

	// Use this for initialization
	void Start () 
	{
		mStarted = true;
		InitLabelFont ();
		ShowFixLabelBySet ();
	}
	/// <summary>
	/// Shows the fix label by set.
	/// 根据设置更新label显示;
	/// </summary>
	void ShowFixLabelBySet()
	{
		if (mStarted) 
		{
			//设置字体的大小;
			SetLabelShowSize ();
			//设置字体的颜色;
			SetLabelShowColor ();
			//设置字体的描边;
			SetLabelOutlineColor();
			//设置某个种类的默认显示配置;
			SetLabelKind();
		} 
		else 
		{
			refreshNow = true;
		}
	}
	//设置某个种类的默认显示;
	void SetLabelKind()
	{
		if (kind == TxtKind.None) 
		{
			return;
		}
		switch (kind) 
		{
		case TxtKind.Common:
			SetLabel_Common ();
			break;
		case TxtKind.Title:
			SetLabel_Title ();
			break;
		case TxtKind.BigBtn:
			SetLabel_BigBtn ();
			break;
		case TxtKind.NormalBtn:
			SetLabel_NormalBtn ();
			break;
//		case TxtKind.TabBtn:
//			SetLabel_TabBtn();
//			break;
//		case TxtKind.TabBtnSelect:
//			SetLabel_TabBtnSelect();
//			break;
		case TxtKind.ContentTitle:
			SetLabel_ContentTitle ();
			break;
		case TxtKind.RankTitle:
			SetLabel_RankTitle ();
			break;
		case TxtKind.Content:
			SetLabel_Content ();
			break;
		case TxtKind.TabBtnSelect:
			SetLabel_TabBtnSelect ();
			break;
		case TxtKind.TabBtnSelect1:
			SetLabel_TabBtnSelect1 ();
			break;
		case TxtKind.TabBtn:
			SetLabel_TabBtn ();
			break;
		case TxtKind.TabBtn1:
			SetLabel_TabBtn1 ();
			break;
		case TxtKind.NormalBtnYellow:
			SetLabel_NormalBtnYellow ();
			break;
		case TxtKind.NormalBtnBlue:
			SetLabel_NormalBtnBlue ();
			break;
		case TxtKind.SmallBtnBlue:
			SetLabel_SmallBtnBlue ();
			break;
		case TxtKind.SmallBtnRed:
			SetLabel_SmallBtnRed ();
			break;
		}
	}
	//初始化字体;
	void InitLabelFont()
	{
		if (label == null) 
		{
			label = transform.GetComponent<UILabel> ();
		}
		if (label != null) 
		{
			if (label.bitmapFont == null) 
			{
				UIFont fontFit = Resources.Load ("Font/Font16", typeof(UIFont)) as UIFont;
				if (fontFit != null) 
				{
					label.bitmapFont = fontFit;
					//#if UNITY_EDITOR
					//label.MakePixelPerfect();						
					//#endif
				}
			}
		}
	}
	/// <summary>
	/// 设置Label的字体大小.
	/// </summary>
	void SetLabelShowSize()
	{
		if (showSize == TxtSize.Default) 
		{
			return;
		}
		if (label != null) 
		{
			label.fontSize=(int)showSize;
			Vector2 tempSpace=LabelSkin.GetLabelSpaceing(showSize);
			label.spacingX=(int)tempSpace.x;
			label.spacingY=(int)tempSpace.y;
		}
	}
	public static Vector2 GetLabelSpaceing(TxtSize varSize)
	{
		Vector2 tempSpace = Vector2.zero;
		if (mFontsSpace == null) 
		{
			mFontsSpace = new Dictionary<TxtSize, Vector2> ();
			mFontsSpace.Add (TxtSize.size14, new Vector2 (0.0f, 4.0f));
			mFontsSpace.Add (TxtSize.size16, new Vector2 (1.0f, 4.0f));
			mFontsSpace.Add (TxtSize.size18, new Vector2 (1.0f, 5.0f));
			mFontsSpace.Add (TxtSize.size22, new Vector2 (1.0f, 5.0f));
			mFontsSpace.Add (TxtSize.size26, new Vector2 (0.0f, 6.0f));
			mFontsSpace.Add (TxtSize.size35, new Vector2 (0.0f, 6.0f));
			mFontsSpace.Add (TxtSize.size50, new Vector2 (1.0f, 10.0f));
		}
		if (mFontsSpace != null) 
		{
			if(mFontsSpace.TryGetValue(varSize,out tempSpace))
			{
				return tempSpace;
			}
		}
		return tempSpace;
	}
	/// <summary>
	/// 设置Label的字体颜色.
	/// </summary>
	void SetLabelShowColor()
	{
		if (showColor == TxtColor.Default) 
		{
			return;
		}
		if (label != null) 
		{
			label.color=ColorSet.getColor(showColor);
		}
	}
	/// <summary>
	/// 设置Label字体的描边.
	/// </summary>
	void SetLabelOutlineColor()
	{
		if (label != null) 
		{
			if(bShowOutLine&&OutlineColor!=TxtOutlineColor.None)
			{
				label.effectStyle = UILabel.Effect.Outline;
				label.effectColor = ColorSet.getOutlineColor(OutlineColor);
				label.effectDistance = new Vector2(0.78f,0.5f);
			}
			else
			{
				label.effectStyle = UILabel.Effect.None;
			}
		}

	}
	/// <summary>
	/// Sets the label_ common.
	/// 通用文本的配置;
	/// </summary>
	/// 

	void SetLabel_TabBtnSelect()
	{
//		if(showColor == TxtColor.Default)//用种类默认的;
//		{
//			label.color = ColorSet.getColor(TxtColor.LabelButtonBig);
//		}
//		if(bShowOutLine)
//		{
//			label.effectStyle = UILabel.Effect.Outline;
//			label.effectColor = OutlineColor == TxtColor.Default? new Color(46.0f/255.0f,78.0f/255.0f,103.0f/255.0f):ColorSet.getColor(OutlineColor);
//			label.effectDistance = new Vector2(0.78f,0.5f);
//		}
//		else{
//			label.effectStyle = UILabel.Effect.None;
//		}
//		if(showSize == TxtSize.Default)
//		{
//			//label.font =  Resources.Load("font/size26", typeof(UIFont)) as UIFont;
//			label.fontSize=26;
//			//#if UNITY_EDITOR
//			label.MakePixelPerfect();						
//			//#endif
//		}
	}
	void SetLabel_TabBtnSelect1()//用于不选项字体为白色
	{
//		if(showColor == TxtColor.Default)//用种类默认的;
//		{
//			label.color = ColorSet.getColor1(TxtColor.LabelButtonBig);
//		}
//		if(bShowOutLine)
//		{
//			label.effectStyle = UILabel.Effect.Outline;
//			label.effectColor = OutlineColor == TxtColor.Default? new Color(46.0f/255.0f,78.0f/255.0f,103.0f/255.0f):ColorSet.getColor(OutlineColor);;
//			label.effectDistance = new Vector2(0.78f,0.5f);
//		}
//		else{
//			label.effectStyle = UILabel.Effect.None;
//		}
//		if(showSize == TxtSize.Default)
//		{
//			//label.font =  Resources.Load("font/size26", typeof(UIFont)) as UIFont;
//			label.fontSize=26;
//			//#if UNITY_EDITOR
//			//label.MakePixelPerfect();						
//			//#endif
//		}
	}
	void SetLabel_TabBtn()
	{
//		if(showColor == TxtColor.Default)//用种类默认的;
//		{
//			label.color = ColorSet.getColor(TxtColor.LabelButtomSmall);
//		}
//		if(bShowOutLine)
//		{
//			//				label.effectStyle = UILabel.Effect.Outline;
//			//				label.effectColor = Color.black;
//			//				label.effectDistance = new Vector2(0.78f,0.5f);
//			label.effectStyle = UILabel.Effect.None;
//		}
//		else{
//			label.effectStyle = UILabel.Effect.None;
//		}
//		if(showSize == TxtSize.Default)
//		{
//			//label.font =  Resources.Load("font/size22", typeof(UIFont)) as UIFont;
//			label.fontSize=22;
//			//#if UNITY_EDITOR
//			label.MakePixelPerfect();						
//			///#endif
//		}
	}
	void SetLabel_TabBtn1()
	{
//		if(showColor == TxtColor.Default)//用种类默认的;
//		{
//			label.color = ColorSet.getColor(TxtColor.White);
//		}
//		if(bShowOutLine)
//		{
//			//				label.effectStyle = UILabel.Effect.Outline;
//			//				label.effectColor = Color.black;
//			//				label.effectDistance = new Vector2(0.78f,0.5f);
//			label.effectStyle = UILabel.Effect.None;
//		}
//		else{
//			label.effectStyle = UILabel.Effect.None;
//		}
//		if(showSize == TxtSize.Default)
//		{
//			//label.font =  Resources.Load("font/size22", typeof(UIFont)) as UIFont;
//			label.fontSize=22;
//			//#if UNITY_EDITOR
//			//label.MakePixelPerfect();						
//			///#endif
//		}
	}
	void SetLabel_NormalBtnYellow()
	{
//		if(showColor == TxtColor.Default)//用种类默认的;
//		{
//			label.color = ColorSet.getColor(TxtColor.ProminentButtonYellow);
//		}
//		if(bShowOutLine)
//		{
//			label.effectStyle = UILabel.Effect.Outline;
//			label.effectColor = OutlineColor == TxtColor.Default?new Color(146.0f/255.0f,15.0f/255.0f,15.0f/255.0f):ColorSet.getColor(OutlineColor);;
//			label.effectDistance = new Vector2(0.78f,0.5f);
//		}
//		else{
//			label.effectStyle = UILabel.Effect.None;
//		}
//		if(showSize == TxtSize.Default)
//		{
//			//label.font =  Resources.Load("font/size26", typeof(UIFont)) as UIFont;
//			label.fontSize=26;
//			//#if UNITY_EDITOR
//			//label.MakePixelPerfect();						
//			//#endif
//		}
	}
	void SetLabel_NormalBtnBlue()
	{
//		if(showColor == TxtColor.Default)//用种类默认的;
//		{
//			label.color = ColorSet.getColor(TxtColor.ProminentButtonBlue);
//		}
//		if(bShowOutLine)
//		{
//			label.effectStyle = UILabel.Effect.Outline;
//			label.effectColor = OutlineColor == TxtColor.Default?new Color(46.0f/255.0f,78.0f/255.0f,103.0f/255.0f):ColorSet.getColor(OutlineColor);;
//			label.effectDistance = new Vector2(0.78f,0.5f);
//		}
//		else{
//			label.effectStyle = UILabel.Effect.None;
//		}
//		if(showSize == TxtSize.Default)
//		{
//			//label.font =  Resources.Load("font/size26", typeof(UIFont)) as UIFont;
//			label.fontSize=26;
//			//#if UNITY_EDITOR
//			//label.MakePixelPerfect();						
//			//#endif
//		}
	}
	void SetLabel_SmallBtnBlue()
	{
//		if(showColor == TxtColor.Default)//用种类默认的;
//		{
//			label.color = ColorSet.getColor(TxtColor.SmallButtonBlue);
//		}
//		if(bShowOutLine)
//		{
//			label.effectStyle = UILabel.Effect.Outline;
//			label.effectColor = OutlineColor == TxtColor.Default?new Color(46.0f/255.0f,78.0f/255.0f,103.0f/255.0f):ColorSet.getColor(OutlineColor);;
//			label.effectDistance = new Vector2(0.78f,0.5f);
//		}
//		else{
//			label.effectStyle = UILabel.Effect.None;
//		}
//		if(showSize == TxtSize.Default)
//		{
//			//label.font =  Resources.Load("font/size22", typeof(UIFont)) as UIFont;
//			label.fontSize=22;
//			//#if UNITY_EDITOR
//			//label.MakePixelPerfect();						
//			//#endif
//		}
	}
	void SetLabel_SmallBtnRed()
	{
//		if(showColor == TxtColor.Default)//用种类默认的;
//		{
//			label.color = ColorSet.getColor(TxtColor.SmallButtonRed);
//		}
//		if(bShowOutLine)
//		{
//			label.effectStyle = UILabel.Effect.Outline;
//			label.effectColor = OutlineColor == TxtColor.Default?new Color(46.0f/255.0f,78.0f/255.0f,103.0f/255.0f):ColorSet.getColor(OutlineColor);;
//			label.effectDistance = new Vector2(0.78f,0.5f);
//		}
//		else{
//			label.effectStyle = UILabel.Effect.None;
//		}
//		if(showSize == TxtSize.Default)
//		{
//			//label.font =  Resources.Load("font/size22", typeof(UIFont)) as UIFont;
//			label.fontSize=22;
//			//#if UNITY_EDITOR
//			//label.MakePixelPerfect();						
//			//#endif
//		}
	}
	void SetLabel_Common()
	{
//		if(showColor == TxtColor.Default)//用种类默认的;
//		{
//			label.color = ColorSet.getColor(TxtColor.White);
//		}
//		if(bShowOutLine)
//		{
//			label.effectStyle = UILabel.Effect.Outline;
//			label.effectColor = OutlineColor == TxtColor.Default?new Color(46.0f/255.0f,78.0f/255.0f,103.0f/255.0f):ColorSet.getColor(OutlineColor);;
//			label.effectDistance = new Vector2(0.78f,0.5f);
//		}
//		else{
//			label.effectStyle = UILabel.Effect.None;
//		}
//		if(showSize == TxtSize.Default)
//		{
//			//label.font =  Resources.Load("font/size24", typeof(UIFont)) as UIFont;
//			//label.font =  Resources.Load("font/size26", typeof(UIFont)) as UIFont;
//			label.fontSize=26;
//			//#if UNITY_EDITOR
//			//label.MakePixelPerfect();						
//			//#endif
//		}
	}
	void SetLabel_Title()
	{
//		if(showColor == TxtColor.Default)//用种类默认的;
//		{
//			label.color = ColorSet.getColor(TxtColor.Title);
//		}
//		if(bShowOutLine)
//		{
//			label.effectStyle = UILabel.Effect.Outline;
//			label.effectColor = OutlineColor == TxtColor.Default?new Color(46.0f/255.0f,78.0f/255.0f,103.0f/255.0f):ColorSet.getColor(OutlineColor);;
//			label.effectDistance = new Vector2(0.78f,0.5f);
//		}
//		else{
//			label.effectStyle = UILabel.Effect.None;
//		}
//		if(showSize == TxtSize.Default)
//		{
//			//label.font =  Resources.Load("font/size34", typeof(UIFont)) as UIFont;
//			label.fontSize=34;
//			//#if UNITY_EDITOR
//			//label.MakePixelPerfect();						
//			//#endif
//		}
	}
	void SetLabel_BigBtn()
	{
//		if(showColor == TxtColor.Default)//用种类默认的;
//		{
//			label.color = ColorSet.getColor(TxtColor.BigBtn);
//		}
//		if(bShowOutLine)
//		{
//			label.effectStyle = UILabel.Effect.Outline;
//			label.effectColor = OutlineColor == TxtColor.Default?new Color(46.0f/255.0f,78.0f/255.0f,103.0f/255.0f):ColorSet.getColor(OutlineColor);;
//			label.effectDistance = new Vector2(0.78f,0.5f);
//		}
//		else{
//			label.effectStyle = UILabel.Effect.None;
//		}
//		if(showSize == TxtSize.Default)
//		{
//			//label.font =  Resources.Load("font/size34", typeof(UIFont)) as UIFont;
//			label.fontSize=34;
//			//#if UNITY_EDITOR
//			//label.MakePixelPerfect();						
//			//#endif
//		}
	}
	void SetLabel_NormalBtn()
	{
//		if(showColor == TxtColor.Default)//用种类默认的;
//		{
//			label.color = ColorSet.getColor(TxtColor.SmallBtn);
//		}
//		if(bShowOutLine)
//		{
//			label.effectStyle = UILabel.Effect.Outline;
//			label.effectColor = OutlineColor == TxtColor.Default?new Color(46.0f/255.0f,78.0f/255.0f,103.0f/255.0f):ColorSet.getColor(OutlineColor);;
//			label.effectDistance = new Vector2(0.78f,0.5f);
//		}
//		else{
//			label.effectStyle = UILabel.Effect.None;
//		}
//		if(showSize == TxtSize.Default)
//		{
//			//label.font =  Resources.Load("font/size21", typeof(UIFont)) as UIFont;
//			label.fontSize=22;
//			//#if UNITY_EDITOR
//			//label.MakePixelPerfect();						
//			//#endif
//		}
	}
//	void SetLabel_TabBtn()
//	{
//		if(showColor == TxtColor.Default)//用种类默认的;
//		{
//			label.color = ColorSet.getColor(TxtColor.TabBtnDark);
//		}
	//			if(bShowOutLine)
	//			{
	//				label.effectStyle = UILabel.Effect.Outline;
	//				label.effectColor = OutlineColor == TxtColor.Default?new Color(46.0f/255.0f,78.0f/255.0f,103.0f/255.0f):ColorSet.getColor(OutlineColor);;
	//				label.effectDistance = new Vector2(0.78f,0.5f);
	//			}
	//			else{
	//				label.effectStyle = UILabel.Effect.None;
	//			}
//		if(showSize == TxtSize.Default)
//		{
//			label.font =  Resources.Load("font/size25", typeof(UIFont)) as UIFont;
//			
//			#if UNITY_EDITOR
//			label.MakePixelPerfect();						
//			#endif
//		}
//	}
//	void SetLabel_TabBtnSelect()
//	{
//		if(showColor == TxtColor.Default)//用种类默认的;
//		{
//			label.color = ColorSet.getColor(TxtColor.TabBtn);
//		}
	//			if(bShowOutLine)
	//			{
	//				label.effectStyle = UILabel.Effect.Outline;
	//				label.effectColor = OutlineColor == TxtColor.Default?new Color(46.0f/255.0f,78.0f/255.0f,103.0f/255.0f):ColorSet.getColor(OutlineColor);;
	//				label.effectDistance = new Vector2(0.78f,0.5f);
	//			}
	//			else{
	//				label.effectStyle = UILabel.Effect.None;
	//			}
//		if(showSize == TxtSize.Default)
//		{
//			label.font =  Resources.Load("font/size25", typeof(UIFont)) as UIFont;
//			
//			#if UNITY_EDITOR
//			label.MakePixelPerfect();						
//			#endif
//		}
//	}

	void SetLabel_ContentTitle()
	{
//		if(showColor == TxtColor.Default)//用种类默认的;
//		{
//			label.color = ColorSet.getColor(TxtColor.DarkOrange);
//		}
//		if(bShowOutLine)
//		{
//			label.effectStyle = UILabel.Effect.Outline;
//			label.effectColor = OutlineColor == TxtColor.Default?new Color(46.0f/255.0f,78.0f/255.0f,103.0f/255.0f):ColorSet.getColor(OutlineColor);;
//			label.effectDistance = new Vector2(0.78f,0.5f);
//		}
//		else{
//			label.effectStyle = UILabel.Effect.None;
//		}
//		if(showSize == TxtSize.Default)
//		{
//			//label.font =  Resources.Load("font/size30", typeof(UIFont)) as UIFont;
//			label.fontSize=35;
//			//#if UNITY_EDITOR
//			//label.MakePixelPerfect();						
//			//#endif
//		}
	}
	void SetLabel_RankTitle()
	{
//		if(showColor == TxtColor.Default)//用种类默认的;
//		{
//			label.color = ColorSet.getColor(TxtColor.OrangeYellow);
//		}
//		if(bShowOutLine)
//		{
//			label.effectStyle = UILabel.Effect.Outline;
//			label.effectColor = OutlineColor == TxtColor.Default? new Color(46.0f/255.0f,78.0f/255.0f,103.0f/255.0f):ColorSet.getColor(OutlineColor);;
//			label.effectDistance = new Vector2(0.78f,0.5f);
//		}
//		else{
//			label.effectStyle = UILabel.Effect.None;
//		}
//		if(showSize == TxtSize.Default)
//		{
//			//label.font =  Resources.Load("font/size25", typeof(UIFont)) as UIFont;
//			label.fontSize=26;
//			//#if UNITY_EDITOR
//			//label.MakePixelPerfect();						
//			//#endif
//		}
	}
	void SetLabel_Content()
	{
//		if(showColor == TxtColor.Default)//用种类默认的;
//		{
//			label.color = ColorSet.getColor(TxtColor.WhiteYellow);
//		}
//		if(bShowOutLine)
//		{
//			label.effectStyle = UILabel.Effect.Outline;
//			label.effectColor = OutlineColor == TxtColor.Default? new Color(46.0f/255.0f,78.0f/255.0f,103.0f/255.0f):ColorSet.getColor(OutlineColor);;
//			label.effectDistance = new Vector2(0.78f,0.5f);
//		}
//		else{
//			label.effectStyle = UILabel.Effect.None;
//		}
//		if(showSize == TxtSize.Default)
//		{
//			//label.font =  Resources.Load("font/size21", typeof(UIFont)) as UIFont;
//			label.fontSize=22;
//			//#if UNITY_EDITOR
//			//label.MakePixelPerfect();						
//			//#endif
//		}
	}
	/// <summary>
	/// Refreses the show now.
	/// 手动刷新显示设置;
	/// </summary>
	public void RefresShowNow()
	{
		refreshNow = false;
		ShowFixLabelBySet();
	}

	#if UNITY_EDITOR
	void LateUpdate ()
	{
		if (refreshNow)
		{
			refreshNow = false;
			ShowFixLabelBySet();
		}
	}
	#endif
}
