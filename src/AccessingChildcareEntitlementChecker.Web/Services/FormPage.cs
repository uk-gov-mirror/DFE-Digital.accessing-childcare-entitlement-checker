using Contentful.Core.Models;

namespace AccessingChildcareEntitlementChecker.Web.Services
{
    // --- Contentful Models (Aligned with FormBuilder Space) ---

    public class FormPage
    {
        public string PageId { get; set; }
        public string Heading { get; set; }
        public Document Content { get; set; } // Contentful Rich Text
        public List<FormField> Fields { get; set; }
        public FormPage DefaultNextPage { get; set; }
        public List<RoutingRule> RoutingRules { get; set; }

        public List<JsonRoutingRule> JsonRoutingRules { get; set; }

        public string BackButtonLabel { get; set; }
    }
}