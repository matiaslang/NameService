## Name Service

------

### What is this?

Name Service is a BackEnd application for [Name sorter](https://github.com/matiaslang/NameSorter) user interface, demo application to show occurrences of names listed. This service provides endpoints to modify and fetch data saved in AWS dynamo database.

------

#### How does it work?

At the moment the backend can only be accessed via API -requests, for example by using Postman. Name Service UI uses these endpoints after the request has been authrorized by using different service which provides a token to be included with the request.



------

#### TODOs:

- [ ] Add support for getting the GET payload namelist sorted either in alphabetical order, or based by amount

- [ ] Improve payload error handling

- [ ] Improve validation for incoming requests

- [ ] implement proper unit tests

- [ ] Improve comments

- [ ] Add easier way for testing locally without any requirements (AWS accounts etc)

  