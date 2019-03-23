using System.Collections.Generic;
using NUnit.Framework;

using FlagsNet.Providers;

namespace FlagsNet.Tests
{
    public class ManagerTest
    {
        Manager manager;

        [SetUp]
        public void Setup()
        {
            var flags = new Dictionary<string, string> {
                {"feature.key.enabled", "true"},
                {"feature.key.disabled", "false"},
                {"feature.number", "1"},
                {"feature.list", "1|2|3"},
            };

            manager = new Manager(new MemoryFlagSource(flags));
        }

        [Test]
        public void Test_Manager_Should_Return_Active_Switch()
        {
            var feature = manager.Active("feature.key.enabled");

            Assert.IsTrue(feature);
        }

        [Test]
        public void Test_Manager_Should_Return_Inactive_Switch()
        {
            var feature = manager.Active("feature.key.disabled");

            Assert.IsFalse(feature);
        }

        [Test]
        public void Test_Manager_Should_Return_Active_When_Number_Matches()
        {
            var feature = manager.Active("feature.number", "1");

            Assert.IsTrue(feature);
        }

        [Test]
        public void Test_Manager_Should_Return_Inactive_When_Number_Does_Not_Match()
        {
            var feature = manager.Active("feature.number", "2");

            Assert.IsFalse(feature);
        }

        [Test]
        [TestCase("1")]
        [TestCase("2")]
        [TestCase("3")]
        public void Test_Manager_Should_Return_Active_When_Number_Belongs_To_List(string number)
        {
            var feature = manager.Active("feature.list", number);
            Assert.IsTrue(feature);
        }

        [Test]
        public void Test_Manager_Should_Return_Active_When_At_Least_One_Number_Belongs_To_List()
        {
            var feature = manager.Active("feature.list", "1", "5");
            Assert.IsTrue(feature);
        }

        [Test]
        public void Test_Manager_Should_Return_Inactive_When_Number_Does_Not_Belong_To_List()
        {
            var feature = manager.Active("feature.list", "5");
            Assert.IsFalse(feature);
        }
    }
}