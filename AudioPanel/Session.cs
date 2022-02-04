using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;
using Mxr = Mixer.Mixer;
namespace Mixer.Controller
{
   public class Session : IComparable
    {

        [DllImport("kernel32.dll")]
        static extern bool GetExitCodeProcess(IntPtr hProcess, out uint lpExitCode);

        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool CloseHandle(IntPtr handle);

        [Flags]
        public enum ProcessAccessFlags : uint
        {
            PROCESS_QUERY_LIMITED_INFORMATION = 0x00001000,
        }

        public int sessionId { get; set; }
        public UInt32 processId { get; set; }
        public string sessionName { get; set; }
        public Process process { get; set; }

        public Session(int i, UInt32 u)
        {
            this.sessionId = i;
            this.processId = Mxr.getSessionPID(this.sessionId);
            this.sessionName = Marshal.PtrToStringAnsi( Mxr.getSessionName(this.sessionId) );
            this.process = Process.GetProcessById((int)this.processId);
            try
            {
                Console.WriteLine(this.process.ProcessName);
            }
            catch (Exception _)
            {

            }
            //this.process.EnableRaisingEvents = true;
            //this.process.Exited += (sender, e) =>
            //{
            //    Console.WriteLine("Process exited");
            //    SessionManagerSingleton.Instance.GetSession().checkSessions();
            //};
            // this.waitForDeath();
        }
        public bool  isProcessDead()
        {
           IntPtr hProcess = OpenProcess(ProcessAccessFlags.PROCESS_QUERY_LIMITED_INFORMATION, false, (int)this.processId);
            uint exitCode;
          
                GetExitCodeProcess(hProcess, out exitCode);

           // Console.Write("Process Status: {0},{1} \n", this.sessionName, exitCode);
         
            if (exitCode != 259) // ExitCode of 259 is STILL_ACTIVE
            {
                Console.WriteLine("Process died!!!!! {0}, {1}", this.sessionName, this.sessionId);
                CloseHandle(hProcess);
                return true;
            }

            // do your thing, this process is alive and you have a handle now


            CloseHandle(hProcess);
           
            //SessionManagerSingleton.Instance.GetSession().checkSessions();
            return false;
        }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;
            var other = obj as Session;
            return sessionId.CompareTo(other.sessionId);
        }

     
    }
}
