namespace AccessingChildcareEntitlementChecker.Web.Models
{
    public class FormData
    {
        public string CurrentPageId { get; set; }

        // We still need the polymorphic list here to handle different GDS inputs
        public List<AnswerBase> Answers { get; set; } = new();
    }
}
