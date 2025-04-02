using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace AppCalisto.Helpers
{
    public class CompanyHelper : ICompanyHelper
    {
        private readonly IConfiguration _configuration;

        public CompanyHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IEnumerable<SelectListItem> GetAll()
        {
            var companies = _configuration.GetSection("Companies").Get<List<string>>() ?? new List<string>();
            return companies.Select(c => new SelectListItem { Text = c, Value = c }).ToList();
        }
    }
}
