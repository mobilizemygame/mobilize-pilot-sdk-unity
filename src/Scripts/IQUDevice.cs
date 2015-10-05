using System;
using UnityEngine; 
#if UNITY_IOS && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif

namespace IQU.SDK
{
  /// <summary>
  /// IQUDevice encapsulates device related actions, acting as an interface to the native plugin code.
  /// </summary>
  internal class IQUDevice
  {
    #region Private vars

#if UNITY_ANDROID && !UNITY_EDITOR
    
    /// <summary>
    /// Reference to java's class
    /// </summary>
    private AndroidJavaClass m_androidIds;
    
#endif

#if UNITY_IOS && !UNITY_EDITOR

    /// <summary>
    /// Reference to objective c's method
    /// </summary>
    [DllImport ("__Internal")]
    private static extern bool IQUSDKIsAdvertisingTrackingEnabled();

    /// <summary>
    /// Reference to objective c's method
    /// </summary>
    [DllImport ("__Internal")]
    private static extern string IQUSDKAdvertisingIdentifier();

    /// <summary>
    /// Reference to objective c's method
    /// </summary>
    [DllImport ("__Internal")]
    private static extern string IQUSDKVendorIdentifier();
    
#endif

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="IQU.SDK.IQUDevice"/> class.
    /// </summary>
    internal IQUDevice()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
#elif UNITY_IOS && !UNITY_EDITOR
      this.AdTrackingEnabled = true;
      this.AdvertisingId = "";
      this.VendorId = "";
#else
      this.Done = false;
#endif
    }

    #endregion

    #region Internal methods

    /// <summary>
    /// Initializes the device. The Done property will be set to true once initialization has finished.
    /// </summary>
    internal void Initialize() 
    {
#if UNITY_ANDROID && !UNITY_EDITOR
      // get context
      AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
      AndroidJavaObject applicationContext = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
      // start retrieval of advertisement id
      this.m_androidIds = new AndroidJavaClass("com.iqu.sdk.unity.android.AndroidIds");
      this.m_androidIds.CallStatic("start", applicationContext);
#elif UNITY_IOS && !UNITY_EDITOR
      // get ids from plugin
      this.AdTrackingEnabled = IQUSDKIsAdvertisingTrackingEnabled();
      this.AdvertisingId = IQUSDKAdvertisingIdentifier();
      this.VendorId = IQUSDKVendorIdentifier();
      this.Done = true;
#else

      this.Done = true;
#endif
    }

    /// <summary>
    /// Clears any references and used resources.
    /// </summary>
    internal void Destroy() 
    {
#if UNITY_ANDROID && !UNITY_EDITOR
      this.m_androidIds = null;
#endif
    }

    #endregion

    #region Internal properties

#if UNITY_ANDROID && !UNITY_EDITOR 

    /// <summary>
    /// The Done property is set true once the device has finished initialization.
    /// </summary>
    internal bool Done 
    { 
      get 
      {
        return (this.m_androidIds == null) ? false : this.m_androidIds.CallStatic<int>("getAdvertisingResult") >= 0;
      }
    }

    /// <summary>
    /// This property is true when limited ad tracking is disabled. If the limited ad tracking could not be 
    /// determined this property will return true.
    /// </summary>
    internal bool AdTrackingEnabled 
    { 
      get 
      {
        return (this.m_androidIds == null) ? true : this.m_androidIds.CallStatic<int>("getLimitedAdTracking") == 0;
      }
    }

    /// <summary>
    /// The advertising id or empty string if it could not be obtained.
    /// </summary>
    internal string AdvertisingId 
    {
      get 
      {
        return (this.m_androidIds == null) ? "" : this.m_androidIds.CallStatic<string>("getAdvertisingId");
      }
    }

    /// <summary>
    /// This property contains the value from Settings.Secure.ANDROID_ID
    /// </summary>
    internal string AndroidId
    {
      get 
      {
        return (this.m_androidIds == null) ? "" : this.m_androidIds.CallStatic<string>("getAndroidId");
      }
    }

    /// <summary>
    /// This property contains the value from android.os.Build.SERIAL
    /// </summary>
    internal string AndroidSerial 
    {
      get 
      {
        return (this.m_androidIds == null) ? "" : this.m_androidIds.CallStatic<string>("getSerial");
      }
    }
    
#elif UNITY_IOS && !UNITY_EDITOR 

    /// <summary>
    /// The Done property is set true once the device has finished initialization.
    /// </summary>
    internal bool Done { get; private set; }
    
    /// <summary>
    /// This property is true when limited ad tracking is disabled. If the limited ad tracking could not be 
    /// determined this property will return true.
    /// </summary>
    internal bool AdTrackingEnabled { get; private set; }
    
    /// <summary>
    /// The advertising id or empty string if it could not be obtained.
    /// </summary>
    internal string AdvertisingId { get; private set; }

    /// <summary>
    /// This property contains the vendor ID or the device id in case the iOS version 
    /// does not support the vendor id.
    /// </summary>
    internal string VendorId { get; private set; }

#else

    /// <summary>
    /// The Done property is set true once the device has finished initialization.
    /// </summary>
    internal bool Done { get; private set; }

#endif

    #endregion
  }
}
