using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ManageStores.Helpers
{
    public static class DropDownItems
    {
        public static List<SelectListItem> FileType()
        {
            List<SelectListItem> options = new List<SelectListItem>()
            {
               new SelectListItem
                {
                    Text = "CSV",
                    Value = "1"
                },
               new SelectListItem
                {
                    Text = "Json",
                    Value = "2"
                },
               new SelectListItem
                {
                    Text = "XML",
                    Value = "3"
                },

            };

            return options;
        }
    }
}