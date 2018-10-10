using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amazon.Recognition.API.BLL.Params
{
    public class BaseRecognitionParameters
    {
        public RegionEndpoint RegEndpoint { get; set; } = RegionEndpoint.USEast1; //(N.Virginia)
        public String BucketName { get; set; }
        public String PhotoName { get; set; }
        public String PhotoVersion { get; set; }
    }
}
