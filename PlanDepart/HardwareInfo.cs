using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
//using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EMSCheck
{
    public partial class HardwareInfo : Form
    {
        internal string itmname;
        internal string itmid;
        internal string qty;

        public HardwareInfo()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            itmid = this.textBox1.Text;
            itmname = this.textBox2.Text;
            qty = this.textBox3.Text;
            this.DialogResult = DialogResult.OK;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void textBox2_MouseClick(object sender, MouseEventArgs e)
        {
            string itmid = this.textBox1.Text;
            SqlConnection sqlcon = new SqlConnection("Data Source=192.168.1.25;Initial Catalog=AIO75_SH_BEQJ;Persist Security Info=True;User ID=sa;Password=pushiAIO7");
            string sql = string.Format("select itmname from mditm where itmid='{0}'", itmid);
            sqlcon.Open();

            SqlCommand cmd2 = new SqlCommand(sql, sqlcon);

            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter(cmd2);
            da.Fill(ds);
            DataTable dt = ds.Tables[0];
            foreach(DataRow dr in dt.Rows)
            {
                string str = dr[0].ToString();
                this.textBox2.Text = str;
            }
            sqlcon.Close();
        }
    }
}
