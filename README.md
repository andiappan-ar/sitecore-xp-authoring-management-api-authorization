
# Module : SitecoreXP AuthoringManagement Authorization 
How to Authenticate and Access Sitecore Authoring and Management GraphQL API Using Bearer Tokens

## 📝 Introduction

This project is a working sample demonstrating how to obtain an access token for the 'Sitecore Authoring and Management GraphQL API' using bearer tokens in client applications. It also includes a basic example of executing an authoring GraphQL call to the Authoring and Management GraphQL API.

I referred to this article for the implementation. https://doc.sitecore.com/xp/en/developers/104/sitecore-experience-manager/use-bearer-tokens-in-client-applications.html.

## Project structure 

![image](https://github.com/user-attachments/assets/b7211d03-aed0-46de-8d1f-dbadc6f3b4f3)


**`Sitecore.Identity`**  : The components of this project are intended for the Sitecore XP Identity Server role.

**`Sitecore.Platform`**  : The components of this project are intended for the Sitecore XP CMS/CD role, where you enable your GraphQL endpoints. A typical use case would be within a CMS environment.

**`AuthoringManagement.Authorization`**  : This is your identity client application, used to connect to your Sitecore Identity Server and obtain an access token for the authoring GraphQL API.

## 🛠 Techs invloved in the sample

* **Sitecore 10.3** (or any version that supports the Authoring Management API)
* **.NET Core 2.1** (Please ensure compatibility when running this sample)

## ▶️ How to run this project
Please follow the steps below to run this project:

### 🔧 step 1 : Setup client identity in Sitecore-Identity-Server

Deploy the identity server configurations to the Sitecore Identity Server.

**`File to deploy`**  : src/Sitecore.Identity/Config/production/Sitecore.IdentityServer.Host.Custom.AuthoringManagement.Authorization.xml

**`Location to deploy in Sitecore identity server`**  : {Sitecore-IdentityServer}\Config\production

The following configurations are customizable. As part of the sample project, everything has been configured accordingly. Please ensure you align with the configurations below if you make any changes.

```config
<ClientId>AuthoringManagement.Authorization</ClientId>
<ClientName>AuthoringManagement.Authorization</ClientName>
<AllowedCorsOriginsGroup1>https://localhost:44360</AllowedCorsOriginsGroup1>
```

(❗) Once you deploy the file , Please restart the identity server.

### 🔧 step 2 : Enable the GraphQL IDE in Sitecore-CMS

Deploy the patch file as mentioned below,

**`File to deploy`**  : src/Sitecore.Platform/App_config/Include/AuthoringManagement.Authorization/zzz.Custom.AuthoringAPI.Enabler.config

**`Location to deploy in Sitecore identity server`**  : {Sitecore-CMS}\App_Config\Include\AuthoringManagement.Authorization

(❗) This change will cause your server restart.


### 🔧 step 3 : Run the sample solution

(✋) Before proceeding, ensure that the environment where you are running this application has access to both your Sitecore Identity Server and CMS. The Identity Server is required for obtaining tokens, and CMS access is needed to execute the sample GraphQL query.

(✋) Please update the URLs in the appsettings.json `(src/SitecoreXP.AuthoringManagement.Authorization/appsettings.json)` according to your environment setup.

Explanation of the Configuration below,

**`CurrentAppIdentityEndpoint`**  : This is the endpoint of your client application's identity server. It should point to the identity server instance hosted for this application. For example, https://localhost:44360/identity is used during local development.

**`SitecoreIdentityEndpoint`**  : The URL of your Sitecore Identity Server. This endpoint is used to obtain access tokens for interacting with Sitecore APIs. Replace https://sc-10.3identityserver.dev.local/ with the URL of your Sitecore Identity Server instance.

**`SitecoreIdentityServerClientId`**  : The client ID registered in your Sitecore Identity Server for this application. Ensure this matches the ID configured in your Identity Server to authorize this application. For example, AuthoringManagement.Authorization is used in this sample.

**`SitecorePlatformGraphQLURL`**  : The endpoint for the Sitecore Authoring and Management GraphQL API. Update this to match the URL of your Sitecore CMS instance where the GraphQL service is hosted. The sample uses https://sc-10.3sc.dev.local/sitecore/api/authoring/graphql/v1.

**Note(📗)**:
Ensure these values are correctly aligned with your environment settings to avoid connectivity or authentication issues.

### 🏃 step 4 : Run the client identity project

The application will start at the URL `https://localhost:44360` and then redirect to the `Sitecore Identity Server` as shown below.

Log in using your CMS credentials.

After logging in, you will see a page displaying access token details. This token is required to authorize access to the Authoring Management API.

![image](https://github.com/user-attachments/assets/4651c0ce-29a7-4000-b9a7-268747c9a9b4)


Next, navigate to the URL by clicking the `Connect Sitecore >` link. This action triggers a sample GraphQL query that retrieves and displays the site list from your CMS.

![image](https://github.com/user-attachments/assets/5446f322-8e35-4deb-8b5e-731d305cca6d)



## ➕ Extra things for play

### Try different GQL queries

If you want to execute a different GraphQL query, navigate to the file below and update the GraphQL query as needed.

**`File`**  : src/SitecoreXP.AuthoringManagement.Authorization/Controllers/SitecoreController.cs

```
    // Define the GraphQL query to fetch site information
    var query = @"
                  query {
                      sites {
                          name
                      }
                  }";
```


## ➡️ TL;DR

This project demonstrates how to authenticate and access the Sitecore Authoring and Management GraphQL API using bearer tokens. It includes configurations for the Sitecore Identity Server and CMS roles, along with a sample GraphQL query to retrieve site information. Follow the outlined steps to configure, run the application, and modify queries as needed. Ensure your environment URLs and settings align with your specific setup.
