using System;
using System.IO;
using System.Collections.Generic;

namespace IQU.SDK
{
  /// <summary>
  /// IQUIds is a collection that can store an id for every id type.
  /// </summary>
  internal class IQUIds
  {
    #region Private vars

    /// <summary>
    /// Use array to store ids
    /// </summary>
    private string[] m_ids;
    
    /// <summary>
    /// Size to use for m_ids
    /// </summary>
    private static int m_count = -1;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    internal IQUIds()
    {
      // first instance?
      if (IQUIds.m_count < 0)
      {
        // determine max index value for type
        Array types = Enum.GetValues(typeof(IQUIdType));
        IQUIds.m_count = 1 + (int)types.GetValue(types.Length - 1);
      }
      // create array and fill it with empty strings
      this.m_ids = new string[IQUIds.m_count];
      this.ClearIds();
    }

    /// <summary>
    /// Creates ids container using another ids container as source.
    /// </summary>
    /// <param name="aSource">Source to make copy from.</param> 
    private IQUIds(IQUIds aSource)
    {
      this.m_ids = (string[])aSource.m_ids.Clone();
    }

    #endregion

    #region Internal methods

    /// <summary>
    /// Cleans up references and used resources.
    /// </summary>
    internal void Destroy()
    {
    }

    /// <summary>
    /// Returns an id value for a certain type. If the id is not known, an empty
    /// string is returned.
    /// </summary>
    /// <returns>id value or "" if no value was stored for that type.</returns>
    /// <param name="aType">Id type to get value for.</param> 
    internal String Get(IQUIdType aType)
    {
      // handle types that have a fixed value and not stored in the array
      switch (aType)
      {
#if UNITY_ANDROID && !UNITY_EDITOR
        case IQUIdType.AndroidId:
          return IQUSDK.Instance.Device.AndroidId;
        case IQUIdType.AndroidSerial:
          return IQUSDK.Instance.Device.AndroidSerial;
#elif UNITY_IOS && !UNITY_EDITOR
        case IQUIdType.IOSVendor:
          return IQUSDK.Instance.Device.VendorId;
#endif
        default:
          return this.m_ids[(int)aType];
      }
    }

    /// <summary>
    /// Store a value for a certain type. Any previous value is overwritten.
    /// </summary>
    /// <param name="aType">Type to store value for.</param>
    /// <param name="aValue">Value to store for the type.</param>
    internal void Set(IQUIdType aType, String aValue)
    {
      // don't store value for types that have a fixed value
      switch (aType)
      {
#if UNITY_ANDROID && !UNITY_EDITOR
        case IQUIdType.AndroidId:
        case IQUIdType.AndroidSerial:
          return;
#elif UNITY_IOS && !UNITY_EDITOR
        case IQUIdType.IOSVendor:
          return;
#endif
        default:
          this.m_ids[(int)aType] = aValue == null ? "" : aValue;
          break;
      }
    }

    /// <summary>
    /// Save the ids.
    /// </summary>
    /// <param name="aWriter">Writer to store values with.</param>
    internal void Save(BinaryWriter aWriter)
    {
      foreach (IQUIdType type in Enum.GetValues(typeof(IQUIdType)))
      {
        string value = this.Get(type);
        if (value.Length > 0)
        {
          aWriter.Write((byte)type);
          aWriter.Write(value);
        }
      }
      aWriter.Write((byte)0xff);
    }

    /// <summary>
    /// Load the ids.
    /// </summary>
    /// <param name="aReader">Reader to read values from.</param>
    internal void Load(BinaryReader aReader)
    {
      byte typeValue;
      while ((typeValue = aReader.ReadByte()) != 0xff)
      {
        string value = aReader.ReadString();
        this.Set((IQUIdType)typeValue, value);
      }
    }

    /// <summary>
    /// Returns a copy of this instance.
    /// </summary>
    /// <returns>IQUIds instance containing the same ids.</returns>
    internal IQUIds Clone()
    {
      return new IQUIds(this);
    }

    /// <summary>
    /// Returns ids as JSON formatted string; only non empty ids are returned.
    /// </summary>
    /// <returns>JSON formatted string</returns>
    internal string ToJSONString()
    {
      Dictionary<string, string> result = new Dictionary<string, string>(this.m_ids.Length);
      foreach (IQUIdType type in Enum.GetValues(typeof(IQUIdType)))
      {
        string value = this.Get(type);
        if (value.Length > 0)
        {
          result[this.GetJSONName(type)] = value;
        }
      }
      return MiniJSON.Json.Serialize(result);
    }

    #endregion

    #region Private methods

    /// <summary>
    /// Returns property name for use with JSON formatted definitions.
    /// </summary>
    /// <returns>name for use with JSON formatted definitions.</returns>
    /// <param name="aType">Type to get JSON name for</param>
    private String GetJSONName(IQUIdType aType)
    {
      switch (aType)
      {
        case IQUIdType.Custom:
          return "custom_user_id";
        case IQUIdType.Facebook:
          return "facebook_user_id";
        case IQUIdType.GooglePlus:
          return "google_plus_user_id";
        case IQUIdType.SDK:
          return "iqu_sdk_id";
        case IQUIdType.Twitter:
          return "twitter_user_id";
#if UNITY_ANDROID && !UNITY_EDITOR
        case IQUIdType.AndroidAdTracking:
          return "android_ad_tracking";
        case IQUIdType.AndroidAdvertising:
          return "android_advertising_id";
        case IQUIdType.AndroidId:
          return "android_id";
        case IQUIdType.AndroidSerial:
          return "android_serial";
#endif  
#if UNITY_IOS && !UNITY_EDITOR
        case IQUIdType.IOSAdvertising:
          return "ios_advertising_identifier";
        case IQUIdType.IOSVendor:
          return "ios_vendor_id";
        case IQUIdType.IOSAdTracking:
          return "ios_ad_tracking";
#endif        
        default:
          return aType.ToString();
      }
    }

    /// <summary>
    /// Clear all stored ids by replacing them with an empty string.
    /// </summary>
    private void ClearIds()
    {
      for (int index = 0; index < this.m_ids.Length; index++)
      {
        this.m_ids[index] = "";
      }
    }

    #endregion
  }
}
