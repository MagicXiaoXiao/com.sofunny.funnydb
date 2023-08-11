using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Threading.Tasks;
using SoFunny.FunnyDB;
using System.Threading;

/// <summary>
/// FunnySDK Access Guide
/// </summary>
public class ExampleMain : MonoBehaviour {

    public GameObject initPanel;
    public GameObject eventPanel;
    public InputField dIDInput;
    public InputField endPonitInput;

    public Text deviceIDText;
    public Button backSettingBtn;

    private bool isInit = false;

    void Start() {
        eventPanel.SetActive(false);
        initPanel.SetActive(true);
        var endPoint = PlayerPrefs.GetString("fdb.end.point");
        if (!string.IsNullOrEmpty(endPoint)) {
            endPonitInput.text = endPoint;
        }
    }


    public void InitSDK() {
        FunnyDBSDK.EnableDebug();
        PlayerPrefs.DeleteAll();
        var config = new FunnyDBConfig("demo", "secret");
        config.SetEndPoint("http://ingest.funnydb-stage.funnydata.net");
        if (!string.IsNullOrEmpty(dIDInput.text)) {
            config.SetDeviceID(dIDInput.text);
        }

        if (!string.IsNullOrEmpty(endPonitInput.text)) {
            config.SetEndPoint(endPonitInput.text);
        }
        FunnyDBSDK.Initialize(config);
        //FunnyDBSDK.SetSDKSendType(DBSDK_SEND_TYPE_ENUM.NOW);
        //FunnyDBSDK.SetSDKSendType(DBSDK_SEND_TYPE_ENUM.DELAY);

        //FunnyDBSDK.SetSDKStatus(DBSDK_STATUS_ENUM.DEFAULT);
        //FunnyDBSDK.SetSDKStatus(DBSDK_STATUS_ENUM.ONLY_COLLECT);
        //FunnyDBSDK.SetSDKStatus(DBSDK_STATUS_ENUM.STOP_COLLECT);

        isInit = true;

        FunnyDBSDK.ShowFDBToast("SDK 初始化完毕");
    }

    //public void DoRepeatReport()
    //{
    //    for(int i = 0; i < 100; i++)
    //    {
    //        FDBEvent.ReportEvent("auto_Test", null);
    //    }
    //}

    public void Flush()
    {
        FunnyDBSDK.Flush();
    }

    public void gotoReportView() {
        if (isInit) {
            initPanel.SetActive(false);
            eventPanel.SetActive(true);
            deviceIDText.text = FunnyDBSDK.GetDeviceId();
        }
    }

    public void CopyInputDeviceId() {
        if (string.IsNullOrEmpty(dIDInput.text)) {
            return;
        }

        GUIUtility.systemCopyBuffer = dIDInput.text;

        FunnyDBSDK.ShowFDBToast("复制完成");
    }

    public void CopyDeviceId() {
        GUIUtility.systemCopyBuffer = FunnyDBSDK.GetDeviceId();
        FunnyDBSDK.ShowFDBToast("复制完成");
    }

    public void RandomDID() {
        dIDInput.text = Guid.NewGuid().ToString();
    }

    public void SaveEndPoint() {
        if (string.IsNullOrEmpty(endPonitInput.text)) {
            return;
        }

        PlayerPrefs.SetString("fdb.end.point", endPonitInput.text);
        PlayerPrefs.Save();

        FunnyDBSDK.ShowFDBToast("已保存");
    }

    public void ClearEndPoint() {
        PlayerPrefs.DeleteKey("fdb.end.point");
        endPonitInput.text = "";
    }

    public void SetSendTypeNow()
    {
        FunnyDBSDK.SetSDKSendType(DBSDK_SEND_TYPE_ENUM.NOW);
    }

    public void SetSendTypeDelay()
    {
        FunnyDBSDK.SetSDKSendType(DBSDK_SEND_TYPE_ENUM.DELAY);
    }

    public void SetSDKStatusDefault()
    {
        FunnyDBSDK.SetSDKStatus(DBSDK_STATUS_ENUM.DEFAULT);
    }

    public void SetSDKStatusStopCollect()
    {
        FunnyDBSDK.SetSDKStatus(DBSDK_STATUS_ENUM.STOP_COLLECT);
    }

    public void SetSDKStatusOnlyCollect()
    {
        FunnyDBSDK.SetSDKStatus(DBSDK_STATUS_ENUM.ONLY_COLLECT);
    }

    public void BackSettings()
    {
        eventPanel.SetActive(false);
        initPanel.SetActive(true);
    }
}
