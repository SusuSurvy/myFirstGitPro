using UnityEngine;
using System.Collections;
using System;

namespace Framework.Common
{
    // 用于计算场景在不同分辨率和相机参数下的缩放因子，与UI适配
    public static class ResolutionAdapter
    {
        // 基准分辨率为1920*1080，竖屏相机OrthographicSize 17.06667，横屏 5.0
        private static readonly float StandardCameraSizePortrait = 17.06667f;
        private static readonly float StandardScreenHeightPortrait = 1920.0f;
        private static readonly float StandardCameraSizeLandscape = 5.0f;
        private static readonly float StandardScreenHeightLandscape = 1080.0f;

        private static float _landscapeToPortraitScaler = 1.0f;
        private static float _landscapeScaler = 1.0f;
        private static float _portraitScaler = 1.0f;

        public static bool NeedUpdateScaler = true;

        public static float LandscapeToPortraitScaler
        {
            get { return _landscapeToPortraitScaler; }
        }
        public static float PortraitToLandscapeScaler
        {
            get { return 1.0f / _landscapeToPortraitScaler; }
        }

        // 横屏缩放因子
        public static float LandscapeScaler
        {
            get
            {
                if (NeedUpdateScaler)
                {
                    UpdateScaler();
                }
                return _landscapeScaler;
            }
        }
        // 竖屏缩放因子
        public static float PortraitScaler
        {
            get
            {
                if (NeedUpdateScaler)
                {
                    UpdateScaler();
                }
                return _portraitScaler;
            }
        }

        static ResolutionAdapter()
        {
            _landscapeToPortraitScaler = (StandardScreenHeightLandscape * StandardCameraSizePortrait) / (StandardScreenHeightPortrait * StandardCameraSizeLandscape);
            NeedUpdateScaler = true;
        }

        public static void UpdateScaler()
        {
            GameObject canvasObj = GameObject.FindGameObjectWithTag("ScaleRefCanvas");
            if (canvasObj == null)
            {
                return;
            }
            Canvas canvas = canvasObj.GetComponent<Canvas>();
            if (canvas)
            {
                float scaleFactor = canvas.rootCanvas.scaleFactor;
                UpdateScalerLandscape(scaleFactor);
                UpdateScalerPortrait(scaleFactor);
                NeedUpdateScaler = false;
            }
        }

        private static void UpdateScalerLandscape(float canvasScalerFactor)
        {
            float standardHeight = StandardCameraSizeLandscape * 200;
            float cameraHeight = Camera.main.orthographicSize * 200;
            float screenHeight = Math.Min(Screen.width, Screen.height);
            float canvasHeight = StandardScreenHeightLandscape * canvasScalerFactor;
            float particleHeight = cameraHeight * canvasHeight / screenHeight;
            _landscapeScaler = particleHeight / standardHeight;
        }
        private static void UpdateScalerPortrait(float canvasScalerFactor)
        {
            float standardHeight = StandardCameraSizePortrait * 200;
            float cameraHeight = Camera.main.orthographicSize * 200;
            float screenHeight = Math.Max(Screen.width, Screen.height);
            float canvasHeight = StandardScreenHeightPortrait * canvasScalerFactor;
            float particleHeight = cameraHeight * canvasHeight / screenHeight;
            _portraitScaler = particleHeight / standardHeight;
        }

        public static void AdaptParticleScale(GameObject gameObj, float scale = 1.0f)
        {
            if (Screen.width > Screen.height)
            {
                AdaptParticleScaleLandscape(gameObj, scale);
            }
            else
            {
                AdaptParticleScalePortrait(gameObj, scale);
            }
        }

        // 调整横屏下粒子的缩放比例，gameObj为粒子的父对象，scale参数为额外缩放比例
        public static void AdaptParticleScaleLandscape(GameObject gameObj, float scale = 1.0f)
        {
            ScaleParticleSystem(gameObj, LandscapeScaler * scale);
        }
        // 调整竖屏下粒子的缩放比例，gameObj为粒子的父对象，scale参数为额外缩放比例
        public static void AdaptParticleScalePortrait(GameObject gameObj, float scale = 1.0f)
        {
            ScaleParticleSystem(gameObj, PortraitScaler * scale);
        }

        public static void ScaleParticleSystem(GameObject gameObj, float scale)
        {
            if (gameObj == null)
            {
                return;
            }

            var particleSystems = gameObj.GetComponentsInChildren<ParticleSystem>();
            foreach (var particleSystem in particleSystems)
            {
                var main = particleSystem.main;
                main.scalingMode = ParticleSystemScalingMode.Hierarchy;
            }

            gameObj.transform.localScale *= scale;

            //var particles = gameObj.GetComponentsInChildren<ParticleSystem>(true);
            //var max = particles.Length;
            //for (int idx = 0; idx < max; idx++)
            //{
            //    var particle = particles[idx];
            //    if (particle == null)
            //    {
            //        continue;
            //    }
            //    particle.scalingMode = ParticleSystemScalingMode.Local;
            //    particle.transform.localScale *= scale;
            //}
        }
    }
}
