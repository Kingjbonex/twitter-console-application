# twitter-console-application
A Twitter .net core console application.

## Features
Using the Twitter API v2 sampled stream endpoint, the application keeps track of the following:

* Total number of tweets received 
* Top 10 Hashtags

## Running the tool
Running the tool from 
> .\TwitterCLI.exe sample --apikey 2QcUn...vyxZc --secretkey lFoM3uYs4TJyN...VWiNVSeMeDt --token AAAAAAAAAA...yG4s1Ed8nHG --time 1

This will run the application for 1 minute and output the top 10 hashtags pulled from the Twitter Sample Stream V2. Replace the apikey, secretkey, and token with values from your Developer Portal on Twitter.

## Setting up Developer Environment

When running this tool, the CLI library will detect the encoding used by the output.

> Spectre.Console can detect the following items:
 Output encoding: the built-in widgets will use the encoding that is detected to fallback when needed when UTF-8 is not detected e.g. if a Table is configured to use a rounded border, 
 but a user's output encoding does not support the extended ASCII characters then a fallback set of characters will be used.

If you do not have your terminal of choice configured to output UTF-8 properly, you will see interesting results in the hashtag table. You will be able to select and copy said text and paste it into an editor to see the text.

see this post for help: https://stackoverflow.com/questions/49476326/displaying-unicode-in-powershell