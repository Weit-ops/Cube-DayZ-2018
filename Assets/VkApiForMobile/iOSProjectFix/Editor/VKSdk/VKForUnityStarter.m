//
//  Starter.m
//  Unity-iPhone
//
//  Created by Admin on 25.05.15.
//
//

#import "VKForUnityStarter.h"
#import "VKSdk/VKSdk.h"

@interface VKForUnityStarter ()<UIAlertViewDelegate>
  
@end

@implementation VKForUnityStarter

- (void)vkSdkNeedCaptchaEnter:(VKError *)captchaError{
    VKCaptchaViewController *vc = [VKCaptchaViewController captchaControllerWithError:captchaError];
    [vc presentIn:UnityGetGLViewController()];
}

- (void)vkSdkTokenHasExpired:(VKAccessToken *)expiredToken{
    
}

- (void)vkSdkUserDeniedAccess:(VKError *)authorizationError{
    NSString* unityMessage=[NSString stringWithFormat:@"%ld/%@/%@", (long)authorizationError.errorCode, @"#",  authorizationError.errorMessage];
    UnitySendMessage("VkApi", "AccessDeniedMessage",[unityMessage cStringUsingEncoding:NSASCIIStringEncoding]);
    [self release];
}
- (void)vkSdkShouldPresentViewController:(UIViewController *)controller{
    [UnityGetGLViewController() presentViewController:controller animated:YES completion:nil];
}

- (void)vkSdkReceivedNewToken:(VKAccessToken *)newToken{
    [self parseAccessTokenAndSendToUnity:newToken];
}

- (void)vkSdkAcceptedUserToken:(VKAccessToken *)token{
    [self parseAccessTokenAndSendToUnity:token];
}

- (void)vkSdkRenewedToken:(VKAccessToken *)newToken{
    [self parseAccessTokenAndSendToUnity:newToken];
}

-(void)alertView:(UIAlertView *)alertView didDismissWithButtonIndex:(NSInteger)buttonIndex {
}

-(void)autorize:(const char*) authUrl{
    
    NSMutableDictionary* queryStringDictionary=[VKForUnityStarter parseUrl:authUrl];
    
    NSString *appid=[queryStringDictionary objectForKey:@"client_id"];
    NSArray *scope=[[queryStringDictionary objectForKey:@"scope"] componentsSeparatedByString:@","];
    BOOL revokeAccess=[[queryStringDictionary objectForKey:@"revokeAccess"]boolValue];
    BOOL forceOAtuh=[[queryStringDictionary objectForKey:@"forceOAtuh"]boolValue];
    
    //[VKSdk initializeWithDelegate:self andAppId:appid];
    
    [VKSdk authorize:scope revokeAccess:revokeAccess forceOAuth:forceOAtuh];
}

- (void)parseAccessTokenAndSendToUnity:(VKAccessToken*)token {
    VKAccessToken* myaccesstoken=token;
    
    NSString* accesstoken=[myaccesstoken accessToken];
    NSString* userid= [myaccesstoken userId];
    NSString* expiresIn= [myaccesstoken expiresIn];
    NSString* unityMessage=[NSString stringWithFormat:@"%@%@%@%@%@%@%@", accesstoken, @"#", @"playg",@"#",expiresIn,@"#",userid];
    UnitySendMessage("VkApi", "ReceiveNewTokenMessage",[unityMessage cStringUsingEncoding:NSASCIIStringEncoding]);
    [self release];
    
}
-(void)dealloc{
    [super dealloc];
}

+(NSMutableDictionary*)parseUrl:(const char*) authUrl{
    NSMutableDictionary *queryStringDictionary = [[NSMutableDictionary alloc] init];
    
    NSString * urlstring=[NSString stringWithUTF8String:authUrl];
    
    NSArray* preUrlparams= [urlstring componentsSeparatedByString:@"?"];
    NSArray *urlparams = [[preUrlparams objectAtIndex:1] componentsSeparatedByString:@"&"];
    
    for (NSString *keyValuePair in urlparams)
    {
        NSArray *pairComponents = [keyValuePair componentsSeparatedByString:@"="];
        NSString *key = [[pairComponents firstObject] stringByRemovingPercentEncoding];
        NSString *value = [[pairComponents lastObject] stringByRemovingPercentEncoding];
        
        
        [queryStringDictionary setObject:value forKey:key];
    }
    return queryStringDictionary;
}
@end
void _VkAuthorization(const char* authUrl){
    NSMutableDictionary*dict=[VKForUnityStarter parseUrl:authUrl];
    VKForUnityStarter* sdkHandler=[[VKForUnityStarter alloc] init];
    
    [VKSdk initializeWithDelegate:sdkHandler andAppId:[dict objectForKey:@"client_id"]];
    [sdkHandler autorize:authUrl];
    
}
void _doLogOutUser(){
   
}
