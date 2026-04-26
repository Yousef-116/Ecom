<div align="center">
  <img src="docs/images/banner.png" alt="Ecom Banner" width="100%">
  <br />
  
  # ⚙️ Ecom Backend: High-Performance API
  
  **The robust, scalable, and secure backbone of the Ecom platform, built with .NET 8.**
  
  [![.NET 8](https://img.shields.io/badge/.NET-8.0-512bd4?style=for-the-badge&logo=dotnet)](https://dotnet.microsoft.com/)
  [![Entity Framework Core](https://img.shields.io/badge/EF_Core-8.0-512bd4?style=for-the-badge&logo=dotnet)](https://learn.microsoft.com/en-us/ef/core/)
  [![Redis](https://img.shields.io/badge/Redis-FF4438?style=for-the-badge&logo=redis&logoColor=white)](https://redis.io/)
  [![SQL Server](https://img.shields.io/badge/SQL_Server-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)](https://www.microsoft.com/sql-server)
  [![Swagger](https://img.shields.io/badge/Swagger-85EA2D?style=for-the-badge&logo=swagger&logoColor=black)](https://swagger.io/)
</div>

---

## 🌟 Backend Overview

This repository contains the backend implementation for the **Ecom** platform. It is a RESTful API designed using **Clean Architecture** principles, ensuring that the business logic is isolated from external concerns. The API is optimized for speed, security, and developer productivity.

## 🏗️ Architectural Foundations

The backend is structured into four main layers to maintain a strict separation of concerns:

- **Ecom.API**: The presentation layer. Contains Controllers, Middlewares, and API-specific configurations.
- **Ecom.Core**: The domain layer. Defines Entities, Interfaces, DTOs, and core business logic.
- **Ecom.Infrastructure**: The data layer. Implements repository interfaces, EF Core DbContext, migrations, and external service integrations (Redis, Email).
- **Unit of Work & Repository Pattern**: Centralized data access logic to ensure atomic transactions and clean service code.

## 🎮 Controller Breakdown

The API is organized into several key controllers, each handling a specific domain of the application:

| Controller | Responsibility | Key Features |
| :--- | :--- | :--- |
| **`ProductsController`** | Catalog Management | Pagination, Sorting, Multi-category filtering, File uploads, Output Caching. |
| **`BasketsController`** | Shopping State | High-speed CRUD operations backed by **Redis** for ephemeral data storage. |
| **`OrdersController`** | Checkout & History | Secure order creation, status tracking, and retrieval of user-specific history. |
| **`AccountController`** | Identity & Auth | Secure registration/login using JWT, profile management, and role-based access. |
| **`PaymentsController`** | Stripe Integration | Payment Intent creation and management via Stripe's payment gateway. |
| **`CategoriesController`** | Taxonomy | Hierarchical management of product categories for structured browsing. |
| **`ProductRatings`** | Social Proof | Customer review system including star ratings and feedback text. |

## 🚀 Technical Features

### ⚡ Performance Optimization
- **Redis Caching**: Used for shopping baskets to ensure sub-millisecond response times.
- **Output Caching**: Product and Category endpoints are cached at the API level to reduce database load.
- **Asynchronous Processing**: Every data operation is fully asynchronous (`async/await`) to maximize throughput.

### 🛡️ Security & Reliability
- **JWT Authentication**: Stateless authentication using JSON Web Tokens.
- **Automated Mapping**: Deep mapping between Entities and DTOs using **AutoMapper**.
- **Global Error Handling**: Custom middleware to ensure consistent JSON error responses across the entire API.

### 💳 Financial Integration
- **Stripe Payments**: Robust integration with Stripe for processing secure transactions.
- **Order Management**: Transactional integrity during order placement, ensuring stock and payment consistency.

## 🛠️ Setup & Local Development

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/sql-server)
- [Redis](https://redis.io/download) (or run via Docker: `docker run -p 6379:6379 -d redis`)

### Installation Steps

1.  **Clone & Navigate**
    ```bash
    git clone https://github.com/Yousef-116/Ecom.git
    cd Ecom
    ```

2.  **Restore Dependencies**
    ```bash
    dotnet restore
    ```

3.  **Database Migration**
    ```bash
    cd Ecom.API
    dotnet ef database update
    ```

4.  **Run Application**
    ```bash
    dotnet run
    ```

5.  **Access Documentation**
    Once running, navigate to `https://localhost:xxxx/swagger` to view the interactive API documentation.

## 📬 Future Roadmap
- [ ] Integration of Hangfire for background job processing.
- [ ] Implementation of SignalR for real-time order status updates.
- [ ] Multi-currency and multi-language support.

---
<div align="center">
  Developed by <a href="https://github.com/Yousef-116">Yousef-116</a>
</div>
