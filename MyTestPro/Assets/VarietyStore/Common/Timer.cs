using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using JetBrains.Annotations;
using Object = UnityEngine.Object;

namespace Framework.Common
{
    public class Timer
    {
        public float Duration { get; private set; }

        public bool IsLooped { get; set; }

        public bool IsCompleted { get; private set; }

        public bool UsesRealTime { get; private set; }

        public bool IsPaused
        {
            get { return this.m_timeElapsedBeforePause.HasValue; }
        }

        public bool IsCancelled
        {
            get { return this.m_timeElapsedBeforeCancel.HasValue; }
        }

        public bool IsDone
        {
            get { return this.IsCompleted || this.IsCancelled || this.IsOwnerDestroyed; }
        }


        public static Timer New(float duration, Action onComplete, Action<float> onUpdate = null,
            bool isLooped = false, bool useRealTime = false, MonoBehaviour autoDestroyOwner = null)
        {
            if (Timer.m_manager == null)
            {
                TimerManager managerInScene = Object.FindObjectOfType<TimerManager>();
                if (managerInScene != null)
                {
                    Timer.m_manager = managerInScene;
                }
                else
                {
                    GameObject managerObject = new GameObject { name = "TimerManager" };
                    Timer.m_manager = managerObject.AddComponent<TimerManager>();
                }
            }

            Timer timer = new Timer(duration, onComplete, onUpdate, isLooped, useRealTime, autoDestroyOwner);
            Timer.m_manager.RegisterTimer(timer);

            return timer;
        }


        public static void Cancel(Timer timer)
        {
            if (timer != null)
            {
                timer.Cancel();
            }
        }

        public static void Pause(Timer timer)
        {
            if (timer != null)
            {
                timer.Pause();
            }
        }

        public static void PauseAll()
        {
            Timer.m_manager.PauseAllTimers();
        }

        public static void Resume(Timer timer)
        {
            if (timer != null)
            {
                timer.Resume();
            }
        }

        public static void ResumeAll()
        {
            Timer.m_manager.ResumeAllTimers();
        }

        public static void CancelAllRegisteredTimers()
        {
            if (Timer.m_manager != null)
            {
                Timer.m_manager.CancelAllTimers();
            }
        }

        public void Cancel()
        {
            if (this.IsDone)
            {
                return;
            }

            this.m_timeElapsedBeforeCancel = this.GetTimeElapsed();
            this.m_timeElapsedBeforePause = null;
        }

        public void Pause()
        {
            if (this.IsPaused || this.IsDone)
            {
                return;
            }

            this.m_timeElapsedBeforePause = this.GetTimeElapsed();
        }


        public void Resume()
        {
            if (!this.IsPaused || this.IsDone)
            {
                return;
            }

            this.m_timeElapsedBeforePause = null;
        }

        public float GetTimeElapsed()
        {
            if (this.IsCompleted || this.GetWorldTime() >= this.GetFireTime())
            {
                return this.Duration;
            }

            return this.m_timeElapsedBeforeCancel ??
                   this.m_timeElapsedBeforePause ??
                   this.GetWorldTime() - this.m_startTime;
        }

        public float GetTimeRemaining()
        {
            return this.Duration - this.GetTimeElapsed();
        }

        public float GetRatioComplete()
        {
            return this.GetTimeElapsed() / this.Duration;
        }

        public float GetRatioRemaining()
        {
            return this.GetTimeRemaining() / this.Duration;
        }

        #region Private
        private static TimerManager m_manager;

        private bool IsOwnerDestroyed
        {
            get { return this.m_hasAutoDestroyOwner && this.m_autoDestroyOwner == null; }
        }

        private readonly Action m_onComplete;
        private readonly Action<float> m_onUpdate;
        private float m_startTime;
        private float m_lastUpdateTime;

        private float? m_timeElapsedBeforeCancel;
        private float? m_timeElapsedBeforePause;

        private readonly MonoBehaviour m_autoDestroyOwner;
        private readonly bool m_hasAutoDestroyOwner;


        private Timer(float duration, Action onComplete, Action<float> onUpdate,
            bool isLooped, bool usesRealTime, MonoBehaviour autoDestroyOwner)
        {
            this.Duration = duration;
            this.m_onComplete = onComplete;
            this.m_onUpdate = onUpdate;

            this.IsLooped = isLooped;
            this.UsesRealTime = usesRealTime;

            this.m_autoDestroyOwner = autoDestroyOwner;
            this.m_hasAutoDestroyOwner = autoDestroyOwner != null;

            this.m_startTime = this.GetWorldTime();
            this.m_lastUpdateTime = this.m_startTime;
        }



        private float GetWorldTime()
        {
            return this.UsesRealTime ? Time.realtimeSinceStartup : Time.time;
        }

        private float GetFireTime()
        {
            return this.m_startTime + this.Duration;
        }

        private float GetTimeDelta()
        {
            return this.GetWorldTime() - this.m_lastUpdateTime;
        }

        private void Update()
        {
            if (this.IsDone)
            {
                return;
            }

            if (this.IsPaused)
            {
                this.m_startTime += this.GetTimeDelta();
                this.m_lastUpdateTime = this.GetWorldTime();
                return;
            }

            this.m_lastUpdateTime = this.GetWorldTime();

            if (this.m_onUpdate != null)
            {
                this.m_onUpdate(this.GetTimeElapsed());
            }

            if (this.GetWorldTime() >= this.GetFireTime())
            {

                if (this.m_onComplete != null)
                {
                    this.m_onComplete();
                }

                if (this.IsLooped)
                {
                    this.m_startTime = this.GetWorldTime();
                }
                else
                {
                    this.IsCompleted = true;
                }
            }
        }
        #endregion


        #region Timer Manager
        private class TimerManager : MonoBehaviour
        {
            private List<Timer> m_timers = new List<Timer>();
            private List<Timer> m_timersToAdd = new List<Timer>();

            public void RegisterTimer(Timer timer)
            {
                this.m_timersToAdd.Add(timer);
            }

            public void CancelAllTimers()
            {
                foreach (Timer timer in this.m_timers)
                {
                    timer.Cancel();
                }

                this.m_timers = new List<Timer>();
                this.m_timersToAdd = new List<Timer>();
            }

            public void PauseAllTimers()
            {
                foreach (Timer timer in this.m_timers)
                {
                    timer.Pause();
                }
            }

            public void ResumeAllTimers()
            {
                foreach (Timer timer in this.m_timers)
                {
                    timer.Resume();
                }
            }


            private void Update()
            {
                this.UpdateAllTimers();
            }

            private void UpdateAllTimers()
            {
                if (this.m_timersToAdd.Count > 0)
                {
                    this.m_timers.AddRange(this.m_timersToAdd);
                    this.m_timersToAdd.Clear();
                }

                foreach (Timer timer in this.m_timers)
                {
                    timer.Update();
                }

                this.m_timers.RemoveAll(it => it.IsDone);
            }
        }

        #endregion

    }
}