using System;
using UnityEngine;

namespace IQU.SDK
{
  /// <summary>
  /// IQULocalStorage implements methods to locally store and retrieve values.
  /// </summary>
  internal class IQULocalStorage
  {
    /// <summary>
    /// Cleans up references and used resources.
    /// </summary>
    internal void Destroy()
    {
    }

    /// <summary>
    /// Gets a string.
    /// </summary>
    /// <returns>The stored string or aDefault.</returns>
    /// <param name="aKey">A key to get the string for.</param>
    /// <param name="aDefault">A default value to use when there is no string stored for the key.</param>
    internal string GetString(string aKey, string aDefault = "")
    {
      return PlayerPrefs.GetString(aKey, aDefault);
    }
    
    /// <summary>
    /// Gets an integer.
    /// </summary>
    /// <returns>The stored integer or aDefault.</returns>
    /// <param name="aKey">A key to get the integer for.</param>
    /// <param name="aDefault">A default value to use when there is no integer stored for the key.</param>
    internal int GetInt(string aKey, int aDefault = 0)
    {
      return PlayerPrefs.GetInt(aKey, aDefault);
    }
    
    /// <summary>
    /// Gets a floating number.
    /// </summary>
    /// <returns>The stored floating number or aDefault.</returns>
    /// <param name="aKey">A key to get the floating number for.</param>
    /// <param name="aDefault">A default value to use when there is no floating number stored for the key.</param>
    internal float GetFloat(string aKey, float aDefault = 0.0f)
    {
      return PlayerPrefs.GetFloat(aKey, aDefault);
    }
    
    /// <summary>
    /// Gets a boolean.
    /// </summary>
    /// <returns>The stored boolean or aDefault.</returns>
    /// <param name="aKey">A key to get the boolean for.</param>
    /// <param name="aDefault">A default value to use when there is no boolean stored for the key.</param>
    internal bool GetBool(string aKey, bool aDefault = true)
    {
      return this.HasKey(aKey) ? this.GetInt(aKey) == 1 : aDefault;
    }
    
    /// <summary>
    /// Gets a long (64bit) number.
    /// </summary>
    /// <returns>The stored long value or aDefault.</returns>
    /// <param name="aKey">A key to get the long value for.</param>
    /// <param name="aDefault">A default value to use when there is no long value stored for the key.</param>
    internal long GetLong(string aKey, long aDefault = 0L)
    {
      return this.HasKey(aKey) ? long.Parse(PlayerPrefs.GetString(aKey)) : aDefault;
    }

    /// <summary>
    /// Gets a byte array.
    /// </summary>
    /// <returns>The stored bytes or aDefault.</returns>
    /// <param name="aKey">A key to get the bytes for.</param>
    /// <param name="aDefault">A default value to use when there is no bytes stored for the key.</param>
    internal byte[] GetBytes(string aKey, byte[] aDefault = null)
    {
      string encoded = this.GetString(aKey, "");
      if (encoded.Length > 0)
      {
        return Convert.FromBase64String(encoded);
      }
      else
      {
        return aDefault;
      }
    }
    
    /// <summary>
    /// Deletes all stored data.
    /// </summary>
    internal void DeleteAll()
    {
      PlayerPrefs.DeleteAll();
    }
    
    /// <summary>
    /// Deletes the data for specific key.
    /// </summary>
    /// <param name="aKey">A key to delete the data for.</param>
    internal void DeleteKey(string aKey)
    {
      PlayerPrefs.DeleteKey(aKey);
    }
    
    /// <summary>
    /// Checks if there is a locally stored data for a specific key.
    /// </summary>
    /// <returns><c>true</c> if has there is data for the key; otherwise, <c>false</c>.</returns>
    /// <param name="aKey">A key to check.</param>
    internal bool HasKey(string aKey)
    {
      return PlayerPrefs.HasKey(aKey);
    }
    
    /// <summary>
    /// Flushes any cached data to disc. This method might halt the application, so care should be taken when
    /// calling it.
    /// </summary>
    internal void Save()
    {
      PlayerPrefs.Save();
    }
    
    /// <summary>
    /// Stores a floating number in the storage.
    /// </summary>
    /// <param name="aKey">Key to store value for.</param>
    /// <param name="aValue">A value to store.</param>
    internal void SetFloat(string aKey, float aValue)
    {
      PlayerPrefs.SetFloat(aKey, aValue);
    }
    
    /// <summary>
    /// Stores an integer in the storage.
    /// </summary>
    /// <param name="aKey">Key to store value for.</param>
    /// <param name="aValue">A value to store.</param>
    internal void SetInt(string aKey, int aValue)
    {
      PlayerPrefs.SetInt(aKey, aValue);
    }
    
    /// <summary>
    /// Stores a string in the storage.
    /// </summary>
    /// <param name="aKey">Key to store value for.</param>
    /// <param name="aValue">A value to store.</param>
    internal void SetString(string aKey, string aValue)
    {
      PlayerPrefs.SetString(aKey, aValue);
    }
    
    /// <summary>
    /// Stores a boolean in the storage.
    /// </summary>
    /// <param name="aKey">Key to store value for.</param>
    /// <param name="aValue">A value to store.</param>
    internal void SetBool(string aKey, bool aValue)
    {
      this.SetInt(aKey, aValue ? 1 : 0);
    }
    
    /// <summary>
    /// Stores a long in the storage.
    /// </summary>
    /// <param name="aKey">Key to store value for.</param>
    /// <param name="aValue">A value to store.</param>
    internal void SetLong(string aKey, long aValue)
    {
      PlayerPrefs.SetString(aKey, aValue.ToString());
    }

    /// <summary>
    /// Stores byte array in the storage.
    /// </summary>
    /// <param name="aKey">Key to store value for.</param>
    /// <param name="aValue">A value to store.</param>
    internal void SetBytes(string aKey, byte[] aValue)
    {
      this.SetString(aKey, Convert.ToBase64String(aValue));
    }
  }
}
