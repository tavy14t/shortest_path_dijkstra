using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication11
{   
    public partial class Form2 : Form
    {
        
        public Form2()
        {   
            InitializeComponent();
            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                {
                    SharedData.labels[i, j] = new Label();
                    tableLayoutPanel1.Controls.Add(SharedData.labels[i, j], i, j);
                }
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            for (int i = 0; i < SharedData.n; i++)
            {
                comboBox1.Items.Add(i+1);
                for (int j = 0; j < SharedData.n; j++)
                {
                    SharedData.labels[i, j].Text = SharedData.matrix[i, j].ToString();
                }
            } 
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox2.Items.Clear();
            for (int i = 1; i <= SharedData.n; i++)
            {   
                if( comboBox1.SelectedItem.ToString() != i.ToString() )
                comboBox2.Items.Add(i);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {  
            int i, j;
            if(comboBox1.SelectedItem != null)
                i = Convert.ToInt16(comboBox1.SelectedItem);
            else i = Convert.ToInt16(comboBox1.Text) <= SharedData.n ? Convert.ToInt16(comboBox1.Text) : 0;
            
            if(comboBox1.SelectedItem != null)
                j = Convert.ToInt16(comboBox2.SelectedItem);
            else j = Convert.ToInt16(comboBox2.Text) <= SharedData.n ? Convert.ToInt16(comboBox2.Text) : 0;
            if (i == 0 || j == 0 || i==j) return;
            int nr;
            int.TryParse(textBox1.Text, out nr );
            if (nr >= 10000) return;
            SharedData.matrix[i - 1,j - 1] = nr;
            SharedData.matrix[j - 1,i - 1] = nr;
            for (i = 0; i < SharedData.n; i++)
            {
                for (j = 0; j < SharedData.n; j++)
                {
                        SharedData.labels[i, j].Text = SharedData.matrix[i, j].ToString();
                        //tableLayoutPanel1.Controls.Add(new Label() { Text = SharedData.matrix[i,j].ToString() }, i, j);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
