using System.Collections.Generic;
using FlagsNet.Filters;
using NUnit.Framework;

namespace FlagsNet.Tests.Filters
{

    public class PathBuilderTest
    {
        [Test]
        [TestCase("name", "flags-net", "$[?(@.name == 'flags-net')]")]
        [TestCase("name__in", "flags-net", "$[?(@.name)].name[?(@ == 'flags-net')]")]
        [TestCase("name__ne", "flags-net", "$[?(@.name != 'flags-net')]")]
        [TestCase("exp__gt", "30", "$[?(@.exp > '30')]")]
        [TestCase("exp__gte", "30", "$[?(@.exp >= '30')]")]
        [TestCase("exp__lt", "30", "$[?(@.exp < '30')]")]
        [TestCase("exp__lte", "30", "$[?(@.exp <= '30')]")]
        public void Test_Should_Parse_Query_To_Json_Path(string op, string value, string result)
        {
            var parameters = new Dictionary<string, string>
            {
                { op , value}
            };

            var jsonPath = PathBuilder.Parse(parameters);

            Assert.AreEqual(result, jsonPath);
        }

        [Test]
        public void Test_Should_Parse_Query_With_Many_Constraints()
        {
            var parameters = new Dictionary<string, string>
            {
                { "name" , "flags-net"},
                { "exp__gt" , "30"},
                { "parameter__in", "path"}
            };

            var jsonPath = PathBuilder.Parse(parameters);
            Assert.AreEqual("$[?(@.name == 'flags-net' && @.exp > '30' && @.parameter)].parameter[?(@ == 'path')]", jsonPath);
        }

        [Test]
        public void Test_Should_Throws_Exception_When_Has_Many_In_Operator()
        {
            var parameters = new Dictionary<string, string>
            {
                { "parameter__in", "path"},
                { "parameter2__in", "path"}
            };

            try
            {
                PathBuilder.Parse(parameters);
                Assert.Fail("PathFilterException should be thrown");
            }
            catch(PathFilterException)
            {
                Assert.Pass();
            }
        }
    }
}