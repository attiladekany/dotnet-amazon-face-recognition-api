using Amazon.Recognition.API.BLL.Params;
using System;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web.Http;
using Amazon.Recognition.API.BLL;

namespace Amazon.Recognition.API.Controllers
{
    public class RecognizeController : ApiController
    {
        [System.Web.Mvc.HttpPost]
        //public IHttpActionResult Recognize([FromBody]String base64String)
        public IHttpActionResult Recognize(RecognizeParam param)
        {
            if (String.IsNullOrEmpty(param.Base64String))
            {
                return BadRequest("Bad request: base64String parameter must have value.");
            }

            var stream = new MemoryStream();
            var photoBytes = Convert.FromBase64String(param.Base64String);

            using (var ms = new MemoryStream(photoBytes, 0, photoBytes.Length))
            {
                Image image = Image.FromStream(ms, true);
                image.Save(stream, ImageFormat.Jpeg);
            }

            var awsAccessKeyId = ConfigurationManager.AppSettings["awsAccessKeyId"];
            var awsSecretAccessKey = ConfigurationManager.AppSettings["awsSecretAccessKey"];
            var bucketName = ConfigurationManager.AppSettings["bucketName"];
            var photoName = $"{DateTime.Now}_{Guid.NewGuid()}";

            var req = new RecogniseParams()
            {
                PhotoName = photoName,
                PhotoVersion = "1",
                BucketName = bucketName,
                RegEndpoint = RegionEndpoint.USEast1,
                InputStream = stream,
                ContentType = "image/jpeg"
            };

            AmazonRekognition service = new AmazonRekognition(awsAccessKeyId, awsSecretAccessKey);
            var resp = service.Recognise(req);

            return Ok(resp);
        }

        [HttpGet]
        public IHttpActionResult Recognize()
        {
            var test = "ok";
            return Json(new
            {
                test
            });
        }

    }
}
