Feature: UserService

Scenario: Register new valid User - Server Response Status Is Ok
	Given User registration values are valid
	When Register User
	Then Create User Request Status is 'OK'

Scenario: Invalid User Register New User FirstName Allways NULL ResponseStatus Is InternalServerError
	Given User registration values are invalid
	When Register User
	Then Create User Request Status is 'InternalServerError'
