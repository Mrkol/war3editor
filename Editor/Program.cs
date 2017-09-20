using System;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using Editor.GUI;
using Environment = Editor.MapRepresentation.Environment;

namespace Editor
{
    class Program
    {
        public static EnvironmentEditor EnvironmentEditorForm;

        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.CurrentCulture = CultureInfo.InvariantCulture;
            

            // FileStream input =
            //     new FileStream(@"C:\Users\Roman\war3editor\Tools\(2)BootyBay.w3m", FileMode.Open);
            // Mpq.FileDescriptor fd = new Mpq.FileDescriptor(input);

            //FileStream output =
            //                new FileStream(@"C:\Users\Roman\Documents\Visual Studio 2015\Projects\Editor\Tools\Output\(listfile)", FileMode.Create);
            //Mpq.ExtractFile(fd, "(listfile)", output);
            //output.Close();

            // uint num = Mpq.ExtractAll(fd, @"C:\Users\Roman\war3editor\Tools\Output2\");
            // Console.WriteLine("Extracted " + num + " files");

            FileStream envfile =
                new FileStream(@"C:\Users\Roman\war3editor\Tools\Output\war3map.w3e", FileMode.Open);
            byte[] envraw = new byte[envfile.Length];
            envfile.Read(envraw, 0, (int)envfile.Length);
            Environment env = Environment.Read(envraw);
            envfile.Close();

            //FileStream newenv =
            //    new FileStream(@"C:\Users\Roman\Documents\Visual Studio 2015\Projects\Editor\Tools\Output\new.w3e", FileMode.Create);
            //env.Write(newenv);
            EnvironmentEditorForm = new EnvironmentEditor(env);

            WinapiAccess.Activate();
            Application.Run(EnvironmentEditorForm);
            WinapiAccess.Suspend();

            Console.ReadLine();
        }
    }
}
