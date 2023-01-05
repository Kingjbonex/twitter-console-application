# twitter-console-application
A Twitter .net core console application.

## Features
Using the Twitter API v2 sampled stream endpoint, the application keeps track of the following:

* Total number of tweets received 
* Top 10 Hashtags

## Setting up Developer Environment

### Secrets

There are 3 secrets that you need to set in the Secret Manager Tool.

Using the dotnet user-secrets tool, ensure that you have a secret repository initialized. Run this command in the directory that has the TwitterCLI.csproj.

`dotnet user-secrets init`

Once initialized, add the 3 secrets needed for running the application.

`dotnet user-secrets set KEY_NAME KEY_VALUE`

* twitter_api_key
* twitter_secret_key
* twitter_bearer_token
