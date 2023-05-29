#if (UNITY_EDITOR || UNITY_STANDALONE)

using System.Net.NetworkInformation;
using UnityEngine;

/// <summary>
/// Devices Info
/// </summary>
namespace SoFunny.FunnyDB {
    public class DevicesInfo {
        private static string deviceId;
        private static string userId;
        private static string channel;
        private static string sdkType;
        private static string sdkVersion;
        private static string deviceModel;
        private static string manufacturer;
        private static int screenHeight;
        private static int screenWidth;
        private static string os;
        private static string osPlatform;
        private static string osVersion;
        private static string carrier;

        public static string DeviceId {
            get {
                if (string.IsNullOrEmpty(deviceId)) {
                    return GetMacAddress();
                }
                return deviceId;
            }
            set { deviceId = value; }
        }
        public static string UserId {
            get { return userId; }
            set { userId = value; }
        }
        public static string Channel {
            get {
                return string.IsNullOrEmpty(channel) ? "unknown": channel;
            }
            set { channel = value; }
        }
        public static string SdkType {
            get { return Constants.VALUE_SDK_TYPE; }
        }
        public static string SdkVersion {
            get { return Constants.FUNNY_DB_VERSION; }
        }
        public static string DeviceModel {
            get { return SystemInfo.deviceModel; }
            set { deviceModel = value; }
        }
        public static string Manufacturer {
            get { return Constants.VALUE_UNKNOWN; }
        }
        public static int ScreenHeight {
            get { return Screen.height; }
        }
        public static int ScreenWidth {
            get { return Screen.width; }
        }
        public static string Os {
            get { return SystemInfo.operatingSystem; }
        }
        public static string OsPlatform {
            get { return Application.platform.ToString(); }
        }
        public static string OsVersion {
            get { return Constants.VALUE_UNKNOWN; }
        }
        public static string Carrier {
            get { return Constants.VALUE_UNKNOWN; }
        }


        private static string GetMacAddress() {
            string physicalAddress = "unknown";
            NetworkInterface[] nice = NetworkInterface.GetAllNetworkInterfaces();

            foreach (var adaper in nice) {
                if (adaper.Description == "en0") {
                    physicalAddress = adaper.GetPhysicalAddress().ToString();
                    break;
                }
                else {
                    physicalAddress = adaper.GetPhysicalAddress().ToString();
                    if (!string.IsNullOrEmpty(physicalAddress)) {
                        break;
                    }
                }
            }

            return physicalAddress;
        }

    }
}

#endif