using SpreadsheetGear;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace EMSCheck
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SqlConnection sqlcon = new SqlConnection("Data Source=192.168.1.25;Initial Catalog=AIO75_SH_BEQJ;Persist Security Info=True;User ID=sa;Password=pushiAIO7");
            string sql = string.Format("select * from v_YPchQuery where NumAtCrd='{0}' order by crdname,z_buyname,ddh", this.textBox1.Text);

            try
            {
                sqlcon.Open();
                SqlDataAdapter dadorb = new SqlDataAdapter(sql, sqlcon);
                //绑定数据
                DataSet dsdorb = new DataSet();
                dadorb.Fill(dsdorb, "table");

                PCGroup pcGroup = new PCGroup();
                double totleArea = 0;
                foreach (DataRow dr in dsdorb.Tables[0].Rows)
                {
                    PCData dataEntity = new PCData();
                    dataEntity.PCH = dr["NumAtCrd"].ToString();
                    dataEntity.DDH = dr["DDH"].ToString();
                    dataEntity.GH = dr["SortID"].ToString();
                    dataEntity.JXS = dr["CrdName"].ToString();
                    dataEntity.KH = dr["Z_BuyName"].ToString();
                    dataEntity.COLOR = dr["BarCodes"].ToString();
                    dataEntity.AREA = double.Parse(dr["MJ"].ToString());
                    totleArea += dataEntity.AREA;

                    dataEntity.BC = dr["BS"].ToString();
                    pcGroup.list.Add(dataEntity);

                }

                pcGroup.totalAREA = totleArea;
                BindData(pcGroup);
                textBox2.Text = totleArea.ToString();


            }
            catch (SystemException ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                sqlcon.Close();
                //MessageBox.Show("OK");
            }
        }

        public void BindData(PCGroup pcGroup)
        {
            this.dataGridView1.Rows.Clear();
            foreach (PCData dataEntity in pcGroup.list)
            {
                int index = this.dataGridView1.Rows.Add();
                this.dataGridView1.Rows[index].Cells[0].Value = index + 1;
                this.dataGridView1.Rows[index].Cells[1].Value = dataEntity.DDH;
                this.dataGridView1.Rows[index].Cells[2].Value = dataEntity.GH;

                this.dataGridView1.Rows[index].Cells[4].Value = dataEntity.JXS;
                this.dataGridView1.Rows[index].Cells[5].Value = dataEntity.KH;

                this.dataGridView1.Rows[index].Cells[7].Value = dataEntity.COLOR;
                this.dataGridView1.Rows[index].Cells[8].Value = dataEntity.BC;
                this.dataGridView1.Rows[index].Cells[9].Value = dataEntity.AREA;// ToString("0.2f");
                this.dataGridView1.Rows[index].Tag = dataEntity;

            }
            this.dataGridView1.Tag = pcGroup;
        }

        public string ReportBillTemplate = @"Temp\workbill.xls";

        private void button2_Click(object sender, EventArgs e)
        {
            string dir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            SaveFileDialog sadlg = new SaveFileDialog();
            sadlg.Filter = "批次统计|.xls";
            sadlg.FileName =Path.Combine(dir, this.textBox1.Text + ".xls");
            if (sadlg.ShowDialog() == DialogResult.OK)
            {
                string savepath = sadlg.FileName;
                if (File.Exists(savepath))
                    File.Delete(savepath);
                WriteReport(savepath);
            }
        }

        void WriteReport(string filename)
        {
            if (File.Exists(ReportBillTemplate))
            {
                File.Copy(ReportBillTemplate, filename);
                IWorkbook bookReport = null;
                if (File.Exists(filename))
                {
                    bookReport = Factory.GetWorkbook(filename);
                }
                IRange cells = bookReport.Worksheets[0].Cells;
                int index = 0;
                int startrow = 7;
                bool iswhitebk = true;

                PCGroup pcGroup = dataGridView1.Tag as PCGroup;
                int groupindex = 0;
                foreach (PCData nc in pcGroup.list)
                {
                    string cellB = string.Format("B{0}", startrow + index);
                    string cellC = string.Format("C{0}", startrow + index);
                    string cellE = string.Format("E{0}", startrow + index);
                    string cellF = string.Format("F{0}", startrow + index);
                    string cellH = string.Format("H{0}", startrow + index);
                    string cellI = string.Format("I{0}", startrow + index);
                    string cellJ = string.Format("J{0}", startrow + index);


                    cells[cellB].Formula = nc.DDH;
                    cells[cellC].Formula = nc.GH;
                    cells[cellE].Formula = nc.JXS;
                    cells[cellF].Formula = nc.KH;
                    cells[cellH].Formula = nc.COLOR;
                    cells[cellI].Formula = nc.BC;
                    cells[cellJ].Formula = nc.AREA.ToString();

                    index++;
                }
                cells["B3"].Formula = this.textBox1.Text;
                cells["F3"].Formula = this.textBox2.Text;
                bookReport.Save();
                bookReport.Close();
            }
        }
    }

    public partial class PCGroup
    {
        public List<PCData> list = new List<PCData>();
        public double totalAREA;
    }

    public partial class PCData
    {
        public string PCH;
        public string DDH;
        public string GH;
        public string JXS;
        public string KH;
        public string COLOR;
        public double AREA;
        public string BC;

    }
}
