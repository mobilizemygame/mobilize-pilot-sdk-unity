using System;
using System.Text;
using System.IO;
using UnityEngine;

namespace IQU.SDK
{
  /// <summary>
  /// IQUMessageQueue contains a list of IQUMessage instances. It can store the
  /// messages to a local storage and return the whole list as a JSON string.
  ///
  /// For WEBPLAYER targets the messages are stored in the persistent storage (via IQULocalStorage); 
  /// for other targets the messages are saved to a binary file.
  /// </summary>
  internal class IQUMessageQueue
  {
    #region Private consts

    /// <summary>
    /// Name of file (or local storage key) to store the messages in.
    /// </summary>
    private const string FileName = "IQUSDK_messages.bin";

    /// <summary>
    /// Version of stored data. This value should be increased whenever the
    /// format of the stored messages changes.
    /// </summary>
    private const int FileVersion = 1;

    #endregion

    #region Private vars

    /// <summary>
    /// Points to first message in chain.
    /// </summary>
    private IQUMessage m_first;

    /// <summary>
    /// Points to last message in chain.
    /// </summary>
    private IQUMessage m_last;

    /// <summary>
    /// Cached JSON string.
    /// </summary>
    private string m_cachedJSONString;

    /// <summary>
    /// When true the JSON string should be recreated.
    /// </summary>
    private bool m_dirtyJSON;

    /// <summary>
    /// When true the queue should save itself to persistent storage.
    /// </summary>
    private bool m_dirtyStored;

    #endregion

    #region Constructors


    /// <summary>
    /// Initializes the instance to an empty queue.
    /// </summary>
    internal IQUMessageQueue()
    {
      this.reset();
    }

    #endregion

    #region Internal methods

    /// <summary>
    /// Cleans up references and used resources.
    /// </summary>
    internal void Destroy()
    {
      this.Clear(false);
    }

    /// <summary>
    /// Checks if the queue is empty.
    /// </summary>
    /// <returns><code>true</code> when there are no messages in the queue.</returns>
    internal bool IsEmpty()
    {
      return this.m_last == null;
    }

    /// <summary>
    /// Counts the number of messages in the queue.
    /// </summary>
    /// <returns>number of messages</returns>
    internal int GetCount()
    {
      int result = 0;
      for (IQUMessage message = this.m_first; message != null; message = message.Next)
      {
        result++;
      }
      return result;
    }

    /// <summary>
    /// Add a message to end of queue.
    /// </summary>
    /// <param name="aMessage">The message to add to the end.</param>
    internal void Add(IQUMessage aMessage)
    {
      if (this.m_first == null)
      {
        this.m_first = aMessage;
      }
      if (this.m_last != null)
      {
        this.m_last.Next = aMessage;
      }
      this.m_last = aMessage;
      // update queue property so the IQUMessage will call onMessageChanged
      aMessage.Queue = this;
      // queue has changed.
      this.m_dirtyJSON = true;
      this.m_dirtyStored = true;
    }

    /// <summary>
    /// Prepend a queue before the current queue. This will move the items from
    /// aQueue to this queue.
    /// <p>
    /// After this call, aQueue will be empty.
    /// </p>
    /// 
    /// @param aQueue
    ///            
    /// @param aChangeQueue
    ///            
    /// </summary>
    /// <param name="aQueue">The queue to prepend before this queue.</param>
    /// <param name="aChangeQueue"> When <code>true</code> change the queue
    /// property in every message to this queue.</param>
    internal void Prepend(IQUMessageQueue aQueue, bool aChangeQueue)
    {
      if (!aQueue.IsEmpty())
      {
        // if this queue is empty, copy cached JSON string and dirty state;
        // else reset it.
        if (this.m_first == null)
        {
          this.m_cachedJSONString = aQueue.m_cachedJSONString;
          this.m_dirtyJSON = aQueue.m_dirtyJSON;
          this.m_dirtyStored = aQueue.m_dirtyStored;
        }
        else
        {
          this.m_cachedJSONString = null;
          this.m_dirtyJSON = true;
          this.m_dirtyStored = true;
        }
        // get first and last
        IQUMessage first = aQueue.m_first;
        IQUMessage last = aQueue.m_last;
        // this queue is empty?
        if (this.m_last == null)
        {
          // yes, just copy last
          this.m_last = last;
        }
        else
        {
          // add the first message in the chain to the chain in aQueue
          last.Next = this.m_first;
        }
        // chain starts now with the first message in the chain of aQueue
        this.m_first = first;
        // update queue property?
        if (aChangeQueue)
        {
          for (IQUMessage message = first; message != null; message = message.Next)
          {
            message.Queue = this;
          }
        }
        // aQueue is now empty
        aQueue.reset();
      }
    }

    /// <summary>
    /// Destroy the queue. It will call destroy on every message and remove any
    /// reference to each message instance.
    /// <p>
    /// After this method, the queue will be empty and can be filled again.
    /// </p>
    /// </summary>
    /// <param name="aClearStorage">When <code>true</code> clear the persistently stored messages.</param>
    internal void Clear(bool aClearStorage)
    {
      IQUMessage message = this.m_first;
      while (message != null)
      {
        IQUMessage next = message.Next;
        message.Destroy();
        message = next;
      }
      this.reset();
      // delete the local file
      if (aClearStorage)
      {
        this.DeleteStorage();
      }
    }

    /// <summary>
    /// Clears the references to the messages, but don't destroy the messages
    /// themselves.
    /// <p>
    /// After this method the queue will be empty.
    /// </p>
    /// </summary>
    internal void reset()
    {
      this.m_first = null;
      this.m_last = null;
      this.m_cachedJSONString = null;
      this.m_dirtyJSON = false;
      this.m_dirtyStored = false;
    }

    /// <summary>
    /// Saves the messages to persistent storage. This method only performs the
    /// save if new messages have been added or one of the messages changed.
    /// </summary>
    internal void Save()
    {
      // only store if at least one messages stored state is dirty
      if (this.m_dirtyStored)
      {
        try
        {
#if UNITY_WEBPLAYER
          MemoryStream stream = new MemoryStream();
#else
          string fileName = Application.persistentDataPath + "/" + IQUMessageQueue.FileName;
          FileStream stream = new FileStream(fileName, FileMode.Create);
#endif
          BinaryWriter writer = new BinaryWriter(stream);
          writer.Write(IQUMessageQueue.FileVersion);
          int count = this.GetCount();
          writer.Write(count);
          for (IQUMessage message = this.m_first; message != null; message = message.Next)
          {
            message.Save(writer);
          }
          writer.Flush();
#if UNITY_WEBPLAYER
          IQUSDK.Instance.LocalStorage.SetBytes(IQUMessageQueue.FileName, stream.GetBuffer());
#endif
          stream.Close();
          IQUSDK.Instance.AddLog("[Queue] saved " + count + " messages.");
        }
        catch (Exception error)
        {
          IQUSDK.Instance.AddLog("[Queue][Error] while saving: " + error.Message);
        }
        this.m_dirtyStored = false;
      }
    }

    /// <summary>
    /// Loads the messages from persistent storage.
    /// </summary>
    internal void Load()
    {
      this.Clear(false);
      try
      {
#if UNITY_WEBPLAYER
        // exit if there is no stored data
        if (!IQUSDK.Instance.LocalStorage.HasKey(IQUMessageQueue.FileName)) 
        {
          return;
        }
        MemoryStream stream = new MemoryStream(IQUSDK.Instance.LocalStorage.GetBytes(IQUMessageQueue.FileName)));
#else
        // exit if there is no file
        string fileName = Application.persistentDataPath + "/" + IQUMessageQueue.FileName;
        if (!File.Exists(fileName))
        {
          return;
        }
        FileStream stream = new FileStream(fileName, FileMode.Open);
#endif
        BinaryReader reader = new BinaryReader(stream);
        int version = reader.ReadInt32();
        // correct version?
        if (version == IQUMessageQueue.FileVersion)
        {
          // get number of stored messages and load the messages
          int count = reader.ReadInt32();
          for (int index = 0; index < count; index++)
          {
            IQUMessage message = new IQUMessage();
            message.Load(reader);
            this.Add(message);
          }
          reader.Close();
          stream.Close();
          IQUSDK.Instance.AddLog("[Queue] loaded " + count + " messages.");
        }
        else
        {
          // unsupported version, close and delete storage (no longer usefull)
          reader.Close();
          stream.Close();
          this.DeleteStorage();
        }
        // no need to store the just loaded messages
        this.m_dirtyStored = false;
      }
      catch (Exception error)
      {
        IQUSDK.Instance.AddLog("[Queue][Error] while loading: " + error.Message);
      }
    }

    /// <summary>
    /// Returns the queue as a JSON formatted string.
    /// </summary>
    /// <returns>JSON formatted string.</returns>
    internal String ToJSONString()
    {
      // rebuild string if there is none or one or more messages became dirty.
      if ((this.m_cachedJSONString == null) || this.m_dirtyJSON)
      {
        this.m_cachedJSONString = this.BuildJSONString();
        this.m_dirtyJSON = false;
      }
      return this.m_cachedJSONString;
    }

    /// <summary>
    /// Updates an id within all the stored messages.
    /// </summary>
    /// <param name="aType">Id type to update value for.</param>
    /// <param name="aNewValue">New value to use.</param>
    internal void UpdateId(IQUIdType aType, String aNewValue)
    {
      for (IQUMessage message = this.m_first; message != null; message = message.Next)
      {
        message.UpdateId(aType, aNewValue);
      }
    }

    /// <summary>
    /// Checks if queue contains at least one message for a certain event type.
    /// 
    /// @param aType
    ///            Event type to check
    /// 
    /// @return <code>true</code> if there is at least one message,
    ///         <code>false</code> if not.
    /// </summary>
    internal bool HasEventType(string aType)
    {
      for (IQUMessage message = this.m_first; message != null; message = message.Next)
      {
        if (message.EventType == aType)
        {
          return true;
        }
      }
      return false;
    }

    #endregion

    #region Event handlers

    /// <summary>
    /// This handler is called by IQUMessage when the contents changes.
    /// </summary>
    /// <param name="aMessage">Message with changed content</param>
    internal void HandleMessageChanged(IQUMessage aMessage)
    {
      this.m_dirtyJSON = true;
      this.m_dirtyStored = true;
    }

    #endregion

    #region Private methods

    /// <summary>
    /// Builds JSON formatted definition string from all messages in the queue.
    /// It creates the following format:
    /// <p>
    /// [ {...},{...},... ]
    /// </p>
    /// </summary>
    /// <returns>JSON formatted definition string.</returns>
    private String BuildJSONString()
    {
      StringBuilder result = new StringBuilder();
      result.Append('[');
      bool notEmpty = false;
      for (IQUMessage message = this.m_first; message != null; message = message.Next)
      {
        if (notEmpty)
        {
          result.Append(',');
        }
        result.Append(message.ToJSONString());
        notEmpty = true;
      }
      result.Append(']');
      return result.ToString();
    }

    /// <summary>
    /// Delete the persistent storage.
    /// </summary>
    private void DeleteStorage()
    {
#if UNITY_WEBPLAYER
      if (IQUSDK.Instance.LocalStorage.HasKey(IQUMessageQueue.FileName)) 
      {
        IQUSDK.Instance.AddLog("[Queue] deleting storage.");
        IQUSDK.Instance.LocalStorage.Delete(IQUMessageQueue.FileName);
      }
#else
      string fileName = Application.persistentDataPath + "/" + IQUMessageQueue.FileName;
      if (File.Exists(fileName))
      {
        IQUSDK.Instance.AddLog("[Queue] deleting storage.");
        File.Delete(fileName);
      }
#endif
    }

    #endregion
  }
}