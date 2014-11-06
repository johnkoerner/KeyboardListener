using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Diagnostics;
namespace KeyboardListener
{

    /// <summary>
    /// <example>This example shows how to use the <see cref="KeyboardListener.Listener" /> class.
    /// <code>
    ///private void Form1_Load(object sender, EventArgs e)
    ///
    ///{
    ///    lstKeys.Items.Clear();
    ///    lstKeys.Items.AddRange(Enum.GetNames(typeof(KeyboardListener.Keycode)));
    ///    SelectedKeys.Checked = true;
    ///
    ///    // Listen for the A key to be pressed
    ///    oLis.AddKeycode(KeyboardListener.Keycode.VK_A);
    ///    oLis.KeyPressed += new KeyboardListener.Listener.KeypressedEventHandler(oLis_KeyPressed);
    ///    oLis.Listen();
    ///}
    ///
    ///void oLis_KeyPressed(KeyboardListener.Keycode keycodes)
    ///{
    ///    SetTextBoxText(keycodes.ToString() + " pressed");
    ///}
    ///
    ///
    ///
    ///private void SetTextBoxText(string text)
    ///{
    ///    if (textBox1.InvokeRequired)
    ///    {
    ///        textBox1.Invoke(new SetTextBoxTextInvoker(SetTextBoxText), text);
    ///    }
    ///    else
    ///    {
    ///        textBox1.Text += text + Environment.NewLine;
    ///        textBox1.SelectionStart = textBox1.TextLength;
    ///        textBox1.ScrollToCaret();
    ///    }
    ///
    ///}
    /// </code>
    /// </example>
    /// </summary>
    public class Listener : IDisposable
    {

        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(int vKey);


        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags,
           int dwExtraInfo);

 

        private const int KEYEVENTF_KEYUP = 0x02;
        private const int KEYDOWN = -32767;
        private readonly List<Keycode> watchCodes;
        private readonly BackgroundWorker mainBW;

        /// <summary>
        /// Delegate for the event that is raised whenever a keypress that is being watched occurs.
        /// </summary>
        /// <param name="keycodes">Keycode that fired the event</param>
        public delegate void KeypressedEventHandler(Keycode keycodes);      

        /// <summary>
        /// Event that is raised whenever a keypress that is being watched occurs.
        /// </summary>
        public event KeypressedEventHandler KeyPressed;

        /// <summary>
        /// Listen for all keypresses
        /// </summary>
        public bool AllKeys = false;
           
        /// <summary>
        /// Adds a keycode that the listener should listen for.
        /// </summary>
        /// <param name="keycode">Keycode to be added to the watch list.</param>
        public void AddKeycode(Keycode keycode)
        {
            watchCodes.Add(keycode);
        }

        /// <summary>
        /// Removes a single keycode from the list of keycodes to listen for.
        /// </summary>
        /// <param name="keycode">Keycode to be removed from the watch list.</param>
        public void RemoveKeycode(Keycode keycode)
        {
            watchCodes.Remove(keycode);
        }

        /// <summary>
        /// Clears all keycodes that the listener is currently looking for.
        /// </summary>
        public void ClearKeycodes()
        {
            watchCodes.Clear();
        }

        /// <summary>
        /// Start listening for the selected keys
        /// </summary>
        public void Listen()
        {
            mainBW.RunWorkerAsync();
        }

        public Listener()
        {
            mainBW = new BackgroundWorker();
            mainBW.DoWork += new DoWorkEventHandler(mainBW_DoWork);
            mainBW.WorkerSupportsCancellation = true;
            watchCodes = new List<Keycode>();
        }

        private void mainBW_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                if (AllKeys)
                {
                    for (int i = 0; i < 255; i++)
                    {
                        short ret = GetAsyncKeyState(i);
                        if (ret == KEYDOWN)
                        {
                            this.KeyPressed((Keycode)i);
                        }
                    }
                }
                else if (watchCodes.Count >0)
                {
                    foreach (int iKey in watchCodes)
                    {

                        short ret = GetAsyncKeyState(iKey);
                        if (ret != 0)
                        {
                            keybd_event((byte)iKey, 0x45, KEYEVENTF_KEYUP, 0);
                            this.KeyPressed((Keycode)iKey);
                        }

                    }
                }

                if (mainBW.CancellationPending) break;
                System.Threading.Thread.Sleep(50);
            }
        }


        /// <summary>
        /// Stop listening for keystrokes
        /// </summary>
        public void StopListening()
        {
            mainBW.CancelAsync();
        }


        public void Dispose()
        {
            if (mainBW.IsBusy)
                mainBW.CancelAsync();
        }
    }
}
