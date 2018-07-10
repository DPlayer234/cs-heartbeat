using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heartbeat
{
    /// <summary>
    ///     Interface for the creation of usable coroutines.
    /// </summary>
    /// <typeparam name="TResult">The type of result values</typeparam>
    public interface ICoroutine<out TResult> : IEnumerable<TResult>
    {
        /// <summary> The last result of <seealso cref="Resume"/> </summary>
        TResult LastResult { get; }

        /// <summary> Whether the coroutine has finished. </summary>
        bool Finished { get; }

        /// <summary>
        ///     Resumes the coroutine once.
        /// </summary>
        /// <returns>The value yielded by the coroutine.</returns>
        TResult Resume();

        /// <summary>
        ///     Resets the coroutine
        /// </summary>
        void Reset();
    }

    /// <summary>
    ///     Implements <see cref="ICoroutine{TResult}"/> for basic functional coroutines.
    /// </summary>
    /// <typeparam name="TResult">The type of result values</typeparam>
    public class Coroutine<TResult> : ICoroutine<TResult>
    {
        /// <summary> The underlying enumerator. </summary>
        protected IEnumerator<TResult> procedure;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Coroutine{TResult}"/> class.
        /// </summary>
        /// <param name="procedure">The procedure to run.</param>
        public Coroutine(CoroutineProcedure procedure)
        {
            if (procedure == null) throw new NullReferenceException("A Coroutine cannot be started without an enumerator.");

            this.procedure = procedure.Invoke();
        }

        /// <summary>
        ///     Default constructor. Does nothing.
        /// </summary>
        protected Coroutine() { }

        /// <summary>
        ///     The delegate type for result-only coroutines.
        /// </summary>
        /// <returns>The enumerator for the coroutine.</returns>
        public delegate IEnumerator<TResult> CoroutineProcedure();

        /// <summary> The last result of <seealso cref="Resume"/>. </summary>
        public TResult LastResult { get; protected set; }

        /// <summary> Whether the coroutine has finished. </summary>
        public bool Finished { get; protected set; } = false;

        /// <summary>
        ///     Resumes the coroutine once.
        /// </summary>
        /// <returns>The value yielded by the coroutine.</returns>
        public TResult Resume()
        {
            if (this.Finished) throw new InvalidOperationException("Cannot resume a finished coroutine.");

            this.Finished = this.procedure.MoveNext();

            this.LastResult = this.procedure.Current;

            return this.LastResult;
        }

        /// <summary>
        ///     Resets the coroutine
        /// </summary>
        public void Reset()
        {
            this.procedure.Reset();
        }

        /// <summary>
        ///     Returns an enumerator which allows you to iterate over all the results of the coroutine.
        /// </summary>
        /// <returns>The enumerator</returns>
        IEnumerator<TResult> IEnumerable<TResult>.GetEnumerator()
        {
            while (!this.Finished)
            {
                yield return this.Resume();
            }
        }

        /// <summary>
        ///     Returns an enumerator which allows you to iterate over all the results of the coroutine.
        /// </summary>
        /// <returns>The enumerator</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return (this as IEnumerable<TResult>).GetEnumerator();
        }
    }

    /// <summary>
    ///     Implements <see cref="ICoroutine{TResult}"/> for basic functional coroutines with
    ///     a single variable parameter.
    /// </summary>
    /// <typeparam name="TArg">The type of the parameter</typeparam>
    /// <typeparam name="TResult">The type of result values</typeparam>
    public class Coroutine<TArg, TResult> : Coroutine<TResult>
    {
        /// <summary> The argument reference object passed to the procedure. </summary>
        private Reference<TArg> argument;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Coroutine{TArg, TResult}"/> class.
        /// </summary>
        /// <param name="procedure">The procedure to run.</param>
        public Coroutine(CoroutineProcedure procedure) : base()
        {
            if (procedure == null) throw new NullReferenceException("A Coroutine cannot be started without an enumerator.");

            this.argument = new Reference<TArg>();

            this.procedure = procedure.Invoke(this.argument);
        }

        /// <summary>
        ///     The delegate type for result and parameter coroutines.
        /// </summary>
        /// <returns>The enumerator for the coroutine.</returns>
        new public delegate IEnumerator<TResult> CoroutineProcedure(Reference<TArg> argument);

        /// <summary>
        ///     Gets or sets the argument passed to the coroutine.
        ///     Setting this value is not needed if <seealso cref="Resume(TArg)"/> is used.
        /// </summary>
        public TArg Argument
        {
            get
            {
                return this.argument.Value;
            }

            set
            {
                this.argument.Value = value;
            }
        }

        /// <summary>
        ///     Resumes the coroutine once.
        /// </summary>
        /// <param name="arg">The parameter given to the coroutine.</param>
        /// <returns>The value yielded by the coroutine.</returns>
        public TResult Resume(TArg arg)
        {
            this.argument.Value = arg;

            return this.Resume();
        }
    }
}
