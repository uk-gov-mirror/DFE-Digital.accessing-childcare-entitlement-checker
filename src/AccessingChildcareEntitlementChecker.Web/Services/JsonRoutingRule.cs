namespace AccessingChildcareEntitlementChecker.Web.Services
{
    // --- JSON Specific Routing Rule (Flattened) ---

    public class JsonRoutingRule
    {
        public string RuleName { get; set; }
        public string ConditionFieldId { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }
        public string DestinationPageId { get; set; }
    }

}
