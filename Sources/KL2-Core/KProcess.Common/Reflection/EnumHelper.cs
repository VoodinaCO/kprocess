using System;

namespace KProcess
{
    /// <summary>
    /// Fournis des méthodes d'aide de manipulation des énumérations.
    /// </summary>
    public static class EnumHelper
    {

        /// <summary>
        /// Fournit un moyen de faire un switch sur une valeur d'énumération de type flag.
        /// </summary>
        /// <typeparam name="TEnum">Le type de l'énumération.</typeparam>
        /// <param name="value">La valeur de l'énumération.</param>
        /// <param name="switchAction">L'action de switch à exécuter.</param>
        public static void SwitchFlags<TEnum>(this TEnum value, Action<TEnum> switchAction)
            where TEnum : struct, IConvertible
        {
            var enumType = typeof(TEnum);
            if (!enumType.IsEnum)
                throw new ArgumentException("Le type TEnum doit être une énumération.");

            ulong valueNum = ((IConvertible)value).ToUInt64(null);

            var checkTypeValues = Enum.GetValues(enumType);
            foreach (TEnum iteratingValue in checkTypeValues)
            {
                ulong currentNum = ((IConvertible)iteratingValue).ToUInt64(null);

                if ((valueNum & currentNum) == currentNum)
                    switchAction(iteratingValue);
            }
        }
    }
}
