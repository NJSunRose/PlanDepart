using EMSCheck;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
//using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication4
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.textBox1.Text))
                return;
            if (this.radioButton1.Checked)
            {
                if (this.textBox1.Text.IndexOf("SJ") == 0)
                    return;
                if (this.textBox1.Text.IndexOf("EMS-") < 0)
                {
                    this.textBox1.Text = "EMS-" + this.textBox1.Text;
                }
            }
            //连接数据库
            SqlConnection sqlcon = new SqlConnection("Data Source=192.168.1.25;Initial Catalog=AIO75_SH_BEQJ;Persist Security Info=True;User ID=sa;Password=pushiAIO7");
            try
            {
                sqlcon.Open();
                //获取数据
                SqlDataAdapter da = new SqlDataAdapter(string.Format("select * from vyucbomb  where bomid in (select bomid from ucbom where ordernumatcrd = '{0}')", this.textBox1.Text), sqlcon);
                //绑定数据
                DataSet ds = new DataSet();
                da.Fill(ds, "table");
                dataGridView1.DataSource = ds;
                dataGridView1.DataMember = "table";
                if (ds.Tables[0].Rows.Count > 0)
                {
                    this.label2.Text = "五金信息：编号异常！请联系建模组检查模型！";
                    this.label2.ForeColor = Color.Red;
                }
                else
                    this.label2.ForeColor = Color.Black;

                //获取数据
                SqlDataAdapter daboma = new SqlDataAdapter(string.Format("select * from vyucboma  where bomid in (select bomid from ucbom where ordernumatcrd = '{0}')", this.textBox1.Text), sqlcon);
                //绑定数据
                DataSet dsboma = new DataSet();
                daboma.Fill(dsboma, "table");
                dataGridView2.DataSource = dsboma;
                dataGridView2.DataMember = "table";
                if (dsboma.Tables[0].Rows.Count > 0)
                {
                    this.label3.Text = "板件信息：编号异常！请联系建模组检查模型！";
                    this.label3.ForeColor = Color.Red;
                }
                else
                    this.label3.ForeColor = Color.Black;


                //获取数据
                SqlDataAdapter dadorb = new SqlDataAdapter(string.Format("select doorid,itmid,itmname,itmtename,serials from vyucdorb where doorid in (select doorid from ucdor where ordernumatcrd = '{0}')", this.textBox1.Text), sqlcon);
                //绑定数据
                DataSet dsdorb = new DataSet();
                dadorb.Fill(dsdorb, "table");
                dataGridView3.DataSource = dsdorb;
                dataGridView3.DataMember = "table";
                //if (dsdorb.Tables[0].Rows.Count > 0)
                //{
                //    this.label4.Text = "门车间产品信息";
                //    this.label4.ForeColor = Color.Red;
                //}
                //else
                //    this.label4.ForeColor = Color.Black;


            }
            catch (Exception ex)
            {

            }
            finally
            {
                sqlcon.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                SqlConnection sqlcon = new SqlConnection("Data Source=192.168.1.25;Initial Catalog=AIO75_SH_BEQJ;Persist Security Info=True;User ID=sa;Password=pushiAIO7");
                try
                {
                    sqlcon.Open();

                    string bomid = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
                    string linenum = dataGridView1.SelectedRows[0].Cells[1].Value.ToString();

                    SqlCommand cmd = new SqlCommand(string.Format("delete ucbomb where bomid in('{0}') and linenum = {1}", bomid, linenum), sqlcon);
                    cmd.ExecuteNonQuery();

                    SqlCommand cmd2 = new SqlCommand(string.Format("update ucbomb set linenum = linenum - 1 where bomid in('{0}') and linenum > {1})", bomid, linenum), sqlcon);
                    cmd2.ExecuteNonQuery();
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    sqlcon.Close();
                    MessageBox.Show("OK");
                }
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                SqlConnection sqlcon = new SqlConnection("Data Source=192.168.1.25;Initial Catalog=AIO75_SH_BEQJ;Persist Security Info=True;User ID=sa;Password=pushiAIO7");
                try
                {
                    string itmid = "";
                    string itmname = "";
                    string qty = "";
                    using (HardwareInfo hdifo = new HardwareInfo())
                    {
                        if (hdifo.ShowDialog() == DialogResult.OK)
                        {
                            itmid = hdifo.itmid;
                            itmname = hdifo.itmname;
                            qty = hdifo.qty;
                        }
                    }
                    if (string.IsNullOrEmpty(itmid) || string.IsNullOrEmpty(itmname))
                    {
                        MessageBox.Show("编码和名称不能为空!");
                        return;
                    }
                    sqlcon.Open();

                    string bomid = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
                    string linenum = dataGridView1.SelectedRows[0].Cells[1].Value.ToString();

                    string sql = string.Format("update ucbomb set itmid={2},itmname='{3}',qty={4} where bomid in('{0}') and linenum = {1}", bomid, linenum, itmid, itmname, qty);
                    SqlCommand cmd2 = new SqlCommand(sql, sqlcon);
                    cmd2.ExecuteNonQuery();
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    sqlcon.Close();
                    MessageBox.Show("OK");
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.textBox2.Text))
                return;
            //if (this.radioButton1.Checked)
            {
                if (this.textBox2.Text.IndexOf("SJ") == 0)
                    return;
                if (this.textBox2.Text.IndexOf("EMS-") < 0)
                {
                    this.textBox2.Text = "EMS-" + this.textBox2.Text;
                }
            }

            SqlConnection sqlcon = new SqlConnection("Data Source=192.168.1.25;Initial Catalog=AIO75_SH_BEQJ;Persist Security Info=True;User ID=sa;Password=pushiAIO7");
            try
            {
                sqlcon.Open();

                SqlDataAdapter dadorb = new SqlDataAdapter(string.Format("select * from v_Att where ordernumatcrd ='{0}'and itmid='{1}'", this.textBox2.Text, this.textBox3.Text), sqlcon);
                //绑定数据
                DataSet dsdorb = new DataSet();
                dadorb.Fill(dsdorb, "table");
                dataGridView4.DataSource = dsdorb;
                dataGridView4.DataMember = "table";

            }
            catch (SystemException ex)
            {

            }
            finally
            {
                sqlcon.Close();
                //MessageBox.Show("OK");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (dataGridView4.SelectedRows.Count > 0)
            {
                SqlConnection sqlcon = new SqlConnection("Data Source=192.168.1.25;Initial Catalog=AIO75_SH_BEQJ;Persist Security Info=True;User ID=sa;Password=pushiAIO7");
                try
                {
                    sqlcon.Open();

                    string bomid = dataGridView4.SelectedRows[0].Cells[5].Value.ToString();
                    string linenum = dataGridView4.SelectedRows[0].Cells[1].Value.ToString();

                    SqlCommand cmd = new SqlCommand(string.Format("delete ucmrdb where docnum ={0} and linenum = {1}", bomid, linenum), sqlcon);
                    cmd.ExecuteNonQuery();

                    SqlCommand cmd2 = new SqlCommand(string.Format("update ucmrdb set linenum = linenum - 1 where bomid ={0} and linenum > {1})", bomid, linenum), sqlcon);
                    cmd2.ExecuteNonQuery();
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    sqlcon.Close();
                    MessageBox.Show("OK");
                }
            }
        }
    }
}
