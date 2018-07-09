using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heartbeat
{
    /// <summary>
    ///     A Timer with queable tasks.
    /// </summary>
    public class Timer
    {
        /// <summary> A list of all tasks </summary>
        protected List<TimerTask> tasks = new List<TimerTask>();
        
        /// <summary> The total time so far. </summary>
        public double TotalTime { get; private set; } = 0.0;

        /// <summary> Delegate for non-repeating tasks. </summary>
        public delegate void NoRepeatFunction();

        /// <summary>
        ///     Runs a given function after some time.
        /// </summary>
        /// <param name="delay">The delay in seconds</param>
        /// <param name="closure">The function</param>
        public void RunAfter(double delay, NoRepeatFunction closure)
        {
            this.tasks.Add(new NoRepeatTask(this, closure)
            {
                RunTime = this.TotalTime + delay
            });
        }

        /// <summary>
        ///     Starts a <seealso cref="Coroutine{TResult}"/> in the Timer.
        ///     The given procedure is expected to yield <seealso cref="double"/>? values.
        ///     The yielded value is the wait time in seconds until the coroutine is next
        ///     resumed. If null is returned, the coroutine will be canceled.
        /// </summary>
        /// <param name="procedure">The procedure</param>
        /// <param name="delay">The delay before the start.</param>
        /// <returns>The created coroutine</returns>
        public Coroutine<double?> StartCoroutine(Coroutine<double?>.CoroutineProcedure procedure, double delay = 0.0)
        {
            Coroutine<double?> coroutine = new Coroutine<double?>(procedure);

            this.tasks.Add(new RepeatTask(this, coroutine)
            {
                RunTime = this.TotalTime + delay
            });

            return coroutine;
        }

        /// <summary>
        ///     Progresses the timer by <seealso cref="Engine.PreciseDeltaTime"/>.
        /// </summary>
        public void Progress()
        {
            this.Progress(Engine.PreciseDeltaTime);
        }

        /// <summary>
        ///     Progresses the timer by <paramref name="deltaTime"/>.
        /// </summary>
        /// <param name="deltaTime">The time since the last update</param>
        public void Progress(double deltaTime)
        {
            this.TotalTime += deltaTime;

            for (int i = this.tasks.Count - 1; i >= 0; i--)
            {
                if (this.tasks[i].Progress(deltaTime))
                {
                    this.tasks.RemoveAt(i);
                }
            }
        }

        /// <summary>
        ///     General abstract class for tasks of the timer
        /// </summary>
        protected abstract class TimerTask
        {
            public TimerTask(Timer timer)
            {
                this.Timer = timer;
            }

            public Timer Timer { get; private set; }

            public double RunTime { get; set; } = 0.0;

            public bool Finished { get; protected set; } = false;

            public bool Progress(double deltaTime)
            {
                if (this.Timer.TotalTime > this.RunTime)
                {
                    this.Run();
                }

                return this.Finished;
            }

            protected abstract void Run();
        }

        /// <summary>
        ///     A task that never repeats.
        /// </summary>
        protected class NoRepeatTask : TimerTask
        {
            private NoRepeatFunction function;

            public NoRepeatTask(Timer timer, NoRepeatFunction function) : base(timer)
            {
                this.function = function;
            }

            protected override void Run()
            {
                this.function.Invoke();
                this.Finished = true;
            }
        }

        /// <summary>
        ///     A task that can repeat indefinitely.
        /// </summary>
        protected class RepeatTask : TimerTask
        {
            private Coroutine<double?> coroutine;

            public RepeatTask(Timer timer, Coroutine<double?> coroutine) : base(timer)
            {
                this.coroutine = coroutine;
            }

            protected override void Run()
            {
                double? response = this.coroutine.Resume();

                if (response == null || this.coroutine.Finished)
                {
                    this.Finished = true;
                    return;
                }

                this.RunTime = Math.Max(this.Timer.TotalTime, this.RunTime + (double)response);
            }
        }
    }
}
