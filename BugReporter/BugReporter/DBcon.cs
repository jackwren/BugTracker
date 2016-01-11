using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlServerCe;

namespace BugReporter
{
    public class DBcon
    {
       
        SqlCeConnection mySqlConnection;

        public void Connection()
        {
        
        mySqlConnection = new SqlCeConnection(@"Data Source=C:\temp\Mydatabase.sdf ");
 
       	try
       	{
            //opens connection
           	mySqlConnection.Open();
        }
 
       	catch (SqlCeException ex)
       	{
            Console.WriteLine("-- Unable to Connect");
       	}
        }

        public void LoadUsers()
        {
            //writes a select sql statement to access everything
            String usrcmd = "@SELECT Count(*) FROM users WHERE user_name = @user_name AND password = @password";

            SqlCeCommand mySqlCommand = new SqlCeCommand(usrcmd, mySqlConnection);

            try
            {
                SqlCeDataReader mySqlDataReader = mySqlCommand.ExecuteReader();
            }

            catch (SqlCeException ex)
            {

                Console.WriteLine("-- Unable to Connect");
            }

        }

        public void LoadBugs()
        {
           //writes a select sql statement to access everything
           String selcmd = "SELECT * FROM tbl_bug ORDER BY bug_id";

           SqlCeCommand mySqlCommand = new SqlCeCommand(selcmd, mySqlConnection);

           try
           {
               SqlCeDataReader mySqlDataReader = mySqlCommand.ExecuteReader();
           }

           catch (SqlCeException ex)
           {

               Console.WriteLine("-- Unable to Connect");
           }

        }

        

        public void LoadEditBug()
        {
            //writes a select sql statement to access everything
            String edcmd = "SELECT bug_id, code FROM tbl_bug ORDER BY bug_id";

            SqlCeCommand mySqlCommand = new SqlCeCommand(edcmd, mySqlConnection);

            try
            {
                SqlCeDataReader mySqlDataReader = mySqlCommand.ExecuteReader();
            }

            catch (SqlCeException ex)
            {

                Console.WriteLine("-- Unable to Connect");
            }
        }

    }
}
