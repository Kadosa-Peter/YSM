﻿using System;
using System.Diagnostics.CodeAnalysis;

namespace Ysm.Core.Helpers
{
    /// <summary>
    /// Stores an Func without causing a hard reference
    /// to be created to the Func's owner. The owner can be garbage collected at any time.
    /// </summary>
    /// <typeparam name="T">The type of the Func's parameter.</typeparam>
    /// <typeparam name="TResult">The type of the Func's return value.</typeparam>
    public class WeakFunc<T, TResult> : WeakFunc<TResult>, IExecuteWithObjectAndResult
    {

        private Func<T, TResult> _staticFunc;

        /// <summary>
        /// Gets or sets the name of the method that this WeakFunc represents.
        /// </summary>
        public override string MethodName
        {
            get
            {
                if (_staticFunc != null)
                {

                    return _staticFunc.Method.Name;
                }


                return Method.Name;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the Func's owner is still alive, or if it was collected
        /// by the Garbage Collector already.
        /// </summary>
        public override bool IsAlive
        {
            get
            {
                if (_staticFunc == null
                    && Reference == null)
                {
                    return false;
                }

                if (_staticFunc != null)
                {
                    if (Reference != null)
                    {
                        return Reference.IsAlive;
                    }

                    return true;
                }

                return Reference.IsAlive;
            }
        }

        /// <summary>
        /// Initializes a new instance of the WeakFunc class.
        /// </summary>
        /// <param name="func">The Func that will be associated to this instance.</param>
        public WeakFunc(Func<T, TResult> func)
            : this(func?.Target, func)
        {
        }

        /// <summary>
        /// Initializes a new instance of the WeakFunc class.
        /// </summary>
        /// <param name="target">The Func's owner.</param>
        /// <param name="func">The Func that will be associated to this instance.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods",MessageId = "1",Justification = "Action should fail with an exception if func is null.")]
        public WeakFunc(object target, Func<T, TResult> func)
        {

            if (func.Method.IsStatic)
            {
                _staticFunc = func;

                if (target != null)
                {
                    // Keep a reference to the target to control the
                    // WeakAction's lifetime.
                    Reference = new WeakReference(target);
                }

                return;
            }

            Method = func.Method;
            FuncReference = new WeakReference(func.Target);

            Reference = new WeakReference(target);
        }

        /// <summary>
        /// Executes the Func. This only happens if the Func's owner
        /// is still alive. The Func's parameter is set to default(T).
        /// </summary>
        /// <returns>The result of the Func stored as reference.</returns>
        public new TResult Execute()
        {
            return Execute(default(T));
        }

        /// <summary>
        /// Executes the Func. This only happens if the Func's owner
        /// is still alive.
        /// </summary>
        /// <param name="parameter">A parameter to be passed to the action.</param>
        /// <returns>The result of the Func stored as reference.</returns>
        public TResult Execute(T parameter)
        {
            if (_staticFunc != null)
            {
                return _staticFunc(parameter);
            }

            object funcTarget = FuncTarget;
            
            if (IsAlive)
            {
                if (Method != null
                    && FuncReference != null
                    && funcTarget != null)
                {
                    return (TResult) Method.Invoke(
                        funcTarget,
                        new object[]
                        {
                            parameter
                        });
                }


            }

            return default(TResult);
        }

        /// <summary>
        /// Executes the Func with a parameter of type object. This parameter
        /// will be casted to T. This method implements <see cref="IExecuteWithObject.ExecuteWithObject" />
        /// and can be useful if you store multiple WeakFunc{T} instances but don't know in advance
        /// what type T represents.
        /// </summary>
        /// <param name="parameter">The parameter that will be passed to the Func after
        /// being casted to T.</param>
        /// <returns>The result of the execution as object, to be casted to T.</returns>
        public object ExecuteWithObject(object parameter)
        {
            T parameterCasted = (T)parameter;
            return Execute(parameterCasted);
        }

        /// <summary>
        /// Sets all the funcs that this WeakFunc contains to null,
        /// which is a signal for containing objects that this WeakFunc
        /// should be deleted.
        /// </summary>
        public new void MarkForDeletion()
        {
            _staticFunc = null;
            base.MarkForDeletion();
        }
    }
}