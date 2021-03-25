using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Reporting.WinForms;

namespace WindowsFormsApp2.Report
{
    public partial class PrintInvoice : Form
    {
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter adp;
        public PrintInvoice()
        {
            InitializeComponent();
        }

        private void PrintInvoice_Load(object sender, EventArgs e)
        {
            con = new SqlConnection(@"Data Source=JAY;Initial Catalog=First;Integrated Security=True");
            cmd = new SqlCommand("select sum(SubTotal) from Invoice_Details where InvoiceId = " + Convert.ToInt32(Form1.id), con);
            con.Open();
            int temp = Convert.ToInt32(cmd.ExecuteScalar());
            con.Close();
            adp = new SqlDataAdapter("select * from Invoice_Details where InvoiceId = " + Convert.ToInt32(Form1.id), con);
            ReportParameterCollection rpc = new ReportParameterCollection();
            rpc.Add(new ReportParameter("total", temp.ToString()));
            DataSet1 ds = new DataSet1();
            adp.Fill(ds, "Invoice_Table"); 
            ReportDataSource dataSource = new ReportDataSource("DataSet1", ds.Tables[0]);
            this.reportViewer1.LocalReport.SetParameters(rpc);
            this.reportViewer1.LocalReport.DataSources.Clear();
            this.reportViewer1.LocalReport.DataSources.Add(dataSource);
            this.reportViewer1.RefreshReport();
        }

        private void reportViewer1_Load(object sender, EventArgs e)
        {

        }
    }
}
