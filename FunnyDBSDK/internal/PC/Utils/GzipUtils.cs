#if UNITY_STANDALONE || UNITY_EDITOR

using System;
using System.IO;
using System.IO.Compression;
/// <summary>
/// GzipUtils
/// </summary>
namespace SoFunny.FunnyDB.PC
{

    internal static class GzipUtils
    {
        internal static byte[] Compress(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (GZipStream gzipStream = new GZipStream(ms, CompressionMode.Compress))
                {
                    gzipStream.Write(data, 0, data.Length);
                }
                return ms.ToArray();
            }
        }

        private static byte[] CloneByteArray(byte[] array)
        {
            if (array == null)
            {
                return null;
            }
            return (byte[])array.Clone();
        }

        internal static byte[] AddAll(byte[] array1, params byte[] array2)
        {
            if (array1 == null)
            {
                return CloneByteArray(array2);
            }
            else if (array2 == null)
            {
                return CloneByteArray(array1);
            }

            byte[] joinedArray = new byte[array1.Length + array2.Length];
            Array.Copy(array1, 0, joinedArray, 0, array1.Length);
            Array.Copy(array2, 0, joinedArray, array1.Length, array2.Length);
            return joinedArray;
        }
    }
}
#endif