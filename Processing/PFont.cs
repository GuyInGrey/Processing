using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Processing
{
    public class PFont
    {
        internal Font Font;

        public string Name => Font.Name;
        public float Size => Font.Size;
    }
}