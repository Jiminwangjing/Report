
using KEDI.Core.Premise.Resources;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Localization;
using System;
using System.Reflection;

namespace KEDI.Core.Localization.Resources
{
    public class CultureLocalizer
    {
        private readonly IStringLocalizer _localizer;
        public CultureLocalizer(IStringLocalizerFactory factory)
        {
            var type = typeof(ViewResource);
            _localizer = factory.Create(type.Name, type.Assembly.FullName);
        }

        // if we have formatted string we can provide arguments         
        // e.g.: @Localizer.Text("Hello {0}", User.Name)
        public LocalizedString GetValue(string key, params string[] arguments)
        {
            return arguments == null ? _localizer[key] : _localizer[key, arguments];
        }

        public LocalizedString this[string key, params string[] args]
        {
            get { return GetValue(key, args); }
        }
    }
}
