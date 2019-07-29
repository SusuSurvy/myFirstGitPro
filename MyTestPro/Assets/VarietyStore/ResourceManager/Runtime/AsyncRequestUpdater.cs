using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace Framework.Resource
{
    public class AsyncRequestUpdater : MonoBehaviour
    {
        public  ResourceManager ResourceManager { get; set; }


        public void Update()
        {
            if(ResourceManager != null)
            {
                ResourceManager.UpdateAsyncRequest();
            }
        }

        public static void Init(ResourceManager manager)
        {
            var managerObj = GameObject.Find(Configs.RESOURCES_MANAGER_OBJECT_NAME);
            if (managerObj == null)
            {
                managerObj = new GameObject(Configs.RESOURCES_MANAGER_OBJECT_NAME);
                GameObject.DontDestroyOnLoad(managerObj);
                AsyncRequestUpdater updater = managerObj.AddComponent<AsyncRequestUpdater>();
                updater.ResourceManager = manager;
            }
        }

        public static void Destroy()
        {
            var managerObj = GameObject.Find(Configs.RESOURCES_MANAGER_OBJECT_NAME);
            if (managerObj != null)
            {
                GameObject.Destroy(managerObj);
            }
        }
    }
}