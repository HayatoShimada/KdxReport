# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a **business trip report submission system (出張報告書提出システム)** built as a Blazor Server Side + Minimal API monolithic application. The system allows users to submit, view, search, and comment on business trip reports with support for Markdown formatting, PDF uploads, and OCR-based search.

**Tech Stack:**
- Blazor Server Side (C#)
- Entity Framework Core with Npgsql Provider
- PostgreSQL database
- MinIO S3 for file storage (PDF and images) - future migration to AWS S3 planned
- Tesseract OCR for document search
- Frontend: Bootstrap, jQuery, SweetAlert2, Toastr, FontAwesome, Google Fonts

**Deployment:**
- Docker containerization with docker-compose.yml
- Timezone: Japan (JST)
- Current target: Local Windows PC server
- Future migration: Azure

## Core Features

### Authentication (✅ Implemented)
- Cookie-based authentication with BCrypt password hashing
- Default password: `123456` (must be changed on first login)
- Role-based access control (Admin/User roles)
- AuthService handles login, logout, password changes
- Forced password change on first login
- Session management with 8-hour expiration

### Business Trip Reports
- Submit reports with: trip period, submitter, companions, customer, equipment, and content
- Support both Markdown content and PDF uploads
- Markdown is rendered to HTML for viewing
- OCR search capability for PDF documents
- Full-text search across database content

### Read Status Tracking (✅ Implemented)
- ReadStatus model tracks which users have read which reports
- Unread reports are automatically shown on the dashboard
- MarkAsRead functionality updates read status when viewing reports

### Approval Workflow (✅ Implemented)
- Three states: pending, approved, rejected
- Admin users can approve/reject reports from dashboard
- Tracks who approved and when (ApprovedBy, ApprovedAt fields)
- Pending approval section visible only to admins

### Threading System
- Each report has an associated thread for comments
- Comments support replies (返信先コメントID field)
- Track who commented and when
- File attachments per thread

## Database Schema

The system uses PostgreSQL with the following main entities:

**Users & Authorization:**
- Users (ユーザー): ID, name, email, password, timestamps
- Roles (ロール): ID, name, timestamps
- RoleUsers (ロールユーザー): Many-to-many relationship

**Business Entities:**
- Companies (会社): ID, name, timestamps
- Company Contacts (会社担当者): ID, name, email, phone, timestamps
- Equipment (設備): ID, name, total counter number, timestamps

**Reports & Communication:**
- Trip Reports (出張報告書): Links to company, contact, equipment; includes trip period, submitter, companions, content, approval status
- ReadStatus (既読状態): Tracks which users have read which reports
- Threads (スレッド): Per-report discussion threads
- Comments (コメント): Thread comments with reply support
- Attachments (添付ファイル): File storage references (MinIO S3 paths)

## Key Architecture Considerations

### Monolithic Design
This is a single application combining Blazor Server Side UI with Minimal API endpoints. Keep all business logic, data access, and presentation concerns in a unified project structure.

### File Storage Strategy
- Store PDFs and images in MinIO S3 (local object storage)
- Store S3 paths in the Attachments table
- Design file storage layer with abstraction to facilitate future AWS S3 migration

### OCR Integration
- Use Tesseract OCR to extract text from PDF uploads
- Index extracted text for search functionality
- Consider caching OCR results to avoid reprocessing

### Timezone Handling
- All datetime operations must use JST (Japan Standard Time)
- Configure Entity Framework and application to consistently use JST
- Be explicit about timezone conversion when displaying or storing dates

### Docker Deployment
- PostgreSQL database runs in container
- MinIO S3-compatible storage runs in container
- Application container connects to both services
- Use docker-compose.yml for orchestration

## Implemented Features

### Services
- **AuthService** (`Services/AuthService.cs`): Authentication, password management
- **TripReportService** (`Services/TripReportService.cs`): Report CRUD, read status, approval workflow

### Pages
- **Login** (`/login`): User authentication
- **Logout** (`/logout`): Sign out
- **ChangePassword** (`/change-password`): Password change with first-login detection
- **Home/Dashboard** (`/`): Displays unread reports, approval queue, statistics

### Database Migrations
- `InitialCreate`: Base schema
- `AddReadStatusAndApproval`: Read tracking and approval workflow

### Known Limitations
- Thread model conflicts with `System.Threading.Thread`, use `Models.Thread` with full namespace
- Database retry logic in Program.cs handles PostgreSQL container startup delays

## Implementation Notes

### Entity Framework Setup
- Use Npgsql provider for PostgreSQL
- Configure migrations for schema management
- Implement proper foreign key relationships per the schema above

### Markdown Rendering
- Implement Markdown-to-HTML conversion for report content display
- Sanitize HTML output to prevent XSS attacks
- Consider using a library like Markdig for C#

### Search Functionality
- Implement dual search: database full-text search AND OCR-extracted content
- Index OCR results for performance
- Consider PostgreSQL full-text search capabilities

### Thread/Comment System
- Implement nested comment display (replies)
- Real-time updates using Blazor Server's SignalR connection
- Track and display comment timestamps and authors
