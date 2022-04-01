# SDE Assignment 3

Submitted by: Ashish Jacob Sam (M21CS003)

---

## Problem statement

This assignment requires to use the following GCP(Google Cloud Platform) services in an application and showcase it.
- Storage Services: Offers limitless storage on cloud. 
- Cloud AI: Offers cloud based execution of ML tasks
- Computing Service: Service for preparing a VM for hosting a web-server or for preforming computations.

For this, the following application use case is defined


This Assignment is developed using [.NET SDK 6] on a linux machine and is hosted in a debian-based VM using the Cloud services. Being a .NET web app, the following ASP project is prepared as shown

## Google Compute Engine

Google Compute Engine allows to prepare/launch VMs on cloud. For this assignment.

Configure the VM instance to support http traffic as mentioned in the [official documentation](https://cloud.google.com/vpc/docs/firewalls). Also for the specific C# project, in the file `Properties/launchSetting.json`, edit the IP addresses and port as mentioned in this [StackOverflow answer](https://stackoverflow.com/a/65381082/6488350), and publish the application using the command line argument `--urls` to specify the port number 80.

Now the web can be accessed at the given external IP address as shown