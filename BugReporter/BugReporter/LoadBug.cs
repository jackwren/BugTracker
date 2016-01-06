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
    public partial class LoadBug : Form
    {
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

            String sqlquery = "SELECT * FROM tbl_bug WHERE user_name ='" + userNme + "'";

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

        private void button1_Click(object sender, EventArgs e)
        {
            userNme = user.Text;
            populateListBox();
        }

    }
}
