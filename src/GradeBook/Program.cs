using System;
using System.IO;
using System.Linq;
using System.Text;
using NLog;

namespace GradeBook
{
    public static class MainClass
    {
        // NLog Nuget package is used to implement logging. The configuration for Nlog can be retrieved from NLog.Config
        // Nlog provides options to Log to multiple targets. Two target options are chosen for this application Console and File
        // For every log statement - Timestamp, Level, MethodName will be recorded
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static void Main(string[] args)
        {
            try
            {
                Logger.Info("Grade calculation start");

                // Local variables declaration

                var directoryName = "Input";
                var filePaths = Directory.GetFiles(directoryName, "*.csv");
                string highPerfClass = string.Empty;
                var highPerfAverage = 0.0;
                var averageOfAllStudents = 0.0;
                StringBuilder outputString = new StringBuilder();

                Logger.Debug("Number of CSV files in given directory: " + filePaths.Length);

                // AddGrade() and GetStatistics() methods will be called for each CSV file in the directory
                // where the actual average calculation is done

                foreach (var file in filePaths)
                {
                    Logger.Info("FilePath: " + file);
                    var fileName = file.ToString().Replace(directoryName + "\\", "").Replace(".csv", "");

                    // Implement an interfact IBook  
                    // The capabilities of the book are in the IBook. This way, reading grade information from a file can be
                    // done from Database or InMemory...
                    // This also implements one of the SOLID principals close for modifications and open for extensions

                    var lines = File.ReadLines(file).ToList();
                    if (lines.Count > 0)
                    {
                        IBook book = new DiskBook(file);

                        book.AddGrade();
                        var stats = book.GetStatistics();

                        // Adding the average for each class to get Highest performing class
                        averageOfAllStudents += stats.Average;
                        if (stats.Average > highPerfAverage)
                        {
                            highPerfAverage = stats.Average;
                            highPerfClass = fileName;
                        }

                        // Appending output string for each class file
                        outputString.Append(FormatOutput(stats, fileName));
                    }
                    else
                    {
                        Logger.Info("Empty file cannot be processed, File name is: " + file);
                    }
                }

                // Writing output to file
                WriteToOutputFile("\n The highest performing class : " + highPerfClass
                                 + $"\n The average score for all students: {(averageOfAllStudents / filePaths.Length):N1}"
                                 + outputString);
            }
            // Centralized exception handling, 
            // Since logic is wrapped inside above try block, exceptions can be caught in the below catch
            catch (Exception ex)
            {
                Logger.Error(ex.Message + ex.StackTrace);
            }
            // Finally block
            finally
            {
                Console.Read();
            }
        }

        /// <summary>
        /// Format the output string
        /// </summary>
        /// <param name="statistics"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>

        public static StringBuilder FormatOutput(Statistics statistics, string fileName)
        {
            StringBuilder outputString = new StringBuilder();
            outputString.Append("\n *********************************************************************************************");
            outputString.Append("\n Class Name: " + fileName) ;
            outputString.Append($"\n Average score of the class: {statistics.Average}");
            outputString.Append($"\n Total number of students within the class: {(statistics.originalStudents.Count + statistics.discardedStudents.Count)}");
            outputString.Append($"\n The number of students used to calculate the class average: {statistics.originalStudents.Count}");
            outputString.Append("\n The names of any students who were discarded from consideration: ");
            if (statistics.discardedStudents.Count > 0)
            {
                foreach (var item in statistics.discardedStudents)
                {
                    outputString.Append($"{item.Name}\n");
                }
            }
            else
            {
                outputString.Append("N/A\n");
            }
            return outputString;
        }

        /// <summary>
        /// This method will verify if output.txt file already exits. If yes, delete it.
        /// And write output text to file.
        /// </summary>
        /// <param name="outputText"></param>
        public static void WriteToOutputFile(string outputText)
        {
            if (File.Exists("output.txt"))
                File.Delete(@"output.txt");

            using (var writer = File.AppendText("output.txt"))
            {
                writer.WriteLine(outputText);
            }
        }
    }
}
