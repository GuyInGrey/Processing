using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Processing
{
    public class CanvasFormUI
    {
        public CanvasForm Form;
        public bool Running { get; internal set; } = true;
        public event EventHandler<PKeyEventArgs> KeyDown;
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