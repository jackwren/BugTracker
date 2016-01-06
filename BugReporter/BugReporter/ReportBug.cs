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
 
        //writes a select sql statement to access everything
       	String selcmd = "SELECT * FROM tbl_bug ORDER BY bug_id";
 
       	SqlCeCommand mySqlCommand = new SqlCeCommand(selcmd, mySqlConnection);
 
       	try
       	{
            //opens connection
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
        	txtID.Text = txtStatus.Text = cmbImp.Text = "";
            textEditorControl1.Text = string.Empty;
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
    }
}
