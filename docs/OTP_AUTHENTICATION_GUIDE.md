# Authentication Flow with OTP Integration

This document explains how to use the enhanced authentication system with conditional OTP (One-Time Password) support.

## Overview

The authentication system now supports conditional OTP verification based on security policies. OTP is required when:

1. **Admin users** - Always require OTP for elevated security
2. **Suspicious activity** - New IP addresses, unusual patterns
3. **Random security checks** - 10% of logins (configurable)
4. **Time-based policies** - Different rules during business hours

## Authentication Flow

### 1. Standard Login (No OTP Required)

**Request:**
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "userpassword"
}
```

**Success Response (200):**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresAt": "2025-09-09T13:00:00Z"
}
```

**Note:** The refresh token is automatically set as an HttpOnly secure cookie.

### 2. Login with OTP Required

**Request:**
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "admin@example.com",
  "password": "adminpassword"
}
```

**OTP Required Response (428 Precondition Required):**
```json
{
  "isSuccess": false,
  "error": {
    "code": "auth.otp_required",
    "message": "Verificação OTP necessária. Um código foi enviado para seu e-mail.",
    "type": "Validation",
    "statusCode": 428,
    "details": {
      "requiresOtp": true,
      "email": "admin@example.com"
    }
  }
}
```

### 3. OTP Verification

**Request:**
```http
POST /api/auth/verify-otp
Content-Type: application/json

{
  "email": "admin@example.com",
  "otpCode": "123456"
}
```

**Success Response (200):**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresAt": "2025-09-09T13:00:00Z"
}
```

**Invalid OTP Response (401):**
```json
{
  "isSuccess": false,
  "error": {
    "code": "auth.unauthorized",
    "message": "Código OTP inválido ou expirado.",
    "type": "Unauthorized",
    "statusCode": 401
  }
}
```

## Client Implementation Example

```javascript
class AuthService {
  async login(email, password) {
    const response = await fetch('/api/auth/login', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ email, password })
    });

    const data = await response.json();

    if (response.status === 428) {
      // OTP required
      return {
        requiresOtp: true,
        email: data.error.details.email,
        message: data.error.message
      };
    }

    if (response.ok) {
      // Store access token
      localStorage.setItem('accessToken', data.accessToken);
      return { success: true, accessToken: data.accessToken };
    }

    throw new Error(data.error.message);
  }

  async verifyOtp(email, otpCode) {
    const response = await fetch('/api/auth/verify-otp', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ email, otpCode })
    });

    const data = await response.json();

    if (response.ok) {
      localStorage.setItem('accessToken', data.accessToken);
      return { success: true, accessToken: data.accessToken };
    }

    throw new Error(data.error.message);
  }
}

// Usage example
const authService = new AuthService();

try {
  const result = await authService.login('user@example.com', 'password');
  
  if (result.requiresOtp) {
    // Show OTP input form
    const otpCode = await showOtpForm(result.message);
    const otpResult = await authService.verifyOtp(result.email, otpCode);
    console.log('Login successful with OTP');
  } else {
    console.log('Login successful without OTP');
  }
} catch (error) {
  console.error('Login failed:', error.message);
}
```

## Configuration

### OTP Settings (appsettings.json)
```json
{
  "Security": {
    "Otp": {
      "ExpirationSeconds": 300
    }
  }
}
```

### Email Settings (appsettings.json)
```json
{
  "Email": {
    "FromEmail": "no-reply@yourcompany.com",
    "FromName": "Your Company",
    "SmtpHost": "smtp.yourprovider.com",
    "SmtpPort": 587,
    "SmtpUsername": "your-username",
    "SmtpPassword": "your-password"
  }
}
```

## Customizing OTP Requirements

You can customize when OTP is required by modifying the `AuthenticationContextService.EvaluateOtpRequirement` method:

```csharp
private Task<bool> EvaluateOtpRequirement(User user, AuthenticationClientInfo? clientInfo)
{
    // Always require OTP for Admin users
    if (user.Role?.Name == "Admin")
        return Task.FromResult(true);

    // Require OTP for users logging in from new locations
    if (IsNewLocation(clientInfo?.IpAddress))
        return Task.FromResult(true);

    // Custom business logic
    if (user.LastLoginDate < DateTime.UtcNow.AddDays(-30))
        return Task.FromResult(true); // First login in 30 days

    return Task.FromResult(false);
}
```

## Security Best Practices

1. **Rate Limiting**: Implement rate limiting on OTP endpoints
2. **IP Tracking**: Track and analyze login patterns
3. **Session Management**: Proper refresh token rotation
4. **Audit Logging**: Log all authentication events
5. **OTP Delivery**: Use multiple channels (SMS, Email, App)

## Error Handling

All authentication endpoints return structured error responses:

- **400 Bad Request**: Invalid input data
- **401 Unauthorized**: Invalid credentials or expired OTP
- **403 Forbidden**: Account not approved
- **428 Precondition Required**: OTP verification needed
- **500 Internal Server Error**: Server-side issues

## Testing

Use the following test scenarios:

1. **Normal user login** (should not require OTP)
2. **Admin user login** (should require OTP)
3. **Invalid OTP code** (should fail)
4. **Expired OTP code** (should fail)
5. **Multiple OTP attempts** (should handle properly)
