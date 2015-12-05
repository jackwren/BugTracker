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
    public partial class ReportBug : Form
    {

        SqlCeConnection mySqlConnection;
        
        public ReportBug()
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
        }
 
       	catch (SqlCeException ex)
       	{
 
         	  MessageBox.Show("Failure" + ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
       	}
 
    	}
 
    	public void cleartxtBoxes()
    	{
        	txtID.Text = txtCode.Text  = txtStatus.Text = cmbImp.Text = "";
    	}
 
        public bool checkInputs()
    	{
        	bool rtnvalue = true;
        	
       	if (string.IsNullOrEmpty(txtID.Text) ||
            string.IsNullOrEmpty(txtCode.Text) ||
            string.IsNullOrEmpty(txtStatus.Text) ||
           	string.IsNullOrEmpty(cmbImp.Text))
        	{
            	MessageBox.Show("Error: Please check your inputs");
            	rtnvalue = false;
        	}
 
        	return (rtnvalue);
 
    	}
 
 
        public void insertRecord(String ID, String code, String status, String importance, String commandString)
    	{
 
        	try
  	      {
            	SqlCeCommand cmdInsert = new SqlCeCommand(commandString, mySqlConnection);
 
                cmdInsert.Parameters.AddWithValue("@ID", ID);
                cmdInsert.Parameters.AddWithValue("@code", code);
                cmdInsert.Parameters.AddWithValue("@status", status);
                cmdInsert.Parameters.AddWithValue("@importance", importance);
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
          if (checkInputs())
  	      {
 
            	String commandString = "INSERT INTO tbl_bug(bug_id, code, status, importance) VALUES (@ID, @code, @status, @importance)";
 
            	insertRecord(txtID.Text, txtCode.Text, txtStatus.Text, cmbImp.Text, commandString);
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
    }
}
