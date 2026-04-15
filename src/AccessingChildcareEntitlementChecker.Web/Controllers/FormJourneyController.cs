using AccessingChildcareEntitlementChecker.Web.Models;
using AccessingChildcareEntitlementChecker.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AccessingChildcareEntitlementChecker.Web.ModelBinder;


namespace AccessingChildcareEntitlementChecker.Web.Controllers
{
    public class FormJourneyController : Controller
    {
        private readonly JsonFormService _cmsService;


        public FormJourneyController(JsonFormService cmsService)
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

        ///// <summary>
        ///// POST: Processes user input and calculates the next destination.
        ///// </summary>
        //[HttpPost("journey/{pageId}")]
        //[ValidateAntiForgeryToken] // AZ-500: Prevents Cross-Site Request Forgery
        //public async Task<IActionResult> Process(string pageId, IFormCollection form)
        //{
        //    var currentPage = await _cmsService.GetPageAsync(pageId);

        //    // Map form data to a dictionary for the Logic Engine
        //    // AZ-500: Sanitize inputs here if necessary
        //    var answers = new Dictionary<string, string>();
        //    foreach (var key in form.Keys)
        //    {
        //        answers.Add(key, form[key]);
        //    }

        //    // Use the Logic Engine from our Service to find the next slug
        //    string nextPageId = _cmsService.ResolveNextPage(currentPage, answers);

        //    if (string.IsNullOrEmpty(nextPageId))
        //    {
        //        // Fallback if no routing matches and no default is set
        //        return RedirectToAction("Index", new { pageId = "page-error" });
        //    }

        //    return RedirectToAction("Index", new { pageId = nextPageId });
        //}


        /// <summary>
        /// POST: Processes user input and calculates the next destination.
        /// </summary>
        [HttpPost("journey/{pageId}")]
        [ValidateAntiForgeryToken] // AZ-500: Prevents Cross-Site Request Forgery
          public async Task<IActionResult> Process(
            string pageId,
            [ModelBinder(BinderType = typeof(FormSubmissionModelBinder))] FormSubmission submission)
        {
            // The ModelBinder has already:
            // 1. Fetched metadata from Contentful.
            // 2. Mapped raw form fields to typed polymorphic objects (DateAnswer, SingleValueAnswer, etc.).
            // 3. Ignored any 'junk' or malicious fields not defined in the CMS.

            if (submission == null)
            {
                return BadRequest("Invalid form submission structure.");
            }

            // Fetch metadata again for validation and view re-rendering
            var currentPage = await _cmsService.GetPageAsync(pageId);
            if (currentPage == null) return NotFound();

            // 1. Perform Server-Side Validation using CMS-driven metadata
            // AZ-500: Never rely solely on client-side GDS validation.
            var validationResults = submission.Validate(currentPage);

            if (validationResults.Any())
            {
                foreach (var error in validationResults)
                {
                    // Map validation errors to ModelState for GDS-compliant error display
                    ModelState.AddModelError(error.MemberNames.First(), error.ErrorMessage);
                }

                // Return to the Index view to display the Error Summary and messages
                return View("Index", currentPage);
            }

            if(submission !=null)
            {
                string nextPageId = _cmsService.ResolveNextPage(currentPage, submission);

                if (!string.IsNullOrEmpty(nextPageId))
                {
                    // Fallback if no rules match and no default is set
                   // return RedirectToAction("Index", new { pageId = "page-error" });
                    return RedirectToAction("Index", new { pageId = nextPageId });
                }

            }
            return BadRequest("Invalid form submission structure.");
            // 2. Execute Routing Logic

            // 3. Move to the next step in the journey

        }


    }
}

