using System.Collections.Generic;

namespace MadPoint.BlogViewer.AppWeb.Models
{
    public class ViewerModel
    {
        public string BlogTitle { get; set; }
        public string BlogUrl { get; set; }
        public IEnumerable<BlogEntryModel> BlogEntries { get; set; }
    }
}