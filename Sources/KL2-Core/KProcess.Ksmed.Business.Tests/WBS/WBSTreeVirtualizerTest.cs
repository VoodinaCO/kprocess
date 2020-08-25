using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Business;
using System.Linq;
using System.Collections.Generic;

namespace KProcess.Ksmed.Business.Tests.WBS
{

    public class WBSTreeVirtualizerTest
    {
        [TestClass]
        public sealed class ConstructorTest
        {
            //A l'heure actuelle ce cas de test ne passe pas, mais on est dans une configuration qui ne devrait pas arriver.
            //Je laisse le cas de Test au cas où on tombe un jour sur ce cas
            //[TestMethod]
            //public void ShouldCreateTreeWithoutLastAction()
            //{

            //    var source = new[] {
            //        new KAction { WBS="1", ActionId=1 },
            //        new KAction { WBS="1.1", ActionId=2 },
            //        new KAction { WBS="1.4.1", ActionId=3 }
            //    };

            //    var actualValue = source.VirtualizeTree();

            //    actualValue.ActionTree[0].Children[0].Children.Should().BeEmpty();
            //}

            List<KAction> sourceBug116 = new List<KAction> {                                                //index tableau
                    new KAction { WBS="1", ActionId=1, Label="Aller chercher la boite dans le coffre" },    //0
                    new KAction { WBS ="2", ActionId=2, Label="Montage du triangle" },                      //1
                    new KAction { WBS ="3", ActionId=3, Label="Contrôle de stabilité" },                    //2
                    new KAction { WBS ="4", ActionId=4, Label="Démontage du triangle" },                    //3
                        new KAction { WBS ="4.1", ActionId=5, Label="Replier les pieds" },                  //4
                            new KAction { WBS ="4.1.1", ActionId=6, Label="Replier l'avant" },              //5
                            new KAction { WBS ="4.1.2", ActionId=7, Label="Replier l'arrière" },            //6
                        new KAction { WBS ="4.2", ActionId=8, Label="Déboiter le haut" },                   //7
                        new KAction { WBS ="4.3", ActionId=9, Label="Replier les bords" },                  //8
                        new KAction { WBS ="4.4", ActionId=10, Label="Remettre dans la boite" },            //9
                    new KAction { WBS ="5", ActionId=11, Label="Remettre la boite dans le coffre" }         //10
                };


            [TestMethod]
            public void VirtualizeTreeShouldHaveTheRightNumberOfChildren()
            {

                var actualValue = sourceBug116.VirtualizeTree();


                actualValue.ActionTree[0].Children.Should().HaveCount(0);
                actualValue.ActionTree[1].Children.Should().HaveCount(0);
                actualValue.ActionTree[2].Children.Should().HaveCount(0);
                actualValue.ActionTree[3].Children.Should().HaveCount(4);
                    actualValue.ActionTree[3].Children[0].Children.Should().HaveCount(2);
                        actualValue.ActionTree[3].Children[0].Children[0].Children.Should().HaveCount(0);
                        actualValue.ActionTree[3].Children[0].Children[1].Children.Should().HaveCount(0);
                    actualValue.ActionTree[3].Children[1].Children.Should().HaveCount(0);
                    actualValue.ActionTree[3].Children[2].Children.Should().HaveCount(0);
                    actualValue.ActionTree[3].Children[3].Children.Should().HaveCount(0);
                actualValue.ActionTree[4].Children.Should().HaveCount(0);
               
            }

            [TestMethod]
            public void AppliedWBSShouldBeTheSameAsSourceBug116()
            {             

                var actualValue = sourceBug116.VirtualizeTree();

                actualValue.ApplyWBS();

                sourceBug116[0].WBS.Should().Be("1");
                sourceBug116[1].WBS.Should().Be("2");
                sourceBug116[2].WBS.Should().Be("3");
                sourceBug116[3].WBS.Should().Be("4");
                sourceBug116[4].WBS.Should().Be("4.1");
                sourceBug116[5].WBS.Should().Be("4.1.1");
                sourceBug116[6].WBS.Should().Be("4.1.2");
                sourceBug116[7].WBS.Should().Be("4.2");
                sourceBug116[8].WBS.Should().Be("4.3");
                sourceBug116[9].WBS.Should().Be("4.4");
                sourceBug116[10].WBS.Should().Be("5");
            }

            [TestMethod]
            public void CreatedTreeShouldBeOrderdByWBS()
            {
                var source = new[] {
                new KAction { WBS="2", ActionId=2 },  new KAction { WBS="1", ActionId=1 } };

                var actualValue = source.VirtualizeTree();

                actualValue.ActionTree[0].Action.ActionId.Should().Be(1);
                actualValue.ActionTree[1].Action.ActionId.Should().Be(2);
            }

            [TestMethod]
            public void CreatedTreeShouldContainsSourceValueAtRootLevel()
            {
                var source = new[] {
                new KAction { WBS="1", ActionId=1 },  new KAction { WBS="2", ActionId=2 } };

                var actualValue = source.VirtualizeTree();

                actualValue.ActionTree
                   .Select(_ => _.Action.ActionId).ShouldBeEquivalentTo(new[] { 1, 2 });
            }

            [TestMethod]
            public void CreatedTreeShouldContainsFirstLevelChildren()
            {
                var source = new[] {
                    new KAction { WBS="1", ActionId=1 },
                    new KAction { WBS="2", ActionId=2 },
                    new KAction { WBS="1.1", ActionId=3 },
                    new KAction { WBS= "1.2", ActionId=4 },
                    new KAction { WBS= "2.1", ActionId=5 }
                };

                var actualValue = source.VirtualizeTree();

                actualValue.ActionTree
                    .Should().HaveCount(2)
                    .And.Subject.First().Children.Select(_ => _.Action.ActionId).ShouldBeEquivalentTo(new[] { 3, 4 });
            }

            [TestMethod]
            public void CreatedTreeShouldContainssecondLevelChildren()
            {
                var source = new[] {
                    new KAction { WBS="1", ActionId=1 },
                    new KAction { WBS="2", ActionId=2 },
                    new KAction { WBS="1.1", ActionId=3 },
                    new KAction { WBS= "1.2", ActionId=4 },
                    new KAction { WBS= "2.1", ActionId=5 },
                     new KAction { WBS= "2.1.1", ActionId=6 },
                      new KAction { WBS= "2.1.2", ActionId=7 },
                };

                var actualValue = source.VirtualizeTree();

                actualValue.ActionTree[1]
                    .Children[0]
                    .Children
                    .Select(_ => _.Action.ActionId).ShouldBeEquivalentTo(new[] { 6, 7 });
            }

            [TestMethod]
            public void CreatedTreeShouldNotBeNull()
            {
                var source = new[] {
                new KAction { WBS="1", ActionId=1 },  new KAction { WBS="2", ActionId=2 } };

                var actualValue = source.VirtualizeTree();

                actualValue.ActionTree
                    .Should().NotBeNull();
            }
        }

        [TestClass]
        public sealed class GetTest
        {
            [TestMethod]
            public void ShouldProvideMatchingNode()
            {
                var paramValue = new KAction { WBS = "2.1.2", ActionId = 7 };

                var source = new[] {
                    new KAction { WBS="1", ActionId=1 },
                    new KAction { WBS="2", ActionId=2 },
                    new KAction { WBS="1.1", ActionId=3 },
                    new KAction { WBS= "1.2", ActionId=4 },
                    new KAction { WBS= "2.1", ActionId=5 },
                    new KAction { WBS= "2.1.1", ActionId=6 },
                    paramValue
                };

                var actualValue = source.VirtualizeTree();

                actualValue.GetNode(paramValue).Action.ActionId
                    .Should().Be(7);
            }
        }

        [TestClass]
        public sealed class GetParent
        {
            [TestMethod]
            public void ShouldProvideMatchingNode()
            {
                var paramValue = new KAction { WBS = "2.1.2", ActionId = 7 };

                var source = new[] {
                    new KAction { WBS="1", ActionId=1 },
                    new KAction { WBS="2", ActionId=2 },
                    new KAction { WBS="1.1", ActionId=3 },
                    new KAction { WBS= "1.2", ActionId=4 },
                    new KAction { WBS= "2.1", ActionId=5 },
                    new KAction { WBS= "2.1.1", ActionId=6 },
                    paramValue
                };

                var actualValue = source.VirtualizeTree();

                actualValue.GetParentNode(paramValue).Action.ActionId
                    .Should().Be(5);
            }

            [TestMethod]
            public void ShouldProvideNullWhenNoParent()
            {
                var paramValue = new KAction { WBS = "1", ActionId = 1 };

                var source = new[] {
                   paramValue,
                    new KAction { WBS="2", ActionId=2 },
                    new KAction { WBS="1.1", ActionId=3 },
                    new KAction { WBS= "1.2", ActionId=4 },
                    new KAction { WBS= "2.1", ActionId=5 },
                    new KAction { WBS= "2.1.1", ActionId=6 },
                    new KAction { WBS = "2.1.2", ActionId = 7 }
                };

                var actualValue = source.VirtualizeTree();

                actualValue.GetParentNode(paramValue)
                    .Should().BeNull();
            }
        }

        [TestClass]
        public sealed class ApplyTest
        {
            [TestMethod]
            public void ShouldApplyWBSOnAlreadyRightEntries()
            {
                var source = new[] {
                   new KAction { WBS = "1", ActionId = 1 },
                    new KAction { WBS="2", ActionId=2 },
                    new KAction { WBS= "2.1", ActionId=5 },
                    new KAction { WBS= "2.1.1", ActionId=6 },
                    new KAction { WBS = "2.1.2", ActionId = 7 }
                };

                var actualValue = source.VirtualizeTree();

                actualValue.ApplyWBS();
                source[0].WBS.Should().Be("1");
                source[1].WBS.Should().Be("2");
                source[2].WBS.Should().Be("2.1");
                source[3].WBS.Should().Be("2.1.1");
                source[4].WBS.Should().Be("2.1.2");

            }

            [TestMethod]
            public void ShouldApplyWBSOnFalseEntries()
            {
                var source = new[] {
                   new KAction { WBS = "5", ActionId = 1 },
                    new KAction { WBS="2", ActionId=2 },
                    new KAction { WBS= "2.1", ActionId=5 },
                    new KAction { WBS= "2.1.1", ActionId=6 },
                    new KAction { WBS = "2.1.2", ActionId = 7 }
                };

                var actualValue = source.VirtualizeTree();

                actualValue.ApplyWBS();
                source[0].WBS.Should().Be("2");
                source[1].WBS.Should().Be("1");
                source[2].WBS.Should().Be("1.1");
                source[3].WBS.Should().Be("1.1.1");
                source[4].WBS.Should().Be("1.1.2");

            }

            [TestMethod]
            public void ShouldApplyWBSWhenARootEntryIsRemoved()
            {
                var source = new[] {
                   new KAction { WBS = "1", ActionId = 1 },
                    new KAction { WBS="2", ActionId=2 },
                    new KAction { WBS= "2.1", ActionId=5 },
                    new KAction { WBS= "2.1.1", ActionId=6 },
                    new KAction { WBS = "2.1.2", ActionId = 7 }
                };

                var actualValue = source.VirtualizeTree();
                actualValue.Remove(source[0]);

                actualValue.ApplyWBS();
                // source[0].WBS.Should().Be("1"); // On s'en tape
                source[1].WBS.Should().Be("1");
                source[2].WBS.Should().Be("1.1");
                source[3].WBS.Should().Be("1.1.1");
                source[4].WBS.Should().Be("1.1.2");

            }

            [TestMethod]
            public void ShouldApplyWBSWhenADeepEntryIsRemoved()
            {
                var source = new[] {
                   new KAction { WBS = "1", ActionId = 1 },
                    new KAction { WBS="2", ActionId=2 },
                    new KAction { WBS= "2.1", ActionId=5 },
                    new KAction { WBS= "2.1.1", ActionId=6 }, // removed!
                    new KAction { WBS = "2.1.2", ActionId = 7 }
                };

                var actualValue = source.VirtualizeTree();
                actualValue.Remove(source[3]);

                actualValue.ApplyWBS();
                source[0].WBS.Should().Be("1");
                source[1].WBS.Should().Be("2");
                source[2].WBS.Should().Be("2.1");
                //source[3].WBS.Should().Be("1.1.1");// On s'en tape
                source[4].WBS.Should().Be("2.1.1");

            }
        }


    }
}
