using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Memory.Internal
{
    /// <summary>
    /// Represents a function detour that allows redirecting function calls to a custom hook.
    /// </summary>
    public class Detour
    {
        private readonly IntPtr _hook;

        /// <summary>
        /// This variable ensures the delegate instance is not collected by the garbage collector.
        /// </summary>
        private readonly Delegate _hookDelegate;

        private readonly List<byte> _new;
        private readonly List<byte> _original;
        private readonly IntPtr _target;
        private readonly Delegate _targetDelegate;

        /// <summary>
        /// Initializes a new instance of the <see cref="Detour"/> class.
        /// </summary>
        /// <param name="target">The original function delegate.</param>
        /// <param name="hook">The hook function delegate.</param>
        /// <param name="name">The name of the detour.</param>
        internal Detour(Delegate target, Delegate hook, string name)
        {
            Name = name;
            _targetDelegate = target;
            _target = Marshal.GetFunctionPointerForDelegate(target);
            _hookDelegate = hook;
            _hook = Marshal.GetFunctionPointerForDelegate(hook);

            // Store the original bytes
            _original = new List<byte>(MemoryManager.ReadBytes(_target, 6));

            // Setup the detour bytes
            _new = new List<byte> { 0x68 };
            _new.AddRange(BitConverter.GetBytes(_hook.ToInt32()));
            _new.Add(0xC3);
        }

        /// <summary>
        /// Gets a value indicating whether this detour is currently applied.
        /// </summary>
        public bool IsApplied { get; private set; }

        /// <summary>
        /// Gets the name of this detour.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Applies this detour by writing new bytes to memory.
        /// </summary>
        /// <returns>True if successful, otherwise false.</returns>
        public bool Apply()
        {
            if (MemoryManager.WriteBytes(_target, _new.ToArray()) == _new.Count)
            {
                IsApplied = true;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Removes this detour by restoring the original bytes.
        /// </summary>
        /// <returns>True if successful, otherwise false.</returns>
        public bool Remove()
        {
            if (MemoryManager.WriteBytes(_target, _original.ToArray()) == _original.Count)
            {
                IsApplied = false;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Calls the original function without the detour applied.
        /// </summary>
        /// <param name="args">The arguments to pass to the original function.</param>
        /// <returns>The return value of the original function.</returns>
        public object CallOriginal(params object[] args)
        {
            Remove();
            object result = _targetDelegate.DynamicInvoke(args);
            Apply();
            return result;
        }

        /// <summary>
        /// Destructor to ensure detour removal before garbage collection.
        /// </summary>
        ~Detour()
        {
            if (IsApplied)
            {
                Remove();
            }
            GC.SuppressFinalize(this);
        }
    }
}
