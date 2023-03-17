*** Settings ***
Documentation     		Custom keywords for DB tests
Library           		DatabaseLibrary 
Library					OperatingSystem

*** Variables ***

# DB Credentials, move to cfg file
${DBName}  		d0020e
${DBUser}  		projectmember
${DBHost}  		127.0.0.1
${DBPassword}
${DBPort}  		33306

# Tables in DB
${coords}		position_data


*** Keywords ***	
	
Connect to DB
	Set Log Level    		NONE
	${DBPassword} =		Get Value From User				SSH password:		hidden=yes
	Connect To Database		pymysql	${DBName}	${DBUser}	${DBPassword}	${DBHost}	${DBPort}
	Set Log Level    		TRACE
	Table Must Exist		${coords}
	