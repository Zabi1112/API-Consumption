using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBackendEventsInterface
{
    public void CallEvent(BackendEventType eventType);
}