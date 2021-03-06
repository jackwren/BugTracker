﻿using System;
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
    public partial class Register : Form
    {

        SqlCeConnection mySqlConnection;

        public Register()
        {
            InitializeComponent();
            populateListBox();
        }

        public void populateListBox()
        {
            mySqlConnection = new SqlCeConnection(@"Data Source=C:\temp\Mydatabase.sdf ");

            //establishes a connection
            String selcmd = "SELECT user_name, password FROM users ORDER BY user_name";

            SqlCeCommand mySqlCommand = new SqlCeCommand(selcmd, mySqlConnection);

            try
            {
                //opens up connection to database
                mySqlConnection.Open();

                SqlCeDataReader mySqlDataReader = mySqlCommand.ExecuteReader();
            }

            catch (SqlCeException ex)
            {

                MessageBox.Show("Failure" + ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        public bool checkInputs()
        {
            bool rtnvalue = true;
            //checks to see if inputs are empty
            if (string.IsNullOrEmpty(textBox1.Text) ||
                string.IsNullOrEmpty(textBox2.Text))
            {
                MessageBox.Show("Error: Please check your inputs");
                rtnvalue = false;
            }

            return (rtnvalue);

        }

        public void insertRecord(String user, String pass, String commandString)
        {
            try
            {
                //makes a new command with the query created in commandstring
                SqlCeCommand cmdInsert = new SqlCeCommand(commandString, mySqlConnection);

                //adds fields from textbox to query
                cmdInsert.Parameters.AddWithValue("@user", user);
                cmdInsert.Parameters.AddWithValue("@pass", pass);
                cmdInsert.ExecuteNonQuery();
                MessageBox.Show("Succesfully signed up, you can now log in!");
                this.Close();
            }
            catch (SqlCeException ex)
            {
                MessageBox.Show(" This username - "+ user +" has already been used, try another!" );
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (checkInputs())
            {
                //makes a sql query to insert data
                String commandString = "INSERT INTO users(user_name, password) VALUES (@user, @pass)";
                insertRecord(textBox1.Text, textBox2.Text, commandString);
                populateListBox();
                
                
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
