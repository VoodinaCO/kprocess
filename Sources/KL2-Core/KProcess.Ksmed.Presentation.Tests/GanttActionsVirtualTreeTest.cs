using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KProcess.Ksmed.Presentation.ViewModels;
using System.Collections.Generic;
using KProcess.Ksmed.Presentation.Shell.ViewModels;
using FluentAssertions;
using System.Linq;

namespace KProcess.Ksmed.Presentation.Tests
{
    [TestClass]
    public class ActionsGraphManipulationTest
    {
        [TestMethod]
        public void ActionsGraphManipulationTestCreation_basicWithTwoItems()
        {
            var items = new List<ActionGridItem>
            {
                new ActionGridItem(new Models.KAction { ActionId = 0 }, 0),
                new ActionGridItem(new Models.KAction { ActionId = 1 }, 0)
            };

            var virtualTree = ActionsGraphManipulator.Virtualize(items);

            virtualTree.Root.Items.Should().HaveCount(2);
            virtualTree.Root.Items.OfType<ActionsGraphManipulator<ActionGridItem>.INodeContainer>().Should().BeEmpty();
        }

        [TestMethod]
        public void ActionsGraphManipulationTestCreation_withGroup_AtTheEnd()
        {
            var items = new List<ActionGridItem>
            {
                new ActionGridItem(new Models.KAction { ActionId = 0 }, 0),
                new ActionGridItem(new Models.KAction { ActionId = 1 }, 0) { IsGroup = true },
                new ActionGridItem(new Models.KAction { ActionId = 2 }, 1),
                new ActionGridItem(new Models.KAction { ActionId = 3 }, 1),
                new ActionGridItem(new Models.KAction { ActionId = 4 }, 1)
            };

            var virtualTree = ActionsGraphManipulator.Virtualize(items);

            virtualTree.Root.Items.Should().HaveCount(2);
            virtualTree.Root.Items[1].Should().BeAssignableTo<ActionsGraphManipulator<ActionGridItem>.INodeContainer>();

            virtualTree.Root.Items[1].As<ActionsGraphManipulator<ActionGridItem>.INodeContainer>().Items.Should().HaveCount(3);
        }

        [TestMethod]
        public void ActionsGraphManipulationTestCreation_withGroup_AtTheBeginning()
        {
            var items = new List<ActionGridItem>
            {
                new ActionGridItem(new Models.KAction { ActionId = 0 }, 0) { IsGroup = true },
                new ActionGridItem(new Models.KAction { ActionId = 1 }, 1),
                new ActionGridItem(new Models.KAction { ActionId = 2 }, 1),
                new ActionGridItem(new Models.KAction { ActionId = 3 }, 0),
                new ActionGridItem(new Models.KAction { ActionId = 4 }, 0)
            };

            var virtualTree = ActionsGraphManipulator.Virtualize(items);

            virtualTree.Root.Items.Should().HaveCount(3);
            virtualTree.Root.Items[1].Content.Action.ActionId.Should().Be(3);
        }

        [TestMethod]
        public void ActionsGraphManipulationTestCreation_withNestedGroup()
        {
            var items = new List<ActionGridItem>
            {
                new ActionGridItem(new Models.KAction { ActionId = 0 }, 0) { IsGroup = true },
                new ActionGridItem(new Models.KAction { ActionId = 1 }, 1),
                new ActionGridItem(new Models.KAction { ActionId = 2 }, 1) { IsGroup = true },
                new ActionGridItem(new Models.KAction { ActionId = 3 }, 2),
                new ActionGridItem(new Models.KAction { ActionId = 4 }, 2)
            };

            var virtualTree = ActionsGraphManipulator.Virtualize(items);

            virtualTree.Root.Items.Should().HaveCount(1);
            virtualTree.Root.Items[0].As<ActionsGraphManipulator<ActionGridItem>.INodeContainer>().Items.Should().HaveCount(2);
            virtualTree.Root
                .Items[0].As<ActionsGraphManipulator<ActionGridItem>.INodeContainer>()
                .Items[1].As<ActionsGraphManipulator<ActionGridItem>.INodeContainer>()
                .Items.Should().HaveCount(2);
        }

        [TestMethod]
        public void ActionsGraphManipulationTestCreation_withMultipleGroups()
        {
            var items = new List<ActionGridItem>
            {
                new ActionGridItem(new Models.KAction { ActionId = 0 }, 0) { IsGroup = true },
                new ActionGridItem(new Models.KAction { ActionId = 1 }, 1),
                new ActionGridItem(new Models.KAction { ActionId = 2 }, 1) { IsGroup = true },
                new ActionGridItem(new Models.KAction { ActionId = 3 }, 2),
                new ActionGridItem(new Models.KAction { ActionId = 4 }, 0) { IsGroup = true },
                new ActionGridItem(new Models.KAction { ActionId = 5 }, 1),
                new ActionGridItem(new Models.KAction { ActionId = 6 }, 1) { IsGroup = true }
            };

            var virtualTree = ActionsGraphManipulator.Virtualize(items);

            virtualTree.Root.Items.Should().HaveCount(2);
        }

        [TestMethod]
        public void ActionsGraphManipulationTestMoveUp_FirstItemCannotMoveUp()
        {
            var items = new List<ActionGridItem>
            {
                new ActionGridItem(new Models.KAction { ActionId = 0 }, 0), // This one!
                new ActionGridItem(new Models.KAction { ActionId = 1 }, 0),
                new ActionGridItem(new Models.KAction { ActionId = 2 }, 0)
            };

            var virtualTree = ActionsGraphManipulator.Virtualize(items);

            virtualTree.Root.Items[0].MoveUpCommand.CanExecute(null).Should().BeFalse();
        }

        [TestMethod]
        public void ActionsGraphManipulationTestMoveUp_basicItemDoMovesUp()
        {
            var items = new List<ActionGridItem>
            {
                new ActionGridItem(new Models.KAction { ActionId = 0 }, 0),
                new ActionGridItem(new Models.KAction { ActionId = 1 }, 0),
                new ActionGridItem(new Models.KAction { ActionId = 2 }, 0) // This one!
            };

            var virtualTree = ActionsGraphManipulator.Virtualize(items);
            var target = virtualTree.Root.Items[2];

            target.MoveUpCommand.CanExecute(null).Should().BeTrue();
            target.MoveUpCommand.Execute(null);
            target.GetIndex().Should().Be(1);
            virtualTree.Root.Items[1].Content.Action.ActionId.Should().Be(target.Content.Action.ActionId); // 2
        }

        [TestMethod]
        public void ActionsGraphManipulationTestMoveUp_ItemDoMovesUpFromGroupInFirstPlace()
        {
            var items = new List<ActionGridItem>
            {
                new ActionGridItem(new Models.KAction { ActionId = 0 }, 0),
                new ActionGridItem(new Models.KAction { ActionId = 1 }, 0) { IsGroup = true },
                new ActionGridItem(new Models.KAction { ActionId = 2 }, 1), // This one!
                new ActionGridItem(new Models.KAction { ActionId = 3 }, 1),
                new ActionGridItem(new Models.KAction { ActionId = 4 }, 1)
            };

            var virtualTree = ActionsGraphManipulator.Virtualize(items);
            var target = ((ActionsGraphManipulator<ActionGridItem>.INodeContainer)virtualTree.Root.Items[1]).Items.First();

            target.MoveUpCommand.CanExecute(null).Should().BeTrue();
            target.MoveUpCommand.Execute(null);
            target.GetIndex().Should().Be(1);
            target.Parent.Should().Be(virtualTree.Root);
            virtualTree.Root.Items[1].Content.Action.ActionId.Should().Be(target.Content.Action.ActionId); // 2
            virtualTree.Root.Items.Should().HaveCount(3);
        }

        [TestMethod]
        public void ActionsGraphManipulationTestMoveUp_GroupDoMovesUp()
        {
            var items = new List<ActionGridItem>
            {
                new ActionGridItem(new Models.KAction { ActionId = 0 }, 0),
                new ActionGridItem(new Models.KAction { ActionId = 1 }, 0) { IsGroup = true }, // This one!
                new ActionGridItem(new Models.KAction { ActionId = 2 }, 1), 
                new ActionGridItem(new Models.KAction { ActionId = 3 }, 1)
            };

            var virtualTree = ActionsGraphManipulator.Virtualize(items);
            var target = virtualTree.Root.Items[1];

            target.MoveUpCommand.CanExecute(null).Should().BeTrue();
            target.MoveUpCommand.Execute(null);
            target.GetIndex().Should().Be(0);
            virtualTree.Root.Items[0].Content.Action.ActionId.Should().Be(target.Content.Action.ActionId); // 1
            virtualTree.Root.Items.Should().HaveCount(2);
        }

        [TestMethod]
        public void ActionsGraphManipulationTestMoveUp_IntoGroup()
        {
            var items = new List<ActionGridItem>
            {
                new ActionGridItem(new Models.KAction { ActionId = 0 }, 0),
                new ActionGridItem(new Models.KAction { ActionId = 1 }, 0) { IsGroup = true },
                new ActionGridItem(new Models.KAction { ActionId = 2 }, 1), 
                new ActionGridItem(new Models.KAction { ActionId = 3 }, 0) // This one!
            };

            var virtualTree = ActionsGraphManipulator.Virtualize(items);
            var target = virtualTree.Root.Items[2];

            target.MoveUpCommand.CanExecute(null).Should().BeTrue();
            target.MoveUpCommand.Execute(null);
            target.GetIndex().Should().Be(1);
            virtualTree.Root.Items.Should().HaveCount(2);
            ((ActionsGraphManipulator<ActionGridItem>.INodeContainer)virtualTree.Root.Items[1]).Items.Should().HaveCount(2);
        }

        [TestMethod]
        public void ActionsGraphManipulationTestCannotMoveDown_basic()
        {
            var items = new List<ActionGridItem>
            {
                new ActionGridItem(new Models.KAction { ActionId = 0 }, 0),
                new ActionGridItem(new Models.KAction { ActionId = 1 }, 0), 
                new ActionGridItem(new Models.KAction { ActionId = 2 }, 0) // This one!
            };

            var virtualTree = ActionsGraphManipulator.Virtualize(items);
            var target = virtualTree.Root.Items[2];

            target.MoveDownCommand.CanExecute(null).Should().BeFalse();
        }

        [TestMethod]
        public void ActionsGraphManipulationTestMoveDown_basicFirstItem()
        {
            var items = new List<ActionGridItem>
            {
                new ActionGridItem(new Models.KAction { ActionId = 0 }, 0), // This one!
                new ActionGridItem(new Models.KAction { ActionId = 1 }, 0), 
                new ActionGridItem(new Models.KAction { ActionId = 2 }, 0) 
            };

            var virtualTree = ActionsGraphManipulator.Virtualize(items);
            var target = virtualTree.Root.Items[0];

            target.MoveDownCommand.CanExecute(null).Should().BeTrue();
            target.MoveDownCommand.Execute(null);

            target.GetIndex().Should().Be(1);
        }

        [TestMethod]
        public void ActionsGraphManipulationTestMoveDown_IntoGroup()
        {
            var items = new List<ActionGridItem>
            {
                new ActionGridItem(new Models.KAction { ActionId = 0 }, 0), // This one!
                new ActionGridItem(new Models.KAction { ActionId = 1 }, 0) { IsGroup = true },
                new ActionGridItem(new Models.KAction { ActionId = 2 }, 1), 
            };

            var virtualTree = ActionsGraphManipulator.Virtualize(items);
            var target = virtualTree.Root.Items[0];

            target.MoveDownCommand.CanExecute(null).Should().BeTrue();
            target.MoveDownCommand.Execute(null);
            target.GetIndex().Should().Be(0);
            virtualTree.Root.Items.Should().HaveCount(1);
            ((ActionsGraphManipulator<ActionGridItem>.INodeContainer)virtualTree.Root.Items[0]).Items.Should().HaveCount(2);
        }

        [TestMethod]
        public void ActionsGraphManipulationTestMoveDown_LeavesGroup()
        {
            var items = new List<ActionGridItem>
            {
                new ActionGridItem(new Models.KAction { ActionId = 0 }, 0), 
                new ActionGridItem(new Models.KAction { ActionId = 1 }, 0) { IsGroup = true },
                new ActionGridItem(new Models.KAction { ActionId = 2 }, 1), 
                new ActionGridItem(new Models.KAction { ActionId = 3 }, 1) // This one!
            };

            var virtualTree = ActionsGraphManipulator.Virtualize(items);
            var target = virtualTree.ResolveNodeByFlatIndex(3);

            target.MoveDownCommand.CanExecute(null).Should().BeTrue();
            target.MoveDownCommand.Execute(null);

            target.GetIndex().Should().Be(2);
            virtualTree.Root.Items.Should().HaveCount(3);
            ((ActionsGraphManipulator<ActionGridItem>.INodeContainer)virtualTree.Root.Items[1]).Items.Should().HaveCount(1);
        }

        [TestMethod]
        public void ActionsGraphManipulationTestMoveDown_UniqueItemLeavesGroup()
        {
            var items = new List<ActionGridItem>
            {
                new ActionGridItem(new Models.KAction { ActionId = 0 }, 0), 
                new ActionGridItem(new Models.KAction { ActionId = 1 }, 0) { IsGroup = true },
                new ActionGridItem(new Models.KAction { ActionId = 2 }, 1) // This one!
            };

            var virtualTree = ActionsGraphManipulator.Virtualize(items);
            var target = virtualTree.ResolveNodeByFlatIndex(2);

            // C'est implémenté mais non supporté par le diagramme de Gantt pour l'instant (cf. commentaire dans ActionsGraphManipulator)
            /*
            target.MoveDownCommand.CanExecute(null).Should().BeTrue();
            target.MoveDownCommand.Execute(null);

            virtualTree.Root.Items.Select(item => item.Content.Action.ActionId).ShouldBeEquivalentTo(new []{ 0, 2 });
             */

            target.MoveDownCommand.CanExecute(null).Should().BeFalse(); // Desactivé pour l'instant
        }

        [TestMethod]
        public void ActionsGraphManipulation_NodeLevel()
        {
            var items = new List<ActionGridItem>
            {
                new ActionGridItem(new Models.KAction { ActionId = 0 }, 0),
                new ActionGridItem(new Models.KAction { ActionId = 1 }, 0) { IsGroup = true },
                new ActionGridItem(new Models.KAction { ActionId = 2 }, 1) { IsGroup = true },
                new ActionGridItem(new Models.KAction { ActionId = 3 }, 2)
            };

            var virtualTree = ActionsGraphManipulator.Virtualize(items);
            var target0 = virtualTree.Root.Items[0];
            var target1 = virtualTree.Root.Items[1];
            var target2 = ((ActionsGraphManipulator<ActionGridItem>.INodeContainer)target1).Items[0];
            var target3 = ((ActionsGraphManipulator<ActionGridItem>.INodeContainer)target2).Items[0];

            target0.GetLevel().Should().Be(0);
            target1.GetLevel().Should().Be(0);
            target2.GetLevel().Should().Be(1);
            target3.GetLevel().Should().Be(2);
        }

        [TestMethod]
        public void ActionsGraphManipulation_ResolveFirstNode()
        {
            var items = new List<ActionGridItem>
            {
                new ActionGridItem(new Models.KAction { ActionId = 0 }, 0),
                new ActionGridItem(new Models.KAction { ActionId = 1 }, 0)
            };

            var virtualTree = ActionsGraphManipulator.Virtualize(items);

            virtualTree.ResolveNodeByFlatIndex(0).Content.Action.ActionId.Should().Be(0);
        }

        [TestMethod]
        public void ActionsGraphManipulation_ResolveLastNode()
        {
            var items = new List<ActionGridItem>
            {
                new ActionGridItem(new Models.KAction { ActionId = 0 }, 0),
                new ActionGridItem(new Models.KAction { ActionId = 1 }, 0)
            };

            var virtualTree = ActionsGraphManipulator.Virtualize(items);

            virtualTree.ResolveNodeByFlatIndex(1).Content.Action.ActionId.Should().Be(1);
        }

        [TestMethod]
        public void ActionsGraphManipulation_ResolveNodeIndexInGroup()
        {
            var items = new List<ActionGridItem>
            {
                new ActionGridItem(new Models.KAction { ActionId = 0 }, 0),
                new ActionGridItem(new Models.KAction { ActionId = 1 }, 0) { IsGroup = true },
                new ActionGridItem(new Models.KAction { ActionId = 2 }, 1) { IsGroup = true },
                new ActionGridItem(new Models.KAction { ActionId = 3 }, 2)
            };

            var virtualTree = ActionsGraphManipulator.Virtualize(items);

            virtualTree.ResolveNodeByFlatIndex(3).Content.Action.ActionId.Should().Be(3);
        }

        [TestMethod]
        public void ActionsGraphManipulation_ResolveNodeIndexInMultipleGroups()
        {
            var items = new List<ActionGridItem>
            {
                new ActionGridItem(new Models.KAction { ActionId = 0 }, 0),
                new ActionGridItem(new Models.KAction { ActionId = 1 }, 0) { IsGroup = true },
                new ActionGridItem(new Models.KAction { ActionId = 2 }, 1) { IsGroup = true },
                new ActionGridItem(new Models.KAction { ActionId = 3 }, 2),
                new ActionGridItem(new Models.KAction { ActionId = 4 }, 0),
                new ActionGridItem(new Models.KAction { ActionId = 5 }, 0) { IsGroup = true },
                new ActionGridItem(new Models.KAction { ActionId = 6 }, 1) { IsGroup = true }
            };

            var virtualTree = ActionsGraphManipulator.Virtualize(items);

            virtualTree.ResolveNodeByFlatIndex(6).Content.Action.ActionId.Should().Be(6);
        }

        [TestMethod]
        public void ActionsGraphManipulation_WBSOn_basicFlattenList()
        {
            var items = new List<ActionGridItem>
            {
                new ActionGridItem(new Models.KAction { ActionId = 0 }, 0),
                new ActionGridItem(new Models.KAction { ActionId = 1 }, 0),
                new ActionGridItem(new Models.KAction { ActionId = 2 }, 0)
            };

            var virtualTree = ActionsGraphManipulator.Virtualize(items);

            var movedItem = virtualTree.Root.Items[2];
            movedItem.MoveUpCommand.Execute(null);
            movedItem.MoveUpCommand.Execute(null);

            virtualTree.BuildFlatList().Select(item => item.Action.WBS)
                .Should().BeEquivalentTo(new[] { "1", "2", "3" });
        }

        [TestMethod]
        public void ActionsGraphManipulation_IndentationOn_basicFlattenList()
        {
            var items = new List<ActionGridItem>
            {
                new ActionGridItem(new Models.KAction { ActionId = 0 }, 0),
                new ActionGridItem(new Models.KAction { ActionId = 1 }, 0),
                new ActionGridItem(new Models.KAction { ActionId = 2 }, 0)
            };

            var virtualTree = ActionsGraphManipulator.Virtualize(items);

            var movedItem = virtualTree.Root.Items[2];
            movedItem.MoveUpCommand.Execute(null);

            virtualTree.BuildFlatList().Select(item => item.Indentation)
                .Should().BeEquivalentTo(new[] { 0, 0, 0 });
        }

        [TestMethod]
        public void ActionsGraphManipulation_WBSOn_ListWithGroup()
        {
            var items = new List<ActionGridItem>
            {
                new ActionGridItem(new Models.KAction { ActionId = 0 }, 0),
                new ActionGridItem(new Models.KAction { ActionId = 1 }, 0) { IsGroup = true },
                new ActionGridItem(new Models.KAction { ActionId = 2 }, 1) { IsGroup = true },
                new ActionGridItem(new Models.KAction { ActionId = 3 }, 2)
            };

            var virtualTree = ActionsGraphManipulator.Virtualize(items);

            virtualTree.BuildFlatList().Select(item => item.Action.WBS)
                .Should().BeEquivalentTo(new[] { "1", "2", "2.1", "2.1.1" });
        }

        [TestMethod]
        public void ActionsGraphManipulation_IndentationOn_ListWithGroup()
        {
            var items = new List<ActionGridItem>
            {
                new ActionGridItem(new Models.KAction { ActionId = 0 }, 0),
                new ActionGridItem(new Models.KAction { ActionId = 1 }, 0) { IsGroup = true },
                new ActionGridItem(new Models.KAction { ActionId = 2 }, 1) { IsGroup = true },
                new ActionGridItem(new Models.KAction { ActionId = 3 }, 2)
            };

            var virtualTree = ActionsGraphManipulator.Virtualize(items);

            virtualTree.BuildFlatList().Select(item => item.Indentation)
                .Should().BeEquivalentTo(new[] { 0, 0, 1, 2 });
        }

        [TestMethod]
        public void ActionsGraphManipulation_ApplyNewGraph_basic()
        {
            var items = new List<ActionGridItem>
            {
                new ActionGridItem(new Models.KAction { ActionId = 0 }, 0),
                new ActionGridItem(new Models.KAction { ActionId = 1 }, 0)
            };

            var virtualTree = ActionsGraphManipulator.Virtualize(items);
            virtualTree.Apply(items);

            items.Select(item => item.Action.ActionId).Should().BeEquivalentTo(new[] { 0, 1 });
        }

        [TestMethod]
        public void ActionsGraphManipulation_ApplyNewGraph_WithUpAction()
        {
            var items = new List<ActionGridItem>
            {
                new ActionGridItem(new Models.KAction { ActionId = 0 }, 0),
                new ActionGridItem(new Models.KAction { ActionId = 1 }, 0),
                new ActionGridItem(new Models.KAction { ActionId = 2 }, 0)
            };

            var virtualTree = ActionsGraphManipulator.Virtualize(items);
            var target = virtualTree.ResolveNodeByFlatIndex(2);

            
            target.MoveUpCommand.CanExecute(null).Should().BeTrue();
            target.MoveUpCommand.Execute(null);
            target.MoveUpCommand.Execute(null);
            virtualTree.Apply(items);

            items.Select(item => item.Action.ActionId).Should().BeEquivalentTo(new[] { 2, 0, 1 });
        }

        [TestMethod]
        public void ActionsGraphManipulation_ApplyNewGraph_WithUpActionRemovingGroup()
        {
            var items = new List<ActionGridItem>
            {
                new ActionGridItem(new Models.KAction { ActionId = 0 }, 0),
                new ActionGridItem(new Models.KAction { ActionId = 1 }, 0) { IsGroup = true },
                new ActionGridItem(new Models.KAction { ActionId = 2 }, 1),
            };

            var virtualTree = ActionsGraphManipulator.Virtualize(items);
            var target = virtualTree.ResolveNodeByFlatIndex(2);

            /* Ce cas est géré mais non supporté par le diagramme de gantt.

            target.MoveUpCommand.CanExecute(null).Should().BeTrue();
            target.MoveUpCommand.Execute(null);
            virtualTree.Apply(items);

            items.Select(item => item.Action.ActionId).Should().BeEquivalentTo(new[] { 0, 2 });
             */

            target.MoveUpCommand.CanExecute(null).Should().BeFalse(); // Ce cas est désactivé pour l'instant
        }
    }
}
