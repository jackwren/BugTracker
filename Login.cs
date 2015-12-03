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
            login();
        }

        public void login()
        {
           

            if (txt_user_name.Text != "" & txt_password.Text != "")
            {
                string queryText = @"SELECT Count(*) FROM users 
                             WHERE user_name = @user_name AND password = @password";
                using (SqlCeConnection cn = new SqlCeConnection(@"Data Source=C:\temp\Mydatabase.sdf "))
                using (SqlCeCommand cmd = new SqlCeCommand(queryText, cn))
                {
                    cn.Open();
                    cmd.Parameters.AddWithValue("@user_name", txt_user_name.Text);
                    cmd.Parameters.AddWithValue("@password", txt_password.Text);
                    int result = (int)cmd.ExecuteScalar();
                    if (result > 0)
                    {
                        this.Hide();
                        Home frmchild = new Home();
                        frmchild.Show();
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
