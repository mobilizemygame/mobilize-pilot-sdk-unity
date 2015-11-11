using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace IQU.SDK
{
  /// <summary>
  /// IQUNetwork takes care of sending data to the %IQU server.
  /// </summary>
  internal class IQUNetwork
  {
    #region Internal consts

    /// <summary>
    /// Name in JSONObject of field containing integer response code.
    /// </summary>
    internal const String Code = "RESPONSE_CODE";

    /// <summary>
    /// Name in JSONObject of field containing error description.
    /// </summary>
    internal const String Error = "RESPONSE_ERROR";

    #endregion

    #region Private consts

    /// <summary>
    /// URL to service (must end with /)
    /// </summary>
    private const String URL = "https://tracker.iqugroup.com/v3/";

    #endregion

    #region Private types

    /// <summary>
    /// Define delegate for callbacks from Send methods.
    /// </summary>
    /// <param name="aResult">Contains the result from the server (after it has been parsed)</param>
    internal delegate void IOCallback(IDictionary<string, object> aResult);

    #endregion

    #region Private vars

    /// <summary>
    /// The service url to use
    /// </summary>
    private String m_serviceUrl;

    /// <summary>
    /// The API key
    /// </summary>
    private String m_apiKey;

    /// <summary>
    /// The secret key (used to generate HMAC hash with)
    /// </summary>
    private String m_secretKey;

    #endregion

    #region Internal methods

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="anApiKey">API key</param> 
    /// <param name="aSecretKey">Secret key</param>
    internal IQUNetwork(String anApiKey, String aSecretKey)
    {
      // for now just copy, defined just in case the future supports multiple
      // urls
      this.m_serviceUrl = URL;
      // copy key and secret remove spaces, carriage returns and line feeds
      this.m_apiKey = anApiKey.Replace("\n", "").Replace("\r", "").Replace(" ", "");
      this.m_secretKey = aSecretKey.Replace("\n", "").Replace("\r", "").Replace(" ", "");
    }

    /// <summary>
    /// Cleans up references and resources. 
    /// </summary>
    internal void Destroy()
    {
    }

    /// <summary>
    /// Tries to send one or more messages to server.
    /// </summary>
    /// <param name="aMessages">MessageQueue to send</param>
    /// <param name="aSuccess">Callback that will be called if sending was successful.</param>
    /// <param name="aFail">Callback that will be called if sending failed.</param>
    internal void Send(IQUMessageQueue aMessages, Action aSuccess, Action aFail)
    {
      this.SendSigned(this.m_serviceUrl, aMessages.ToJSONString(), (IDictionary<string, object> aResult) => {
        if (aResult.ContainsKey(IQUNetwork.Error) || !aResult.ContainsKey("status"))
        {
          aFail();
        }
        else if (aResult["status"].ToString().ToLower() != "ok")
        {
          aFail();
        }
        else
        {
          aSuccess();
        }
      });
    }

    /// <summary>
    /// Tries to send a small message to the server to see if it is reachable.
    /// </summary>
    /// <param name="aSuccess">Callback that will be called if the server is available.</param>
    /// <param name="aFail">Callback that will be called if the server is not reachable.</param>
    internal void CheckServer(Action aSuccess, Action aFail)
    {
      this.Send(this.m_serviceUrl + "?ping", (IDictionary<string, object> aResult) => {
        if (aResult.ContainsKey(IQUNetwork.Error))
        {
          aFail();
        }
        else
        {
          aSuccess();
        }
      });
    }

    #endregion

    #region Private methods

    /// <summary>
    /// Get string as array of byte.
    /// </summary>
    /// <returns>The bytes.</returns>
    /// <param name="aText">String.</param>
    private byte[] GetBytes(string aText)
    {
      return Encoding.UTF8.GetBytes(aText);
    }
    
    /// <summary>
    /// Generate a HMAC256 hash.
    /// </summary>
    /// <returns>The HMAC256 hash as hexadecimal string.</returns>
    /// <param name="aSecret">API secret</param>
    /// <param name="aContent">the content of the message</param>
    private string GenerateHMACSHA512(string aSecret, string aContent)
    {
      Encoding encoding = Encoding.UTF8;
      var keyByte = encoding.GetBytes (aSecret);
      var hmacsha512 = new HMACSHA512 (keyByte);
      hmacsha512.ComputeHash (encoding.GetBytes (aContent));
      return this.ByteToHex (hmacsha512.Hash);
    }
	
    /// <summary>
    /// Convert byte array to hex string.
    /// </summary>
    /// <returns>Hex string (lowercase).</returns>
    /// <param name="aBuff">Byte array to convert</param>
    private string ByteToHex(byte[] aBuff) 
    {
      string sbinary = "";
      for (int i = 0; i < aBuff.Length; i++) 
      {
        /* hex format */
        sbinary += aBuff [i].ToString ("x2"); 
      }
      return sbinary;
    }

    /// <summary>
    /// Sends a signed message to the server.
    /// </summary>
    /// <param name="anUrl">URL without parameters</param>
    /// <param name="aPostContent">Content that will be posted to the server.</param>
    /// <param name="aCallback">Callback that will be invoked once sending has finished.</param>
    private void SendSigned(string anUrl, string aPostContent, IOCallback aCallback)
    {
      string signature = this.GenerateHMACSHA512(this.m_secretKey, aPostContent);
      this.Send(anUrl + "?api_key=" + this.m_apiKey + "&signature=" + signature, aPostContent, aCallback);
    }

    /// <summary>
    /// Invokes an url using GET.
    /// </summary>
    /// <param name="anUrl">An URL to invoke.</param>
    /// <param name="aCallback">Callback that will be invoked once sending has finished.</param>
    private void Send(string anUrl, IOCallback aCallback)
    {
      this.Send(anUrl, null, aCallback);
    }

    /// <summary>
    /// Sends data to an url.
    /// </summary>
    /// <param name="anUrl">An URL.</param>
    /// <param name="aPostContent">A post content or null if there is no post content.</param>
    /// <param name="aCallback">Callback that will be invoked once sending has finished.</param>
    private void Send(string anUrl, string aPostContent, IOCallback aCallback)
    {
      IQUSDK.Instance.Helper.StartCoroutine(this.DoSend(anUrl, aPostContent, aCallback));
    }

    /// <summary>
    /// Return all header values (concatenated). 
    /// </summary>
    /// <returns>All header values.</returns>
    /// <param name="aWWW">WWW instance to get headers from</param>
    private string GetHeaders(WWW aWWW)
    {
      string result = "";
      if (aWWW.responseHeaders != null)
      {
        foreach (string key in aWWW.responseHeaders.Keys)
        {
          try
          {
            if (key != null)
            {
              result += "[" + key + "]='" + aWWW.responseHeaders[key] + "';";
            }
          }
          catch (Exception)
          {
          }
        }
      }
      return result;
    }
    
    /// <summary>
    /// Try to obtain response code.
    /// </summary>
    /// <returns>The code or -1 if not found.</returns>
    /// <param name="aWWW">A WWW instance</param>
    private int GetResponseCode(WWW aWWW)
    {
      int result = -1;
      // first check status in header
      if ((aWWW.responseHeaders != null))
      {
        string status = "";
        if (aWWW.responseHeaders.ContainsKey("STATUS"))
        {
          status = aWWW.responseHeaders["STATUS"];
        }
        else if (aWWW.responseHeaders.ContainsKey("NULL"))
        {
          status = aWWW.responseHeaders["NULL"];
        }
        string[] parts = status.Split(' ');
        if (parts.Length >= 2)
        {
          if (!int.TryParse(parts[1], out result))
          {
            result = -1;
          }
        }
      }
      // result not yet found and there is an error?
      if ((result < 0) && !string.IsNullOrEmpty(aWWW.error))
      {
        // yes, check if error starts with a result
        string[] parts = aWWW.error.Split(' ');
        if (parts.Length > 0)
        {
          if (!int.TryParse(parts[0], out result))
          {
            result = -1;
          }
        }
      }
      return result;
    }

    /// <summary>
    /// Simulates an off-line state. The method invokes the callback with an error result.
    /// </summary>
    /// <param name="aCallback">Callback that will be invoked once sending has finished.</param>
    private void SimulateOffline(IOCallback aCallback)
    {
      // debug
      IQUSDK.Instance.AddLog("[Network] simulating off-line.");
      // build result for off-line
      Dictionary<string, object> result = new Dictionary<string, object>(1);
      result.Add(IQUNetwork.Error, "simulating off-line, IQUSDK.Instance.TestMode == IQUTestMode.SimulateOffline");
      aCallback(result);
    }

    /// <summary>
    /// Simulates a correct response without any actual server communication. The method invokes the callback with a successful result.
    /// </summary>
    /// <param name="aCallback">Callback that will be invoked once sending has finished.</param>
    private void SimulateServer(IOCallback aCallback)
    {
      // debug
      IQUSDK.Instance.AddLog("[Network] simulating server response.");
      // simulate on-line and successful attempt
      Dictionary<string, object> result = new Dictionary<string, object>(4);
      result.Add(IQUNetwork.Code, 200);
      result.Add("status", "ok");
      result.Add("request_id", "2a7-558bf465ed65-b79a84");
      result.Add("time", "2015-06-26 12:00:00 UTC");
      aCallback(result);
    }

    /// <summary>
    /// Sends a message with optional POST content to the server.
    /// </summary>
    /// <param name="anUrl">URL to send data to.</param>
    /// <param name="aPostContent">POST content or null if there is none.</param>
    /// <param name="aCallback">Callback that will be invoked once sending has finished.</param>
    private IEnumerator DoSend(string anUrl, string aPostContent, IOCallback aCallback)
    {
      // debug
#if DEBUG || UNITY_EDITOR
      IQUSDK.Instance.AddLog("[Network][Sending] " + anUrl);
      if (aPostContent != null) {
        IQUSDK.Instance.AddLog("[Network][Content] " + aPostContent);
      }
#endif
      switch (IQUSDK.Instance.TestMode)
      {
        case IQUTestMode.SimulateOffline:
          // wait for 2 seconds
          yield return new WaitForSeconds(2);
          this.SimulateOffline(aCallback);
          return false;
        case IQUTestMode.SimulateServer:
          // wait for 2 seconds
          yield return new WaitForSeconds(2);
          this.SimulateServer(aCallback);
          return false;
      }
      // set custom headers
#if UNITY_2 || UNITY_3 || UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4
      Hashtable postHeaders = new Hashtable(10);
#else
      Dictionary<string, string> postHeaders = new Dictionary<string, string>(30);
#endif
      postHeaders.Add("Content-Type", "application/json");
      postHeaders.Add("SdkType", IQUSDK.SdkType);
      postHeaders.Add("SdkVersion", IQUSDK.SdkVersion);
      // encoding is used to get the JSON string in correct byte array
      UTF8Encoding encoding = new UTF8Encoding();
      // send request
      WWW request = new WWW(anUrl, aPostContent == null ? null : encoding.GetBytes(aPostContent), postHeaders);
      yield return request;
      // check if server returned content
      bool hasContent = String.IsNullOrEmpty(request.error) && !String.IsNullOrEmpty(request.text);
      // debug
#if DEBUG || UNITY_EDITOR
      IQUSDK.Instance.AddLog("[Network][Response] " + anUrl);
      if (!String.IsNullOrEmpty(request.error))
      {
        IQUSDK.Instance.AddLog("[Network][Response][Error] " + request.error);
      }
      IQUSDK.Instance.AddLog("[Network][Response][Headers] " + this.GetHeaders(request));
      if (hasContent)
      {
        string text = request.text.Replace("\n", "");
        IQUSDK.Instance.AddLog("[Network][Response][Content] " + text);
      }
#endif
      // server returned content? Y: assume it is JSON and de-serialize it; N: create empty result
      Dictionary<string,object> result = hasContent ? MiniJSON.Json.Deserialize(request.text) as Dictionary<string, object> : new Dictionary<string, object>(2);
      // error parsing json?
      if (result == null)
      {
        // yes, set error
        result = new Dictionary<string, object>(2);
        result.Add(IQUNetwork.Error, "error parsing JSON result data.");
      }
      else
      {
        // try to determine code
        int code = this.GetResponseCode(request);
        // sometimes server returns 100, even though all content has been received; replace with 200
        if (code == 100)
        {
          code = 200;
        }
        // add code and error (if any)
        if (code >= 0)
        {
          // add code
          result.Add(IQUNetwork.Code, code);
          // debug
          IQUSDK.Instance.AddLog("[Network][Response][Code] " + code.ToString());
        }
        if (!String.IsNullOrEmpty(request.error))
        {
          result.Add(IQUNetwork.Error, request.error);
        }
        else if (((code < 200) || (code > 299)) && (code >= 0))
        {
          result.Add(IQUNetwork.Error, "http status code " + code.ToString() + " indicates invalid action");
        }
      }
      aCallback(result);
    }

    #endregion
  }
}
