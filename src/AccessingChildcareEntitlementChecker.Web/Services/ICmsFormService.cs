using AccessingChildcareEntitlementChecker.Web.Models;

namespace AccessingChildcareEntitlementChecker.Web.Services
{
    public interface ICmsFormService
    {
        Task<FormPage> GetPageAsync(string pageId);
        string ResolveNextPage(FormPage currentPage, FormSubmission submission);
    }
}