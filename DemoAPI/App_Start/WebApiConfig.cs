using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web.Http;

namespace DemoAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // config.Formatters.JsonFormatter.SupportedMediaTypes
            //   .Add(new MediaTypeHeaderValue("application/json"));

            //Clear current formatters
            config.Formatters.Clear();

            //Add only a json formatter
           config.Formatters.Add(new JsonMediaTypeFormatter());
        }
    }
}
