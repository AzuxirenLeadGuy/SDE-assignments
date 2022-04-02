using Microsoft.AspNetCore.Mvc;
using Google.Cloud.Storage.V1;
using Google.Cloud.AIPlatform.V1;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.AIPlatform.V1.Schema.Predict.Instance;
using Google.Cloud.AIPlatform.V1.Schema.Predict.Params;
using Google.Cloud.Vision.V1;
using System.Diagnostics;
namespace SDE_A3_M21CS003.Controllers;

[ApiController]
[Route("[controller]")]
public class ImagePredictController : ControllerBase
{
    readonly Random random;
    private readonly ImageAnnotatorClientBuilder builder;
    public ImagePredictController()
    {
        random = new Random();
        builder = new() { JsonCredentials = JsonCredentials };
    }
    public readonly static GoogleCredential credentials = GoogleCredential.FromJson(JsonCredentials);
    internal static string MachineName = "";
    [HttpPost(Name = "GetPrediction")]
    public ResultObject Post()
    {
        string bucketName = "a3-bucket";
        string imagename = "image-a3-" + random.NextInt64().ToString();
        Image img = Image.FromStreamAsync(Request.Body).Result;
        var storage = StorageClient.Create(credentials);
        storage.UploadObject(bucketName, imagename, null, Request.Body);
        Stream downstream = new MemoryStream();
        storage.DownloadObject(bucketName, imagename, downstream);
        ImageAnnotatorClient client = builder.Build();
        var obj = new ResultObject(client.DetectFaces(img).ToArray(), client.DetectLogos(img).ToArray()) { MachineName = MachineName };
        return obj;
    }
    public const string JsonCredentials = "{\"type\": \"service_account\", \"project_id\": \"assignment3-344904\", \"private_key_id\": \"e28eec3faca68cbe3a15a58f3d1ee062718a962e\", \"private_key\": \"-----BEGIN PRIVATE KEY-----\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQDIoUVzxJ5cq1CU\nRnxOsqXLZCK37VW/53gu+VNboidqevMQv0Hlwu7GnHqGWHGFGQeJTDgczLkIrqrK\nF3fWOFbJKHF85H1Qr5JrS5slLjzJEKl65YD2tSYpHXWj0g/jDYjDvrvH5IomKB5+\nHZujcFukMqc5Kkn1t1YaRiJ8om4BGI8z5c6Dt60HcHWR2FWLfcozAnb18SwUy9UL\nBBGy3KL44watkPRF1vyrAAIGCaaI+r7z6tV8i5UsmaY+MGwy8o+25lUsdbz9FUMq\nzf3dB+79ta/7POA3SvTbprb5NcJK8IPFSP999sReBIev2PP5BQpoJyKrAIPkY0A5\nUiZYfmF1AgMBAAECggEADyxF0LPdoiEr+Tq8wJrkIlX9W4RDHZNHfzid5D/cKUJe\ndYrGqgtW/DBDV+ZXwiVwG3hnL05lmVF/BcTxQYMwlQqjopAJJiamX8pE/nKDvapZ\nQZEacrilHSuRhQUoQcH5RjcmLmo17x14841mWDLxdfaFYl5fX4ow+dBfX/uiGXT8\nCgvTRk5gTfIEVgFcmhklHWKgxGqPNwB9oBMD+br78TEgBeFSDTQqs0nhMpEAwyYX\nNq2UvLEPTx89O3ITBtkXXBHfyE8qTFcYdS4tqhFFseZfw+yoIXLpjh64sDEsWpKu\nOCndXGIi5qt41fQlCS2XNmme01gT7h1fqqb8RJohwQKBgQDud2eMe48e3VxlVIR7\nl70p+r7j2YYWRKngl54mOarqfx+yL3ds4AFQjttCuP6KbFmqZ12cp/XG+CAMgP6d\nNxl4Ol702h2CYfBMXRwNi6Ydm+ouhdPXbnl/jJEijY9aa6FBE+VZQb19UFxVnWZr\n2mSEEfqBwnKuQXpJ03fK+pgzpQKBgQDXYa4sb1dEnSPwsmJv8Lw94ha7z022/LA0\nOlecIhzRnuEji1bOzd2qfDgS+EpzlW518MPu+PVBtGLstPvB0RePxgQn24hahHvR\nNFg2LAHYYWU4KuPxn0EKpdOdMD2/6oQgpUxuUZlMqK1Vp6azKapNEpbCXOll0axI\npGue/9bNkQKBgHEzv7VBxoxJWx5DQ+0D+GmgPdQeI/gYOmiCsTHGOGu/U0WcIN5P\nbHBYkJ+ZAESn8Nzi2t6OIS+GBHjLF5N8X5viYcdsb4vAgHlPzgxkGcR+imnxX8Iq\nEIVLLm6vsrii1Vb2Ye7ANXrylhKSeNeEQikr+7Zzn0s3jBzVxU1XLr99AoGAH289\nOXUCmpwIq8+NOGM9RhbiVITcBdH5cZX2RW5hIGL83vAuOj6nlINOt84PJ7Fujr4W\nCqHBQdau/Xcoq0/2DWDMKHkM/JELdSwbl8RyfH8yg7EW1aP1nTU4nl91E7aJlC4r\n6on0QtD3g2HVnZ5+IYOOB04CLn8ckvmUT39tfVECgYEA1hBeXGObaBf54GW8jy86\nXO6N41ugH9gInMntl2bwE71zXF3yFLKV1WeJX28udn30M5rIw+Tx4gFV60wC1R7C\n0AKN+v25ztymY9n2f36AHrMemr+SU1ZVBCwSDntEuiq+I0a5EMJKimoFWcDDI86t\n4g1Y0SEz4okXdDqZhp2SelI=\n-----END PRIVATE KEY-----\n\", \"client_email\": \"ajsam-14@assignment3-344904.iam.gserviceaccount.com\", \"client_id\": \"107141962401276170929\", \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\", \"token_uri\": \"https://oauth2.googleapis.com/token\", \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\", \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/ajsam-14%40assignment3-344904.iam.gserviceaccount.com\" }";
}


/// <summary>
/// The structure of the result returned by this API
/// </summary>
public class ResultObject
{
    public string MachineName { get; set; }
    /// <summary>
    /// Collection of faces in the image
    /// </summary>
    /// <value></value>
    public FaceObject[] Faces { get; set; }
    /// <summary>
    /// Collection of logos in the image
    /// </summary>
    /// <value></value>
    public LogoObject[] Logos { get; set; }
    /// <summary>
    /// Construuctor for this class
    /// </summary>
    private ResultObject()
    {
        MachineName = "";
        Faces = Array.Empty<FaceObject>();
        Logos = Array.Empty<LogoObject>();
    }
    /// <summary>
    /// Construuctor for this class
    /// </summary>
    public ResultObject(FaceAnnotation[] facesAnn, EntityAnnotation[] logosAnn)
    {
        MachineName = "";
        Faces = new FaceObject[facesAnn.Length];
        Logos = new LogoObject[logosAnn.Length];
        int i;
        for (i = facesAnn.Length - 1; i >= 0; i--)
        {
            Faces[i] = new(facesAnn[i]);
        }
        for (i = logosAnn.Length - 1; i >= 0; i--)
        {
            Logos[i] = new(logosAnn[i]);
        }
    }
    /// <summary>
    /// object returned for invalid input or any other exception
    /// </summary>
    public static ResultObject Default => new();
}
/// <summary>
/// An object denoting a face in the image
/// </summary>
public class FaceObject
{
    /// <summary>
    /// The confidence value for this portion of the image is a Face
    /// </summary>
    /// <value></value>
    public float Confidence { get; set; }
    /// <summary>
    /// How likely is this face happy
    /// </summary>
    /// <value></value>
    public string JoyLiklihood { get; set; }
    /// <summary>
    /// How likely is this face Sad
    /// </summary>
    /// <value></value>
    public string SadLiklihood { get; set; }
    /// <summary>
    /// How likely is this face angry
    /// </summary>
    /// <value></value>
    public string AngryLiklihood { get; set; }
    /// <summary>
    /// Constructor for this class
    /// </summary>
    /// <param name="face"></param>
    public FaceObject(FaceAnnotation face)
    {
        Confidence = face.DetectionConfidence;
        JoyLiklihood = Enum.GetName(face.JoyLikelihood) ?? "?";
        SadLiklihood = Enum.GetName(face.SorrowLikelihood) ?? "?";
        AngryLiklihood = Enum.GetName(face.AngerLikelihood) ?? "?";
    }
}
/// <summary>
/// An object denoting a Logo in the image
/// </summary>
public class LogoObject
{
    /// <summary>
    /// The confidence value for this portion of image is Logo
    /// </summary>
    /// <value></value>
    public float Confidence { get; set; }
    /// <summary>
    /// Description of this logo
    /// </summary>
    /// <value></value>
    public string Description { get; set; }
    /// <summary>
    /// Constructor for this class
    /// </summary>
    /// <param name="logo"></param>
    public LogoObject(EntityAnnotation logo)
    {
        Confidence = logo.Score / 10.0f;
        Description = logo.Description;
    }
}