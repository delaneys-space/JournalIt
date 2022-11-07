using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JournalIt
{
    public overload class System.Windows.Point
    {
        // Specify which operator to overload (+),  
        // the types that can be added (two System.Windows.Point objects), 
        // and the return type (System.Windows.Point). 
        public static System.Windows.Point operator +(System.Windows.Point p1, System.Windows.Point p2)
        {
            System.Windows.Point point = new System.Windows.Point(p1.X + p2.X,
                                                                  p1.Y + p2.Y);
            return point;
        }

        // Specify which operator to overload (-),  
        // the types that can be added (two System.Windows.Point objects), 
        // and the return type (System.Windows.Point). 
        public static System.Windows.Point operator -(System.Windows.Point p1, System.Windows.Point p2)
        {
            System.Windows.Point point = new System.Windows.Point(p1.X - p2.X,
                                                                  p1.Y - p2.Y);
            return point;
        }
    }
}
