using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.Activities.IntegrityChecker.Interfaces;
using SFA.DAS.Activities.IntegrityChecker.Utils;

namespace SFA.DAS.Activities.UnitTests.IntegrityChecker.Utils
{
    [TestFixture]
    public class ZipperTests
    {
        [Test]
        public void Zip_SingleIdenticalSegment_ShouldReturnSameSequence()
        {
            var fixtures = new ZipperTestFixtures<int>()
                .WithASegment(1, 2, 3, 4, 5)
                .WithBSegment(1, 2, 3, 4, 5)
                .AssertExpectedItemCount(5)
                .AssertExpectedSequence(1, 2, 3, 4, 5)
                .AssertExpectedInA(true, true, true, true, true)
                .AssertExpectedInB(true, true, true, true, true);
        }

        [Test]
        public void Zip_SingleSegmentAIsSupersetOfSegmentB_ShouldReturnSameSequence()
        {
            var fixtures = new ZipperTestFixtures<int>()
                .WithASegment(1, 2, 3, 4, 5)
                .WithBSegment(1, 2, 3)
                .AssertExpectedItemCount(5)
                .AssertExpectedSequence(1, 2, 3, 4, 5)
                .AssertExpectedInA(true, true, true, true, true)
                .AssertExpectedInB(true, true, true, false, false);
        }

        [Test]
        public void Zip_SingleSegmentAIsSubsetOfSegmentB_ShouldReturnSameSequence()
        {
            var fixtures = new ZipperTestFixtures<int>()
                .WithASegment(1, 2, 3)
                .WithBSegment(1, 2, 3, 4, 5)
                .AssertExpectedItemCount(5)
                .AssertExpectedSequence(1, 2, 3, 4, 5)
                .AssertExpectedInA(true, true, true, false, false)
                .AssertExpectedInB(true, true, true, true, true);
        }

        [Test]
        public void Zip_SingleSegmentANoIntersectionWithSegmentB_ShouldReturnUnionSequence()
        {
            var fixtures = new ZipperTestFixtures<int>()
                .WithASegment(1, 2, 3, 4, 5)
                .WithBSegment(6, 7, 8, 9, 10)
                .AssertExpectedItemCount(10)
                .AssertExpectedSequence(1, 2, 3, 4, 5, 6, 7, 8, 9, 10)
                .AssertExpectedInA(true, true, true, true, true, false, false, false, false, false)
                .AssertExpectedInB(false, false, false, false, false, true, true, true, true, true);
        }

        [Test]
        public void Zip_SingleSegmentASmallIntersectionWithSegmentB_ShouldReturnUnionSequence()
        {
            var fixtures = new ZipperTestFixtures<int>()
                .WithASegment(1, 2, 3, 4, 5)
                .WithBSegment(4, 5, 6, 7, 8)
                .AssertExpectedItemCount(8)
                .AssertExpectedSequence(1, 2, 3, 4, 5 ,6 ,7 ,8)
                .AssertExpectedInA(true, true, true, true, true, false, false, false)
                .AssertExpectedInB(false, false, false, true, true, true, true, true);
        }

        [Test]
        public void Zip_SingleEmptySegmentWithSegmentB_ShouldReturnUnionSequence()
        {
            var fixtures = new ZipperTestFixtures<int>()
                .WithASegment()
                .WithBSegment(1, 2, 3, 4, 5)
                .AssertExpectedItemCount(5)
                .AssertExpectedSequence(1, 2, 3, 4, 5)
                .AssertExpectedInA(false, false, false, false, false)
                .AssertExpectedInB(true, true, true, true, true);
        }

        [Test]
        public void Zip_MultiSegmentAIdenticalToSingleSegmentB_ShouldReturnSameSequence()
        {
            var fixtures = new ZipperTestFixtures<int>()
                .WithASegment(1, 2, 3)
                .WithASegment(4, 5)
                .WithBSegment(1, 2, 3, 4, 5)
                .AssertExpectedItemCount(5)
                .AssertExpectedSequence(1, 2, 3, 4, 5)
                .AssertExpectedInA(true, true, true, true, true)
                .AssertExpectedInB(true, true, true, true, true);
        }

        [Test]
        public void Zip_MultiSegmentAIdenticalToMultiSegmentB_ShouldReturnSameSequence()
        {
            var fixtures = new ZipperTestFixtures<int>()
                .WithASegment(1, 2, 3)
                .WithASegment(4, 5)
                .WithBSegment(1, 2, 3)
                .WithBSegment(4, 5)
                .AssertExpectedItemCount(5)
                .AssertExpectedSequence(1, 2, 3, 4, 5)
                .AssertExpectedInA(true, true, true, true, true)
                .AssertExpectedInB(true, true, true, true, true);
        }

        [Test]
        public void Zip_MultiSegmentAInterspersedWithMultiSegmentBPatternB_ShouldReturnSameSequence()
        {
            var fixtures = new ZipperTestFixtures<int>()
                .WithASegment(1, 5, 10)
                .WithASegment(15, 20)
                .WithBSegment(2, 4, 6)
                .WithBSegment(8, 10)
                .AssertExpectedItemCount(9)
                .AssertExpectedSequence(1, 2, 4, 5, 6, 8, 10, 15, 20)
                .AssertExpectedInA(true, false, false, true, false, false, true, true, true)
                .AssertExpectedInB(false, true, true, false, true, true, true, false, false);
        }
    }

    public class ZipperTestFixtures<T>
    {
        private readonly Lazy<ZipperItem<T>[]> _actualResults;

        public ZipperTestFixtures()
        {
            _actualResults = new Lazy<ZipperItem<T>[]>(InitialiseResults);
        }

        public List<T[]> ASegments { get; } = new List<T[]>();
        public List<T[]> BSegments { get; } = new List<T[]>();

        public ZipperTestFixtures<T> WithASegment(params T[] segment)
        {
            ASegments.Add(segment);
            return this;
        }

        public ZipperTestFixtures<T> WithBSegment(params T[] segment)
        {
            BSegments.Add(segment.ToArray());
            return this;
        }

        public IEnumerable<T> GetASegments()
        {
            return GetSegment(ASegments);
        }

        public IEnumerable<T> GetBSegments()
        {
            return GetSegment(BSegments);
        }

        public IEnumerable<T> GetSegment(List<T[]> segments)
        {
            if (segments.Count == 0)
            {
                return null;
            }

            var result = segments[0];
            segments.RemoveAt(0);
            return result;
        }

        private ZipperItem<T>[] InitialiseResults()
        {
            var actualItems = Zipper.Zip(GetASegments, GetBSegments).ToArray();
            return actualItems;
        }

        public ZipperTestFixtures<T> AssertExpectedItemCount(int expectedItemCount)
        {
            Assert.AreEqual(expectedItemCount, _actualResults.Value.Length);
            return this;
        }

        public ZipperTestFixtures<T> AssertExpectedSequence(params T[] expectedItems)
        {
            for(int i = 0; i < expectedItems.Length; i++)
            {
                var expectedItem = expectedItems[i];
                var actualItem = _actualResults.Value[i];
                Assert.AreEqual(expectedItem, actualItem.Item);
            }
            return this;
        }

        public ZipperTestFixtures<T> AssertExpectedInA(params bool[] expectedInAValues)
        {
            for (int i = 0; i < expectedInAValues.Length; i++)
            {
                var expectedInA = expectedInAValues[i];
                var actualItem = _actualResults.Value[i];
                Assert.AreEqual(expectedInA, actualItem.IsInA);
            }
            return this;
        }

        public ZipperTestFixtures<T> AssertExpectedInB(params bool[] expectedInBValues)
        {
            for (int i = 0; i < expectedInBValues.Length; i++)
            {
                var expectedInB = expectedInBValues[i];
                var actualItem = _actualResults.Value[i];
                Assert.AreEqual(expectedInB, actualItem.IsInB);
            }
            return this;
        }
    }
}
