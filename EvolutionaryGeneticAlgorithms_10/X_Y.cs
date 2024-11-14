using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    internal class X_Y
    {
        public int x;
        public int y;
        public X_Y(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public static List<List<X_Y>> Generation_List(List<List<int>> processingTimes)
        {
            List<List<X_Y>> pen = new List<List<X_Y>>();
            for (int i = 0; i < processingTimes.Count; i++)
            {
                List<X_Y> yy = new List<X_Y>();
                for (int j = 0; j < processingTimes[0].Count; j++)
                {
                    X_Y x_y = new X_Y(0, 0);
                    yy.Add(x_y);
                }
                pen.Add(yy);
            }
            return pen;
        }
    }
}