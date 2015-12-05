using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlServerCe;

namespace BugReporter
{
    public partial class EditData : Form
    {
        SqlCeConnection mySqlConnection;
        SqlCeCommand mySqlCommand;

        public EditData()
        {
            InitializeComponent();
            Connection();
            LoadSubjects();

        }

        private void Connection()
        {
            mySqlConnection = new SqlCeConnection(@"Data Source=C:\temp\Mydatabase.sdf ");

            String selcmd = "SELECT bug_id, code FROM tbl_bug ORDER BY bug_id";

            mySqlCommand = new SqlCeCommand(selcmd, mySqlConnection);

            try
            {
                mySqlConnection.Open();
                SqlCeDataReader mySqlDataReader = mySqlCommand.ExecuteReader();

            }

            catch (SqlCeException ex)
            {

                MessageBox.Show("Failure" + ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }


        private void LoadSubjects()
        {
           

            using (SqlCeCommand cmd = new SqlCeCommand ("SELECT bug_id FROM tbl_bug", mySqlConnection))
            {
                using (IDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        comboBox1.Items.Add(dr["bug_id"].ToString());
                    }
                }
            }

        }

        private void Code()
        {
            int code = comboBox1.SelectedIndex + 1;

            string sqlquery = "SELECT code FROM tbl_bug WHERE bug_id = " + code;

            SqlCeCommand command = new SqlCeCommand(sqlquery, mySqlConnection);


            try
            {

                SqlCeDataReader sdr = command.ExecuteReader();

                while (sdr.Read())
                {
                    txtCode.Text = (sdr["code"].ToString());
                }

            }

            catch (SqlCeException ex)
            {

                MessageBox.Show("Failure with connection!");
            }

            

        }

        private void load_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex >= 0)
            {
                Code();
            }
            else
            {
                MessageBox.Show("Please select and ID then re-load the Code, Thanks!");
            }

        }

        public void cleartxtBoxes()
        {
            comboBox1.Text = txtCode.Text = txtStatus.Text = cmbImp.Text = "";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            cleartxtBoxes();
        }




        private void Update_Click(object sender, EventArgs e)
        {
              SqlCeCommand cmd = new SqlCeCommand("UPDATE tbl_bug SET bug_id = @bug_id, code = @code, status = @status, importance = @importance WHERE bug_id = @bug_id", mySqlConnection);

              cmd.Parameters.AddWithValue("@bug_id", comboBox1.Text);
              cmd.Parameters.AddWithValue("@code", txtCode.Text);
              cmd.Parameters.AddWithValue("@status", txtStatus.Text);
              cmd.Parameters.AddWithValue("@importance", cmbImp.Text);
              cmd.ExecuteNonQuery();

              LoadBug ld = new LoadBug();
              this.Close();
              ld.Show();
 
                
        }


    }
}
