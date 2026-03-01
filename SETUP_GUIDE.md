# E-Learning Platform - Setup & Usage Guide

## 🎯 Implementation Complete!

Your subscription-based learning platform backend has been fully implemented following Clean Architecture principles.

---

## 📦 Required NuGet Packages

Make sure to install these packages:

```bash
# AWS S3 SDK (for Cloudflare R2 Storage)
dotnet add Services/Services.csproj package AWSSDK.S3

# FluentValidation (REQUIRED)
dotnet add Services/Services.csproj package FluentValidation
dotnet add Services/Services.csproj package FluentValidation.DependencyInjectionExtensions
dotnet add Start/Start.csproj package FluentValidation.AspNetCore
```

---

## 🗄️ Database Migration

Run the following commands to create and apply the migration:

```bash
# Navigate to the solution directory
cd "E:\.net\E-Learning"

# Add migration
dotnet ef migrations add RemoveStripeAddManualPaymentSystem --project Persistence --startup-project Start

# Update database
dotnet ef database update --project Persistence --startup-project Start
```

---

## 🔐 Cloudflare R2 Configuration

1. **Create Cloudflare R2 Bucket**:
   - Go to: https://dash.cloudflare.com → R2 → Create bucket
   - Bucket name: e.g., `elearning-payment-screenshots`
   - **Keep bucket PRIVATE** - do NOT enable public access policy

2. **Create R2 API Token**:
   - Go to R2 → Manage R2 API Tokens → Create API Token
   - Permissions: Object Read & Write
   - Copy: Access Key ID and Secret Access Key

3. **Update appsettings.json** with your actual R2 credentials:
```json
"CloudflareR2": {
  "AccessKey": "your-r2-access-key-id",
  "SecretKey": "your-r2-secret-access-key",
  "BucketName": "elearning-payment-screenshots",
  "Endpoint": "https://<account-id>.r2.cloudflarestorage.com"
}
```

4. **Configure R2 Bucket as PRIVATE (CRITICAL)**:
   - In Cloudflare dashboard: R2 → Your bucket → Settings
   - **Keep bucket PRIVATE** - do NOT enable public access
   - The system automatically generates pre-signed URLs (7-day expiry) for admin review
   - This ensures payment screenshots remain secure and not publicly accessible

---

## 📋 API Endpoints

### **Content Management (Admin)**

```http
POST   /api/content                    # Create content
PUT    /api/content/{id}               # Update content
DELETE /api/content/{id}               # Delete content
PATCH  /api/content/{id}/visibility    # Toggle visibility
PATCH  /api/content/{id}/live-status   # Toggle live session
GET    /api/content                    # Get all (admin)
```

### **Content Access (Public/User)**

```http
GET    /api/content/visible            # Get visible content (public)
GET    /api/content/{id}               # Get single content
GET    /api/content/{id}/access        # Access purchased content (auth)
```

### **Purchase Flow**

```http
GET    /api/purchase/my-purchases      # Get my purchases
GET    /api/purchase                   # Get all purchases (admin)
```

### **Manual Payment (E-Wallet)**

```http
POST   /api/manualpayment                      # Submit payment request (with file upload)
GET    /api/manualpayment/my-requests          # Get my requests
GET    /api/manualpayment/pending              # Get pending (admin)
PUT    /api/manualpayment/{id}/approve         # Approve request (admin)
PUT    /api/manualpayment/{id}/reject          # Reject request (admin)
```

---

## 🔄 Complete Purchase Flow

### **Manual Payment (InstaPay/Vodafone Cash)**

1. **User**: Transfers money via InstaPay/Vodafone Cash to admin's wallet
2. **User**: Takes screenshot of transaction
3. **User**: POST `/api/manualpayment` (multipart/form-data) with:
   - `ContentId`: 1
   - `TransferMethod`: 1 (InstaPay) or 2 (VodafoneCash)
   - `ReferenceNumber`: "REF123456"
   - `Screenshot`: (file upload - JPEG/PNG/WebP/GIF, max 5MB)
4. **Backend**: Validates file and uploads to Cloudflare R2
5. **Admin**: Reviews via `/api/manualpayment/pending`
6. **Admin**: PUT `/api/manualpayment/{id}/approve` (or `/reject`)
   - For rejection: include query param `?reason=Invalid+transaction`
7. **Backend**: If approved, creates Purchase automatically within transaction
8. **User**: Access content via `/api/content/{id}/access`

---

## 🎨 Content Types & Access

### **Video Content**
- Returns YouTube embed URL
- Example: `https://www.youtube.com/embed/VIDEO_ID`

### **Live Content**
- Only accessible when `IsLiveActive = true`
- Returns YouTube live embed URL when active

### **PDF Content**
- Returns download/view URL
- Implement your PDF service (S3, Azure Blob, etc.)

---

## 🔒 Authorization

### **Roles:**
- `Admin`: Full access to all endpoints
- `User`: Can purchase and access content

### **Usage in Controllers:**
```csharp
[Authorize(Roles = "Admin")]  // Admin only
[Authorize]                    // Any authenticated user
[AllowAnonymous]              // Public access
```

---

## ⚙️ Background Service

**PurchaseExpirationBackgroundService** runs every 24 hours to:
- Find purchases where `ExpiryDate < DateTime.UtcNow`
- Set `IsActive = false`
- Automatically blocks expired access

---

## 📊 Database Schema

### **Contents** Table
- Stores all learning content (Video/Live/PDF)
- Controls visibility and pricing

### **Purchases** Table
- Tracks all successful purchases
- Links to ManualPaymentRequest (optional)
- Manages expiry dates and access control

### **ManualPaymentRequests** Table
- Stores e-wallet payment submissions with screenshot URLs
- Admin reviews and approves/rejects
- Creates Purchase on approval (within transaction)

---

## 🧪 Testing Guide

### **1. Create Content (Admin)**
```bash
POST /api/content
{
  "title": "Advanced ASP.NET Core",
  "description": "Complete guide to ASP.NET Core",
  "thumbnailUrl": "https://cdn.com/thumb.jpg",
  "type": 1,  // 1=Video, 2=Live, 3=PDF
  "price": 99.99,
  "accessDurationWeeks": 12,
  "youtubeVideoId": "dQw4w9WgXcQ",
  "isVisible": true
}
```

### **2. Purchase via Manual Payment**
```bash
# Use multipart/form-data
curl -X POST https://localhost:5001/api/manualpayment \
  -H "Authorization: Bearer {token}" \
  -F "ContentId=1" \
  -F "TransferMethod=1" \
  -F "ReferenceNumber=IP20240212001" \
  -F "Screenshot=@/path/to/payment-proof.jpg"
```

### **3. Access Content**
```bash
GET /api/content/1/access
# Returns YouTube embed URL or PDF URL
```

---

## 🛡️ Security Best Practices

✅ **Implemented:**
- JWT Authentication
- Role-based authorization
- Input validation with FluentValidation
- File upload validation (type, size, content)
- **Private R2 bucket with pre-signed URLs (7-day expiry)**
- **Duplicate payment request prevention**
- **Idempotent approval (cannot approve twice)**
- No direct DbContext usage
- Unit of Work transaction management
- Secure access control (purchase verification)
- Transaction-safe payment approval

✅ **Recommended:**
- Use HTTPS in production
- Store Cloudflare R2 keys in Azure Key Vault / environment variables
- Implement rate limiting
- Add request logging
- Use signed URLs for PDF downloads (temporary access)
- Enable CORS only for trusted frontend origins

---

## 🚀 Deployment Checklist

- [ ] Update connection strings
- [ ] Add real Cloudflare R2 API keys and bucket configuration
- [ ] **Ensure R2 bucket is configured as PRIVATE (not public)**
- [ ] Run database migrations
- [ ] Set up email notifications (optional)
- [ ] Configure CORS for frontend
- [ ] Enable logging (Serilog, Application Insights)
- [ ] Set up CDN for content thumbnails
- [ ] Implement PDF secure storage (Azure Blob, S3, or R2)
- [ ] Configure file upload size limits in IIS/Kestrel
- [ ] Test duplicate payment request prevention
- [ ] Verify pre-signed URLs are generated correctly

---

## 📝 Example Requests

### Create Video Content
```json
POST /api/content
{
  "title": "React Masterclass",
  "description": "Learn React from scratch",
  "price": 49.99,
  "type": 1,
  "accessDurationWeeks": 8,
  "youtubeVideoId": "abc123",
  "isVisible": true
}
```

### Submit Manual Payment (Multipart/Form-Data)
```bash
# Note: This is NOT JSON - it's multipart/form-data with file upload
POST /api/manualpayment
Content-Type: multipart/form-data

Form fields:
- ContentId: 1
- TransferMethod: 1
- ReferenceNumber: IP20240212001
- Screenshot: [file] payment-proof.jpg
```

### Approve Manual Payment (Admin)
```bash
PUT /api/manualpayment/5/approve
# No request body needed
```

### Reject Manual Payment (Admin)
```bash
PUT /api/manualpayment/5/reject?reason=Invalid+transaction+reference
# Reason is provided as query parameter
```

---

## 🎓 Architecture Overview

```
┌─────────────────┐
│  Presentation   │  ← Controllers (ContentController, PurchaseController, etc.)
└────────┬────────┘
         │
┌────────▼────────┐
│    Services     │  ← Business Logic (ContentService, PurchaseService, etc.)
└────────┬────────┘
         │
┌────────▼────────┐
│  Persistence    │  ← EF Core, Repositories, Unit of Work
└────────┬────────┘
         │
┌────────▼────────┐
│     Domain      │  ← Entities, Interfaces, Specifications
└─────────────────┘
```

---

## 💡 Key Features Implemented

✅ Content Management (CRUD)
✅ Manual Payment (InstaPay/Vodafone Cash)
✅ Cloudflare R2 File Storage Integration
✅ File Upload Validation (type, size)
✅ Access Control with Expiry
✅ YouTube Embed Integration
✅ Live Session Management
✅ Purchase History
✅ Admin Dashboard Support
✅ Background Purchase Expiration
✅ Role-based Authorization
✅ Clean Architecture
✅ Specification Pattern
✅ Unit of Work Pattern
✅ AutoMapper
✅ FluentValidation
✅ Transaction-Safe Payment Approval
✅ Global Exception Handling (existing)

---

## 📞 Support

If you encounter issues:
1. Check logs for detailed errors
2. Verify Cloudflare R2 credentials and bucket configuration
3. Ensure database migrations are applied
4. Validate API keys in appsettings.json
5. Check user roles are properly assigned
6. Verify file upload size limits (default: 5MB)

---

## 🎉 You're Ready!

Run the application:
```bash
dotnet run --project Start
```

Access Swagger UI: `https://localhost:5001/swagger`

Happy coding! 🚀
