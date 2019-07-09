# Deployment of a REST API  hosted on Azure App Service, Azure Function, Virtual Machine and Azure Kubernetes Service

<a href="https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2Fflecoqui%2FTestRESTAPIServices%2Fmaster%2Fazuredeploy.json" target="_blank">
    <img src="http://azuredeploy.net/deploybutton.png"/>
</a>
<a href="http://armviz.io/#/?load=https%3A%2F%2Fraw.githubusercontent.com%2Fflecoqui%2FTestRESTAPIServices%2Fmaster%2Fazuredeploy.json" target="_blank">
    <img src="http://armviz.io/visualizebutton.png"/>
</a>

This template allows you to deploy a REST API  hosted on Azure App Service, Azure Function, Virtual Machine and Azure Kubernetes Service. Moreover, the applications and functions source code  will be stored on github and automatically deployed on Azure.


![](https://raw.githubusercontent.com/flecoqui/TestRESTAPIServices/master/Docs/1-architecture.png)



## CREATE RESOURCE GROUP:

**Azure CLI:** azure group create "ResourceGroupName" "RegionName"

**Azure CLI 2.0:** az group create an "ResourceGroupName" -l "RegionName"

For instance:

    azure group create TestRESTAPIServicesrg eastus2

    az group create -n TestRESTAPIServicesrg -l eastus2

## DEPLOY THE SERVICES:

**Azure CLI:** azure group deployment create "ResourceGroupName" "DeploymentName"  -f azuredeploy.json -e azuredeploy.parameters.json*

**Azure CLI 2.0:** az group deployment create -g "ResourceGroupName" -n "DeploymentName" --template-file "templatefile.json" --parameters @"templatefile.parameter..json"  --verbose -o json

For instance:

    azure group deployment create TestRESTAPIServicesrg TestRESTAPIServicesdep -f azuredeploy.json -e azuredeploy.parameters.json -vv

    az group deployment create -g TestRESTAPIServicesrg -n TestRESTAPIServicesdep --template-file azuredeploy.json --parameter @azuredeploy.parameters.json --verbose -o json


When you deploy the service you can define the following parameters:</p>
**namePrefix:**						The name prefix which will be used for all the services deployed with this ARM Template</p>
**WebAppSku:**						The WebApp Sku Capacity, by defualt F1</p>
**azFunctionAppSku:**				The Azure Function App Sku Capacity, by defualt F1</p>
**repoURL:**                        The github repository url</p>
**branch:**                         The branch name in the repository</p>

Once deployed, the following services are available in the resource group:


![](https://raw.githubusercontent.com/flecoqui/TestRESTAPIServices/master/Docs/1-deploy.png)


## TEST THE SERVICES:
Once the services are deployed, you can open the Web page hosted on the Azure App Service.
For instance :

     http://<websitename>.azurewebsites.net//WebApp/WebApp.html
 
With Curl you can test the Azure Functions:
For instance :

     curl -d "{}" -H "Content-Type: application/json"  -X POST   https://testspeechfunction.azurewebsites.net/api/Function1App
     curl -d "{}" -H "Content-Type: application/json"  -X POST   https://testspeechfunction.azurewebsites.net/api/Function2App

</p>


## DELETE THE RESOURCE GROUP:

**Azure CLI:** azure group delete "ResourceGroupName" "RegionName"

**Azure CLI 2.0:** az group delete -n "ResourceGroupName" "RegionName"

For instance:

    azure group delete TestRESTAPIServicesrg eastus2

    az group delete -n TestRESTAPIServicesrg 

