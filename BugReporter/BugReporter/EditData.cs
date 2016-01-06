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
using ICSharpCode.TextEditor.Document;
using System.IO;

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
            //locates the database in temp folder
            mySqlConnection = new SqlCeConnection(@"Data Source=C:\temp\Mydatabase.sdf ");

            //establishs a connection with database
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
            //selects id from database and lists them in a combobox, so user can select
            using (SqlCeCommand cmd = new SqlCeCommand ("SELECT bug_id FROM tbl_bug", mySqlConnection))
            {
                using (IDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        comboBox1.Items.Add(dr["bug_id"].ToString()); // adds list of id's to comboBox to select some code
                    }
                }
            }

        }

        private void Code()
        {
            int code = comboBox1.SelectedIndex + 1; 
            // had to add plus one, as it was retriving id as the previous number, so if picked bug_id2, it would give me 1's code

            //selectes the code from the database, where = to what bug id selected
            string sqlquery = "SELECT code, user_name FROM tbl_bug WHERE bug_id = " + code;   

            SqlCeCommand command = new SqlCeCommand(sqlquery, mySqlConnection);

            try
            {
                //reads the command and exports code from database to the textbox - txtCode
                SqlCeDataReader sdr = command.ExecuteReader();

                while (sdr.Read())
                {
                    textEditorControl1.Text = (sdr["code"].ToString());
                    txt_User.Text = (sdr["user_name"].ToString());
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
                Code();//runs code method when button clicked
            }
            else
            {
                MessageBox.Show("Please select and ID then re-load the Code, Thanks!");
            }

        }

        public void cleartxtBoxes()
        {
            comboBox1.Text = textEditorControl1.Text = txtStatus.Text = cmbImp.Text = "";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            cleartxtBoxes();
        }


        private void Update_Click(object sender, EventArgs e)
        {

            //when the update button is clicked, runs SQL update query and adds all new fields to the database
            SqlCeCommand cmd = new SqlCeCommand("UPDATE tbl_bug SET bug_id = @bug_id, code = @code, status = @status, importance = @importance, user_name = @user_name WHERE bug_id = @bug_id", mySqlConnection);

            //adds the the text fields and combo fields to the query, so can add to database
            cmd.Parameters.AddWithValue("@bug_id", comboBox1.Text);
            cmd.Parameters.AddWithValue("@code", textEditorControl1.Text);
            cmd.Parameters.AddWithValue("@status", txtStatus.Text);
            cmd.Parameters.AddWithValue("@importance", cmbImp.Text);
            cmd.Parameters.AddWithValue("@user_name", txt_User.Text);
            cmd.ExecuteNonQuery();

            
            this.Close();
            
 
                
        }

        //syntax library
        private void textEditorControl1_Load(object sender, EventArgs e)
        {

            string dirc = Application.StartupPath;
            FileSyntaxModeProvider fsmp;
            if (Directory.Exists(dirc))
            {
                fsmp = new FileSyntaxModeProvider(dirc);
                HighlightingManager.Manager.AddSyntaxModeFileProvider(fsmp);
                textEditorControl1.SetHighlighting("C#");
            }


        }


    }
}
