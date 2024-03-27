using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Object = System.Object;

public class ApiResponseUtility : MonoBehaviour
{
    //The format of the backend message is:
    //{
    //    code:   //Is the code in int
    //    message:   //special message to send
    //    content:    //Could either be a dictionary of the response OR in case of errors it is an error message//
    // }


    // TODO Keep in mind model that you want to DeserializeObject must be in below format
    //  public class DummyDataModel
    //  {
    //     public int code = 0;
    //     public string message;
    //     public ActualModel content ; // could be in array format as per your model like ActualModel[] content 
    //  }
    // Call As DummyDataModel dm =  ParseApiResponseIntoModel<DummyDataModel>(returnedPayLoad);

    public static T ParseApiResponseIntoModel<T>(string receivedMessage)
    {
        var fromJsonModel = JsonConvert.DeserializeObject<T>(receivedMessage);

        return fromJsonModel;
    }

    // TODO Model can be DeserializeObject directly without core format 
    public static T ParseApiContentResponseIntoModel<T>(string receivedMessage)
    {
        try
        {
            JToken parsedData = JToken.Parse(receivedMessage);

            if (IsJTokenIsNullOrEmpty(parsedData))
            {
                Debug.LogError("response content is  null");
            }

            JToken content = parsedData["content"];

            if (IsJTokenIsNullOrEmpty(content))
            {
                Debug.LogError("content is  null");
            }

            string code = (string)parsedData["code"];
            ResponseCodes rCode = (ResponseCodes)int.Parse(code);
            if (content.Type == JTokenType.String && rCode != ResponseCodes.Success)
            {
                Debug.LogError("content is  string and has error");
                throw new ArgumentException((string)content);
            }

            var fromJsonModel = JsonConvert.DeserializeObject<T>(content.ToString());

            return fromJsonModel;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            throw;
        }
    }

    // TODO Keep in mind model that you want to DeserializeObject must be in below format
    //  public class DummyDataModel
    //  {
    //     public int code = 0;
    //     public string message;
    //     public ActualModel content ; // could be in array format as per your model like ActualModel[] content 
    //  }
// Call As DummyDataModel[] dm =  ParseApiResponseIntoModelArray<DummyDataModel>(returnedPayLoad);

    public static T[] ParseApiResponseIntoModelArray<T>(string jsonMessage)
    {
        try
        {
            var fromJsonModel = JsonConvert.DeserializeObject<T[]>(jsonMessage);

            return fromJsonModel;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            throw;
        }
    }

    // TODO Model can be DeserializeObject directly into an array  without core format 
    public static T[] ParseApiContentResponseIntoModelArray<T>(string receivedMessage)
    {
        try
        {
            JToken parsedData = JToken.Parse(receivedMessage);

            if (IsJTokenIsNullOrEmpty(parsedData))
            {
                Debug.LogError("response content is  null");
            }

            JToken content = parsedData["content"];

            if (IsJTokenIsNullOrEmpty(content))
            {
                Debug.LogError("content is  null");
            }
            string code = (string)parsedData["code"];
            ResponseCodes rCode = (ResponseCodes)int.Parse(code);
            if (content.Type == JTokenType.String && rCode != ResponseCodes.Success)
            {
                Debug.LogError("content is  string and has error");
                throw new ArgumentException((string)content);
            }

            var fromJsonModel = JsonConvert.DeserializeObject<T[]>(content.ToString());

            return fromJsonModel;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            throw;
        }
    }

    public static ResponseHelper ParseApiResponseBase(string receivedMessage = null, string contentKey = null)
    {
        ResponseHelper response = new ResponseHelper();

        try
        {
            if (string.IsNullOrEmpty(receivedMessage))
            {
                Debug.Log("Json is  null");
                response.contentString = "Response content is  null due to json empty or null";

                //    throw new Exception(response.contentString);
                return response;
            }

            JToken parsedData = JToken.Parse(receivedMessage);

            if (IsJTokenIsNullOrEmpty(parsedData))
            {
                Debug.Log("response content is  null");
                response.contentString = "Response content is  null";
                // throw new Exception(response.contentString);
                return response;
            }

            JToken content = "";
            string code = "";
            string message = "";

            code = (string)parsedData["code"];
            content = parsedData["content"];
            response.ResponseCode = (ResponseCodes)int.Parse(code);

            content = (string.IsNullOrEmpty(contentKey)) ? content : content[contentKey];

            if (IsJTokenIsNullOrEmpty(content))
            {
                Debug.LogError("content is  null");
                response.contentString = " content is  null";
                // throw new Exception(response.contentString);
                return response;
            }

            if (content.Type == JTokenType.Array)
            {
                response.contentJArray = (JArray)content;
            }
            else if (content.Type == JTokenType.Object)
            {
                response.ContentDictionary =
                    JsonConvert.DeserializeObject<Dictionary<string, Object>>(content.ToString());

                // parse and save auth tokens
                SaveAuthTokenData(response);
            }

            else if (content.Type == JTokenType.String)
            {
                response.contentString = (string)content;
            }


            return response;
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            throw;
        }
    }

    #region Utility Methods

    static bool IsJTokenIsNullOrEmpty(JToken tokenValues)
    {
        return (tokenValues == null
                || (tokenValues.Type == JTokenType.String && string.IsNullOrEmpty(tokenValues.ToString()))
                || (!tokenValues.HasValues && tokenValues.Type != JTokenType.String));
    }

    public static void SaveAuthTokenData(ResponseHelper responseHelper)
    {
        if (responseHelper.ContentDictionary != null)
        {
            if (responseHelper.ContentDictionary.ContainsKey("accessToken"))
            {
                PlayerPrefsManager.AccessToken = responseHelper.ContentDictionary["accessToken"].ToString();
            }

            if (responseHelper.ContentDictionary.ContainsKey("refreshToken"))
            {
                PlayerPrefsManager.RefreshToken = responseHelper.ContentDictionary["refreshToken"].ToString();
            }
        }
    }

    #endregion
}

public struct ResponseHelper
{
    public Dictionary<string, object> ContentDictionary;
    public Dictionary<string, JToken> TokenDictionary;

    public JToken contentJToken;
    public JArray contentJArray;

    public string contentString;

    public ResponseCodes ResponseCode;
}