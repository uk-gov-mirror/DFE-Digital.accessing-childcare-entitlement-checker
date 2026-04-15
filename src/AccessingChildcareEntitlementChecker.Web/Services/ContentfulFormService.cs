using AccessingChildcareEntitlementChecker.Web.Models;
using Contentful.Core;
using Contentful.Core.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AccessingChildcareEntitlementChecker.Web.Services
{
    /// <summary>
    /// POC: Contentful Form Engine Service
    /// This service acts as the 'Logic Engine' that interprets CMS data into a GDS journey.
    /// Relevant: Yes - The SDK fetches the data, but this service evaluates the business rules.
    /// </summary>
    public class ContentfulFormService : IContentfulFormService
    {
        private readonly IContentfulClient _client;

        public ContentfulFormService(IContentfulClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Fetches a specific page by its ID and resolves all references (Fields, Rules, and Options).
        /// </summary>
        public async Task<FormPage> GetPageAsync(string pageId)
        {
            var builder = QueryBuilder<FormPage>.New
                .ContentTypeIs("formPage")
                .FieldEquals("fields.pageId", pageId)
                .Include(4); // Deep include to resolve Page -> Rules -> ConditionField (FormField)

            var result = await _client.GetEntries(builder);


            return result.FirstOrDefault();
        }

        /// <summary>
        /// Logic Engine: Determines the next page based on user input and CMS-defined rules.
        /// Now handles ConditionField as a Reference to a FormField.
        /// </summary>
        public string ResolveNextPage(FormPage currentPage, Dictionary<string, string> userAnswers)
        {
            if (currentPage.RoutingRules == null || !currentPage.RoutingRules.Any())
            {
                return currentPage.DefaultNextPage?.PageId;
            }

            foreach (var rule in currentPage.RoutingRules)
            {
                // Accessing FieldId via the Resolved Reference
                var targetFieldId = rule.ConditionField?.FieldId;

                if (!string.IsNullOrEmpty(targetFieldId) && userAnswers.TryGetValue(targetFieldId, out var userValue))
                {
                    if (EvaluateRule(rule.Operator, userValue, rule.Value))
                    {
                        return rule.Destination?.PageId;
                    }
                }
            }

            // Fallback to the default path if no rules match
            return currentPage.DefaultNextPage?.PageId;
        }

        public string ResolveNextPage(FormPage currentPage, FormSubmission submission)
        {
            if (currentPage?.RoutingRules == null || !currentPage.RoutingRules.Any())
            {
                return currentPage?.DefaultNextPage?.PageId;
            }

            foreach (var rule in currentPage.RoutingRules)
            {
                var targetFieldId = rule.ConditionField?.FieldId;
                var answer = submission.Answers.FirstOrDefault(a => a.FieldId == targetFieldId);

                if (answer != null)
                {
                    // Logic handles both single values (Radios) and multi-values (Checkboxes)
                    var valueToCompare = answer is MultiValueAnswer multi ? string.Join(",", multi.Values) : answer.GetRawValue();

                    if (EvaluateRule(rule.Operator, valueToCompare, rule.Value))
                    {
                        return rule.Destination?.PageId;
                    }
                }
            }

            return currentPage.DefaultNextPage?.PageId;
        }

        public string ResolveNextPage(FormPage currentPage, string userAnswer)
        {
            return null;
        }

        private bool EvaluateRule(string op, string actual, string expected)
        {
            return op.ToLower() switch
            {
                "equals" => string.Equals(actual, expected, StringComparison.OrdinalIgnoreCase),
                "notequals" => !string.Equals(actual, expected, StringComparison.OrdinalIgnoreCase),
                "greaterthan" => decimal.TryParse(actual, out var a) &&
                                 decimal.TryParse(expected, out var e) && a > e,
                "lessthan" => decimal.TryParse(actual, out var a) &&
                              decimal.TryParse(expected, out var e) && a < e,
                _ => false
            };
        }
    }
}