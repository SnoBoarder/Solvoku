//
//  IOSNativeUtility.m
//  Unity-iPhone
//
//  Created by Osipov Stanislav on 4/29/14.
//
//

#import "IOSNativeUtility.h"

@implementation IOSNativeUtility

static IOSNativeUtility *_sharedInstance;
static NSString* templateReviewURLIOS7  = @"itms-apps://itunes.apple.com/app/idAPP_ID";
NSString *templateReviewURL = @"itms-apps://ax.itunes.apple.com/WebObjects/MZStore.woa/wa/viewContentsUserReviews?type=Purple+Software&id=APP_ID";

+ (id)sharedInstance {
    
    if (_sharedInstance == nil)  {
        _sharedInstance = [[self alloc] init];
    }
    
    return _sharedInstance;
}

-(void) redirectToRatigPage:(NSString *)appId {
#if TARGET_IPHONE_SIMULATOR
    NSLog(@"APPIRATER NOTE: iTunes App Store is not supported on the iOS simulator. Unable to open App Store page.");
#else
    
    
    NSString *reviewURL;
    NSArray *vComp = [[UIDevice currentDevice].systemVersion componentsSeparatedByString:@"."];
    
    
    if ([[vComp objectAtIndex:0] intValue] >= 7) {
        reviewURL = [templateReviewURLIOS7 stringByReplacingOccurrencesOfString:@"APP_ID" withString:[NSString stringWithFormat:@"%@", appId]];
    }  else {
        reviewURL = [templateReviewURL stringByReplacingOccurrencesOfString:@"APP_ID" withString:[NSString stringWithFormat:@"%@", appId]];
    }
    
    NSLog(@"redirecting to iTunes page, IOS version: %i", [[vComp objectAtIndex:0] intValue]);
    NSLog(@"redirect URL: %@", reviewURL);
    
    
	
    [[UIApplication sharedApplication] openURL:[NSURL URLWithString:reviewURL]];
#endif
}




- (void) ShowSpiner {
    
    [[UIApplication sharedApplication] beginIgnoringInteractionEvents];
    
    if([self spinner] != nil) {
        return;
    }
    
    UIViewController *vc =  UnityGetGLViewController();
    
    
    [self setSpinner:[[UIActivityIndicatorView alloc] initWithActivityIndicatorStyle:UIActivityIndicatorViewStyleWhiteLarge]];

    
    [[UIDevice currentDevice] beginGeneratingDeviceOrientationNotifications];

    NSArray *vComp = [[UIDevice currentDevice].systemVersion componentsSeparatedByString:@"."];
    if ([[vComp objectAtIndex:0] intValue] >= 8) {
        NSLog(@"IOS 8 detected");
        [[self spinner] setFrame:CGRectMake(0,0, vc.view.frame.size.width, vc.view.frame.size.height)];
    } else {
        if([[UIDevice currentDevice] orientation] == UIDeviceOrientationPortrait || [[UIDevice currentDevice] orientation] == UIDeviceOrientationPortraitUpsideDown) {
            NSLog(@"portarait detected");
            [[self spinner] setFrame:CGRectMake(0,0, vc.view.frame.size.width, vc.view.frame.size.height)];
            
        } else {
            NSLog(@"landscape detected");
            [[self spinner] setFrame:CGRectMake(0,0, vc.view.frame.size.height, vc.view.frame.size.width)];
        }

    }
    
  
    
  
    
    [self spinner].opaque = NO;
    [self spinner].backgroundColor = [UIColor colorWithWhite:0.0f alpha:0.0f];
    
    
    [UIView animateWithDuration:0.8 animations:^{
        [self spinner].backgroundColor = [UIColor colorWithWhite:0.0f alpha:0.5f];
    }];
   
    
    
     
     [vc.view addSubview:[self spinner]];
     [[self spinner] startAnimating];
    
  //  [[self spinner] retain];

}

- (void) HideSpiner {
    
    if([self spinner] != nil) {
        [[UIApplication sharedApplication] endIgnoringInteractionEvents];
        
        [self spinner].backgroundColor = [UIColor colorWithWhite:0.0f alpha:0.5f];
        [UIView animateWithDuration:0.8 animations:^{
            [self spinner].backgroundColor = [UIColor colorWithWhite:0.0f alpha:0.0f];

        } completion:^(BOOL finished) {
            [[self spinner] removeFromSuperview];
            [[self spinner] release];
            [self setSpinner:nil];
        }];
        
       
    }
    
    
}


extern "C" {
    
    
    //--------------------------------------
	//  IOS Native Plugin Section
	//--------------------------------------
    
    void _ISN_RedirectToAppStoreRatingPage(char* appId) {
        [[IOSNativeUtility sharedInstance] redirectToRatigPage: [ISNDataConvertor charToNSString:appId ]];
    }
    
    
    void _ISN_ShowPreloader() {
        [[IOSNativeUtility sharedInstance] ShowSpiner];
    }
    
    
    void _ISN_HidePreloader() {
        [[IOSNativeUtility sharedInstance] HideSpiner];
    }
    
    
    //--------------------------------------
	//  Native PopUps Plugin Section
	//--------------------------------------
    
    
    void _MNP_RedirectToAppStoreRatingPage(char* appId) {
        _ISN_RedirectToAppStoreRatingPage(appId);
    }
    
    
    void _MNP_ShowPreloader() {
        _ISN_ShowPreloader();
    }
    
    
    void _MNP_HidePreloader() {
        _ISN_HidePreloader();
    }
    
    
}
@end
