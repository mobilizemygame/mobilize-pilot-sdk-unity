namespace IQU.SDK
{
  /// <summary>
  /// Possible test modes available for the %SDK.  
  /// </summary>
  public enum IQUTestMode
  {
    /// <summary>
    /// Normal operation mode.
    /// </summary>
    None = 0,

    /// <summary>
    /// Don't perform any network IO, simulate that every transaction is successful.
    /// </summary>
    SimulateServer = 1,

    /// <summary>
    /// Simulate that the server is off-line.
    /// </summary>
    SimulateOffline = 2
  }
}
