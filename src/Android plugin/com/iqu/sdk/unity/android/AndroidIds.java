package com.iqu.sdk.unity.android;

import android.content.Context;
import android.os.Build;
import android.provider.Settings.Secure;

import java.lang.reflect.Method;

/**
 * AndroidIds implements various methods to obtain Android specific ID values.
 */
public class AndroidIds {
	//
	// PRIVATE VARS
	//

	/**
	 * Contains the advertising id
	 */
	private static String m_advertisingId;

	/**
	 * Indicates if ad tracking should be limited.
	 */
	private static Boolean m_limitedAdTracking;

	/**
	 * Result of retrieving the advertising id
	 */
	private static int m_result = -2;

	/**
	 * Result of retrieving the Android ID (null means it still needs to be
	 * retrieved)
	 */
	private static String m_androidId = null;

	/**
	 * Context to use when retrieving the id
	 */
	private static Context m_context;

	//
	// PUBLIC METHODS
	//

	/**
	 * Start retrieving the advertising id information. Information is retrieved
	 * using a separate thread and reflection, so code will not generate errors
	 * if google play service jar is missing.
	 * 
	 * @param aContext
	 *            : Context instance to use
	 */
	public static void start(Context aContext) {
		m_result = -1;
		m_context = aContext;
		new Thread(new Runnable() {
			@Override
			public void run() {
				// See sample code at
				// http://developer.android.com/google/play-services/id.html
				try {
					// use reflection to call classes (in case SDK user did not
					// include
					// Google Play jar files).
					@SuppressWarnings("rawtypes")
					Class adIdClientClass = Class
							.forName("com.google.android.gms.ads.identifier.AdvertisingIdClient");
					@SuppressWarnings("unchecked")
					Method getAdvertisingIdInfoMethod = adIdClientClass
							.getDeclaredMethod("getAdvertisingIdInfo",
									Context.class);
					Object adInfo = getAdvertisingIdInfoMethod.invoke(null,
							m_context);
					@SuppressWarnings("rawtypes")
					Class adInfoClass = adInfo.getClass();
					@SuppressWarnings("unchecked")
					Method getIdMethod = adInfoClass.getDeclaredMethod("getId");
					@SuppressWarnings("unchecked")
					Method isLimitAdTrackingEnabledMethod = adInfoClass
							.getDeclaredMethod("isLimitAdTrackingEnabled");
					m_advertisingId = getIdMethod.invoke(adInfo).toString();
					m_limitedAdTracking = ((Boolean) isLimitAdTrackingEnabledMethod
							.invoke(adInfo)).booleanValue();
					m_result = 0;
				} catch (Exception e) {
					// check class name
					String name = e.getClass().getName();
					if (name == "IOException") {
						// Unrecoverable error connecting to Google Play
						// services
						// (e.g. the old version of the service doesn't support
						// getting AdvertisingId).
						m_result = 1;
					} else if (name == "GooglePlayServicesNotAvailableException") {
						// Google Play services is not available entirely.
						m_result = 2;
					} else if (name == "GooglePlayServicesRepairableException") {
						// Encountered a recoverable error connecting to Google
						// Play
						// services.
						m_result = 3;
					} else {
						// other error (probably class not found)
						m_result = 4;
					}
					// clear any stored value
					m_advertisingId = "";
					m_limitedAdTracking = false;
				}
			}
		}).start();
	}

	/**
	 * Return serial ID (Build.SERIAL).
	 * 
	 * @return serial ID.
	 */
	public static String getSerial() {
		return Build.SERIAL;
	}

	/**
	 * Return Secure.ANDROID_ID; only available after start has been called.
	 * 
	 * @return Android ID or empty string if it is not available yet.
	 */
	public static String getAndroidId() {
		if (m_androidId == null) {
			if (m_context != null) {
				m_androidId = Secure.getString(m_context.getContentResolver(),
						Secure.ANDROID_ID);
				if (m_androidId == null) {
					m_androidId = "";
				}
			}
		}
		return m_androidId == null ? "" : m_androidId;
	}

	/**
	 * Return advertising id.
	 * 
	 * @return advertising id
	 */
	public static String getAdvertisingId() {
		return m_advertisingId;
	}

	/**
	 * Return limited ad tracking setting.
	 * 
	 * @return limited ad tracking
	 */
	public static int getLimitedAdTracking() {
		return m_limitedAdTracking ? 1 : 0;
	}

	/**
	 * Get result from retrieving advertising id.
	 * 
	 * The following values can be returned:
	 * <ul>
	 * <li>-2: no retrieval has been started</li>
	 * <li>-1: busy retrieving</li>
	 * <li>0: successfully retrieved id & limited values</li>
	 * <li>1: IO exception occurred (old version of google player services?)</li>
	 * <li>2: Google Play services is not available entirely.</li>
	 * <li>3: Encountered a recoverable error connecting to Google Play
	 * services.</li>
	 * <li>4: Encountered another error (Google Play Services not available).</li>
	 * </ul>
	 * 
	 * @return -2, -1, 0, 1, 2, 3 or 4
	 */
	public static int getAdvertisingResult() {
		return m_result;
	}
}
