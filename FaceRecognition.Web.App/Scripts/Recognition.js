function environmentalDetectionCallback(result) {
    if (result != null && !result.checksPassed) {
        alert('Please use Firefox or Chrome and be equipped by a webcam!');
    }
}
var _RealeyesitEnvDetectParams = _RealeyesitEnvDetectParams || {};
_RealeyesitEnvDetectParams._callback = environmentalDetectionCallback;
(function () {
    var envDetect = document.createElement('script');
    envDetect.type = 'text/javascript';
    envDetect.async = true;
    envDetect.src = 'https://codesdwncdn.realeyesit.com/environment-checker/release/3/Realeyesit.EnvironmentalDetectionAPI.min.js';
    var s = document.getElementsByTagName('script')[0];
    s.parentNode.insertBefore(envDetect, s);
})();

//1. Registrate free account to Amazon
//2. check and understand FaceRecognition API
//3. Create snapshot from html5 live video stream
//4. Create Analyze api endpoint
//5. Call ajax request when recognize button clicked

function Recognize() {
    var video = document.querySelector('#video');
    var canvas = document.querySelector('#canvas');
    var img = document.querySelector('#img');

    //canvas.width = video.videoWidth; //500; 
    //canvas.height = video.videoHeight; //375; 

    canvas.getContext('2d').drawImage(video, 0, 0);
    // Other browsers will fall back to image/png
    img.src = canvas.toDataURL('image/jpeg');

    //get base64 str of img src
    var encodedString = img.src.split(',')[1];
    if (encodedString != null) {
        //console.log(encodedString);

        // call ajax pass the encoded string
        var jsonData = JSON.stringify({
            base64String: encodedString
        });

        RecognizeAjax(jsonData);
    }
}

function ClearCanvas(canvas, context) {
    context.clearRect(0, 0, canvas.width, canvas.height);
    context.beginPath();
}

//Ref.: https://docs.aws.amazon.com/AWSJavaScriptSDK/latest/AWS/Rekognition.html#detectFaces-property
function DrawRectangles(boundingBox) {
    var canvas = document.getElementById('canvas-face-rect');
    var context = canvas.getContext('2d');
    ClearCanvas(canvas, context);

    //document.getElementById('canvas-face-rect').getContext('2d').clearRect(0,
    //    0,
    //    document.getElementById('canvas-face-rect').width,
    //    document.getElementById('canvas-face-rect').height);

    var x = parseInt(canvas.width * boundingBox.Left);
    var y = parseInt(canvas.height * boundingBox.Top);
    var w = parseInt(canvas.width * boundingBox.Width);
    var h = parseInt(canvas.height * boundingBox.Height);

    console.log('boundingBox');
    console.log(boundingBox);
    console.log('x: ' + x + '|' + ' y: ' + y + '|' + ' w: ' + w + '|' + ' h: ' + h);

    context.rect(x, y, w, h);
    context.strokeStyle = "#14ca00cc";
    context.lineWidth = "2";
    context.stroke();
}

function CreateResultHtml(faceDetail, htmlResult, index) {

    if (faceDetail.Gender != null) {
        htmlResult += "Gender: " + faceDetail.Gender.Value.Value + "<br/>";
    }
    if (faceDetail.AgeRange != null) {
        htmlResult += "AgeRange: " + "Low: " + faceDetail.AgeRange.High + " Low: " + faceDetail.AgeRange.Low + "<br/>";
    }
    if (faceDetail.Smile != null) {
        htmlResult += "Smile: " + (faceDetail.Smile.Value ? "yes" : "no") + " | Confidence: " + faceDetail.Smile.Confidence + "<br/>";
    }
    if (faceDetail.Beard != null) {
        htmlResult += "Has beard: " + (faceDetail.Beard.Value ? "yes" : "no") + " | Confidence: " + faceDetail.Beard.Confidence + "<br/>";
    }
    if (faceDetail.Emotions != null && faceDetail.Emotions.length === 3) {
        htmlResult += "Emotions: " + faceDetail.Emotions[0].Type.Value + " | Confidence: " + faceDetail.Emotions[0].Confidence + "<br/>";
    }
    if (faceDetail.Emotions != null && faceDetail.Emotions.length === 3) {
        htmlResult += "Emotions: " + faceDetail.Emotions[1].Type.Value + " | Confidence: " + faceDetail.Emotions[1].Confidence + "<br/>";
    }
    if (faceDetail.Emotions != null && faceDetail.Emotions.length === 3) {
        htmlResult += "Emotions: " + faceDetail.Emotions[2].Type.Value + " | Confidence: " + faceDetail.Emotions[2].Confidence + "<br/>";
    }
    if (faceDetail.Eyeglasses != null) {
        htmlResult += "Eyeglasses: " + (faceDetail.Eyeglasses.Value ? "yes" : "no") + " | Confidence: " + faceDetail.Eyeglasses.Confidence + "<br/>";
    }
    if (faceDetail.EyesOpen != null) {
        htmlResult += "EyesOpen: " + (faceDetail.EyesOpen.Value ? "yes" : "no") + " | Confidence: " + faceDetail.EyesOpen.Confidence + "<br/>";
    }
    if (faceDetail.MouthOpen != null) {
        htmlResult += "MouthOpen: " + (faceDetail.MouthOpen.Value ? "yes" : "no") + " | Confidence: " + faceDetail.MouthOpen.Confidence + "<br/>";
    }
    if (faceDetail.Mustache != null) {
        htmlResult += "Mustache: " + (faceDetail.Mustache.Value ? "yes" : "no") + " | Confidence: " + faceDetail.Mustache.Confidence + "<br/>";
    }
    if (faceDetail.Sunglasses != null) {
        htmlResult += "Sunglasses: " + (faceDetail.Sunglasses.Value ? "yes" : "no") + " | Confidence: " + faceDetail.Sunglasses.Confidence + "<br/>";
    }


    console.log(faceDetail.BoundingBox);
    return htmlResult;
}

var faceDetails;
function RenderResult(response) {
    var resultDiv = $('#result');
    resultDiv[0].innerHTML = '';
    if (response != null && response.FaceDetails.length >= 0) {
        faceDetails = response.FaceDetails;

        var facesCount = response.FaceDetails.length;
        var htmlResult = "Faces: " + facesCount + "<br/>";

        var line = document.createElement("HR");
        resultDiv[0].appendChild(line);

        for (var i = 0; i < faceDetails.length; i++) {
            var div = document.createElement('div');

            div.innerHTML += CreateResultHtml(faceDetails[i], htmlResult, i);
            resultDiv[0].appendChild(div);

            if (faceDetails.length > 0) {
                resultDiv[0].appendChild(line);
            }

            DrawRectangles(response.FaceDetails[i].BoundingBox);
        }

    }
}

function RecognizeAjax(jsonData) {
    $.ajax({
        type: 'POST',
        case: false,
        contentType: 'application/json; charset=utf-8',
        url: '/api/Recognize/Recognize/',
        data: jsonData,
        success: function (response, status, xhr) { RenderResult(response); console.log(response); console.log('status: ' + status) },
        error: function (event, jqxhr, settings, thrownError) { },
        complete: function () { }

    });
}