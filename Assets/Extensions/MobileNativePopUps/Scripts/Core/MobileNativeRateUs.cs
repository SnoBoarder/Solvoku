using UnityEngine;
using System;
using UnionAssets.FLE;
using System.Collections;

public class MobileNativeRateUs : EventDispatcherBase {

	public string title;
	public string message;
	public string yes;
	public string later;
	public string no;


	public string url;
	public string appleId;

	public Action<MNDialogResult> OnComplete = delegate {};




	public MobileNativeRateUs(string title, string message) {

		this.title = title;
		this.message = message;
		this.yes = "Rate app";
		this.later = "Later";
		this.no = "No, thanks";

	}
	
	public MobileNativeRateUs(string title, string message, string yes, string later, string no) {
		this.title = title;
		this.message = message;
		this.yes = yes;
		this.later = later;
		this.no = no;
	}


	public void SetAndroidAppUrl(string _url) {
		url = _url;
	}

	public void SetAppleId(string _appleId) {
		appleId = _appleId;
	}
	
	public void Start() {
		
		#if UNITY_WP8 || UNITY_METRO
		MNWP8RateUsPopUp rate = MNWP8RateUsPopUp.Create(title, message);
		rate.addEventListener(BaseEvent.COMPLETE, OnCompleteListener);
		#endif
		
		
		#if UNITY_IPHONE
		MNIOSRateUsPopUp rate = MNIOSRateUsPopUp.Create(title, message, yes, later, no);
		rate.appleId = appleId;
		rate.addEventListener(BaseEvent.COMPLETE, OnCompleteListener);
		#endif
		
		#if UNITY_ANDROID
		MNAndroidRateUsPopUp rate = MNAndroidRateUsPopUp.Create(title, message, url, yes, later, no);
		rate.addEventListener(BaseEvent.COMPLETE, OnCompleteListener);
		#endif

	}
	
	
	
	private void OnCompleteListener(CEvent e) {
		OnComplete((MNDialogResult)e.data);
		dispatch(BaseEvent.COMPLETE, e.data);
	}
}

