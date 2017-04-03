using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace WindowsFormsApplication11
{
    class SharedData
    {
        //public static List<List<int>> matrix ;//= new List<List<int>>();
        public static int[,] matrix = new int[10,10];
        public static int n = 0;
        public static Label[,] labels = new Label[10, 10];
    }
}
