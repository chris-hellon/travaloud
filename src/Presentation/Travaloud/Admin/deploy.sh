#!/bin/bash

# Step 1: Build and Publish the Application
dotnet publish -c Release -o ./publish

# Step 2: Remove all files on the server
ssh root@167.172.89.238 "rm -rf /var/www/Travaloud/*"

# Step 3: Copy the new files to the server
rsync -av ./publish/ root@167.172.89.238:/var/www/Travaloud

# Step 4: Restart the application using Supervisor
ssh root@167.172.89.238 "sudo supervisorctl restart Travaloud"

# Step 5: Delete the local publish folder
rm -rf ./publish