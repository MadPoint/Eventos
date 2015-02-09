using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MadPoint.BlogViewer.AppWeb.Models
{
    public class BlogEntryModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Author { get; set; }
        public string Date { get; set; }
    }
}