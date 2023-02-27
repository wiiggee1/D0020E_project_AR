*** Settings ***
Documentation     		Connecting to the DB
Resource				./resources/vm.resource
Resource				./resources/db.resource
Test Setup				Set Log Level		TRACE


*** Test Cases ***	
Initialize Connection
	[tags]				Connection
    Connect to VM
	Log 				\n\nConnection to VM established!		console=yes
	Connect to DB
	Log					\nConnection to DB established!			console=yes
	
Disconnect
	[tags]				Connection	UpdateDB	Tables	Procedures	Admin
	Disconnect from VM