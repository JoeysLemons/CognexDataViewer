using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CognexDataViewer.Models
{
    public class CameraData
    {
        public string DeviceName { get; set; }
        public string JobName { get; set; }
        public string TimeStamp { get; set; }
        public List<TagMeasurement> Measurements { get; set; }
    }
}
