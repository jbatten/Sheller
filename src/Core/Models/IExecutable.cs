using System;
using System.Threading.Tasks;

namespace Sheller.Models
{
    /// <summary>
    /// A top-level interface for executables.
    /// </summary>
    public interface IExecutable
    {
        /// <summary>
        /// Executes the executable.
        /// </summary>
        /// <returns>A task which results in an <see cref="ICommandResult"/> (i.e., the result of the execution).</returns>
        Task<ICommandResult> ExecuteAsync();
        /// <summary>
        /// Executes the executable.
        /// </summary>
        /// <param name="resultSelector">A <see cref="Func{T}"/> which takes an <see cref="ICommandResult"/> and computes a new <typeparamref name="TResult"/> to be returned.</param>
        /// <typeparam name="TResult">The resulting type as defined by the <paramref name="resultSelector"/>.</typeparam>
        /// <returns>A task which results in a <typeparamref name="TResult"/> (i.e., the result of the execution).</returns>
        Task<TResult> ExecuteAsync<TResult>(Func<ICommandResult, TResult> resultSelector);
        /// <summary>
        /// Executes the executable.
        /// </summary>
        /// <param name="resultSelector">A <see cref="Func{T}"/> which takes an <see cref="ICommandResult"/> and computes a new <see cref="Task"/> of <typeparamref name="TResult"/> to be returned.</param>
        /// <typeparam name="TResult">The resulting type as defined by the <paramref name="resultSelector"/>.</typeparam>
        /// <returns>A task which results in a <typeparamref name="TResult"/> (i.e., the result of the execution).</returns>
        Task<TResult> ExecuteAsync<TResult>(Func<ICommandResult, Task<TResult>> resultSelector);

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>A `new` instance of type <see cref="IExecutable"/> with the same settings as the invoking instance.</returns>
        IExecutable Clone();

        /// <summary>
        /// Adds an argument (which are appended space-separated to the execution command) to the execution context and returns a `new` context instance.
        /// </summary>
        /// <param name="args">An arbitrary list of strings to be added as parameters.</param>
        /// <returns>A `new` instance of type <see cref="IExecutable"/> with the arguments passed to this call.</returns>
        IExecutable WithArgument(params string[] args);

        /// <summary>
        /// Sets the timeout on the entire execution of this entire execution context.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        /// <returns>A `new` instance of type <see cref="IExecutable"/> with the timeout set to the value passed to this call.</returns>
        IExecutable WithTimeout(TimeSpan timeout);

        /// <summary>
        /// Adds a standard output handler (of which there may be many) to the execution context and returns a `new` context instance.
        /// </summary>
        /// <param name="standardOutputHandler">An <see cref="Action"/> that handles a new line in the standard output of the executable.</param>
        /// <returns>A `new` instance of type <see cref="IExecutable"/> with the standard output handler passed to this call.</returns>
        IExecutable WithStandardOutputHandler(Action<string> standardOutputHandler);
        /// <summary>
        /// Adds an error output handler (of which there may be many) to the execution context and returns a `new` context instance.
        /// </summary>
        /// <param name="standardErrorHandler">An <see cref="Action"/> that handles a new line in the standard error of the executable.</param>
        /// <returns>A `new` instance of type <see cref="IExecutable"/> with the standard error handler passed to this call.</returns>
        IExecutable WithStandardErrorHandler(Action<string> standardErrorHandler);

        /// <summary>
        /// Adds a wait <see cref="Func{T}"/> (of which there may be many) to the execution context and returns a `new` context instance.
        /// </summary>
        /// <param name="waitFunc">A <see cref="Func{T}"/> which takes an <see cref="ICommandResult"/> and returns a <see cref="Task"/> which will function as wait condition upon the completion of execution.</param>
        /// <returns>A `new` instance of type <see cref="IExecutable"/> with the wait func passed to this call.</returns>
        IExecutable WithWait(Func<ICommandResult, Task> waitFunc);
        /// <summary>
        /// Sets the wait timeout on the <see cref="WithWait"/> <see cref="Func{T}"/>.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        /// <returns>A `new` instance of type <see cref="IExecutable"/> with the wait timeout set to the value passed to this call.</returns>
        IExecutable WithWaitTimeout(TimeSpan timeout);
    }

    /// <summary>
    /// A top-level interface for executables.
    /// </summary>
    /// <typeparam name="TExecutable">The type of the executable class implementing this interface.  This allows the base class to return `new` instances for daisy chaining.</typeparam>
    public interface IExecutable<out TExecutable> : IExecutable where TExecutable : IExecutable<TExecutable>
    {
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>A `new` instance of <typeparamref name="TExecutable"/> with the same settings as the invoking instance.</returns>
        new TExecutable Clone();

        /// <summary>
        /// Adds an argument (which are appended space-separated to the execution command) to the execution context and returns a `new` context instance.
        /// </summary>
        /// <param name="args">An arbitrary list of strings to be added as parameters.</param>
        /// <returns>A `new` instance of <typeparamref name="TExecutable"/> with the arguments passed to this call.</returns>
        new TExecutable WithArgument(params string[] args);

        /// <summary>
        /// Sets the timeout on the entire execution of this entire execution context.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        /// <returns>A `new` instance of <typeparamref name="TExecutable"/> with the timeout set to the value passed to this call.</returns>
        new TExecutable WithTimeout(TimeSpan timeout);

        /// <summary>
        /// Adds a standard output handler (of which there may be many) to the execution context and returns a `new` context instance.
        /// </summary>
        /// <param name="standardOutputHandler">An <see cref="Action"/> that handles a new line in the standard output of the executable.</param>
        /// <returns>A `new` instance of <typeparamref name="TExecutable"/> with the standard output handler passed to this call.</returns>
        new TExecutable WithStandardOutputHandler(Action<string> standardOutputHandler);
        /// <summary>
        /// Adds an error output handler (of which there may be many) to the execution context and returns a `new` context instance.
        /// </summary>
        /// <param name="standardErrorHandler">An <see cref="Action"/> that handles a new line in the standard error of the executable.</param>
        /// <returns>A `new` instance of <typeparamref name="TExecutable"/> with the standard error handler passed to this call.</returns>
        new TExecutable WithStandardErrorHandler(Action<string> standardErrorHandler);

        /// <summary>
        /// Adds a wait <see cref="Func{T}"/> (of which there may be many) to the execution context and returns a `new` context instance.
        /// </summary>
        /// <param name="waitFunc">A <see cref="Func{T}"/> which takes an <see cref="ICommandResult"/> and returns a <see cref="Task"/> which will function as wait condition upon the completion of execution.</param>
        /// <returns>A `new` instance of <typeparamref name="TExecutable"/> with the wait func passed to this call.</returns>
        new TExecutable WithWait(Func<ICommandResult, Task> waitFunc);
        /// <summary>
        /// Sets the wait timeout on the <see cref="WithWait"/> <see cref="Func{T}"/>.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        /// <returns>A `new` instance of <typeparamref name="TExecutable"/> with the wait timeout set to the value passed to this call.</returns>
        new TExecutable WithWaitTimeout(TimeSpan timeout);
    }

    /// <summary>
    /// An interface for executables that define the execute method with a special result type.
    /// </summary>
    /// <typeparam name="TExecutable">The type of the executable class implementing this interface.  This allows the base class to return `new` instances for daisy chaining.</typeparam>
    /// <typeparam name="TResult">The result type of the executable.</typeparam>
    public interface IExecutable<out TExecutable, TResult> : IExecutable<TExecutable> where TExecutable : IExecutable<TExecutable>
    {
        /// <summary>
        /// Executes the executable.
        /// </summary>
        /// <returns>A task which results in a <typeparamref name="TResult"/> (i.e., the result of the execution).</returns>
        new Task<TResult> ExecuteAsync();
    }
}