namespace IQU.SDK
{
	/// <summary>
	/// Define the possible ids supported by the %SDK. The id types are defined with a
	/// fixed integer value, since the id type value is used when saving and loading
	/// id values.
	/// </summary>
	public enum IQUIdType
	{
		/// <summary>
		/// An id that is generated by %SDK and stored in local persistent storage.
		/// </summary>
		SDK = 0,

		/// <summary>
		/// An id that is obtained from Facebook.
		/// </summary>
		Facebook = 1,

		/// <summary>
		/// An id that is obtained from Google+.
		/// </summary>
		GooglePlus = 2,

		/// <summary>
		/// An id that is obtained from Twitter.
		/// </summary>
		Twitter = 3,

		/// <summary>
		/// A custom id
		/// </summary>
		Custom = 4,

#if UNITY_ANDROID && !UNITY_EDITOR
		/// <summary>
		/// An id obtained from android.os.Build.SERIAL
		/// </summary>
		AndroidSerial = 5,

		/// <summary>
		/// An id obtained from settings.secure.ANDROID_ID
		/// </summary>
		AndroidId = 6,

		/// <summary>
		/// An id obtained from Google Play.
		/// </summary>
		AndroidAdvertising = 7,

		/// <summary>
		/// Not really an id, but contains the limit ad tracking value from Google
		/// Play ("0" indicates limited ad tracking).
		/// </summary>
		AndroidAdTracking = 8
#endif

#if UNITY_IOS && !UNITY_EDITOR
    /// <summary>
    /// The vendor id (available only with iOS targets)
    /// </summary>
		IOSVendor = 5,

    /// <summary>
    /// The advertising id (available only with iOS targets)
    /// </summary>
		IOSAdvertising = 6,

    /// <summary>
    /// Not really an id, but contains the limit ad tracking value ("0" indicates limited ad tracking).
    /// </summary>
    IOSAdTracking = 7
#endif
	}
}