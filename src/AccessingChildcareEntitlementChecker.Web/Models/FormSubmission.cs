using AccessingChildcareEntitlementChecker.Web.Services;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AccessingChildcareEntitlementChecker.Web.Models
{
    // --- Polymorphic Binding Models ---

    /// <summary>
    /// Composition model that represents a full form submission.
    /// </summary>
    public class FormSubmission
    {
        public string PageId { get; set; }
        public List<AnswerBase> Answers { get; set; }

        public FormSubmission(FormData data)
        {
            PageId = data.CurrentPageId;
            Answers = data.Answers;
        }
        /// <summary>
        /// Validates the submission against the Contentful Field definitions.
        /// AZ-500: Server-side validation is the primary defense against 'Bypassing Client Checks'.
        /// </summary>
        public List<ValidationResult> Validate(FormPage pageMetadata)
        {
            var results = new List<ValidationResult>();

            foreach (var field in pageMetadata.Fields)
            {
                var answer = Answers.FirstOrDefault(a => a.FieldId == field.FieldId);

                // Example Validation: Required Check
                if (string.IsNullOrEmpty(answer?.GetRawValue()))
                {
                    results.Add(new ValidationResult($"Select an option for {field.Label}", new[] { field.FieldId }));
                }
            }

            return results;
        }
    }
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
    [JsonDerivedType(typeof(TextAnswer), typeDiscriminator: "text")]
    [JsonDerivedType(typeof(SingleValueAnswer), typeDiscriminator: "single")]
    [JsonDerivedType(typeof(MultiValueAnswer), typeDiscriminator: "multi")]
    [JsonDerivedType(typeof(DateAnswer), typeDiscriminator: "date")]
    public abstract class AnswerBase
    {
        public string FieldId { get; set; }
        public abstract string GetRawValue();
    }

    public class TextAnswer : AnswerBase
    {
        public string Value { get; set; }
        public override string GetRawValue() => Value;
    }

    public class SingleValueAnswer : AnswerBase // Radios, Select
    {
        public string SelectedValue { get; set; }
        public override string GetRawValue() => SelectedValue;
    }

    public class MultiValueAnswer : AnswerBase // Checkboxes
    {
        public List<string> Values { get; set; } = new();
        public override string GetRawValue() => string.Join(",", Values);
    }

    public class DateAnswer : AnswerBase // GDS 3-part date
    {
        public string Day { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public override string GetRawValue() => $"{Year}-{Month}-{Day}";
    }

}
