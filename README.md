# Mini POS AI Query Assistant

Simple Mini POS demo with a .NET 9 Web API backend and a Next.js frontend. Browse table data, ask natural-language questions via Google Gemini, review the generated SQL, then approve and execute read-only queries.

## Prerequisites

- .NET 9 SDK
- Node.js 18+
- SQL Server LocalDB (or update the connection string)
- A [Google Gemini API key](https://aistudio.google.com/apikey)

## Backend setup

```bash
cd backend
dotnet user-secrets init
dotnet user-secrets set "Gemini:ApiKey" "YOUR_GEMINI_API_KEY"
dotnet run --launch-profile http
```

API listens on `http://localhost:5206`.

Optional: copy `appsettings.Local.json.example` to `appsettings.Local.json` and fill in the key (that file is gitignored). If you use a local file, load it from `Program.cs` or keep using user secrets.

### Endpoints

| Method | Path | Purpose |
|--------|------|---------|
| GET | `/api/data/{tableName}` | Return rows for `Products`, `Categories`, `Customers`, or `Sales` |
| POST | `/api/ai/ask` | `{ "question": "..." }` → Gemini SQL, server-side validate, execute |

Only validated read-only `SELECT` queries against allowed POS tables are executed. Out-of-domain or unsafe questions return `{ "found": false, "message": "Data not found" }`.

## Frontend setup

```bash
cd frontend
cp .env.local.example .env.local
npm install
npm run dev
```

Open `http://localhost:3000`.

## Database

On startup the API creates the `MiniPosAi` database (LocalDB by default) with five tables and hardcoded sample data:

- Categories
- Products
- Customers
- Sales
- SaleItems
