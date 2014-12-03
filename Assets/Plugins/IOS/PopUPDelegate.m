//
//  RatePopUPDelegate.m
//
//  Created by Osipov Stanislav on 1/12/13.
//
//

#import "PopUPDelegate.h"
#import "ISNDataConvertor.h"
#import "IOSNativePopUpsManager.h"

@implementation PopUPDelegate

- (void)alertView:(UIAlertView *)alertView clickedButtonAtIndex:(NSInteger)buttonIndex {
    [IOSNativePopUpsManager unregisterAllertView];
    UnitySendMessage("IOSPopUp", "onPopUpCallBack",  [ISNDataConvertor NSIntToChar:buttonIndex]);
}


@end
