using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using PerformanceTester.Types;

namespace PerformanceTester.Tests
{
    [TestFixture]
    public class NumberRangeTests
    {
        [TestCase("1", NumberRangeOptions.None, 1)]
        [TestCase(" 1 ", NumberRangeOptions.None, 1)]
        [TestCase("", NumberRangeOptions.None)]
        [TestCase("1,2", NumberRangeOptions.None, 1, 2)]
        [TestCase("1, 2", NumberRangeOptions.None, 1, 2)]
        [TestCase("1-2", NumberRangeOptions.None, 1, 2)]
        [TestCase("1 - 2", NumberRangeOptions.None, 1, 2)]
        [TestCase("1 - 2, 4", NumberRangeOptions.None, 1, 2, 4)]
        [TestCase("1-2, 4", NumberRangeOptions.None, 1, 2, 4)]
        [TestCase("1-3, 2-4", NumberRangeOptions.None, 1, 2, 3, 2, 3, 4)]
        [TestCase("2-4, 1-3", NumberRangeOptions.None, 2, 3, 4, 1, 2, 3)]

        [TestCase("1-3, 2-4", NumberRangeOptions.RemoveDuplicates, 1, 2, 3, 4)]
        [TestCase("1, 2, 2, 3, 3, 3", NumberRangeOptions.RemoveDuplicates, 1, 2, 3)]

        [TestCase("2-4, 1-3", NumberRangeOptions.OrderAscending, 1, 2, 2, 3, 3, 4)]
        [TestCase("4, 3, 2, 1", NumberRangeOptions.OrderAscending, 1, 2, 3, 4)]


        [TestCase("2-4, 1-3", NumberRangeOptions.OrderDescending, 4, 3, 3, 2, 2, 1)]
        [TestCase("4, 3, 2, 1", NumberRangeOptions.OrderDescending, 4, 3, 2, 1)]

        [TestCase("2-4, 1-3", NumberRangeOptions.OrderAscending | NumberRangeOptions.RemoveDuplicates, 1, 2, 3, 4)]
        public void GetInts_GiveAValidValue_ShouldReturnTheExpectedSetOfInts(string definition, NumberRangeOptions options, params int[] expectedSet)
        {
            var actualSet = NumberRange.ToInts(definition, options);
            CheckActualIntSet(expectedSet, actualSet);
        }

        private void CheckActualIntSet(int[] expectedSet, IEnumerable<int> actualSet)
        {
            var actualSetArray = actualSet.ToArray();

            Assert.AreEqual(expectedSet.Length, actualSetArray.Length);

            for (int i = 0; i < expectedSet.Length; i++)
            {
                Assert.AreEqual(expectedSet[i], actualSetArray[i]);
            }
        }
    }
}

