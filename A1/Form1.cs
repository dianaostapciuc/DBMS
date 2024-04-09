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
using System.Security.Cryptography;
using System.Diagnostics.Contracts;

namespace A1
{
    public partial class Form1 : Form
    {
        SqlConnection cs = new SqlConnection("Data Source=DESKTOP-0B2ERMB\\SQLEXPRESS;Initial Catalog=hair_products;Integrated Security=True;TrustServerCertificate=true;");
        SqlDataAdapter da = new SqlDataAdapter();
        DataSet ds = new DataSet();
        DataSet ds_child = new DataSet();
        BindingSource bs = new BindingSource();
        BindingSource bs_child = new BindingSource();


        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            da.SelectCommand = new SqlCommand("SELECT * FROM POSITION", cs);
            ds.Clear();
            da.Fill(ds);
            dataGridView2.DataSource = ds.Tables[0];
            bs.DataSource = ds.Tables[0];

        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                cs.Open();

                SqlCommand getLastEidCmd = new SqlCommand("SELECT MAX(eid) FROM EMPLOYEE", cs);
                int lastEid = (int)getLastEidCmd.ExecuteScalar();
                int newEid = lastEid + 1;
                Random rnd = new Random();
                int newDid = rnd.Next(1, 6);

                da.InsertCommand = new SqlCommand("INSERT INTO EMPLOYEE VALUES(@eid, @oid, @did, @name, @dob, @salary)", cs);
                da.InsertCommand.Parameters.Add("@eid", SqlDbType.Int).Value = newEid;
                da.InsertCommand.Parameters.Add("@oid", SqlDbType.Int).Value = textBox1.Text;
                da.InsertCommand.Parameters.Add("@did", SqlDbType.Int).Value = newDid;
                da.InsertCommand.Parameters.Add("@name", SqlDbType.VarChar).Value = textBox3.Text;
                da.InsertCommand.Parameters.Add("@dob", SqlDbType.Date).Value = DateTime.Parse(textBox4.Text);
                da.InsertCommand.Parameters.Add("@salary", SqlDbType.Int).Value = Int32.Parse(textBox5.Text);

                da.InsertCommand.ExecuteNonQuery();
                MessageBox.Show("Added to the database!");
                cs.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                cs.Close();
            }
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView2_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView2.SelectedRows.Count > 0)
            {
                int selectedOid = Convert.ToInt32(dataGridView2.SelectedRows[0].Cells["oid"].Value);

                da.SelectCommand = new SqlCommand("SELECT * FROM EMPLOYEE WHERE oid = @selectedOid", cs);
                da.SelectCommand.Parameters.Add("@selectedOid", SqlDbType.Int).Value = selectedOid;
                ds_child.Clear();
                da.Fill(ds_child);
                bs_child.DataSource = ds_child.Tables[0];

                DataView dv = new DataView(ds_child.Tables[0]);
                dv.RowFilter = $"oid = {selectedOid}";
                dataGridView1.DataSource = dv;


            }
        }

        private void dataGridView1Update()
        {
            dataGridView1.ClearSelection();
            dataGridView1.Rows[bs_child.Position].Selected = true;
            records();
        }

        private void dataGridView2Update()
        {
            dataGridView2.ClearSelection();
            dataGridView2.Rows[bs.Position].Selected = true;
        }

        private void records()
        {
            label11.Text = "Record " + (bs_child.Position+1) + " of  "
           + bs_child.Count;
        }

        private void btnPrevious_Click_Click(object sender, EventArgs e)
        {
            bs_child.MovePrevious();
            dataGridView1Update();
            records();
        }

        private void btnNext_Click_Click(object sender, EventArgs e)
        {
            bs_child.MoveNext();
            dataGridView1Update();
            records();
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }


        private void btnUpdate_Click(object sender, EventArgs e)
        {
            int x;
            da.UpdateCommand = new SqlCommand("Update EMPLOYEE set name = @name, date_of_birth = @dob, salary = @salary where eid = @id", cs);

            try
            {
                da.UpdateCommand.Parameters.Add("@name", SqlDbType.VarChar).Value = textBox3.Text;
                da.UpdateCommand.Parameters.Add("@dob", SqlDbType.Date).Value = textBox4.Text;
                da.UpdateCommand.Parameters.Add("@salary", SqlDbType.Int).Value = textBox5.Text;

                da.UpdateCommand.Parameters.Add("@id", SqlDbType.Int).Value = ds_child.Tables[0].Rows[bs_child.Position][0];

            }
            catch (Exception ex){
                MessageBox.Show(ex.Message);
                return;
            }

            cs.Open();
            x = da.UpdateCommand.ExecuteNonQuery();
            cs.Close();
            if (x >= 1)
            {
                MessageBox.Show("The record has been updated");
            }
        }


        private void btnDelete_Click(object sender, EventArgs e)
        {
            DialogResult dr;
            dr = MessageBox.Show("Are you sure?\n No undo after delete", "Confirm Deletion", MessageBoxButtons.YesNo);
            if (dr == DialogResult.Yes)
            {
                da.DeleteCommand = new SqlCommand("Delete from EMPLOYEE where eid = @eid", cs);
                da.DeleteCommand.Parameters.Add("@eid",SqlDbType.Int).Value = ds_child.Tables[0].Rows[bs_child.Position][0];
                cs.Open();
                da.DeleteCommand.ExecuteNonQuery();
                cs.Close();
                ds.Clear();
                da.SelectCommand = new SqlCommand("SELECT * FROM POSITION", cs);
                da.Fill(ds);
            }
            else
            {
                MessageBox.Show("Deletion Aborded");
            }

        }

        private void btnNext_Parent_Click(object sender, EventArgs e)
        {
            bs.MovePrevious();
            dataGridView2Update();
        }

        private void btnPrevious_Parent_Click(object sender, EventArgs e)
        {
            bs.MoveNext();
            dataGridView2Update();
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
