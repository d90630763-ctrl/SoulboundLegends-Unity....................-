CREATE EXTENSION IF NOT EXISTS pgcrypto;

CREATE TABLE IF NOT EXISTS "Accounts" (
    "Id" uuid PRIMARY KEY,
    "NormalizedEmail" varchar(320) NOT NULL UNIQUE,
    "DisplayEmail" varchar(320),
    "PasswordHash" text,
    "Status" varchar(32) NOT NULL,
    "CreatedAt" timestamptz NOT NULL,
    "LastLoginAt" timestamptz,
    "FailedLoginCount" integer NOT NULL,
    "LockoutUntil" timestamptz,
    "Roles" text NOT NULL
);

CREATE TABLE IF NOT EXISTS "ExternalIdentities" (
    "Id" uuid PRIMARY KEY,
    "AccountId" uuid NOT NULL REFERENCES "Accounts"("Id") ON DELETE CASCADE,
    "Provider" varchar(64) NOT NULL,
    "ProviderSubject" varchar(512) NOT NULL,
    "EmailAtProvider" varchar(320),
    "LinkedAt" timestamptz NOT NULL,
    "LastVerifiedAt" timestamptz NOT NULL,
    UNIQUE ("Provider","ProviderSubject")
);

CREATE TABLE IF NOT EXISTS "Sessions" (
    "Id" uuid PRIMARY KEY,
    "AccountId" uuid NOT NULL REFERENCES "Accounts"("Id") ON DELETE CASCADE,
    "DeviceId" varchar(256) NOT NULL,
    "IpAddress" varchar(128),
    "UserAgentHash" varchar(128),
    "CreatedAt" timestamptz NOT NULL,
    "LastActivityAt" timestamptz NOT NULL,
    "ExpiresAt" timestamptz NOT NULL,
    "ReconnectGraceUntil" timestamptz NOT NULL,
    "RevokedAt" timestamptz,
    "RevocationReason" varchar(256),
    "CurrentRefreshTokenHash" varchar(128) NOT NULL UNIQUE,
    "RefreshGeneration" bigint NOT NULL
);

CREATE TABLE IF NOT EXISTS "SecurityEvents" (
    "Id" bigserial PRIMARY KEY,
    "AccountId" uuid REFERENCES "Accounts"("Id") ON DELETE SET NULL,
    "Type" varchar(256) NOT NULL,
    "IpAddress" varchar(128),
    "MetadataJson" text,
    "CreatedAt" timestamptz NOT NULL
);

CREATE TABLE IF NOT EXISTS "RecoveryChallenges" (
    "Id" uuid PRIMARY KEY,
    "AccountId" uuid NOT NULL REFERENCES "Accounts"("Id") ON DELETE CASCADE,
    "TokenHash" varchar(128) NOT NULL UNIQUE,
    "ExpiresAt" timestamptz NOT NULL,
    "UsedAt" timestamptz,
    "CreatedAt" timestamptz NOT NULL
);

CREATE TABLE IF NOT EXISTS "PlayerIdentityLinks" (
    "Id" uuid PRIMARY KEY,
    "AccountId" uuid NOT NULL UNIQUE REFERENCES "Accounts"("Id") ON DELETE CASCADE,
    "PlayerId" varchar(256) NOT NULL UNIQUE,
    "CharacterId" varchar(256),
    "LinkedAt" timestamptz NOT NULL
);

CREATE INDEX IF NOT EXISTS "IX_Sessions_AccountId" ON "Sessions"("AccountId");
CREATE INDEX IF NOT EXISTS "IX_SecurityEvents_AccountId" ON "SecurityEvents"("AccountId");
CREATE INDEX IF NOT EXISTS "IX_SecurityEvents_CreatedAt" ON "SecurityEvents"("CreatedAt");
