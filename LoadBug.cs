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
        SqlCeConnection mySqlConnection;
        Font defaultFont = SystemFonts.DefaultFont;
        public LoadBug()
        {
            InitializeComponent();
            populateListBox();
        }

        public void populateListBox()
        {
            mySqlConnection = new SqlCeConnection(@"Data Source=C:\temp\Mydatabase.sdf ");

            String selcmd = "SELECT bug_id, code, status, importance FROM tbl_bug ORDER BY bug_id";

            SqlCeCommand mySqlCommand = new SqlCeCommand(selcmd, mySqlConnection);

            try
            {
                mySqlConnection.Open();

                SqlCeDataReader mySqlDataReader = mySqlCommand.ExecuteReader();

                listView1.Items.Clear();

                listView1.Font = new Font(defaultFont.FontFamily, defaultFont.Size, FontStyle.Bold);

                while (mySqlDataReader.Read())
                {

                    ListViewItem item1 = new ListViewItem(mySqlDataReader["bug_id"] + ":");
                    item1.SubItems.Add(mySqlDataReader["code"] + "");
                    item1.SubItems.Add(mySqlDataReader["status"] + "");
                    item1.SubItems.Add(mySqlDataReader["importance"] + "");


                    listView1.Items.AddRange(new ListViewItem[] { item1 });

                   
                }
            }

            catch (SqlCeException ex)
            {

                MessageBox.Show("Failure" + ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

    }
}
