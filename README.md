# Real-Time Chat Application with Sentiment Analysis

This project is a full-stack real-time chat application with integrated sentiment analysis. It uses a C# backend with SignalR for real-time communication and a React frontend for the user interface.

## Features

- Real-time messaging using SignalR
- User authentication and chat room functionality
- Sentiment analysis of messages using Azure Cognitive Services
- Persistent storage of chat messages in Azure SQL Database
- Color-coded messages based on sentiment analysis results

## Tech Stack

### Backend
- ASP.NET Core
- SignalR for real-time communication
- Entity Framework Core for database operations
- Azure Cognitive Services for sentiment analysis
- Azure SQL Database for data persistence

### Frontend
- React
- Chakra UI for styling
- @microsoft/signalr for SignalR client

## Prerequisites

- .NET 8.0 SDK
- Node.js and npm
- Azure account (for Cognitive Services and SQL Database)
- Docker and Docker Compose (for local testing)

## Setup

1. Clone the repository: git clone https://github.com/AlexanderSemenuk/RealTimeChat.git

2. Backend setup:
- Navigate to the server directory
- Update the `appsettings.json` file with your Azure SQL Database and Cognitive Services credentials
- Update Redis and Azure SignalR connection strings with your credentials
- Run `dotnet restore` to install dependencies
- Run `dotnet ef database update` to apply migrations to your database

3. Frontend setup:
- Navigate to the client directory
- Run `npm install` to install dependencies
- Update the SignalR hub URL in `App.js` if necessary

4. Local testing with Docker:
- Ensure Docker and Docker Compose are installed on your machine
- Create a `docker-compose.yml` file in the root directory (see below for an example)
- Run `docker-compose up` to start the application

## Docker Compose

For local testing, a `docker-compose.yml` file is needed. Here's a basic example:

```yaml
services:
  chat.redis:
    image: redis
    restart: always
    ports:
      - "6379:6379"
