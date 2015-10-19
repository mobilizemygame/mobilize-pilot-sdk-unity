using System;
using System.Collections.Generic;
using System.Diagnostics; 
using UnityEngine;

namespace IQU.SDK
{
  /// <summary>
  /// IQUSDK offers methods and properties to use the analytics service of %IQU.
  /// </summary>
  public class IQUSDK
  {
  #region Public consts
  
    /// <summary>
    /// %SDK version.
    /// </summary>
    public const string SdkVersion = "1.0.2";
  
    /// <summary>
    /// %SDK type. 
    /// </summary>
    public const string SdkType = "Unity";

  #endregion

    #region Private consts & types

    /// <summary>
    /// Local storage key for the the %SDK id.
    /// </summary>
    private const string SDKIdKey = "IQU_SDK_ID";

    /// <summary>
    /// Initial update interval value
    /// </summary>
    private const long DefaultUpdateInterval = 200;

    /// <summary>
    /// Initial interval in milliseconds between server available checks
    /// </summary>
    private const long DefaultCheckServerInterval = 2000;

    /// <summary>
    /// Interval in milliseconds between heartbeat messages
    /// </summary>
    private const long HeartbeatInterval = 60000;

    /// <summary>
    /// Event type values.
    /// </summary>
    private const string EventRevenue = "revenue";
    private const string EventHeartbeat = "heartbeat";
    private const string EventItemPurchase = "item_purchase";
    private const string EventTutorial = "tutorial";
    private const string EventMilestone = "milestone";
    private const string EventMarketing = "marketing";
    private const string EventUserAttribute = "user_attribute";
    private const string EventCountry = "country";
    private const string EventPlatform = "platform";

    /// <summary>
    /// The different states this instance can be in
    /// </summary>
    private enum State
    {
      None,
      WaitForDevice,
      ProcessPending,
      WaitForSend
    }

    #endregion

    #region Private vars

    /// <summary>
    /// See property definition.
    /// </summary>
    private static IQUSDK m_instance;

    /// <summary>
    /// See property definition.
    /// </summary>
    private bool m_initialized;

    /// <summary>
    /// See property definition.
    /// </summary>
    private bool m_logEnabled;

    /// <summary>
    /// See property definition.
    /// </summary>
    private string m_log;

    /// <summary>
    /// See property definition.
    /// </summary>
    private IQUTestMode m_testMode;

    /// <summary>
    /// See property definition.
    /// </summary>
    private bool m_serverAvailable;

    /// <summary>
    /// See property definition.
    /// </summary>
    private bool m_analyticsEnabled;

    /// <summary>
    /// See property definition.
    /// </summary>
    private long m_checkServerInterval;

    /// <summary>
    /// See property definition.
    /// </summary>
    private bool m_payable;

    /// <summary>
    /// Contains the various ids
    /// </summary>
    private IQUIds m_ids;

    /// <summary>
    /// Network part of %IQU %SDK.
    /// </summary>
    private IQUNetwork m_network;

    /// <summary>
    /// Local storage part of %IQU %SDK.
    /// </summary>
    private IQULocalStorage m_localStorage;

    /// <summary>
    /// Contains messages that are pending to be sent.
    /// </summary>
    private IQUMessageQueue m_pendingMessages;

    /// <summary>
    /// Contains messages currently being sent.
    /// </summary>
    private IQUMessageQueue m_sendingMessages;

    /// <summary>
    /// Time before a new server check is allowed.
    /// </summary>
    private long m_checkServerTime;

    /// <summary>
    /// Time of last heartbeat message.
    /// </summary>
    private long m_heartbeatTime;

    /// <summary>
    /// See property definition.
    /// </summary>
    private IQUHelper m_helper;

    /// <summary>
    /// See property definition.
    /// </summary>
    private GameObject m_gameObject;

    /// <summary>
    /// Device instance.
    /// </summary>
    private IQUDevice m_device;

    /// <summary>
    /// The current state the %IQU %SDK is in
    /// </summary>
    private State m_state;

    /// <summary>
    /// Will be set to true if the application becomes paused.
    /// </summary>
    private bool m_paused;

    #endregion

    #region Constructors

    /// <summary>
    /// Creates the instance and initializes all private variables.
    /// </summary>
    private IQUSDK()
    {
      this.m_checkServerInterval = IQUSDK.DefaultCheckServerInterval;
      this.m_checkServerTime = -IQUSDK.DefaultCheckServerInterval;
      this.m_device = null;
      this.m_analyticsEnabled = true;
      this.m_gameObject = null;
      this.m_heartbeatTime = -IQUSDK.HeartbeatInterval;
      this.m_helper = null;
      this.m_ids = new IQUIds();
      this.m_initialized = false;
      this.m_localStorage = null;
      this.m_log = "";
#if DEBUG || UNITY_EDITOR
      this.m_logEnabled = true;
#else
      this.m_logEnabled = false;
#endif
      this.m_network = null;
      this.m_paused = false;
      this.m_payable = true;
      this.m_pendingMessages = null;
      this.m_sendingMessages = null;
      this.m_serverAvailable = true;
      this.m_state = State.None;
      this.m_testMode = IQUTestMode.None;
    }

    #endregion

    #region Public methods

    /// <summary>
    /// Calls Start(string anApiKey, string aSecretKey, bool aPayable) with <c>true</c> for payable parameter.
    /// 
    /// <param name="anApiKey">API key</param>
    /// <param name="aSecretKey">Secret key</param>
    /// </summary>
    public void Start(string anApiKey, string aSecretKey)
    {
      this.Start(anApiKey, aSecretKey, true);
    }

    /// <summary>
    /// Starts the %SDK using and sets the Payable property to the specified value.
    ///
    /// If the %SDK is already started, another call to this method will be ignored.
    /// 
    /// <param name="anApiKey">API key</param>
    /// <param name="aSecretKey">Secret key</param>
    /// <param name="aPayable">Initial payable value</param>
    /// </summary>
    public void Start(string anApiKey, string aSecretKey, bool aPayable)
    {
      this.Initialize(anApiKey, aSecretKey, aPayable);
    }

    /// <summary>
    /// Calls Start(string anApiKey, string aSecretKey, string aCustomId, bool aPayable) with <c>true</c> for payable parameter.
    /// 
    /// <param name="anApiKey">API key</param>
    /// <param name="aSecretKey">Secret key</param>
    /// <param name="aCustomId">A custom id that the %SDK should use.</param>
    /// </summary>
    public void Start(string anApiKey, string aSecretKey, string aCustomId)
    {
      this.Start(anApiKey, aSecretKey, aCustomId, true);
    }

    /// <summary>
    /// Calls Start(string anApiKey, string aSecretKey) and then calls SetCustomId(string anId) to set the custom id.
    ///
    /// If the %SDK is already started, another call to this method will only update the custom id.
    /// 
    /// <param name="anApiKey">API key</param>
    /// <param name="aSecretKey">Secret key</param>
    /// <param name="aCustomId">A custom id that the %SDK should use.</param>
    /// <param name="aPayable">Initial payable value</param>
    /// </summary>
    public void Start(string anApiKey, string aSecretKey, string aCustomId, bool aPayable)
    {
      this.Start(anApiKey, aSecretKey, aPayable);
      this.SetCustomId(aCustomId);
    }

    #endregion

    #region Public id methods

    /// <summary>
    /// Return id for a certain type. If the id is not known (yet), the method
    /// will return an empty string.
    /// </summary>
    /// <returns>stored id value or empty string if it not (yet) known.</returns>
    /// <param name="aType">Type to get id for.</param>
    public string GetId(IQUIdType aType)
    {
      return this.m_ids.Get(aType);
    }

    /// <summary>
    /// Sets the Facebook id the %SDK should use.
    /// </summary>
    /// <param name="anId">Facebook id.</param>
    public void SetFacebookId(string anId)
    {
      this.SetId(IQUIdType.Facebook, anId);
    }

    /// <summary>
    /// Removes the current used Facebook id.
    /// </summary>
    public void ClearFacebookId()
    {
      this.SetId(IQUIdType.Facebook, "");
    }

    /// <summary>
    /// Sets the Google+ id the %SDK should use.
    /// </summary>
    /// <param name="anId">Google+ id.</param>
    public void SetGooglePlusId(string anId)
    {
      this.SetId(IQUIdType.GooglePlus, anId);
    }

    /// <summary>
    /// Removes the current used Google+ id.
    /// </summary>
    public void ClearGooglePlusId()
    {
      this.SetId(IQUIdType.GooglePlus, "");
    }

    /// <summary>
    /// Sets the Twitter id the %SDK should use.
    /// </summary>
    /// <param name="anId">Twitter id.</param>
    public void SetTwitterId(string anId)
    {
      this.SetId(IQUIdType.Twitter, anId);
    }

    /// <summary>
    /// Removes the current used Twitter id.
    /// </summary>
    public void ClearTwitterId()
    {
      this.SetId(IQUIdType.Twitter, "");
    }

    /// <summary>
    /// Sets the custom id the %SDK should use.
    /// </summary>
    /// <param name="anId">Custom id.</param>
    public void SetCustomId(string anId)
    {
      this.SetId(IQUIdType.Custom, anId);
    }

    /// <summary>
    /// Removes the current used custom id.
    /// </summary>
    public void ClearCustomId()
    {
      this.SetId(IQUIdType.Custom, "");
    }

    #endregion

    #region Public analytic methods

    /// <summary>
    /// Tracks payment made by the user.
    ///
    /// If the %IQU %SDK has not been initialized or IQUSDK.AnalyticsEnabled property is <c>false</c>, this method will do nothing.
    /// </summary>
    /// <param name="anAmount">Amount</param>
    /// <param name="aCurrency">Currency code (ISO 4217 standard)</param>
    /// <param name="aReward">Name of reward or null if there no such value</param>
    public void TrackRevenue(float anAmount, string aCurrency, string aReward)
    {
      // exit if not enabled
      if (!this.AnalyticsEnabled)
      {
        return;
      }
      Dictionary<string, object> trackEvent = this.CreateEvent(EventRevenue, true);
      trackEvent.Add("amount", anAmount);
      trackEvent.Add("currency", aCurrency);
      if (aReward != null)
      {
        trackEvent.Add("reward", aReward);
      }
      this.AddEvent(trackEvent);
    }

    /// <summary>
    /// Tracks revenue, just calls TrackRevenue(float anAmount, string aCurrency, string aReward) with null for aReward.
    ///
    /// If the %IQU %SDK has not been initialized or IQUSDK.AnalyticsEnabled property is <c>false</c>, this method will do nothing.
    /// </summary>
    /// <param name="anAmount">Amount</param>
    /// <param name="aCurrency">Currency code (ISO 4217 standard)</param>
    public void TrackRevenue(float anAmount, string aCurrency)
    {
      this.TrackRevenue(anAmount, aCurrency, null);
    }

    /// <summary>
    /// Tracks payment made by the user including an amount in a virtual currency.
    ///
    /// If the %IQU %SDK has not been initialized or IQUSDK.AnalyticsEnabled property is <c>false</c>, this method will do nothing.
    /// </summary>
    /// <param name="anAmount">Amount</param>
    /// <param name="aCurrency">Currency code (ISO 4217 standard)</param>
    /// <param name="aVirtualCurrencyAmount">Amount of virtual currency rewarded with this purchase</param>
    /// <param name="aReward">Name of reward or null if there no such value</param>
    public void TrackRevenue(float anAmount, string aCurrency, float aVirtualCurrencyAmount, string aReward)
    {
      // exit if not enabled
      if (!this.AnalyticsEnabled)
      {
        return;
      }
      Dictionary<string, object> trackEvent = this.CreateEvent(EventRevenue, true);
      trackEvent.Add("amount", anAmount);
      trackEvent.Add("currency", aCurrency);
      trackEvent.Add("vc_amount", aVirtualCurrencyAmount);
      if (aReward != null)
      {
        trackEvent.Add("reward", aReward);
      }
      this.AddEvent(trackEvent);
    }

    /// <summary>
    /// Tracks revenue, just calls TrackRevenue(float anAmount, string aCurrency, float aVirtualCurrencyAmount, string aReward) 
    /// with null for aReward.
    ///
    /// If the %IQU %SDK has not been initialized or IQUSDK.AnalyticsEnabled property is <c>false</c>, this method will do nothing.
    /// </summary>
    /// <param name="anAmount">Amount</param>
    /// <param name="aCurrency">Currency code (ISO 4217 standard)</param>
    /// <param name="aVirtualCurrencyAmount">Amount of virtual currency rewarded with this purchase</param>
    public void TrackRevenue(float anAmount, string aCurrency, float aVirtualCurrencyAmount)
    {
      this.TrackRevenue(anAmount, aCurrency, aVirtualCurrencyAmount, null);
    }

    /// <summary>
    /// Tracks an item purchase.
    ///
    /// If the %IQU %SDK has not been initialized or IQUSDK.AnalyticsEnabled property is <c>false</c>, this method will do nothing.
    /// </summary>
    /// <param name="aName">Name of item</param>
    public void TrackItemPurchase(string aName)
    {
      // exit if not enabled
      if (!this.AnalyticsEnabled)
      {
        return;
      }
      Dictionary<string, object> trackEvent = this.CreateEvent(EventItemPurchase, true);
      trackEvent.Add("name", aName);
      this.AddEvent(trackEvent);
    }

    /// <summary>
    /// Tracks an item purchase including amount in virtual currency.
    ///
    /// If the %IQU %SDK has not been initialized or IQUSDK.AnalyticsEnabled property is <c>false</c>, this method will do nothing.
    /// </summary>
    /// <param name="aName">Name of item</param>
    /// <param name="aVirtualCurrencyAmount">Amount of virtual currency rewarded with this purchase</param>
    public void TrackItemPurchase(string aName, float aVirtualCurrencyAmount)
    {
      // exit if not enabled
      if (!this.AnalyticsEnabled)
      {
        return;
      }
      Dictionary<string, object> trackEvent = this.CreateEvent(EventItemPurchase, true);
      trackEvent.Add("name", aName);
      trackEvent.Add("vc_amount", aVirtualCurrencyAmount);
      this.AddEvent(trackEvent);
    }

    /// <summary>
    /// Tracks tutorial progression achieved by the user.
    ///
    /// If the %IQU %SDK has not been initialized or IQUSDK.AnalyticsEnabled property is <c>false</c>, this method will do nothing.
    /// </summary>
    /// <param name="aStep">Step name or number of the tutorial.</param>
    public void TrackTutorial(string aStep)
    {
      // exit if not enabled
      if (!this.AnalyticsEnabled)
      {
        return;
      }
      Dictionary<string, object> trackEvent = this.CreateEvent(EventTutorial, true);
      trackEvent.Add("step", aStep);
      this.AddEvent(trackEvent);
    }

    /// <summary>
    /// Tracks a milestone achieved by the user, e.g. if the user achieved a level.
    ///
    /// If the %IQU %SDK has not been initialized or IQUSDK.AnalyticsEnabled property is <c>false</c>, this method will do nothing.
    /// </summary>
    /// <param name="aName">Milestone name</param>
    /// <param name="aValue">Value of the milestone</param>
    public void TrackMilestone(string aName, string aValue)
    {
      // exit if not enabled
      if (!this.AnalyticsEnabled)
      {
        return;
      }
      Dictionary<string, object> trackEvent = this.CreateEvent(EventMilestone, true);
      trackEvent.Add("name", aName);
      trackEvent.Add("value", aValue);
      this.AddEvent(trackEvent);
    }

    /// <summary>
    /// Tracks a marketing source. All parameters are optional, if a value is not known null must be used.
    ///
    /// If the %IQU %SDK has not been initialized or IQUSDK.AnalyticsEnabled property is <c>false</c>, this method will do nothing.
    /// </summary>
    /// <param name="aPartner">Marketing partner name or null if there is none.</param>
    /// <param name="aCampaign">Marketing campaign name or null if there is none.</param>
    /// <param name="anAd">Marketing ad name or null if there is none.</param>
    /// <param name="aSubId">Marketing partner sub id or null if there is none.</param>
    /// <param name="aSubSubId">Marketing partner sub sub id or null if there is none.</param>
    public void TrackMarketing(string aPartner, string aCampaign, string anAd, string aSubId,
            string aSubSubId)
    {
      // exit if not enabled
      if (!this.AnalyticsEnabled)
      {
        return;
      }
      Dictionary<string, object> trackEvent = this.CreateEvent(EventMarketing, false);
      this.PutField(trackEvent, "partner", aPartner);
      this.PutField(trackEvent, "campaign", aPartner);
      this.PutField(trackEvent, "ad", aPartner);
      this.PutField(trackEvent, "subid", aPartner);
      this.PutField(trackEvent, "subsubid", aPartner);
      this.AddEvent(trackEvent);
    }

    /// <summary>
    /// Tracks an user attribute, e.g. gender or birthday.
    ///
    /// If the %IQU %SDK has not been initialized or IQUSDK.AnalyticsEnabled property is <c>false</c>, this method will do nothing.
    /// </summary>
    /// <param name="aName">Name of the user attribute, e.g. gender</param>
    /// <param name="aValue">Value of the user attribute, e.g. female</param>
    public void TrackUserAttribute(string aName, string aValue)
    {
      // exit if not enabled
      if (!this.AnalyticsEnabled)
      {
        return;
      }
      Dictionary<string, object> trackEvent = this.CreateEvent(EventUserAttribute, false);
      trackEvent.Add("name", aName);
      trackEvent.Add("value", aValue);
      this.AddEvent(trackEvent);
    }

    /// <summary>
    /// Tracks the country of the user, only required for S2S implementations.
    ///
    /// If the %IQU %SDK has not been initialized or IQUSDK.AnalyticsEnabled property is <c>false</c>, this method will do nothing.
    /// </summary>
    /// <param name="aCountry">Country as specified in ISO3166-1 alpha-2, e.g. US, NL, DE</param>
    public void TrackCountry(string aCountry)
    {
      // exit if not enabled
      if (!this.AnalyticsEnabled)
      {
        return;
      }
      Dictionary<string, object> trackEvent = this.CreateEvent(EventCountry, false);
      trackEvent.Add("value", aCountry);
      this.AddEvent(trackEvent);
    }

    #endregion

    #region Public properties

    /// <summary>
    /// Gets the singleton IQUSDK instance. If no instance exists a new instance will be created.
    /// </summary>
    public static IQUSDK Instance
    {
      get
      {
        if (IQUSDK.m_instance == null)
        {
          IQUSDK.m_instance = new IQUSDK();
        }
        return IQUSDK.m_instance;
      }
    }

    /// <summary>
    /// Returns the analytics enabled state. When the analytics is not enabled, all tracking calls 
    /// are ignored and no tracking messages are sent to the server.
    ///
    /// On Android and iOS devices the enabled state depends on the limit ad tracking. If
    /// the user turned on limited ad tracking, this property will return <c>false</c>.
    /// </summary>
    public bool AnalyticsEnabled
    {
      get
      {
        return this.m_analyticsEnabled;
      }
    }

    /// <summary>
    /// Returns initialized state. After a call to IQUSDK.Start this property will return <c>true</c>.
    /// </summary>
    public bool Initialized
    {
      get
      {
        return this.m_initialized;
      }
    }

    /// <summary>
    /// Returns if a payable event is active or not.
    ///
    /// The default value is <c>true</c>.
    /// </summary>
    public bool Payable
    {
      get
      {
        return this.m_payable;
      }
      set
      {
        this.m_payable = value;
      }
    }

    /// <summary>
    /// This property determines the time between server availability checks in milliseconds.
    ///
    /// This property is used once the sending of a message fails. The property determines the time the %SDK
    /// waits before checking the availability of the server and trying to resend the queued messages.
    ///
    /// The default value is 2000 (2 seconds).
    /// </summary>
    public long CheckServerInterval
    {
      get
      {
        return this.m_checkServerInterval;
      }
      set
      {
        this.m_checkServerInterval = Math.Max(100, value);
      }

    }

    /// <summary>
    /// Turns the log on or off. This property only is of use when DEBUG has been defined, else no log messages are generated and this
    /// property does nothing.
    /// 
    /// The current log will be cleared when turning off the logging.
    /// 
    /// The default value is <c>true</c> when DEBUG is defined or the %SDK is run from within the Unity editor. Else it is set to <c>false</c>.
    /// </summary>
    public bool LogEnabled
    {
      get
      {
        return this.m_logEnabled;
      }
      set
      {
        this.m_logEnabled = value;
        if (!value)
        {
          this.m_log = "";
        }
      }
    }

    /// <summary>
    /// The current log. When DEBUG has been defined, various classes will add informational messages to this.
    /// </summary>
    public string Log
    {
      get
      {
        return this.m_log;
      }
    }

    /// <summary>
    /// Returns the sever availability state. The state is updated when messages are sent to the server.
    ///
    /// While the server is not available, messages will be queued to be sent once the server is available again.
    ///
    /// If the server is not available and there are pending messages, the class
    /// will check the server availability at regular intervals (see CheckServerInterval). Once the server
    /// becomes available again, the messages are sent to the server.
    /// </summary>
    public bool ServerAvailable
    {
      get
      {
        return this.m_serverAvailable;
      }
    }

    /// <summary>
    /// Gets the current test mode property value.
    ///
    /// The default value is ::None
    ///
    /// Use ::SimulateServer to prevent any network traffic.
    ///
    /// Use ::SimulateOffline to test the %SDK behaviour while the server is not available.
    ///
    /// To go back to normal operation use ::None
    /// </summary>
    public IQUTestMode TestMode
    {
      get
      {
        return this.m_testMode;
      }
      set
      {
        this.m_testMode = value;
      }
    }

    #endregion

    #region Internal properties

    /// <summary>
    /// Returns the IQUDevice instance.
    /// </summary>
    internal IQUDevice Device { get { return this.m_device; } }

    /// <summary>
    /// Returns the local storage helper.
    /// </summary>
    internal IQULocalStorage LocalStorage { get { return this.m_localStorage; } }
    
    /// <summary>
    /// Returns the IQUHelper instance component that has been attached to GameObject.
    /// </summary>
    internal IQUHelper Helper { get { return this.m_helper; } }

    /// <summary>
    /// Gets the game object created and managed by IQU.
    /// </summary>
    internal GameObject GameObject { get { return this.m_gameObject; } }

    #endregion

    #region Internal methods

    /// <summary>
    /// Add message to log. This method only functions when DEBUG or UNITY_EDITOR is defined.
    /// </summary>
    /// <param name="aMessage">Message to add to the log.</param>
    [Conditional("DEBUG"), Conditional("UNITY_EDITOR")] 
    internal void AddLog(string aMessage)
    {
      if (this.m_logEnabled)
      {
        UnityEngine.Debug.Log(aMessage); 
        this.m_log = this.m_log + aMessage + "\n";
      }
    }

    #endregion

    #region Event handlers

    /// <summary>
    /// Handles the update event, this method is called every frame.
    /// </summary>
    internal void HandleUpdate()
    {
      if (this.m_paused)
      {
        return;
      }
      switch (this.m_state)
      {
        case State.WaitForDevice:
          this.ProcessDevice();
          break;
        case State.ProcessPending:
          this.ProcessPendingMessages();
          break;
      }
    }

    /// <summary>
    /// Handles application focus changes.
    /// </summary>
    /// <param name="aFocus">If set to <c>true</c> a focus.</param>
    internal void HandleApplicationFocus(bool aFocus)
    {
    }

    /// <summary>
    /// Handles the application pause state changes.
    /// </summary>
    /// <param name="aPause">Set to <c>true</c> when paused.</param>
    internal void HandleApplicationPause(bool aPause)
    {
      if (aPause)
      {
        // save pending data to persistent storage
        if (this.m_localStorage != null)
        {
          this.m_localStorage.Save();
        }
        // save pending messages to persistent storage (in case someone stops
        // the app from outside).
        if (this.m_pendingMessages != null)
        {
          this.m_pendingMessages.Save();
        }
      }
      this.m_paused = aPause;
    }

    /// <summary>
    /// Handles the application quitting.
    /// </summary>
    internal void HandleApplicationQuit()
    {
      this.ClearReferences();
    }

    /// <summary>
    /// Handles the failure of sending messages.
    /// </summary>
    private void HandleSendMessagesFail()
    {
      // debug
      if (this.ServerAvailable)
      {
        this.AddLog("[Network] server is not available.");
      }
      // server no longer available
      this.SetServerAvailable(false);
      // add back to pending messages
      this.m_pendingMessages.Prepend(this.m_sendingMessages, false);
      // save all messages (including new ones that might have been added in between)
      this.m_pendingMessages.Save();
      // ready to process the pending queue again.
      this.m_state = State.ProcessPending;
    }

    /// <summary>
    /// Handles the success of sending messages.
    /// </summary>
    private void HandleSendMessagesSuccess()
    {
      // debug
      if (!this.ServerAvailable)
      {
        this.AddLog("[Network] server is available.");
      }
      // server is available (again)
      this.SetServerAvailable(true);
      // destroy the send messages and any persistent storage used
      this.m_sendingMessages.Clear(true);
      // ready to process the pending queue again
      this.m_state = State.ProcessPending;
    }

    #endregion

    
    #region Private support methods
    
    /// <summary>
    /// Gets current time/date in milliseconds.
    /// </summary>
    /// <returns>DateTime.Now in milliseconds.</returns>
    internal long GetMilliseconds()
    {
      return DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
    }
    
    /// <summary>
    /// Checks if the server is available.
    /// </summary>
    /// <param name="aSuccess">Action to invoke when server is available.</param>
    /// <param name="aFail">Action to invoke when server is not available.</param>
    private void CheckServer(Action aSuccess, Action aFail)
    {
      // server is not available since last check action?
      if (!this.ServerAvailable)
      {
        // get current time
        long currentTime = this.GetMilliseconds();
        // not enough time has passed since last check?
        if (currentTime < this.m_checkServerTime)
        {
          aFail();
        }
        else
        {
          // store new time
          this.m_checkServerTime = currentTime + this.CheckServerInterval;
          // check if the server is reachable and return result
          this.m_network.CheckServer(aSuccess, aFail);
        }
      }
      else
      {
        // don't perform any checks, if server became unavailable this will
        // be detected when sending the current pending messages.
        aSuccess();
      }
    }
    
    /// <summary>
    /// Puts a field and value into a Dictionary if the value is not null and not an empty string.
    /// </summary>
    /// <param name="anObject">JSON object to put value in</param>
    /// <param name="aName">Name of value</param>
    /// <param name="aValue">Value to put in</param>
    private void PutField(IDictionary<string, object> aDictionary, string aName, string aValue)
    {
      if (aValue == null)
      {
        return;
      }
      if (aValue.Length == 0)
      {
        return;
      }
      aDictionary.Add(aName, aValue);
    }
    
    #endregion

    #region Private initiliazation methods

    /// <summary>
    /// Initializes the instance. This method takes care of all initialization.
    /// </summary>
    /// <param name="anApiKey">API key</param>
    /// <param name="aSecretKey">API secret</param>
    /// <param name="aPayable">Initial payable value.</param>
    private void Initialize(string anApiKey, string aSecretKey, bool aPayable)
    {
      // exit if already initialized
      if (this.Initialized)
      {
        this.AddLog("[Init] WARNING: already initialized");
        return;
      }
      // create a game object and attach the internal component to it
      this.m_gameObject = new GameObject("__IQU_SDK__");
      this.m_helper = this.m_gameObject.AddComponent<IQUHelper>();
      // create local storage
      this.m_localStorage = new IQULocalStorage();
      // create network
      this.m_network = new IQUNetwork(anApiKey, aSecretKey);
      // create device
      this.m_device = new IQUDevice();
      // create message queues
      this.m_pendingMessages = new IQUMessageQueue();
      this.m_sendingMessages = new IQUMessageQueue();
      // update properties
      this.Payable = aPayable;
      // retrieve or create an unique ID
      this.ObtainSdkId();
      // wait for device
      this.m_state = State.WaitForDevice;
      // start initializing the the device
      this.m_device.Initialize();
      // update properties
      this.SetInitialized(true);
      // debug
      this.AddLog("[Init] %IQU %SDK is initialized");
    }

    /// <summary>
    /// Continues with initialization of the %IQU %SDK after the device initialized successful.
    /// </summary>
    private void InitializeAfterDevice()
    {
      // clear any messages added after initialize and before this method if analytics is not allowed.
      if (!this.AnalyticsEnabled)
      {
        this.m_pendingMessages.Clear(false);
      }
      // load stored messages and prepend to pending queue
      IQUMessageQueue stored = new IQUMessageQueue();
      stored.Load();
      this.m_pendingMessages.Prepend(stored, true);
      stored.Destroy();
      // add platform message if there is none
      if (!this.m_pendingMessages.HasEventType(EventPlatform) && this.AnalyticsEnabled)
      {
        this.TrackPlatform();
      }
      // process pending queue with next update
      this.m_state = State.ProcessPending;
    }

    /// <summary>
    /// Clears references to used instances.
    /// </summary>
    private void ClearReferences()
    {
      if (this.m_gameObject != null)
      {
        GameObject.Destroy(this.m_gameObject);
        this.m_gameObject = null;
        this.m_helper = null;
      }
      if (this.m_localStorage != null)
      {
        this.m_localStorage.Destroy();
        this.m_localStorage = null;
      }
      if (this.m_network != null)
      {
        this.m_network.Destroy();
        this.m_network = null;
      }
      if (this.m_pendingMessages != null)
      {
        this.m_pendingMessages.Destroy();
        this.m_pendingMessages = null;
      }
      if (this.m_sendingMessages != null)
      {
        this.m_sendingMessages.Destroy();
        this.m_sendingMessages = null;
      }
      if (this.m_ids != null)
      {
        this.m_ids.Destroy();
        this.m_ids = null;
      }
    }

    /// <summary>
    /// Processes the device result. Depending on the device, the %SDK might get disabled if the user opted out of ad tracking.
    /// </summary>
    private void ProcessDevice()
    {
      // exit if device is still busy initializing
      if (!this.m_device.Done)
      {
        return;
      }
      // debug
      this.AddLog("[Init] Device is initialized");
      #if UNITY_ANDROID && !UNITY_EDITOR
      this.SetId(IQUIdType.AndroidAdvertising, this.m_device.AdvertisingId);
      this.SetId(IQUIdType.AndroidAdTracking, this.m_device.AdTrackingEnabled ? "1" : "0");
      this.SetAnalyticsEnabled(this.m_device.AdTrackingEnabled);
      #elif UNITY_IOS && !UNITY_EDITOR
      this.SetId(IQUIdType.IOSAdvertising, this.m_device.AdvertisingId);
      this.SetId(IQUIdType.IOSAdTracking, this.m_device.AdTrackingEnabled ? "1" : "0");
      this.SetAnalyticsEnabled(this.m_device.AdTrackingEnabled);
      #endif
      this.InitializeAfterDevice();
    }
    
    #endregion

    #region Private property setters

    /// <summary>
    /// Sets enabled property value.
    /// </summary>
    /// <param name="aValue">New value to use</param>
    private void SetInitialized(bool aValue)
    {
      this.m_initialized = aValue;
    }
    
    /// <summary>
    /// Sets enabled property value.
    /// </summary>
    /// <param name="aValue">New value to use</param>
    private void SetAnalyticsEnabled(bool aValue)
    {
      this.m_analyticsEnabled = aValue;
    }
    
    /// <summary>
    /// Sets serverAvailable property value.
    /// 
    /// <param name="aValue">New value to use</param>
    /// </summary>
    private void SetServerAvailable(bool aValue)
    {
      this.m_serverAvailable = aValue;
    }

    #endregion
    
    #region Private id related methods
    
    /// <summary>
    /// Store a new id or update existing id with new value.
    /// </summary>
    /// <param name="aType">Type to store id for.</param>
    /// <param name="aValue">Value to store.</param>
    private void SetId(IQUIdType aType, string aValue)
    {
      this.m_ids.Set(aType, aValue);
      if (this.Initialized)
      {
        this.m_pendingMessages.UpdateId(aType, aValue);
      }
    }
    
    /// <summary>
    /// Initializes the id managed by the %SDK. Try to retrieve it from local storage, if it fails create a new id.
    /// </summary>
    private void ObtainSdkId()
    {
      // get id from local storage
      string id = this.m_localStorage.GetString(SDKIdKey, "");
      // create new id and store it when none was found
      if (id.Length == 0)
      {
        id = System.Guid.NewGuid().ToString();
        this.m_localStorage.SetString(SDKIdKey, id);
      }
      // set %SDK id
      this.SetId(IQUIdType.SDK, id);
    }

    #endregion

    #region Private message related methods

    /// <summary>
    /// Processes the pending messages (if any) and try to send them to the
    /// server.
    /// </summary>
    private void ProcessPendingMessages()
    {
      // exit if there are no pending messages
      if (this.m_pendingMessages.IsEmpty())
      {
        return;
      }
      // Move messages from pending messages to sending messages; this
      // will clear the pending message queue. The sending messages queue
      // is always empty before this call.
      // The queue property in every message is not updated, since
      // messages will not change while they are in the sending queue.
      this.m_sendingMessages.Prepend(this.m_pendingMessages, false);
      // check if a new heartbeat message needs to be created
      this.TrackHeartbeat(this.m_sendingMessages);
      // any message that needs to be sent?
      if (!this.m_sendingMessages.IsEmpty())
      {
        // busy with IO
        this.m_state = State.WaitForSend;
        // check server
        this.CheckServer(this.SendMessages, this.HandleSendMessagesFail);
      }
    }

    /// <summary>
    /// Tries to send the messages to the server. See HandleSendMessagesSuccess and HandleSendMessagesFail how
    /// success and failure are handled.
    /// </summary>
    private void SendMessages()
    {
      this.m_network.Send(this.m_sendingMessages, this.HandleSendMessagesSuccess, this.HandleSendMessagesFail);
    }

    /// <summary>
    /// Adds a message to the pending message list if the %SDK is initialized; else the message gets destroyed.
    /// </summary>
    /// <param name="aMessage">Message to add.</param>
    private void AddMessage(IQUMessage aMessage)
    {
      // only add if %IQU %SDK has been initialized.
      if (this.Initialized)
      {
        this.m_pendingMessages.Add(aMessage);
      }
      else
      {
        // message was not added, destroy the instance
        aMessage.Destroy();
      }
    }

    #endregion

    #region Private event related methods

    /// <summary>
    /// Creates a message from an event and it to the pending queue.
    /// </summary>
    /// <param name="anEvent">Event to create message for.</param>
    private void AddEvent(IDictionary<string, object> anEvent)
    {
      IQUMessage message = new IQUMessage(this.m_ids, anEvent);
      this.AddMessage(message);
    }

    /// <summary>
    /// Creates an event with a certain type and adds optionally a time-stamp for the current date and time.
    /// </summary>
    /// <returns>JSONObject instance containing event</returns>
    /// <param name="anEventType">Type to use</param>
    /// <param name="anAddTimestamp">When <c>true</c> add "timestamp" field.</param>
    private Dictionary<string, object> CreateEvent(string anEventType, bool anAddTimestamp)
    {
      Dictionary<string, object> result = new Dictionary<string, object>();
      result.Add("type", anEventType);
      if (anAddTimestamp)
      {
        result.Add("timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
      }
      return result;
    }

    #endregion

    #region Private tracking methods

    /// <summary>
    /// Checks if enough time has passed since last heartbeat message. If it has the method
    /// adds a new heartbeat message.
    /// </summary>
    /// <param name="aMessages">Message queue to add the heartbeat message to.</param>
    private void TrackHeartbeat(IQUMessageQueue aMessages)
    {
      long currentTime = this.GetMilliseconds();
      if (currentTime > this.m_heartbeatTime + HeartbeatInterval)
      {
        Dictionary<string, object> trackEvent = this.CreateEvent(EventHeartbeat, true);
        trackEvent.Add("is_payable", this.m_payable);
        aMessages.Add(new IQUMessage(this.m_ids, trackEvent));
        this.m_heartbeatTime = currentTime;
      }
    }

    /// <summary>
    /// Tracks the platform of the user.
    /// </summary>
    private void TrackPlatform()
    {
      Dictionary<string, object> trackEvent = this.CreateEvent(EventPlatform, false);
      trackEvent.Add("device_model", SystemInfo.deviceModel);
      trackEvent.Add("os_name", SystemInfo.operatingSystem);
      trackEvent.Add("screen_size_width", Screen.width);
      trackEvent.Add("screen_size_height", Screen.height);
      trackEvent.Add("screen_size_dpi", Screen.dpi);
      this.AddEvent(trackEvent);
    }

    #endregion
  }
}
        