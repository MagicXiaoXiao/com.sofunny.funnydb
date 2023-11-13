using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SoFunny.FunnyDB;
using Newtonsoft.Json;
using System;
using SoFunny.FunnySDK.UIModule;

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
                Dictionary<string, object> customProperties = JsonConvert.DeserializeObject<Dictionary<string, object>>(contentLabel.text);
                if (customProperties == null)
                {
                    customProperties = new Dictionary<string, object>();
                }
                switch (type)
                {
                    case FDBExampleReportType.Event:
                        //FDBEvent.ReportEvent(nameLabel.text, contentLabel.text);
                        FDBEvent.ReportEvent(nameLabel.text, customProperties);
                        Toast.Show("发送事件 - " + nameLabel.text);
                        break;
                    case FDBExampleReportType.AddUser:
                        FDBEvent.ReportAddUser(customProperties);
                        Toast.Show("发送 AddUser");
                        break;
                    case FDBExampleReportType.AddDevice:
                        FDBEvent.ReportAddDevice(customProperties);
                        Toast.Show("发送 AddDevice");
                        break;
                    case FDBExampleReportType.SetUser:
                        FDBEvent.ReportSetUser(customProperties);
                        Toast.Show("发送 SetUser");
                        break;
                    case FDBExampleReportType.SetDevice:
                        FDBEvent.ReportSetDevice(customProperties);
                        Toast.Show("发送 SetDevice");
                        break;
                    case FDBExampleReportType.SetOnceUser:
                        FDBEvent.ReportSetOnceUser(customProperties);
                        Toast.Show("发送 SetOnceUser");
                        break;
                    case FDBExampleReportType.SetOnceDevice:
                        FDBEvent.ReportSetOnceDevice(customProperties);
                        Toast.Show("发送 SetOnceDevice");
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
        Dictionary<string,object> properties = new Dictionary<string, object>();
        // string
        properties["channel"] = "FunnySample";
        // 数字 int
        properties["count"] = 10;
        // 数字 float
        properties["floatValue"] = 3.14159;
        // bool
        properties["boolValue"] = false;
        // Object
        properties["Object"] = new Hashtable() { { "flower", "red" } };
        //object for array
        properties["obj_arr"] = new List<object>() { new Hashtable() { { "flower", "blue" } } };
        // string list 字符列表
        properties["arr"] = new List<object>() { "value" };
        string reportStr = JsonConvert.SerializeObject(properties);
        FDBEvent.ReportEvent("many_types_event", reportStr);
        Toast.Show("发送事件 - many_types_event");
    }

}
