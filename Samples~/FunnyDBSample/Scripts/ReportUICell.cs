using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SoFunny.FunnyDB;
using Newtonsoft.Json;
using System;

public enum FDBExampleReportType {
    Event,

    AddUser,
    AddDevice,

    SetUser,
    SetDevice,

    SetOnceUser,
    SetOnceDevice,
}

public class ReportUICell : MonoBehaviour
{
    public FDBExampleReportType type;

    public InputField nameLabel;
    public InputField contentLabel;
    public InputField reportCntInputField;
    private int repeatCnt = 1;

    // Start is called before the first frame update
    void Start()
    {
        var sampleContent = "{\"test\":\"test_data\"}";
        switch (type) {
            case FDBExampleReportType.Event:
                nameLabel.text = "sample";
                contentLabel.text = sampleContent;
                break;
            case FDBExampleReportType.AddUser:
            case FDBExampleReportType.AddDevice:
            case FDBExampleReportType.SetUser:
            case FDBExampleReportType.SetDevice:
            case FDBExampleReportType.SetOnceUser:
            case FDBExampleReportType.SetOnceDevice:
                contentLabel.text = sampleContent;
                break;
            default:
                break;
        }
        reportCntInputField?.onValueChanged.AddListener((v) =>
        {
            int.TryParse(v, out repeatCnt);
        });
    }

    public void ReportEvent() {
        for (int i = 0; i < repeatCnt; i++)
        {
            try
            {
                Hashtable hashtable = JsonConvert.DeserializeObject<Hashtable>(contentLabel.text);
                if (hashtable == null)
                {
                    hashtable = new Hashtable();
                }
                switch (type)
                {
                    case FDBExampleReportType.Event:
                        FDBEvent.ReportEvent(nameLabel.text, hashtable);
                        break;
                    case FDBExampleReportType.AddUser:
                        FDBEvent.ReportAddUser(hashtable);
                        break;
                    case FDBExampleReportType.AddDevice:
                        FDBEvent.ReportAddDevice(hashtable);
                        break;
                    case FDBExampleReportType.SetUser:
                        FDBEvent.ReportSetUser(hashtable);
                        break;
                    case FDBExampleReportType.SetDevice:
                        FDBEvent.ReportSetDevice(hashtable);
                        break;
                    case FDBExampleReportType.SetOnceUser:
                        FDBEvent.ReportSetOnceUser(hashtable);
                        break;
                    case FDBExampleReportType.SetOnceDevice:
                        FDBEvent.ReportSetOnceDevice(hashtable);
                        break;
                    default:
                        break;
                }

                //FunnyDBSDK.ShowFDBToast($"已发起上报 - {type}");
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.Message);
                FunnyDBSDK.ShowFDBToast($"上报出错- {ex.Message}");
            }
        }
    }

    public void ReportWithManyTypes()
    {
        Dictionary<string, object> properties = new Dictionary<string, object>();
        properties["channel"] = "ta";//string
        properties["age"] = 1;//number - int
        properties["weight"] = 5.46;//number - float
        properties["balance"] = -0.4;//number - negative
        properties["isVip"] = true;//bool
        properties["birthday"] = new DateTime(2022, 01, 01);//date
        properties["object"] = new Dictionary<string, object>() { { "key", "value" } };//object
        properties["object_arr"] = new List<object>() { new Dictionary<string, object>() { { "key", "value" } } };//object array
        properties["arr"] = new List<object>() { "value" };//array

        FDBEvent.ReportEvent("many_types_event", properties);
    }

}
