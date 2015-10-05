using UnityEngine;
using System.Collections;

namespace IQU.SDK
{
  /// <summary>
  /// IQUHelper is a simple component that acts as stub so the %IQU %SDK can use MonoBehaviour methods and
  /// react to MonoBehaviour events.
  /// </summary>
  internal class IQUHelper : MonoBehaviour
  {
    #region Unity methods

    /// <summary>
    /// Awake is called when the component is created.
    /// </summary>
    void Awake() 
    {
      // make sure the attached gameObject is not destroyed (when a new scene is loaded).
      Object.DontDestroyOnLoad(transform.gameObject);
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
      IQUSDK.Instance.HandleUpdate();
    }

    /// <summary>
    /// OnApplicationFocus is called whenever the application gets or looses the focus.
    /// </summary>
    /// <param name="aFocusStatus">The new focus state.</param>
    void OnApplicationFocus(bool aFocusStatus) 
    {
      IQUSDK.Instance.HandleApplicationFocus(aFocusStatus);
    }

    /// <summary>
    /// OnApplicationPause is called whenever the application is paused or resumed.
    /// </summary>
    /// <param name="aPauseStatus">The new pause state.</param>
    void OnApplicationPause(bool aPauseStatus) 
    {
      IQUSDK.Instance.HandleApplicationPause(aPauseStatus);
    }

    /// <summary>
    /// OnApplicationQuit is called when the application is about to quit.
    /// </summary>
    void OnApplicationQuit()
    {
      IQUSDK.Instance.HandleApplicationQuit();
    }

    #endregion
  }
}

