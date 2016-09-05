using UnityEngine;
using System.Collections;
//using System.Data;
using System.Runtime.InteropServices;

public class ButtonColorChange : MonoBehaviour {
	public GameObject buttonRoot;
	public GameObject RadioButtonRoot;
	public TxtColor showColor = TxtColor.Default;
	public TxtSize showSize = TxtSize.Default;
	private bool isChecked = false;
	public bool isNowChecked = false;
	private bool m_isNowChecked = false;
	public bool isEffect = false;
	private Color m_clrNowColor;
	private UIFont m_fNowSize;
	private int m_nNowSize;
	private UILabel.Effect nowEffect;
	// Use this for initialization
	void Awake ()
	{

		}
	void Start () {
		if (buttonRoot == null) {
			UIButton button = transform.parent.gameObject.GetComponent<UIButton>();
			if(button != null)
			{
				buttonRoot = button.gameObject;
			}
			else{
				button = transform.parent.parent.gameObject.GetComponent<UIButton>();
				if(button != null)
				{
					buttonRoot = button.gameObject;
				}
			}
		}
		if (buttonRoot != null) {
			UILabel label = gameObject.GetComponent<UILabel> ();
			m_clrNowColor = label.color;
			ButtonRootDelegate.RegisterBtnDelegate (buttonRoot).m_mydeleage += Delegae_ColorChange;
			m_fNowSize = label.font;
			m_nNowSize = (int)transform.localScale.x;
			ButtonRootDelegate.RegisterBtnDelegate (buttonRoot).list_ObjectList.Add(gameObject);
			nowEffect = label.effectStyle;
		}

		m_isNowChecked = isNowChecked;
		change();

	}
	public void setIsChecked(bool b)
	{
		m_isNowChecked = b;
	}
	public static ButtonColorChange RegisterInstance(GameObject obj)
	{
		ButtonColorChange btnclrChange = obj.GetComponent<ButtonColorChange> ();
		if (btnclrChange == null) {
			btnclrChange = obj.AddComponent<ButtonColorChange>();
		}
		return btnclrChange;
	}

	public void change()
	{
		UILabel label = gameObject.GetComponent<UILabel> ();
		if (m_isNowChecked != isChecked) {
			if(!m_isNowChecked)
			{
				isChecked = false;
				label.color = m_clrNowColor;
				label.font = m_fNowSize;
				transform.localScale = new Vector2( m_nNowSize,m_nNowSize);
				label.effectStyle = nowEffect;
			}
			else{
				if(ButtonRadioRoot.ReigisterInstance(RadioButtonRoot).objNowObject == null || ButtonRadioRoot.ReigisterInstance(RadioButtonRoot).objNowObject == buttonRoot)
				{
					ButtonRadioRoot.ReigisterInstance(RadioButtonRoot).SetObjNowObject(buttonRoot);
					isChecked = true;
					label.color = StringToColor (ColorSet.colorArr [(int)showColor]);
					if(showSize != TxtSize.Default)
						label.font =  Resources.Load(getPath(), typeof(UIFont)) as UIFont;
					else 
						label.font = m_fNowSize ;
					if(isEffect)
					{
					label.effectStyle = UILabel.Effect.Outline;
					label.effectColor = new Color(46.0f/255.0f,78.0f/255.0f,103.0f/255.0f);
					label.effectDistance = new Vector2(0.78f,0.5f);
					}
				}
				else{
					isChecked = false;
					m_isNowChecked = false;
				}
			}
		}
	}
	 Color StringToColor(string nColor)
	{
		nColor = nColor.ToLower ();
		int r = 0;
		int g = 0;
		int b = 0;
		int nTag = 0;
		foreach(char ch in nColor)
		{
			int nNum = 0;
			switch(ch)
			{
			case '0':
				nNum = 0;
				break;
			case '1':
				nNum = 1;
				break;
			case '2':
				nNum = 2;
				break;
			case '3':
				nNum = 3;
				break;
			case '4':
				nNum = 4;
				break;
			case '5':
				nNum = 5;
				break;
			case '6':
				nNum = 6;
				break;
			case '7':
				nNum = 7;
				break;
			case '8':
				nNum = 8;
				break;
			case '9':
				nNum = 9;
				break;
			case 'a':
				nNum = 10;
				break;
			case 'b':
				nNum = 11;
				break;
			case 'c':
				nNum = 12;
				break;
			case 'd':
				nNum = 13;
				break;
			case 'e':
				nNum = 14;
				break;
			case 'f':
				nNum = 15;
				break;
			}
			if(nTag == 0 ||nTag == 1)
			{
				r += (nTag != 0) ? nNum : nNum * 16;
			}
			if(nTag == 2 ||nTag == 3)
			{
				g += (nTag != 2) ? nNum : nNum * 16;
			}
			if(nTag == 4 ||nTag == 5)
			{
				b += (nTag != 4) ? nNum : nNum * 16;
			}
			nTag++;
		}
		Color clrColor = new Color(r/255.0f,g/255.0f,b/255.0f);
		if (showColor == TxtColor.Default)
						clrColor = m_clrNowColor;
		return clrColor;
	}
	string getPath()
	{
		string strPath ="";
//		UILabel label = gameObject.GetComponent<UILabel> ();
		switch (showSize) {
		case TxtSize.Default:
			break;
		case TxtSize.size16:
			strPath = "font/size16";
			transform.localScale = new Vector2( 16,16);
			break;
		case TxtSize.size18:
			strPath = "font/size18";
			transform.localScale = new Vector2( 18,18);
			break;
		case TxtSize.size22:
			strPath = "font/size22";
			transform.localScale = new Vector2( 22,22);
			break;
		case TxtSize.size26:
			strPath = "font/size26";
			transform.localScale = new Vector2( 26,26);
			break;
		case TxtSize.size35:
			strPath = "font/size35";
			transform.localScale = new Vector2( 35,35);
			break;
		case TxtSize.size50:
			strPath = "font/size50";
			transform.localScale = new Vector2( 50,50);
			break;
		}
		return strPath;
	}
	void Delegae_ColorChange(GameObject obj)
	{
		if (RadioButtonRoot != null) {
			//Helper.Log ("");
						ButtonRadioRoot.ReigisterInstance (RadioButtonRoot).changeCheckedObject (buttonRoot);
				}
		UILabel label = gameObject.GetComponent<UILabel> ();
		if (!isChecked) {
			isChecked = true;
			m_isNowChecked = true;			
			label.color = StringToColor (ColorSet.colorArr [(int)showColor]);
			if(showSize != TxtSize.Default)
			label.font =  Resources.Load(getPath(), typeof(UIFont)) as UIFont;
			else 
				label.font = m_fNowSize ;
			if(isEffect)
			{
				label.effectStyle = UILabel.Effect.Outline;
				label.effectColor = new Color(46.0f/255.0f,78.0f/255.0f,103.0f/255.0f);
				label.effectDistance = new Vector2(0.78f,0.5f);
			}
				} 
		

	}
}