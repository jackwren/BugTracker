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
using Nemiro.OAuth;
using Nemiro.OAuth.LoginForms;
using System.IO;

namespace BugReporter
{
    public partial class LoadBug : Form
    {

        private string CurrentPath = "/";
        private SqlCeConnection mySqlConnection;
        private String userNme;

        public LoadBug()
        {
            InitializeComponent();
            populateListBox();
            
        }

        public void toUser(String sData)
        {
            // Here is where I wrote to my textbox
            user.Text = sData;
           
        }

        public void populateListBox()
        {
            mySqlConnection = new SqlCeConnection(@"Data Source=C:\temp\Mydatabase.sdf ");

            //writes a sql query to select everything from the tbl_bug

            String sqlquery = "SELECT * FROM tbl_bug";

            SqlCeCommand mySqlCommand = new SqlCeCommand(sqlquery, mySqlConnection);
      
            try
            {
                mySqlConnection.Open();

                SqlCeDataReader mySqlDataReader = mySqlCommand.ExecuteReader();

                listView1.Items.Clear();

                

                while (mySqlDataReader.Read())
                {
                    // adds everything from database into a listview table, with sub boxes to space into columns
                    ListViewItem item1 = new ListViewItem(mySqlDataReader["bug_id"] + ":");
                    item1.SubItems.Add(mySqlDataReader["code"] + "");
                    item1.SubItems.Add(mySqlDataReader["status"] + "");
                    item1.SubItems.Add(mySqlDataReader["importance"]+ "");

                    listView1.Items.AddRange(new ListViewItem[] { item1 });

                   
                }
            }

            catch (SqlCeException ex)
            {

                MessageBox.Show("Failure " + ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void edit_Click(object sender, EventArgs e)
        {
            this.Close();
            EditData edit = new EditData();
            edit.Show();
            
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

        private void CreateFolder_Result(RequestResult result)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<RequestResult>(CreateFolder_Result), result);
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

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private void LoadBug_Load(object sender, EventArgs e)
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

        private void createFolderBtn_Click_1(object sender, EventArgs e)
        {
            OAuthUtility.PostAsync("https://api.dropboxapi.com/1/fileops/create_folder", new HttpParameterCollection { 
            { "access_token", Properties.Settings.Default.AccessToken }, 
            { "root", "auto" }, 
            { "path", Path.Combine(this.CurrentPath, createFolder.Text).Replace("\\", "/") } 
            },
               callback: CreateFolder_Result
               );
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
