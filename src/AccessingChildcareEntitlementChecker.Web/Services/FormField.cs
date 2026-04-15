using Newtonsoft.Json;

namespace AccessingChildcareEntitlementChecker.Web.Services
{
    public class FormField
    {
        public string FieldId { get; set; }
        //public string[] Type { get; set; } // radios, checkboxes, select, text
        public string Label { get; set; }
        public string Hint { get; set; }
        public List<FieldOption> Options { get; set; }

        [JsonProperty("Type")]
        public string FieldType { get; set; } // e.g., "text", "radios"

        public string ErrorMessage { get; set; }

    }
}