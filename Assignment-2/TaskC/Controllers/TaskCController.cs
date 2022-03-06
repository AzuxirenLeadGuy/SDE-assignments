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

public class ResultObject
{
    public FaceObject[] Faces { get; set; }
    public LogoObject[] Logos { get; set; }
    private ResultObject()
    {
        Faces = Array.Empty<FaceObject>();
        Logos = Array.Empty<LogoObject>();
    }
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
    public static ResultObject Default => new();
}
public class FaceObject
{
    public float Confidence { get; set; }
    public string JoyLiklihood { get; set; }
    public string SadLiklihood { get; set; }
    public string AngryLiklihood { get; set; }
    public FaceObject(FaceAnnotation face)
    {
        Confidence = face.DetectionConfidence;
        JoyLiklihood = Enum.GetName(face.JoyLikelihood) ?? "?";
        SadLiklihood = Enum.GetName(face.SorrowLikelihood) ?? "?";
        AngryLiklihood = Enum.GetName(face.AngerLikelihood) ?? "?";
    }
}
public class LogoObject
{
    public float Confidence { get; set; }
    public string Description { get; set; }
    public LogoObject(EntityAnnotation logo)
    {
        Confidence = logo.Score / 10.0f;
        Description = logo.Description;
    }
}