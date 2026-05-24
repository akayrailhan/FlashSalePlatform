# FlashSalePlatform

FlashSalePlatform is a full-stack flight booking platform that I built to simulate a real cloud-based reservation system. The application lets users sign up, log in, browse flights, open a flight’s checkout page, see a dynamic price breakdown, and complete a booking through a secure API flow.

I built this project to practice modern web development end to end. It combines a React frontend, an ASP.NET Core backend, Supabase authentication, Redis-based booking safety, Azure deployment for the API, Vercel deployment for the frontend, and GitHub Actions for automated delivery.

## What the project does

- Lets users create an account or log in with Supabase Auth
- Shows available flights on a dashboard
- Passes the selected flight to the checkout page
- Calculates a realistic ticket price breakdown
- Creates bookings through a protected backend API
- Displays the user’s tickets after booking
- Prevents overselling by using a Redis distributed lock
- Sends logs and telemetry to Azure monitoring tools

## Tech stack

### Frontend
- React 19
- TypeScript
- Vite
- Tailwind CSS
- React Router
- Axios
- Supabase JS
- React Hot Toast
- Lucide React

### Backend
- ASP.NET Core 9 Web API
- Entity Framework Core
- PostgreSQL
- MediatR
- StackExchange.Redis
- JWT Bearer authentication
- Serilog
- Application Insights
- Prometheus metrics
- Swagger / OpenAPI

### Cloud and DevOps
- Azure App Service for the backend
- Vercel for the frontend
- GitHub Actions for CI/CD
- Docker for containerized backend deployment

## Project structure

```text
FlashSalePlatform/
├── Dockerfile
├── README.md
├── .github/
│   └── workflows/
│       └── deploy-azure.yml
├── TicketAPI/
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
└── galajet-frontend/
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

## How the application works

### 1. Authentication
The user starts on the login page. The same screen supports both sign-up and login. Supabase Auth handles registration, session creation, and token management.

### 2. Dashboard
After authentication, the user reaches the dashboard. The dashboard loads flights from the backend and shows route, date, and base price for each flight.

### 3. Checkout
When the user clicks **Bilet Al**, the selected flight object is passed to the checkout page through React Router state. The checkout page shows the real route and date and calculates the full price breakdown.

### 4. Booking
When the payment form is submitted, the frontend sends the booking request to the backend with the Supabase access token. The backend validates the user, checks the flight, and creates the booking.

### 5. Ticket listing
The user can go to **My Tickets** and see booked flights after the booking is completed.

## Important backend features

### Redis distributed lock
I added a Redis-based lock in the booking handler to prevent overselling. If multiple users try to book the same flight at the same time, only one request can proceed at a time.

### Global exception handling
The API uses centralized exception middleware. This keeps the responses clean and consistent instead of returning raw server errors.

### JWT protection
The backend accepts Supabase JWT access tokens and protects booking-related endpoints.

### Logging and monitoring
Serilog is used for structured logs. Application Insights is connected for telemetry, and Prometheus metrics are exposed for runtime visibility.

## Frontend features

### Login and sign-up
The login page includes both login and sign-up flow in the same UI.

### Flight dashboard
Flights are shown in a card layout with a modern Tailwind design.

### Checkout summary
The checkout page shows:
- base price
- taxes and fees
- service fee
- total amount

### Protected navigation
Authenticated pages are protected so users cannot access them before logging in.

## Deployment

### Frontend deployment on Vercel
The frontend is deployed to Vercel. I added a `vercel.json` rewrite so React Router works correctly on refresh and direct navigation.

### Backend deployment on Azure
The backend is deployed to Azure App Service using a Docker-based workflow.

### CI/CD with GitHub Actions
The workflow in `.github/workflows/deploy-azure.yml` builds the Docker image, pushes it to GitHub Container Registry, logs in to Azure with OIDC, and deploys the image to the Azure Web App.

## Local development

### Prerequisites
- Node.js
- .NET 9 SDK
- PostgreSQL
- Redis
- Supabase project

### Run the backend
```bash
cd TicketAPI
dotnet restore
dotnet run
```

You can also run it on a custom port:

```bash
dotnet run --urls http://localhost:5274
```

### Run the frontend
```bash
cd galajet-frontend
npm install
npm run dev
```

## Environment variables

### Frontend `.env`
```env
VITE_SUPABASE_URL=your_supabase_url
VITE_SUPABASE_ANON_KEY=your_supabase_anon_key
VITE_API_BASE_URL=http://localhost:5274/
```

### Backend configuration
```env
ConnectionStrings__Supabase=your_postgres_connection_string
ConnectionStrings__Redis=your_redis_connection_string
SupabaseAuth__Issuer=your_supabase_issuer
SupabaseAuth__Audience=authenticated
APPLICATIONINSIGHTS_CONNECTION_STRING=your_application_insights_connection_string
```

## Docker

The backend includes a Dockerfile for containerized deployment.

### Build the image
```bash
docker build -t flashsaleplatform-api .
```

### Run the container
```bash
docker run -p 8080:8080 flashsaleplatform-api
```

## Why this project is useful

This project helped me combine frontend development, backend API design, cloud deployment, authentication, observability, and concurrency control in one application. It is a good example of how a real-world cloud booking platform can be built with modern tools.

## Summary

FlashSalePlatform is a complete cloud-based flight booking platform with a React frontend, ASP.NET Core backend, Supabase authentication, Redis booking safety, Azure hosting, Vercel hosting, and GitHub Actions automation. It was built as both a learning project and a strong portfolio project.