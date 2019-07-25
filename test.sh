#!/bin/bash
# Parameter 1 resourceGroupName 
# Parameter 2 prefixName 
# Parameter 3 cpuCores 
# Parameter 4 memoryInGb
# Parameter 4 aksVMSize
# Parameter 5 memoryInGb
# Parameter 6 aksNodeCount
resourceGroupName=$1
prefixName=$2 
cpuCores=$3 
memoryInGb=$4
aksVMSize=$5
aksNodeCount=$6


#############################################################################
log()
{
	echo "$1"
	echo "$1" >> ./install-container.log
}
#############################################################################
check_os() {
    grep ubuntu /proc/version > /dev/null 2>&1
    isubuntu=${?}
    grep centos /proc/version > /dev/null 2>&1
    iscentos=${?}
    grep redhat /proc/version > /dev/null 2>&1
    isredhat=${?}	
	if [ -f /etc/debian_version ]; then
    isdebian=0
	else
	isdebian=1	
    fi

	if [ $isubuntu -eq 0 ]; then
		OS=Ubuntu
		VER=$(lsb_release -a | grep Release: | sed  's/Release://'| sed -e 's/^[ \t]*//' | cut -d . -f 1)
	elif [ $iscentos -eq 0 ]; then
		OS=Centos
		VER=$(cat /etc/centos-release)
	elif [ $isredhat -eq 0 ]; then
		OS=RedHat
		VER=$(cat /etc/redhat-release)
	elif [ $isdebian -eq 0 ];then
		OS=Debian  # XXX or Ubuntu??
		VER=$(cat /etc/debian_version)
	else
		OS=$(uname -s)
		VER=$(uname -r)
	fi
	
	ARCH=$(uname -m | sed 's/x86_//;s/i[3-6]86/32/')

	log "OS=$OS version $VER Architecture $ARCH"
}
if [ -z "$resourceGroupName" ]; then
   log 'resourceGroupName not set'
   exit 1
fi
if [ -z "$prefixName" ]; then
   log 'prefixName not set'
   exit 1
fi
if [ -z "$cpuCores" ]; then
   log 'cpuCores not set'
   exit 1
fi
if [ -z "$memoryInGb" ]; then
   log 'memoryInGb not set'
   exit 1
fi
if [ -z "$aksVMSize" ]; then
   log 'aksVMSize not set'
   exit 1
fi
if [ -z "$aksNodeCount" ]; then
   log 'aksNodeCount not set'
   exit 1
fi


environ=`env`
log "Environment before installation: $environ"

log "Installation script is starting for resource group: $resourceGroupName with prefixName: $prefixName cpu: $cpuCores memory: $memoryInGb AKS VM Size: $aksVMSize and AKS node count: $aksNodeCount
at $date"
check_os
if [ $iscentos -ne 0 ] && [ $isredhat -ne 0 ] && [ $isubuntu -ne 0 ] && [ $isdebian -ne 0 ];
then
    log "unsupported operating system"
    exit 1 
else
# To be completed
fi
exit 0 


