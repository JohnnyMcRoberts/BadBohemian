namespace TrimLineEndSpaces
{
    using System;
    using System.IO;
    using System.Threading;

    class Program
    {
        static void Main(string[] args)
        {

            // Test if input arguments were supplied:
            if (args.Length == 0)
            {
                Console.WriteLine("Please enter a file to trim.");
                Console.WriteLine("Usage: TrimLineEndSpaces <file> [update]");
                return;
            }

            // Check the file exists.
            string inFile = args[0];
            bool fileExists = File.Exists(inFile);
            if (fileExists == false)
            {
                Console.WriteLine("Please enter a file that exists to trim.");
                Console.WriteLine("Usage: TrimLineEndSpaces <file> [update]");
                return;
            }

            bool reportOnly = args.Length < 2 || args[1] != "update";

            RemoveLineEndSpaces(inFile, reportOnly);
        }

        private static void RemoveLineEndSpaces(string inFileName, bool reportOnly)
        {
            // open the infile to read.
            StreamReader file = new StreamReader(inFileName);

            // create a temp file to write to.
            bool spacesToRemove = false;
            string tempFileName = GetTempFilePathWithExtension(Path.GetExtension(inFileName));
            using (StreamWriter tempFile = new StreamWriter(tempFileName))
            {
                // read the file line by line
                int counter = 1;
                string line;
                while ((line = file.ReadLine()) != null)
                {
                    string cleanLine = line.TrimEnd(' ');

                    if (cleanLine.Length != line.Length)
                    {
                        Console.WriteLine("Removing spaces from line " + counter +
                                          "\n old = \'" + line + "'" +
                                          "\n new = \'" + cleanLine + "'");
                        spacesToRemove = true;
                    }
                    counter++;
                    tempFile.WriteLine(cleanLine);
                }
            }

            file.Close();

            if (spacesToRemove && !reportOnly)
            {
                Console.WriteLine("overwriting with a cleaned version of the file '" + tempFileName + "'");

                // wait until sure both are closed
                while (IsFileLocked(tempFileName) || IsFileLocked(inFileName))
                {
                    Thread.Sleep(100);
                }

                // Copy over the file
                File.Copy(tempFileName, inFileName, true);

            }
            else
            {
                if (!reportOnly)
                    Console.WriteLine("No line endings to fix - leave as is");
            }

            // delete the temp file
            File.Delete(tempFileName);
      
        }

        private static bool IsFileLocked(string fileName)
        {
            FileStream stream = null;
            FileInfo file = new FileInfo(fileName);

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }

        public static string GetTempFilePathWithExtension(string extension)
        {
            string path = Path.GetTempPath();
            string fileName = Guid.NewGuid().ToString() + extension;
            return Path.Combine(path, fileName);
        }
    }
}
