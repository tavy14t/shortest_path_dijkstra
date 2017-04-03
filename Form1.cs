using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO;
namespace WindowsFormsApplication11
{   
    public partial class Form1 : Form
    {
        List<Point> last_points = new List<Point>();
        public System.Windows.Forms.Panel[,] panels;
        public int state = 1; // pt drawing
        public bool startPoint;
        public Point start;
        public Point end;
        public int[,] m = new int[28, 17];
        public bool endPoint;
        public int sleeptime;
        public Thread fromstart, fromend;
        public Boolean doubleThreaded = false;
        public int length;
        public Form2 f2 = new Form2();
        public System.Timers.Timer t = new System.Timers.Timer(500);
        Point poz_noua;
        int new_k = 0; bool SeFace = false;
        bool finish = false;
        static int[] diri = new int[] { -1, 1, 0, 0 };
        static int[] dirj = new int[] { 0, 0, 1, -1 };
        int Dij_start;
        int[] T = new int[10];

        public Form1()
        {
            InitializeComponent();
            panels = new System.Windows.Forms.Panel[35, 20];
            for (int i = 0; i < 28; i++)
                for (int j = 0; j < 17; j++)
                {
                    panels[i, j] = new System.Windows.Forms.Panel();
                    panels[i, j].Location = new System.Drawing.Point(i * 35 + 18, j * 35 + 23);
                    panels[i, j].BackColor = System.Drawing.Color.Green;
                    panels[i, j].Size = new System.Drawing.Size(34, 34);
                    panels[i, j].MouseClick += Form1_MouseClick;
                    groupBox1.Controls.Add(panels[i, j]);
                }
            trackBar1.Minimum = 10;
            trackBar1.Maximum = 120;
            trackBar1.Value = 65;
            sleeptime = 65;
            
            drawGraphics = drawPanel.CreateGraphics();
            f2.FormClosing += f2_Disposed;
            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                    SharedData.matrix[i, j] = 0;

            t.Elapsed += t_Elapsed;
        }
        void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            for (int i = 0; i < 28; i++)
            {
                for (int j = 0; j < 17; j++)
                {
                    if (sender == panels[i, j])
                    {
                        if (state == 1)
                        {
                            if (e.Button == MouseButtons.Left)
                                panels[i, j].BackColor = Color.Black;
                            else if (panels[i, j].BackColor == Color.Black)
                                panels[i, j].BackColor = Color.Green;
                        }
                        else if (state == 2)
                        {
                            if (e.Button == MouseButtons.Left && startPoint == false)
                            {
                                panels[i, j].BackColor = Color.Blue;
                                startPoint = true;
                                start.X = i;
                                start.Y = j;
                            }
                            else if (e.Button == MouseButtons.Right && panels[i, j].BackColor == Color.Blue)
                            {
                                panels[i, j].BackColor = Color.Green;
                                startPoint = false;
                            }
                        }
                        else if (state == 3)
                        {
                            if (e.Button == MouseButtons.Left && endPoint == false)
                            {
                                panels[i, j].BackColor = Color.Red;
                                endPoint = true;
                                end.X = i;
                                end.Y = j;
                            }
                            else if (e.Button == MouseButtons.Right && panels[i, j].BackColor == Color.Red)
                            {
                                panels[i, j].BackColor = Color.Green;
                                endPoint = false;
                            }
                        }

                    }
                }
            }
            if (endPoint == true && startPoint == true) button4.Enabled = true;
            else button4.Enabled = false;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            state = 2;
            button1.Enabled = true;
            button2.Enabled = false;
            button3.Enabled = true;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            state = 3;
            button1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = false;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            state = 1;
            button1.Enabled = false;
            button2.Enabled = true;
            button3.Enabled = true;
        }
        private bool validate(int i, int j)
        {
            if (m[i, j] != 0) return false;
            return true;
        }
        private void dijkstrastart()
        {
            m[start.X, start.Y] = 1;
            int pas = 1; bool sch = false;
            do
            {
                sch = false;
                for (int i = 0; i < 28; i++)
                    for (int j = 0; j < 17; j++)
                        if (m[i, j] == pas)
                        {
                            for (int k = 0; k < 4; k++)
                            {
                                int next_i = i + diri[k];
                                int next_j = j + dirj[k];
                                if (next_i >= 0 && next_i < 28 && next_j >= 0 && next_j < 17)
                                    if (m[next_i, next_j] < 0 || panels[next_i, next_j].BackColor == Color.Red)
                                    {
                                        pas = pas + Math.Abs(m[next_i, next_j]);
                                        if (doubleThreaded)
                                            fromend.Abort();
                                        if (doubleThreaded) pas--;
                                        MessageBox.Show("The length of the path is " + pas + " steps.");
                                        length = pas;
                                        drum(start.X, start.Y, 0);
                                        return;
                                    }
                                    else if (m[next_i, next_j] == 0)
                                    {
                                        m[next_i, next_j] = pas + 1;
                                        panels[next_i, next_j].BackColor = Color.Yellow;
                                        sch = true;
                                    }
                                Thread t = new Thread(Wait);
                                t.Start();
                                t.Join();
                            }
                        }
                pas++;
            } while (sch);
            if (doubleThreaded)
                fromend.Abort();
            MessageBox.Show("There is not a path between these points");

        }
        private void dijkstraend()
        {
            m[end.X, end.Y] = -1;
            int pas = -1; bool sch = false;
            do
            {
                sch = false;
                for (int i = 0; i < 28; i++)
                    for (int j = 0; j < 17; j++)
                        if (m[i, j] == pas)
                        {
                            for (int k = 0; k < 4; k++)
                            {
                                int next_i = i + diri[k];
                                int next_j = j + dirj[k];
                                if (next_i >= 0 && next_i < 28 && next_j >= 0 && next_j < 17)
                                    if (m[next_i, next_j] > 0 && m[next_i, next_j] != 999)
                                    {
                                        pas = Math.Abs(pas) + m[next_i, next_j];
                                        fromstart.Abort();
                                        if (doubleThreaded) pas--;
                                        MessageBox.Show("The length of the path is " + pas + " steps.");
                                        length = pas;
                                        drum(start.X, start.Y, 0);
                                        return;
                                    }
                                    else if (m[next_i, next_j] == 0)
                                    {
                                        sch = true;
                                        m[next_i, next_j] = pas - 1;
                                        panels[next_i, next_j].BackColor = Color.Yellow;
                                    }
                                Thread t = new Thread(Wait);
                                t.Start();
                                t.Join();
                            }
                        }
                pas--;
            } while (sch);
            fromstart.Abort();
            MessageBox.Show("There is not a path between these points");
        }
        private void button4_Click(object sender, EventArgs e)
        {
            button4.Enabled = false;
            trackBar2.Enabled = false;

            for (int i = 0; i < 28; i++)
                for (int j = 0; j < 17; j++)
                    if (panels[i, j].BackColor == Color.Black)
                        m[i, j] = 999;

            fromstart = new Thread(dijkstrastart);
            fromstart.Start();
            if (doubleThreaded)
            {
                fromend = new Thread(dijkstraend);
                fromend.Start();
            }
        }
        void drum(int pozx, int pozy, int length2)
        {

            if (length2 > length) return;

            if (finish) return;

            //directie 1
            if (pozx + 1 >= 1 && pozx + 1 <= 27)
                if (panels[pozx + 1, pozy].BackColor == Color.Red)
                {
                    finish = true;
                    return;
                }
            if (pozx + 1 >= 1 && pozx + 1 <= 27)
                if (panels[pozx + 1, pozy].BackColor == Color.Yellow)
                {
                    panels[pozx + 1, pozy].BackColor = Color.Purple;
                    drum(pozx + 1, pozy, length2 + 1);
                    if (finish == false)
                        panels[pozx + 1, pozy].BackColor = Color.Yellow;
                    else return;
                }

            //directie 2
            if (pozx - 1 >= 0 && pozx - 1 <= 26)
                if (panels[pozx - 1, pozy].BackColor == Color.Red)
                {
                    finish = true;
                    return;
                }
            if (pozx - 1 >= 0 && pozx - 1 <= 26)
                if (panels[pozx - 1, pozy].BackColor == Color.Yellow)
                {
                    panels[pozx - 1, pozy].BackColor = Color.Purple;
                    drum(pozx - 1, pozy, length2 + 1);
                    if (finish == false)
                        panels[pozx - 1, pozy].BackColor = Color.Yellow;
                    else return;
                }

            //directie 3
            if (pozy + 1 >= 1 && pozy + 1 <= 16)
                if (panels[pozx, pozy + 1].BackColor == Color.Red)
                {
                    finish = true;
                    return;
                }
            if (pozy + 1 >= 1 && pozy + 1 <= 16)
                if (panels[pozx, pozy + 1].BackColor == Color.Yellow)
                {
                    panels[pozx, pozy + 1].BackColor = Color.Purple;
                    drum(pozx, pozy + 1, length2 + 1);
                    if (finish == false)
                        panels[pozx, pozy + 1].BackColor = Color.Yellow;
                    else return;
                }


            //directie 4
            if (pozy - 1 >= 0 && pozy - 1 <= 15)
                if (panels[pozx, pozy - 1].BackColor == Color.Red)
                {
                    finish = true;
                    return;
                }
            if (pozy - 1 >= 0 && pozy - 1 <= 15)
                if (panels[pozx, pozy - 1].BackColor == Color.Yellow)
                {
                    panels[pozx, pozy - 1].BackColor = Color.Purple;
                    drum(pozx, pozy - 1, length2 + 1);
                    if (finish == false)
                        panels[pozx, pozy - 1].BackColor = Color.Yellow;
                    else return;
                }

        }
        private void Wait(object obj)
        {
            Thread.Sleep(sleeptime);
        }
        private void button5_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 28; i++)
                for (int j = 0; j < 17; j++)
                {
                    panels[i, j].BackColor = System.Drawing.Color.Green;
                    m[i, j] = 0;
                }
            button1.Enabled = false;
            button2.Enabled = true;
            button4.Enabled = false;
            button3.Enabled = true;
            endPoint = false;
            startPoint = false;
            state = 1;
            finish = false;
            trackBar2.Enabled = true;
            button4.Enabled = true;
        }
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            sleeptime = 120 - trackBar1.Value;
        }
        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            if (trackBar2.Value == 1)
                doubleThreaded = true;
            else doubleThreaded = false;
        }
        private void saveMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                if(groupBox1.Visible==true)
                saveFileDialog1.FileName = "lee";
                else
                saveFileDialog1.FileName = "dijkstra";
            saveFileDialog1.Filter = "txt files (*.txt)|*.txt";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter sw = new StreamWriter(saveFileDialog1.FileName))
                {   
                    if(groupBox1.Visible==true)
                    for (int i = 0; i < 28; i++)
                    {
                        for (int j = 0; j < 17; j++)
                            if (panels[i, j].BackColor == Color.Black)
                                sw.Write(1 + " ");
                            else sw.Write(0 + " ");
                        sw.WriteLine();
                    }
                    else
                    {
                        sw.WriteLine(SharedData.n);
                        for (int i = 0; i < SharedData.n; i++)
                        {
                            sw.WriteLine(last_points[i].X);
                            sw.WriteLine(last_points[i].Y);
                        }
                        for (int i = 0; i < SharedData.n; i++) { 
                            for (int j = 0; j < SharedData.n; j++)
                                sw.WriteLine(SharedData.matrix[i, j]);
                        }
                    }
                }
            }
        }
        private void openMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "txt files (*.txt)|*.txt";
            ofd.FilterIndex = 1;
            ofd.RestoreDirectory = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                using (StreamReader sr = new StreamReader(ofd.FileName))
                {   
                    if(groupBox1.Visible==true)
                    for (int i = 0; i < 28; i++)
                    {
                        for (int j = 0; j < 17; j++)
                        {
                            int nr = sr.Read() - 48;
                            if (nr == 1) panels[i, j].BackColor = Color.Black;
                            sr.Read();
                        }
                        sr.Read();
                        sr.Read();
                    }
                    else
                    {
                        int.TryParse(sr.ReadLine(),out SharedData.n);
                        //Console.WriteLine(SharedData.n);
                        for (int i = 0; i < SharedData.n; i++)
                        {   
                        int nr1,nr2;
                        int.TryParse(sr.ReadLine(), out nr1);
                        int.TryParse(sr.ReadLine(), out nr2);
                        last_points.Add(new Point(nr1, nr2));
                        //Console.WriteLine(last_points[i].X + "  " + last_points[i].Y);
                        }

                        for (int i = 0; i < SharedData.n; i++)
                        {  
                            for (int j = 0; j < SharedData.n; j++)
                            {
                                int.TryParse(sr.ReadLine(), out SharedData.matrix[i, j]);
                            }
                        }
                        drawPaths();
                    }
                }
            }
        }
        private void graphsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            groupBox1.Visible = false;
            groupBox2.Visible = true;
            groupBox3.Visible = true;
        }
        private void matrixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            groupBox3.Visible = false;
            groupBox2.Visible = false;
            groupBox1.Visible = true;
        }
        void drawPanel_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            t.Enabled = true;
            t.Start();
            for(int i = 0; i < SharedData.n; i++)
                if (Math.Abs(e.X - last_points[i].X) <= 60 && Math.Abs(e.Y - last_points[i].Y) <= 60)
                {
                    new_k = i;
                    break;
                }
        }
        void drawPanel_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (SeFace)
            {
                poz_noua = new Point(e.X, e.Y);
                last_points[new_k] = poz_noua;
                drawPaths();
                Thread.Sleep(78); // Nu face lag
            }
        }
        void t_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            SeFace = true;
            t.Stop();
        }
        void drawPanel_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            t.Stop();
            SeFace = false;
            drawPaths();
        }
        void drawPanel_MouseClick(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left && SharedData.n<10 && SeFace == false)
            {   
                last_points.Add(new Point(e.X, e.Y));
                SharedData.n++;
            }
            else if (e.Button == MouseButtons.Right && SharedData.n > 0 && SeFace == false)
            {
                last_points.RemoveAt(SharedData.n - 1);
                for (int i = 0; i < SharedData.n - 1; i++)
                {
                        SharedData.matrix[SharedData.n - 1,i] = 0;
                        SharedData.matrix[i,SharedData.n - 1] = 0;
                }   
                SharedData.n--;
            }
            if(SeFace == false)
            drawPaths();
        }       
        private void button6_Click(object sender, EventArgs e)
        {       
                f2.ShowDialog();
        }
        void f2_Disposed(object sender, EventArgs e)
        {
            drawPaths();
        }
        public void drawPaths()
        {
            drawGraphics.Clear(Color.Gray);

            for(int i=0;i<SharedData.n;i++)
                for(int j=0;j<SharedData.n;j++)
                {
                    if (SharedData.matrix[i,j] != 0)
                    {
                        drawGraphics.DrawLine(new Pen(Color.BurlyWood, 5), last_points[i], last_points[j]);
                        if(SeFace==false)
                        drawGraphics.FillEllipse(new SolidBrush(Color.BurlyWood), (last_points[i].X + last_points[j].X) / 2 - 15, (last_points[i].Y + last_points[j].Y) / 2 - 9, 30, 18);
                       if(SeFace==false){ 
                        // pentru lungimile de pe laturi
                       if(SharedData.matrix[i,j]>=1000)
                            drawGraphics.DrawString(SharedData.matrix[i, j].ToString(), new Font("Arial Narrow", 8, FontStyle.Regular), new SolidBrush(Color.Brown), (last_points[i].X + last_points[j].X) / 2 - 11, (last_points[i].Y + last_points[j].Y) / 2 - 7);
                        else if (SharedData.matrix[i, j] < 100 && SharedData.matrix[i, j] >= 10)
                            drawGraphics.DrawString(SharedData.matrix[i, j].ToString(), new Font("Arial Narrow", 8, FontStyle.Regular), new SolidBrush(Color.Brown), (last_points[i].X + last_points[j].X) / 2 - 8, (last_points[i].Y + last_points[j].Y) / 2 - 7);
                        else if (SharedData.matrix[i, j] >= 100 && SharedData.matrix[i, j] < 1000)
                            drawGraphics.DrawString(SharedData.matrix[i, j].ToString(), new Font("Arial Narrow", 8, FontStyle.Regular), new SolidBrush(Color.Brown), (last_points[i].X + last_points[j].X) / 2 - 8, (last_points[i].Y + last_points[j].Y) / 2 - 7);
                        else
                            drawGraphics.DrawString(SharedData.matrix[i, j].ToString(), new Font("Arial Narrow", 8, FontStyle.Regular), new SolidBrush(Color.Brown), (last_points[i].X + last_points[j].X) / 2 - 4, (last_points[i].Y + last_points[j].Y) / 2 - 7);
                       }
                    }   
                }
            for (int i = 0; i < SharedData.n; i++)
            {
                drawGraphics.FillEllipse((Brush)new SolidBrush(Color.Brown), last_points[i].X - 30, last_points[i].Y - 30, 60, 60);
                if(SeFace==false)
                    if(i+1<10)
                    drawGraphics.DrawString((i + 1).ToString(), new Font("Arial", 16), (Brush)new SolidBrush(Color.Black), last_points[i].X - 10, last_points[i].Y - 10);
                    else
                    drawGraphics.DrawString((i + 1).ToString(), new Font("Arial", 16), (Brush)new SolidBrush(Color.Black), last_points[i].X - 17, last_points[i].Y - 10);
            }
        }
        private void button7_Click(object sender, EventArgs e)
        {
            int.TryParse(textBox1.Text,out Dij_start);
            if (Dij_start <= 0 || Dij_start > SharedData.n)
            {
                MessageBox.Show("Incorect Value!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
            {   
                button6.Enabled = false;
                richTextBox1.Text = "";
                int[] D = new int[10];
                int[] S = new int[10];

                for (int i = 0; i < 10; i++)
                    T[i] = D[i] = S[i] = 0;

                int minim, poz = 0, inf = int.MaxValue / 3;
                Dij_start--;
                S[Dij_start] = 1;
                for (int i = 0; i < SharedData.n; i++)
                    for (int j = 0; j < SharedData.n; j++)
                        if (SharedData.matrix[i, j] == 0)
                            SharedData.matrix[i, j] = inf;

                for (int i = 0; i < SharedData.n; i++)
                {
                    D[i] = SharedData.matrix[Dij_start, i];
                    if (i != Dij_start && D[i] < inf)
                        T[i] = Dij_start;
                }

                for (int i = 0; i < SharedData.n - 1; i++)
                {
                    minim = inf;

                    for (int j = 0; j < SharedData.n; j++)
                        if (S[j] == 0 && D[j] < minim)     ///  <=> Daca nodul j nu e vizitat si distanta nu e infinita
                        {
                            minim = D[j];     /// Cautam distanta cea mai mica intre nodul r si celelalte noduri
                            poz = j;          /// poz retine nodul de legatura de distanta minima
                        }

                    S[poz] = 1;           /// trecem nodul de distanta minima la cele vizitate

                    for (int j = 0; j < SharedData.n; j++)
                        if (S[j] == 0 && D[j] > D[poz] + SharedData.matrix[poz, j])    /// daca nodul j e nevizitat si distanta pana la j e mai mare decat o ruta ocolitoare
                        {                       /// D[poz] e cel mai scurt drum gasit anterior
                            D[j] = D[poz] + SharedData.matrix[poz, j];
                            T[j] = poz;
                        }
                }

                for (int i = 0; i < SharedData.n; i++)
                    if (D[i] >= 10000) D[i] = 0;
                for (int i = 0; i < SharedData.n; i++)
                {
                    if (i != Dij_start)
                    {   
                        richTextBox1.AppendText((Dij_start + 1) + " -> " + (i + 1) + " = " + D[i] + "\n");
                        if (D[i] != 0)
                        {
                            richTextBox1.AppendText("Nodes: " + (Dij_start + 1) + "  ");
                            drum(i);
                            richTextBox1.AppendText("\n");
                        }
                    }
                }
            }
            for (int i = 0; i < SharedData.n; i++)
                for (int j = 0; j < SharedData.n; j++)
                    if (SharedData.matrix[i, j] >= 10000)
                        SharedData.matrix[i, j] = 0;
            button6.Enabled = true;
        }
        void drum(int i)
        {
            if (T[i]!=0) drum(T[i]);
            richTextBox1.AppendText((i+1) + "  ");
        }
        private void button8_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "";
            last_points.Clear();
            drawGraphics.Clear(Color.Gray);
            SharedData.n = 0;
            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                {
                    SharedData.matrix[i, j] = 0;
                    SharedData.labels[i, j].Text = "";
                }
        }
      
    }
}
