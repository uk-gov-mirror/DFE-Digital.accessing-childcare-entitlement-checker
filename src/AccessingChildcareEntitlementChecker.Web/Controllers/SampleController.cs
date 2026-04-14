using Contentful.Core;
using Microsoft.AspNetCore.Mvc;

namespace AccessingChildcareEntitlementChecker.Web.Controllers
{
    public class SampleController : Controller
    {
        private readonly IContentfulClient _client;

        public SampleController(IContentfulClient client)
        {
            _client = client;
        }
    }
}
