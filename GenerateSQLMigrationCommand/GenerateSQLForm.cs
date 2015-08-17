using EnvDTE;
using EnvDTE80;

using Microsoft.VisualStudio.Shell;
using System;
using System.Windows.Forms;

namespace GenerateSQLMigrationCommand
{
    public partial class GenerateSQLForm : Form
    {
        public GenerateSQLForm()
        {
            InitializeComponent();
            hideError();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            var description = txtDescription.Text.Trim().ToLower();

            if (description.Length < 1)
            {
                showError("You must enter a valid description");
                return;
            }

            // otherwise go ahead and generate the SQL file

            var generator = new ScriptGenerator();
            var finalFileName = generator.generate(description);
            var dte = Package.GetGlobalService(typeof(DTE)) as DTE2;  
                                      
            dte.ItemOperations.OpenFile(finalFileName);
            dte = null;
            this.Close();

        }

        private void showError(string error)
        {
            lblError.Text = error;
            lblError.Show();
        }
        private void hideError()
        {
            lblError.Text = "";
            lblError.Hide();
        }
    }
}
