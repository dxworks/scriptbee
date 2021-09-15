using System;
using Microsoft.AspNetCore.Mvc.Routing;

namespace ScriptBeeWebApp.Controllers
{
    public class ApiControllerRouteAttribute : Attribute, IRouteTemplateProvider
    {
        public string Template => "api/[controller]";
        public int? Order => 2;
        public string Name { get; set; }
    }
}