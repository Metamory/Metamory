# Metamory Server

## Development setup

### Set up ASP.NET Core SSL certificate for development.
At the command prompt, run:
```bash
dotnet dev-certs https --trust
```

### To run the server
At the command prompt, run:
```
dotnet run --project Metamory.WebApi
```

### To run the server as a docker container
At the command prompt, run:
```
docker pull aeinbu/metamory:latest
docker run -dit -p 5000:5000 -v ~/metamory-data:/data --name my-metamory aeinbu/metamory
```
Exchange your data directory for `~/metamory-data`.

## License and Copyright
This project is open sourced under the MIT Licence. See [LICENSE.txt](./LICENSE.txt) for details.  
Copyright (c) 2016-2021 Arjan Einbu
