# SDE Assignment 3

Submitted by: Ashish Jacob Sam (M21CS003)

---

## Problem statement

This assignment requires to use the following GCP(Google Cloud Platform) services in an application and showcase it.
- Storage Services: Offers limitless storage on cloud. 
- Cloud AI: Offers cloud based execution of ML tasks and Artificial intelligence
- Computing Service: Service for preparing a VM for hosting a web-server or for preforming computations.

For this, the following application use case is defined


This Assignment is developed using [.NET SDK 6] on a linux machine and is hosted in a debian-based VM using the Cloud services. Being a .NET web app, the following ASP project is prepared as shown

## Google Compute Engine

Google Compute Engine allows to prepare/launch VMs on cloud. For this assignment.

Configure the VM instance to support http traffic as mentioned in the [official documentation](https://cloud.google.com/vpc/docs/firewalls). Also for the specific C# project, in the file `Properties/launchSetting.json`, edit the IP addresses and port as mentioned in this [StackOverflow answer](https://stackoverflow.com/a/65381082/6488350), and publish the application using the command line argument `--urls` to specify the port number 80.

Now the web can be accessed at the given external IP address as shown:

Next, we can set up a instance group following the guide [here](https://cloud.google.com/compute/docs/tutorials/high-availability-load-balancing).

The following is used as a start-up script for the instances:

```bash
#! /bin/bash

# install the dependancies
sudo apt install -y screen git curl make wget;

wget https://packages.microsoft.com/config/debian/11/packages-microsoft-prod.deb -O packages-microsoft-prod.deb;
sudo dpkg -i packages-microsoft-prod.deb;
rm packages-microsoft-prod.deb;

sudo apt-get update;
sudo apt-get install -y apt-transport-https && sudo apt-get update &&  sudo apt-get install -y dotnet-sdk-6.0;
  
# All dependancies are installed.
sleep 120;

sudo screen -dmS webapp;
sudo screen -S webapp -p 0 -X stuff 'mkdir /home/ashishjacobsam/webapp;\n git clone https://AzuxirenLeadGuy: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@github.com/AzuxirenLeadGuy/SDE-assignments.git /home/ashishjacobsam/webapp > /home/ashishjacobsam/log.txt 2> /home/ashishjacobsam/errors.txt;\n dotnet build /home/ashishjacobsam/webapp/SDE_A3_M21CS003/SDE_A3_M21CS003.csproj;\n make -C /home/ashishjacobsam/webapp/SDE_A3_M21CS003/;\n make run -C /home/ashishjacobsam/webapp/SDE_A3_M21CS003/; \n';

```

This script will install dotnet sdk and clone a specified repository to run and build the application

## Google Cloud Store

A bucket is prepared to store the files for the web application, named as 'sde-a3-bucket' For setting it up, [this guide](https://cloud.google.com/storage/docs/reference/libraries) was referred.

Storing a file can be done by simply using the following script:

```csharp
using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;
using System;

public class CreateBucketSample
{
    public Bucket CreateBucket(
        string projectId = "your-project-id",
        string bucketName = "your-unique-bucket-name")
    {
        var storage = StorageClient.Create();
        var bucket = storage.CreateBucket(projectId, bucketName);
        Console.WriteLine($"Created {bucketName}.");
        return bucket;
    }
}
```

## Cloud AI

Google offers multiple subpackages in its umbrella service for AI, including Vertex AI, Vision AI and AutoML. For this assignment, the Vision AI API is selected, specifically to read and identify characters from the image. 

In this assignment, an API is created which consumes the Cloud AI APIs to check if the document contains a certain keyword, and uploads them to the common storage.

The following structures are in use by the program

```csharp
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
}
```

Next a load balancer is configured for a group of VMs which is created using the [following tutorial](https://cloud.google.com/compute/docs/tutorials/high-availability-load-balancing#create_a_regional_managed_instance_group)

