using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SmallTown.Common
{

    public interface ITimeout
    {
        bool Cancel();
    }
    public class HashWheelTimer
    {
        public readonly static Queue<Timeout> TimePool = new Queue<Timeout>();
        const int WorkerStateInit = 0;
        const int WorkerStateStarted = 1;
        const int WorkerStateShutdown = 2;

        readonly int mask;
        readonly long tickDuration;

        readonly long tickInterval;

        int wtState = WorkerStateInit; // 0 - init, 1 - started, 2 - shut down

        readonly HashedWheelSlot[] wheel;

        public int WheelTimer
        {
            get
            {
                return wtState;
            }
        }

        DateTime startTime;
        DateTime StartTime
        {
            get
            {
                return startTime;
            }
        }

        long ticks = 0;
        public HashWheelTimer(
             TimeSpan tickInterval,
             int ticksPerWheel
            )
        {
            if (tickInterval <= TimeSpan.Zero)
            {
                throw new ArgumentException(" TickInterval must be greater than 0: " + tickInterval.ToString());
            }
            if (Math.Ceiling(tickInterval.TotalMilliseconds) > int.MaxValue)
            {
                throw new ArgumentException(" TickInterval must be greater than 0: must be less than or equal to "+int.MaxValue.ToString()+" ms.");
            }
            if (ticksPerWheel <= 0)
            {
                throw new ArgumentException(" TickInterval must be greater than 0: "+ ticksPerWheel.ToString());
            }
            if (ticksPerWheel > int.MaxValue / 2 + 1)
            {
                throw new ArgumentOutOfRangeException(
                    "TicksPerWheel may not be greater than 2^30: "+ ticksPerWheel.ToString());
            }

            // Normalize ticksPerWheel to power of two and initialize the wheel.
            //归一化ticksPerWheel为2的幂并初始化轮子
            this.wheel = CreateWheel(this,ticksPerWheel);

            this.mask = this.wheel.Length - 1;

            this.tickInterval =Convert.ToInt32(tickInterval.TotalMilliseconds);
            this.tickDuration = tickInterval.Ticks;

            for(int i=0;i< this.wheel.Length * 4; i++)
            {
                TimePool.Enqueue(new Timeout(this));
            }

            // Prevent overflow
            if (this.tickDuration >= long.MaxValue / this.wheel.Length)
            {
                throw new ArgumentException(
                    string.Format(
                        "tickInterval: {0} (expected: 0 < tickInterval in nanos < {1}",
                        tickInterval,
                        long.MaxValue / this.wheel.Length));
            }
        }

        ~HashWheelTimer()
        {
            // This object is going to be GCed and it is assumed the ship has sailed to do a proper shutdown. If
            // we have not yet shutdown then we want to make sure we decrement the active instance count.
            wtState = WorkerStateShutdown;

            TimePool.Clear();
        }

        public void Tick()
        {
            if(wtState== WorkerStateInit)
            {
                wtState = WorkerStateStarted;
                startTime = DateTime.Now;
            }
            if(wtState!= WorkerStateStarted)
            {
                return;
            }

            TimeSpan currentDiff=DateTime.Now - startTime;

            long willTick_delta = currentDiff.Ticks / (tickInterval*10000) - ticks;


            if (willTick_delta > 0)
            {
                for(long i=0;i< willTick_delta; i++)
                {
                    int idx = (int)(this.ticks & this.mask);
                    //this.ProcessCancelledTasks();
                    HashedWheelSlot slot = this.wheel[idx];
                    slot.ExpireTimeouts();

                    //UnityEngine.Debug.Log("<color=yellow>" + idx.ToString() + "]</color>");
                    this.ticks++;


                    
                }
                
               
            }
        }

        static HashedWheelSlot[] CreateWheel(HashWheelTimer owner, int ticksPerWheel)
        {
            ticksPerWheel = NormalizeTicksPerWheel(ticksPerWheel);
            var wheel = new HashedWheelSlot[ticksPerWheel];
            for (int i = 0; i < wheel.Length; i++)
            {
                wheel[i] = new HashedWheelSlot(owner);
            }
            return wheel;
        }

        static int NormalizeTicksPerWheel(int ticksPerWheel)
        {
            int normalizedTicksPerWheel = 1;
            while (normalizedTicksPerWheel < ticksPerWheel)
            {
                normalizedTicksPerWheel <<= 1;
            }
            return normalizedTicksPerWheel;
        }


        public Timeout GetTimeOut()
        {
            if (TimePool.Count > 0)
            {
                return TimePool.Dequeue();
            }
            else
            {
                throw new Exception("Timeout not enough please check design");
            }
        }

        public void PutBack(Timeout o)
        {
            if (TimePool.Count < (wheel.Length * 4))
            {
                TimePool.Enqueue(o);
            }
            else
            {
                throw new Exception("SomeOne Timeout not belong this wheel.");
            }
        }

        /// <summary>
        /// 添加重复的定时器
        /// </summary>
        /// <param name="task">触发的任务</param>
        /// <param name="interval">时间间隔</param>
        /// <param name="delay">第一触发的时间到现在的时间间隔</param>
        public ITimeout AddRepeatTimeTrigger(Action task,Action finial, TimeSpan interval, int times,TimeSpan delay)
        {
            if (wtState != WorkerStateStarted)
            {
                if (wtState == WorkerStateInit)
                {
                    throw new Exception("The WheelTime not start,please start first.");
                }
                else if (wtState == WorkerStateShutdown)
                {
                    throw new Exception("The WheelTime was Shutdown,please check again.");
                }

            }
            if (task == null)
            {
                throw new ArgumentNullException("task is null");
            }


            if (TimePool.Count > 0)
            {
                Timeout t = TimePool.Dequeue();
                t.Reset();

               

                t.SetRepeat(task, finial, interval, times);

                TimeSpan deadline = DateTime.Now + delay - startTime;
                if (deadline > TimeSpan.Zero)
                {
                    int addTick = Mathf.CeilToInt(delay.Ticks / (tickInterval * 10000));
                    if (addTick == 0) addTick = 1;//下一次
                    long totalTicks = addTick + ticks;

                    t.RemainingRounds = addTick / this.wheel.Length;
                    int slot_TotalIndex =(int)(totalTicks & mask);
                    this.wheel[slot_TotalIndex].AddTimeout(t);
                }
                else
                {
                    throw new Exception("触发时间必须比当前时间晚.");
                }
                return t;
            }
            return null;
        }


        internal void AttemperTimeTriggerToNext(Timeout timeout)
        {
            if (wtState != WorkerStateStarted)
            {
                if (wtState == WorkerStateInit)
                {
                    throw new Exception("The WheelTime not start,please start first.");
                }
                else if (wtState == WorkerStateShutdown)
                {
                    throw new Exception("The WheelTime was Shutdown,please check again.");
                }

            }
            timeout.WaitForNextTriggle();
            TimeSpan deadline = timeout.LastTriggleTime.Add(timeout.Interval)-DateTime.Now;
            if (deadline > TimeSpan.Zero)
            {
                int addTick = Mathf.CeilToInt(timeout.Interval.Ticks / (tickInterval * 10000));
               
                //int currentIdx = (int)(ticks & mask);

                long totalTicks = addTick + ticks;

                timeout.RemainingRounds = addTick / this.wheel.Length;
                int slot_TotalIndex = (int)(totalTicks & mask);

                //UnityEngine.Debug.Log("<color=red>[Current idx=" + currentIdx.ToString() + "],[NextIdx="+ slot_TotalIndex .ToString()+ "]</color>");
                this.wheel[slot_TotalIndex].AddTimeout(timeout);
            }
            else//已经晚了就在下一个tick触发吧
            {
                int idex = (int)(ticks+1) & mask;
                this.wheel[idex].AddTimeout(timeout);
            }
        }

        /// <summary>
        /// 添加一般触发器
        /// </summary>
        /// <param name="task">触发的任务</param>
        /// <param name="delay">触发的时间到现在的时间间隔</param>
        public ITimeout AddNormalTimeTrigger(Action task, TimeSpan delay)
        {
            if (wtState != WorkerStateStarted)
            {
                if (wtState == WorkerStateInit)
                {
                    throw new Exception("The WheelTime not start,please start first.");
                }
                else if (wtState == WorkerStateShutdown)
                {
                    throw new Exception("The WheelTime was Shutdown,please check again.");
                }
                
            }

            if (task == null)
            {
                throw new ArgumentNullException("task is null");
            }
            if (TimePool.Count > 0)
            {
                Timeout t = TimePool.Dequeue();
                t.Reset();

                TimeSpan deadline = DateTime.Now + delay - startTime;
                t.SetNormal(task, deadline);

               
                if (deadline > TimeSpan.Zero)
                {
                    t.Deadline = deadline;

                    int addTick = Mathf.CeilToInt(delay.Ticks / (tickInterval * 10000));
                    if (addTick == 0) addTick = 1;//下一次
                    long totalTicks = addTick + ticks;


                    t.RemainingRounds = addTick / this.wheel.Length;
                    int slot_TotalIndex = (int)(totalTicks & mask);

                    int idx = (int)(this.ticks & this.mask);


                    //int slot_TotalIndex = (int)(totalTicks % this.wheel.Length);

                    this.wheel[slot_TotalIndex].AddTimeout(t);
                }
                else
                {
                    throw new Exception("触发时间必须比当前时间晚.");
                }
                return t;
            }


            return null;
        }



        public void Stop()
        {
            wtState = WorkerStateShutdown;
        }
    }

    #region Class HashedWheelTimeout
    public sealed class Timeout: ITimeout
    {
        const int StInit = 0;
        const int StExpired = 2;
        internal const int StCanceled = 1;


        internal HashWheelTimer timer;
        internal TimeSpan Deadline;
        internal DateTime lastTriggerTime= DateTime.MinValue;
        internal Action task;
        internal Action finialTask;
        internal int repeatTimes;
        internal int state = StInit;

        // remainingRounds will be calculated and set by transferTimeoutsToBuckets() before the
        // HashedWheelTimeout will be added to the correct HashedWheelBucket.
        //剩余轮数将在transferTimeoutsToBuckets()中计算和设置，他在 HashedWheelTimeout之前将被添加到正确的HashedWheelSlot中
        internal long RemainingRounds;

        // This will be used to chain timeouts in HashedWheelTimerBucket via a double-linked-list.
        // As only the workerThread will act on it there is no need for synchronization / volatile.
        internal Timeout Next;

        internal Timeout Prev;

        // The Slot to which the timeout was added
        //所添加到定时器的指定槽
        internal HashedWheelSlot Slot;


        bool isRepeat = false;
        public bool IsRepeat
        {
            get
            {
                return isRepeat;
            }
        }

        public TimeSpan Interval
        {
            get
            {
                return Deadline;
            }
        }


        public DateTime LastTriggleTime
        {
            get
            {
                return lastTriggerTime;
            }
        }

        public Timeout(HashWheelTimer timer)
        {
            this.timer = timer;
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="timer"></param>
        ///// <param name="task"></param>
        ///// <param name="isRepeat"></param>
        ///// <param name="deadline"></param>
        //public Timeout(HashWheelTimer timer, Action task, bool isRepeat, TimeSpan deadline)
        //{
        //    this.timer = timer;
        //    this.task = task;
        //    this.isRepeat = isRepeat;//重复的时间会被每个槽添加，每个槽上得触发时间都不一样。
            
        //    this.Deadline = deadline;
        //}

        public void SetNormal(Action task, TimeSpan deadline)
        {
            this.task = task;
            this.isRepeat = false;
            this.Deadline = deadline;
            this.lastTriggerTime = DateTime.MinValue;
        }
        public void SetRepeat(Action task, Action finial, TimeSpan interval,int times=-1)
        {
            this.task = task;
            this.finialTask = finial;
            this.repeatTimes = times;
            this.isRepeat = true;
            this.Deadline = interval;
            this.lastTriggerTime = DateTime.MinValue;

        }

        public void Reset()
        {
            this.task = null;
            this.Deadline = TimeSpan.Zero;
            this.isRepeat = false;
            this.finialTask = null;
            this.repeatTimes = -1;
            this.lastTriggerTime = DateTime.MinValue;
            state = StInit;
        }

        public void WaitForNextTriggle()
        {
            state = StInit;
        }

        public HashWheelTimer Timer
        {
            get
            {
                return this.timer;
            }
        }

        public Action Task
        {
            get
            {
                return task;
            }
        }
        public bool Cancel()
        {
            this.state = StCanceled;
            return true;
        }

        internal void Remove()
        {
            HashedWheelSlot bucket = this.Slot;
            if (bucket != null)
            {
                // timeout got canceled before it was added to the bucket
                bucket.Remove(this);
            }
        }


        internal int State
        {
            get
            {
                return this.state;
            }
        }

        public bool Canceled
        {
            get
            {
                return this.State == StCanceled;
            }
        }

        public bool Expired
        {
            get
            {
                return this.State == StExpired;
            }
        }


        internal void Expire()
        {
            this.state = StExpired;
            //try
            //{
            //    this.Task.Run(this);
            //}
            //catch (Exception t)
            //{
            //    //if (Logger.WarnEnabled)
            //    //{
            //    //    Logger.Warn($"An exception was thrown by {this.Task.GetType().Name}.", t);
            //    //}
            //}
           
            if (this.Task != null)
            {
                //long deltaTime = 0;
                //if (this.lastTriggerTime != DateTime.MinValue)
                //{
                //    deltaTime = Convert.ToInt64((DateTime.Now - this.lastTriggerTime).TotalMilliseconds);
                //}

                //this.lastTriggerTime = DateTime.Now;
                //this.Task(deltaTime);
                this.lastTriggerTime = DateTime.Now;
                this.Task();
            }
            if (isRepeat)
            {
                this.repeatTimes--;
                

                if (this.repeatTimes == 0 && this.finialTask!=null)
                {
                    this.finialTask();
                }

            }
           
        }
        /*
        public override string ToString()
        {
            PreciseTimeSpan currentTime = PreciseTimeSpan.FromStart - this.timer.StartTime;
            TimeSpan remaining = this.Deadline - currentTime.ToTimeSpan();

            StringBuilder buf = new StringBuilder(192)
                .Append(this.GetType().Name)
                .Append('(')
                .Append("deadline: ");
            if (remaining > TimeSpan.Zero)
            {
                buf.Append(remaining)
                    .Append(" later");
            }
            else if (remaining < TimeSpan.Zero)
            {
                buf.Append(-remaining)
                    .Append(" ago");
            }
            else
            {
                buf.Append("now");
            }

            if (this.Canceled)
            {
                buf.Append(", cancelled");
            }

            return buf.Append(", task: ")
                .Append(this.Task)
                .Append(')')
                .ToString();
        }
        */
    }
    #endregion

    #region Class HashedWheelSlot
    /// <summary>
    /// Slot that stores HashedWheelTimeouts. These are stored in a linked-list like datastructure to allow easy
    /// removal of HashedWheelTimeouts in the middle. Also the HashedWheelTimeout act as nodes themself and so no
    /// extra object creation is needed.
    /// </summary>
    sealed class HashedWheelSlot
    {
        // Used for the linked-list datastructure
        Timeout head;
        Timeout tail;

        HashWheelTimer woner;
        public HashedWheelSlot(HashWheelTimer timer)
        {
            woner = timer;
        }

        public void AddTimeout(Timeout timeout)
        {
            timeout.Slot = this;
            if (this.head == null)
            {
                this.head = this.tail = timeout;
            }
            else
            {
                this.tail.Next = timeout;
                timeout.Prev = this.tail;
                this.tail = timeout;
            }
        }

        public void ExpireTimeouts()
        {
            Timeout timeout = this.head;

            // process all timeouts
            while (timeout != null)
            {
                Timeout next = timeout.Next;
                if (timeout.Canceled)
                {
                    next = this.Remove(timeout);
                    woner.PutBack(next);
                    timeout = next;
                    continue;
                }
                if (timeout.RemainingRounds <= 0)
                {
                    next = this.Remove(timeout);
                    timeout.Expire();
                    if (timeout.IsRepeat && timeout.repeatTimes!=0)//如果是重复时间就重新计算所在槽
                    {
                        woner.AttemperTimeTriggerToNext(timeout);
                    }
                    else
                    {
                        woner.PutBack(next);
                    }
                }
                else
                {
                    timeout.RemainingRounds--;
                }
                timeout = next;
            }
        }

        public Timeout Remove(Timeout timeout)
        {
            Timeout next = timeout.Next;
            // remove timeout that was either processed or cancelled by updating the linked-list
            if (timeout.Prev != null)
            {
                timeout.Prev.Next = next;
            }
            if (timeout.Next != null)
            {
                timeout.Next.Prev = timeout.Prev;
            }

            if (timeout == this.head)
            {
                // if timeout is also the tail we need to adjust the entry too
                if (timeout == this.tail)
                {
                    this.tail = null;
                    this.head = null;
                }
                else
                {
                    this.head = next;
                }
            }
            else if (timeout == this.tail)
            {
                // if the timeout is the tail modify the tail to be the prev node.
                this.tail = timeout.Prev;
            }
            // null out prev, next and bucket to allow for GC.
            timeout.Prev = null;
            timeout.Next = null;
            timeout.Slot = null;
            return next;
        }
        public void ClearTimeouts(HashSet<Timeout> set)
        {
            while (true)
            {
                Timeout timeout = this.PollTimeout();
                if (timeout == null)
                {
                    return;
                }
                if (timeout.Expired || timeout.Canceled)
                {
                    continue;
                }
                set.Add(timeout);
            }
        }

        Timeout PollTimeout()
        {
            Timeout head = this.head;
            if (head == null)
            {
                return null;
            }
            Timeout next = head.Next;
            if (next == null)
            {
                this.tail = this.head = null;
            }
            else
            {
                this.head = next;
                next.Prev = null;
            }

            // null out prev and next to allow for GC.
            head.Next = null;
            head.Prev = null;
            head.Slot = null;
            return head;
        }
    }
    #endregion
}

