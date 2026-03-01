# Deployment Checklist - Live Session & Google Drive Features

## ✅ Pre-Deployment Verification

### 1. Database Migration
- [x] Migration generated: `EnhanceLiveSessionAndContentModules`
- [x] Migration applied successfully to development database
- [ ] Migration tested on staging environment
- [ ] Backup production database before applying

**Apply to Production:**
```bash
cd Persistence
dotnet ef database update --startup-project ../Start/Start.csproj --context StoreContext
```

---

### 2. Build & Compilation
- [x] Solution builds without errors
- [x] All unit tests pass (if applicable)
- [x] No breaking changes to existing endpoints

**Verify:**
```bash
dotnet build --configuration Release
dotnet test
```

---

### 3. Configuration Updates

#### Required App Settings
Ensure `appsettings.json` includes:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Database=...;"
  },
  "Jwt": {
    "Key": "your-secret-key",
    "Issuer": "your-issuer",
    "Audience": "your-audience"
  }
}
```

#### Environment Variables (Production)
- [ ] Database connection string configured
- [ ] JWT secret key set (strong, unique)
- [ ] CORS origins configured
- [ ] Logging levels appropriate for production

---

### 4. API Documentation
- [x] Swagger documentation updated
- [x] API testing guide updated ([API_TESTING_GUIDE.http](API_TESTING_GUIDE.http))
- [x] Sample requests documented
- [ ] Postman collection exported (optional)

---

### 5. Security Review

#### Authentication & Authorization
- [x] Admin endpoints require `[Authorize(Roles = "Admin")]`
- [x] Instructor endpoints validate ownership
- [x] Student endpoints check enrollment
- [x] JWT token validation enabled

#### Input Validation
- [x] FluentValidation rules applied
- [x] URL validation for meeting links
- [x] URL validation for Google Drive links
- [x] Date range validation (StartTime < EndTime)

#### Data Protection
- [ ] Sensitive fields not exposed in DTOs
- [ ] SQL injection protected (EF Core parameterized queries)
- [ ] XSS protection enabled
- [ ] HTTPS enforced in production

---

### 6. Testing Checklist

#### Live Session Endpoints
- [ ] Admin creates session successfully
- [ ] Instructor creates session for own course
- [ ] Instructor cannot edit others' sessions
- [ ] Student views upcoming sessions
- [ ] Session `IsLive` computed correctly based on time
- [ ] Activation/deactivation works
- [ ] Delete cascades correctly

#### Google Drive Integration
- [ ] Upload content with Google Drive URL
- [ ] URL converts to `/preview` format
- [ ] Non-Drive URLs stored as-is
- [ ] Validation rejects invalid URLs
- [ ] Content retrieval includes `externalVideoUrl`

#### Authorization Tests
- [ ] Unauthorized users receive 401
- [ ] Non-admin/instructor receive 403 on admin endpoints
- [ ] Students cannot access non-enrolled course sessions

---

### 7. Performance Considerations

#### Database Indexes
- [x] Index on `LiveSessions.CourseId`
- [x] Index on `LiveSessions.InstructorId`
- [x] Index on `LiveSessions.ScheduledStart`
- [x] Index on `LiveSessions.IsActive`

#### Query Optimization
- [ ] Review query performance for `GetUpcomingSessions`
- [ ] Implement pagination for large result sets
- [ ] Consider caching for frequently accessed data

---

### 8. Monitoring & Logging

#### Application Insights / Logging
- [ ] Log live session creation events
- [ ] Log authorization failures
- [ ] Monitor API response times
- [ ] Track validation errors

#### Health Checks
- [ ] Database connectivity check
- [ ] External services (Zoom API, if integrated)

---

### 9. Rollback Plan

#### If Issues Arise
```bash
# Rollback migration
cd Persistence
dotnet ef database update PreviousMigrationName --startup-project ../Start/Start.csproj --context StoreContext

# Redeploy previous version
git checkout previous-stable-tag
dotnet publish -c Release
```

#### Data Backup
- [ ] Database backup taken before deployment
- [ ] Backup retention policy confirmed

---

### 10. Post-Deployment Verification

#### Smoke Tests
```bash
# 1. Health check
curl https://your-api.com/health

# 2. Test admin login
curl -X POST https://your-api.com/api/authentication/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@example.com","password":"AdminPass123!"}'

# 3. Create test live session
curl -X POST https://your-api.com/api/admin/courses/1/live \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{
    "title":"Test Session",
    "meetingLink":"https://zoom.us/j/123",
    "instructorId":"test-id",
    "scheduledStart":"2026-03-01T10:00:00Z",
    "scheduledEnd":"2026-03-01T11:00:00Z"
  }'

# 4. Test Google Drive content
curl -X POST https://your-api.com/api/content \
  -H "Authorization: Bearer {admin-token}" \
  -H "Content-Type: application/json" \
  -d '{
    "title":"Test Video",
    "type":1,
    "price":10,
    "accessDurationWeeks":4,
    "externalVideoUrl":"https://drive.google.com/file/d/abc123/view"
  }'
```

#### Verify
- [ ] All endpoints respond correctly
- [ ] Swagger UI loads at `/swagger`
- [ ] Database queries execute without errors
- [ ] Logs show no critical errors

---

### 11. User Communication

#### Notify Stakeholders
- [ ] Email admin users about new features
- [ ] Update instructor documentation
- [ ] Provide student guide for viewing sessions
- [ ] Announce Google Drive video support

#### Documentation Updates
- [ ] Update user manual
- [ ] Update API documentation site
- [ ] Create tutorial videos (optional)

---

### 12. Maintenance Plan

#### Regular Tasks
- [ ] Monitor session creation metrics
- [ ] Review inactive sessions weekly
- [ ] Archive old recordings
- [ ] Update Google Drive permissions

#### Known Limitations
- Instructor ownership validation requires manual check (TODO: integrate with Course entity)
- Student enrollment validation needs CourseAccessService integration
- No automated notifications (future enhancement)

---

## 🚀 Deployment Steps

### Option A: Azure App Service
```bash
# 1. Publish application
dotnet publish -c Release -o ./publish

# 2. Deploy to Azure
az webapp deployment source config-zip \
  --resource-group YourResourceGroup \
  --name YourAppName \
  --src ./publish.zip

# 3. Run migrations on Azure SQL
dotnet ef database update --connection "Azure-Connection-String"
```

### Option B: Docker Deployment
```bash
# 1. Build Docker image
docker build -t elearning-api:latest .

# 2. Run container
docker run -d -p 5000:80 \
  -e ConnectionStrings__DefaultConnection="..." \
  -e Jwt__Key="..." \
  elearning-api:latest

# 3. Run migrations
docker exec -it container-id dotnet ef database update
```

### Option C: IIS Deployment
```bash
# 1. Publish to folder
dotnet publish -c Release -o C:\inetpub\wwwroot\elearning-api

# 2. Configure IIS application pool (.NET 8.0)
# 3. Update web.config with production connection string
# 4. Run migrations via command line
```

---

## ✅ Sign-Off

- [ ] **Developer**: Code reviewed and tested
- [ ] **QA**: All test cases passed
- [ ] **DevOps**: Deployment scripts validated
- [ ] **Product Owner**: Features approved
- [ ] **Backup Verified**: Database backup confirmed

---

**Deployment Date:** _________________  
**Deployed By:** _________________  
**Version:** 1.1.0 (Live Session & Google Drive Support)
