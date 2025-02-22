using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Memory.Internal
{
    internal static class Imports
    {

        [Flags]
        internal enum ProcessAccessFlags
        {
            DELETE = 0x00010000,
            READ_CONTROL = 0x00020000,
            SYNCHRONIZE = 0x00100000,
            WRITE_DAC = 0x00040000,
            WRITE_OWNER = 0x00080000,
            PROCESS_ALL_ACCESS = 0x001F0FFF,
            PROCESS_CREATE_PROCESS = 0x0080,
            PROCESS_CREATE_THREAD = 0x0002,
            PROCESS_DUP_HANDLE = 0x0040,
            PROCESS_QUERY_INFORMATION = 0x0400,
            PROCESS_QUERY_LIMITED_INFORMATION = 0x1000,
            PROCESS_SET_INFORMATION = 0x0200,
            PROCESS_SET_QUOTA = 0x0100,
            PROCESS_SUSPEND_RESUME = 0x0800,
            PROCESS_TERMINATE = 0x0001,
            PROCESS_VM_OPERATION = 0x0008,
            PROCESS_VM_READ = 0x0010,
            PROCESS_VM_WRITE = 0x0020
        }


        [DllImport("kernel32.dll")]
        internal static extern bool VirtualProtect(IntPtr address, int size, uint newProtect, out uint oldProtect);

        [DllImport("kernel32.dll")]
        internal static extern IntPtr OpenProcess(ProcessAccessFlags desiredAccess, bool inheritHandle, int processId);

        [DllImport("kernel32.dll")]
        internal static extern bool WriteProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            byte[] lpBuffer,
            int dwSize,
            ref int lpNumberOfBytesWritten);

        [Flags]
        public enum Protection
        {
            PAGE_NOACCESS = 0x01,
            PAGE_READONLY = 0x02,
            PAGE_READWRITE = 0x04,
            PAGE_WRITECOPY = 0x08,
            PAGE_EXECUTE = 0x10,
            PAGE_EXECUTE_READ = 0x20,
            PAGE_EXECUTE_READWRITE = 0x40,
            PAGE_EXECUTE_WRITECOPY = 0x80,
            PAGE_GUARD = 0x100,
            PAGE_NOCACHE = 0x200,
            PAGE_WRITECOMBINE = 0x400
        }

        [DllImport("kernel32.dll")]
        static extern bool VirtualProtect(IntPtr lpAddress, UIntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);
    }
}
