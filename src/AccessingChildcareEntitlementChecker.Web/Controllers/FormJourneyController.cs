using AccessingChildcareEntitlementChecker.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace AccessingChildcareEntitlementChecker.Web.Controllers
{
    public class FormJourneyController : Controller
    {
        private readonly ContentfulFormService _cmsService;

        public FormJourneyController(ContentfulFormService cmsService)
        {
            _cmsService = cmsService;
        }

        /// <summary>
        /// GET: Displays a dynamic page from Contentful.
        /// Defaults to 'page-user-role' for the POC start. https://localhost:7245/journey/page-user-role
        /// </summary>
        [HttpGet("journey/{pageId?}")]
        public async Task<IActionResult> Index(string pageId = "user-role-page")
        {
            
            
            var page = await _cmsService.GetPageAsync(pageId);

            if (page == null)
            {
                return NotFound("Page not found in Contentful. Check Page ID and Publish status.");
            }

            return View(page);
        }

        /// <summary>
        /// POST: Processes user input and calculates the next destination.
        /// </summary>
        [HttpPost("journey/{pageId}")]
        [ValidateAntiForgeryToken] // AZ-500: Prevents Cross-Site Request Forgery
        public async Task<IActionResult> Process(string pageId, IFormCollection form)
        {
            var currentPage = await _cmsService.GetPageAsync(pageId);

            // Map form data to a dictionary for the Logic Engine
            // AZ-500: Sanitize inputs here if necessary
            var answers = new Dictionary<string, string>();
            foreach (var key in form.Keys)
            {
                answers.Add(key, form[key]);
            }

            // Use the Logic Engine from our Service to find the next slug
            string nextPageId = _cmsService.ResolveNextPage(currentPage, answers);

            if (string.IsNullOrEmpty(nextPageId))
            {
                // Fallback if no routing matches and no default is set
                return RedirectToAction("Index", new { pageId = "page-error" });
            }

            return RedirectToAction("Index", new { pageId = nextPageId });
        }
    }
}
