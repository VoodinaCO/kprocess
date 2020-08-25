using KProcess.Ksmed.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KProcess.Ksmed.Security
{
    public static class Rights
    {
        static readonly Dictionary<string, Dictionary<Screen, Right>> Values;

        static Rights()
        {
            Values = new Dictionary<string, Dictionary<Screen, Right>>
            {
                [KnownRoles.Administrator] = Enum.GetValues(typeof(Screen)).Cast<Screen>().ToDictionary(key => key, value => Right.Writing),
                [KnownRoles.Supervisor] = new Dictionary<Screen, Right>
                {
                    [Screen.Training] = Right.Reading,
                    [Screen.Qualification] = Right.Nothing,
                    [Screen.Inspection] = Right.Reading,
                    [Screen.Audit] = Right.Writing
                },
                [KnownRoles.Operator] = new Dictionary<Screen, Right>
                {
                    [Screen.Training] = Right.Reading,
                    [Screen.Qualification] = Right.Nothing,
                    [Screen.Inspection] = Right.Nothing,
                    [Screen.Audit] = Right.Nothing
                },
                [KnownRoles.Trainer] = new Dictionary<Screen, Right>
                {
                    [Screen.Training] = Right.Writing,
                    [Screen.Qualification] = Right.Nothing,
                    [Screen.Inspection] = Right.Nothing,
                    [Screen.Audit] = Right.Nothing
                }
            };
            // Droits Technician - hérite de Operator
            var technicianRight = Values[KnownRoles.Operator].ToDictionary(key => key.Key, value => value.Value);
            technicianRight[Screen.Inspection] = Right.Writing;
            Values.Add(KnownRoles.Technician, technicianRight);
            // Droits Evaluator - hérite de Trainer
            var evaluatorRight = Values[KnownRoles.Trainer].ToDictionary(key => key.Key, value => value.Value);
            evaluatorRight[Screen.Qualification] = Right.Writing;
            Values.Add(KnownRoles.Evaluator, evaluatorRight);
        }

        public static bool CanBeReadBy(this Screen screen, string roleCode)
        {
            if (Values.ContainsKey(roleCode) && Values[roleCode].ContainsKey(screen))
                return (Values[roleCode][screen] & Right.Reading) == Right.Reading;
            return false;
        }

        public static bool CanBeReadBy(this Screen screen, IEnumerable<string> roleCodes) =>
            roleCodes?.Any(role => screen.CanBeReadBy(role)) == true;

        public static bool CanBeReadBy(this Screen screen, User user)
        {
            if (user?.RoleCodes.Any() == true)
                return screen.CanBeReadBy(user.RoleCodes);
            if (user?.Roles.Any() == true)
                return screen.CanBeReadBy(user.Roles.Select(_ => _.RoleCode));
            return false;
        }

        public static bool CanBeWrittenBy(this Screen screen, string roleCode)
        {
            if (Values.ContainsKey(roleCode) && Values[roleCode].ContainsKey(screen))
                return (Values[roleCode][screen] & Right.Writing) == Right.Writing;
            return false;
        }

        public static bool CanBeWrittenBy(this Screen screen, IEnumerable<string> roleCodes) =>
            roleCodes?.Any(role => screen.CanBeWrittenBy(role)) == true;

        public static bool CanBeWrittenBy(this Screen screen, User user)
        {
            if (user?.RoleCodes.Any() == true)
                return screen.CanBeWrittenBy(user.RoleCodes);
            if (user?.Roles.Any() == true)
                return screen.CanBeWrittenBy(user.Roles.Select(_ => _.RoleCode));
            return false;
        }
    }

    public enum Right
    {
        Nothing = 0,
        Reading = 1,
        Writing = 3
    }

    public enum Screen
    {
        Training,
        Qualification,
        Inspection,
        Audit
    }
}
