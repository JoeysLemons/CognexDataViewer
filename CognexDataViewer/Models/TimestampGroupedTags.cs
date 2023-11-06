using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CognexDataViewer.Models
{
    public class TimestampGroupedTags
    {
        public string Timestamp { get; set; }
        public List<Tag> Items { get; set; }
    }
}
