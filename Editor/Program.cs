using System;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using Editor.GUI;
using Environment = Editor.MapRepresentation.Environment;
using Editor.ModelRepresentation;

namespace Editor
{
    class Program
    {
        public static EnvironmentEditor EnvironmentEditorForm;

        [STAThread]
        static void Main(string[] args)
        {          

            // FileStream input =
            //     new FileStream(@"C:\Users\Roman\war3editor\Tools\(2)BootyBay.w3m", FileMode.Open);
            // Mpq.FileDescriptor fd = new Mpq.FileDescriptor(input);

            //FileStream output =
            //                new FileStream(@"C:\Users\Roman\Documents\Visual Studio 2015\Projects\Editor\Tools\Output\(listfile)", FileMode.Create);
            //Mpq.ExtractFile(fd, "(listfile)", output);
            //output.Close();

            // uint num = Mpq.ExtractAll(fd, @"C:\Users\Roman\war3editor\Tools\Output2\");
            // Console.WriteLine("Extracted " + num + " files");

            FileStream rock = 
                new FileStream(@"C:\Users\Roman\war3editor\Tools\AshenRock0.mdx", FileMode.Open);
            byte[] rockraw = new byte[rock.Length];
            rock.Read(rockraw, 0, (int)rock.Length);
            ModelX mdx = Parser.Read(rockraw);
            //Console.WriteLine(mdx.ToString(0));

            FileStream envfile =
                new FileStream(@"C:\Users\Roman\war3editor\Tools\Output\war3map.w3e", FileMode.Open);
            byte[] envraw = new byte[envfile.Length];
            envfile.Read(envraw, 0, (int)envfile.Length);
            Environment env = Environment.Read(envraw);
            envfile.Close();

            //FileStream newenv =
            //    new FileStream(@"C:\Users\Roman\Documents\Visual Studio 2015\Projects\Editor\Tools\Output\new.w3e", FileMode.Create);
            //env.Write(newenv);


            Application.EnableVisualStyles();
            Application.CurrentCulture = CultureInfo.InvariantCulture;
            
            /*EnvironmentEditorForm = new EnvironmentEditor(env);
            EnvironmentEditorForm.FormClosed += (e, s) => 
                { 
                    Console.WriteLine("closed");
                    WinapiAccess.Suspend();
                    Application.Exit();
                };*/

            TestOpenGLForm form = new TestOpenGLForm();
            form.FormClosed += (e, s) =>
                {
                    WinapiAccess.Suspend();
                    Application.Exit();
                };


            WinapiAccess.Activate();
            Application.Run(form);
        }
    }
}
