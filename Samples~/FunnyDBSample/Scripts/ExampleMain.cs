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
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using SoFunny.FunnyDB.PC;

/// <summary>
/// FunnySDK Access Guide
/// </summary>
public class ExampleMain : MonoBehaviour {

    public GameObject initPanel;
    public GameObject eventPanel;
    public InputField dIDInput;
    public InputField endPonitInput;

    public InputField sendIntervalTimeInput;
    public InputField sendMaxCntInput;


    public Text deviceIDText;
    public Button backSettingBtn;

    private bool isInit = false;


    private void Awake()
    {
        #region 反射获取 FunnyDB 实例
        string SpaceName = "SoFunny.FunnyDB";
        string className = "FunnyDBSDK";
        Assembly assembly = Assembly.Load("SoFunny.FunnyDB");
        Type type = assembly.GetType(SpaceName + "." + className);
        if (type != null)
        {
            // 获取静态方法
            MethodInfo methodInfo = type.GetMethod("getIntValue", BindingFlags.Static | BindingFlags.Public);

            if (methodInfo != null)
            {
                // 调用静态方法
                int plusInfo = (int)methodInfo.Invoke(null, new object[] { 3 });
                Debug.Log("plus value: " + plusInfo);
            }
            else
            {
                Console.WriteLine("静态方法未找到！");
            }
        }
        else
        {
            Console.WriteLine("类型未找到！");
        }

        #endregion
    }

    void Start() {
        sendIntervalTimeInput.onValueChanged.AddListener((v) =>
        {
            int intervalTime = int.Parse(v);
            if (intervalTime < 1000)
            {
                sendIntervalTimeInput.text = 1000.ToString();
                Debug.LogWarning("值需大于 1000");
                return;
            }
            FunnyDBSDK.SetReportInterval(intervalTime);
        });

        sendMaxCntInput.onValueChanged.AddListener((v) =>
        {
            int maxCnt = int.Parse(v);
            if (maxCnt < 1 || maxCnt > 50)
            {
                sendMaxCntInput.text = maxCnt < 1 ? 1.ToString() : 50.ToString();
                Debug.LogWarning("值需大于 1 小于 50");
                return;
            }
            FunnyDBSDK.SetReportLimit(maxCnt);
        });

        eventPanel.SetActive(false);
        initPanel.SetActive(true);
        var endPoint = PlayerPrefs.GetString("fdb.end.point");
        if (!string.IsNullOrEmpty(endPoint)) {
            endPonitInput.text = endPoint;
        }
    }


    public void InitSDK() {
        //PlayerPrefs.DeleteAll();
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
        FunnyDBSDK.SetSDKSendType(EnumConstants.DBSDK_SEND_TYPE_ENUM.NOW);
    }

    public void SetSendTypeDelay()
    {
        FunnyDBSDK.SetSDKSendType(EnumConstants.DBSDK_SEND_TYPE_ENUM.DELAY);
    }

    public void SetSDKStatusDefault()
    {
        FunnyDBSDK.SetSDKStatus(EnumConstants.DBSDK_STATUS_ENUM.DEFAULT);
    }

    public void SetSDKStatusStopCollect()
    {
        FunnyDBSDK.SetSDKStatus(EnumConstants.DBSDK_STATUS_ENUM.STOP_COLLECT);
    }

    public void SetSDKStatusOnlyCollect()
    {
        FunnyDBSDK.SetSDKStatus(EnumConstants.DBSDK_STATUS_ENUM.ONLY_COLLECT);
    }

    public void BackSettings()
    {
        eventPanel.SetActive(false);
        initPanel.SetActive(true);
    }

    public void ClearCache()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("缓存已经清空，注意内存未清空");
    }

    public void MockCrash()
    {
        string SpaceName = "SoFunny.FunnyDB";
        string className = "FunnyDBSDKss";
        Assembly assembly = Assembly.Load("SoFunny.FunnyDB");
        Type type = assembly.GetType(SpaceName + "." + className);
        type.GetArrayRank();
    }

}
