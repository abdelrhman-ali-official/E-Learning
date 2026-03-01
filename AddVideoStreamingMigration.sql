-- Add Video Streaming columns to Contents table
ALTER TABLE Contents ADD VideoObjectKey NVARCHAR(500) NULL;
ALTER TABLE Contents ADD LiveStreamUrl NVARCHAR(500) NULL;

-- Create WatchProgress table
CREATE TABLE WatchProgress (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UserId NVARCHAR(450) NOT NULL,
    ContentId INT NOT NULL,
    LastPositionSeconds INT NOT NULL,
    IsCompleted BIT NOT NULL DEFAULT 0,
    LastUpdated DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_WatchProgress_Users FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE,
    CONSTRAINT FK_WatchProgress_Contents FOREIGN KEY (ContentId) REFERENCES Contents(Id) ON DELETE CASCADE
);

-- Create unique index
CREATE UNIQUE INDEX IX_WatchProgress_UserId_ContentId ON WatchProgress(UserId, ContentId);

-- Verify changes
SELECT 'Contents columns added' AS Step;
SELECT 'WatchProgress table created' AS Step;
