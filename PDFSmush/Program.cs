using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PDFSmush
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            string type = args[0];
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NTQwOTA1QDMxMzkyZTMzMmUzMEV3aEhrWXB6T0w5SjlkUXpwRnJvQWkvbGFqZyszK3BsRFNOdHpCUXpsbjA9;NTQwOTA2QDMxMzkyZTMzMmUzMEtUN21LcE50aDY4YlAwNWRNbHhWT1dvcS9NVkllbkJQOWZjZWVjR0NnNFU9;NTQwOTA3QDMxMzkyZTMzMmUzMGlPSnErcHFrb1ZHMHVGbktWZkdIVUQ2bzJwbit3VGRSUEpQSCtDNDlLSk09;NTQwOTA4QDMxMzkyZTMzMmUzMGt4cnZDNFZrVVY2bU1YSGE4QmRGdm5OQkRlQzZ5NU5HNWFPQUo1NVVMRzA9;NTQwOTA5QDMxMzkyZTMzMmUzMEZiTzR5eDYwNnBJSXhoMDA2MG96SmpkR3pnd3dST1BFZXR2REVlZms4anc9;NTQwOTEwQDMxMzkyZTMzMmUzMEsrS2xTUUw5NVhhTHhlQldXWHduaDV1MDNxc3NKRUZzSGM4dURWb2lBTmM9");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            switch (type)
            {
                case "Full":
                    Application.Run(new FullSmush(args));
                    break;
                case "Simple":
                    Application.Run(new SimpleSmush(args));
                    break;
                case "Cover":
                    Application.Run(new CoverGenerator(args));
                    break;
                default:
                    Application.Run(new FullSmush(args));
                    break;
                        
            }
            
        }
    }
}