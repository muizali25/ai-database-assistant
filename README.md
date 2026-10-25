# Mini POS AI Query Assistant

A small demo app where you can:

- Browse POS tables (Products, Categories, Customers, Sales)
- Ask questions in normal English
- Get answers from your database using Google Gemini

This project has two folders:

- `backend` → .NET 9 Web API + SQL Server
- `frontend` → Next.js website

---

## What you need before starting

Install these on your computer:

1. **[.NET 9 SDK](https://dotnet.microsoft.com/download)**
2. **[Node.js](https://nodejs.org/)** (version 18 or newer)
3. **SQL Server LocalDB**  
   - Comes with Visual Studio / SQL Server Express on Windows  
   - Or use any SQL Server and change the connection string
4. A free **[Google Gemini API key](https://aistudio.google.com/apikey)**

---

## Clone the project

```bash
git clone https://github.com/YOUR_USERNAME/ai-database-assistant.git
cd ai-database-assistant
```

---

## Database setup (important)

**You do NOT need to run migrations.**

This project does **not** use EF migrations.

When you start the backend the first time, it will:

1. Create the database `MiniPosAi` automatically
2. Create all 5 tables automatically
3. Insert sample data automatically (products, customers, sales, etc.)

So after clone, you only need SQL Server LocalDB running. No extra DB scripts.

### Tables created automatically

- Categories
- Products
- Customers
- Sales
- SaleItems

### Default connection string

In `backend/appsettings.json`:

```text
Server=(localdb)\mssqllocaldb;Database=MiniPosAi;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true
```

If you use another SQL Server, change only this connection string.

### Note about Development mode

In Development, the backend may delete and recreate the database on startup so sample data stays fresh. That is normal for this demo.

---

## Step 1: Start the backend

```bash
cd backend
dotnet user-secrets init
dotnet user-secrets set "Gemini:ApiKey" "PASTE_YOUR_GEMINI_KEY_HERE"
dotnet run --launch-profile http
```

Backend URL:

```text
http://localhost:5206
```

### Other way to set Gemini key

1. Copy `backend/appsettings.Local.json.example`
2. Rename copy to `appsettings.Local.json`
3. Put your key inside
4. Run the backend

Do **not** commit your real API key to GitHub.

---

## Step 2: Start the frontend

Open a new terminal:

```bash
cd frontend
copy .env.local.example .env.local
npm install
npm run dev
```

On Mac/Linux use:

```bash
cp .env.local.example .env.local
```

Frontend URL:

```text
http://localhost:3000
```

---

## How to use the app

1. Open `http://localhost:3000`
2. Use the table dropdown to view data
3. Click a sample question, or open the AI chat button
4. Ask a question about products, customers, or sales
5. Wait for the progress bar
6. Results show on the main page

If the question is not related to this POS data, the app shows **Data not found**.

---

## Quick API check (optional)

```text
GET  http://localhost:5206/api/data/Products
POST http://localhost:5206/api/ai/ask
Body: { "question": "List products with low stock" }
```

---

## Common problems

**Backend fails to start / database error**  
- Make sure LocalDB is installed  
- Or update the connection string for your SQL Server

**AI questions fail**  
- Check that your Gemini API key is set correctly

**Frontend cannot load data**  
- Make sure backend is running on `http://localhost:5206`  
- Check `frontend/.env.local` has:

```text
NEXT_PUBLIC_API_URL=http://localhost:5206
```

---

## Project structure

```text
ai-database-assistant/
  backend/     # .NET API, Gemini, SQL validation, seed data
  frontend/    # Next.js UI, table view, AI chat
```

---

## License

See the `LICENSE` file in this repository.
