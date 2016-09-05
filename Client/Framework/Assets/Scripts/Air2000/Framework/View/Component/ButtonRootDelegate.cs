using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ButtonRootDelegate : MonoBehaviour {
	public delegate void MyButtonDelegate(GameObject obj);
	public MyButtonDelegate m_mydeleage; 
	public List<GameObject> list_ObjectList;

	void OnClick()
	{
		if (m_mydeleage != null) {
			m_mydeleage(gameObject);	
		}
	}

	public static ButtonRootDelegate RegisterBtnDelegate(GameObject obj)
	{

		ButtonRootDelegate cBtndelegate = obj.GetComponent<ButtonRootDelegate> ();
		if(cBtndelegate == null)
		{
			cBtndelegate = obj.AddComponent<ButtonRootDelegate>();
		}
		UIButtonMessage btnMessage = obj.GetComponent<UIButtonMessage> ();
		if (btnMessage == null) {
			btnMessage = obj.AddComponent<UIButtonMessage>();
		}
		if (cBtndelegate.list_ObjectList == null)
		{
			cBtndelegate.list_ObjectList=new List<GameObject>();
		}
		return cBtndelegate;
	}

}
