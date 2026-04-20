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
            var data = GetFormData();

            var existing = data.Answers.FirstOrDefault(a => a.FieldId == fieldId);
            if (existing != null) data.Answers.Remove(existing);

            data.Answers.Add(new SingleValueAnswer { FieldId = fieldId, SelectedValue = value });

            var json = JsonSerializer.Serialize(data);
            _httpContextAccessor.HttpContext.Session.SetString(SessionKey, json);
        }

        public FormData GetFormData()
        {
            var json = _httpContextAccessor.HttpContext.Session.GetString(SessionKey);
            if (string.IsNullOrEmpty(json)) return new FormData();

            return JsonSerializer.Deserialize<FormData>(json) ?? new FormData();
        }

        // This is where you convert the 'Data' into a 'Submission' for your logic
        public FormSubmission GetConsolidatedSubmission()
        {
            return new FormSubmission(GetFormData());
        }

        public void Clear() => _httpContextAccessor.HttpContext.Session.Remove(SessionKey);
    }
}