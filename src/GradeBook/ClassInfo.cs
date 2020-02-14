using NLog;
using System;
using System.IO;
using System.Linq;

namespace GradeBook
{
    /// <summary>
    /// Class Info
    /// </summary>
    public class ClassInfo
    {
        public string Name { get; set; }
        public double Grade { get; set; }

    }

    /// <summary>
    /// Name of a book
    /// </summary>

    public class NamedObject
    {
        public NamedObject(string name)
        {
            Name = name;
        }

        public string Name
        {
            get;
            set;
        }
    }
    /// <summary>
    /// An IBook interface defines the capabilities of a book.
    /// </summary>

    public interface IBook
    {
        void AddGrade();
        Statistics GetStatistics();
        string Name { get; }
    }
    /// <summary>
    /// All the Books have to implement IBook interface 
    /// </summary>
    public abstract class Book : NamedObject, IBook
    {
        public Book(string name) : base(name)
        {
        }
        public abstract void AddGrade();
        public abstract Statistics GetStatistics();
    }

    /// <summary>
    /// The derived class DiskBook implements Book. We can have InMemoryBook() which can also implement Book. 
    /// DiskBook implements requirements that are specific to DiskBook, like reading csv file to record grades in memory....  
    /// </summary>
    public class DiskBook : Book
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        Statistics result = new Statistics();

        public DiskBook(string name) : base(name)
        {
        }

        /// <summary>
        /// Grades from the file are stored in memory
        /// </summary>
        public override void AddGrade()
        {
            Logger.Info("Inside AddGrade()");

            using (var reader = File.OpenText(Name))
            {

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (!string.IsNullOrEmpty(line))
                    {
                        if (!line.Contains("Student Name"))
                        {
                            try
                            {
                                var values = line.Split(',');
                                var grade = Math.Floor(double.Parse(values[1]));
                                Logger.Debug($"Actual Grade: {values[1]} ");
                                Logger.Debug($"Grade after truncate: {grade}");

                                if (grade > 0)
                                {
                                    result.originalStudents.Add(new ClassInfo { Grade = grade, Name = values[0] });
                                }
                                else
                                {
                                    Logger.Debug($"Discarded name & grade: {values[0]} {grade}");
                                    result.discardedStudents.Add(new ClassInfo { Grade = grade, Name = values[0] });
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.Error(ex.Message + ex.StackTrace);
                            }
                        }
                    }
                }

            }

        }


        /// <summary>
        /// Calculate the Statistics
        /// </summary>
        /// <returns></returns>
        public override Statistics GetStatistics()
        {
            Logger.Info("Inside GetStatistics()");

            foreach (var item in result.originalStudents)
            {
                result.Add(item.Grade, item.Name);
                Logger.Debug($"Total Grade for now: {result.Sum}");
            }

            Logger.Debug($"Average of class: {result.Average}");

            return result;
        }
    }

}
