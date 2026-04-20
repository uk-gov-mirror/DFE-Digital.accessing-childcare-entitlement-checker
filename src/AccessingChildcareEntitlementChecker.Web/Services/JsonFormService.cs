using AccessingChildcareEntitlementChecker.Web.Models;
using Newtonsoft.Json;

namespace AccessingChildcareEntitlementChecker.Web.Services
{
    public class JsonFormService : ICmsFormService
    {

        private readonly IWebHostEnvironment _env;
        private readonly string _jsonPath;

        public JsonFormService(IWebHostEnvironment env)
        {
            _env = env;
            // Place your JSON file in the 'wwwroot/data' folder or similar
            _jsonPath = Path.Combine(_env.WebRootPath, "data", "form_journey.json");
        }

        private async Task<List<FormPage>> GetAllPagesAsync()
        {
            if (!File.Exists(_jsonPath)) return new List<FormPage>();

            var json = await File.ReadAllTextAsync(_jsonPath);
            var container = JsonConvert.DeserializeObject<FormJourneyContainer>(json);
            return container?.Pages ?? new List<FormPage>();
        }

        public async Task<FormPage> GetPageAsync(string pageId)
        {
            var pages = await GetAllPagesAsync();
            return pages.FirstOrDefault(p => p.PageId == pageId);
        }

        /// <summary>
        /// Logic Engine: Same logic as Contentful version, but runs against local POCOs.
        /// </summary>
        public string ResolveNextPage(FormPage currentPage, FormSubmission submission)
        {
            if (currentPage?.JsonRoutingRules == null || !currentPage.JsonRoutingRules.Any())
            {
                return currentPage?.DefaultNextPage?.PageId;
            }

            foreach (var rule in currentPage.JsonRoutingRules)
            {
                // In JSON, we store the ID directly for simplicity
                var targetFieldId = rule.ConditionFieldId;
                var answer = submission.Answers.FirstOrDefault(a => a.FieldId == targetFieldId);

                if (answer != null)
                {
                    var userValue = answer.GetRawValue();
                    if (EvaluateRule(rule.Operator, userValue, rule.Value))
                    {
                        return rule.DestinationPageId;
                    }
                }
            }

            return currentPage.DefaultNextPage?.PageId;
        }

        private bool EvaluateRule(string op, string actual, string expected)
        {
            if (string.IsNullOrEmpty(op)) return false;

            // Apply same sanitisation logic as the Custom Converter
            actual = actual?.Trim().Trim('`', '\'', '\"');
            expected = expected?.Trim().Trim('`', '\'', '\"');

            return op.ToLower() switch
            {
                "equals" => string.Equals(actual, expected, StringComparison.OrdinalIgnoreCase),
                "greaterthan" => decimal.TryParse(actual, out var a) && decimal.TryParse(expected, out var e) && a > e,
                _ => false
            };
        }

        public Task<ResultsPage> GetResultsPageAsync()
        {
            throw new NotImplementedException();
        }
    }

}
