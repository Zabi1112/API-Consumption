using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public class ApiCallHelper : MonoBehaviour
{
    public string userId = "1";
    public string userNamePublic = "adeel";
    public string userEmailPublic = "adeel@clv.com";
    public string userPassPublic = "123";
    public string country = "pk";
    public LogInType logInType = LogInType.email;
    // Start is called before the first frame update

    public Action<ResponseHelper> OnResponseReceived;
    public Action<string> OnErrorReceived;
    public UserModel userdata;
    public UserDataModel userdataModel;
    public StatsModel[] statsData;

    void Start()
    {
    }


    [ContextMenu(nameof(GetDataDirectInToModel))]
    public void GetDataDirectInToModel()
    {
        Dictionary<string, string> apiParams = new Dictionary<string, string>();
        apiParams.Add("email", userEmailPublic);
        apiParams.Add("userId", userId);
        apiParams.Add("country", country);
        apiParams.Add("password", userPassPublic);
        apiParams.Add("username", userNamePublic);
        apiParams.Add("loginType", logInType.ToString());

        ApiBuilder apiBuilder = new ApiBuilder(
            this, ApiCallType.PostAsBody, ApiUrls.loginOrSignUpUrl, apiParams, 0);
        // Define the callback separately
        Action<UnityWebRequest> callback = request =>
        {
            if (request == null)
            {
                ResponseHelper responseHelper =
                    ApiResponseUtility.ParseApiResponseBase("there is no internet");
            }

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"Response Received with Tried {apiBuilder.retryTimes} : \n");
                UserModel _userData =
                    ApiResponseUtility.ParseApiContentResponseIntoModel<UserModel>(request.downloadHandler.text);

                userdata = _userData;
                string model = "";
                model += (userdata.UserId + " " + userdata.Username + " " + userdata.Email + "\n");

                ResponseHelper responseHelper = new ResponseHelper();
                responseHelper.contentString = model;
                OnResponseReceived?.Invoke(responseHelper);
            }
            else
            {
                OnErrorReceived?.Invoke($"Error Received with Tried {apiBuilder.retryTimes} : \n" + request.error);
                Debug.Log(request.error);
            }
        };

        // Now set the callback on the apiBuilder
        apiBuilder.callBack = callback;


        ApiHttpUtility.PostApiCallAsForm(apiBuilder);
    }


    [ContextMenu(nameof(GetDataInToFormattedModel))]
    public void GetDataInToFormattedModel()
    {
        Dictionary<string, string> apiParams = new Dictionary<string, string>();
        apiParams.Add("email", userEmailPublic);
        apiParams.Add("userId", userId);
        apiParams.Add("country", country);
        apiParams.Add("password", userPassPublic);
        apiParams.Add("username", userNamePublic);
        apiParams.Add("loginType", logInType.ToString());

        ApiBuilder apiBuilder = new ApiBuilder(
            this, ApiCallType.PostAsBody, ApiUrls.loginOrSignUpUrl, apiParams, 0);
        // Define the callback separately
        Action<UnityWebRequest> callback = request =>
        {
            if (request == null)
            {
                ResponseHelper responseHelper =
                    ApiResponseUtility.ParseApiResponseBase("there is no internet");
            }

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"Response Received with Tried {apiBuilder.retryTimes} : \n");
                UserDataModel _userData =
                    ApiResponseUtility.ParseApiResponseIntoModel<UserDataModel>(request.downloadHandler.text);

                userdataModel = _userData;
               var data = userdataModel.content;
                string model = "";

                model += (data.UserId + " " + data.Username + " " + data.Email + "\n");


                ResponseHelper responseHelper = new ResponseHelper();
                responseHelper.contentString = model;
                OnResponseReceived?.Invoke(responseHelper);
            }
            else
            {
                OnErrorReceived?.Invoke($"Error Received with Tried {apiBuilder.retryTimes} : \n" + request.error);
                Debug.Log(request.error);
            }
        };

        // Now set the callback on the apiBuilder
        apiBuilder.callBack = callback;

        ApiHttpUtility.PostApiCallAsForm(apiBuilder);
    }


    [ContextMenu(nameof(GetDataInToFormattedModelContentArrayWithRetry))]
    public void GetDataInToFormattedModelContentArrayWithRetry()
    {
        Dictionary<string, string> apiParams = new Dictionary<string, string>();
        apiParams.Add("userId", userId);
        
        int callCounter = 2;

        ApiBuilder apiBuilder = new ApiBuilder(
            this, ApiCallType.PostAsBody, ApiUrls.getAllStats, apiParams, callCounter);
        // Define the callback separately
        Action<UnityWebRequest> callback = request =>
        {
            if (request == null)
            {
                ResponseHelper responseHelper =
                    ApiResponseUtility.ParseApiResponseBase("");
            }

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"Response Received with Tried {apiBuilder.retryTimes} : \n");
                StatsModel[] _userData =
                    ApiResponseUtility.ParseApiContentResponseIntoModelArray<StatsModel>(request.downloadHandler
                        .text);

                statsData = _userData;

                string model = "";
                foreach (var userdata in _userData)
                {
                    model += (userdata.UserId + " " + userdata.Username + " " + userdata.TotalKills + " " +
                              userdata.TotalGames + "\n");
                }


                ResponseHelper responseHelper = new ResponseHelper();
                responseHelper.contentString = model;
                OnResponseReceived?.Invoke(responseHelper);
            }
            else
            {
                OnErrorReceived?.Invoke($"Error Received with Tried {apiBuilder.retryTimes} : \n" + request.error);
                Debug.Log(request.error);
            }
        };

        // Now set the callback on the apiBuilder
        apiBuilder.callBack = callback;

        ApiHttpUtility.PostApiCallAsForm(apiBuilder);
    }

//Dummy
    [ContextMenu(nameof(GetDataDirectInToModelArrayWithRetry))]
    public void GetDataDirectInToModelArrayWithRetry()
    {
        Dictionary<string, string> apiParams = new Dictionary<string, string>();
        apiParams.Add("email", userEmailPublic);
        apiParams.Add("userId", userId);
        apiParams.Add("country", country);
        apiParams.Add("password", userPassPublic);
        apiParams.Add("username", userNamePublic);
        apiParams.Add("loginType", logInType.ToString());


        int callCounter = 2;

        ApiBuilder apiBuilder = new ApiBuilder(
            this, ApiCallType.PostAsBody, ApiUrls.getAllUsers, apiParams, callCounter);
        // Define the callback separately
        Action<UnityWebRequest> callback = request =>
        {
            if (request == null)
            {
                ResponseHelper responseHelper =
                    ApiResponseUtility.ParseApiResponseBase("");
            }

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"Response Received with Tried {apiBuilder.retryTimes} : \n");
                DummyModel[] _userData =
                    ApiResponseUtility.ParseApiResponseIntoModelArray<DummyModel>(request.downloadHandler.text);


                string model = "";
                foreach (var userdata in _userData)
                {
                    model += (userdata.userId + " " + userdata.username + " " + userdata.password + "\n");
                }

                ResponseHelper responseHelper = new ResponseHelper();
                responseHelper.contentString = model;
                OnResponseReceived?.Invoke(responseHelper);
            }
            else
            {
                OnErrorReceived?.Invoke($"Error Received with Tried {apiBuilder.retryTimes} : \n" + request.error);
                Debug.Log(request.error);
            }
        };

        // Now set the callback on the apiBuilder
        apiBuilder.callBack = callback;

        ApiHttpUtility.PostApiCallAsForm(apiBuilder);
    }


    [ContextMenu(nameof(RefreshAuthTokens))]
    public void RefreshAuthTokens()
    {
        Dictionary<string, string> apiParams = new Dictionary<string, string>();
        apiParams.Add("userId", PlayerPrefsManager.UserId);

        ApiBuilder apiBuilder = new ApiBuilder(
            this, ApiCallType.GetAsUrl, ApiUrls.refreshToken, apiParams, _callBack: request =>
            {
                if (request == null)
                {
                    ResponseHelper responseHelper =
                        ApiResponseUtility.ParseApiResponseBase("there is no internet");
                }

                if (request.result == UnityWebRequest.Result.Success)
                {
                    ResponseHelper responseHelper =
                        ApiResponseUtility.ParseApiResponseBase(request.downloadHandler.text);

                    OnResponseReceived?.Invoke(responseHelper);
                }
                else
                {
                    OnErrorReceived?.Invoke("Error Received: \n" + request.error);
                    Debug.Log(request.error);
                }
            }
        );

        ApiHttpUtility.PostApiCallAsForm(apiBuilder);
    }

    #region Utility Methods

    [ContextMenu(nameof(SaveUserIdInPref))]
    public void SaveUserIdInPref()
    {
        PlayerPrefsManager.UserId = userId.ToString();
    }

    static void SaveUserId(ResponseHelper responseHelper)
    {
        if (responseHelper.ContentDictionary != null)
        {
            if (responseHelper.ContentDictionary.ContainsKey("id"))
            {
                PlayerPrefsManager.UserId = responseHelper.ContentDictionary["id"].ToString();
            }
        }
    }

    #endregion
}