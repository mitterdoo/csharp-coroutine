using System;
using System.Collections.Generic;

namespace mitterdoo.Coroutine
{
	/// <summary>
	///	A generic exception thrown by an executing coroutine.
	/// </summary>
	class CoroutineException : Exception
	{
		public CoroutineException()
		{ }

		public CoroutineException(string msg) : base(msg)
		{ }

		public CoroutineException(string msg, Exception inner) : base(msg, inner)
		{ }
	}

	/// <summary>
	/// Used for passing any type into the resume point for a running coroutine.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	class Getter<T>
	{
		public T Value;
		public Getter(T input)
		{
			Value = input;
		}
	}


	class Coroutine<TIn, TOut>
	{
		/// <summary>
		/// The required setup for a coroutine function.
		/// </summary>
		/// <param name="input">A reference to the value passed by the invoking <see cref="Resume(TIn)"/>. Use <c>input.Value</c>.</param>
		/// <returns></returns>
		public delegate IEnumerable<TOut> coroutineDelegate(Getter<TIn> input);
		private Getter<TIn> inputData; // When resuming this coroutine, this is the data that will be fed into the function
		private IEnumerator<TOut> enumerator;

		/// <summary>
		/// Whether the wrapped coroutine can be resumed
		/// </summary>
		public bool Alive
		{
			get;
			private set;
		}

		/// <summary>
		/// Creates a wrapper for a <see cref="IEnumerable{T}" /> function styled as a coroutine.
		/// Note: This does not immediately start the coroutine. It must be ran with <see cref="Resume(TIn)"/>.
		/// </summary>
		/// <param name="enumerable">The coroutine function to wrap this class around.</param>
		public Coroutine(coroutineDelegate enumerable)
		{
			inputData = new Getter<TIn>(default(TIn));
			enumerator = enumerable(inputData).GetEnumerator();
			Alive = true;
		}

		/// <summary>
		/// Begins or resumes the wrapped coroutine function.
		/// </summary>
		/// <param name="input">A value to pass into the resumed coroutine.</param>
		/// <returns>The value of the last <c>yield return</c> executed in the coroutine.</returns>
		/// <exception cref="CoroutineException">Thrown when the <see cref="Alive"/> is false</exception>
		public TOut Resume(TIn input)
		{
			if (!Alive)
			{
				throw new CoroutineException("Cannot resume dead coroutine!");
			}
			inputData.Value = input;
			Alive = enumerator.MoveNext();
			return enumerator.Current;
		}
	}
}