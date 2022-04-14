using System.IO;
using NUnit.Framework;
using TestApp.Models;

namespace Tests
{
    [TestFixture]
    public class ProcessDataTests
    {
        // Data in files inputData1.csv and expectedResult1.csv
        // has been taken from Daniel's task description
        // DO NOT SAVE these files via Excel! By default, it removes ending zeros! 
        private const string OutputFileName1 = "test.csv";
        private const string OutputFileName2 = "test2.csv";
        private const string InputFileName1 = @"..\..\..\FileFixtures\inputData1.csv";
        private const string InputFileName2 = @"..\..\..\FileFixtures\inputData2.csv";
        private const string ExpectedResultFileName = @"..\..\..\FileFixtures\expectedResult1.csv";
        [SetUp]
        public void Init()
        {
            var data = new QuoteData();
            data.ProcessData(InputFileName1, OutputFileName1);
        }

        [Test]
        public void ProcessDataResult()
        {
            FileAssert.AreEqual( ExpectedResultFileName, OutputFileName1 );
        }
        
        [Test]
        public void ProcessFailedDataResult()
        {
            var data = new QuoteData();

            Assert.Throws<InvalidDataException>(() => data.ProcessData(InputFileName2, OutputFileName2));
        }
    }
}