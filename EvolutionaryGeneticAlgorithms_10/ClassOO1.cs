using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    internal class ClassOO
    {
        public int x;
        public int y;
        public ClassOO(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public static List<List<ClassOO>> Generation_List(List<List<int>> processingTimes)
        {
            List<List<ClassOO>> pen = new List<List<ClassOO>>();
            for (int i = 0; i < processingTimes.Count; i++)
            {
                List<ClassOO> yy = new List<ClassOO>();
                for (int j = 0; j < processingTimes[0].Count; j++)
                {
                    ClassOO x_y = new ClassOO(0, 0);
                    yy.Add(x_y);
                }
                pen.Add(yy);
            }
            return pen;
        }
    }
}