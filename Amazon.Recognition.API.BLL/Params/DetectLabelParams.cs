using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amazon.Recognition.API.BLL.Params
{
    public class DetectLabelParams : BaseRecognitionParameters
    {
        public Int32 MaxLabels { get; set; }
        public float MinConfidence { get; set; }
    }
}
