# Library Management System

A comprehensive library management system built with C# and .NET.

## Features

- **User Authentication** with Argon2 password hashing
- **Role-based Access Control** (Admin/User)
- **Fuzzy Search** for books with typo tolerance
- **Overdue Book Alerts** with color-coded warnings
- **Session Management** for secure user sessions
- **File-based Data Storage** (books.txt, users.txt, borrow.txt)

## Architecture

- **Modular Design** with separate services:
  - `AuthService` - Authentication and registration
  - `SessionService` - User session management
  - `PasswordService` - Secure password hashing with Argon2
  - `Books` - Book operations and search
  - `Users` - User management

## Usage

1. Run the application
2. Login with admin credentials: `admin` / `123`
3. Use admin panel to manage books and users
4. Regular users can search, borrow, and return books

## Security Features

- Argon2id password hashing
- Timing attack protection
- Session-based authentication
- Role-based authorization

## Requirements

- .NET 8.0
- Konscious.Security.Cryptography.Argon2 package