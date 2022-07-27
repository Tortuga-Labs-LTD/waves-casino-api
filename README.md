# Waves Casino API (Roulette)

This component monitors a Waves address and pulls chain information to cache it in a database. That information can be queried using a REST API.

## How to run the solution
1. It is required to have a running [PostgreSQL](https://www.postgresql.org/) server running
1. Get your database connection string. Locate your `appsettings` config file in your solution and paste it on the `ConnectionStrings:DefaultConnection` setting
1. Get the Waves address where the Smart Contract is published and copy it. Locate your `appsettings` config file in your solution and paste it on the `games:options:dappAddress` setting
1. (Optionally) you can change the frequency for the background service to sync the changes by changing `games:options:sleepTimeMs` in your `appsettings` config file
1. Database mirgrations need to be applied manually (more information here: [https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/?tabs=vs](https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/?tabs=vs)).
    - Note, to create the database you might need to run `Update-Database` from Visual Studio or equivalent
1. Produce the Docker container or run your solution
