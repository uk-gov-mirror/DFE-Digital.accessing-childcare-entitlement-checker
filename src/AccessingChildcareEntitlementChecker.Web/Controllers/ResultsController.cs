using AccessingChildcareEntitlementChecker.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace AccessingChildcareEntitlementChecker.Web.Controllers
{
    public class ResultsController : Controller
    {
        private readonly ICmsFormService _cmsService;
        private readonly ISessionService _sessionService;

        public ResultsController(ICmsFormService cmsService, ISessionService sessionService)
        {
            _cmsService = cmsService;
            _sessionService = sessionService;
        }
        [HttpGet("results")]
        public async Task<IActionResult> Index()
        {
            var pageMetadata = await _cmsService.GetResultsPageAsync();
            var userSubmission = _sessionService.GetConsolidatedSubmission();

            if (pageMetadata == null) return NotFound();

            // Logic to pull specific data for the "What Jack can get" heading
            var nameAnswer = userSubmission.Answers.FirstOrDefault(a => a.FieldId == "childname");
            ViewData["ChildName"] = nameAnswer?.GetRawValue() ?? "your child";
            //pageMetadata.Guidance.Content = pageMetadata.Guidance.Content.ToString().Replace("{ChildName}", nameAnswer?.GetRawValue() ?? "your child");

            return View(pageMetadata);
        }
    }
}
