using AccessingChildcareEntitlementChecker.Web.Models;
using AccessingChildcareEntitlementChecker.Web.Services;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AccessingChildcareEntitlementChecker.Web.ModelBinder
{
    public class FormSubmissionModelBinder : IModelBinder
    {
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null) throw new ArgumentNullException(nameof(bindingContext));

            // 1. Extract the Page ID from the route data
            var pageId = bindingContext.ValueProvider.GetValue("pageId").FirstValue;
            if (string.IsNullOrEmpty(pageId))
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return;
            }

            // 2. Resolve the CMS Service via the RequestServices (DI)
            var cmsService = bindingContext.HttpContext.RequestServices.GetRequiredService<ICmsFormService>();
            var currentPage = await cmsService.GetPageAsync(pageId);

            if (currentPage == null)
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return;
            }

            var submission = new FormSubmission { PageId = pageId };
            var form = bindingContext.HttpContext.Request.Form;

            // 3. Polymorphic Mapping based on Contentful Metadata
            foreach (var field in currentPage.Fields)
            {
                AnswerBase answer = field.FieldType.ToLower() 
                switch
                {
                    "radios" => new SingleValueAnswer
                    {
                        FieldId = field.FieldId,
                        SelectedValue = form[field.FieldId]
                    },
                    "checkboxes" => new MultiValueAnswer
                    {
                        FieldId = field.FieldId,
                        Values = form[field.FieldId].ToList()
                    },
                    "date" => new DateAnswer
                    {
                        FieldId = field.FieldId,
                        Day = form[$"{field.FieldId}-day"],
                        Month = form[$"{field.FieldId}-month"],
                        Year = form[$"{field.FieldId}-year"]
                    },
                    _ => new TextAnswer
                    {
                        FieldId = field.FieldId,
                        Value = form[field.FieldId]
                    }
                };

                submission.Answers.Add(answer);
            }

            bindingContext.Result = ModelBindingResult.Success(submission);
        }
    }
}
