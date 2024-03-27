using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;

public class ApiTestUiUtility : MonoBehaviour
{
    public TMP_Text responseText;
    public TMP_Text errorText;


     private ApiCallHelper _apiCallHelper;
    // private LoginApiConsumer responseHelper;
    // private SignUpAPiConsumption signUpAPiConsumption;
    private LogInSignUpAPiConsumption signUpAPiConsumption;

    private void OnEnable()
    {
        _apiCallHelper = GetComponent<ApiCallHelper>();
        _apiCallHelper.OnResponseReceived += OnResponseReceived;
        _apiCallHelper.OnErrorReceived += OnErrorReceived;
        //
        // responseHelper = GetComponent<LoginApiConsumer>();
        // responseHelper.OnResponseReceived += OnResponseReceived;
        // responseHelper.OnErrorReceived += OnErrorReceived;

        signUpAPiConsumption = GetComponent<LogInSignUpAPiConsumption>();
        signUpAPiConsumption.OnResponseReceived += OnResponseReceived;
        signUpAPiConsumption.OnErrorReceived += OnErrorReceived;
    }

    private void OnDisable()
    {
        _apiCallHelper.OnResponseReceived -= OnResponseReceived;
        _apiCallHelper.OnErrorReceived -= OnErrorReceived;
        //
        // responseHelper.OnResponseReceived -= OnResponseReceived;
        // responseHelper.OnErrorReceived -= OnErrorReceived;

        signUpAPiConsumption.OnResponseReceived -= OnResponseReceived;
        signUpAPiConsumption.OnErrorReceived -= OnErrorReceived;
    }

    [ContextMenu(nameof(ResetTexts))]
    public void ResetTexts()
    {
        if (responseText) responseText.text = "Returned Data: ";
        if (errorText) errorText.text = "Error: ";
    }

    // Update is called once per frame
    void OnResponseReceived(ResponseHelper responseHelper)
    {
        ResetTexts();
        if (responseText)
        {
            responseText.text = StoreAllData(responseHelper);
        }
        else
        {
            Debug.Log(StoreAllData(responseHelper));
        }
    }

    private void OnErrorReceived(string message)
    {
        ResetTexts();
        if (errorText)
        {
            errorText.text = message;
        }
        else
        {
            Debug.LogError(message);
        }
    }
    // Method to store all data into contentString in one go

// Method to store all data from ResponseHelper into its contentString
    public string StoreAllData(ResponseHelper responseHelper)
    {
        // Initialize a new dictionary to hold all the data
        var allData = new Dictionary<string, string>(); // Using string to ensure proper JSON conversion

        string convertedData = "";
        // Convert and add the ContentDictionary
        if (responseHelper.ContentDictionary != null)
        {
            //allData["ContentDictionary"] = JsonConvert.SerializeObject(responseHelper.ContentDictionary);
            convertedData = JsonConvert.SerializeObject(responseHelper.ContentDictionary);
        }

        // Convert and add the TokenDictionary directly if possible
        else if (responseHelper.TokenDictionary != null)
        {
            // allData["TokenDictionary"] = JsonConvert.SerializeObject(responseHelper.TokenDictionary);
            convertedData = JsonConvert.SerializeObject(responseHelper.TokenDictionary);
        }

        // Add the contentJToken as a string
        else if (responseHelper.contentJToken != null)
        {
            //allData["contentJToken"] = responseHelper.contentJToken.ToString();
            convertedData = JsonConvert.SerializeObject(responseHelper.contentJToken);
        }

        // Add the contentJArray as a string
        else if (responseHelper.contentJArray != null)
        {
            convertedData = JsonConvert.SerializeObject(responseHelper.contentJArray);
        }

        // Add the contentString as a string
        else if (responseHelper.contentString != null)
        {
            convertedData = responseHelper.contentString;
        }

        // Convert all accumulated data into a single JSON string
        // responseHelper.contentString = JsonConvert.SerializeObject(allData);

        // Convert all accumulated data into a single JSON string and return it
        // return JsonConvert.SerializeObject(allData);
        return convertedData;
    }
}