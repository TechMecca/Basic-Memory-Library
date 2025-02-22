using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Memory.Internal
{
    /// <summary>
    /// Provides low-level memory manipulation functionalities.
    /// </summary>
    internal static unsafe class MemoryManager
    {
        /// <summary>
        /// Writes a value of type T to the specified memory address.
        /// </summary>
        /// <typeparam name="T">The type of the value to write.</typeparam>
        /// <param name="address">The memory address to write to.</param>
        /// <param name="value">The value to write.</param>
        [HandleProcessCorruptedStateExceptions]
        internal static void Write<T>(IntPtr address, T value) => Marshal.StructureToPtr(value, address, false);

        /// <summary>
        /// Reads a value of type T from the specified memory address.
        /// </summary>
        /// <typeparam name="T">The type of the value to read.</typeparam>
        /// <param name="address">The memory address to read from.</param>
        /// <returns>The value read from memory.</returns>
        [HandleProcessCorruptedStateExceptions]
        internal static T Read<T>(IntPtr address)
        {
            try
            {
                return *(T*)address;
            }
            catch (AccessViolationException)
            {
                Console.WriteLine($"Access Violation on {address} with type {typeof(T).Name}");
                return default;
            }
        }

        /// <summary>
        /// Reads a null-terminated ASCII string from the specified memory address.
        /// </summary>
        /// <param name="address">The memory address to read from.</param>
        /// <returns>The string read from memory.</returns>
        [HandleProcessCorruptedStateExceptions]
        internal static string ReadString(IntPtr address)
        {
            var buffer = ReadBytes(address, 512);
            if (buffer.Length == 0)
                return default;

            var ret = Encoding.ASCII.GetString(buffer);
            return ret.Contains('\0') ? ret.Substring(0, ret.IndexOf('\0')) : ret;
        }

        /// <summary>
        /// Reads a specified number of bytes from memory.
        /// </summary>
        /// <param name="address">The memory address to read from.</param>
        /// <param name="count">The number of bytes to read.</param>
        /// <returns>An array containing the bytes read.</returns>
        [HandleProcessCorruptedStateExceptions]
        internal static byte[] ReadBytes(IntPtr address, int count)
        {
            try
            {
                var ret = new byte[count];
                var ptr = (byte*)address;

                for (var i = 0; i < count; i++)
                    ret[i] = ptr[i];

                return ret;
            }
            catch (AccessViolationException)
            {
                Console.WriteLine($"Access Violation on {address} with type Byte[]");
                return default;
            }
        }

        /// <summary>
        /// Writes an array of bytes to the specified memory address.
        /// </summary>
        /// <param name="address">The memory address to write to.</param>
        /// <param name="bytes">The byte array to write.</param>
        /// <returns>The number of bytes written.</returns>
        internal static int WriteBytes(IntPtr address, byte[] bytes)
        {
            if (address == IntPtr.Zero)
                return 0;

            var access = Imports.ProcessAccessFlags.PROCESS_CREATE_THREAD |
                         Imports.ProcessAccessFlags.PROCESS_QUERY_INFORMATION |
                         Imports.ProcessAccessFlags.PROCESS_SET_INFORMATION |
                         Imports.ProcessAccessFlags.PROCESS_TERMINATE |
                         Imports.ProcessAccessFlags.PROCESS_VM_OPERATION |
                         Imports.ProcessAccessFlags.PROCESS_VM_READ |
                         Imports.ProcessAccessFlags.PROCESS_VM_WRITE |
                         Imports.ProcessAccessFlags.SYNCHRONIZE;

            var process = Imports.OpenProcess(access, false, Process.GetCurrentProcess().Id);

            int ret = 0;
            Imports.WriteProcessMemory(process, address, bytes, bytes.Length, ref ret);

            var protection = Imports.Protection.PAGE_EXECUTE_READWRITE;
            Imports.VirtualProtect(address, bytes.Length, (uint)protection, out uint _);
            return ret;
        }
    }
}
