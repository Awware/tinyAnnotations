using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyAnnotations
{
    public class PositionThat
    {
        public int Position { get; set; }
        public int StartPosition { get; set; }
        public PositionThat()
        {
            Position = 0;
            StartPosition = 0;
        }
        public PositionThat(int start, int currentEnd)
        {
            Position = currentEnd;
            StartPosition = start;
        }
    }
}
