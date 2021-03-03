using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Hosting;
using Spice.Models;
using Spice.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Utility.Filters
{
    public class HandleException : IExceptionFilter
    {
        private readonly IWebHostEnvironment env;
        private readonly IModelMetadataProvider prov;

        public HandleException(IWebHostEnvironment env,IModelMetadataProvider prov)
        {
            this.env = env;
            this.prov = prov;
        }
        public void OnException(ExceptionContext context)
        {
            if (env.IsDevelopment())
            {
                var vm = new ExcepViewModel()
                {
                    Message = context.Exception.Message,
                    Title = "Internal Server Exception"
                };
                var r = new RedirectToActionResult("HandleException", "Home", new { area = "Customer" });
                r.RouteValues = new RouteValueDictionary(vm);
                //r.ExecuteResult(context);
                
                var result = new ViewResult { ViewName = "HandleException" };
                result.ViewData = new ViewDataDictionary(prov, context.ModelState);
                result.ViewData.Add("ExcepViewModel", vm);

                context.Result = r;

            }
        }
    }
}
