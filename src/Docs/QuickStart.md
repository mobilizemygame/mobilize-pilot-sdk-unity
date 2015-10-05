@mainpage %IQU %SDK for Unity 3D

@section introduction Introduction
%IQU is a class that encapsulates the %IQU %SDK and offers various methods and properties to communicate with the %IQU server.


@section quickstart Quick usage guide

1. Methods and properties can be accessed trough the static IQU.SDK.IQUSDK.Instance method.
2. Either call IQU.SDK.IQUSDK.Start to start the %IQU %SDK or add the IQUSDKComponent from the /Assets/IQU/SDK/Components folder to one of the root
   game objects and fillin the various fields. The component will start the %SDK automatically.
3. Add additional Ids via IQU.SDK.IQUSDK.SetFacebookId, IQU.SDK.IQUSDK.SetGooglePlusId, IQU.SDK.IQUSDK.SetTwitterId or IQU.SDK.IQUSDK.SetCustomId.
4. Start calling analytic tracking methods to send messages to the server.
5. Update the IQU.SDK.IQUSDK.Payable property to indicate the player is busy with a payable action.
6. IMPORTANT: When using a stripping level other then disabled, copy (or move) the link.xml file from the /Assets/IQU/SDK/Docs/ 
   to the /Assets (root) folder of your project.


@section network Network communication

The %IQU %SDK uses a separate thread to send messages to the server (to prevent blocking the main thread). This means that there might be a small delay
before messages are actually sent to the server. The maximum delay is determined by the IQU.SDK.IQUSDK.UpdateInterval property.

If the %SDK fails to send a message to the %IQU server, messages are queued and are sent when the server is available again. The queued messages are stored
in persistent storage so they still can be resent after an application restart.


@section ids Ids

The %SDK supports various ids which are included with every tracking message sent to the server. See IQU.SDK.IQUIdType for the types supported
by the SDK. Use IQU.SDK.IQUSDK.SetId to get an id value.

Some ids are determined by the %SDK itself, other ids must be set via one of
the following methods: IQU.SDK.IQUSDK.SetFacebookId, IQU.SDK.IQUSDK.SetGooglePlusId, IQU.SDK.IQUSDK.SetTwitterId or
IQU.SDK.IQUSDK.SetCustomId

On Android and iOS devices the %SDK will try to obtain the advertising id and limited ad tracking. The %SDK contains small plugins to
obtain these values. If limited ad tracking is enabled, the %SDK will disable the tracking messages. Calling any of the tracking
methods will do nothing.

For on Android devices, the %SDK requires Google Play libraries to obtain these values. The %SDK checks for the existence of the
Google Play classes and uses reflection to call the Google Play services so there are no errors when the Google Play libraries
are not included within the application.

On iOS the %SDK also checks for the required classes and uses reflection to obtain the values so there are no errors when running 
the application on iOS versions that don't support the advertising ID.

To remove support for the advertising id on iOS and any reference to the classes needed to obtain the advertising id, 
the following file can be deleted: /Assets/Plugin/iOS/IQUSDKAdvertisingStub.m


@section information Informational properties

The %IQU %SDK offers the following informational properties:

1. IQU.SDK.IQUSDK.AnalyticsEnabled indicates if the %IQU %SDK analytics part is enabled. When disabled the tracking methods will do nothing.
   The analytics part is disabled when the user enabled limited ad tracking with the Google Play services or on iOS.
2. IQU.SDK.IQUSDK.ServerAvailable to get information if the messages were sent successfully or not.


@section testing Testing

The %IQU %SDK contains the following properties to help with testing the SDK:
1. IQU.SDK.IQUSDK.LogEnabled property to turn logging on or off.
2. IQU.SDK.IQUSDK.Log property which will be filled with messages from various methods.
3. IQU.SDK.IQUSDK.TestMode property to test the %SDK without any server interaction or to simulate an offline situation 
   with the server not being available.
  
To turn on debug messages from various classes DEBUG needs to be defined when building the application. When testing 
from within the editor, debug messages are always generated.
   

@section timing Advance timing

The IQU.SDK.IQUSDK.CheckServerInterval property determines the time between checks for server availability. 
If sending of data fails, the update method will wait the time, as set by this property, before trying to send the data again.
