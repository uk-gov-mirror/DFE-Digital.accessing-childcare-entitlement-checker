using Microsoft.AspNetCore.Html;

namespace AccessingChildcareEntitlementChecker.Web.Models
{
    public class ResultsPageViewModel
    {
        // Maps to: 'Internal Title' (for logging/debugging)
        public string InternalTitle { get; set; }

        // Maps to: 'Child Name Field' (e.g. "Jack")
        public string ChildName { get; set; }

        // The dynamically generated header (e.g. "What Jack can get now")
        public string PageHeading => $"What {ChildName} can get now";

        // Maps to: 'Show 15hr Scheme' (Boolean)
        public bool Show15HrScheme { get; set; }

        // Maps to: 'Show Working Parent' (Boolean)
        public bool ShowWorkingParent { get; set; }

        // Maps to: 'Guidance' (Rich Text from Contentful)
        public HtmlString Guidance { get; set; }

        // List of schemes to be rendered by the generic UI component
        public List<SchemeViewModel> EligibleSchemes { get; set; } = new List<SchemeViewModel>();
    }

    public class SchemeViewModel
    {
        public string Title { get; set; }
        public List<SchemeDetailRow> Details { get; set; } = new List<SchemeDetailRow>();
        public List<SchemeLink> Actions { get; set; } = new List<SchemeLink>();
    }

    public class SchemeDetailRow
    {
        public string Label { get; set; }
        public string Value { get; set; }
    }

    public class SchemeLink
    {
        public string Text { get; set; }
        public string Url { get; set; }
    }
}
