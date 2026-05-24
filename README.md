# ✈️ FlashSalePlatform (GalaJet Premium Flight Booking)

## 🚀 Live Demo & Quick Links
* **🌍 Frontend (User Interface):** [https://flash-sale-platform.vercel.app](https://flash-sale-platform.vercel.app)
* **⚙️ Backend API (Swagger UI):** [https://galajet-api-b2eyb4b6dbexfnd2.italynorth-01.azurewebsites.net/swagger](https://galajet-api-b2eyb4b6dbexfnd2.italynorth-01.azurewebsites.net/swagger)

> **💡 Note for Reviewers:** The backend API is hosted on Azure App Service. If the platform has been inactive, the first API request (e.g., fetching flights) might take a few seconds due to Azure's standard "cold start" behavior.

---

## 📖 Introduction
FlashSalePlatform is a full-stack flight booking platform that I built to simulate a real cloud-based reservation system. The application lets users sign up, log in, browse flights, open a flight’s checkout page, see a dynamic price breakdown, and complete a booking through a secure API flow.

I built this project to practice modern web development end to end. It combines a React frontend, an ASP.NET Core backend, Supabase authentication, Redis-based booking safety, Azure deployment for the API, Vercel deployment for the frontend, and GitHub Actions for automated delivery.

## ✨ What the Project Does
* **🔐 Authentication:** Lets users create an account or log in with Supabase Auth.
* **🛩️ Flight Browsing:** Shows available flights on a dynamic dashboard.
* **💳 Dynamic Checkout:** Passes the selected flight to the checkout page and calculates a realistic ticket price breakdown.
* **🎫 Secure Reservations:** Creates bookings through a protected backend API.
* **📂 Order History:** Displays the user’s tickets after booking.
* **🛑 Concurrency Control:** Prevents overselling by using a Redis distributed lock.
* **📊 Observability:** Sends logs and telemetry to Azure monitoring tools.

---

## 🛠️ Tech Stack

### 💻 Frontend
* **⚛️ React 19** & **TypeScript**
* **⚡ Vite**
* **🎨 Tailwind CSS**
* **🛣️ React Router**
* **🔗 Axios** & **Supabase JS**
* **🔔 React Hot Toast** & **Lucide React**

### ⚙️ Backend
* **🟢 ASP.NET Core 9 Web API**
* **🗄️ Entity Framework Core** & **🐘 PostgreSQL**
* **🔄 MediatR** (CQRS Pattern)
* **🛑 StackExchange.Redis**
* **🔑 JWT Bearer Authentication**
* **📝 Serilog** & **📊 Application Insights**
* **📈 Prometheus Metrics**
* **📖 Swagger / OpenAPI**

### ☁️ Cloud and DevOps
* **☁️ Azure App Service** (Backend Hosting)
* **▲ Vercel** (Frontend Hosting)
* **🐳 Docker** (Containerized Backend Deployment)

---

## 📂 Project Structure

```text
FlashSalePlatform/
├── Dockerfile
├── README.md
├── TicketAPI/                # .NET 9 Backend
│   ├── Controllers/
│   ├── Commands/
│   ├── Queries/
│   ├── Handlers/
│   ├── DTOs/
│   ├── Data/
│   ├── Exceptions/
│   ├── Middleware/
│   ├── Models/
│   ├── Migrations/
│   └── Program.cs
└── galajet-frontend/         # React/Vite Frontend
	├── src/
	│   ├── components/
	│   ├── context/
	│   ├── services/
	│   ├── types/
	│   ├── App.tsx
	│   └── main.tsx
	├── package.json
	└── vercel.json
```

## ⚙️ How the Application Works

### 1. Authentication 🔐
The user starts on the login page. The same screen supports both sign-up and login. Supabase Auth handles registration, session creation, and token management.

### 2. Dashboard 🛫
After authentication, the user reaches the dashboard. The dashboard loads flights from the backend and shows route, date, and base price for each flight.

### 3. Checkout 🛒
When the user clicks **Bilet Al**, the selected flight object is passed to the checkout page through React Router state. The checkout page shows the real route and date and calculates the full price breakdown.

### 4. Booking ✅
When the payment form is submitted, the frontend sends the booking request to the backend with the Supabase access token. The backend validates the user, checks the flight, and creates the booking.

### 5. Ticket Listing 🎫
The user can go to **My Tickets** and see booked flights after the booking is completed.

🧠 Important Backend Features
🛑 Redis Distributed Lock: I added a Redis-based lock in the booking handler to prevent overselling. If multiple users try to book the same flight at the same time, only one request can proceed at a time.

🛡️ Global Exception Handling: The API uses centralized exception middleware. This keeps the responses clean and consistent instead of returning raw server errors.

🔑 JWT Protection: The backend accepts Supabase JWT access tokens and protects booking-related endpoints.

📊 Logging and Monitoring: Serilog is used for structured logs. Application Insights is connected for telemetry, and Prometheus metrics are exposed for runtime visibility.

🎨 Frontend Features
👋 Login and Sign-up: The login page includes both login and sign-up flow in the same UI.

📱 Flight Dashboard: Flights are shown in a card layout with a modern Tailwind design.

💰 Checkout Summary: The checkout page dynamically shows base price, taxes and fees, service fee, and total amount.

🔒 Protected Navigation: Authenticated pages are protected so users cannot access them before logging in.

🌍 Deployment
* **Frontend Deployment on Vercel:** The frontend is deployed to Vercel. I added a `vercel.json` rewrite so React Router works correctly on refresh and direct navigation.
* **Backend Deployment on Azure:** The backend is deployed to Azure App Service using a Docker-based workflow.

💻 Local Development

### Prerequisites
* Node.js
* .NET 9 SDK
* PostgreSQL & Redis
* Supabase Project

### Run the Backend ⚙️
```bash
cd TicketAPI
dotnet restore
dotnet run
```

```bash
# You can also run it on a custom port:
dotnet run --urls http://localhost:5274
```

### Run the Frontend 🌐
```bash
cd galajet-frontend
npm install
npm run dev
```

### Environment Variables 🔐

#### Frontend `.env`
```env
VITE_SUPABASE_URL=your_supabase_url
VITE_SUPABASE_ANON_KEY=your_supabase_anon_key
VITE_API_BASE_URL=http://localhost:5274/
```

#### Backend Configuration (`appsettings.json` or Environment Variables)
```json
"ConnectionStrings__Supabase": "your_postgres_connection_string",
"ConnectionStrings__Redis": "your_redis_connection_string",
"SupabaseAuth__Issuer": "your_supabase_issuer",
"SupabaseAuth__Audience": "authenticated",
"APPLICATIONINSIGHTS_CONNECTION_STRING": "your_application_insights_connection_string"
```

### Docker 🐳
The backend includes a Dockerfile for containerized deployment.

```bash
# Build the image
docker build -t flashsaleplatform-api .

# Run the container
docker run -p 8080:8080 flashsaleplatform-api
```

🎯 Why This Project is Useful
This project helped me combine frontend development, backend API design, cloud deployment, authentication, observability, and concurrency control in one application. It is a robust example of how a real-world cloud booking platform can be built with modern tools.

📝 Summary
FlashSalePlatform is a complete cloud-based flight booking platform with a React frontend, ASP.NET Core backend, Supabase authentication, Redis booking safety, Azure hosting, Vercel hosting, and GitHub Actions automation. It was built as both a comprehensive learning experience and a strong portfolio project.