﻿@Authenticate
@Storage
@Files:sample.jpg:t.testdata.groupShared1.jpeg;sample.jpg:t.testdata.userShared1.jpg
Feature: ImagesController
	Simple calculator for adding two numbers

@CustomData
@File:Data\\AddSharesWithUsersAndGroups.sql
Scenario: Get user images
	Given I use 'Images' endpoint
	When I make call to endpoint with an authorization token using GET method
	Then Response code is 'OK'
	And Response mapped as 'FileList' is the same as 'Results\\GetUserImages.json'

@CustomData
@File:Data\\AddSharesWithUsersAndGroups.sql
Scenario: Get non existing image
	Given I use 'ImagesGetImage' endpoint
	And I add 'nonexisting.jpg;0' as url parameter
	When I make call to endpoint with an authorization token using GET method
	Then Response code is 'NotFound'

@CustomData
@File:Data\\AddSharesWithUsersAndGroups.sql
Scenario: Post image
	Given I use 'Images' endpoint
	And I prepare form from table
	  | file            | targetType | height | width |
	  | File:sample.jpg | bmp        | 100    | 100   |
	When I make call to endpoint with an authorization token using POST method
	Then Response code is 'OK'
	
@CustomData
@File:Data\\AddSharesWithUsersAndGroups.sql
Scenario: Get image
	Given I use 'Images' endpoint
	And I prepare form from table
	  | file            | targetType | height | width |
	  | File:sample.jpg | bmp        | 100    | 100   |
	When I make call to endpoint with an authorization token using POST method
	Then Response code is 'OK'
	Given I use 'ImagesGetImage' endpoint
	And I add 'false;sample.jpg;100x100;bmp' as url parameter
	When I make call to endpoint with an authorization token using GET method
	Then Response code is 'OK'
	And Response file is same as 'response100x100.jpg'
		
@CustomData
@File:Data\\AddSharesWithUsersAndGroups.sql
Scenario: Get images throws exception
	Given I use 'Images' endpoint
	And I prepare form from table
	  | file            | targetType | height   | width   |
	  | File:<Filename> | bmp        | <height> | <width> |
	When I make call to endpoint with an authorization token using POST method
	Then Response code is 'BadRequest'
	And Response message contains '<ExpectedMessage>'

Examples: 
  | Filename   | height | width | ExpectedMessage                 |
  | sample.jpg | 0      | 0     | "The UserId field is required." |
  | sample.jpg | 100    | 0     |                                 |
  | sample.jpg | 0      | 100   |                                 |
  
@CustomData
@File:Data\\AddSharesWithUsersAndGroups.sql
Scenario: Get image throws exception
	Given I use 'Images' endpoint
	And I prepare form from table
	  | file            | targetType | height | width |
	  | File:sample.jpg | bmp        | 100    | 100   |
	When I make call to endpoint with an authorization token using POST method
	Then Response code is 'OK'
	Given I use 'ImagesGetImage' endpoint
	And I add 'false;<filename>;<height>x<width>;bmp' as url parameter
	When I make call to endpoint with an authorization token using GET method
	Then Response code is 'BadRequest'
	And Response message contains '<ExpectedMessage>'
	
Examples: 
  | Filename      | height | width | ExpectedMessage |
  | sample.jpg    | 0      | 0     |                 |
  | wrongFilename | 100    | 100   |                 |
  | sample.jpg    | 100    | 0     |                 |
  | sample.jpg    | 0      | 100   |                 |