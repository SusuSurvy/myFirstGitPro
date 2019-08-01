using System;
using UnityEngine;
using System.Collections;

namespace Lobby3D.Common
{
    public static class ConfigLoader
    {
        public static bool LoadTextFileSyn(string fileName, string path, Func<string, byte[], bool> loadFileDelegate)
        {
            bool result = false;
            WWW loadNetworkwww = new WWW(path);

            while (!loadNetworkwww.isDone)
            {
            }

            result = loadFileDelegate(fileName, loadNetworkwww.bytes);

            loadNetworkwww.Dispose();

            return result;
        }

        public static string[] GetCSVContent(byte[] contentTxt)
        {
            // (utf-8 no-bom) ģʽ
            string str = System.Text.Encoding.UTF8.GetString(contentTxt);
            if (str.EndsWith("\n"))
            {
                str = str.Substring(0, str.Length - 1); // ignore last \n
            }
            string[] csvContent = str.Split('\n');

            for (int index = 0; index < csvContent.Length; index++)
            {
                csvContent[index] = csvContent[index].TrimEnd(new char[] {'\r', '\n', '\t'});
                csvContent[index] = csvContent[index].TrimStart(new char[] {'\r', '\n', '\t'});
            }
            return csvContent;
        }

        public static byte[] GetFileBytes(string path)
        {
            WWW loadNetworkwww = new WWW(path);

            while (!loadNetworkwww.isDone)
            {
            }

            return loadNetworkwww.bytes;
        }
    }
}