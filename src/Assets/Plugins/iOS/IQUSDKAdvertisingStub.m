#import <Foundation/Foundation.h>

#pragma mark INTERFACE

/**
  IQUSDKAdvertisingStub uses reflection to try to obtain the properties from ASIdentifierManager.
*/
@interface IQUSDKAdvertisingStub : NSObject

#pragma mark Public methods

/**
  Returns the advertising identifier or empty string if none could be determined.
 
  @return advertising identifier or @""
*/
+ (NSString*)advertisingIdentifier;

/**
   Returns the advertising tracking enabled value; if the value could not be obtained the method will return true.
 
   @return advertising tracking enabled or true if none could be determined.
*/
+ (bool)isAdvertisingTrackingEnabled;

@end

#pragma mark IMPLEMENTATION

@implementation IQUSDKAdvertisingStub

#pragma mark Private variables

/**
  Contains the advertising identifier.
*/
static NSString* m_advertisingIdentifier = nil;

/**
  Contains the advertising tracking enabled.
*/
static bool m_isAdvertisingTrackingEnabled;

#pragma mark Private methods

/**
  Tries to obtain the identifier and tracking enabled. This method only tries it once, if it fails it will set
  the identifier to empty string and tracking enabled to true.
*/
+ (void)initialize {
  if (m_advertisingIdentifier != nil) {
    return;
  }
  // set default values (in case classes or methods fail)
  m_advertisingIdentifier = @"";
  m_isAdvertisingTrackingEnabled = true;
  // get ASIdentifierManager class
  Class identifierManager = NSClassFromString(@"ASIdentifierManager");
  if (identifierManager != nil) {
    // sharedManager = [ASIdentifierManager sharedManager]
    SEL sharedManagerSel = NSSelectorFromString(@"sharedManager");
    IMP sharedManagerImp = [identifierManager methodForSelector:sharedManagerSel];
    id sharedManager = ((id (*)(id, SEL))sharedManagerImp)(identifierManager, sharedManagerSel);
    if (sharedManager != nil) {
     // call [sharedManager isAdvertisingTrackingEnabled]
      SEL advertisingEnabledSel = NSSelectorFromString(@"isAdvertisingTrackingEnabled");
      IMP advertisingEnabledImp = [sharedManager methodForSelector:advertisingEnabledSel];
      m_isAdvertisingTrackingEnabled =
          (bool)((BOOL (*)(id, SEL))advertisingEnabledImp)(sharedManager, advertisingEnabledSel);
      // call [sharedManager advertisingIdentifier] and store result as string
      SEL advertisingIdentifierSel = NSSelectorFromString(@"advertisingIdentifier");
      IMP advertisingIdentifierImp = [sharedManager methodForSelector:advertisingIdentifierSel];
      NSUUID* uuid = ((NSUUID* (*)(id, SEL))advertisingIdentifierImp)(sharedManager, advertisingIdentifierSel);
      m_advertisingIdentifier = uuid.UUIDString;
    }
  }
}

#pragma mark Public methods

/**
  Implements the advertigingIdentifier method.
*/
+ (NSString*)advertisingIdentifier {
  [self initialize];
  return m_advertisingIdentifier;
}

/**
  Implements the isAdvertisingTrackingEnabled method.
*/
+ (bool)isAdvertisingTrackingEnabled {
  [self initialize];
  return m_isAdvertisingTrackingEnabled;
}

@end
