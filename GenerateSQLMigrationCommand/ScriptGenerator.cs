using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerateSQLMigrationCommand
{
    class ScriptGenerator
    {
        private string sqlDirectory = @"c:\opt\whatclinic\src\iis\db\src\main\sql";
        private string gitPath = @"c:\Program Files (x86)\Git\bin\git.exe";

        public ScriptGenerator()
        {

        }

        public string generate(string description)
        { 
           
            if (!isGitAvailable())
            {
                Console.WriteLine("git is not available to this plugin so cannot continue");
                return null;
            }
            
            var fileName = generateFileName(description);
            var fullFileName = sqlDirectory + @"\" + fileName;

            
            List<string> content = new List<string>();

            content.Add("/*");
            content.Add("  author    : " + getGitUserName());
            content.Add("  filename  : " + fileName);
            content.Add("  date      : " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            content.Add("*/");
            content.Add(" ");
            content.Add("GO");
            content.Add("");
            content.Add("          /* your sql here */");
            content.Add("");
            content.Add("GO");

            System.IO.File.WriteAllLines(fullFileName, content.ToArray());

            runGitCommand("git add \"" + fullFileName + "\"");

            return fullFileName;
        }



        private string generateFileName(string description = "your description here")
        {
            string fileName = "";
            var now = DateTime.Now;

            fileName += now.ToString("yyyyMMddHHmmss") + " - " + getGitUserName() + " - " + description + ".sql";
            return fileName;
        }

        private string getGitUserName()
        {
            var res = runGitCommand("git config user.name");
            return res.Trim().ToLower().Split(' ')[0];

        }

        private Boolean isGitAvailable()
        {
            return (runGitCommand(" --version").StartsWith("git version"));

        }
        private string runGitCommand(string commandToExec)
        {
            commandToExec = commandToExec.Replace("git ", "");

            return execCommand(gitPath, sqlDirectory, commandToExec);
        }
        private string runCommand(string commandToExec)
        {
            commandToExec = "/C " + commandToExec;
            return execCommand(@"C:\Windows\System32\cmd.exe", @"C:\Windows\System32", commandToExec);
        }

        private string execCommand(string executablePath, string workingDir, string command)
        {

            // Start the child process.
            Process p = new Process();
            var startInfo = p.StartInfo;
            startInfo.WorkingDirectory = workingDir;

            startInfo.FileName = executablePath;
            startInfo.Verb = "runas";
            startInfo.Arguments = command;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            // Redirect the output stream of the child process.
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;

            p.Start();
            // Do not wait for the child process to exit before
            // reading to the end of its redirected stream.
            // p.WaitForExit();
            // Read the output stream first and then wait.
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            return output;

        }


    }
}
