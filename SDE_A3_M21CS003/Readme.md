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
sudo apt install -y screen git curl make

wget https://packages.microsoft.com/config/debian/11/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb

sudo apt-get update; \
  sudo apt-get install -y apt-transport-https && \
  sudo apt-get update && \
  sudo apt-get install -y dotnet-sdk-6.0
  
# All dependancies are installed.

git clone https://AzuxirenLeadGuy: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@github.com/AzuxirenLeadGuy/SDE-assignments.git
cd SDE-assignments/SDE_A3_M21CS003
screen -dmS webapp
screen -S webapp -p 0 -X stuff 'make && make run\n'
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