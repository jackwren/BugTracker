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
    public partial class Home : Form
    {
        public Home()
        {
            InitializeComponent();
        }

        public void WriteToText(string sData)
        {
            // Here is where I wrote to my textbox
            label1.Text = sData;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Bug Reporting Program              \n\r\n\rLeeds Beckett University               \n\r\n\r(C) 2015 Jack Wren");
        }

        private void cascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }

        private void tileVerticallyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }

        private void tileHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void reportBugToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReportBug frmchild = new ReportBug();
            frmchild.MdiParent = this;
            frmchild.Show();
            frmchild.toUser(label1.Text);
        }

        private void loadBugToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadBug frmchild = new LoadBug();
            frmchild.MdiParent = this;
            frmchild.Show();
            frmchild.toUser(label1.Text);
        } 
        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void log_out_Click(object sender, EventArgs e)
        {
            this.Hide();
            Login log = new Login();
            log.Show();
        }
        
    }
}
