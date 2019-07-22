
#usage install-software-windows.ps1 dnsname

param
(
      [string]$resourceGroupName = $null,
      [string]$prefixName = $null
)
function WriteLog($msg)
{
Write-Host $msg
$msg >> install-aci-windows.log
}

if($prefixName -eq $null) {
     WriteLog "Installation failed prefixName parameter not set "
     throw "Installation failed prefixName parameter not set "

}
if($resourceGroupName -eq $null) {
     WriteLog "Installation failed resourceGroupName parameter not set "
     throw "Installation failed resourceGroupName parameter not set "
}
$acrName = $prefixName + 'acr'
$acrDeploymentName = $prefixName + 'acrdep'
$acrSPName = $prefixName + 'acrsp'
$akvName = $prefixName + 'akv'
$acrSPPassword = ''
$acrSPAppId = ''
$acrSPObjectId = ''
$akvDeploymentName = $prefixName + 'akvdep'
$aciDeploymentName = $prefixName + 'acidep'
$imageName = 'testwebapp.linux:v1'

function WriteLog($msg)
{
Write-Host $msg
$msg >> install-aci-windows.log
}
function Get-Password($file)
{
    foreach($line in (Get-Content $file  ))
    {
	    $nline = $line.Split(':", ',[System.StringSplitOptions]::RemoveEmptyEntries)
	    if($nline.Length -gt 1) 
	    {
  	    if($nline[0] -eq "password")
  	        {
		        return $nline[1]
      		        break
  	        }
  	    }
    }
    return $null
}


WriteLog "Creating Azure Container Registry" 
az group deployment create -g $resourceGroupName -n $acrDeploymentName --template-file azuredeploy.acr.json --parameter namePrefix=$prefixName --verbose -o json 
az group deployment show -g $resourceGroupName -n $acrDeploymentName --query properties.outputs

WriteLog "Building and registrying the image in Azure Container Registry"
echo az acr build --registry $acrName   --image $imageName ..\..\. -f ..\..\Docker\Dockerfile.linux >> install-aci-windows.log
az acr build --registry $acrName   --image $imageName ..\..\. -f ..\..\Docker\Dockerfile.linux


WriteLog "Creating Service Principal" 
az acr show --name $acrName --query id --output tsv > acrid.txt
$acrID = Get-Content .\acrid.txt -Raw 
az ad sp create-for-rbac --name http://$acrSPName --scopes $acrID --role acrpull --query password --output tsv > sppassword.txt
$acrSPPassword  = Get-Password .\sppassword.txt 
if($acrSPPassword -eq $null) {
     WriteLog "ACR SP Password not found "
     throw "ACR SP Password not found "
}
WriteLog ("SPPassword: " + $acrSPPassword)


az ad sp show --id http://$acrSPName --query appId --output tsv > spappid.txt
$acrSPAppId  = Get-Content  .\spappid.txt -Raw  
$acrSPAppId = $acrSPAppId.replace("`n","").replace("`r","")

WriteLog ("SPAppId: " + $acrSPAppId)

az ad signed-in-user show --query objectId --output tsv > spobjectid.txt
$acrSPObjectId  = Get-Content  .\spobjectid.txt -Raw  
$acrSPObjectId = $acrSPObjectId.replace("`n","").replace("`r","")
WriteLog ("SPObjectId: " + $acrSPObjectId)

WriteLog "Creating Azure Key Vault" 
az group deployment create -g $resourceGroupName -n $akvDeploymentName --template-file azuredeploy.akv.json --parameter namePrefix=$prefixName objectId=$acrSPObjectId  appId=$acrSPAppId  password=$acrSPPassword --verbose -o json
az group deployment show -g $resourceGroupName -n $akvDeploymentName --query properties.outputs

$pullusr = $acrName + '-pull-usr'
$pullpwd = $acrName + '-pull-pwd'

az keyvault secret show --vault-name $akvName --name $pullusr --query value -o tsv > akvappid.txt
az keyvault secret show --vault-name $akvName --name $pullpwd --query value -o tsv > akvpassword.txt



WriteLog "Deploying a container on Azure Container Instance" 
$cmdtest = "az group deployment create -g " + $resourceGroupName +" -n " + $aciDeploymentName + "--template-file azuredeploy.aci.json --parameter namePrefix=" + $prefixName + " imageName=" + $imageName +" appId=" + $acrSPAppId + " password=" + $acrSPPassword +"  cpuCores='0.4' memoryInGb='0.3' --verbose -o json"
WriteLog $cmdtest


az group deployment create -g $resourceGroupName -n $aciDeploymentName --template-file azuredeploy.aci.json --parameter namePrefix=$prefixName imageName=$imageName  appId=$acrSPAppId  password=$acrSPPassword cpuCores="0.4" memoryInGb="0.3" --verbose -o json
az group deployment show -g $resourceGroupName -n $aciDeploymentName --query properties.outputs


WriteLog "Installation completed !" 

