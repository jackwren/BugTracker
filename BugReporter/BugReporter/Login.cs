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
    public partial class Login : Form
    {
       
        public Login()
        {
            InitializeComponent();
        }

        private void cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void sign_in_Click(object sender, EventArgs e)
        {

            if (checkInputs())
            {
                login();
            }
            else { }
        }

        public bool checkInputs()
        {
            bool rtnvalue = true;
            //checks if empty
            if (string.IsNullOrEmpty(txt_user_name.Text) ||
                string.IsNullOrEmpty(txt_password.Text))
            {
                MessageBox.Show("Error: Please check your inputs");
                rtnvalue = false;
            }

            return (rtnvalue);

        }

        public void login()
        {
           //checks fields are empty

            if (txt_user_name.Text != "" & txt_password.Text != "")
            {
                string queryText = @"SELECT Count(*) FROM users 
                             WHERE user_name = @user_name AND password = @password";
                //makes connection to database, and writes a insert query
                using (SqlCeConnection cn = new SqlCeConnection(@"Data Source=C:\temp\Mydatabase.sdf "))
                using (SqlCeCommand cmd = new SqlCeCommand(queryText, cn))
                {
                    cn.Open();
                    //opens connection, sets fields with names like user_name
                    cmd.Parameters.AddWithValue("@user_name", txt_user_name.Text);
                    cmd.Parameters.AddWithValue("@password", txt_password.Text);
                    int result = (int)cmd.ExecuteScalar();
                    if (result > 0)
                    {
                        this.Hide();
                        Home frmchild = new Home();
                        frmchild.Show();
                        frmchild.WriteToText(txt_user_name.Text);
                    }  
                    else
                    {
                        MessageBox.Show("User Not Found!");
                    }
                        
                }
            }  


        }

        private void button1_Click(object sender, EventArgs e)
        {
            Register reg = new Register();
            reg.Show();
        }
    }
}
