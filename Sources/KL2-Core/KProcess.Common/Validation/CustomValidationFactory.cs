using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace KProcess.Common.Validation
{
    /// <summary>
    /// Représente une fabrique de validateurs à partir des attributs.
    /// </summary>
    /// <remarks>
    /// Certaines infos trouvées ici :
    /// http://www.cuttingedge.it/blogs/steven/pivot/entry.php?id=80
    /// </remarks>
    public class CustomAttributeValidationFactory : AttributeValidatorFactory
    {
        /// <summary>
        /// Crée le validateur pour la cible et le jeu de règles spécifiés.
        /// </summary>
        /// <param name="targetType">Le <see cref="T:System.Type"/> à valider.</param>
        /// <param name="ruleset">Le jeu de règles à utiliser lors de la validation.</param>
        /// <param name="mainValidatorFactory">La brique à utiliser lors de la construction des validateurs imbriqués..</param>
        /// <returns>
        /// Un <see cref="T:Microsoft.Practices.EnterpriseLibrary.Validation.Validator"/>.
        /// </returns>
        protected override Validator InnerCreateValidator(Type targetType, string ruleset, ValidatorFactory mainValidatorFactory)
        {
            var baseValidator = base.InnerCreateValidator(targetType, ruleset, mainValidatorFactory);

            // Créer le self validator
            var metaDataValidator = CreateSelfValidator(targetType, ruleset, mainValidatorFactory);

            if (metaDataValidator == null)
            {
                return baseValidator;
            }

            // Combiner le self validator
            return new AndCompositeValidator(baseValidator, metaDataValidator);
        }

        /// <summary>
        /// Crée un SelfValidator si la classe à valider possède des metadatas.
        /// </summary>
        /// <param name="targetType">Le type cible.</param>
        /// <param name="ruleset">Le jeu de règles.</param>
        /// <param name="mainValidatorFactory">La brique à utiliser lors de la construction des validateurs imbriqués..</param>
        /// <returns>Le validateur créé.</returns>
        private Validator CreateSelfValidator(Type targetType, string ruleset, ValidatorFactory mainValidatorFactory)
        {
            var validators = new List<Validator>();

            // Si la classe utilise la validation par metadata, forcer quand même la SelfValidation à avoir lieu
            var selfValidationMethod = targetType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .FirstOrDefault(m => ValidationReflectionHelper.GetCustomAttributes(m, typeof(SelfValidationAttribute), false).Any());

            if (selfValidationMethod != null)
                validators.Add(new SelfValidationValidator(selfValidationMethod));

            // Parcourir toutes les propriétés qui possèdent de la validation classe VAB en plus de la validation METADATA
            // Pour chacun des ValueValidatorAttribute (ou dérivé) trouvé, ajouter un validateur
            foreach (var prop in targetType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var validatedElement = (IValidatedElement)new ValidationAttributeValidatedElement(prop);
                var builder = new MemberAccessValidatorBuilderFactory().GetPropertyValueAccessValidatorBuilder(prop, validatedElement);

                foreach (IValidatorDescriptor att in prop.GetCustomAttributes(typeof(ValueValidatorAttribute), false))
                    validators.Add(CreateValidatorForValidatedElement(att, validatedElement, builder, mainValidatorFactory));
            }

            return new AndCompositeValidator(validators.ToArray());
        }

        /// <summary>
        /// Crée un validator de propriété.
        /// </summary>
        /// <param name="attribute">L'attribut de validation.</param>
        /// <param name="validatedElement">La propriété validée.</param>
        /// <param name="builder">Le constructeur de validation composite.</param>
        /// <param name="mainValidatorFactory">La brique à utiliser lors de la construction des validateurs imbriqués..</param>
        /// <returns>La validateur de propriété.</returns>
        protected Validator CreateValidatorForValidatedElement(IValidatorDescriptor attribute, IValidatedElement validatedElement, CompositeValidatorBuilder builder, ValidatorFactory mainValidatorFactory)
        {

            Validator valueValidator = attribute.CreateValidator(validatedElement.TargetType, validatedElement.MemberInfo.ReflectedType, new MemberAccessValidatorBuilderFactory().MemberValueAccessBuilder, mainValidatorFactory);
            builder.AddValueValidator(valueValidator);

            return builder.GetValidator();
        }

    }
}
