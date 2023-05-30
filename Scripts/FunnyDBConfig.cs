using System;

namespace SoFunny.FunnyDB {

	/// <summary>
    /// FunnyDB OAID 配置类型
    /// </summary>
	public enum FDB_OAID_TYPE {
		/// <summary>
        /// 签名数据类型
        /// </summary>
		CertData,
		/// <summary>
        /// 签名文件类型
        /// </summary>
		CertFileName
	}

	/// <summary>
    /// FunnyDB 参数配置类
    /// </summary>
	public class FunnyDBConfig {
		internal string keyID;
		internal string keySecret;

		internal string endPoint = string.Empty;

		internal string deviceID = string.Empty;

		internal bool oaidEnable = false;
		internal FDB_OAID_TYPE oaidType;
		internal string oaidData;

		internal string channel = string.Empty;

		public FunnyDBConfig(string keyID, string keySecret) {
			this.keyID = keyID;
			this.keySecret = keySecret;
		}

		/// <summary>
        /// 设置 EndPoint 地址
        /// </summary>
        /// <param name="url"></param>
		public void SetEndPoint(string url) {
			endPoint = url;
		}

		/// <summary>
        /// 设置自定义设备唯一标识
        /// </summary>
        /// <param name="id"></param>
		public void SetDeviceID(string id) {
			deviceID = id;
		}

		/// <summary>
        /// 设置渠道标识
        /// </summary>
        /// <param name="value"></param>
		public void SetChannel(string value) {
			channel = value;
		}

		/// <summary>
        /// 开启 OAID
        /// </summary>
        /// <param name="type">开启类型</param>
        /// <param name="dataOrName">签名文件内容或文件名</param>
		public void OAIDEnable(FDB_OAID_TYPE type, string dataOrName) {
			oaidType = type;
			oaidData = dataOrName;
			oaidEnable = true;
		}

	}

}



