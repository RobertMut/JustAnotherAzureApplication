@Authenticate
Feature: PostAccounts
	Simple calculator for adding two numbers
	
Scenario: 01 Login
	Given I use 'Accounts' endpoint
	And I prepare request with following values using 'LoginModel' model
	| UserName | Password |
	| Default  | 12345    |
 	When I make call to endpoint
 	Then Response code is 'OK'
 	
 Scenario: 02 Validate login exceptions
	 Given I use 'Accounts' endpoint
	 And I prepare request with following values using 'LoginModel' model
	   | UserName   | Password   |
	   | <Username> | <Password> |
	 When I make call to endpoint
	 Then Response code is '<ExpectedCode>'
	 And Response message contains '<ErrorMessage>'

Examples: 
| Username    | Password  | ErrorMessage                                      | ExpectedCode |
| Default     |           | "title":"One or more validation errors occurred." | BadRequest   |
|             | 12345     | "title":"One or more validation errors occurred." | BadRequest   |
| NonExisting | WrongPass | User NonExisting not found!                       | NotFound     |

Scenario: 03 Register user
	Given I use 'AccountsRegister' endpoint
	And I prepare request with following values using 'RegisterModel' model
	| Username          | Password |
	| t_testdata_Sample | Sample   |
 	When I make call to endpoint with an authorization token
 	Then Response code is 'OK'
 	And Database contains user 't_testdata_Sample'
 	Given I clear the database data
 	
Scenario: 04 Validate register exceptions
	Given I use 'AccountsRegister' endpoint
	And I prepare request with following values using 'RegisterModel' model
	  | Username   | Password   |
	  | <Username> | <Password> |
	When I make call to endpoint with an authorization token
	Then Response code is '<ExpectedCode>'
	And Response message contains '<ErrorMessage>'
	And Database not contains user '<Username>'
	Given I clear the database data

Examples: 
  | Username        | Password | ErrorMessage                                      | ExpectedCode |
  | t_testdata_user |          | "title":"One or more validation errors occurred." | BadRequest   |
  |                 | 12345    | "title":"One or more validation errors occurred." | BadRequest   |
  
Scenario: 05 Validate register duplicated user exception
	Given I use 'AccountsRegister' endpoint
	And I prepare request with following values using 'RegisterModel' model
	  | Username | Password |
	  | Default  | 12345    |
	When I make call to endpoint with an authorization token
	Then Response code is 'Conflict'
	And Response message contains 'Default'
	And Database contains user 'Default'
	Given I clear the database data