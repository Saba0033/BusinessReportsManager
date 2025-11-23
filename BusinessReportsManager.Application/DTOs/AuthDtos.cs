namespace BusinessReportsManager.Application.DTOs;

public record LoginRequest(string Email, string Password);
public record LoginResponse(string Token, DateTime ExpiresAtUtc);
public record RegisterRequest(string FullName, string Email, string Password);
public record RegisterResponse(string UserId, string Email, string FullName);