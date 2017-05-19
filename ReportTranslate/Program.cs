using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ReportTranslate
{
    class Program
    {
        static System.Collections.Specialized.StringCollection log = new System.Collections.Specialized.StringCollection();

        static void Main(string[] args)
        {
            DirectoryInfo carpeta = new DirectoryInfo(@"C:\Users\gparedes\Desktop\Escritorio 07-2016\Reportes Formulacion\Formulacion 2017vf");
            DirectoryInfo newCarpeta = new DirectoryInfo(@"C:\Users\gparedes\Desktop\Escritorio 07-2016\Reportes Formulacion\Formulacion 2017vf");

            XmlHelper _helper = new XmlHelper();

            if (!newCarpeta.Exists)
            {
                newCarpeta.Create();
            }

            //Now Create all of the directories
            //foreach (string dirPath in Directory.GetDirectories(carpeta.FullName, "*",
            //    SearchOption.AllDirectories))
            //    Directory.CreateDirectory(dirPath.Replace(carpeta.FullName, newCarpeta.FullName));

            //Translate Oracle to SQL
            foreach (string newPath in Directory.GetFiles(carpeta.FullName, "*.rdl*", SearchOption.AllDirectories)) {
                _helper.TranslateReportOracleToSql(newPath, newPath.Replace(carpeta.FullName, newCarpeta.FullName));
                _helper.ReplaceDatasource(newPath);
                _helper.ReplaceServer(newPath);
            }



            //Replace DataSource
            //foreach (string newPath in Directory.GetFiles(newCarpeta.FullName, "*.rdl*", SearchOption.AllDirectories))
            //    _helper.ReplaceDatasource(newPath);

            Console.ReadKey();
        }



        static void WalkDirectoryTree(System.IO.DirectoryInfo root, System.IO.DirectoryInfo newRoot)
        {
            System.IO.FileInfo[] files = null;
            System.IO.DirectoryInfo[] subDirs = null;

            // First, process all the files directly under this folder
            try
            {
                XmlHelper _helper = new XmlHelper();
                
                files = root.GetFiles("*.rdl");

                foreach (var archivo in files)
                {
                    _helper.TranslateReportOracleToSql(archivo.FullName, "");
                    //Console.WriteLine(string.Format("Archivo:{0} \nDirectorio:{1} \nFullName:{2}", archivo.Name, archivo.DirectoryName, archivo.FullName));
                }
            }
            // This is thrown if even one of the files requires permissions greater
            // than the application provides.
            catch (UnauthorizedAccessException e)
            {
                // This code just writes out the message and continues to recurse.
                // You may decide to do something different here. For example, you
                // can try to elevate your privileges and access the file again.
                log.Add(e.Message);
            }

            catch (System.IO.DirectoryNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }

            if (files != null)
            {
                //foreach (System.IO.FileInfo fi in files)
                //{
                //    // In this example, we only access the existing FileInfo object. If we
                //    // want to open, delete or modify the file, then
                //    // a try-catch block is required here to handle the case
                //    // where the file has been deleted since the call to TraverseTree().
                //    Console.WriteLine(fi.FullName);
                //}

                // Now find all the subdirectories under this directory.
                subDirs = root.GetDirectories();

                foreach (System.IO.DirectoryInfo dirInfo in subDirs)
                {
                    // Resursive call for each subdirectory.
                  //  WalkDirectoryTree(dirInfo);
                }
            }
        }




    }


}
