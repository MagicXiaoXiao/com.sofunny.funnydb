using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using SoFunny.FunnyDB;
using System.Collections.Generic;
using System.Net.NetworkInformation;

/// <summary>
/// FunnySDK Access Guide
/// </summary>
public class ExampleMain : MonoBehaviour {

    public GameObject initPanel;
    public GameObject eventPanel;
    public InputField dIDInput;
    public InputField endPonitInput;

    public Text deviceIDText;

    private bool isInit = false;

    private void Awake() {

    }

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

        var config = new FunnyDBConfig("demo", "secret");

        if (!string.IsNullOrEmpty(dIDInput.text)) {
            config.SetDeviceID(dIDInput.text);
        }

        if (!string.IsNullOrEmpty(endPonitInput.text)) {
            config.SetEndPoint(endPonitInput.text);
        }

        FunnyDBSDK.Initialize(config);
        isInit = true;

        FunnyDBSDK.ShowFDBToast("SDK 初始化完毕");
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

}
