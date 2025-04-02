using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace AppCalisto.Helpers
{
    public interface ICompanyHelper
    {
        public IEnumerable<SelectListItem> GetAll();
    }
}
