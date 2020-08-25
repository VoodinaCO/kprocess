using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using KProcess.Ksmed.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KProcess.Ksmed.Business.Tests
{
    enum CollectionComparison
    {
        Size,
        Content,
    }

    class EntityComparer<TEntity>
        where TEntity : IObjectWithChangeTracker
    {
        private List<string> _mustBeDifferent;
        private List<string> _mustBeDifferentOrNull;
        private List<string> _mustBeNull;
        private List<string> _ignore;
        private Dictionary<string, CollectionComparison> _collectionComparisons;
        private TestContext _testContext;

        public EntityComparer(TestContext testContext)
        {
            _testContext = testContext;
            _mustBeDifferent = new List<string>();
            _mustBeDifferentOrNull = new List<string>();
            _mustBeNull = new List<string>();
            _ignore = new List<string>();
            _collectionComparisons = new Dictionary<string, CollectionComparison>();
        }

        public EntityComparer<TEntity> MustBeDifferent(params string[] propertyNames)
        {
            _mustBeDifferent.AddRangeNew(propertyNames);
            return this;
        }

        public EntityComparer<TEntity> MustBeDifferentOrNull(params string[] propertyNames)
        {
            _mustBeDifferentOrNull.AddRangeNew(propertyNames);
            return this;
        }

        public EntityComparer<TEntity> MustTargetBeNull(params string[] propertyNames)
        {
            _mustBeNull.AddRangeNew(propertyNames);
            return this;
        }

        public EntityComparer<TEntity> Ignore(params string[] propertyNames)
        {
            _ignore.AddRangeNew(propertyNames);
            return this;
        }

        public EntityComparer<TEntity> Collection(string propertyName, CollectionComparison comparisonMode)
        {
            _collectionComparisons.Add(propertyName, comparisonMode);
            return this;
        }

        public bool Compare(TEntity entity1, TEntity entity2)
        {
            var valuesEntity1 = entity1.GetCurrentValues();
            var valuesEntity2 = entity2.GetCurrentValues();

            // Valider les noms de propriétés

            foreach (var propName in _mustBeDifferent.Union(_mustBeDifferentOrNull).Union(_mustBeNull))
            {
                if (!valuesEntity1.ContainsKey(propName))
                    Assert.Fail("Nom de propriété {0} invalide", propName);
            }

            foreach (var kvp in valuesEntity1.Where(k => !_ignore.Contains(k.Key)))
            {
                var value1 = kvp.Value;
                var value2 = valuesEntity2[kvp.Key];

                bool invert = _mustBeDifferent.Contains(kvp.Key);
                bool invertOrNull = _mustBeDifferentOrNull.Contains(kvp.Key);
                bool mustBenull = _mustBeNull.Contains(kvp.Key);

                if (mustBenull)
                {
                    if (value2 != null)
                    {
                        ShowValueDifferentError("MustBeNull", entity1, kvp.Key, value1, value2);
                        return false;
                    }
                }
                else
                {
                    if (value1 is Array && value2 is Array)
                    {

                        // Il s'agit d'une collection
                        // Les éléments ne sont pas nécessairement dans le même ordre

                        var array1 = (Array)value1;
                        var array2 = (Array)value2;

                        if (_collectionComparisons.ContainsKey(kvp.Key))
                        {
                            switch (_collectionComparisons[kvp.Key])
                            {
                                case CollectionComparison.Size:
                                    if (array1.Length != array2.Length)
                                    {
                                        ShowValueDifferentError("CollectionComparison.Size", entity1, kvp.Key, array1, array2);
                                        return false;
                                    }
                                    break;

                                case CollectionComparison.Content:

                                    // Vérifier la taille
                                    if (array1.Length != array2.Length)
                                    {
                                        ShowValueDifferentError("CollectionComparison.ContentSize", entity1, kvp.Key, array1, array2);
                                        return false;
                                    }

                                    // Vérifier ensuite le contenu
                                    for (int i = 0; i < array1.Length; i++)
                                    {
                                        if (!CompareEquality(array1.GetValue(i), array2.GetValue(i), invertOrNull, invertOrNull, entity1, kvp.Key))
                                        {
                                            ShowValueDifferentError("CollectionComparison.Content", entity1, kvp.Key, array1, array2);
                                            return false;
                                        }
                                    }

                                    break;

                            }

                        }

                    }
                    else if (!CompareEquality(value1, value2, invert, invertOrNull, entity1, kvp.Key))
                        return false;

                }
            }

            return true;

        }

        private bool CompareEquality(object value1, object value2, bool invert, bool invertOrNull, TEntity entity1, string propertyName)
        {
            if (value1 is IComparable && value2 is IComparable)
            {
                var comparable1 = (IComparable)value1;
                var comparable2 = (IComparable)value2;

                bool isDifferent = (comparable1.CompareTo(comparable2) != 0);
                if (isDifferent)
                {
                    if (!invert && !invertOrNull)
                    {
                        ShowValueDifferentError("MustBeDifferent OR MustBeDifferentOrNull", entity1, propertyName, value1, value2);
                        return false;
                    }
                }
                else
                {
                    if (invertOrNull && comparable1 != null)
                    {
                        ShowValueDifferentError("MustBeDifferentOrNull", entity1, propertyName, value1, value2);
                        return false;
                    }
                    else if (invert)
                    {
                        ShowValueDifferentError("MustBeDifferent", entity1, propertyName, value1, value2);
                        return false;
                    }
                }

            }
            else
            {
                bool isDifferent = value1 != value2;
                if (isDifferent)
                {
                    if (!invert && !invertOrNull)
                    {
                        ShowValueDifferentError("MustBeDifferent OR MustBeDifferentOrNull", entity1, propertyName, value1, value2);
                        return false;
                    }
                }
                else
                {
                    if (invertOrNull && value1 != null)
                    {
                        ShowValueDifferentError("MustBeDifferentOrNull", entity1, propertyName, value1, value2);
                        return false;
                    }
                    else if (invert)
                    {
                        ShowValueDifferentError("MustBeDifferent", entity1, propertyName, value1, value2);
                        return false;
                    }
                }
            }

            return true;
        }

        private void ShowValueDifferentError(string rule, IObjectWithChangeTracker obj, string propertyName, object value1, object value2)
        {
            _testContext.WriteLine("Règle : {0}. Sur l'objet {1}, la valeur de la propriété {2} vaut '{3}' et '{4}'",
                rule, obj, propertyName, value1, value2);
        }

    }
}
