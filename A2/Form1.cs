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
using System.Configuration;
using System.Net.NetworkInformation;

namespace A1
{
    public partial class Form1 : Form
    {
        SqlConnection cs;
        SqlDataAdapter da = new SqlDataAdapter();
        DataSet ds = new DataSet();
        DataSet ds_child = new DataSet();
        BindingSource bs = new BindingSource();
        BindingSource bs_child = new BindingSource();
        List<TextBox> textBoxes = new List<TextBox>();


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

            string con = ConfigurationManager.ConnectionStrings["cn"].ConnectionString;
            cs = new SqlConnection(con);
            string select = ConfigurationSettings.AppSettings["select"];
            da.SelectCommand = new SqlCommand(select, cs);
            ds.Clear();
            da.Fill(ds);
            dataGridView2.DataSource = ds.Tables[0];
            bs.DataSource = ds.Tables[0];

            List<string> col_names = new List<string>(ConfigurationSettings.AppSettings["ChildColumnNames"].Split(','));
            int nr = int.Parse(ConfigurationSettings.AppSettings["ChildNumberOfColumns"]);
            int i = 0;
            foreach(string col in col_names)
            {
                TextBox textbox = new TextBox();
                textbox.Location = new System.Drawing.Point(10, 10 + i * 30);
                panel1.Controls.Add(textbox);
                textBoxes.Add(textbox);
                textbox.Text = col;
                i++;
            }


        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                cs.Open();


                string insert = ConfigurationSettings.AppSettings["InsertQuery"];
                da.InsertCommand = new SqlCommand(insert, cs);

                List<string> col_names = new List<string>(ConfigurationSettings.AppSettings["ChildColumnNames"].Split(','));
                if(col_names.Count == textBoxes.Count)
                {
                    for(int i=0; i<col_names.Count; i++) 
                    {
                        da.InsertCommand.Parameters.AddWithValue("@" + col_names[i], textBoxes[i].Text);
                    }
                }

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

                string ptKey = ConfigurationManager.AppSettings["ParentTableKey"];
                int selectedKey = Convert.ToInt32(dataGridView2.SelectedRows[0].Cells[ptKey].Value);
                string selectParameter = ConfigurationManager.AppSettings["ChildTableParameter"];

                try
                {
                    string connectionString = ConfigurationManager.ConnectionStrings["cn"].ConnectionString;
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        string selectCommand = ConfigurationManager.AppSettings["SelectSpecific"];
                        SqlDataAdapter da = new SqlDataAdapter(selectCommand, connection);
                        da.SelectCommand.Parameters.Add(selectParameter, SqlDbType.Int).Value = selectedKey;
                        ds_child.Clear();
                        da.Fill(ds_child);
                        bs_child.DataSource = ds_child.Tables[0];

                        DataView dv = new DataView(ds_child.Tables[0]);
                        dataGridView1.DataSource = dv;
                    }
                }
                catch(Exception ex) {
                    MessageBox.Show(ex.Message);
                }
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
            string update = ConfigurationManager.AppSettings["UpdateQuery"];
            da.UpdateCommand = new SqlCommand(update, cs);

            try
            {
                List<string> col_names = new List<string>(ConfigurationManager.AppSettings["ChildColumnNames"].Split(','));
                string selectKey = ConfigurationManager.AppSettings["ChildTableKey"];
                string parentKey = ConfigurationManager.AppSettings["ParentTableKey"];
                string foreignKey = ConfigurationManager.AppSettings["ForeignKey"];


                if (col_names.Count == textBoxes.Count)
                {
                    for (int i = 0; i < col_names.Count; i++)
                    {
                        if (foreignKey!= "")
                        {
                            if (col_names[i] != selectKey && col_names[i] != foreignKey && col_names[i] != parentKey)
                            {
                                da.UpdateCommand.Parameters.AddWithValue("@" + col_names[i], textBoxes[i].Text);
                            }
                        }
                        else
                        {
                            if (col_names[i] != selectKey && col_names[i] != parentKey)
                            {
                                da.UpdateCommand.Parameters.AddWithValue("@" + col_names[i], textBoxes[i].Text);
                            }
                        }
                        
                    }
                }

                da.UpdateCommand.Parameters.Add("@" + selectKey, SqlDbType.Int).Value = ds_child.Tables[0].Rows[bs_child.Position][0];
            }
            catch (Exception ex)
            {
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
                string delete = ConfigurationSettings.AppSettings["DeleteQuery"];
                da.DeleteCommand = new SqlCommand(delete, cs);
                string childKey = ConfigurationSettings.AppSettings["ChildTableKey"];
                da.DeleteCommand.Parameters.Add("@" + childKey,SqlDbType.Int).Value = ds_child.Tables[0].Rows[bs_child.Position][0];
                cs.Open();
                da.DeleteCommand.ExecuteNonQuery();
                cs.Close();
                ds.Clear();
                string select = ConfigurationSettings.AppSettings["select"];
                da.SelectCommand = new SqlCommand(select, cs);
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
