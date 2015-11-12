#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

#pragma mark INTERFACE

/**
  IQUSDKAdvertising uses reflection to try to obtain the properties from IQUSDKAdvertigingStub.
*/
@interface IQUSDKAdvertising : NSObject

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

/**
   Returns the vendor id or unique device id if the iOS version does not support vendor ids.
 
   @return advertising tracking enabled or true if none could be determined.
*/
+ (NSString*)vendorIdentifier;

@end

#pragma mark IMPLEMENTATION

@implementation IQUSDKAdvertising

#pragma mark Private static variables

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
  // get IQUSDKAdvertisingStub class
  Class advertisingStub = NSClassFromString(@"IQUSDKAdvertisingStub");
  if (advertisingStub != nil) {
    // call [advertisingStub isAdvertisingTrackingEnabled]
    SEL advertisingEnabledSel = NSSelectorFromString(@"isAdvertisingTrackingEnabled");
    IMP advertisingEnabledImp = [advertisingStub methodForSelector:advertisingEnabledSel];
    m_isAdvertisingTrackingEnabled =
        (bool)((BOOL (*)(id, SEL))advertisingEnabledImp)(advertisingStub, advertisingEnabledSel);
    // call [advertisingStub advertisingIdentifier]
    SEL advertisingIdentifierSel = NSSelectorFromString(@"advertisingIdentifier");
    IMP advertisingIdentifierImp = [advertisingStub methodForSelector:advertisingIdentifierSel];
    m_advertisingIdentifier = ((NSString*  (*)(id, SEL))advertisingIdentifierImp)(advertisingStub, advertisingIdentifierSel);
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

/**
  Implements the vendorIdentifier method.
*/
+ (NSString*)vendorIdentifier {
  NSString* uuid;
  // use vendor uuid if it is available
  if ([[UIDevice currentDevice] respondsToSelector:NSSelectorFromString(@"identifierForVendor")]) {
    // get vendor uuid as string
    uuid = [UIDevice currentDevice].identifierForVendor.UUIDString;
  } else {
    // vendor id not available, use [UIDevice currentDevice].uniqueIdentifier instead
    UIDevice* currentDevice = [UIDevice currentDevice];
    SEL uniqueIdentifierSel = NSSelectorFromString(@"uniqueIdentifier");
    IMP uniqueIdentifierImp = [currentDevice methodForSelector:uniqueIdentifierSel];
    uuid = ((NSString*  (*)(id, SEL))uniqueIdentifierImp)(currentDevice, uniqueIdentifierSel);
  }
  return uuid;
}

@end

#pragma mark Support functions

/**
  Helper method to create C string copy in memory (instead of the heap)
 
  @param aText Text to make a copy off
 
  @return a copy of text in memory
*/
char* MakeStringCopy (const char* aText)
{
  // NULL strings just stay NULL
  if (aText == NULL)
    return NULL;
  // allocate memory and copy string
  char* result = (char*)malloc(strlen(aText) + 1);
  strcpy(result, aText);
  return result;
}

#pragma mark Plugin functions

/**
  Returns the vendor id or unique device id in case of older iOS versions.
 
  @return vendor id or unique device id
*/
char* IQUSDKVendorIdentifier() {
  return MakeStringCopy([[IQUSDKAdvertising vendorIdentifier] UTF8String]);
}

/**
  Returns the advertising identifier or empty string if none could be found.
 
  @return identifier or empty string.
*/
char* IQUSDKAdvertisingIdentifier() {
  return MakeStringCopy([[IQUSDKAdvertising advertisingIdentifier] UTF8String]);
}

/**
  Returns if tracking is enabled.
 
  @return the tracking enabled state.
*/
bool IQUSDKIsAdvertisingTrackingEnabled() {
  return [IQUSDKAdvertising isAdvertisingTrackingEnabled];
}
