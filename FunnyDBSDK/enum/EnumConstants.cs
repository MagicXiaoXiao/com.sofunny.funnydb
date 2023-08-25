namespace SoFunny.FunnyDB.PC
{
    public class EnumConstants
    {
        public enum DBSDK_STATUS_ENUM
        {
            /// <summary>
            /// 正常上报状态（默认）
            /// </summary>
            DEFAULT = 1,
            /// <summary>
            /// 立即暂停采集数据，不影响未上报数据的正常流程
            /// </summary>
            STOP_COLLECT = 2,
            /// <summary>
            /// 只采集不上报数据，数据直接入库
            /// </summary>
            ONLY_COLLECT = 3,
        }
        public enum DBSDK_SEND_TYPE_ENUM
        {
            /// <summary>
            /// 实时上报（默认）
            /// </summary>
            NOW = 1,
            /// <summary>
            /// 批量延迟
            /// </summary>
            DELAY = 2,
        }
        public enum DBSDK_CUSTOM_TYPE_ENUM
        {
            /// <summary>
            /// 用户
            /// </summary>
            USER = 1,
            /// <summary>
            /// 设备
            /// </summary>
            DEVICE = 2,
        }
        public enum DBSDK_OPERATE_TYPE_ENUM
        {
            /// <summary>
            /// 设置
            /// </summary>
            SET = 1,
            /// <summary>
            /// 新增
            /// </summary>
            ADD = 2,
            /// <summary>
            /// 唯一设置
            /// </summary>
            SET_ONCE = 3,
        }

        /// <summary>
        /// FunnyDB OAID 配置类型
        /// </summary>
        public enum FDB_OAID_TYPE
        {
            /// <summary>
            /// 签名数据类型
            /// </summary>
            CertData,
            /// <summary>
            /// 签名文件类型
            /// </summary>
            CertFileName
        }
    }
}