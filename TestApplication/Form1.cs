using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TestApplication
{
    public partial class Form1 : Form
    {

        //private delegate void SetTextBoxText(string text);
        private delegate void SetTextBoxTextInvoker(string text); 

        KeyboardListener.Listener listener = new KeyboardListener.Listener();

        public Form1()
        {
            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        
        {
            lstKeys.Items.Clear();
            lstKeys.Items.AddRange(Enum.GetNames(typeof(KeyboardListener.Keycode)));
            SelectedKeys.Checked = true;

            listener.KeyPressed += new KeyboardListener.Listener.KeypressedEventHandler(listener_KeyPressed);
            listener.Listen();
        }

        void listener_KeyPressed(KeyboardListener.Keycode oKeycodes)
        {
            SetTextBoxText(oKeycodes.ToString() + " pressed");
        }



        private void SetTextBoxText(string text)
        {
            if (textBox1.InvokeRequired)
            {
                textBox1.Invoke(new SetTextBoxTextInvoker(SetTextBoxText), text);
            }
            else
            {
                textBox1.Text += text + Environment.NewLine;
                textBox1.SelectionStart = textBox1.TextLength;
                textBox1.ScrollToCaret();
            }

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            listener.StopListening();
        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void lstKeys_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateListener();
        }

        void UpdateListener()
        {
            listener.ClearKeycodes();
            if (AllKeys.Checked)
            {
                listener.AllKeys = true;
            }
            else
            {
                listener.AllKeys = false;
                for (int i = 0; i < lstKeys.CheckedItems.Count; i++)
                {
                    listener.AddKeycode((KeyboardListener.Keycode)Enum.Parse(typeof(KeyboardListener.Keycode), lstKeys.CheckedItems[i].ToString()));
                }
            }
        }

        private void AllKeys_CheckedChanged(object sender, EventArgs e)
        {
            UpdateListener();
        }

    }
}
