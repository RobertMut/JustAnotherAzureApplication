@Authenticate
Feature: GroupsController
    Group controller automated tests

@CustomData
@File:Data\\AddGroup.sql
Scenario: Get Groups
    Given I use 'Groups' endpoint
    When I make call to endpoint with an authorization token using GET method
    Then Response code is 'OK'
    And Response mapped as 'GroupsOutput' is the same as 'Results\\GetGroupsResult.json'

Scenario: Create Group
    Given I use 'Groups' endpoint
    And I prepare request with following values using 'CreateGroupModel' model
      | Name        | Description   |
      | <GroupName> | <Description> |
    When I make call to endpoint with an authorization token using POST method
    Then Response code is 'OK'
    Given I clear the database data
Examples:
  | GroupName                                       | Description                                                                                                               |
  | t_testdata_1                                    | Description                                                                                                               |
  | t_testdata_GroupNameGroupNameGroupNameGroupName | DescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescription |
  
Scenario: Create Group returns error message
    Given I use 'Groups' endpoint
    And I prepare request with following values using 'CreateGroupModel' model
      | Name | Description   |
      |      | <Description> |
    When I make call to endpoint with an authorization token using POST method
    Then Response code is 'BadRequest'
    And Response message contains '<ErrorMessage>'
    Given I clear the database data
Examples:
  | GroupName    | Description | ErrorMessage                          | ExpectedCode |
  |              | Description | Name must be not null or empty        | BadRequest   |
  | t_testdata_1 |             | Description must be not null or empty | BadRequest   |

Scenario: Create Group returns error message on too long values
    Given I use 'Groups' endpoint
    And I prepare request with file 'InputJson\\<FileName>'
    When I make call to endpoint with an authorization token using POST method
    Then Response code is '<ExpectedCode>'
    And Response message contains '<ErrorMessage>'
    Given I clear the database data
Examples:
  | FileName                      | ErrorMessage                   | ExpectedCode |
  | GroupsTooLongDescription.json | Group description is too long. | BadRequest   |
  | GroupsTooLongName.json        | Group name is too long.        | BadRequest   |
  
@CustomData
@File:Data\\AddGroup.sql
Scenario: Delete group
    Given I use 'GroupsDeleteById' endpoint
    And I add t_testdata_CustomName as sql parameter under key @groupName
    And I save response returned by sql file GetGroupByName.sql under @groupId and as sql param
    And I replace url parameters with value under key @groupId  
    When I make call to endpoint with an authorization token using DELETE method
    Then Response code is 'OK'
    And Database not contains group 't_testdata_CustomName'

Scenario: Delete not existing group should throw exception
    Given I use 'GroupsDeleteById' endpoint
    And I add '00000000-0000-0000-0000-000000000000' as url parameter
    When I make call to endpoint with an authorization token using DELETE method
    Then Response code is 'BadRequest'
    And Response message contains '"title":"One or more validation errors occurred."'
