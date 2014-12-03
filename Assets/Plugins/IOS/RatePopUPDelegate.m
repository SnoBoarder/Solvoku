//
//  RatePopUPDelegate.m
//
//  Created by Osipov Stanislav on 1/12/13.
//
//

#import "RatePopUPDelegate.h"
#import "ISNDataConvertor.h"
#import "IOSNativeUtility.h"
#import "IOSNativePopUpsManager.h"

@implementation RatePopUPDelegate


- (void)alertView:(UIAlertView *)alertView clickedButtonAtIndex:(NSInteger)buttonIndex {
    
    
    
    [IOSNativePopUpsManager unregisterAllertView];
    UnitySendMessage("IOSRateUsPopUp", "onPopUpCallBack",  [ISNDataConvertor NSIntToChar:buttonIndex]);
}


@end
