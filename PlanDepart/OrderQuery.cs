using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
//using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace EMSCheck
{
    public partial class OrderQuery : Form
    {
        public OrderQuery()
        {
            InitializeComponent();
            //dateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            //dateTimePicker1.CustomFormat = "yyyy-MM-dd-hh";
            this.dateTimePicker1.Value = DateTime.Now.AddMonths(-1);
            //dateTimePicker2.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            //dateTimePicker2.CustomFormat = "yyyy-MM-dd-hh";
            this.dateTimePicker2.Value = DateTime.Now;

        }

        private void button4_Click(object sender, EventArgs e)
        {
            string fromdate = this.dateTimePicker1.Value.ToString("yyyy-MM-dd");
            string todate = this.dateTimePicker2.Value.ToString("yyyy-MM-dd");
            if(File.Exists(@"C:\1.debug"))
                MessageBox.Show(fromdate);
            string state = "全部";
            if (this.radioButton1.Checked)
                state = "全部";
            else if (this.radioButton2.Checked)
                state = "已发货";
            else if (this.radioButton3.Checked)
                state = "部分发货";
            else if (this.radioButton4.Checked)
                state = "没有发货";
            SqlConnection sqlcon = new SqlConnection("Data Source=192.168.1.25;Initial Catalog=AIO75_SH_BEQJ;Persist Security Info=True;User ID=sa;Password=pushiAIO7");

            string sql = "";
            if (string.IsNullOrEmpty(this.textBox2.Text))
                sql = string.Format(@"Select TM1.NumAtCrd [定单号], Case when S1.DocEntry is null then '没有发货'
            when S1.DocEntry is not null  and S2.DocEntry is not null then '部分发货'
            else '已发货' end DocStatus, TM1.DocDate [下单日期], TM1.ReqDate [需求日期],
       TM1.CrdName [门店名称],
       TM2.BuyName [客户], 
      
        Case when TM1.DocStatus = 'C'  then TD2.SADate  else null  end SADate , TD1.HNum [总行数]
From SAOrd TM1
left Join(Select DocEntry, Count(DocEntry) HNum
           from SAOrdA
           Group by DocEntry
           ) TD1 on TM1.DocEntry = TD1.DocEntry
left Join(Select T.DocEntry, Max(T2.DocDate) SADate
           from SAOrdA T
          left
           join SASalA T1 on T.DocEntry = T1.BaseEntry and T.LineNum = T1.BaseLineNum and T.ObjType = T1.BaseType
          left join SASal T2 on T1.DocEntry = T2.DocEntry
           Group by T.DocEntry
           ) TD2 on TM1.DocEntry = TD2.DocEntry
left join UCOrd TM2 on TM1.NumAtCrd = TM2.NumAtCrd
left join(Select Distinct TD.DocEntry
           from SAOrdA TD
           where TD.LineStatus = 'C') S1 on S1.DocEntry = TM1.DocEntry
left join(Select Distinct TD.DocEntry
           from SAOrdA TD
           where TD.LineStatus = 'O') S2 on S2.DocEntry = TM1.DocEntry
where TM1.OpDate >= '{0}'  and TM1.OpDate <='{1}' and  left(TM1.NumAtCrd, 3) = 'EMS' and
((Case when S1.DocEntry is null then '没有发货'
            when S1.DocEntry is not null  and S2.DocEntry is not null then '部分发货'
            else '已发货' end) = '{2}' or '{2}' = '全部') ", fromdate, todate, state);

            else
                sql = string.Format(@"Select TM1.NumAtCrd [定单号], TM1.DocDate [下单日期], TM1.ReqDate [需求日期],
       TM1.CrdName [门店名称],
       TM2.BuyName [客户],
       Case when S1.DocEntry is null then '没有发货'
            when S1.DocEntry is not null  and S2.DocEntry is not null then '部分发货'
            else '已发货' end DocStatus,
        Case when TM1.DocStatus = 'C'  then TD2.SADate  else null  end SADate , TD1.HNum [总行数]
From SAOrd TM1
left Join(Select DocEntry, Count(DocEntry) HNum
           from SAOrdA
           Group by DocEntry
           ) TD1 on TM1.DocEntry = TD1.DocEntry
left Join(Select T.DocEntry, Max(T2.DocDate) SADate
           from SAOrdA T
          left
           join SASalA T1 on T.DocEntry = T1.BaseEntry and T.LineNum = T1.BaseLineNum and T.ObjType = T1.BaseType
          left join SASal T2 on T1.DocEntry = T2.DocEntry
           Group by T.DocEntry
           ) TD2 on TM1.DocEntry = TD2.DocEntry
left join UCOrd TM2 on TM1.NumAtCrd = TM2.NumAtCrd
left join(Select Distinct TD.DocEntry
           from SAOrdA TD
           where TD.LineStatus = 'C') S1 on S1.DocEntry = TM1.DocEntry
left join(Select Distinct TD.DocEntry
           from SAOrdA TD
           where TD.LineStatus = 'O') S2 on S2.DocEntry = TM1.DocEntry
where TM1.OpDate >= '{0}'  and TM1.OpDate <='{1}' and  left(TM1.NumAtCrd, 3) = 'EMS'  and TM1.NumAtCrd ='{3}' and
((Case when S1.DocEntry is null then '没有发货'
            when S1.DocEntry is not null  and S2.DocEntry is not null then '部分发货'
            else '已发货' end) = '{2}' or '{2}' = '全部')", fromdate, todate, state, this.textBox2.Text);
            //if (this.radioButton1.Checked)
            {
                if (this.textBox2.Text.IndexOf("SJ") == 0)
                    return;
                if (!string.IsNullOrEmpty(this.textBox2.Text))
                {
                    if (this.textBox2.Text.IndexOf("EMS-") < 0)
                    {
                        this.textBox2.Text = "EMS-" + this.textBox2.Text;
                    }
                }
            }
            
            try
            {
                sqlcon.Open();
                SqlDataAdapter dadorb = new SqlDataAdapter(sql, sqlcon);
                //绑定数据
                DataSet dsdorb = new DataSet();
                dadorb.Fill(dsdorb, "table");
                dgvOrder.DataSource = dsdorb;
                dgvOrder.DataMember = "table";

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

        private void dgvorderbill_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvorderbill.SelectedRows.Count > 0)
            {
                string ordernum = dgvorderbill.SelectedRows[0].Cells[18].Value.ToString();
                string linenum = dgvorderbill.SelectedRows[0].Cells[0].Value.ToString();
                queryplate(ordernum, linenum);
                queryhardware(ordernum, linenum);
                querydoor(ordernum, linenum);
            }
        }

        private void querydoor(string ordernum, string linenum)
        {
            SqlConnection sqlcon = new SqlConnection("Data Source=192.168.1.25;Initial Catalog=AIO75_SH_BEQJ;Persist Security Info=True;User ID=sa;Password=pushiAIO7");
            string sql = string.Format(@"Select TD2.ItmName [名称],
       Case when TM3.NumAtCrd is null then '未包装' else '已包装' end [包装],TM3.NumAtCrd [包装号],TD2.SortID [分拣号],TD2.PlateID [板件号],TD2.SchPlateID [排程门板号],TD2.ItmSpec [规格],TD2.BarCodes [颜色],
       TD2.Length [成品长],TD2.Width [成品宽] ,TD2.Thickness [厚度],TD2.Area [面积],
       TM4.Ref1 [托号],TM4.LcnID [库位]
       
From SAOrd TM1
left join SAOrdA TD1 on TD1.DocEntry=TM1.DocEntry
left join UCMrd TM2 on TM2.BaseEntry=TD1.DocEntry and TM2.BaseLineNum = TD1.LineNum and TM2.BaseType=TD1.ObjType
left join UCMrdA TD2 on TD2.DocEntry=TM2.DocEntry
left join UCPck  TM3 on TM3.DocEntry=TD2.PackEntry
left join UCPin  TM4 on TM4.DocEntry=TM3.PinEntry
where TD2.Z_PartType='D' and TM1.NumAtCrd='{0}' and TD1.LineNum={1} and TD2.DocEntry is not null", ordernum, linenum);
            try
            {
                sqlcon.Open();

                SqlDataAdapter dadorb = new SqlDataAdapter(sql, sqlcon);
                //绑定数据
                DataSet dsdorb = new DataSet();
                dadorb.Fill(dsdorb, "table");
                dgvdoor.DataSource = dsdorb;
                dgvdoor.DataMember = "table";

            }
            catch (SystemException ex)
            {

            }
            finally
            {
                sqlcon.Close();
            }
        }

        private void queryhardware(string ordernum, string linenum)
        {
            SqlConnection sqlcon = new SqlConnection("Data Source=192.168.1.25;Initial Catalog=AIO75_SH_BEQJ;Persist Security Info=True;User ID=sa;Password=pushiAIO7");
            string sql = string.Format(@"Select Distinct TM6.NumAtCrd [包装号],isnull(TD7.Z_LcnID,'')  [库位号],isnull(TD7.Z_Tuohao,'') [托号]
From SAOrd TM
     left join SAOrdA TD on TM.DocEntry=TD.DocEntry
     left join UCMrd T on TD.DocEntry=T.BaseEntry and TD.ObjType=T.BaseType and TD.LineNum=T.BaseLineNum
left join UCMrdB T2 on T2.DocEntry=T.DocEntry 
left join UCPhrA TD6 on TD6.BaseEntry=T2.DocEntry and TD6.BaseLineNum=T2.LineNum and TD6.BaseType=T2.ObjType
left join UCPhr  TM6 on TM6.DocEntry=TD6.DocEntry
left join WHOinA TD7 on TD7.ItmTAName=TM6.NumAtCrd
where TM.NumAtCrd='{0}' and TD.LineNum={1}  and TM6.DocEntry is not Null", ordernum, linenum);
            try
            {
                sqlcon.Open();

                SqlDataAdapter dadorb = new SqlDataAdapter(sql, sqlcon);
                //绑定数据
                DataSet dsdorb = new DataSet();
                dadorb.Fill(dsdorb, "table");
                dgvHard.DataSource = dsdorb;
                dgvHard.DataMember = "table";

            }
            catch (SystemException ex)
            {

            }
            finally
            {
                sqlcon.Close();
            }
        }

        private void queryplate(string ordernum, string linenum)
        {
            SqlConnection sqlcon = new SqlConnection("Data Source=192.168.1.25;Initial Catalog=AIO75_SH_BEQJ;Persist Security Info=True;User ID=sa;Password=pushiAIO7");
            string sql = string.Format(@"Select TD2.ItmName [名称],TD2.SchPlateID [板件排程号],
       Case when TM3.NumAtCrd is null then '未包装' else '已包装' end [包装], TM3.NumAtCrd [包装号],TM4.Ref1 [托号],TM4.LcnID [库位号],TD2.SortID [分拣号],TD2.PlateID [板件号],TD2.ItmSpec [规格],TD2.BarCodes [颜色],
       TD2.CutLength [开料长],TD2.CutWidth [开料宽],TD2.Length [成品长],TD2.Width [成品宽],TD2.Thickness [厚度],TD2.Area [面积],
       --封边
       cast((Case when TD2.EdgeItmID1 is null  or TD2.EdgeItmID1='' then 0 else 1 end +
       Case when TD2.EdgeItmID2 is null  or TD2.EdgeItmID2='' then 0 else 1 end +
       Case when TD2.EdgeItmID3 is null  or TD2.EdgeItmID3='' then 0 else 1 end +
       Case when TD2.EdgeItmID4 is null  or TD2.EdgeItmID4='' then 0 else 1 end ) as Nvarchar )+'边'  [封边]
       
From SAOrd TM1
left join SAOrdA TD1 on TD1.DocEntry=TM1.DocEntry
left join UCMrd TM2 on TM2.BaseEntry=TD1.DocEntry and TM2.BaseLineNum = TD1.LineNum and TM2.BaseType=TD1.ObjType
left join UCMrdA TD2 on TD2.DocEntry=TM2.DocEntry
left join UCPck  TM3 on TM3.DocEntry=TD2.PackEntry
left join UCPin  TM4 on TM4.DocEntry=TM3.PinEntry
where TD2.Z_PartType<>'D' and TM1.NumAtCrd='{0}' and TD1.LineNum={1} and TD2.DocEntry is not null", ordernum, linenum);
            try
            {
                sqlcon.Open();

                SqlDataAdapter dadorb = new SqlDataAdapter(sql, sqlcon);
                //绑定数据
                DataSet dsdorb = new DataSet();
                dadorb.Fill(dsdorb, "table");
                dgvplate.DataSource = dsdorb;
                dgvplate.DataMember = "table";

            }
            catch (SystemException ex)
            {

            }
            finally
            {
                sqlcon.Close();
            }
        }

        private void dgvOrder_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvOrder.SelectedRows.Count > 0)
            {
                string ordernum = dgvOrder.SelectedRows[0].Cells[0].Value.ToString();
                SqlConnection sqlcon = new SqlConnection("Data Source=192.168.1.25;Initial Catalog=AIO75_SH_BEQJ;Persist Security Info=True;User ID=sa;Password=pushiAIO7");
                string sql = string.Format("select * from v_Yorderline where NumAtCrd='{0}'", ordernum);
//                string sql = string.Format(@"Select TD.LineNum [行号],
//	 TD.ItmName [产品名],
//           Case when S0.A1=S0.A2 and S0.A1>0  then '待排产'
//           when S0.A2>0  and S0.A1>S0.A2 then '部分排产'
//           when S0.A2=0 and S0.A3=S0.A1 then '已排产'
//           when S0.A3>0 and S0.A3<S0.A1 then '部分包装'
//           when S0.A3=0 and isnull(S1.A4,0)=0 then '包装完成'
//           when isnull(S1.A4,0)>0 and isnull(S1.A4,0)<S0.A1 then '部分入库'
//           when isnull(S1.A4,0)=S0.A1 and TD10.DocEntry is null then '入库完成'
//           when TD10.DocEntry is not null and TD.LineStatus='O' then '已排货'
//           when  TD.LineStatus='C' then '已发货'
//      else '' end [发货状态] ,
//    IsNull(right(TM1.NumAtCrd,9),'')+IsNull(right(TM11.NumAtCrd,9),'') [批次号],
// isnull(TM3.Ref1,'') +isnull(TM31.Ref1,'') [托号],isnull(TD7.Z_Tuohao,'') [五金托号],isnull(TD7.Z_LcnID,'') [五金库位] ,TD.BarCodes [颜色],
// isNull(S2.BJBS,0) [板件包数],isnull(S3.WJBS,0) [五金包数],isnull(S4.MBS,0) [门包数],
//isNull(S2.BJBS,0)  +isnull(S4.MBS,0)  [总包数],S8.B1 [板],
//S0.A2 [板已排],S8.B3 [板待包],S7.M1 [门],S7.M2 [门待排],S7.M3 [门待包], TM.NumAtCrd
//From SAOrd TM
//     left join SAOrdA TD on TM.DocEntry=TD.DocEntry
//     left join MDItm TB on TD.ItmID=TB.ItmID
//     left join BCStk TB1 on TB1.ItmID=TB.ItmID and TB1.WhsID=TB.WhsID
//     left join SASalA TD8 on TD8.BaseEntry=TD.DocEntry and TD8.BaseLineNum=TD.LineNum and TD8.BaseType=TD.ObjType
//     left join SASal  TM8 on TM8.DocEntry=TD8.DocEntry
//     left JOin UCDlyA TD10 on TD10.BaseEntry=TD.DocEntry and TD10.BaseLineNum=TD.LineNum and TD10.BaseType=TD.ObjType
//--板件
//     left join UCMrd T on TD.DocEntry=T.BaseEntry and TD.ObjType=T.BaseType and TD.LineNum=T.BaseLineNum and T.BomID<>'SYS'
//     left join (Select S.DocEntry,Count(*) BJBS
//                   from (
//                         Select distinct  DocEntry,PackEntry
//                         from UCMrdA
//                          where Z_PartType<>'D' and PackEntry<>0
//                         ) S 
//                   Group by S.DocEntry
//                   ) S2 on S2.DocEntry=T.DocEntry
//Left Join (
//		Select T0.DocEntry, Count(*) B1, Sum(case when T0.SchEntry > 0 then 0 else 1 end) B2,
//		       Sum(case when T0.PackEntry > 0 then 0 else 1 end) B3
//		From UCMrdA T0
//        where Z_PartType<>'D' 
//		Group By T0.DocEntry
//		) S8 on T.DocEntry = S8.DocEntry
//--五金
//     left join (Select MrdEntry,Count(*) WJBS
//                 from UCPhr
//                 Group by MrdEntry
//                   ) S3 on S3.MrdEntry=T.DocEntry
//--门
//     left join (Select S.DocEntry,Count(*)  MBS
//                   from (
//                         Select distinct  DocEntry,PackEntry
//                         from UCMrdA
//                          where Z_PartType='D' and PackEntry<>0
//                         ) S 
//                   Group by S.DocEntry
//                   ) S4 on S4.DocEntry=T.DocEntry
//Left Join (
//		Select T0.DocEntry, Count(*) M1, Sum(case when T0.SchEntry > 0 then 0 else 1 end) M2,
//		       Sum(case when T0.PackEntry > 0 then 0 else 1 end) M3
//		From UCMrdA T0
//        where Z_PartType='D' 
//		Group By T0.DocEntry
//		) S7 on T.DocEntry = S7.DocEntry
//--板件信息
//Left Join (
//		Select T0.DocEntry, Count(*) A1, Sum(case when T0.SchEntry > 0 then 0 else 1 end) A2,
//		       Sum(case when T0.PackEntry > 0 then 0 else 1 end) A3
//		From UCMrdA T0 
//		Group By T0.DocEntry
//		) S0 on T.DocEntry = S0.DocEntry
//--板件入库信息
//Left Join (
//		Select T0.DocEntry,Count(T0.DocEntry) A4
//        From UCMrdA T0 
//        inner join UCPck T1 on T0.PackEntry=T1.DocEntry
//        inner join UCPin T2 on T2.DocEntry=T1.PinEntry
//        where  T2.DocEntry is not null
//        Group by T0.DocEntry
//		) S1 on T.DocEntry = S1.DocEntry
//--柜体生产批号/托号/库位
//left join (Select  DocEntry,Min(LineNum) LineNum
//           from UCMrdA
//           where Z_PartType<>'D'
//           Group by DocEntry
//           ) T11 on T11.DocEntry=T.DocEntry 
//left join UCMrdA T1 on T1.DocEntry=T11.DocEntry and T1.LineNum=T11.LineNum
//left join UCSch  TM1 on TM1.DocEntry=T1.SchEntry
//left join UCPck  TM2 on TM2.DocEntry=T1.PackEntry
//left join UCPin  TM3 on TM3.DocEntry=TM2.PinEntry
//--移门批号/托号/库位
//left join (Select  DocEntry,Min(LineNum) LineNum
//           from UCMrdA
//           where Z_PartType='D'
//           Group by DocEntry
//           ) T13 on T13.DocEntry=T.DocEntry 
//left join UCMrdA T12 on T12.DocEntry=T13.DocEntry and T12.LineNum=T13.LineNum
//left join UCSch  TM11 on TM11.DocEntry=T12.SchEntry
//left join UCPck  TM21 on TM21.DocEntry=T12.PackEntry
//left join UCPin  TM31 on TM31.DocEntry=TM21.PinEntry

//--五金生产批号/托号/库位
//left join UCMrdB T2 on T2.DocEntry=T.DocEntry and T2.LineNum=1
//left join UCPhrA TD6 on TD6.BaseEntry=T2.DocEntry and TD6.BaseLineNum=T2.LineNum and TD6.BaseType=T2.ObjType
//left join UCPhr  TM6 on TM6.DocEntry=TD6.DocEntry
//left join WHOinA TD7 on TD7.ItmTAName=TM6.NumAtCrd
//where TM.NumAtCrd='{0}'   and TD.LineStatus<>'X'  
//Order by TM.DocEntry,TD.LineNum", ordernum);
                try
                {
                    sqlcon.Open();

                    SqlDataAdapter dadorb = new SqlDataAdapter(sql, sqlcon);
                    //绑定数据
                    DataSet dsdorb = new DataSet();
                    dadorb.Fill(dsdorb, "table");
                    dgvorderbill.DataSource = dsdorb;
                    dgvorderbill.DataMember = "table";

                    ordernum = dgvorderbill.Rows[0].Cells[18].Value.ToString();
                    string linenum = dgvorderbill.Rows[0].Cells[0].Value.ToString();
                    queryplate(ordernum, linenum);
                    queryhardware(ordernum, linenum);
                    querydoor(ordernum, linenum);
                }
                catch (SystemException ex)
                {

                }
                finally
                {
                    sqlcon.Close();
                    MessageBox.Show("OK");
                }
            }
        }

        private void dgvorderbill_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
