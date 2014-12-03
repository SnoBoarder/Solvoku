////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnionAssets.FLE;
using System.Collections;
using System.Collections.Generic;

public class MNWP8Message : MNPopup {
		
	//--------------------------------------
	// INITIALIZE
	//--------------------------------------
			
	public static MNWP8Message Create(string title, string message) {
		MNWP8Message dialog;
		dialog  = new GameObject("WP8Message").AddComponent<MNWP8Message>();
		dialog.title = title;
		dialog.message = message;
		dialog.init();
		
		return dialog;
	}
	
	
	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------
	
	public void init() {
		#if UNITY_WP8 || UNITY_METRO
		WP8PopUps.PopUp.ShowMessageWindow_OK(message, title, onPopUpCallBack);
		#endif

	}
	
	//--------------------------------------
	//  GET/SET
	//--------------------------------------
	
	//--------------------------------------
	//  EVENTS
	//--------------------------------------
	
	public void onPopUpCallBack() {
		dispatch(BaseEvent.COMPLETE);
		Destroy(gameObject);
	}
	
	//--------------------------------------
	//  PRIVATE METHODS
	//--------------------------------------
	
	//--------------------------------------
	//  DESTROY
	//--------------------------------------


}
