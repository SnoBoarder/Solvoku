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

public class MNWP8Dialog : MNPopup {

	//--------------------------------------
	// INITIALIZE
	//--------------------------------------
	
	public static MNWP8Dialog Create(string title, string message) {
		MNWP8Dialog dialog;
		dialog  = new GameObject("WP8Dialog").AddComponent<MNWP8Dialog>();
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
		WP8PopUps.PopUp.ShowMessageWindow_OK_Cancel(message, title, OnOkDel, OnCancelDel);
		#endif

	}
	
	//--------------------------------------
	//  GET/SET
	//--------------------------------------
	
	//--------------------------------------
	//  EVENTS
	//--------------------------------------
	
	
	public void OnOkDel() {
		dispatch(BaseEvent.COMPLETE, MNDialogResult.YES);
		Destroy(gameObject);
	}
	
	public void OnCancelDel() {
		dispatch(BaseEvent.COMPLETE, MNDialogResult.NO);
		Destroy(gameObject);
	}
	
	
	
	//--------------------------------------
	//  PRIVATE METHODS
	//--------------------------------------
	
	//--------------------------------------
	//  DESTROY
	//--------------------------------------


}
