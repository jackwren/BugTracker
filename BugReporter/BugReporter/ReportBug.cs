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
    public partial class ReportBug : Form
    {
        private string CurrentPath = "/";

        SqlCeConnection mySqlConnection;
        SqlCeDataReader mySqlDataReader;
        
        public ReportBug()
        {
            InitializeComponent();
            populateListBox();
            getID();
            
        }

        public void populateListBox()
    	{

       	mySqlConnection = new SqlCeConnection(@"Data Source=C:\temp\Mydatabase.sdf ");
 
        //writes a select sql statement to access everything
       	String selcmd = "SELECT * FROM tbl_bug ORDER BY bug_id";
 
       	SqlCeCommand mySqlCommand = new SqlCeCommand(selcmd, mySqlConnection);
 
       	try
       	{
            //opens connection
           	mySqlConnection.Open();
 
           	mySqlDataReader = mySqlCommand.ExecuteReader();
        }
 
       	catch (SqlCeException ex)
       	{
 
         	  MessageBox.Show("Failure" + ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
       	}
 
    	}
 
    	public void cleartxtBoxes()
    	{
        	txtStatus.Text = cmbImp.Text = "";
            textEditorControl1.Text = string.Empty;
    	}

        //establishes id so the next one is selected
        public void getID() 
        {
            while (mySqlDataReader.Read())
            {
                int getId = Convert.ToInt32(mySqlDataReader["bug_id"]) + 1;
                string myString = getId.ToString();
                txtID.Text = myString;
            }
        }

        public bool checkInputs()
    	{
        	bool rtnvalue = true;
        	//checks if empty
       	if (string.IsNullOrEmpty(txtID.Text) ||
            string.IsNullOrEmpty(textEditorControl1.Text) ||
            string.IsNullOrEmpty(txtStatus.Text) ||
           	string.IsNullOrEmpty(cmbImp.Text))
        	{
            	MessageBox.Show("Error: Please check your inputs");
            	rtnvalue = false;
        	}
 
        	return (rtnvalue);
 
    	}
 
 
        public void insertRecord(String ID, String code, String status, String importance, String user_name,  String commandString)
    	{
 
        	try
  	      {
            	SqlCeCommand cmdInsert = new SqlCeCommand(commandString, mySqlConnection);
 
                //adds paprameters to textboxs to link with sql
                cmdInsert.Parameters.AddWithValue("@ID", ID);
                cmdInsert.Parameters.AddWithValue("@code", code);
                cmdInsert.Parameters.AddWithValue("@status", status);
                cmdInsert.Parameters.AddWithValue("@importance", importance);
                cmdInsert.Parameters.AddWithValue("@user", user_name);
            	cmdInsert.ExecuteNonQuery();
                MessageBox.Show("Succesfully reported your bug to the database!");
        	}
        	catch (SqlCeException ex)
        	{
            	MessageBox.Show(ID + " .." + ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        	}
 
    	}


        private void button1_Click(object sender, EventArgs e)
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


          if (checkInputs())
  	      {
                //writes an insert sql statement  to add all fields to databse
            	String commandString = "INSERT INTO tbl_bug(bug_id, code, status, importance, user_name) VALUES (@ID, @code, @status, @importance, @user)";
 
            	insertRecord(txtID.Text, textEditorControl1.Text, txtStatus.Text, cmbImp.Text, txt_user.Text, commandString);
            	populateListBox ();
            	cleartxtBoxes ();
                
        	}
        }

        private void button2_Click(object sender, EventArgs e)
        {
            cleartxtBoxes();
        }

        private void txtCode_TextChanged(object sender, EventArgs e)
        {
               
        }

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
        public void toUser(string sData)
        {
            // Here is where I wrote to my textbox
            txt_user.Text = sData;
        }

        private void Upload_Click(object sender, EventArgs e)
        {
            
        }

        private void ReportBug_Load(object sender, EventArgs e)
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
