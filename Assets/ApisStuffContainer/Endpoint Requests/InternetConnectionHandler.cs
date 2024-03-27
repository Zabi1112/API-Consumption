using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class InternetConnectionHandler
{
    public static void HandleNoInternet(Action OnRetryButtonClicked, Action OnOkButtonClicked)
    {
        OnRetryButtonClicked?.Invoke();
        OnOkButtonClicked?.Invoke();
    }
}