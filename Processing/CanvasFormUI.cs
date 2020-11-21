using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Processing
{
    public class CanvasFormUI
    {
        /// <summary>
        /// The System.Forms.Form used to display the window. Only touch if you know what you're doing.
        /// </summary>
        public CanvasForm Form;
        /// <summary>
        /// Whether the application is running or not.
        /// </summary>
        public bool Running { get; internal set; } = true;
        /// <summary>
        /// Ran the moment a key is pressed down.
        /// </summary>
        public event EventHandler<PKeyEventArgs> KeyDown;
        /// <summary>
        /// Ran the moment a key is lifted.
        /// </summary>
        public event EventHandler<PKeyEventArgs> KeyUp;
        internal List<(string, Action<bool>)> KeyActions;

        internal void Initialize(int width, int height)
        {
            KeyActions = new List<(string, Action<bool>)>();
            Form = new CanvasForm();
            Form.SetSize(width, height);

            Form.FormClosing += Form_FormClosing;
            Form.KeyDown += Form_KeyDown;
            Form.KeyUp += Form_KeyUp;
        }

        private void Form_KeyUp(object sender, KeyEventArgs e)
        {
            KeyUp?.Invoke(this, new PKeyEventArgs() { Key = e.KeyCode.ToString(), BaseKey = e.KeyCode });
            KeyActions.ForEach(a =>
            {
                if (a.Item1 == e.KeyCode.ToString())
                {
                    a.Item2?.Invoke(false);
                }
            });
        }

        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            KeyDown?.Invoke(this, new PKeyEventArgs() { Key = e.KeyCode.ToString(), BaseKey = e.KeyCode });
            KeyActions.ForEach(a =>
            {
                if (a.Item1 == e.KeyCode.ToString())
                {
                    a.Item2?.Invoke(true);
                }
            });
        }

        /// <summary>
        /// An easy way to trigger an action when a key is pressed or lifted.
        /// </summary>
        /// <param name="key">The key to check for.</param>
        /// <param name="action">The action to run.</param>
        /// <returns></returns>
        public bool AddKeyAction(string key, Action<bool> action)
        {
            if (Enum.TryParse<Keys>(key, out _))
            {
                KeyActions.Add((key, action));
                return true;
            }
            else
            {
                return false;
            }
        }

        internal void BeginForm()
        {
            Application.EnableVisualStyles();
            Application.Run(Form);
        }

        /// <summary>
        /// Exit the application.
        /// </summary>
        public void Close()
        {
            Form.Invoke(new Action(() => { Form.Close(); }));
        }

        private void Form_FormClosing(object s, FormClosingEventArgs e) { Running = false; }
    }

    public class PKeyEventArgs : EventArgs
    {
        public string Key;
        public Keys BaseKey;
    }
}