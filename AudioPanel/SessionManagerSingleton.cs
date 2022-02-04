using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mixer.Controller
{
    public sealed class SessionManagerSingleton
    {
        private SessionManager SessionManager = new SessionManager();
        private Session CurrentSession { get; set; }
        private int CurrentSessionId { get; set; }
        private SessionManagerSingleton() 
        {
            this.SessionManager.Init();
        }

        private static readonly Lazy<SessionManagerSingleton> lazy = new Lazy<SessionManagerSingleton>(() => new SessionManagerSingleton());
        public static SessionManagerSingleton Instance
        {
            get
            {
                return lazy.Value;
            }
        }


        public SessionManager GetSession()
        {
            return this.SessionManager;
        }
        public Session GetSelectedSession()
        {
            return this.CurrentSession;
        }

        public void SetSelectedSession(Session session)
        {

            this.CurrentSession = session;
            this.CurrentSessionId = session.sessionId;

        }

        public int GetCurrentSessionId()
        {
            return this.CurrentSessionId;
        }

        public void setCurrentSessionId(int i)
        {
            this.CurrentSessionId = i;
        }

        public void CycleForwardSessions()
        {
            var sessions = this.GetSession().sessionsDictionary.Where(w => w.Value.sessionId == CurrentSessionId).First();
            if(sessions.Value != null)
            {
                var next = this.GetSession().sessionsDictionary.FirstOrDefault(m => m.Value.sessionId > this.GetCurrentSessionId());
                if (next.Value == null)
                     next = this.GetSession().sessionsDictionary.First();
                   
              this.SetSelectedSession(next.Value);
                this.setCurrentSessionId(next.Value.sessionId);
                Console.WriteLine(this.GetSelectedSession().sessionName);
            }
            else
            {
               var x =  this.GetSession().sessionsDictionary.Values.FirstOrDefault();
                this.SetSelectedSession(x);
                this.setCurrentSessionId(x.sessionId);
            }
        }

        public void CycleBackwardSessions()
        {
            var sessions = this.GetSession().sessionsDictionary.Where(w => w.Value.sessionId == CurrentSessionId).First();
            if (sessions.Value != null)
            {
                var next = this.GetSession().sessionsDictionary.LastOrDefault(m => m.Value.sessionId < this.GetCurrentSessionId());
                if (next.Value == null)
                    next = this.GetSession().sessionsDictionary.Last();
                this.SetSelectedSession(next.Value);
                this.setCurrentSessionId(next.Value.sessionId);
                Console.WriteLine(this.GetSelectedSession().sessionName);
            }
            else
            {
                var x = this.GetSession().sessionsDictionary.Values.FirstOrDefault();
                this.SetSelectedSession(x);
                this.setCurrentSessionId(x.sessionId);
            }
        }

    }
}
