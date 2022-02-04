using Mixer.Controller;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Mxr = Mixer.Mixer;
namespace AudioPanel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        System.Timers.Timer timer = new System.Timers.Timer();
        public MainWindow()
        {
            new Thread(() => { Process.Start("HIDMonitor.exe"); }).Start();
            InitializeComponent();
            this.Left = SystemParameters.WorkArea.Right * 0.75 - this.Width;
            
            timer.Elapsed += Timer_Elapsed;
            timer.Interval = 3500;
            timer.AutoReset = false;
            timer.Enabled = true;
            timer.Start();
             new Thread(() =>
           {
               var _sing = SessionManagerSingleton.Instance;
               //  _sing.GetSession();
               var thing = this.VolumeProgressBar;
               // this.Grid.DataContext = new SessionViewModel();

               var sessionmodel = new SessionViewModel();

               var server = new NamedPipeServerStream("HIDPipe");
               server.WaitForConnection();
               StreamReader streamReader = new StreamReader(server);
               StreamWriter streamWriter = new StreamWriter(server);

               while (true)
               {
                   var line = streamReader.ReadLine();
                   if (line == null)
                       break;
                   if(line.Equals("C: F"))
                   {
                       this.timer.Stop();
                       this.timer.Start();
                       this.mainwindow.Dispatcher.Invoke(new Action(() => { this.mainwindow.Show(); }));
                       Console.WriteLine("Cycle Forward");
                       _sing.CycleForwardSessions();
                       this.Grid.Dispatcher.Invoke(new Action(() => 
                       {
                           this.Topmost = true;
                           this.Activate();
                           this.Grid.DataContext = sessionmodel;
                           sessionmodel.SessionName =  _sing.GetSelectedSession().sessionName.ToUpperInvariant();
                           sessionmodel.SessionVolume = (double)Mxr.getSessionVolume(_sing.GetSelectedSession().sessionId);
                           this.Grid.DataContext = sessionmodel;
                       }
                       ));
                   }

                   if (line.Equals("C: B"))
                   {
                       this.timer.Stop();
                       this.timer.Start();
                       this.mainwindow.Dispatcher.Invoke(new Action(() => { this.mainwindow.Show(); }));
                       Console.WriteLine("VOLUME Down");
                       _sing.CycleBackwardSessions();
                       this.Grid.Dispatcher.Invoke(new Action(() =>
                       {
                           this.Topmost = true;
                           this.Activate();
                           this.Grid.DataContext = sessionmodel;
                           sessionmodel.SessionName = _sing.GetSelectedSession().sessionName.ToUpperInvariant();
                           sessionmodel.SessionVolume = (double)Mxr.getSessionVolume(_sing.GetSelectedSession().sessionId);
                           this.Grid.DataContext = sessionmodel;
                       }
                       ));
                   }

                   if (line.Equals("V: U"))
                   {
                       this.timer.Stop();
                       this.timer.Start();
                       this.mainwindow.Dispatcher.Invoke(new Action(() => { this.mainwindow.Show(); }));
                       Console.WriteLine("Cycle Forward");
                       //_sing.CycleForwardSessions();
                       Mxr.setSessionVolume(_sing.GetSelectedSession().sessionId, ((float)Mxr.getSessionVolume(_sing.GetSelectedSession().sessionId) + 0.01F));
                       this.Grid.Dispatcher.Invoke(new Action(() =>
                       {
                           this.Topmost = true;
                           this.Activate();
                           this.Grid.DataContext = sessionmodel;
                           sessionmodel.SessionName = _sing.GetSelectedSession().sessionName.ToUpperInvariant();
                           sessionmodel.SessionVolume = (double)Mxr.getSessionVolume(_sing.GetSelectedSession().sessionId);
                           this.Grid.DataContext = sessionmodel;
                       }
                       ));
                   }

                   if (line.Equals("V: D"))
                   {
                       this.timer.Stop();
                       this.timer.Start();
                       this.mainwindow.Dispatcher.Invoke(new Action(() => { this.mainwindow.Show(); }));
                       Console.WriteLine("Cycle Forward");
                       //_sing.CycleForwardSessions();
                       Mxr.setSessionVolume(_sing.GetSelectedSession().sessionId, ((float)Mxr.getSessionVolume(_sing.GetSelectedSession().sessionId) - 0.01F));
                       this.Grid.Dispatcher.Invoke(new Action(() =>
                       {
                           this.Topmost = true;
                           this.Activate();
                           this.Grid.DataContext = sessionmodel;
                           sessionmodel.SessionName = _sing.GetSelectedSession().sessionName.ToUpperInvariant();
                           sessionmodel.SessionVolume = (double)Mxr.getSessionVolume(_sing.GetSelectedSession().sessionId);
                           this.Grid.DataContext = sessionmodel;
                       }
                       ));
                   }



               }



               //while (true)
               //{
               //    this.Grid.Dispatcher.Invoke(new Action(() =>
               //           {
               //               var sess = new SessionViewModel();
               //           //this.DataContext = sess;
               //           this.Grid.DataContext = sess;

               //               foreach (var item in _sing.GetSession().sessionsDictionary)
               //               {
               //               //thing.Items.Add(new ListViewItem().DataContext = item);
               //               //thing.Value = 0.9;

               //               // var x = this.Resources["sessionDataSource"] as SessionViewModel;
               //               if (item.Value.sessionId == 3)
               //                   {
               //                       sess.SessionName = item.Value.sessionName;
               //                       sess.SessionVolume = Mixer.Mixer.getSessionVolume(item.Value.sessionId);
               //                   }

               //               }


               //           }));
               //}
           }).Start();
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {

            this.mainwindow.Dispatcher.Invoke(new Action( () => { this.mainwindow.Hide(); }) );
            this.timer.Stop();
        }
    }
}
