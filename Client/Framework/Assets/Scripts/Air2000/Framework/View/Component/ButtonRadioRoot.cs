using UnityEngine;
using System.Collections;

public class ButtonRadioRoot : MonoBehaviour {
	public GameObject objNowObject = null;

	public void SetObjNowObject(GameObject obj)
	{
		objNowObject = obj;
	}
	public static ButtonRadioRoot ReigisterInstance(GameObject obj)
	{
		ButtonRadioRoot btnRadio = obj.GetComponent<ButtonRadioRoot> ();
		if (btnRadio == null) {
			btnRadio = obj.AddComponent<ButtonRadioRoot>();
		}
		return btnRadio;
	}
	public void changeCheckedObject(GameObject obj)
	{
		if ( obj!=objNowObject ) {
			if(objNowObject != null)
			{
				foreach(GameObject objChild in ButtonRootDelegate.RegisterBtnDelegate(objNowObject).list_ObjectList)
				{
					ButtonColorChange.RegisterInstance(objChild).setIsChecked(false);
					ButtonColorChange.RegisterInstance(objChild).change();
				}
				objNowObject = obj;
				
			}
			else objNowObject = obj;
		}
	}
}
