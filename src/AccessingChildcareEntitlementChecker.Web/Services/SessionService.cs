using AccessingChildcareEntitlementChecker.Web.Cms;
using AccessingChildcareEntitlementChecker.Web.Models;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace AccessingChildcareEntitlementChecker.Web.Services
{
    public interface ISessionService
    {
        void SaveAnswer(string fieldId, string value);
        FormSubmission GetConsolidatedSubmission();
        void Clear();
    }

    public class SessionService : ISessionService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string SessionKey = "UserJourneyState";

        public SessionService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void SaveAnswer(string fieldId, string value)
        {
            var submission = GetConsolidatedSubmission();

            // Update existing or add new
            var existing = submission.Answers.FirstOrDefault(a => a.FieldId == fieldId);
            if (existing != null)
            {
                submission.Answers.Remove(existing);
            }

            submission.Answers.Add(new SingleValueAnswer { FieldId = fieldId, SelectedValue = value });

            var json = JsonSerializer.Serialize(submission);
            _httpContextAccessor.HttpContext.Session.SetString(SessionKey, json);
        }

        public FormSubmission GetConsolidatedSubmission()
        {
            var json = _httpContextAccessor.HttpContext.Session.GetString(SessionKey);
            if (string.IsNullOrEmpty(json)) return new FormSubmission();

            return JsonSerializer.Deserialize<FormSubmission>(json) ?? new FormSubmission();
        }

        public void Clear() => _httpContextAccessor.HttpContext.Session.Remove(SessionKey);
    }
}