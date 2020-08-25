using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using KProcess.Ksmed.Presentation.Shell;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KProcess.Ksmed.Presentation.Tests
{


    /// <summary>
    ///This is a test class for LocalSettingsTest and is intended
    ///to contain all LocalSettingsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class LocalSettingsTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for Persist
        ///</summary>
        [TestMethod()]
        public void GridPersistanceTest()
        {
            LocalSettings target = new LocalSettings();

            var columns = new GridLength[] { new GridLength(10), new GridLength(.1, GridUnitType.Star) };
            var rows = new GridLength[] { new GridLength(20), new GridLength(.2, GridUnitType.Star) };

            target.GridPersist("TestGridName", columns, rows);

            target.Save();

            target.Reload();

            GridLength[] columns2;
            GridLength[] rows2;

            target.GridTryRetrieve("TestGridName", out columns2, out rows2);

            CollectionAssert.AreEquivalent(columns, columns2);
            CollectionAssert.AreEquivalent(rows, rows2);
        }


        /// <summary>
        ///A test for OrderColumns
        ///</summary>
        [TestMethod()]
        public void OrderColumnsTest()
        {
            LocalSettings target = new LocalSettings();

            // Tests pré-définis

            string[][][] calcs = new string[][][]
            {
                new string[][] { new string[] {"a", "b", "c", "d"}, new string[] {"a", "b", "e", "f"}, new string[] {"a", "b", "c", "d", "e", "f"}},
                new string[][] { new string[] {"a", "b", "c", "d"}, new string[] {"b", "a", "e", "f"}, new string[] {"b", "c", "d", "a", "e", "f"}},
                new string[][] { new string[] {"a", "b", "c", "d"}, new string[] {"d", "a", "c"}, new string[] {"d", "a", "b", "c"}},
                new string[][] { new string[] {"a", "b", "c", "d"}, new string[] {"d", "b", "c"}, new string[] {"a", "d", "b", "c"}},
                new string[][] { new string[] {"a", "b", "c", "d"}, new string[] {"d", "c"}, new string[] {"a", "b", "d", "c"}},
                new string[][] { new string[] {"a", "b", "c", "d", "e"}, new string[] {"f", "d", "c"}, new string[] {"a", "b", "f", "d", "e", "c"}},
                new string[][] { null, new string[] {"f", "d", "c"}, new string[] {"f", "d", "c"}},
            };

            int i = 0;
            foreach (var test in calcs)
            {
                var original = test[0];
                var @new = test[1];

                var expected = test[2];
                var result = target.OrderColumns(original, @new);

                // Vérifie l'input; tous les éléments d'original et new doivent être dans expected 
                if (original != null)
                {
                    CollectionAssert.IsSubsetOf(expected, original.Union(@new).ToArray());
                    CollectionAssert.IsSubsetOf(original.Union(@new).ToArray(), expected);
                }

                CollectionAssert.AreEqual(expected, result, "Index " + i.ToString());

                i++;
            }


            // Tests random

            var random = new Random();

            for (int j = 0; j < 1000; j++)
            {
                var original = new string[random.Next(100)];
                for (int k = 0; k < original.Length; k++)
                {
                    var letter = GetRandomAsciiChar(random);
                    while (original.Contains(letter.ToString()))
                        letter = GetRandomAsciiChar(random);

                    original[k] = letter.ToString();
                }

                var @new = new string[random.Next(100)];
                for (int k = 0; k < @new.Length; k++)
                {
                    var letter = GetRandomAsciiChar(random);
                    while (@new.Contains(letter.ToString()))
                        letter = GetRandomAsciiChar(random);

                    if (!@new.Contains(letter.ToString()))
                        @new[k] = letter.ToString();
                }

                var result = target.OrderColumns(original, @new);

                // Vérifie le résultat; tous les éléments d'original et new doivent être dans expected
                CollectionAssert.IsSubsetOf(result, original.Union(@new).ToArray());
                CollectionAssert.IsSubsetOf(original.Union(@new).ToArray(), result);

                // Les éléments doivent respecter l'ordre du @new
                var lastIndex = -1;
                foreach (var element in @new)
                {
                    var newIndex = result.IndexOf(element);
                    Assert.IsTrue(newIndex > lastIndex);
                    lastIndex = newIndex;
                }
            }

        }


        private char GetRandomAsciiChar(Random random)
        {
            var letterValue = random.Next(255);
            return Encoding.ASCII.GetChars(new byte[] { (byte)letterValue })[0];
        }
    }
}
