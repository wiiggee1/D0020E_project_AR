*** Settings ***
Documentation     		Custom keywords for VM tests
Library           		SSHLibrary
Library					Dialogs


*** Variables ***
# SSH credentials, move to cfg file
${SSHHost}		130.240.202.127
${SSHUser}		projectmember
${localPort}	33306
${remotePort}	3306
${DBHost}  		127.0.0.1


*** Keywords ***	
Connect to VM
	Set Log Level    		NONE
	${SSHPassword} =		Get Value From User				SSH password:		hidden=yes
    Open Connection			${SSHHost}
	Login					${SSHUser}		${SSHPassword}
	Set Log Level    		TRACE
	Create Local Ssh Tunnel	${localPort}	${DBHost}		${remotePort}
	
Disconnect from VM
	Close All Connections
	