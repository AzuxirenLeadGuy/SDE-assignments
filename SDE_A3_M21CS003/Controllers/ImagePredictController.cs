using Microsoft.AspNetCore.Mvc;
using Google.Cloud.Storage.V1;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Vision.V1;
namespace SDE_A3_M21CS003.Controllers;

[ApiController]
[Route("[controller]")]
public class ImagePredictController : ControllerBase
{
    public const string JsonCredentials = "{\"type\": \"service_account\",\"project_id\": \"a3-iitj\",\"private_key_id\": \"d15eab7b7e0603b159bd1bf901b9f8be18f90220\",\"private_key\": \"-----BEGIN PRIVATE KEY-----\nMIIEvAIBADANBgkqhkiG9w0BAQEFAASCBKYwggSiAgEAAoIBAQCSuyH2A5M7FS5k\nScGyg9GSUIBA1KCguThIphyt/pcWW2jCWxLvWxFkryxojplHK7saKMNQTClIg+CV\nlfxDSx9wFHxNNP5Y+3tDJq+11sR4MiA/vvuPbOzQvMUn4haEz5qJDfW/f4i3FlkI\n+XzGJRUylI3XwFh5uVMVfzbt2oqAV/TI86ZbHJj2x/Oc5rq2tm1oZVrX2AWmleDu\nwCtKU3CpyiPbBcvUZO8rm0FvkC6MI3CjmmoOCK8CMg+3spDavqhInViO+9Bvu24i\nFQxEYI7yMazloOqDOKiViarnK90nPM9vucLaSkbPXnjeUC1UB8J9/xw9T5SH5MXm\nHvAkFn1RAgMBAAECggEAA+WVYs+4fsayuzYcP03x6GVb6pzbATm6DW/f7h0HTTAz\nSYoaqCGuhhKC5un2lyXJq8muWjkTlZnvAnn/v5fEp3jbzj8cjCdtoBxsaTNhd2G5\naof7bVUyut5wi/0cpP1QeNhWkPXlnxsoVS0vC6IbfILme3bfwqfR1Y3R2yJFMGBb\nT12qaDZF4TSsKDGbPIi8NvdLl87sE74Qrvusm2B7KdtrITOiNs0YXcP2Hp+ZBNiL\nTd+rNQdTnEnyYEucSU83f/30NiYG9Qhcws+oRxozqRfBwHgfh3s8Q4U7dSnXaCl4\ntYwww9mvO/3ZPoKgQuKlXE2NQTbiU+xZsHMLFS1SiwKBgQDNqBlVIsmf8dQOscSR\nLP1nsQaeG1ncZsxx/dmbdpQfoEuYIThbDPJdnbxkmGPaZbSMw0ee3pKT8yxlT/eW\nRiCVysZKsddjAaZXrSJ0Si9JIecnHXj1SB5t/xD26MPAyv9Uj3t9v+9dWARFJgju\n6DWyew+gkj5BGW6M2VTFKiKcmwKBgQC2plXT7Bun1P7fRoHsn9zyo6jJE32pxsoI\nV7u/QRdp/dzUJaZm7x1h2IqG23rsnHV8oqAl6ESEkxhlBPcfzfrWOtmZIzFa9k8R\nuYA4/wycLqhimHIWrVkSP8QWQudm26IH7UZjGiRUz8+wBtcgMZbAX3bdlk7yra6K\nH29phz2ugwKBgDnf43kk+A6t1FpqRoIAYjOrCsy1r70ppMEzieiJHzgjOQLP3ncp\nFo+n0xuQExGj64JtdhtMUX2XQlgkf+1nQMZGsTTWBWIUVcZ4r6iAM/xlsRI95gtI\nx193F0QUzvnTZrVaR4tk1yyyl0kKYhsxd+MJZKVwYlqZmJXEl2mA6QqfAoGAEacV\n2HuTWK8vSukHmZUBx7mlkHl/xa7ey9ue26cw2h+c/iv9XylxD4ncKZiA0ul0OW2G\nSOOf66hDzU+jisFfouDhMvnBxWbU0YO6LsgHsfiYcZ8GN8bdOvRKwVQKBVjo2hdG\nnEFkjYXgLsUaeZQRW9peJKBoVPAQVG0hWXOmtH0CgYA6bLoA/Xn91Y4zOZS1YxVp\n/wAld4Fw0YIG0nUIkS8/G8JAVVIkg+vRITBNyAlBVtdmdahsxdDP5xMvtWwEoEVM\n+/P9IlJeh6vREwDEwkprYrC5kgW77aOXbdI00305SypTUnbd45GurO4x9B1Ogs6m\nPnJ+6mR4vksakJZNnrtsFQ==\n-----END PRIVATE KEY-----\n\",\"client_email\": \"ashish@a3-iitj.iam.gserviceaccount.com\",\"client_id\": \"112924551724206061423\",\"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\"token_uri\": \"https://oauth2.googleapis.com/token\",\"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/ashish%40a3-iitj.iam.gserviceaccount.com\"}";
    readonly Random random;
    private readonly ImageAnnotatorClientBuilder builder;
    public ImagePredictController()
    {
        random = new Random();
        builder = new() { JsonCredentials = JsonCredentials };
    }
    public readonly GoogleCredential credentials = GoogleCredential.FromJson(JsonCredentials);
    internal static string MachineName = "";
    [HttpPost]
    public ResultObject Post(string keywords = "")
    {
        if (keywords == null || keywords == string.Empty) return new(MachineName, null!);
        var streamcopy = new MemoryStream();
        Request.Body.CopyToAsync(streamcopy);
        streamcopy.Seek(0, SeekOrigin.Begin);
        var img = Image.FromStreamAsync(streamcopy);
        string bucketName = "sde-a3-bucket";
        string imagename = "image-a3-" + random.NextInt64().ToString();
        var storage = StorageClient.Create(credentials);
        ImageAnnotatorClient client = builder.Build();
        var obj = new ResultObject(MachineName, client.DetectText(img.Result));
        foreach (var keyword in keywords.Split(';', StringSplitOptions.None))
        {
            bool found = false;
            string word = keyword.ToUpper();
            foreach (var txt in obj.Texts)
            {
                string sentence = System.Text.RegularExpressions.Regex.Replace(txt.Description.ToUpper(), @"[^\x20-\x7F]", "");
                if (sentence.Contains(word))
                {
                    found = true;
                    break;
                }
            }
            if (found == false) return obj;
        }
        streamcopy.Seek(0, SeekOrigin.Begin);
        var file = storage.UploadObject(bucketName, imagename, null, streamcopy);
        obj.Uploaded = true;
        obj.UploadURL = file.Name;
        return obj;
    }
}

/// <summary>
/// The structure of the result returned by this API
/// </summary>
public class ResultObject
{
    public string MachineName { get; set; }
    /// <summary>
    /// Collection of texts in the image
    /// </summary>
    /// <value></value>
    public EntityAnnotation[] Texts { get; set; }
    /// <summary>
    /// True if the object is also uploaded to the cloud
    /// </summary>
    /// <value></value>
    public bool Uploaded { get; set; }
    /// <summary>
    /// The URL of the file in the bucket, if file is indeed uploaded to the bucket
    /// </summary>
    /// <value></value>
    public string UploadURL { get; set; }
    public ResultObject(string mac, IEnumerable<EntityAnnotation> list)
    {
        MachineName = mac;
        Texts = list.ToArray();
        Uploaded = false;
        UploadURL = null!;
    }
}