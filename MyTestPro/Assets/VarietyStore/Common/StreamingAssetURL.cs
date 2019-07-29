using UnityEngine;

namespace Framework.Common
{
    public static class StreamingAssetURL
    {
        public static readonly string m_streamingAssetURL;
        public static readonly string m_writeURL;
        public static readonly string m_resourceURL;

        static StreamingAssetURL()
        {
            //if (Application.platform == RuntimePlatform.WindowsWebPlayer ||
            //    Application.platform == RuntimePlatform.OSXWebPlayer)
            //{
            //    m_streamingAssetURL = Application.dataPath;
            //    m_writeURL = "";
            //}
            //else 
            if (Application.platform == RuntimePlatform.Android)
            {
                m_streamingAssetURL = Application.streamingAssetsPath + "/";
                m_writeURL = Application.persistentDataPath + "/";
                m_resourceURL = m_streamingAssetURL;
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                m_streamingAssetURL = "file://" + Application.dataPath + "/Raw/";
                string fileNameBase = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/'));
                m_writeURL = fileNameBase.Substring(0, fileNameBase.LastIndexOf('/')) + "/Documents/";
            }
            else
            {
                //RuntimePlatform.WindowsPlayer, RuntimePlatform.WindowsEditor
                m_streamingAssetURL = "file://" + Application.dataPath + "/StreamingAssets/";
                m_writeURL = Application.dataPath + "/StreamingAssets/";
                m_resourceURL = m_writeURL;
            }
        }
    }
}
