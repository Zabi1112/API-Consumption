using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class NetworkUtility
{
    private static List<IBackendEventsInterface> _listOfCallbacks = new();

    private static readonly ConcurrentQueue<BackendEventType> _eventsQueue = new ConcurrentQueue<BackendEventType>();
    private static readonly object _queueLock = new object();
    private static readonly AutoResetEvent _eventAvailable = new AutoResetEvent(false);

    private static int _fireEventQueueDaemonTimeout = 100;

    public static void SubscribeToEvents(IBackendEventsInterface callbackInterface)
    {
        try
        {
            if (_listOfCallbacks.Contains(callbackInterface))
            {
                throw new Exception("Callback Already Exists Not Adding Again");
            }

            _listOfCallbacks.Add(callbackInterface);
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            throw;
        }
    }

    public static void UnsubscribeToEvents(IBackendEventsInterface callbackInterface)
    {
        try
        {
            _listOfCallbacks.Remove(callbackInterface);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public static string GetFileNameFromUrl(string url)
    {
        int index = url.LastIndexOf('/');
        if (index >= 0 && index < url.Length - 1)
        {
            return url.Substring(index + 1);
        }

        return "";
    }

    public static void FireEvents(BackendEventType typeOfEvent)
    {
        foreach (IBackendEventsInterface eventsInterface in _listOfCallbacks)
        {
            try
            {
                eventsInterface.CallEvent(typeOfEvent);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }


    public static void AddEventToQueue(BackendEventType eventType)
    {
        lock (_queueLock)
        {
            _eventsQueue.Enqueue(eventType);
            _eventAvailable.Set(); // Signal the worker thread that an event is available
        }

        EventProcessingWorker();
    }

    public static void EventProcessingWorker()
    {
        BackendEventType eventType;
        // Wait until an event is available or until a timeout occurs
        if (_eventAvailable.WaitOne(_fireEventQueueDaemonTimeout))
        {
            lock (_queueLock)
            {
                // Dequeue and process the event
                if (_eventsQueue.TryDequeue(out eventType))
                {
                    FireEvents(eventType);
                    if (_eventsQueue.Count > 0)
                        EventProcessingWorker();
                }
            }
        }
    }


    public static string GetErrorHeadingForErrorCode(ResponseCodes code)
    {
        return code switch
        {
            ResponseCodes.Success => "Execution Success",
            ResponseCodes.ConnectionError => "Connection error",
            ResponseCodes.InternalServerError => "Internal Error",
            ResponseCodes.SessionExpire => "Auth Session Expire",
            ResponseCodes.InsufficientResource => "Insufficient Resource",
            ResponseCodes.InvalidInput => "Invalid Input Prams",
            ResponseCodes.ItemNotFound => "Item Not Found",
            _ => String.Empty
        };
    }


    public static bool IsInternetAvailable(Action cb = null)
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            //  GlobalUIManager.Instance.ShowGenericPanel("ERROR", "Internet Connection Failure! ", "", PopUpIconType.None);
            return false;
        }
        else
        {
            return true;
        }

        cb?.Invoke();
    }
}

public enum ResponseCodes
{
    Success = 0,
    ConnectionError = 1,
    InternalServerError = 2,
    SessionExpire = 3,
    InvalidInput = 4,
    InsufficientResource = 5,
    ItemNotFound = 6,
    PlayerNotFound = 7,
    UnknownError = 8,
}

public enum BackendEventType
{
    PlayerUpdate, //the player specs such as name were updated, if you have playForFunUI to update or something else do it 
    GlobalInventoryUpdate, //the global inventory specs such as name were updated, if you have playForFunUI to update or something else do it
    GameSpecificInventoryUpdate, //the game specific inventory specs such as name were updated, for exmaple the tickets, boosters,daily free tickets etc//
    GameSpecificStatUpdate, //Game stats were upated//
    GameSpecificShopUpdate,
    NewPlayerCreated,
    LeaderboardUpdate,
    PlayerLoggedIn,
    StatesModelUpdated,
    PlayerMessagesUpdated,
    AllUserDataFetched,
}

public enum ApiCallType
{
    None,
    PostAsJson,
    PostAsBody,
    PostAsQuery,
    GetAsQuery,
    GetAsUrl,
}