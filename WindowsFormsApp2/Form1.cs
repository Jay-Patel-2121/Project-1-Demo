using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Drawing.Printing;

namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {
        public static int id = 0;
        int pos = 0;
        SqlConnection con;
        SqlCommand cmd;
        int tol = 0;

        SqlDataReader dr;
        PrintDocument document = new PrintDocument();
        PrintDialog dialog = new PrintDialog();
        public Form1()
        {
            InitializeComponent();
            document.PrintPage += new PrintPageEventHandler(document_PrintPage);
        }
        void document_PrintPage(object sender, PrintPageEventArgs e)
        {
         //   e.Graphics.DrawString(txt_print.Text, new Font("Arial", 20, FontStyle.Regular), Brushes.Black, 20, 20);
        }
        void addButtons()
        {
            con = new SqlConnection(@"Data Source=JAY;Initial Catalog=First;Integrated Security=True");
            var rowCount = rowss();
            var columnCount = 3;
            con.Open();
            cmd = new SqlCommand("Select * from Tbl_Students", con);
            dr = cmd.ExecuteReader();
            this.tableLayoutPanel1.ColumnCount = columnCount;
            this.tableLayoutPanel1.RowCount = rowCount;
            this.tableLayoutPanel1.ColumnStyles.Clear();
            this.tableLayoutPanel1.RowStyles.Clear();
            
            for (int i = 0; i < columnCount; i++)
            {
                this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100 / columnCount));
            }
            for (int i = 0; i < rowCount; i++)
            {
                this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100 / 5));
            }
            
            
            while (dr.Read())
            {
                var b = new Button();
                b.Text = dr.GetString(1);
                b.Name = dr.GetString(1);
                b.Click += b_Click;
                b.Dock = DockStyle.Fill;
                this.tableLayoutPanel1.Controls.Add(b);
            }
            con.Close();
        }
        
        void b_Click(object sender, EventArgs e)
        {
            try
            {
                var b = sender as Button;
                int r = 0, qty;
                cmd = new SqlCommand("Select Rate from Tbl_Students where Name = '" + b.Text + "'", con);
                con.Open();
                r = Convert.ToInt32(cmd.ExecuteScalar());
                con.Close();
                qty = Convert.ToInt32(textBox3.Text);
                int sub = r * qty;
                dataGridView.Rows.Add(b.Text, textBox3.Text, r, sub);
                //total
                r = dataGridView.Rows.Count;
                tol = 0;
                for (int i = 0; i < r; i++)
                {
                    tol += Convert.ToInt32(dataGridView.Rows[i].Cells[3].Value);
                }
                label3.Text = "Total : " + tol.ToString();
            }
            catch(Exception )
            {
                MessageBox.Show("Please enter quantity first");
            }
            
        }
        int rowss()
        {
            int r = 0;
            cmd = new SqlCommand("Select count(*) from Tbl_Students", con);
            con.Open();
            r = Convert.ToInt32(cmd.ExecuteScalar());
            con.Close();
            return r;
        }
        private void Form1_Load(object sender, EventArgs e)
        {

            addButtons();
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            new Report.PrintInvoice().Show();
            // txt_print.AppendText("JAY JALARAM KHAMAN \nAND LOCHO HOUSE\nLOCHO\t5\t30\t150\n--------------------------\nTOTAL : 150");
            //dataGridView.Rows.Add("Jay");
            /*dataGridView.DataSource = null;
            dataGridView.DataBind();*/
            //tableLayoutPanel1.Controls.Clear();
            /*dialog.Document = document;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                document.Print();
            }*/
        }

        private void dataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
            MessageBox.Show(dataGridView.SelectedCells[0].Value.ToString());
            
        }

        private void dataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            MessageBox.Show(dataGridView.SelectedCells[0].Value.ToString());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            cmd = new SqlCommand("insert into Tbl_Students values ('" + textBox1.Text + "'," + Convert.ToInt32(textBox2.Text) + ")",con);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
            MessageBox.Show("Inserted !!");
            tableLayoutPanel1.Controls.Clear();
            addButtons();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            cmd = new SqlCommand("insert into Invoice_Master values(GETDATE())", con);
            con.Open();
            cmd.ExecuteNonQuery();
            cmd = new SqlCommand("select max(InvoiceId) from Invoice_Master", con);
            id = Convert.ToInt32(cmd.ExecuteScalar());
            int rows = Convert.ToInt32(dataGridView.Rows.Count) - 1;
            
            for (int i = 0; i < rows; i++)
            {
               
                cmd = new SqlCommand("Insert_Items");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = con;
                cmd.Parameters.AddWithValue("@invoiceid", id);
                cmd.Parameters.AddWithValue("@itemname", dataGridView.Rows[i].Cells[0].Value.ToString());
                cmd.Parameters.AddWithValue("@qty", Convert.ToInt32(dataGridView.Rows[i].Cells[1].Value));
                cmd.Parameters.AddWithValue("@rate", Convert.ToInt32(dataGridView.Rows[i].Cells[2].Value));
                cmd.Parameters.AddWithValue("@subtotal", Convert.ToInt32(dataGridView.Rows[i].Cells[3].Value));
                cmd.ExecuteNonQuery();
                
            }
            con.Close();
            MessageBox.Show("Inserted");
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Before " + tol.ToString());
            tol = tol - Convert.ToInt32(dataGridView.Rows[pos].Cells[3].Value);
            MessageBox.Show("After " + tol.ToString());
            label3.Text = "Total : " + tol.ToString();
            dataGridView.Rows.RemoveAt(pos);
        }

        private void dataGridView_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            pos = e.RowIndex;
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
