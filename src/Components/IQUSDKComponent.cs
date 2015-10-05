using UnityEngine;
using System.Collections;
using IQU.SDK;

/// <summary>
/// The IQUSDKComponent must be added to the root of the first scene. The user should enter valid values for the various properties.
/// 
/// It will call IQU.SDK.IQUSDK.Start to initialize the %SDK.
/// </summary>
public class IQUSDKComponent : MonoBehaviour
{
  #region Component properties
  
  /// <summary>
  /// The API key.
  /// </summary>
  [Tooltip("Fill in the API key.")]
  public string apiKey;
  
  /// <summary>
  /// The secret key.
  /// </summary>
  [Tooltip("Fill in the secret key.")]
  public string secretKey;
  
  /// <summary>
  /// When <c>true</c> call IQU.SDK.IQUSDK.Start with values set by this component. Set to <c>false</c> to call
  /// IQUSDK.Instance.Start at a different moment.
  /// </summary>
  [Tooltip("When checked the component will start the IQU SDK automatically.")]
  public bool autoStart = true;

  /// <summary>
  /// Initial payable value.
  /// </summary>
  [Tooltip("Default payable state")]
  public bool payable = true;
    
  /// <summary>
  /// Default interval in milliseconds to check for server availability.
  /// </summary>
  [Tooltip("Time in milliseconds to check for server availability.")]
  public int checkServerInterval = 2000;

  /// <summary>
  /// Default test mode.
  /// </summary>
  [Tooltip("Initial test mode to use for the IQU SDK.")]
  public IQUTestMode testMode = IQUTestMode.None;
  
  #endregion
  
  #region MonoBehaviour methods
  
  /// <summary>
  /// Copies component properties and call Start if autoStart is set to <c>true</c>
  /// </summary>
  void Start()
  {
    IQUSDK.Instance.CheckServerInterval = this.checkServerInterval;
    IQUSDK.Instance.TestMode = this.testMode;
    if (this.autoStart)
    {
      IQUSDK.Instance.Start(this.apiKey, this.secretKey, this.payable);
    }
    // delete this component (only need to run once)
    Object.Destroy(this);
  }
  
  #endregion
}
