using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Mixer.Controller
{
    public class SessionViewModel : INotifyPropertyChanged
    {
        private double sessionVolume;
        private string sessionName;
        public double SessionVolume { get { return sessionVolume; } set { sessionVolume = value; OnPropertyChanged("SessionVolume"); } }
        public string SessionName { get { return sessionName; } set { sessionName = value; OnPropertyChanged("SessionName"); } }
        public SessionViewModel()
        {
           // SessionVolume = 0.5;
            //SessionName = "Chrome";
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(String info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
