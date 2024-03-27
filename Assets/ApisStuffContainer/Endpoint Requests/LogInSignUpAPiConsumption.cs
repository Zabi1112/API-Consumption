using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LogInSignUpAPiConsumption : MonoBehaviour
{
    public string userId = "1";
    public string userNamePublic = "adeel";
    public string userEmailPublic = "adeel@clv.com";
    public string userPassPublic = "123";
    public string country = "pk";
    public LogInType logInType = LogInType.email;

    public Action<ResponseHelper> OnResponseReceived;

    public Action<string> OnErrorReceived;

    // Start is called before the first frame update
    void Start()
    {
    }

    [ContextMenu(nameof(SignUp))]
    public void SignUp()
    {
        Dictionary<string, string> apiParams = new Dictionary<string, string>();
        apiParams.Add("email", userEmailPublic);
        apiParams.Add("userId", userId);
        apiParams.Add("country", country);
        apiParams.Add("password", userPassPublic);
        apiParams.Add("username", userNamePublic);
        apiParams.Add("loginType", logInType.ToString());

        ApiBuilder apiBuilder = new ApiBuilder(
            this, ApiCallType.PostAsBody, ApiUrls.loginOrSignUpUrl, apiParams, _callBack: request =>
            {
                if (request == null)
                {
                    // Generate error if request is null
                    ResponseHelper responseHelper =
                        ApiResponseUtility.ParseApiResponseBase("");
                    return;
                }

                if (request.result == UnityWebRequest.Result.Success)
                {
                    ResponseHelper responseHelper =
                        ApiResponseUtility.ParseApiResponseBase(request.downloadHandler.text);

                    if (responseHelper.ResponseCode == ResponseCodes.SessionExpire)
                    {
                        SessionApiBuilder builder = new SessionApiBuilder();
                        builder.APICaller = this;
                        builder.MethodToCallAfterRefreshToken = SignUp;
                        builder.OnErrorReceived = OnErrorReceived;

                        SessionHandler.HandleSessionExpire(builder);
                    }

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
}