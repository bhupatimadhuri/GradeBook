using System;
using Xunit;

namespace GradeBook.Test
{
    public class BookTest
    {
        /// <summary>
        /// Test method
        /// </summary>
        [Fact]
        public void DiskBookCalculatesAnAverageGrade()
        {
            //arrange
            var book = new DiskBook("ClassGrades\\ClassA.csv");
            book.AddGrade();
            
            // act
            var result = book.GetStatistics();

            // assert
            Assert.Equal(79.44444444444, result.Average, 1);
            Assert.Equal(9, result.originalStudents.Count);
            Assert.Single(result.discardedStudents);
            Assert.Equal(99, result.High);
            Assert.Equal(52, result.Low);
            Assert.All(result.discardedStudents, item => Assert.Contains("Edith Adkins", item.Name));
        }

        /// <summary>
        /// Test method to verify if Book Name is being set 
        /// </summary>
        [Fact]
        public void FileNameEqual()
        {
            var book = new DiskBook("ClassGrades\\ClassA.csv");
            Assert.Equal("ClassGrades\\ClassA.csv", book.Name);
        }


    }
}
