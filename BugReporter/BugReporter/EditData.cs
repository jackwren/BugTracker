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
using Nemiro.OAuth;
using Nemiro.OAuth.LoginForms;

namespace BugReporter
{
    public partial class EditData : Form
    {
        private string CurrentPath = "/";

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

            if (openFileDialog1.ShowDialog() != System.Windows.Forms.DialogResult.OK) { return; }
            OAuthUtility.PutAsync("https://content.dropboxapi.com/1/files_put/auto/", new HttpParameterCollection
            {
                {"access_token",Properties.Settings.Default.AccessToken},
                {"path", Path.Combine(this.CurrentPath, Path.GetFileName(openFileDialog1.FileName)).Replace("\\","/")},
                {"overwrite","true"},
                {"autorename", "true"},
                {openFileDialog1.OpenFile()},
            },
            callback: Upload_Result
                );



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

        private void EditData_Load(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(Properties.Settings.Default.AccessToken))
            {
                this.GetAccessToken();

            }
            else
            {
                this.GetFiles();
            }

        }

        private void GetAccessToken()
        {
            var login = new DropboxLogin("7ejoi3gpspe8fqv", "zwtn1hf42s1v04s");
            login.Owner = this;
            login.ShowDialog();

            if (login.IsSuccessfully)
            {
                Properties.Settings.Default.AccessToken = login.AccessToken.Value;
                Properties.Settings.Default.Save();
            }
            else
            {
                MessageBox.Show("Error");
            }
        }

        private void GetFiles()
        {
            OAuthUtility.GetAsync
                ("https://api.dropboxapi.com/1/metadata/auto/",
                new HttpParameterCollection { 
                { "path", this.CurrentPath }, 
                { "access_token", Properties.Settings.Default.AccessToken } 
                },
                callback: GetFiles_Result);
        }

        private void GetFiles_Result(RequestResult result)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<RequestResult>(GetFiles_Result), result);
                return;
            }
            if (result.StatusCode == 200)
            {
                listBox1.Items.Clear();
                listBox1.DisplayMember = "path";


                foreach (UniValue file in result["contents"])
                {
                    listBox1.Items.Add(file);

                }
                if (this.CurrentPath != "/")
                {
                    listBox1.Items.Insert(0, UniValue.ParseJson("{path: '..'}"));
                }

            }
            else
            {
                MessageBox.Show("Error");
            }
        }

        private void Upload_Result(RequestResult result)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<RequestResult>(Upload_Result), result);
                return;
            }

            if (result.StatusCode == 200)
            {
                this.GetFiles();
            }
            else
            {
                if (result["error"].HasValue)
                {
                    MessageBox.Show(result["error"].ToString());

                }
                else
                {
                    MessageBox.Show(result.ToString());


                }

            }
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null) { return; }
            UniValue file = (UniValue)listBox1.SelectedItem;

            if (file["path"] == "..")
            {
                if (this.CurrentPath != "/")
                {
                    this.CurrentPath = Path.GetDirectoryName(this.CurrentPath).Replace("\\", "/");
                }
            }
            else
            {
                if (file["is_dir"] == 1)
                {
                    this.CurrentPath = file["path"].ToString();

                }
                else
                {
                    saveFileDialog1.FileName = Path.GetFileName(file["path"].ToString());
                    if (saveFileDialog1.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    {
                        return;
                    }

                    //var web = new WebClient();
                    //web.DownloadFile(String.Format("https://content.dropboxapi.com/1/files/auto/{0}?access_token={1}", file["path"], Properties.Settings.Default.AccessToken), saveFileDialog1.FileName);
                }
            }
            this.GetFiles();
        }


    }
}
