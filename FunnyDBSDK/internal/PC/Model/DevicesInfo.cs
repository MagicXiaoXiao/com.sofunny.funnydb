#if UNITY_STANDALONE || UNITY_EDITOR
using System;
using System.IO;
using System.Net.NetworkInformation;
using UnityEngine;

/// <summary>
/// Devices Info
/// </summary>
namespace SoFunny.FunnyDB.PC
{
    internal class DevicesInfo
    {
        private static string _deviceId;
        private static string _userId;
        private static string _channel;
        private static string _sdkType;
        private static string _sdkVersion;
        private static string _deviceModel;
        private static string _manufacturer;
        private static int _screenHeight;
        private static int _screenWidth;
        private static string _os;
        private static string _osPlatform;
        private static string _osVersion;
        private static string _carrier;
        private static string _deviceName;
        private static float _ramCapacity;
        private static float _diskCapacity;
        private static string _cpuModel;
        private static int _cpuCoreCount;
        private static float _cpuFrequency;
        private static int _timeZone;
        private static string _language;


        internal static string DeviceId
        {
            get
            {
                if (!string.IsNullOrEmpty(_deviceId))
                {
                    return _deviceId;
                }
                _deviceId = GetMacAddress();
                return _deviceId;
            }
            set { _deviceId = value; }
        }
        internal static string UserId
        {
            get { return _userId; }
            set { _userId = value; }
        }
        internal static string Channel
        {
            get
            {
                return string.IsNullOrEmpty(_channel) ? Constants.VALUE_UNKNOWN : _channel;
            }
            set { _channel = value; }
        }
        internal static string SdkType
        {
            get { return "unity"; }
        }
        internal static string SdkVersion
        {
            get { return Constants.FUNNY_DB_VERSION; }
        }
        internal static string DeviceModel
        {
            get
            {
                if (!string.IsNullOrEmpty(_deviceModel))
                {
                    return _deviceModel;
                }
                _deviceModel = SystemInfo.deviceModel;
                return _deviceModel;
            }
            set { _deviceModel = value; }
        }

        internal static string DeviceName
        {
            get
            {
                if (!string.IsNullOrEmpty(_deviceName))
                {
                    return _deviceName;
                }
                _deviceName = SystemInfo.deviceName;
                return _deviceName;
            }
            set { _deviceName = value; }
        }
        internal static string Manufacturer
        {
            get
            {
                if(!string.IsNullOrEmpty(_manufacturer))
                {
                    return _manufacturer;
                }
                _manufacturer = SystemInfo.graphicsDeviceVendor;
                return _manufacturer;
            }
        }
        internal static int ScreenHeight
        {
            get
            {
                if(_screenHeight != 0)
                {
                    return _screenHeight;
                }
                _screenHeight = Screen.height;
                return _screenHeight;
            }
        }
        internal static int ScreenWidth
        {
            get
            {
                if(_screenWidth != 0)
                {
                    return _screenWidth;                    
                }
                _screenWidth = Screen.width;
                return _screenWidth;
            }
        }
        internal static string Os
        {
            get
            {
                if (!string.IsNullOrEmpty(_os))
                {
                    return _os;
                }
                _os = SystemInfo.operatingSystem;
                return _os;
            }
        }
        internal static string OsPlatform
        {
            get
            {
                if (!string.IsNullOrEmpty(_osPlatform))
                {
                    return _osPlatform;
                }
                _osPlatform = Application.platform.ToString();
                return _osPlatform;
            }
        }
        internal static string OsVersion
        {
            get
            {
                if (!string.IsNullOrEmpty(_osVersion))
                {
                    return _osVersion;
                }
                _osVersion = SystemInfo.operatingSystem;
                return _osVersion;
            }
        }
        internal static string Carrier
        {
            get { return Constants.VALUE_UNKNOWN; }
        }

        internal static float RamCapacity
        {
            get {
                if (_ramCapacity != 0f) return _ramCapacity;
                _ramCapacity = SystemInfo.systemMemorySize / 1024;
                return _ramCapacity;
            }
            private set { }
        }

        internal static float DiskCapacity
        {
            get {
                if (_diskCapacity != 0f) return _diskCapacity;
                DriveInfo[] allDrives = DriveInfo.GetDrives();
                long total = 0;
                foreach (DriveInfo drive in allDrives)
                {
                    total += drive.TotalSize;
                }
                _diskCapacity = (float)total / (1024 * 1024 * 1024);
                return _diskCapacity;
            }
            private set { }
        }

        internal static string CPUModel
        {
            get {
                if (!string.IsNullOrEmpty(_cpuModel)) return _cpuModel;
                _cpuModel = SystemInfo.processorType;
                return _cpuModel;
            }
            private set { }
        }

        internal static int CPUCoreCnt
        {
            get {
                if (_cpuCoreCount != 0) return _cpuCoreCount;
                _cpuCoreCount = SystemInfo.processorCount;
                return _cpuCoreCount;
            }
            private set { }
        }
        internal static float CpuFrequency
        {
            get {
                if (_cpuFrequency != 0f) return _cpuFrequency;
                _cpuFrequency = SystemInfo.processorFrequency / 1000f;
                return _cpuFrequency;
            }
            private set { }
        }


        private static string GetMacAddress()
        {
            string physicalAddress = "unknown";
            NetworkInterface[] nice = NetworkInterface.GetAllNetworkInterfaces();

            foreach (var adaper in nice)
            {
                if (adaper.Description == "en0")
                {
                    physicalAddress = adaper.GetPhysicalAddress().ToString();
                    break;
                }
                else
                {
                    physicalAddress = adaper.GetPhysicalAddress().ToString();
                    if (!string.IsNullOrEmpty(physicalAddress))
                    {
                        break;
                    }
                }
            }

            return physicalAddress;
        }

        internal static int TimeZone
        {
            get
            {
                if(_timeZone == 0)
                {
                    _timeZone = (int)TimeUtils.GetZoneOffset();
                }
                return (int)_timeZone;
            }
        }

        internal static string Language
        {
            get
            {
                if (string.IsNullOrEmpty(_language))
                {
                    var lang = Application.systemLanguage;
                    if (lang == SystemLanguage.Unknown)
                    {
                        _language = Constants.VALUE_UNKNOWN;
                    } else
                    {
                        _language = lang.ToString();
                    }
                }
                return _language;
            }
        }
    }
}
#endif