using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CognexDataViewer.Models
{
    public class Tag
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public List<TagMeasurement> Measurements { get; set; } = new List<TagMeasurement>();

        public Tag(string name)
        {
            Name = name;
        }
    }
}
