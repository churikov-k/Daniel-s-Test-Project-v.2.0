using NUnit.Framework;
using TestApp;

namespace Tests
{
    [TestFixture]
    public class DoWorkTests
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
            Program.DoWork(InputFileName1, OutputFileName1);
            Program.DoWork(InputFileName2, OutputFileName2);
        }

        [Test]
        public void DoWorkResult()
        {
            FileAssert.AreEqual( ExpectedResultFileName, OutputFileName1 );
            FileAssert.AreEqual( ExpectedResultFileName, OutputFileName2 );
        }
    }
}