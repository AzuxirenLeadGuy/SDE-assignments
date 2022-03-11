using Microsoft.AspNetCore.Mvc;
using Google.Cloud.Vision.V1;

namespace TaskC.Controllers;

[ApiController]
[Route("[controller]")]
public class TaskCController : ControllerBase
{
    private readonly ILogger<TaskCController> _logger;
    private readonly ImageAnnotatorClientBuilder builder;
    public TaskCController(ILogger<TaskCController> logger)
    {
        _logger = logger;
        builder = new();
        builder.JsonCredentials = System.IO.File.ReadAllText(Program.KeyPath);
    }
    [HttpPost(Name = "LoadFile")]
    public ResultObject Post()
    {
        _logger.LogInformation("Reading file {path}", "image");
        try
        {
            Image img = Image.FromStreamAsync(Request.Body).Result;
            ImageAnnotatorClient client = builder.Build();
            return new(client.DetectFaces(img).ToArray(), client.DetectLogos(img).ToArray());
        }
        catch (Exception ex)
        {
            _logger.LogCritical("Could not read the file at the given path");
            _logger.LogError("{}", ex.Message);
            return ResultObject.Default;
        }
    }
}
/// <summary>
/// The structure of the result returned by this API
/// </summary>
public class ResultObject
{
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
        Faces = Array.Empty<FaceObject>();
        Logos = Array.Empty<LogoObject>();
    }
    /// <summary>
    /// Construuctor for this class
    /// </summary>
    public ResultObject(FaceAnnotation[] facesAnn, EntityAnnotation[] logosAnn)
    {
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