using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public static class ApiHttpUtility
{
    private static readonly ConcurrentQueue<ApiBuilder> _apiCallerQueue = new ConcurrentQueue<ApiBuilder>();
    private static readonly object _apiCallerQueueLock = new object();
    private static readonly AutoResetEvent _apiCallerAvailable = new AutoResetEvent(false);

    private static int _fireEventQueueDaemonTimeout = 100;

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


    #region Call api with json as params in POST method

    public static void PostApiCallAsJson(ApiBuilder apiBuilder)
    {
        if (apiBuilder is null)
        {
            throw new Exception("apiBuilder is null. Please provide ApiBuilder");
        }

        if (!NetworkUtility.IsInternetAvailable())
        {
            ++apiBuilder.retryCallsCounter;
            // indicator turn on to tell that this api call has exited (success or failed)
            apiBuilder.isCallExited = true;
            throw new Exception("There is no internet");
            apiBuilder.callBack?.Invoke(null);
            return;
        }

        if (apiBuilder.retryTimes > 0) // call retry if we need to retry the apic all on connection or network error
        {
            apiBuilder.apiCaller.StopCoroutine(PostAsJsonWithRetry(apiBuilder));
            apiBuilder.apiCaller.StartCoroutine(PostAsJsonWithRetry(apiBuilder));
        }
        else
        {
            apiBuilder.apiCaller.StopCoroutine(PostAsJson(apiBuilder));
            apiBuilder.apiCaller.StartCoroutine(PostAsJson(apiBuilder));
        }
    }


    private static IEnumerator PostAsJson(ApiBuilder apiBuilder)
    {
        using (UnityWebRequest webRequest = new UnityWebRequest(apiBuilder.apiUrl, UnityWebRequest.kHttpVerbPOST))
        {
            // Set the Content-Type header to application/json
            webRequest.SetRequestHeader("Content-Type", "application/json");

            // Set the Authorization header if needed
            SetHeaderTokens(webRequest);

            string jsonData = JsonConvert.SerializeObject(apiBuilder.queryParams);
            // Attach the JSON data to the request
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();

            yield return webRequest.SendWebRequest();

            ++apiBuilder.retryCallsCounter;
            // indicator turn on to tell that this api call has exited (success or failed)
            apiBuilder.isCallExited = true;
            apiBuilder.callBack?.Invoke(webRequest);
        }
    }

    private static IEnumerator PostAsJsonWithRetry(ApiBuilder apiBuilder)
    {
        while (true)
        {
            using (UnityWebRequest webRequest = new UnityWebRequest(apiBuilder.apiUrl, UnityWebRequest.kHttpVerbPOST))
            {
                // Set the Content-Type header to application/json
                webRequest.SetRequestHeader("Content-Type", "application/json");

                // Set the Authorization header if needed
                SetHeaderTokens(webRequest);

                string jsonData = JsonConvert.SerializeObject(apiBuilder.queryParams);
                // Attach the JSON data to the request
                byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
                webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
                webRequest.downloadHandler = new DownloadHandlerBuffer();

                yield return webRequest.SendWebRequest();

                if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                    webRequest.result == UnityWebRequest.Result.ProtocolError)
                {
                    // Retry the API call if there are remaining retry attempts
                    if (apiBuilder.retryTimes > ++apiBuilder.retryCallsCounter) // counter stack over flow
                    {
                        yield return new WaitForEndOfFrame();
                        continue; // Retry the API call
                    }
                }

                ++apiBuilder.retryCallsCounter;
                // indicator turn on to tell that this api call has exited (success or failed)
                apiBuilder.isCallExited = true;
                apiBuilder.callBack?.Invoke(webRequest);
                break; // Exit the loop
            }
        }
    }

    #endregion

    #region Call api with Form as params in POST method

    public static void PostApiCallAsForm(ApiBuilder apiBuilder)
    {
        if (apiBuilder is null)
        {
            throw new Exception("apiBuilder is null. Please provide ApiBuilder");
        }

        if (!NetworkUtility.IsInternetAvailable())
        {
            // InternetConnectionHandler.HandleNoInternet();
            ++apiBuilder.retryCallsCounter;
            // indicator turn on to tell that this api call has exited (success or failed)
            apiBuilder.isCallExited = true;
            throw new Exception("There is no internet");
            UnityWebRequest uwr = new UnityWebRequest();
            apiBuilder.callBack?.Invoke(null);
            return;
        }

        if (apiBuilder.retryTimes > 0) // call retry if we need to retry the apic all on connection or network error
        {
            apiBuilder.apiCaller.StopCoroutine(PostAsFormWithRetry(apiBuilder));
            apiBuilder.apiCaller.StartCoroutine(PostAsFormWithRetry(apiBuilder));
        }
        else
        {
            apiBuilder.apiCaller.StopCoroutine(PostAsForm(apiBuilder));
            apiBuilder.apiCaller.StartCoroutine(PostAsForm(apiBuilder));
        }
    }

    private static IEnumerator PostAsForm(ApiBuilder apiBuilder)
    {
        WWWForm formToSend = new WWWForm();

        foreach (KeyValuePair<string, string> keyValuePair in apiBuilder.queryParams)
        {
            formToSend.AddField(keyValuePair.Key, keyValuePair.Value);
            // requestData.Add(new MultipartFormDataSection(keyValuePair.Key,keyValuePair.Value));
        }

        // Debug.Log("Hitting Post Urls: " + url);
        using (UnityWebRequest webRequest = UnityWebRequest.Post(apiBuilder.apiUrl, formToSend))
        {
            // Set the Authorization header if needed
            SetHeaderTokens(webRequest);

            webRequest.method = UnityWebRequest.kHttpVerbPOST;

            yield return webRequest.SendWebRequest();

            ++apiBuilder.retryCallsCounter;
            // indicator turn on to tell that this api call has exited (success or failed)
            apiBuilder.isCallExited = true;
            apiBuilder.callBack?.Invoke(webRequest);
        }
    }

    private static IEnumerator PostAsFormWithRetry(ApiBuilder apiBuilder)
    {
        WWWForm formToSend = new WWWForm();

        foreach (KeyValuePair<string, string> keyValuePair in apiBuilder.queryParams)
        {
            formToSend.AddField(keyValuePair.Key, keyValuePair.Value);
            // requestData.Add(new MultipartFormDataSection(keyValuePair.Key,keyValuePair.Value));
        }

        // Debug.Log("Hitting Post Urls: " + url);
        while (true)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Post(apiBuilder.apiUrl, formToSend))
            {
                // Set the Authorization header if needed
                SetHeaderTokens(webRequest);

                webRequest.method = UnityWebRequest.kHttpVerbPOST;

                yield return webRequest.SendWebRequest();

                if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                    webRequest.result == UnityWebRequest.Result.ProtocolError)
                {
                    // Retry the API call if there are remaining retry attempts
                    if (apiBuilder.retryTimes >= ++apiBuilder.retryCallsCounter) // counter stack over flow
                    {
                        yield return new WaitForEndOfFrame();
                        continue; // Retry the API call
                    }
                }

                ++apiBuilder.retryCallsCounter;
                // indicator turn on to tell that this api call has exited (success or failed)
                apiBuilder.isCallExited = true;
                apiBuilder.callBack?.Invoke(webRequest);
                break; // Exit the loop
            }
        }
    }

    #endregion
    
    #region Call api with Query in POST method

    public static void PostApiCallAsQuery(ApiBuilder apiBuilder)
    {
        if (apiBuilder is null)
        {
            throw new Exception("apiBuilder is null. Please provide ApiBuilder");
        }

        if (!NetworkUtility.IsInternetAvailable())
        {
            // Invoke the callback with the failure request
            // indicator turn on to tell that this api call has exited (success or failed)
            apiBuilder.isCallExited = true;
            throw new Exception("There is no internet");
            apiBuilder.callBack?.Invoke(null);
            return;
        }

        if (apiBuilder.retryTimes > 0) // call retry if we need to retry the apic all on connection or network error
        {
            apiBuilder.apiCaller.StopCoroutine(PostAsQueryWithRetry(apiBuilder));
            apiBuilder.apiCaller.StartCoroutine(PostAsQueryWithRetry(apiBuilder));
        }
        else
        {
            apiBuilder.apiCaller.StopCoroutine(PostAsQuery(apiBuilder));
            apiBuilder.apiCaller.StartCoroutine(PostAsQuery(apiBuilder));
        }
    }

    private static IEnumerator PostAsQuery(ApiBuilder apiBuilder)
    {
        CountOfRequestsSent++;
        var queryParams = apiBuilder.queryParams;

        string qParams = (queryParams.Count > 0) ? "?" : "";

        //populate parameters
        foreach (var item in queryParams)
            qParams += item.Key + "=" + item.Value + "&";

        qParams = qParams.TrimEnd('&');

        string urlComplete = apiBuilder.apiUrl + qParams;
        //  Debug.Log("Hitting Post Queury Urls: " + urlComplete);
        using (UnityWebRequest webRequest = UnityWebRequest.Post(urlComplete, ""))
        {
            // Set the Authorization header if needed
            SetHeaderTokens(webRequest);

            webRequest.method = UnityWebRequest.kHttpVerbPOST;

            yield return webRequest.SendWebRequest();

            // Invoke the callback regardless of network errors
            // indicator turn on to tell that this api call has exited (success or failed)
            apiBuilder.isCallExited = true;
            apiBuilder.callBack?.Invoke(webRequest);
        }
    }

    private static IEnumerator PostAsQueryWithRetry(ApiBuilder apiBuilder)
    {
        CountOfRequestsSent++;
        var queryParams = apiBuilder.queryParams;

        string qParams = (queryParams.Count > 0) ? "?" : "";

        //populate parameters
        foreach (var item in queryParams)
            qParams += item.Key + "=" + item.Value + "&";

        qParams = qParams.TrimEnd('&');

        string urlComplete = apiBuilder.apiUrl + qParams;
        //  Debug.Log("Hitting Post Queury Urls: " + urlComplete);
        while (true)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Post(urlComplete, ""))
            {
                // Set the Authorization header if needed
                SetHeaderTokens(webRequest);

                webRequest.method = UnityWebRequest.kHttpVerbPOST;

                yield return webRequest.SendWebRequest();

                if (webRequest.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
                {
                    // Retry the API call if there are remaining retry attempts
                    if (apiBuilder.retryTimes >= ++apiBuilder.retryCallsCounter) // counter stack over flow
                    {
                        yield return new WaitForEndOfFrame();
                        continue; // Retry the API call
                    }
                }

                // Invoke the callback with the web request result
                apiBuilder.retryCallsCounter = 0; // Reset the retry counter
                // indicator turn on to tell that this api call has exited (success or failed)
                apiBuilder.isCallExited = true;
                apiBuilder.callBack?.Invoke(webRequest);
                break; // Exit the loop
            }
        }
    }

    #endregion

    #region Call api with Query in GET method

    public static void GetApiCallAsQuery(ApiBuilder apiBuilder)
    {
        CountOfRequestsSent++;

        if (apiBuilder is null)
        {
            throw new Exception("apiBuilder is null. Please provide ApiBuilder");
        }

        if (!NetworkUtility.IsInternetAvailable())
        {
            // indicator turn on to tell that this api call has exited (success or failed)
            apiBuilder.isCallExited = true;
            throw new Exception("There is no internet");
            // Invoke the callback with the failure request
            apiBuilder.callBack?.Invoke(null);
            return;
        }

        if (apiBuilder.retryTimes > 0) // call retry if we need to retry the apic all on connection or network error
        {
            apiBuilder.apiCaller.StopCoroutine(GetAsQueryWithRetry(apiBuilder));
            apiBuilder.apiCaller.StartCoroutine(GetAsQueryWithRetry(apiBuilder));
        }
        else
        {
            apiBuilder.apiCaller.StopCoroutine(GetAsQuery(apiBuilder));
            apiBuilder.apiCaller.StartCoroutine(GetAsQuery(apiBuilder));
        }
    }


    private static IEnumerator GetAsQuery(ApiBuilder apiBuilder)
    {
        var queryParams = apiBuilder.queryParams;

        string qParams = (queryParams.Count > 0) ? "?" : "";
        CountOfRequestsSent++;
        //populate parameters
        foreach (var item in queryParams)
        {
            qParams += item.Key + "=" + item.Value + "&";
            // Debug.Log("qParams Get Data : " + qParams);
        }

        // remove parameter binder
        qParams = qParams.TrimEnd('&');

        var urlComplete = apiBuilder.apiUrl + qParams;

//        Debug.Log("Hitting Get Urls: " + urlComplete);

        using (UnityWebRequest webRequest = UnityWebRequest.Get(urlComplete))
        {
            // Set the Authorization header if needed
            SetHeaderTokens(webRequest);

            // Send the request
            yield return webRequest.SendWebRequest();


            // indicator turn on to tell that this api call has exited (success or failed)
            apiBuilder.isCallExited = true;
            ++apiBuilder.retryCallsCounter;
            apiBuilder.callBack?.Invoke(webRequest);
        }
    }

    private static IEnumerator GetAsQueryWithRetry(ApiBuilder apiBuilder)
    {
        var queryParams = apiBuilder.queryParams;

        string qParams = (queryParams.Count > 0) ? "?" : "";
        CountOfRequestsSent++;
        //populate parameters
        foreach (var item in queryParams)
        {
            qParams += item.Key + "=" + item.Value + "&";
            // Debug.Log("qParams Get Data : " + qParams);
        }

        // remove parameter binder
        qParams = qParams.TrimEnd('&');

        var urlComplete = apiBuilder.apiUrl + qParams;

//        Debug.Log("Hitting Get Urls: " + urlComplete);
        while (true)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(urlComplete))
            {
                // Set the Authorization header if needed
                SetHeaderTokens(webRequest);

                yield return webRequest.SendWebRequest();

                if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                    webRequest.result == UnityWebRequest.Result.ProtocolError)
                {
                    // Retry the API call if there are remaining retry attempts
                    if (apiBuilder.retryTimes >= ++apiBuilder.retryCallsCounter) // counter stack over flow
                    {
                        yield return new WaitForEndOfFrame();
                        continue; // Retry the API call
                    }
                }

                ++apiBuilder.retryCallsCounter;
                // indicator turn on to tell that this api call has exited (success or failed)
                apiBuilder.isCallExited = true;
                apiBuilder.callBack?.Invoke(webRequest);
                break; // Exit the loop
            }
        }
    }

    #endregion

    #region Call api with URL in GET method

    public static void GetApiCallAsUrl(ApiBuilder apiBuilder)
    {
        CountOfRequestsSent++;

        if (apiBuilder is null)
        {
            throw new Exception("apiBuilder is null. Please provide ApiBuilder");
        }

        if (!NetworkUtility.IsInternetAvailable())
        {
            // // indicator turn on to tell that this api call has exited (success or failed)
            // apiBuilder.isCallExited = true;
            // // Invoke the callback with the failure request
            // apiBuilder.callBack?.Invoke(null);
            // return;
            
           // throw new Exception("There is no internet");
        }

        if (apiBuilder.retryTimes > 0) // call retry if we need to retry the apic all on connection or network error
        {
            apiBuilder.apiCaller.StopCoroutine(GetAsUrlWithRetry(apiBuilder));
            apiBuilder.apiCaller.StartCoroutine(GetAsUrlWithRetry(apiBuilder));
        }
        else
        {
            apiBuilder.apiCaller.StopCoroutine(GetAsUrl(apiBuilder));
            apiBuilder.apiCaller.StartCoroutine(GetAsUrl(apiBuilder));
        }
    }

    private static IEnumerator GetAsUrl(ApiBuilder apiBuilder)
    {
        var urlComplete = apiBuilder.apiUrl;

//        Debug.Log("Hitting Get Urls: " + urlComplete);

        using (UnityWebRequest webRequest = UnityWebRequest.Get(urlComplete))
        {
            // Set the Authorization header if needed
            SetHeaderTokens(webRequest);

            // Send the request
            yield return webRequest.SendWebRequest();

            ++apiBuilder.retryCallsCounter;
            // indicator turn on to tell that this api call has exited (success or failed)
            apiBuilder.isCallExited = true;
            apiBuilder.callBack?.Invoke(webRequest);
        }
    }

    private static IEnumerator GetAsUrlWithRetry(ApiBuilder apiBuilder)
    {
        var urlComplete = apiBuilder.apiUrl;

//        Debug.Log("Hitting Get Urls: " + urlComplete);
        while (true)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(urlComplete))
            {
                // Set the Authorization header if needed
                SetHeaderTokens(webRequest);

                yield return webRequest.SendWebRequest();

                if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                    webRequest.result == UnityWebRequest.Result.ProtocolError)
                {
                    // Retry the API call if there are remaining retry attempts
                    if (apiBuilder.retryTimes >= ++apiBuilder.retryCallsCounter) // counter stack over flow
                    {
                        yield return new WaitForEndOfFrame();
                        continue; // Retry the API call
                    }
                }

                ++apiBuilder.retryCallsCounter;
                // indicator turn on to tell that this api call has exited (success or failed)
                apiBuilder.isCallExited = true;
                apiBuilder.callBack?.Invoke(webRequest);
                break; // Exit the loop
            }
        }
    }

    #endregion

    #region Queue utility

    public static void SetHeaderTokens(UnityWebRequest webRequest)
    {
        webRequest.SetRequestHeader("accessToken", PlayerPrefsManager.AccessToken);
        webRequest.SetRequestHeader("refreshToken", PlayerPrefsManager.RefreshToken);
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
                PostApiCallAsQuery(typeOfCall);
            }
            else if (typeOfCall.callType == ApiCallType.PostAsJson)
            {
                PostApiCallAsJson(typeOfCall);
            }
            else if (typeOfCall.callType == ApiCallType.PostAsBody)
            {
                PostApiCallAsForm(typeOfCall);
            }
            else if (typeOfCall.callType == ApiCallType.PostAsJson)
            {
                GetApiCallAsQuery(typeOfCall);
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

    static void EventProcessingWorker()
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
                    while (!eventType.isCallExited)
                    {
                        if (_apiCallerQueue.Count > 0)
                            EventProcessingWorker();
                    }
                }
            }
        }
    }

    #endregion
}

[System.Serializable]
public class ApiBuilder
{
    public MonoBehaviour apiCaller;
    public ApiCallType callType;
    public string apiUrl;
    public Dictionary<string, string> queryParams;
    public Action<UnityWebRequest> callBack;
    public int retryTimes; // assign how many times he needs to retry
    public bool forciblyRun; // run this api by forcibly even if it already running

    // helping methods
    public int retryCallsCounter;
    public bool isCallExited = false;

    public ApiBuilder()
    {
    }

    public ApiBuilder(MonoBehaviour _apiCaller, ApiCallType _callType, string _apiUrl,
        Dictionary<string, string> _queryParams, int _retryTimes = 0, Action<UnityWebRequest> _callBack = null)
    {
        this.apiCaller = _apiCaller;
        this.callType = _callType;
        this.apiUrl = _apiUrl;
        this.queryParams = _queryParams;
        this.retryTimes = _retryTimes;
        this.callBack = _callBack;
        //
        this.retryCallsCounter = 0;
        this.isCallExited = false;
    }
}