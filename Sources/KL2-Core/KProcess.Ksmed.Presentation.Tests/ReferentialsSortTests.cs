using KProcess.Ksmed.Models;
using KProcess.Ksmed.Presentation.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KProcess.Ksmed.Presentation.Tests
{
    [TestClass]
    public class ReferentialsSortTests
    {

        [TestMethod]
        public void SingleComparisons()
        {
            var comparer = new ReferentialsSort();

            var refNull = new Ref1Project { Label = null };
            var refEmpty = new Ref1Project { Label = string.Empty };
            var refInt3 = new Ref1Project { Label = "3" };
            var refInt200 = new Ref1Project { Label = "200" };
            var refStr_a = new Ref1Project { Label = "abcd" };
            var refStr_s = new Ref1Project { Label = "str" };

            // null > "3"
            Assert.AreEqual(-1, comparer.Compare(refInt3, refNull));
            Assert.AreEqual(1, comparer.Compare(refNull, refInt3));

            // null < "abcd"
            Assert.AreEqual(-1, comparer.Compare(refNull, refStr_a));
            Assert.AreEqual(1, comparer.Compare(refStr_a, refNull));

            // null < ""
            Assert.AreEqual(-1, comparer.Compare(refNull, refEmpty));
            Assert.AreEqual(1, comparer.Compare(refEmpty, refNull));

            // "3" < ""
            Assert.AreEqual(-1, comparer.Compare(refInt3, refEmpty));
            Assert.AreEqual(1, comparer.Compare(refEmpty, refInt3));

            // "" < "abcd"
            Assert.AreEqual(-1, comparer.Compare(refEmpty, refStr_a));
            Assert.AreEqual(1, comparer.Compare(refStr_a, refEmpty));

            // "3" < "200"
            Assert.AreEqual(-1, comparer.Compare(refInt3, refInt200));
            Assert.AreEqual(1, comparer.Compare(refInt200, refInt3));

            // "3" < "abcd"
            Assert.AreEqual(-1, comparer.Compare(refInt3, refStr_a));
            Assert.AreEqual(1, comparer.Compare(refStr_a, refInt3));

            // "abcd" < "str"
            Assert.AreEqual(-1, comparer.Compare(refStr_a, refStr_s));
            Assert.AreEqual(1, comparer.Compare(refStr_s, refStr_a));

            // Egalités
            Assert.AreEqual(0, comparer.Compare(refNull, refNull));
            Assert.AreEqual(0, comparer.Compare(refEmpty, refEmpty));
            Assert.AreEqual(0, comparer.Compare(refInt3, refInt3));
            Assert.AreEqual(0, comparer.Compare(refInt200, refInt200));
            Assert.AreEqual(0, comparer.Compare(refStr_a, refStr_a));
            Assert.AreEqual(0, comparer.Compare(refStr_s, refStr_s));
        }

        [TestMethod]
        public void Sort()
        {
            var comparer = new ReferentialsSort();

            var refNull = new Ref1Project { Label = null };
            var refEmpty = new Ref1Project { Label = string.Empty };
            var refInt1 = new Ref1Project { Label = "1" };
            var refInt3 = new Ref1Project { Label = "3" };
            var refInt200 = new Ref1Project { Label = "200" };
            var refInt1100 = new Ref1Project { Label = "1100" };
            var refStr_a = new Ref1Project { Label = "abcd" };
            var refStr_s = new Ref1Project { Label = "str" };

            var actualArray = new IActionReferential[] { refNull, refEmpty, refStr_a, refInt3, refStr_s, refInt200, refInt1, refInt1100 };
            Array.Sort(actualArray, comparer);

            var expectedArray = new IActionReferential[] { refInt1, refInt3, refInt200, refInt1100, refNull, refEmpty, refStr_a, refStr_s };
            CollectionAssert.AreEqual(expectedArray, actualArray);

        }

    }
}
