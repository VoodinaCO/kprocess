using Microsoft.VisualStudio.TestTools.UnitTesting;
using KProcess.Ksmed.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KProcess.Ksmed.Models;

namespace KProcess.Ksmed.Data.Tests
{
    [TestClass()]
    public class KsmedEntitiesExtTests
    {
        [TestMethod()]
        public void SaveChangesTest()
        {
            User user1 = null;
            User user2 = null;

            int result1 = 0;
            int result2 = 0;
            using (var context1 = ContextFactory.GetNewContext())
            {
                user1 = context1.Users.Single(_ => _.Username == "admin");

                KProcess.Ksmed.Security.SecurityContext.AutoLogAsForTests(user1);

                // On modifie le Firstname de l'user1
                user1.Firstname = "test1";
                // On récupère le même user dans un autre context et on le modifie
                using (var context2 = ContextFactory.GetNewContext())
                {
                    user2 = context2.Users.Single(_ => _.Username == "admin");
                    user2.Firstname = "test2";
                    result2 = context2.SaveChanges();
                }
                // Les données de user1 ne correspondent plus à ceux de la base
                result1 = context1.SaveChanges();
            }

            // On remet à zéro
            using (var context = ContextFactory.GetNewContext())
            {
                user1 = context.Users.Single(_ => _.Username == "admin");
                user1.Firstname = string.Empty;
                context.SaveChanges();
            }

            // result2 doit correspondre au numéro d'erreur lors d'une DbUpdateConcurrencyException
            Assert.AreEqual(-666, result1);
        }
    }
}