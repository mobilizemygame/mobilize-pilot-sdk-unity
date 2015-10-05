using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using IQU.SDK;

/// <summary>
/// Example1 implements code to test all aspects of the IQU SDK.
/// </summary>
public class example1 : MonoBehaviour
{
  #region Private vars

  /// <summary>
  /// Reference to control panel.
  /// </summary>
  private GameObject m_panelControl;

  /// <summary>
  /// Reference to ids panel.
  /// </summary>
  private GameObject m_panelIds;

  /// <summary>
  /// Reference to log panel.
  /// </summary>
  private GameObject m_panelLog;

  /// <summary>
  /// Reference to caption of toggle payable button.
  /// </summary>
  private Text m_captionTogglePayable;

  /// <summary>
  /// Reference to initialized text component.
  /// </summary>
  private Text m_textInitialized;

  /// <summary>
  /// Reference to enabled text component.
  /// </summary>
  private Text m_textEnabled;

  /// <summary>
  /// Reference to payable text component.
  /// </summary>
  private Text m_textPayableState;

  /// <summary>
  /// Reference to server available text component.
  /// </summary>
  private Text m_textServerAvailable;

  /// <summary>
  /// Reference to normal network toggle component.
  /// </summary>
  private Toggle m_toggleNormalNetwork;

  /// <summary>
  /// Reference to simulate server toggle component.
  /// </summary>
  private Toggle m_toggleSimulateServer;

  /// <summary>
  /// Reference to simulate offline toggle component.
  /// </summary>
  private Toggle m_toggleSimulateOffline;

  /// <summary>
  /// Reference to ids text component.
  /// </summary>
  private Text m_textIds;

  /// <summary>
  /// Reference to log text component.
  /// </summary>
  private Text m_textLog;

  /// <summary>
  /// Reference to control button component.
  /// </summary>
  private Button m_buttonControl;

  /// <summary>
  /// Reference to ids button component.
  /// </summary>
  private Button m_buttonIds;

  /// <summary>
  /// Reference to log button component.
  /// </summary>
  private Button m_buttonLog;

  /// <summary>
  /// Current active panel
  /// </summary>
  private GameObject m_activePanel;

  /// <summary>
  /// Copy of normal color of a tab button.
  /// </summary>
  private ColorBlock m_colorNormal;

  /// <summary>
  /// Copy of highlighted color of a tab button.
  /// </summary>
  private ColorBlock m_colorHighlighted;

  #endregion

  #region Public event handlers

  /// <summary>
  /// Handles click on the toggle payable button.
  /// </summary>
  public void HandleTogglePayableClick()
  {
    IQUSDK.Instance.Payable = !IQUSDK.Instance.Payable;
  }

  /// <summary>
  /// Handles click on the revenue button.
  /// </summary>
  public void HandleRevenueClick()
  {
    IQUSDK.Instance.TrackRevenue(10.5f, "EUR");
  }

  /// <summary>
  /// Handles click on the purchase button.
  /// </summary>
  public void HandlePurchaseClick()
  {
    IQUSDK.Instance.TrackItemPurchase("Item");
  }

  /// <summary>
  /// Handles click on the tutorial button.
  /// </summary>
  public void HandleTutorialClick()
  {
    IQUSDK.Instance.TrackTutorial("Step 1");
  }

  /// <summary>
  /// Handles click on the milestone button.
  /// </summary>
  public void HandleMilestoneClick()
  {
    IQUSDK.Instance.TrackMilestone("Milestone", "first");
  }

  /// <summary>
  /// Handles click on the marketing button.
  /// </summary>
  public void HandleMarketingClick()
  {
    IQUSDK.Instance.TrackMarketing("Partner", "Campaign", null, null, null);
  }

  /// <summary>
  /// Handles click on the attribute button.
  /// </summary>
  public void HandleAttributeClick()
  {
    IQUSDK.Instance.TrackUserAttribute("gender", "Female");
  }

  /// <summary>
  /// Handles click on the country button.
  /// </summary>
  public void HandleCountryClick()
  {
    IQUSDK.Instance.TrackCountry("NL");
  }

  /// <summary>
  /// Handles click on the facebook button.
  /// </summary>
  public void HandleFacebookClick()
  {
    IQUSDK.Instance.SetFacebookId("Dummy Facebook ID");
  }

  /// <summary>
  /// Handles click on the normal network radio button.
  /// </summary>
  public void HandleNormalNetworkClick()
  {
    IQUSDK.Instance.TestMode = IQUTestMode.None;
  }

  /// <summary>
  /// Handles click on the simulate server radio button.
  /// </summary>
  public void HandleSimulateServerClick()
  {
    IQUSDK.Instance.TestMode = IQUTestMode.SimulateServer;
  }

  /// <summary>
  /// Handles click on the simulate offline radio button.
  /// </summary>
  public void HandleSimulateOfflineClick()
  {
    IQUSDK.Instance.TestMode = IQUTestMode.SimulateOffline;
  }

  /// <summary>
  /// Handles click on the control button.
  /// </summary>
  public void HandleControlClick()
  {
    this.SelectPanel(this.m_panelControl);
  }

  /// <summary>
  /// Handles click on the ids button.
  /// </summary>
  public void HandleIdsClick()
  {
    this.SelectPanel(this.m_panelIds);
  }

  /// <summary>
  /// Handles click on the log button.
  /// </summary>
  public void HandleLogClick()
  {
    this.SelectPanel(this.m_panelLog);
  }

  #endregion

  #region Private methods

  /// <summary>
  /// Finds an object within all the children (including inactive ones).
  /// </summary>
  /// <returns>The object or null if none could be found.</returns>
  /// <paran manem="aParent">Parent to start searching in</param>
  /// <param name="aName">Name of game object.</param>
  private GameObject FindChildGameObject(GameObject aParent, string aName)
  {
    Transform[] childTransforms = aParent.GetComponentsInChildren<Transform>(true);
    foreach(Transform childTransform in childTransforms ){
      if(childTransform.name == aName){
        return childTransform.gameObject;
      }
    }
    return null;
  }

  /// <summary>
  /// Finds a GameObject with a certain name (including inactive ones). 
  /// This method will not find inactive game objects that are in hierarchy 
  /// where the root object is inactive.
  /// </summary>
  /// <returns>The found game object or null if none could be found.</returns>
  /// <param name="aName">The name to search for.</param>
  private GameObject FindGameObject(string aName)
  {
    GameObject[] gameObjects = FindObjectsOfType(typeof(GameObject)) as GameObject[];
    foreach(GameObject gameObject in gameObjects)
    {
      // first check if game object might be the one that is wanted.
      if (gameObject.name == aName) return gameObject;
      // root object?
      if (gameObject.transform.parent == null) {
        // yes, search trough all children
        GameObject result = this.FindChildGameObject(gameObject, aName);
        if (result != null)
        {
          return result;
        }
      }
    }
    return null;
  }

  /// <summary>
  /// Activate a certain panel and deactive the others.
  /// </summary>
  /// <param name="aPanel">Panel to make active.</param>
  private void SelectPanel(GameObject aPanel)
  {
    this.m_panelControl.SetActive(aPanel == this.m_panelControl);
    this.m_panelIds.SetActive(aPanel == this.m_panelIds);
    this.m_panelLog.SetActive(aPanel == this.m_panelLog);
    this.m_activePanel = aPanel;
    // update visual state of buttons
    this.m_buttonControl.colors = aPanel == this.m_panelControl ? this.m_colorHighlighted : this.m_colorNormal;
    this.m_buttonIds.colors = aPanel == this.m_panelIds ? this.m_colorHighlighted : this.m_colorNormal;
    this.m_buttonLog.colors = aPanel == this.m_panelLog ? this.m_colorHighlighted : this.m_colorNormal;
  }

  /// <summary>
  /// Updates the visual state of the control panel components.
  /// </summary>
  private void UpdateControlPanel()
  {
    this.m_textEnabled.text = IQUSDK.Instance.AnalyticsEnabled ? "yes" : "no";
    this.m_textInitialized.text = IQUSDK.Instance.Initialized ? "yes" : "no";
    this.m_textServerAvailable.text = IQUSDK.Instance.ServerAvailable ? "yes" : "no";
    this.m_textPayableState.text = IQUSDK.Instance.Payable ? "on" : "off";
    this.m_captionTogglePayable.text = IQUSDK.Instance.Payable ? "stop" : "start";
    this.m_toggleNormalNetwork.isOn = IQUSDK.Instance.TestMode == IQUTestMode.None;
    this.m_toggleSimulateOffline.isOn = IQUSDK.Instance.TestMode == IQUTestMode.SimulateOffline;
    this.m_toggleSimulateServer.isOn = IQUSDK.Instance.TestMode == IQUTestMode.SimulateServer;
  }

  /// <summary>
  /// Updates the visual state of the ids panel components.
  /// </summary>
  private void UpdateIdsPanel()
  {
    string list = "";
    foreach (IQUIdType type in Enum.GetValues(typeof(IQUIdType)))
    {
      list = list + type.ToString() + ":\n" + IQUSDK.Instance.GetId(type) + "\n\n";
    }
    this.m_textIds.text = list;
  }

  /// <summary>
  /// Updates the visual state of the log panel components.
  /// </summary>
  private void UpdateLogPanel()
  {
    this.m_textLog.text = IQUSDK.Instance.Log;
  }

  #endregion

  #region Unity methods

  /// <summary>
  /// Starts this instance, initialize private variables.
  /// </summary>
  void Start()
  {
    this.m_textEnabled = this.transform.Find("/Canvas/ControlPanel/EnabledText").GetComponent<Text>();
    this.m_textInitialized = this.transform.Find("/Canvas/ControlPanel/InitializedText").GetComponent<Text>();
    this.m_textPayableState = this.transform.Find("/Canvas/ControlPanel/PayableStateText").GetComponent<Text>();
    this.m_textServerAvailable = this.transform.Find("/Canvas/ControlPanel/ServerAvailableText").GetComponent<Text>();
    this.m_captionTogglePayable = this.transform.Find("/Canvas/ControlPanel/TogglePayableButton/Text").GetComponent<Text>();
    this.m_toggleNormalNetwork = this.transform.Find("/Canvas/ControlPanel/NormalNetworkToggle").GetComponent<Toggle>();
    this.m_toggleSimulateOffline = this.transform.Find("/Canvas/ControlPanel/SimulateOfflineToggle").GetComponent<Toggle>();
    this.m_toggleSimulateServer = this.transform.Find("/Canvas/ControlPanel/SimulateServerToggle").GetComponent<Toggle>();
    this.m_panelControl = this.transform.Find("/Canvas/ControlPanel").gameObject;
    this.m_panelIds = this.FindGameObject("IdsPanel");
    this.m_panelLog = this.FindGameObject("LogPanel");
    // use found game objects to get reference (transform.Find will not work because panels are inactive)
    this.m_textLog = this.m_panelLog.transform.Find("LogText").GetComponent<Text>();
    this.m_textIds = this.m_panelIds.transform.Find("IdsText").GetComponent<Text>();
    this.m_buttonControl = this.transform.Find("/Canvas/ControlButton").GetComponent<Button>();
    this.m_buttonIds = this.transform.Find("/Canvas/IdsButton").GetComponent<Button>();
    this.m_buttonLog = this.transform.Find("/Canvas/LogButton").GetComponent<Button>();
    this.m_colorNormal = this.m_buttonControl.colors;
    this.m_colorHighlighted = this.m_buttonControl.colors;
    this.m_colorHighlighted.normalColor = this.m_colorHighlighted.highlightedColor;
    // make sure control panel is selected
    this.SelectPanel(this.m_panelControl);
  }
  
  /// <summary>
  /// Updates the visual state, this method is called every frame.
  /// </summary>
  void Update()
  {
    // handle escape key
    if (Input.GetKeyDown(KeyCode.Escape))
    {
      if (this.m_activePanel == this.m_panelControl)
      {
        Application.Quit();
      }
      else
      {
        this.SelectPanel(this.m_panelControl);
      }
    }
    // only update content of active panel
    if (this.m_activePanel == this.m_panelControl)
    {
      this.UpdateControlPanel();
    }
    else if (this.m_activePanel == this.m_panelIds)
    {
      this.UpdateIdsPanel();
    }
    else if (this.m_activePanel == this.m_panelLog)
    {
      this.UpdateLogPanel();
    }
  }

  #endregion
}
