////////////////////////////////////////////////////////////////////////////////
//  
// @module Common Android Native Lib
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class MNAndroidNative {

	private const string CLASS_NAME = "com.mnp.popups.NativePopupsManager";
	
	private static void CallActivityFunction(string methodName, params object[] args) {
		MNProxyPool.CallStatic(CLASS_NAME, methodName, args);
	}
	
	//--------------------------------------
	//  MESSAGING
	//--------------------------------------


	public static void showDialog(string title, string message) {
		showDialog (title, message, "Yes", "No");
	}

	public static void showDialog(string title, string message, string yes, string no) {
		CallActivityFunction("ShowDialog", title, message, yes, no);
	}

	public static void showMessage(string title, string message) {
		showMessage (title, message, "Ok");
	}

	public static void showMessage(string title, string message, string ok) {
		CallActivityFunction("ShowMessage", title, message, ok);
	}

	public static void showRateDialog(string title, string message, string yes, string laiter, string no) {
		CallActivityFunction("ShowRateDialog", title, message, yes, laiter, no);
	}
	
	public static void ShowPreloader(string title, string message) {
		CallActivityFunction("ShowPreloader",  title, message);
	}
	
	public static void HidePreloader() {
		CallActivityFunction("HidePreloader");
	}

	public static void RedirectStoreRatingPage(string url) {
		CallActivityFunction("OpenAppRatingPage", url);
	}

}
