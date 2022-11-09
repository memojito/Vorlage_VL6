using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

using SharpDX;
using SharpDX.XInput;

using System.Threading;
using System.Windows.Threading;

namespace Vorlage_VL6
{
    // delegate type declaration
    public delegate void UpdateGUIDelType(string[] values);

    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        //controller object
        private Controller XBox;

        //state
        private State myState;

        //thread
        private Thread ControllerThread;

        //delegate
        private UpdateGUIDelType UpdateGUIDel;

        public MainWindow()
        {
            InitializeComponent();

            XBox = new Controller(UserIndex.One);

            ControllerThread = new Thread(ControllerThreadFunction);

            UpdateGUIDel = new UpdateGUIDelType(UpdateGUI);
        }

        private void BT_XBox_Start_Click(object sender, RoutedEventArgs e)
        {
            ControllerThread.Start();
        }

        private void ControllerThreadFunction() {
            int leftTrigger;
            int AnalogRightY;

            string[] ControllerValues = { "", "", "" };

            bool run = true;

            try
            {

                while (run)
                {

                    myState = XBox.GetState();

                    leftTrigger = myState.Gamepad.LeftTrigger;
                    AnalogRightY = myState.Gamepad.RightThumbY;

                    if (myState.Gamepad.Buttons == GamepadButtonFlags.A)
                    {
                        ControllerValues[2] = "A gerdrueckt";
                    }
                    else
                    {
                        ControllerValues[2] = "A nicht gedrueckt";
                    }

                    ControllerValues[0] = leftTrigger.ToString();
                    ControllerValues[1] = AnalogRightY.ToString();

                    UpdateGUI(ControllerValues);

                    Thread.Sleep(20);

                }
            }
            catch (ThreadAbortException)
            {
                run = false;
            }
        }

        private void UpdateGUI(string[] values)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, UpdateGUIDel, values);
                return;
            }
            else
            {
                TB_LeftTrigger.Text = values[0];
                TB_AnalogRY.Text = values[1];
                TB_ButtonA.Text = values[2];
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ControllerThread.Abort();
        }
    }
    
}
