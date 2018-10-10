using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace Amazon.Recognition.API.BLL.Params
{
    public class PutObjectParams : BaseRecognitionParameters
    {
        public Stream InputStrem { get; set; }
        public String ContentType { get; set; } = "image/jpeg";
    }
}
