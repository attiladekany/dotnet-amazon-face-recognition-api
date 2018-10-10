using System;
using System.IO;

namespace Amazon.Recognition.API.BLL.Params
{
    public class RecogniseParams : BaseRecognitionParameters
    {
        public Stream InputStream { get; set; }
        public String ContentType { get; set; } = "image/jpeg";
    }
}
