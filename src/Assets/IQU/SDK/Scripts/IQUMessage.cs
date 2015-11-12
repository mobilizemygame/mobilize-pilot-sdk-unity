using System;
using System.Collections.Generic;
using System.IO;

namespace IQU.SDK
{
  /// <summary>
  /// IQUMessage encapsulates a single message. A message exists of an event and several ids. 
  /// The event will never change, the ids might get updated before the message is sent to the server.
  /// </summary>
  internal class IQUMessage
  {
    #region Private vars

    /// <summary>
    /// See property definition.
    /// </summary>
    private IQUMessage m_next;

    /// <summary>
    /// The event (as JSON string)
    /// </summary>
    private string m_event;

    /// <summary>
    /// The event type.
    /// </summary>
    private string m_eventType;

    /// <summary>
    /// The ids.
    /// </summary>
    private IQUIds m_ids;

    /// <summary>
    /// Queue message is currently in.
    /// </summary>
    private IQUMessageQueue m_queue;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new message instance and set the ids and event.
    /// </summary>
    /// <param name="anIds">Ids to use (a copy is stored)</param>
    /// <param name="anEvent">Event the message encapsulates</param>
    internal IQUMessage(IQUIds anIds, IDictionary<string, object> anEvent)
    {
      // store event as JSON string (no need to convert it every time)
      this.m_event = MiniJSON.Json.Serialize(anEvent);
      // get type
      this.m_eventType = anEvent.ContainsKey("type") ? anEvent["type"].ToString() : "";
      // use copy of ids.
      this.m_ids = anIds.Clone();
      // no queue
      this.m_queue = null;
    }

    /// <summary>
    /// Initializes an empty message instance.
    /// </summary>
    internal IQUMessage()
    {
      this.m_event = "";
      this.m_eventType = "";
      this.m_ids = new IQUIds();
      this.m_queue = null;
    }

    #endregion

    #region Internal methods

    /// <summary>
    /// Removes references and resources.
    /// </summary>
    internal void Destroy()
    {
      this.m_next = null;
      this.m_queue = null;
      if (this.m_ids != null)
      {
        this.m_ids.Destroy();
        this.m_ids = null;
      }
    }

    /// <summary>
    /// Save message data. This will reset the dirty stored state.
    /// </summary>
    /// <param name="aWriter">Writer to save data with</param>
    internal void Save(BinaryWriter aWriter)
    {
      aWriter.Write(this.m_event);
      aWriter.Write(this.m_eventType);
      this.m_ids.Save(aWriter);
    }

    /// <summary>
    /// Load message data.
    /// </summary>
    /// <param name="aReader">Reader to read data from</param>
    internal void Load(BinaryReader aReader)
    {
      this.m_event = aReader.ReadString();
      this.m_eventType = aReader.ReadString();
      this.m_ids.Load(aReader);
    }

    /// <summary>
    /// Update an id with a new value. For certain types the id only gets updated
    /// if it is currently empty.
    /// </summary>
    /// <param name="aType">Type to update</param>
    /// <param name="aNewValue">New value to use</param>
    internal void UpdateId(IQUIdType aType, String aNewValue)
    {
      // get current value and exit for certain types if the current value is
      // not empty.
      String currentValue = this.m_ids.Get(aType);
      switch (aType)
      {
        case IQUIdType.Custom:
        case IQUIdType.Facebook:
        case IQUIdType.Twitter:
        case IQUIdType.GooglePlus:
        case IQUIdType.SDK:
          if (currentValue.Length > 0)
          {
            return;
          }
          break;
      }
      if (currentValue != aNewValue)
      {
        this.m_ids.Set(aType, aNewValue);
        // message changed
        if (this.m_queue != null)
        {
          this.m_queue.HandleMessageChanged(this);
        }
      }
    }

    /// <summary>
    /// Returns the ids and event as JSON formatted string, using the following
    /// format:
    /// <p>
    /// { "identifiers":{..}, "event":{..} }
    /// </p>
    /// <p>
    /// The dirty JSON state will be reset.
    /// </p>
    /// </summary>
    /// <returns>JSON formatted object definition string</returns>
    internal string ToJSONString()
    {
      return "{" + "\"identifiers\":" + this.m_ids.ToJSONString() + "," + "\"event\":"
        + this.m_event + "}";
    }

    #endregion

    #region Internal properties

    /// <summary>
    /// The next property contains the next message in the linked list chain.
    /// </summary>
    internal IQUMessage Next
    {
      get
      {
        return this.m_next;
      }
      set
      {
        this.m_next = value;
      }
    }

    /// <summary>
    /// The queue property contains the queue the message is currently part of.
    /// </summary>
    internal IQUMessageQueue Queue
    {
      get
      {
        return this.m_queue;
      }
      set
      {
        this.m_queue = value;
      }
    }

    /// <summary>
    /// The eventType property contains the type of event or an empty string if
    /// the type could not be determined.
    /// </summary>
    internal string EventType
    {
      get
      {
        return this.m_eventType;
      }
    }

    #endregion
  }
}