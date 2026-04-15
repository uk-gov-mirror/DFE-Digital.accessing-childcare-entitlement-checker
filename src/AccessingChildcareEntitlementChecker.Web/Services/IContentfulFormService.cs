using AccessingChildcareEntitlementChecker.Web.Models;

namespace AccessingChildcareEntitlementChecker.Web.Services
{
    public interface IContentfulFormService
    {
        Task<FormPage> GetPageAsync(string pageId);
        string ResolveNextPage(FormPage currentPage, Dictionary<string, string> userAnswers);
        string ResolveNextPage(FormPage currentPage, FormSubmission submission);
        string ResolveNextPage(FormPage currentPage, string userAnswer);
    }
}