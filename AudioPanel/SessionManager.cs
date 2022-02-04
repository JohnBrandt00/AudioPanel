using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Mxr = Mixer.Mixer;
namespace Mixer.Controller
{
    public class SessionManager
    {
        public int sessionCount { get; set; }
        public Session currentSession { get; set; }

        internal void Init()
        {
            Console.WriteLine("session manager init");
            this.checkSessions();
            Thread thread = new Thread(new ThreadStart(CheckProcessStatus));
            thread.Start();
           
        }

        private void CheckProcessStatus()
        {
            while (true)
            {
               //lock (this.sessionsDictionary)
              //  {
                    
                    foreach (var item in this.sessionsDictionary.ToList())
                    {
                    if (item.Value.sessionName =="Idle")
                    {

                        continue;
                    }
                    else
                    {

                        var thing = item.Value;
                        if (thing.isProcessDead())
                        {
                            Console.WriteLine("The process has died {0}, {1}", thing.processId, thing.sessionName);
                            this.checkSessions();
                        }
                    }
                 //   }
                }
                if (this.sessionCount != Mxr.getSessionCount())
                {
                    Console.WriteLine("detecting a sessionCount mismatch");
                    this.checkSessions();
                }
            }

            
            
        }

        public Dictionary<int, Session> sessionsDictionary = new Dictionary<int, Session>();
        private bool isDirty { get; set; }

        public void markDirty()
        {
            this.isDirty = true;
        }


        public void checkDirty()
        {
            if(this.isDirty)
            {
                this.rebuildDictionary();
            }
        }

        public void rebuildDictionary()
        {
            sessionsDictionary.Clear();
            for (int i = 0; i < Mxr.getSessionCount(); i++)
            {
                Console.WriteLine("Adding session to dictionary: {0} {1}", i,Marshal.PtrToStringAnsi( Mxr.getSessionName(i) ));
                sessionsDictionary.Add(i, new Session(i,Mxr.getSessionPID(i)));
            }
            this.sessionCount = Mxr.getSessionCount();
        }

        public void checkSessions()
        {
            int sessionCount = Mxr.getSessionCount();
            //if(this.sessionCount != sessionCount)
            //{
            //    Console.WriteLine("Session count mismatch");
            //    this.markDirty();
            //}
            for (int i = 0; i < sessionCount; i++)
            {
                try
                {
                    if (Process.GetProcessById((int)Mxr.getSessionPID(i)) == null)
                    {
                        Console.WriteLine("Process PID no longer exists");
                        this.markDirty();
                        break;
                    }
                    if (sessionsDictionary.Count != sessionCount)
                    {
                        Console.WriteLine("Dictionary does not match session count");
                        this.markDirty();
                        break;
                    }
                    if (!sessionsDictionary.ContainsKey(i))
                    {

                        Console.WriteLine("Dictionary does not contain key! {0} {1}");
                        this.markDirty();
                        break;
                    }

                    var sesh = sessionsDictionary.ElementAt(i);
                    
                    if(sesh.Value.processId != Mxr.getSessionPID(i) || sesh.Value.sessionId!= i )
                    {
                        Console.WriteLine("The process or session id mismatch");
                        this.markDirty();
                        break;
                    }

                }catch(Exception e)
                {
                    Console.WriteLine("Error: {0}", e);
                }
            }
            this.checkDirty();
        }
    
    }


}
