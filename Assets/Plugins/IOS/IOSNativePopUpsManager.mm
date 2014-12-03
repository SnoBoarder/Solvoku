//
//  IOSNativePopUpsManager.m
//  Unity-iPhone
//
//  Created by Osipov Stanislav on 5/31/14.
//
//

#import "IOSNativePopUpsManager.h"

@implementation IOSNativePopUpsManager



static UIAlertView* _currentAllert =  nil;



+ (void) unregisterAllertView {
    if(_currentAllert != nil) {
        [_currentAllert release];
        _currentAllert = nil;
    }
}

+(void) dismissCurrentAlert {
    if(_currentAllert != nil) {
        [_currentAllert dismissWithClickedButtonIndex:0 animated:YES];
        [_currentAllert release];
        _currentAllert = nil;
    }
}

+(void) showRateUsPopUp: (NSString *) title message: (NSString*) msg b1: (NSString*) b1 b2: (NSString*) b2 b3: (NSString*) b3 {
    
    UIAlertView *alert = [[UIAlertView alloc] init];
    [alert setTitle:title];
    [alert setMessage:msg];
    [alert setDelegate: [[RatePopUPDelegate alloc] init]];
    
    [alert addButtonWithTitle:b1];
    [alert addButtonWithTitle:b2];
    [alert addButtonWithTitle:b3];
    
    [alert show];
    
    _currentAllert = alert;
    
}




+ (void) showDialog: (NSString *) title message: (NSString*) msg yesTitle:(NSString*) b1 noTitle: (NSString*) b2{
    
    UIAlertView *alert = [[UIAlertView alloc] init];
    [alert setTitle:title];
    [alert setMessage:msg];
    [alert setDelegate: [[PopUPDelegate alloc] init]];
    [alert addButtonWithTitle:b1];
    [alert addButtonWithTitle:b2];
    [alert show];
    
    _currentAllert = alert;
    
}


+(void) showMessage: (NSString *) title message: (NSString*) msg okTitle:(NSString*) b1 {
    
    UIAlertView *alert = [[UIAlertView alloc] init];
    [alert setTitle:title];
    [alert setMessage:msg];
    [alert setDelegate: [[PopUPDelegate alloc] init]];
    [alert addButtonWithTitle:b1];
    [alert show];
    
    _currentAllert = alert;
}

extern "C" {
    
    
    
    
    //--------------------------------------
	//  IOS Native Plugin Section
	//--------------------------------------
    
    void _ISN_ShowRateUsPopUp(char* title, char* message, char* b1, char* b2, char* b3) {
        [IOSNativePopUpsManager showRateUsPopUp:[ISNDataConvertor charToNSString:title] message:[ISNDataConvertor charToNSString:message] b1:[ISNDataConvertor charToNSString:b1] b2:[ISNDataConvertor charToNSString:b2] b3:[ISNDataConvertor charToNSString:b3]];
    }
    
    
    void _ISN_ShowDialog(char* title, char* message, char* yes, char* no) {
        [IOSNativePopUpsManager showDialog:[ISNDataConvertor charToNSString:title] message:[ISNDataConvertor charToNSString:message] yesTitle:[ISNDataConvertor charToNSString:yes] noTitle:[ISNDataConvertor charToNSString:no]];
    }
    
    void _ISN_ShowMessage(char* title, char* message, char* ok) {
        [IOSNativePopUpsManager showMessage:[ISNDataConvertor charToNSString:title] message:[ISNDataConvertor charToNSString:message] okTitle:[ISNDataConvertor charToNSString:ok]];
    }
    
    void _ISN_DismissCurrentAlert() {
        [IOSNativePopUpsManager dismissCurrentAlert];
    }
    
    
    //--------------------------------------
	//  Native PopUps Plugin Section
	//--------------------------------------
    
    void _MNP_ShowRateUsPopUp(char* title, char* message, char* b1, char* b2, char* b3) {
        _ISN_ShowRateUsPopUp(title, message, b1, b2, b3);
    }
    
    
    void _MNP_ShowDialog(char* title, char* message, char* yes, char* no) {
        _ISN_ShowDialog(title, message, yes, no);
    }
    
    void _MNP_ShowMessage(char* title, char* message, char* ok) {
        _ISN_ShowMessage(title, message, ok);
    }
    
    void _MNP_DismissCurrentAlert() {
        _ISN_DismissCurrentAlert();
    }

    
    
}




@end
