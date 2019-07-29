using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace Framework.Resource
{
    public class AsyncRequest
    {
        private enum RequestType
        {
            ResourceRequest = 0,
            BundleRequest,
            BaseRequest,

            All,
        }

        private RequestType type = RequestType.All;
        private ResourceRequest resourceRequest;
        private AssetBundleRequest bundleRequest;
        private AsyncOperation asyncRequest;

        public AsyncRequest(ResourceRequest request)
        {
            resourceRequest = request;
            type = RequestType.ResourceRequest;
        }

        public AsyncRequest(AssetBundleRequest request)
        {
            bundleRequest = request;
            type = RequestType.BundleRequest;
        }

        public AsyncRequest(AsyncOperation request)
        {
            asyncRequest = request;
            type = RequestType.BaseRequest;
        }

        public Object asset
        {
            get
            {

                if (type == RequestType.ResourceRequest
                    && resourceRequest != null)
                {
                    return resourceRequest.asset;
                }

                if (type == RequestType.BundleRequest
                    && bundleRequest != null)
                {
                    return bundleRequest.asset;
                }

                return null;
            }
        }

        public Object[] allAssets
        {
            get
            {
                if (type == RequestType.BundleRequest
                    && bundleRequest != null)
                {
                    return bundleRequest.allAssets;
                }

                return null;
            }
        }

        public bool isDone
        {
            get
            {
                if (type == RequestType.ResourceRequest
                    && resourceRequest != null)
                {
                    return resourceRequest.isDone;
                }

                if (type == RequestType.BundleRequest
                    && bundleRequest != null)
                {
                    return bundleRequest.isDone;
                }

                if(type == RequestType.BaseRequest 
                    && asyncRequest != null)
                {
                    return asyncRequest.isDone;
                }

                return true;
            }
        }
    }
}