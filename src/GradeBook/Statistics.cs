using System;
using System.Collections.Generic;

namespace GradeBook
{
    public class Statistics
    {
        public List<ClassInfo> originalStudents { get; internal set; }
        public List<ClassInfo> discardedStudents { get; internal set; }
        /// <summary>
        /// Calculate Average
        /// </summary>
        public double Average
        {
            get
            {
                return Math.Round((Sum / Count), 1);
            }
        }

        //Record High and Low score for a class
        public double High;
        public double Low;

        public double Sum;
        public int Count;
        
        /// <summary>
        /// Add each grade to Sum
        /// Increment the Count to calculate average
        /// </summary>
        /// <param name="number"></param>
        /// <param name="name"></param>
        public void Add(double number, string name)
        {
            Sum += number;
            Count += 1;
            Low = Math.Min(number, Low);
            High = Math.Max(number, High);
        }

        public Statistics()
        {
            Count = 0;
            Sum = 0.0;
            High = double.MinValue;
            Low = double.MaxValue;
            originalStudents = new List<ClassInfo>();
            discardedStudents = new List<ClassInfo>();
        }
    }
}
