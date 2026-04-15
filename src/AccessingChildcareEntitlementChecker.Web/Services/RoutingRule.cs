namespace AccessingChildcareEntitlementChecker.Web.Services
{
    public class RoutingRule
    {
        public string RuleName { get; set; }
        public FormField ConditionField { get; set; } // Reference to a FormField
        public string Operator { get; set; }          // e.g., "equals"
        public string Value { get; set; }             // e.g., "parent"
        public FormPage Destination { get; set; }      // Reference to a FormPage
    }
}