using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Amazon.Recognition.API.BLL.Params;
using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using Amazon.S3;
using Amazon.S3.Model;
using S3Object = Amazon.Rekognition.Model.S3Object;

namespace Amazon.Recognition.API.BLL
{
    public class AmazonRekognition
    {
        private IAmazonS3 s3Client;
        private IAmazonRekognition recClient;

        private String awsAccessKeyId;
        private String awsSecretAccessKey;

        public AmazonRekognition(String awsAccessKeyId, String awsSecretAccessKey)
        {
            this.awsAccessKeyId = awsAccessKeyId;
            this.awsSecretAccessKey = awsSecretAccessKey;
        }

        #region : DetectLabels :
        public async Task<DetectLabelsResponse> DetectLabels(DetectLabelParams dlp)
        {
            var detectlabelsRequest = new DetectLabelsRequest()
            {
                Image = new Image()
                {
                    Bytes = new MemoryStream(),
                    S3Object = new S3Object()
                    {
                        Bucket = dlp.BucketName,
                        Name = dlp.PhotoName,
                        Version = dlp.PhotoVersion
                    }
                },

                MinConfidence = dlp.MinConfidence,
                MaxLabels = dlp.MaxLabels
            };

            Task<DetectLabelsResponse> detectLabelsResponse = null;
            try
            {
                using (AmazonRekognitionClient recognitionClient = new AmazonRekognitionClient())
                {
                    detectLabelsResponse = recognitionClient.DetectLabelsAsync(detectlabelsRequest);
                }

                Console.WriteLine("Detected labels for " + dlp.PhotoName);
                foreach (Label label in detectLabelsResponse.Result.Labels)
                    Console.WriteLine("{0}: {1}", label.Name, label.Confidence);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return await detectLabelsResponse ?? throw new Exception("response is null");
        }
        #endregion

        #region : DetectFaces :
        public DetectFacesResponse DetectFaces(DetectFaceParams dfp)
        {
            DetectFacesResponse resp = null;
            var conf = new AmazonRekognitionConfig()
            {
                RegionEndpoint = dfp.RegEndpoint
            };

            using (recClient = new AmazonRekognitionClient(awsAccessKeyId, awsSecretAccessKey, conf))
            {
                DetectFacesRequest detectFacesRequest = new DetectFacesRequest()
                {
                    Image = new Image()
                    {
                        S3Object = new S3Object()
                        {
                            Name = dfp.PhotoName,
                            Bucket = dfp.BucketName
                        },
                    },
                    // Attributes can be "ALL" or "DEFAULT". 
                    // "DEFAULT": BoundingBox, Confidence, Landmarks, Pose, and Quality.
                    // "ALL": See https://docs.aws.amazon.com/sdkfornet/v3/apidocs/items/Rekognition/TFaceDetail.html
                    Attributes = new List<String>() { "ALL" }
                };

                try
                {
                    resp = recClient.DetectFaces(detectFacesRequest);

                    if (resp == null)
                    {
                        throw new Exception("AmazonRekognitionClient DetectFaces method call return null.");
                    }
                    //bool hasAll = detectFacesRequest.Attributes.Contains("ALL");
                    //foreach (FaceDetail face in resp.Result.FaceDetails)
                    //{
                    //    Console.WriteLine("BoundingBox: top={0} left={1} width={2} height={3}", face.BoundingBox.Left,
                    //        face.BoundingBox.Top, face.BoundingBox.Width, face.BoundingBox.Height);
                    //    Console.WriteLine("Confidence: {0}\nLandmarks: {1}\nPose: pitch={2} roll={3} yaw={4}\nQuality: {5}",
                    //        face.Confidence, face.Landmarks.Count, face.Pose.Pitch,
                    //        face.Pose.Roll, face.Pose.Yaw, face.Quality);
                    //    if (hasAll)
                    //        Console.WriteLine("The detected face is estimated to be between " +
                    //            face.AgeRange.Low + " and " + face.AgeRange.High + " years old.");
                    //}
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            return resp;
        }
        #endregion

        #region : UploadObject :

        public PutObjectResponse PutObjectIntoS3(PutObjectParams pop)
        {
            var config = new AmazonS3Config()
            {
                RegionEndpoint = pop.RegEndpoint
            };

            PutObjectResponse resp = null;

            try
            {
                var putReq = new PutObjectRequest
                {
                    BucketName = pop.BucketName,
                    Key = pop.PhotoName,
                    InputStream = pop.InputStrem,
                    //FilePath = filePath,
                    ContentType = pop.ContentType
                };

                putReq.Metadata.Add("x-amz-meta-title", "faceRecognitionImages");

                using (s3Client = new AmazonS3Client(awsAccessKeyId, awsSecretAccessKey, config))
                {
                    resp = s3Client.PutObject(putReq);
                }

                if (resp == null)
                {
                    throw new Exception("AmazonS3Client PutObject method call return null.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return resp;
        }

        #endregion

        #region : Recognize :

        public DetectFacesResponse Recognise(RecogniseParams recParam)
        {
            //1. PutObjectIntoS3
            if (String.IsNullOrEmpty(recParam.PhotoName))
            {
                throw new Exception("AmazonRekognition Recognise method: photoName parameter must have value.");
            }

            var photoName = $"{recParam.PhotoName}_{DateTime.Now}";
            var pop = new PutObjectParams()
            {
                PhotoName = photoName,
                PhotoVersion = recParam.PhotoVersion,
                BucketName = recParam.BucketName,
                RegEndpoint = recParam.RegEndpoint,
                InputStrem = recParam.InputStream,
                ContentType = recParam.ContentType
            };

            var resp = PutObjectIntoS3(pop);

            if (resp == null)
            {
                throw new Exception($"PutObjectIntoS3 response is null in Recognise method PhotoName: {photoName}");
            }

            //2. DetectFaces
            var dfp = new DetectFaceParams()
            {
                PhotoName = photoName,
                PhotoVersion = recParam.PhotoVersion,
                BucketName = recParam.BucketName,
                RegEndpoint = recParam.RegEndpoint
            };

            var detectFaceResp = DetectFaces(dfp);

            if (detectFaceResp == null)
            {
                throw new Exception($"DetectFaces response is null in Recognise method PhotoName: {photoName}");
            }

            return detectFaceResp;
        }
        #endregion

    }
}
