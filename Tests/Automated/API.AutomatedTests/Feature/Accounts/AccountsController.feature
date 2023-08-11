@Authenticate
Feature: AccountsController
	Account controller automated tests
	
Scenario: Login
	Given I use 'Accounts' endpoint
	And I prepare request with following values using 'LoginModel' model
	| UserName | Password |
	| Default  | 12345    |
	When I make call to endpoint using POST method
 	Then Response code is 'OK'

 Scenario: Check login exceptions
	 Given I use 'Accounts' endpoint
	 And I prepare request with following values using 'LoginModel' model
	   | UserName   | Password   |
	   | <Username> | <Password> |
	 When I make call to endpoint using POST method
	 Then Response code is '<ExpectedCode>'
	 And Response message contains '<ErrorMessage>'

Examples: 
| Username    | Password  | ErrorMessage                       | ExpectedCode |
| Default     |           | Password must be not null or empty | BadRequest   |
|             | 12345     | Username must be not null or empty | BadRequest   |
| NonExisting | WrongPass | User NonExisting not found!        | NotFound     |

Scenario: Register user
	Given I use 'AccountsRegister' endpoint
	And I prepare request with following values using 'RegisterModel' model
	| Username          | Password |
	| t_testdata_Sample | Sample   |
 	When I make call to endpoint with an authorization token using POST method
 	Then Response code is 'OK'
 	And Database contains user 't_testdata_Sample'
 	Given I clear the database data
 	
Scenario: Check register exceptions
	Given I use 'AccountsRegister' endpoint
	And I prepare request with following values using 'RegisterModel' model
	  | Username   | Password   |
	  | <Username> | <Password> |
	When I make call to endpoint with an authorization token using POST method
	Then Response code is '<ExpectedCode>'
	And Response message contains '<ErrorMessage>'
	And Database not contains user '<Username>'
	Given I clear the database data

Examples: 
  | Username        | Password | ErrorMessage                       | ExpectedCode |
  | t_testdata_user |          | Password must be not null or empty | BadRequest   |
  |                 | 12345    | Username must be not null or empty | BadRequest   |
  
Scenario: Check register duplicated user exception
	Given I use 'AccountsRegister' endpoint
	And I prepare request with following values using 'RegisterModel' model
	  | Username | Password |
	  | Default  | 12345    |
	When I make call to endpoint with an authorization token using POST method
	Then Response code is 'Conflict'
	And Response message contains 'Default'
	And Database contains user 'Default'
	Given I clear the database data