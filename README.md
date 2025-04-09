# Blog Web Application

A full-stack blog application built with .NET Core Web API and React.

## Prerequisites

- .NET 7.0 SDK or later
- Node.js 16.x or later
- SQL Server

## Setup

1. Clone the repository
2. Navigate to the project directory
3. Update the database connection string in `BlogWebApi/appsettings.json`
4. Run the following commands to set up the database:
   ```bash
   cd BlogWebApi
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

## Running the Application

### Option 1: Using the batch script (Windows)

1. Double-click `run-dev.bat` or run it from the command prompt
2. This will start both the backend and frontend servers

### Option 2: Running manually

1. Start the backend:
   ```bash
   cd BlogWebApi
   dotnet run
   ```

2. In a new terminal, start the frontend:
   ```bash
   cd blog-frontend
   npm install
   npm run dev
   ```

## Accessing the Application

- Backend API: https://localhost:7001
- Frontend: http://localhost:5173
- Swagger UI: https://localhost:7001/swagger

## Features

- User registration and authentication
- Create, read, update, and delete blog posts
- View all blogs and individual blog posts
- User profile management
- Responsive design

## API Endpoints

- POST /api/auth/register - Register a new user
- POST /api/auth/login - Login user
- GET /api/blogs - Get all blogs
- GET /api/blogs/{id} - Get a specific blog
- POST /api/blogs - Create a new blog
- PUT /api/blogs/{id} - Update a blog
- DELETE /api/blogs/{id} - Delete a blog
- GET /api/blogs/user - Get user's blogs 