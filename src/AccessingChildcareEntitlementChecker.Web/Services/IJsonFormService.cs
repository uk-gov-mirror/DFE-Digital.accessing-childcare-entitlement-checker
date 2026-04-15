using AccessingChildcareEntitlementChecker.Web.Models;

namespace AccessingChildcareEntitlementChecker.Web.Services
{
    public interface IJsonFormService
    {
        Task<FormPage> GetPageAsync(string pageId);
        string ResolveNextPage(FormPage currentPage, FormSubmission submission);
    }
}