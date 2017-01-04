using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ip2region
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string path = Application.StartupPath + @"\ip2region.db";
            ip2region r = new ip2region(path);
            var ipinfo = r.MemorySearch(textBox1.Text);
            if (ipinfo != null)
                MessageBox.Show(ipinfo.Country + ipinfo.Region + ipinfo.Province + ipinfo.City + ipinfo.ISP);
            else
                MessageBox.Show("查询失败！");
        }
    }
}
