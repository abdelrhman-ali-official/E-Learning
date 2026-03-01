# 🏗️ E-Learning Platform - Production-Grade Refactoring Plan

## 📋 Executive Summary

This document outlines a comprehensive architectural refactoring to transform the E-Learning platform from a mixed-responsibility system into a production-grade, cleanly-structured educational platform following Clean Architecture principles.

---

## 🔍 Current Architecture Analysis

### **Identified Issues:**

1. **Duplication Between Content and Video Modules**
   - ContentController has: `/api/Content/{id}/stream`, `/api/Content/{id}/live-stream`, `/api/Content/{id}/upload-video`
   - StudentVideoController has: `/api/course/{courseId}/videos/{videoId}`, `/api/course/{courseId}/live`
   - **Violation**: Single Responsibility Principle - Content mixing course metadata with video streaming

2. **Multiple Payment Controllers**
   - ManualPaymentController (manual bank transfers)
   - StudentPaymentController (subscription payments)
   - AdminPaymentController (payment approvals)
   - **Issue**: Inconsistent naming, fragmented payment flow

3. **Hard Deletes in Subscription Module**
   - `DELETE /api/StudentSubscription/{id}` permanently removes records
   - **Risk**: Data loss, no audit trail

4. **Inconsistent Routing Conventions**
   - `/api/AdminPackage` (PascalCase)
   - `/api/AdminCoupon` (PascalCase)
   - `/api/course/{courseId}/videos` (lowercase)
   - **Issue**: Lack of standardization

5. **Missing Course Module**
   - Product entity represents e-commerce products (Brand, Type, Quantity)
   - Content entity actually represents courses but poorly named
   - **Confusion**: No clear "Course" concept in the domain

6. **Entity Relationship Misalignment**
   - CourseVideo.CourseId → Product.Id (should reference Course)
   - LiveSession.CourseId → Product.Id (should reference Course)

---

## 🎯 Refactoring Goals

### **1. Domain Clarity**
- ✅ Introduce explicit **Course** entity
- ✅ Remove video streaming from Content
- ✅ Maintain existing Video and LiveSession entities

### **2. Eliminate Duplication**
- ✅ Remove streaming endpoints from ContentController
- ✅ Consolidate payment controllers
- ✅ Single source of truth for video streaming

### **3. Data Integrity**
- ✅ Soft deletes for all user-facing entities
- ✅ Status enums instead of hard deletes
- ✅ Complete audit trails

### **4. API Consistency**
- ✅ Lowercase REST conventions: `/api/admin/courses`, `/api/videos`
- ✅ Resource-oriented URLs
- ✅ Semantic HTTP verbs (PATCH for status changes, not PUT)

### **5. Production Readiness**
- ✅ CourseAccessGuard for centralized authorization
- ✅ Rate limiting on video endpoints (already implemented)
- ✅ Access logging (already implemented)
- ✅ Analytics module for business intelligence

---

## 📦 Phase-by-Phase Implementation Plan

---

### **PHASE 1: Architecture Cleanup**

#### **1A. Remove Content Streaming Duplication**

**Actions:**
- ❌ **Remove** from ContentController:
  - `GET /api/Content/{id}/stream`
  - `GET /api/Content/{id}/live-stream`
  - `POST /api/Content/{id}/upload-video`
  - `POST /api/Content/{id}/progress`
  - `GET /api/Content/{id}/progress`

- ✅ **Keep** in StudentVideoController (already exists):
  - `GET /api/course/{courseId}/videos/{videoId}` (returns embed URL)
  - `GET /api/course/{courseId}/live` (returns live/recording)
  - Progress tracking endpoints

- ✅ **Deprecated**: Content entity video fields (migration will be created later):
  - `YoutubeVideoId` → NULL (use CourseVideo table instead)
  - `VideoObjectKey` → NULL
  - `LiveStreamUrl` → NULL
  - `IsLiveActive` → Remove entirely

**Justification:**
- VideoModule is purpose-built for YouTube embed streaming
- Content should only represent course metadata
- CourseVideo and LiveSession entities are properly indexed and optimized

---

#### **1B. Merge Payment Controllers**

**Current State:**
```
ManualPaymentController
├── POST /api/ManualPayment (submit request)
├── GET /api/ManualPayment/my-requests
├── GET /api/ManualPayment/pending (Admin)
├── PUT /api/ManualPayment/{id}/approve (Admin)
└── PUT /api/ManualPayment/{id}/reject (Admin)

StudentPaymentController
├── POST /api/StudentPayment/submit
└── GET /api/StudentPayment/subscription/{subscriptionId}

AdminPaymentController
├── GET /api/AdminPayment/pending
├── PUT /api/AdminPayment/{id}/approve
└── PUT /api/AdminPayment/{id}/reject
```

**Refactored Structure:**
```
PaymentController (Student)
├── POST /api/payments/manual (with screenshot upload)
├── POST /api/payments/submit (subscription payment request)
├── GET /api/payments/my
├── GET /api/payments/{id}
└── GET /api/payments/subscription/{subscriptionId}

AdminPaymentController (Admin)
├── GET /api/admin/payments/pending
├── GET /api/admin/payments/all
├── GET /api/admin/payments/{id}
├── PATCH /api/admin/payments/{id}/approve
└── PATCH /api/admin/payments/{id}/reject
```

**Benefits:**
- Single student entry point for all payments
- Clear admin namespace separation
- PATCH for status changes (semantic correctness)

---

#### **1C. Soft Delete for Subscriptions**

**Current:**
```csharp
DELETE /api/StudentSubscription/{id}
// Hard deletes subscription from database
```

**Refactored:**
```csharp
PATCH /api/subscriptions/{id}/cancel
// Sets Status = SubscriptionStatus.Cancelled
```

**New SubscriptionStatus Enum:**
```csharp
public enum SubscriptionStatus
{
    PendingPayment = 1,
    Active = 2,
    Expired = 3,
    Cancelled = 4
}
```

**Migration:**
- Add `Status` column to StudentSubscription table
- Add `CancelledAt` datetime column
- Add `CancellationReason` string column (nullable)

---

#### **1D. Normalize All Routes to Lowercase REST**

**Route Mapping:**

| **Current Route**                     | **Refactored Route**                    |
|---------------------------------------|-----------------------------------------|
| `/api/AdminPackage`                   | `/api/admin/packages`                   |
| `/api/AdminCoupon`                    | `/api/admin/coupons`                    |
| `/api/AdminPayment`                   | `/api/admin/payments`                   |
| `/api/AdminSubscription`              | `/api/admin/subscriptions`              |
| `/api/StudentSubscription`            | `/api/subscriptions`                    |
| `/api/StudentPayment`                 | `/api/payments`                         |
| `/api/ManualPayment`                  | `/api/payments` (merged)                |
| `/api/Content`                        | `/api/courses` (renamed entity)         |
| `/api/Products`                       | `/api/products` (keep for e-commerce)   |
| `/api/course/{courseId}/videos`       | `/api/courses/{courseId}/videos`        |
| `/api/admin/course/{courseId}/videos` | `/api/admin/courses/{courseId}/videos`  |
| `/api/admin/live/{id}`                | `/api/admin/live/{id}` (keep)           |

**Principles:**
- All resources in plural lowercase
- Admin routes prefixed with `/api/admin/`
- Student routes under `/api/`
- No PascalCase in URLs

---

### **PHASE 2: Proper Course Module**

#### **2A. Create Course Entity**

**New Course Entity** (replaces/enhances Content):
```csharp
public class Course : BaseEntity<Guid>
{
    // Core Information
    public string Title { get; set; } = string.Empty; // Max 200
    public string Slug { get; set; } = string.Empty; // URL-friendly, unique
    public string Description { get; set; } = string.Empty; // Max 5000
    public string? ThumbnailUrl { get; set; }
    
    // Instructor
    public string InstructorId { get; set; } = string.Empty; // FK to AspNetUsers
    public string InstructorName { get; set; } = string.Empty;
    
    // Pricing & Access
    public decimal Price { get; set; }
    public bool IsFree { get; set; } = false;
    
    // Publication Status
    public bool IsPublished { get; set; } = false;
    public bool IsFeatured { get; set; } = false;
    public DateTime? PublishedAt { get; set; }
    
    // Metadata
    public int EstimatedDurationMinutes { get; set; } // Total course length
    public string? Category { get; set; } // e.g., "Backend Development"
    public string[]? Tags { get; set; } // JSON array
    
    // Soft Delete
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    
    // Timestamps
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation Properties
    public User Instructor { get; set; } = null!;
    public ICollection<CourseVideo> Videos { get; set; } = new List<CourseVideo>();
    public ICollection<LiveSession> LiveSessions { get; set; } = new List<LiveSession>();
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public ICollection<CourseReview> Reviews { get; set; } = new List<CourseReview>();
}
```

**Database Configuration:**
- Unique index on `Slug`
- Index on `IsPublished`, `IsFeatured`, `IsDeleted`
- Full-text index on `Title`, `Description` (for search)
- FK to AspNetUsers (Instructor)

---

#### **2B. Create Enrollment Entity** (replaces Purchase for courses)

**Why Enrollment?**
- **Purchase** is for one-time content buys
- **Enrollment** represents student access to a course (can be via purchase or subscription)

```csharp
public class Enrollment : BaseEntity<Guid>
{
    public Guid CourseId { get; set; }
    public string StudentId { get; set; } = string.Empty; // FK to AspNetUsers
    
    public EnrollmentSource Source { get; set; } // Purchase, Subscription, Free, Coupon
    public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiresAt { get; set; } // Null = lifetime access
    
    public bool IsActive { get; set; } = true;
    public int ProgressPercentage { get; set; } = 0; // Auto-calculated
    
    // Navigation
    public Course Course { get; set; } = null!;
    public User Student { get; set; } = null!;
}

public enum EnrollmentSource
{
    Purchase = 1,      // One-time buy
    Subscription = 2,  // Active subscription
    Free = 3,          // Free course
    AdminGrant = 4,    // Manually granted by admin
    Coupon = 5         // 100% discount coupon
}
```

---

#### **2C. Course Controllers**

**AdminCourseController**: `/api/admin/courses`
```csharp
POST   /courses                  // Create draft course
PUT    /courses/{id}             // Update course
DELETE /courses/{id}             // Soft delete
PATCH  /courses/{id}/publish     // Publish course
PATCH  /courses/{id}/unpublish   // Unpublish course
PATCH  /courses/{id}/feature     // Mark as featured
GET    /courses                  // Get all (including unpublished)
GET    /courses/{id}             // Get single course
GET    /courses/{id}/analytics   // Course-specific analytics
```

**CourseController**: `/api/courses`
```csharp
GET    /                         // Get published courses (paginated, filterable)
GET    /{slug}                   // Get course by slug (public)
GET    /{courseId}/access        // Check if user has access (returns { hasAccess, reason })
GET    /{courseId}/content       // Get course structure (videos, sessions) - requires access
POST   /{courseId}/enroll        // Enroll (if free) or redirect to payment
GET    /{courseId}/progress      // Get user's progress in course
```

---

### **PHASE 3: Video Module Routing Normalization**

**Already Implemented** - Just update routes to lowercase conventions:

**AdminVideoController**:
```
Current:  /api/admin/course/{courseId}/videos
Updated:  /api/admin/courses/{courseId}/videos  (plural "courses")
```

**StudentVideoController**:
```
Current:  /api/course/{courseId}/videos
Updated:  /api/courses/{courseId}/videos  (plural "courses")
```

**All Endpoints:**
```
ADMIN:
POST   /api/admin/courses/{courseId}/videos
PUT    /api/admin/videos/{id}
DELETE /api/admin/videos/{id}
GET    /api/admin/courses/{courseId}/videos

STUDENT:
GET    /api/courses/{courseId}/videos
GET    /api/courses/{courseId}/videos/{videoId}
POST   /api/courses/{courseId}/videos/{videoId}/progress
GET    /api/courses/{courseId}/videos/{videoId}/progress
GET    /api/courses/{courseId}/progress
```

---

### **PHASE 4: Live Session Module Routing**

**AdminLiveSessionController**:
```
Current:  /api/admin/course/{courseId}/live
Updated:  /api/admin/courses/{courseId}/live
```

**All Endpoints:**
```
ADMIN:
POST   /api/admin/courses/{courseId}/live
PUT    /api/admin/live/{id}
PATCH  /api/admin/live/{id}/activate
PATCH  /api/admin/live/{id}/deactivate
PATCH  /api/admin/live/{id}/attach-recording
GET    /api/admin/courses/{courseId}/live

STUDENT:
GET    /api/courses/{courseId}/live           // Get active live session for course
GET    /api/live/active                       // Get all active live sessions (new)
```

---

### **PHASE 5: Subscription & Payment Cleanup**

**SubscriptionController**: `/api/subscriptions`
```csharp
POST   /subscribe               // Subscribe to package
GET    /me                      // Get my subscriptions
GET    /me/status               // Get current status (active/expired/cancelled)
GET    /{id}                    // Get subscription by ID
PATCH  /{id}/cancel             // Soft delete (set Status = Cancelled)
POST   /calculate-price         // Calculate price with coupon
```

**PaymentController**: `/api/payments` (merged)
```csharp
POST   /manual                  // Submit manual payment (InstaPay/Vodafone)
POST   /submit                  // Submit payment request (for subscription)
GET    /my                      // Get my payments
GET    /{id}                    // Get payment details
GET    /subscription/{subscriptionId}  // Get payments for subscription
```

**AdminPaymentController**: `/api/admin/payments`
```csharp
GET    /pending                 // Get pending approval payments
GET    /                        // Get all payments (paginated)
GET    /{id}                    // Get payment details
PATCH  /{id}/approve            // Approve payment
PATCH  /{id}/reject             // Reject payment
```

**AdminSubscriptionController**: `/api/admin/subscriptions`
```csharp
GET    /                        // Get all subscriptions
GET    /{id}                    // Get subscription details
PATCH  /{id}/extend             // Manually extend subscription (admin grant)
PATCH  /{id}/cancel             // Admin cancellation
```

---

### **PHASE 6: Analytics Module**

**New AdminAnalyticsController**: `/api/admin/analytics`

```csharp
GET    /revenue                 // Revenue analytics (total, monthly, yearly)
GET    /revenue/trends          // Revenue over time (chart data)

GET    /subscriptions           // Subscription analytics (active, cancelled, churn rate)
GET    /subscriptions/trends    // Subscription growth over time

GET    /courses                 // Course analytics (most popular, completion rates)
GET    /courses/{courseId}/stats // Specific course stats

GET    /videos                  // Video analytics (most watched, avg watch time)
GET    /videos/engagement       // Engagement metrics

GET    /students                // Student analytics (total, active, inactive)
GET    /students/activity       // Student activity over time

GET    /dashboard               // Aggregated dashboard data
```

**Response Examples:**

**Revenue Analytics:**
```json
{
  "totalRevenue": 125300.50,
  "monthlyRevenue": 15200.00,
  "yearlyRevenue": 98500.00,
  "revenueBySource": {
    "subscriptions": 85000.00,
    "purchases": 40300.50
  },
  "trends": [
    { "month": "2026-01", "revenue": 12000.00 },
    { "month": "2026-02", "revenue": 15200.00 }
  ]
}
```

**Course Analytics:**
```json
{
  "totalCourses": 45,
  "publishedCourses": 38,
  "totalEnrollments": 1250,
  "averageCompletionRate": 67.5,
  "topCourses": [
    {
      "courseId": "...",
      "title": "Clean Architecture Masterclass",
      "enrollments": 320,
      "completionRate": 78.5,
      "avgRating": 4.7
    }
  ]
}
```

---

## 🔐 Security Enhancements

### **Updated CourseAccessService**

**Responsibilities:**
- Check if user enrolled in course (via Enrollment table)
- Validate subscription access
- Check course publication status
- Prevent enumeration attacks

**Usage:**
```csharp
// In VideoService.GetVideoStreamAsync()
await _courseAccessService.ValidateCourseAccessAsync(courseId, userId);

// In CourseController.GetCourseContent()
var access = await _courseAccessService.GetAccessDetailsAsync(courseId, userId);
return Ok(access);
```

### **Rate Limiting** (already implemented)
- ✅ 30 requests/minute on video endpoints
- ✅ Applied via VideoRateLimitingMiddleware

### **Access Logging** (already implemented)
- ✅ All video/live access logged to VideoAccessLog
- ✅ Includes IP address, timestamp, userId

### **Enumeration Protection**
- ✅ Always validate courseId + videoId combination
- ✅ Return 404 (not 403) for non-existent resources to prevent fishing

---

## 📁 Updated Folder Structure

```
Domain/
  Entities/
    CourseEntities/          # NEW
      Course.cs
      Enrollment.cs
    VideoEntities/
      CourseVideo.cs         # EXISTING - update FK to Course
      LiveSession.cs         # EXISTING - update FK to Course
      VideoProgress.cs       # EXISTING
      VideoAccessLog.cs      # EXISTING
    SubscriptionEntities/
      Package.cs
      StudentSubscription.cs # UPDATE - add Status enum, soft delete
      PaymentRequest.cs      # EXISTING (unified payment entity)
    ContentEntities/
      Content.cs             # DEPRECATED or repurposed for other content
      Purchase.cs            # KEEP for one-time purchases
    SecurityEntities/
      User.cs
      Role.cs

Shared/
  CourseModels/              # NEW
    CreateCourseDTO.cs
    UpdateCourseDTO.cs
    CourseResponseDTO.cs
    EnrollmentResponseDTO.cs
    CourseAccessDTO.cs
  SubscriptionModels/
    SubscriptionResponseDTO.cs  # UPDATE - add Status
    CancelSubscriptionDTO.cs    # NEW
  PaymentModels/             # NEW (merged from multiple folders)
    CreateManualPaymentDTO.cs
    SubmitPaymentDTO.cs
    PaymentResponseDTO.cs
    ApprovePaymentDTO.cs
    RejectPaymentDTO.cs
  AnalyticsModels/           # NEW
    RevenueAnalyticsDTO.cs
    CourseAnalyticsDTO.cs
    SubscriptionAnalyticsDTO.cs

Services/
  CourseService.cs           # NEW
  EnrollmentService.cs       # NEW
  PaymentService.cs          # NEW (merged manual + student)
  AnalyticsService.cs        # NEW

Services.Abstractions/
  ICourseService.cs
  IEnrollmentService.cs
  IPaymentService.cs
  IAnalyticsService.cs

Presentation/
  CourseController.cs        # NEW
  AdminCourseController.cs   # NEW
  PaymentController.cs       # NEW (merged)
  AdminPaymentController.cs  # REFACTORED
  SubscriptionController.cs  # REFACTORED (renamed from StudentSubscription)
  AdminSubscriptionController.cs  # EXISTING
  AdminAnalyticsController.cs     # NEW
  VideoController.cs              # REFACTORED (renamed from StudentVideo)
  AdminVideoController.cs         # REFACTORED
  LiveSessionController.cs        # NEW (student endpoints)
  AdminLiveSessionController.cs   # REFACTORED
```

---

## 🗺️ Complete Route Map (After Refactoring)

### **Public/Anonymous Routes**
```
GET  /api/courses                    # Browse published courses
GET  /api/courses/{slug}             # Get course details
```

### **Student Routes** (Authenticated)
```
# Courses
GET    /api/courses/{courseId}/access
GET    /api/courses/{courseId}/content
POST   /api/courses/{courseId}/enroll
GET    /api/courses/{courseId}/progress

# Videos
GET    /api/courses/{courseId}/videos
GET    /api/courses/{courseId}/videos/{videoId}
POST   /api/courses/{courseId}/videos/{videoId}/progress
GET    /api/courses/{courseId}/videos/{videoId}/progress

# Live Sessions
GET    /api/courses/{courseId}/live
GET    /api/live/active

# Subscriptions
POST   /api/subscriptions/subscribe
GET    /api/subscriptions/me
GET    /api/subscriptions/me/status
PATCH  /api/subscriptions/{id}/cancel
POST   /api/subscriptions/calculate-price

# Payments
POST   /api/payments/manual
POST   /api/payments/submit
GET    /api/payments/my
GET    /api/payments/{id}
```

### **Admin Routes** (Authenticated + Admin Role)
```
# Courses
POST   /api/admin/courses
PUT    /api/admin/courses/{id}
DELETE /api/admin/courses/{id}
PATCH  /api/admin/courses/{id}/publish
GET    /api/admin/courses
GET    /api/admin/courses/{id}

# Videos
POST   /api/admin/courses/{courseId}/videos
PUT    /api/admin/videos/{id}
DELETE /api/admin/videos/{id}
GET    /api/admin/courses/{courseId}/videos

# Live Sessions
POST   /api/admin/courses/{courseId}/live
PUT    /api/admin/live/{id}
PATCH  /api/admin/live/{id}/activate
PATCH  /api/admin/live/{id}/deactivate
PATCH  /api/admin/live/{id}/attach-recording
GET    /api/admin/courses/{courseId}/live

# Subscriptions
GET    /api/admin/subscriptions
PATCH  /api/admin/subscriptions/{id}/extend
PATCH  /api/admin/subscriptions/{id}/cancel

# Payments
GET    /api/admin/payments/pending
GET    /api/admin/payments
PATCH  /api/admin/payments/{id}/approve
PATCH  /api/admin/payments/{id}/reject

# Packages
POST   /api/admin/packages
PUT    /api/admin/packages/{id}
DELETE /api/admin/packages/{id}
GET    /api/admin/packages

# Coupons
POST   /api/admin/coupons
PUT    /api/admin/coupons/{id}
DELETE /api/admin/coupons/{id}
GET    /api/admin/coupons

# Analytics
GET    /api/admin/analytics/revenue
GET    /api/admin/analytics/subscriptions
GET    /api/admin/analytics/courses
GET    /api/admin/analytics/videos
GET    /api/admin/analytics/dashboard
```

---

## 📊 Database Migration Strategy

### **Migration 1: Add Course Entity**
- Create `Courses` table
- Create `Enrollments` table
- Migrate data from `Content` to `Courses` (where Type = Course)
- Update `CourseVideo.CourseId` FK to reference `Courses` instead of `Products`
- Update `LiveSession.CourseId` FK to reference `Courses`

### **Migration 2: Soft Delete Subscriptions**
- Add `Status` enum column to `StudentSubscriptions`
- Add `CancelledAt` datetime column
- Add `CancellationReason` string column
- Set existing active subscriptions: `Status = Active`

### **Migration 3: Deprecate Content Video Fields**
- Set `YoutubeVideoId`, `VideoObjectKey`, `LiveStreamUrl` to NULL
- Add warning comment: "Deprecated - use CourseVideo table"

---

## ✅ Success Criteria

After refactoring, the system must:

1. **Pass All Tests**
   - ✅ Build succeeds with 0 errors
   - ✅ All existing functionality preserved
   - ✅ New endpoints tested via Swagger

2. **Clean Architecture Compliance**
   - ✅ No duplication between Content and Video modules
   - ✅ Proper separation of concerns
   - ✅ Domain entities accurately model business logic

3. **API Consistency**
   - ✅ All routes lowercase with plural resource names
   - ✅ Admin routes under `/api/admin/`
   - ✅ Semantic HTTP verbs (PATCH for status updates)

4. **Data Integrity**
   - ✅ No hard deletes on user-facing entities
   - ✅ Full audit trail via Status enums and timestamps
   - ✅ Proper foreign key relationships

5. **Security**
   - ✅ CourseAccessGuard prevents unauthorized access
   - ✅ Rate limiting active on video endpoints
   - ✅ Access logging for video streams
   - ✅ Enumeration protection

6. **Production Readiness**
   - ✅ Comprehensive analytics for business decisions
   - ✅ Scalable architecture supporting future growth
   - ✅ Clear documentation and code comments

---

## 🚀 Implementation Order

1. **Phase 1A-D**: Architecture cleanup (remove duplication, merge controllers, soft deletes, normalize routes)
2. **Phase 2**: Create Course module (entity, services, controllers)
3. **Phase 3-4**: Update Video and Live Session routing to lowercase
4. **Phase 5**: Clean up Subscription and Payment endpoints
5. **Phase 6**: Add Analytics module
6. **Final**: Create migrations, test thoroughly, update API documentation

---

## 📝 Notes

- **Backward Compatibility**: Old routes can be deprecated with 301 redirects if needed
- **Data Migration**: Existing Content records representing courses will be migrated to Course table
- **Testing**: Each phase requires build verification before proceeding
- **Documentation**: Swagger must be updated with XML comments for all new endpoints

---

**Document Version**: 1.0  
**Last Updated**: February 13, 2026  
**Author**: Senior Backend Architect  
**Status**: Ready for Implementation
