=======
# mobilize-pilot-sdk-unity
Mobilize Pilot Unity SDK

## Introduction

`IQUSDK` is a class that encapsulates the IQU SDK and offers various methods and properties to communicate with the IQU server.

## Installation

#### Install with package file

1. Download the *IQU.SDK.unity.vX.X.X.unitypackage* file (located in the root of this repository).
2. Open an existing project or start a new project.
3. Either double click *IQU.SDK.unity.vX.X.X.unitypackage* or use _Assets > Import Package > Custom Package..._ to open the file.
4. **Warning**: the package includes a version of the Google Play services for Android; if this is not required uncheck the *google-play-services_lib* folder in the */Plugins/Android/* folder before importing the files.

#### Install with source

1. Clone the repository or download the zip.
2. Copy all files from the *src/Assets/* folder to the */Assets/* folder of the project.
3. To add Google Play Service support, copy the *google-play-services_lib* folder from */{Android SDK root}/extras/google/google_play_services/libproject/* to */Assets/Plugins/Android/*

#### Finish installation

1. **Important(!)**: when using a stripping level other then disabled, copy (or move) the *link.xml* file from the */Assets/IQU/SDK/Docs/* to the */Assets/* (root) folder of your project. This will prevent the compiler from removing security related classes that are referenced indirectly.
2. To remove support for the advertising id on iOS and any reference to the classes needed to obtain the advertising id, delete the following file: */Assets/Plugin/iOS/IQUSDKAdvertisingStub.m*

The */Assets/IQU/SDK/Examples* folder contains an example scene file and c# script file.

For help open the */Assets/IQU/SDK/Docs/reference.chm* file or unzip the */Assets/IQU/SDK/Docs/reference.html.zip* somewhere outside the project folders (the zip file also contains several support javascript files which generate errors/warnings from Unity3D when placed inside the project folder).

## Quick usage guide

1. Methods and properties can be accessed trough the static `IQUSDK.Instance` property.
2. Either call `IQUSDK.Instance.Start` to start the IQU SDK or add the `IQUSDKComponent` from the */Assets/IQU/SDK/Components* folder to one of the root game objects and fill-in the various fields. The component will start the SDK automatically.
3. Add additional Ids via `IQUSDK.Instance.SetFacebookId`, `IQUSDK.Instance.SetGooglePlusId`, `IQUSDK.Instance.SetTwitterId` or `IQUSDK.Instance.SetCustomId`.
4. Start calling analytic tracking methods to send messages to the server.
5. Update the `IQUSDK.Instance.Payable` property to indicate the player is busy with a payable action.
6. IMPORTANT: When using a stripping level other then disabled, copy (or move) the `link.xml` file from the */Assets/IQU/SDK/Docs/* to the */Assets* (root) folder of your project.

## Network communication

The IQU SDK will only use a single communication action with the server at the time. So when the SDK is busy sending messages, new tracking messages will be placed in a queue. The queued messages will be processed and sent once the previous sending action has finished. This means that sometimes there might be a delay between the tracking call and the actual sending of the tracking message.

If the SDK fails to send a message to the IQU server, messages are queued and are sent when the server is available again. 

Queued messages are stored in persistent storage so they still can be resent after an application restart.

## Ids

The SDK supports various ids which are included with every tracking message sent to the server. See `IQUIdType` for the types supported by the SDK. Use `IQUSDK.Instance.SetId` to get an id value.

Some ids are determined by the SDK itself, other ids must be set via one of
the following methods: `IQUSDK.Instance.SetFacebookId`, `IQUSDK.Instance.SetGooglePlusId`, `IQUSDK.Instance.SetTwitterId` or `IQUSDK.Instance.SetCustomId`

On Android and iOS devices the SDK will try to obtain the advertising id and limited ad tracking. The SDK contains small plugins to obtain these values. If limited ad tracking is enabled, the SDK will disable the tracking messages. Calling any of the tracking methods will do nothing.

For on Android devices, the SDK requires Google Play libraries to obtain these values. The SDK checks for the existence of the Google Play classes and uses reflection to call the Google Play services so there are no errors when the Google Play libraries are not included within the application.

On iOS the SDK also checks for the required classes and uses reflection to obtain the values so there are no errors when running the application on iOS versions that don't support the advertising ID.

To remove support for the advertising id on iOS and any reference to the classes needed to obtain the advertising id, the following file can be deleted: */Assets/Plugin/iOS/IQUSDK.Instance.dvertisingStub.m*

## Informational properties

The IQU SDK offers the following informational properties:

1. `IQUSDK.Instance.AnalyticsEnabled` indicates if the IQU SDK analytics part is enabled. When disabled the tracking methods will do nothing. The analytics part is disabled when the user enabled limited ad tracking with the Google Play services or on iOS.
2. `IQUSDK.Instance.ServerAvailable` to get information if the messages were sent successfully or not.

## Testing

The IQU SDK contains the following properties to help with testing the SDK:

1. `IQUSDK.Instance.LogEnabled` property to turn logging on or off.
2. `IQUSDK.Instance.Log` property which will be filled with messages from various methods.
3. `IQUSDK.Instance.TestMode` property to test the SDK without any server interaction or to simulate an offline situation with the server not being available.
  
To turn on debug messages from various classes `DEBUG` needs to be defined when building the application. When testing from within the editor, debug messages are always generated.
  
## Advance timing

The `IQUSDK.Instance.CheckServerInterval` property determines the time between checks for server availability. If sending of data fails, the update method will wait the time, as set by this property, before trying to send the data again.
