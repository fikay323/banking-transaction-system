# Banking Transaction & Rewards System

A robust, enterprise-grade banking transaction system built with .NET 10, featuring a Rich Domain Model, Clean Architecture, and automated reward point calculations.

## 🚀 Overview

This project implements a secure and scalable banking backend designed to handle core transaction processing (Transfers and Airtime) while enforcing complex business rules for loyalty rewards. It uses a layered architecture to ensure separation of concerns, testability, and maintainability.

## 🏗 Architecture

The system follows **Clean Architecture / Domain-Driven Design (DDD)** principles:

-   **Domain Layer**: Contains rich entities (`Account`, `Customer`, `Transaction`) and encapsulates core business logic, including the reward point calculation engine.
-   **Application Layer**: Orchestrates business flows using the **MediatR** pattern (CQRS-lite). Defines commands, queries, and interfaces.
-   **Infrastructure Layer**: Implements technical concerns such as data access using **Dapper**, transaction management via **Unit of Work**, and audit logging.
-   **API Layer**: The entry point of the application, implementing RESTful endpoints, global exception handling (RFC 7807), and Swagger documentation.

## 🛠 Technology Stack

-   **Runtime**: .NET 10
-   **Database**: SQL Server 2022
-   **ORM/Data Access**: Dapper
-   **Pattern**: MediatR (Command/Query separation)
-   **Containerization**: Docker & Docker Compose
-   **Documentation**: Swagger/OpenAPI

## ✨ Key Features

### 1. Rich Domain Logic
Unlike traditional "anemic" models, the business rules are encapsulated within Domain Entities. For example, the `Account` entity manages its own balance and processes transactions according to reward rules.

### 2. Reward Points Engine
Automated calculation of loyalty points for every transaction:
-   **Base Points**: 1 point per currency unit of the transaction amount.
-   **Tiered Multipliers**:
    -   **Corporate Accounts**: 2x multiplier on points.
    -   **Individual Accounts**: 1x multiplier.
-   **Tenure Bonus**: Accounts older than 4 years receive a **1.5x bonus** on final points.
-   **Exclusions**: Specific transaction types (e.g., Airtime) are excluded from reward points.

### 3. Banking Safeguards
-   **Idempotency**: Prevents duplicate transaction processing using unique request keys.
-   **Unit of Work**: Ensures database atomicity—either all changes (balance, transaction log, points) succeed or all fail.
-   **Audit Logging**: Every sensitive action (Transfers, Airtime purchase) is logged for security and compliance.

### 4. Enterprise Standards
-   **RFC 7807 Error Handling**: Standardized problem details for API errors.
-   **Clean separation**: Strict dependency flow from outer layers to inner Domain.

## 🚦 Getting Started

### Prerequisites
-   Docker Desktop
-   .NET 10 SDK (or compatible)

### Setup & Execution
1.  **Clone the repository**
2.  **Start the Database**:
    ```bash
    docker-compose up -d
    ```
    This will start a SQL Server instance and automatically initialize the schema and seed data via `init.sql`.
3.  **Run the API**:
    ```bash
    dotnet run --project API/API.csproj
    ```

## 🗄 Database Schema
The system uses a relational schema optimized for banking:
-   `CustomerData`: Core customer information.
-   `AccountData`: Account balances and metadata.
-   `TransactionData`: Immutable log of all transactions.
-   `CustomerRewardState`: Snapshot of loyalty points and qualifying counts.
-   `AuditLogs`: Traceability for all system activities.
-   `IdempotencyKeys`: Tracking for request deduplication.

---
*Note: This project is intended for assessment and demonstration of enterprise .NET development patterns.*
