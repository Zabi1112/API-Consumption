using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;


public static class ApiHttpQueueUtility
{
    private static readonly ConcurrentQueue<ApiBuilder> _apiCallerQueue = new ConcurrentQueue<ApiBuilder>();
    private static readonly object _apiCallerQueueLock = new object();
    private static readonly AutoResetEvent _apiCallerAvailable = new AutoResetEvent(false);

    private static int _fireEventQueueDaemonTimeout = 200;

    private static int _countOfRequestsSent = 0;

    private static int CountOfRequestsSent
    {
        get => _countOfRequestsSent;
        set
        {
            _countOfRequestsSent = value;
            //    Debug.Log("Requests Sound:"+_countOfRequestsSent);
        }
    }


    public static void AddEApiCallToQueue(ApiBuilder eventType)
    {
        lock (_apiCallerQueueLock)
        {
            _apiCallerQueue.Enqueue(eventType);
            _apiCallerAvailable.Set(); // Signal the worker thread that an event is available
        }

        EventProcessingWorker();
    }

    static void ExecuteApiCallerQueuedMethod(ApiBuilder typeOfCall)
    {
        try
        {
            if (typeOfCall.callType == ApiCallType.None) return;

            if (typeOfCall.callType == ApiCallType.PostAsQuery)
            {
                ApiHttpUtility.PostApiCallAsQuery(typeOfCall);
            }
            else if (typeOfCall.callType == ApiCallType.PostAsJson)
            {
                ApiHttpUtility.PostApiCallAsJson(typeOfCall);
            }
            else if (typeOfCall.callType == ApiCallType.PostAsBody)
            {
                ApiHttpUtility.PostApiCallAsForm(typeOfCall);
            }
            else if (typeOfCall.callType == ApiCallType.PostAsJson)
            {
                ApiHttpUtility.GetApiCallAsQuery(typeOfCall);
            }
            else
            {
                Debug.LogError("There is no any api call to execute from api caller queue");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public  static void EventProcessingWorker()
    {
        ApiBuilder eventType;
        // Wait until an event is available or until a timeout occurs
        if (_apiCallerAvailable.WaitOne(_fireEventQueueDaemonTimeout))
        {
            lock (_apiCallerQueueLock)
            {
                // Dequeue and process the event
                if (_apiCallerQueue.TryDequeue(out eventType))
                {
                    ExecuteApiCallerQueuedMethod(eventType);
                    
                    Thread.Sleep(1000); // Delay for 2000 milliseconds (2 second)
                    while (!eventType.isCallExited) // check if if
                    {
                        // Introduce a delay
                        Thread.Sleep(1000); // Delay for 1000 milliseconds (1 second)
                        if (_apiCallerQueue.Count > 0)
                            EventProcessingWorker();
                    }
                }
            }
        }
    }
}