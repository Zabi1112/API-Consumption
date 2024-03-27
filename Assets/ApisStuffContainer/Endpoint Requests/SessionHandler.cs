using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SessionHandler
{
    public static void HandleSessionExpire(SessionApiBuilder builder)
    {
        Dictionary<string, string> apiParams = new Dictionary<string, string>();
        apiParams.Add("userId", PlayerPrefsManager.UserId);

        ApiBuilder apiBuilder = new ApiBuilder(
            builder.APICaller, ApiCallType.PostAsBody, ApiUrls.refreshToken, apiParams, _callBack: request =>
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

                    Debug.Log("Tokens are refreshed ");

                    builder.MethodToCallAfterRefreshToken?.Invoke();
                }
                else
                {
                    builder.OnErrorReceived?.Invoke("Error Received: \n" + request.error);
                    Debug.Log(request.error);
                }
            }
        );

        ApiHttpUtility.PostApiCallAsForm(apiBuilder);
    }
}

public class SessionApiBuilder
{
    public MonoBehaviour APICaller;
    public Action MethodToCallAfterRefreshToken;
    public Action<string> OnErrorReceived;
}