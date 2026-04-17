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
        [HttpGet("results/{pageId}")]
        public async Task<IActionResult> Index(string pageId)
        {
            var pageMetadata = await _cmsService.GetPageAsync(pageId);
            var userSubmission = _sessionService.GetConsolidatedSubmission();

            if (pageMetadata == null) return NotFound();

            // Logic to pull specific data for the "What Jack can get" heading
            var nameAnswer = userSubmission.Answers.FirstOrDefault(a => a.FieldId == "child-name");
            ViewData["ChildName"] = nameAnswer?.GetRawValue() ?? "your child";

            return View(pageMetadata);
        }
    }
}
