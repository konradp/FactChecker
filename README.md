# Cat Facts

Cat Facts is a project that provides a simple API to fetch random cat facts. Made for Netwise recruitment process.

## Features

- Fetching random cat facts from an external API and returning them to the client.
- Storing the fetched cat facts in both txt and csv formats.


## Technologies

- ASP.NET Core Web API(.NET 8)
- Swashbuckle (Swagger)

## Getting Started

1. Clone the repository.
2. Start the API:
   ```pwsh
   dotnet run
   ```
3. Access Swagger UI at `http://localhost:5000/swagger`.

## API Endpoints

- `GET /Weather/GetCatFact`: Get a random cat fact.

## License

BSD 3-Clause