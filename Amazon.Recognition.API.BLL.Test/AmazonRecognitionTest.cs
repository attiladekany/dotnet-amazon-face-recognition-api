using System;
using System.IO;
using System.Linq;
using Amazon.Recognition.API.BLL.Params;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Amazon.Recognition.API.BLL.Test
{
    [TestClass]
    public class AmazonRecognitionTest
    {
        String awsAccessKeyId = "";
        String awsSecretAccessKey = "";

        [TestMethod]
        public void DetectLabelsTest()
        {
            //Arrange
            var param = new DetectLabelParams()
            {
                BucketName = "nvirginiadekanybucket",
                PhotoName = "",
                PhotoVersion = "1",
                MaxLabels = 10,
                MinConfidence = 75F
            };

            //Act
            AmazonRekognition service = new AmazonRekognition(awsAccessKeyId, awsSecretAccessKey);
            var resp = service.DetectLabels(param);

            //Assert
        }

        [TestMethod]
        public void DetectFacesTest()
        {
            //Arrange
            var param = new DetectFaceParams()
            {
                BucketName = "nvirginiadekanybucket",
                PhotoName = "steve",
                PhotoVersion = String.Empty
            };

            AmazonRekognition service = new AmazonRekognition(awsAccessKeyId, awsSecretAccessKey);

            //Act
            var resp = service.DetectFaces(param);
            bool hasItem = resp.FaceDetails.Any();

            //Assert
            Assert.AreEqual(true, hasItem);
        }

        [TestMethod]
        public void RecogniseTest()
        {
            Stream stream = new MemoryStream(GetFileBytes());

            //Arrange
            var req = new RecogniseParams()
            {
                PhotoName = "steve",
                PhotoVersion = String.Empty,
                BucketName = "nvirginiadekanybucket",
                RegEndpoint = RegionEndpoint.USEast1,
                InputStream = stream,
                ContentType = "image/jpeg"
            };

            //Act
            AmazonRekognition service = new AmazonRekognition(awsAccessKeyId, awsSecretAccessKey);
            var resp = service.Recognise(req);

            //Assert
            Assert.AreEqual(System.Net.HttpStatusCode.OK, resp.HttpStatusCode);

        }

        [TestMethod]
        public void GetImage()
        {
            var file = GetFileBytes();
            var length = file.Length;

            Assert.AreEqual(13976, length);
        }


        public byte[] GetFileBytes()
        {
            byte[] result = null;
            var appdomainBasedir = AppDomain.CurrentDomain.BaseDirectory;

            var dirInfo = new DirectoryInfo(appdomainBasedir);
            var root = dirInfo.Parent.Parent;

            var files = root.GetFiles();

            var imagePath = String.Empty;
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].Name.Contains("steve"))
                {
                    imagePath = files[i].FullName;
                }
            }

            if (!String.IsNullOrEmpty(imagePath))
            {
                result = File.ReadAllBytes(imagePath);
                //Console.WriteLine($"hossz: {bytes.Length}");
            }

            return result;
        }

    }
}
