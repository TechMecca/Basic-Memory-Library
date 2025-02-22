using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Memory.Internal
{
    /// <summary>
    /// Represents a function patch that allows modifying memory at a specific address.
    /// </summary>
    public class Patch
    {
        private readonly IntPtr _address;
        private readonly byte[] _originalBytes;
        private readonly byte[] _patchBytes;

        /// <summary>
        /// Initializes a new instance of the <see cref="Patch"/> class.
        /// </summary>
        /// <param name="address">The memory address to patch.</param>
        /// <param name="patchWith">The bytes to write.</param>
        /// <param name="name">The name of the patch.</param>
        public Patch(IntPtr address, byte[] patchWith, string name)
        {
            Name = name;
            _address = address;
            _patchBytes = patchWith;
            _originalBytes = MemoryManager.ReadBytes(address, patchWith.Length);
        }

        /// <summary>
        /// Gets or sets a value indicating whether this patch is enabled.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets the name of this patch.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Applies this patch by writing the patch bytes to memory.
        /// </summary>
        /// <returns>True if successful, otherwise false.</returns>
        public bool Apply()
        {
            try
            {
                MemoryManager.WriteBytes(_address, _patchBytes);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Removes this patch by restoring the original bytes.
        /// </summary>
        /// <returns>True if successful, otherwise false.</returns>
        public bool Remove()
        {
            try
            {
                MemoryManager.WriteBytes(_address, _originalBytes);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this patch is currently applied.
        /// </summary>
        public bool IsApplied => MemoryManager.ReadBytes(_address, _patchBytes.Length).SequenceEqual(_patchBytes);

        /// <summary>
        /// Destructor to ensure the patch is removed before garbage collection.
        /// </summary>
        ~Patch()
        {
            if (IsApplied)
                Remove();

            GC.SuppressFinalize(this);
        }
    }
}
