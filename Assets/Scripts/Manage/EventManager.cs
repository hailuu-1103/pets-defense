using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// My event type.
/// </summary>
[System.Serializable]
public class MyEvent : UnityEvent<GameObject, string>
{

}

/// <summary>
/// Message system.
/// </summary>
public class EventManager : MonoBehaviour
{
    public static EventManager instance
    {
        get
        {
            if (!eventManager)
            {
                eventManager = FindObjectOfType(typeof(EventManager)) as EventManager;
                if (!eventManager)
                {
                    Debug.LogError("There needs to be one active EventManger script on a GameObject in your scene.");
                }
                else
                {
                    eventManager.Init();
                }
            }
            return eventManager;
        }
    }

    // Events list
    private Dictionary<string, MyEvent> eventDictionary;
    // Instance of event manager
    private static EventManager eventManager;

    /// <summary>
    /// Init this instance.
    /// </summary>
    void Init()
    {
        if (eventDictionary == null)
        {
            eventDictionary = new Dictionary<string, MyEvent>();
        }
    }

    /// <summary>
    /// Start listening specified event.
    /// </summary>
    /// <param name="eventName">Event name.</param>
    /// <param name="listener">Listener.</param>
    public static void StartListening(string eventName, UnityAction<GameObject, string> listener)
    {
        MyEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new MyEvent();
            thisEvent.AddListener(listener);
            instance.eventDictionary.Add(eventName, thisEvent);
        }
    }

    /// <summary>
    /// Stop listening specified event.
    /// </summary>
    /// <param name="eventName">Event name.</param>
    /// <param name="listener">Listener.</param>
    public static void StopListening(string eventName, UnityAction<GameObject, string> listener)
    {
        if (eventManager == null) return;
        MyEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    /// <summary>
    /// Trigger specified event.
    /// </summary>
    /// <param name="eventName">Event name.</param>
    /// <param name="obj">Object.</param>
    /// <param name="param">Parameter.</param>
    public static void TriggerEvent(string eventName,GameObject obj, string param)
    {
        MyEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke(obj, param);
        }
    }
}
