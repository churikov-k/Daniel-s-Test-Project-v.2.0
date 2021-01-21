using System;
using NUnit.Framework;
using TestApp.Models;

namespace Tests
{
    [TestFixture]
    public class QuarterTests
    {
        [Test]
        [TestCase("Q1_09")]
        [TestCase("Q2_09")]
        [TestCase("Q3_09")]
        [TestCase("Q4_09")]
        [TestCase("Q1_12")]
        public void CtorIsValidTest(string shorthand)
        {
            Assert.True(new Quarter(shorthand).IsValid);
        }
        
        [Test]
        [TestCase("")]
        [TestCase("Q")]
        [TestCase("Q5_09")]
        [TestCase("Q4_")]
        [TestCase("1_12")]
        public void CtorIsNotValidTest(string shorthand)
        {
            Assert.False(new Quarter(shorthand).IsValid);
        }
        
        [Test]
        [TestCase(2014, 1, 1)]
        [TestCase(2014, 3, 31)]
        [TestCase(2009, 4, 1)]
        [TestCase(2009, 6, 30)]
        [TestCase(2020, 7, 1)]
        [TestCase(2020, 9, 30)]
        [TestCase(2022, 10, 1)]
        [TestCase(2022, 12, 31)]
        public void FromDateIsValidTest(int year, int month, int day)
        {
            var date = new DateTime(year,month,day);
            Assert.True(Quarter.FromDate(date).IsValid);
        }
        
        [Test]
        [TestCase(2014, 1, 1, ExpectedResult = "Q1_14")]
        [TestCase(2014, 3, 31, ExpectedResult = "Q1_14")]
        [TestCase(2009, 4, 1, ExpectedResult = "Q2_09")]
        [TestCase(2009, 6, 30, ExpectedResult = "Q2_09")]
        [TestCase(2020, 7, 1, ExpectedResult = "Q3_20")]
        [TestCase(2020, 9, 30, ExpectedResult = "Q3_20")]
        [TestCase(2022, 10, 1, ExpectedResult = "Q4_22")]
        [TestCase(2022, 12, 31, ExpectedResult = "Q4_22")]
        public string FromDateShorthandTest(int year, int month, int day)
        {
            var date = new DateTime(year,month,day);
            return Quarter.FromDate(date).ToString();
        }
    }
}