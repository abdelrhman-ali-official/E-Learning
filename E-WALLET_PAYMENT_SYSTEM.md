# E-Learning Platform - E-Wallet Payment System

## 📋 SYSTEM MODIFICATIONS COMPLETED

### ✅ What Was Changed:

1. **Removed Stripe Integration**
   - Deleted StripeService.cs
   - Deleted StripeWebhookController.cs
   - Removed IStripeService interface
   - Updated Purchase entity (removed StripeSessionId and StripePaymentIntentId)
   - Simplified PurchaseService (removed checkout logic)
   - Updated all service managers and interfaces

2. **Implemented E-Wallet Manual Payment System**
   - Full manual payment workflow for InstaPay and Vodafone Cash
   - Screenshot upload to Cloudflare R2 storage
   - Admin approval/rejection workflow
   - Automatic purchase creation on approval

3. **Implemented Cloudflare R2 Storage**
   - CloudflareR2StorageService for image uploads
   - File type validation (images only)
   - File size validation (5 MB limit)
   - Secure URL generation

4. **Added FluentValidation**
   - CreateContentDTOValidator
   - CreateManualPaymentRequestDTOValidator
   - ReviewManualPaymentDTOValidator

---

## 🚀 NEXT STEPS - REQUIRED ACTIONS

### 1. Install Required NuGet Packages

```bash
cd "E:\.net\E-Learning"

# Install AWS S3 SDK for Cloudflare R2
dotnet add Services/Services.csproj package AWSSDK.S3

# Install FluentValidation
dotnet add Services/Services.csproj package FluentValidation
dotnet add Services/Services.csproj package FluentValidation.DependencyInjectionExtensions
dotnet add Start/Start.csproj package FluentValidation.AspNetCore

# Optional: For better file handling
dotnet add Presentation/Presentation.csproj package Microsoft.AspNetCore.Http.Features
```

### 2. Create Database Migration

```bash
# Remove old Stripe migrations if needed
# Then create new migration

dotnet ef migrations add RemoveStripeAddCloudflareStorage --project Persistence --startup-project Start

# Apply migration
dotnet ef database update --project Persistence --startup-project Start
```

### 3. Configure Cloudflare R2

#### Step 1: Create R2 Bucket
1. Log in to Cloudflare Dashboard
2. Go to R2 Object Storage
3. Create a new bucket (e.g., "elearning-uploads")
4. **Keep bucket PRIVATE** (do NOT enable public access)

#### Step 2: Generate API Tokens
1. In R2 settings, create API token
2. Select permissions: Object Read & Write
3. Copy Access Key ID and Secret Access Key

#### Step 3: Update appsettings.json
```json
"CloudflareR2": {
  "AccessKey": "YOUR_ACTUAL_ACCESS_KEY_HERE",
  "SecretKey": "YOUR_ACTUAL_SECRET_KEY_HERE",
  "BucketName": "elearning-uploads",
  "Endpoint": "https://YOUR_ACCOUNT_ID.r2.cloudflarestorage.com"
}
```

**How to get your Account ID:**
- Found in Cloudflare Dashboard → R2 → Overview

**Security Note:**
- Bucket is configured as PRIVATE
- System generates pre-signed URLs (7-day expiry) for admin review
- Payment screenshots are NOT publicly accessible
- Format: `https://<ACCOUNT_ID>.r2.cloudflarestorage.com`

**Public Endpoint:**
- If using custom domain: `https://cdn.yourdomain.com`
- If using R2 public URL: Same as Endpoint

---

## 📊 NEW PAYMENT FLOW

### User Journey:

1. **User transfers money via InstaPay/Vodafone Cash**
   - Manually transfers money to admin's wallet
   - Gets reference number from transaction

2. **User submits payment proof**
   ```http
   POST /api/manualpayment
   Content-Type: multipart/form-data
   
   ContentId: 1
   TransferMethod: 1 (1=InstaPay, 2=VodafoneCash)
   ReferenceNumber: "IP20240212001"
   screenshot: [file upload]
   ```
   - System uploads screenshot to Cloudflare R2
   - Creates ManualPaymentRequest with Status=Pending

3. **Admin reviews request**
   ```http
   GET /api/manualpayment/pending
   Authorization: Bearer {admin-token}
   ```
   - Admin sees pending requests
   - Reviews screenshot and reference number

4. **Admin approves or rejects**

   **Approve:**
   ```http
   PUT /api/manualpayment/{id}/approve
   Authorization: Bearer {admin-token}
   ```
   - Creates Purchase record
   - Sets ExpiryDate = Now + AccessDurationWeeks
   - Marks ManualPaymentRequest as Approved
   - **All in ONE transaction (Unit of Work)**

   **Reject:**
   ```http
   PUT /api/manualpayment/{id}/reject
   Content-Type: application/json
   
   {
     "status": 3,
     "rejectionReason": "Invalid reference number"
   }
   ```

5. **User accesses content**
   ```http
   GET /api/content/{id}/access
   Authorization: Bearer {user-token}
   ```
   - System checks for valid purchase
   - Returns YouTube embed URL or PDF download link

---

## 🔐 SECURITY FEATURES

✅ **Private Storage (CRITICAL):**
- Cloudflare R2 bucket configured as **PRIVATE**
- Payment screenshots NOT publicly accessible
- Pre-signed URLs (7-day expiry) generated for admin review only
- Automatic URL expiration after review period

✅ **Duplicate Prevention:**
- Users cannot submit multiple pending requests for same content
- Checks for existing active purchases before submission
- Prevents payment request spam

✅ **File Upload Security:**
- Only authenticated users can upload
- Only images allowed (JPEG, PNG, WebP, GIF)
- Max file size: 5 MB
- Unique file names (GUID-based)
- Organized folder structure (year/month)

✅ **Payment Security:**
- Admin-only approval endpoints
- Idempotent approval (cannot approve twice)
- Transaction safety with Unit of Work
- Validation at every step
- No direct purchase creation by users

✅ **Access Control:**
- Purchase verification required
- Expiry date checking
- Active status validation
- Role-based authorization

---

## 🎯 API ENDPOINTS

### Manual Payment (User)

```http
# Submit payment request with screenshot
POST /api/manualpayment
Authorization: Bearer {user-token}
Content-Type: multipart/form-data

contentId=1&transferMethod=1&referenceNumber=IP123456&screenshot={file}

# Get my payment requests
GET /api/manualpayment/my-requests
Authorization: Bearer {user-token}
```

### Manual Payment (Admin)

```http
# Get pending requests
GET /api/manualpayment/pending?pageIndex=1&pageSize=10
Authorization: Bearer {admin-token}

# Approve request
PUT /api/manualpayment/{id}/approve
Authorization: Bearer {admin-token}

# Reject request
PUT /api/manualpayment/{id}/reject
Authorization: Bearer {admin-token}
Content-Type: application/json

{
  "status": 3,
  "rejectionReason": "Invalid reference number. Please resubmit with correct details."
}
```

### Purchases

```http
# Get my purchases
GET /api/purchase/my-purchases
Authorization: Bearer {user-token}

# Get all purchases (Admin)
GET /api/purchase?pageIndex=1&pageSize=10
Authorization: Bearer {admin-token}
```

### Content Management

```http
# Create content (Admin)
POST /api/content
# Update content (Admin)
PUT /api/content/{id}
# Delete content (Admin)
DELETE /api/content/{id}
# Get visible content (Public)
GET /api/content/visible
# Access purchased content (User)
GET /api/content/{id}/access
```

---

## 🛠️ ARCHITECTURE COMPLIANCE

✅ **Clean Architecture** - All layers properly separated
✅ **Unit of Work** - All database operations use UnitOfWork
✅ **Specifications** - All queries use Specification pattern
✅ **No DbContext Injection** - No direct DbContext usage
✅ **Async/Await** - All operations are asynchronous
✅ **DTOs** - Data transfer objects for all API interactions
✅ **AutoMapper** - Entity to DTO mapping
✅ **FluentValidation** - Input validation
✅ **Role-based Auth** - Admin and User roles
✅ **Transaction Safety** - Manual payment approval is transactional

---

## 📁 FILES CREATED/MODIFIED

### Created:
- `Services/CloudflareR2StorageService.cs`
- `Services.Abstractions/IStorageService.cs`
- `Services/Validators/CreateContentDTOValidator.cs`
- `Services/Validators/CreateManualPaymentRequestDTOValidator.cs`
- `Services/Validators/ReviewManualPaymentDTOValidator.cs`

### Modified:
- `Domain/Entities/ContentEntities/Purchase.cs` (removed Stripe fields)
- `Persistence/Data/Configurations/PurchaseConfiguration.cs`
- `Services/PurchaseService.cs` (simplified, removed Stripe)
- `Services/ManualPaymentService.cs` (added screenshot URL parameter)
- `Services/ServiceManager.cs` (added StorageService)
- `Services.Abstractions/IPurchaseService.cs`
- `Services.Abstractions/IManualPaymentService.cs`
- `Services.Abstractions/IServiceManager.cs`
- `Presentation/PurchaseController.cs` (removed checkout endpoint)
- `Presentation/ManualPaymentController.cs` (added file upload handling)
- `Shared/ContentModels/CreateManualPaymentRequestDTO.cs`
- `Start/appsettings.json` (replaced Stripe with Cloudflare R2)
- `Start/Program.cs` (added FluentValidation)

### Deleted:
- `Services/StripeService.cs`
- `Services.Abstractions/IStripeService.cs`
- `Presentation/StripeWebhookController.cs`
- `Services/Specifications/GetPurchaseBySessionIdSpecification.cs`
- `Shared/ContentModels/CheckoutSessionResultDTO.cs`
- `Shared/ContentModels/CreateCheckoutSessionDTO.cs`

---

## 🧪 TESTING GUIDE

### 1. Upload Screenshot Test

```bash
curl -X POST https://localhost:5001/api/manualpayment \
  -H "Authorization: Bearer YOUR_USER_TOKEN" \
  -F "contentId=1" \
  -F "transferMethod=1" \
  -F "referenceNumber=IP123456789" \
  -F "screenshot=@/path/to/screenshot.jpg"
```

### 2. Admin Approval Test

```bash
curl -X PUT https://localhost:5001/api/manualpayment/1/approve \
  -H "Authorization: Bearer YOUR_ADMIN_TOKEN"
```

### 3. Verify Purchase Created

```bash
curl https://localhost:5001/api/purchase/my-purchases \
  -H "Authorization: Bearer YOUR_USER_TOKEN"
```

### 4. Access Content

```bash
curl https://localhost:5001/api/content/1/access \
  -H "Authorization: Bearer YOUR_USER_TOKEN"
```

---

## ⚙️ ENVIRONMENT VARIABLES (Production)

For production, use environment variables instead of appsettings.json:

```bash
export CloudflareR2__AccessKey="your-access-key"
export CloudflareR2__SecretKey="your-secret-key"
export CloudflareR2__BucketName="elearning-uploads"
export CloudflareR2__Endpoint="https://account-id.r2.cloudflarestorage.com"
export CloudflareR2__PublicEndpoint="https://cdn.yourdomain.com"
```

---

## 🔍 VALIDATION RULES

### CreateManualPaymentRequestDTO:
- ✅ ContentId must be > 0
- ✅ TransferMethod must be 1 or 2
- ✅ ReferenceNumber required, max 100 chars
- ✅ ReferenceNumber can only contain alphanumeric, hyphens, underscores
- ✅ Screenshot file required
- ✅ Screenshot must be image (JPEG, PNG, WebP, GIF)
- ✅ Screenshot max size: 5 MB

### ReviewManualPaymentDTO:
- ✅ Status must be 2 (Approved) or 3 (Rejected)
- ✅ RejectionReason required when status=3
- ✅ RejectionReason max 500 chars

---

## 🎉 SYSTEM READY!

After completing the setup steps above, your e-learning platform will have:

✅ **Complete E-Wallet Payment System**
- InstaPay and Vodafone Cash support
- Screenshot upload to Cloudflare R2
- Admin approval workflow

✅ **No Stripe Dependencies**
- Completely removed
- Cleaner codebase
- No external payment gateway costs

✅ **Production-Ready Security**
- File validation
- Size limits
- Secure storage
- Transaction safety

✅ **Full Admin Control**
- Manual review of all payments
- Approve/reject functionality
- Complete audit trail

---

## 📞 SUPPORT

If you encounter issues:
1. ✅ Check Cloudflare R2 credentials
2. ✅ Verify bucket permissions
3. ✅ Ensure migrations are applied
4. ✅ Check file upload limits in IIS/Kestrel
5. ✅ Review API logs for validation errors

---

**System transformation complete! 🚀**

No Stripe. No external payment gateways. Full manual control.
Perfect for local payment methods and admin oversight.
