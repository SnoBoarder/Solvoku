using UnityEngine;
using System;
using UnionAssets.FLE;
using System.Collections;

public class MobileNativeDialog : EventDispatcherBase {

	public Action<MNDialogResult> OnComplete = delegate {};

	public MobileNativeDialog(string title, string message) {
		init(title, message, "Yes", "No");
	}

	public MobileNativeDialog(string title, string message, string yes, string no) {
		init(title, message, yes, no);
	}


	private void init(string title, string message, string yes, string no) {

		#if UNITY_WP8 || UNITY_METRO
		MNWP8Dialog dialog  = MNWP8Dialog.Create(title, message);
		dialog.addEventListener(BaseEvent.COMPLETE, OnCompleteListener);
		#endif


		#if UNITY_IPHONE
		MNIOSDialog dialog  = MNIOSDialog.Create(title, message, yes, no);
		dialog.addEventListener(BaseEvent.COMPLETE, OnCompleteListener);
		#endif

		#if UNITY_ANDROID
		MNAndroidDialog dialog  = MNAndroidDialog.Create(title, message, yes, no);
		dialog.addEventListener(BaseEvent.COMPLETE, OnCompleteListener);
		#endif


	}



	private void OnCompleteListener(CEvent e) {
		OnComplete((MNDialogResult)e.data);
		dispatch(BaseEvent.COMPLETE, e.data);
	}
}

