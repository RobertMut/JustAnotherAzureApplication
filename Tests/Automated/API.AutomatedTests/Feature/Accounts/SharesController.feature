@Authenticate
@Storage
@Files:sample.jpg:t_testdata_groupShared1.jpeg;sample.jpg:t_testdata_userShared1.jpg
Feature: SharesController
	Shares controller automated tests

@CustomData
@File:Data\\AddSharesWithUsersAndGroups.sql
Scenario: Get user shares
	Given I use 'SharesGetUserSharesByUserId' endpoint
	And I add t_testdata_CustomUser2 as sql parameter under key @username
	And I save response returned by sql file GetUserByName.sql under @userId and as sql param
	And I replace url parameters with value under key @userId  
	When I make call to endpoint with an authorization token using GET method
	Then Response code is 'OK'
	And Response mapped as 'ShareOutput' is the same as 'Results\\UserSharesResult.json'
	
@CustomData
@File:Data\\AddSharesWithUsersAndGroups.sql
Scenario: Get group shares
	Given I use 'SharesGetGroupShareByGroupId' endpoint
	And I add t_testdata_CustomGroup as sql parameter under key @groupName
	And I save response returned by sql file GetGroupByName.sql under @groupId and as sql param
	And I replace url parameters with value under key @groupId  
	When I make call to endpoint with an authorization token using GET method
	Then Response code is 'OK'
	And Response mapped as 'ShareOutput' is the same as 'Results\\GroupSharesResult.json'

@CustomData
@File:Data\\AddSharesWithUsersAndGroups.sql
Scenario: Delete user share
	Given I use 'SharesDeleteUserShareByFileAndUserId' endpoint
	And I add t_testdata_CustomUser2 as sql parameter under key @username
	And I save response returned by sql file GetUserByName.sql under @userId and as sql param
	And I replace url parameters with value under key t_testdata_userShared1.jpg;@userId
	When I make call to endpoint with an authorization token using DELETE method
	Then Response code is 'OK'
	Given I add t_testdata_userShared1.jpg as sql parameter under key @filename
	Then Database not contains user share 't_testdata_CustomUser2'

@CustomData
@File:Data\\AddSharesWithUsersAndGroups.sql
Scenario: Delete group share
	Given I use 'SharesDeleteGroupShareByFileAndGroupId' endpoint
	And I add t_testdata_CustomGroup as sql parameter under key @groupName
	And I save response returned by sql file GetGroupByName.sql under @groupId and as sql param
	And I replace url parameters with value under key t_testdata_groupShared1.jpg;@groupId
	When I make call to endpoint with an authorization token using DELETE method
	Then Response code is 'OK'
	Given I add t_testdata_groupShared1.jpg as sql parameter under key @filename
	Then Database not contains group share 't_testdata_CustomGroup'

@CustomData
@File:Data\\AddSharesWithUsersAndGroups.sql
Scenario: Add user share
	Given I use 'SharesAddUserShare' endpoint
	And I add t_testdata_CustomUser3 as sql parameter under key @username
	And I save response returned by sql file GetUserByName.sql under @userId and as sql param
	And I add t_testdata_CustomUser1 as sql parameter under key @username
	And I save response returned by sql file GetUserByName.sql under @otherUserId and as sql param
	And I prepare request with following values using 'UserShareModel' model
	  | UserId  | OtherUserId  | Filename   | Permissions     |
	  | @userId | @otherUserId | <Filename> | <PermissionInt> |
	When I make call to endpoint with an authorization token using POST method
	Then Response code is 'OK'
Examples: 
  | Filename                   | PermissionInt |
  | t_testdata_userShared1.jpg | 1             |
  | t_testdata_userShared1.jpg |               |
  | t_testdata_userShared1.jpg | 3             |

@CustomData
@File:Data\\AddSharesWithUsersAndGroups.sql
Scenario: Add group share by user
	Given I use 'SharesAddGroupShare' endpoint
	And I add t_testdata_CustomGroup as sql parameter under key @groupName
	And I save response returned by sql file GetGroupByName.sql under @groupId and as sql param
	And I add t_testdata_CustomUser3 as sql parameter under key @username
	And I save response returned by sql file GetUserByName.sql under @userId and as sql param
	And I prepare request with following values using 'GroupShareModel' model
	  | GroupId  | UserId  | Filename   | Permissions     |
	  | @groupId | @userId | <Filename> | <PermissionInt> |
	When I make call to endpoint with an authorization token using POST method
	Then Response code is 'OK'
Examples: 
  | Filename                    | PermissionInt |
  | t_testdata_userShared1.jpg | 1             |
  | t_testdata_userShared1.jpg |               |
  | t_testdata_userShared1.jpg | 3             |

@CustomData
@File:Data\\AddSharesWithUsersAndGroups.sql
Scenario: Add group share by user without file
	Given I use 'SharesAddGroupShare' endpoint
	And I add t_testdata_CustomGroup as sql parameter under key @groupName
	And I save response returned by sql file GetGroupByName.sql under @groupId and as sql param
	And I add t_testdata_CustomUser3 as sql parameter under key @username
	And I save response returned by sql file GetUserByName.sql under @userId and as sql param
	And I prepare request with following values using 'GroupShareModel' model
	  | GroupId  | UserId  | Filename   | Permissions |
	  | @groupId | @userId | <Filename> | 0           |
	When I make call to endpoint with an authorization token using POST method
	Then Response code is 'NotFound'
	And Response message contains 'File uknownFile.jpg not found!'
Examples: 
  | Filename       |
  | uknownFile.jpg |