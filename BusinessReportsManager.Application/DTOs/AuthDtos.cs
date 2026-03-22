namespace BusinessReportsManager.Application.DTOs;

public record LoginRequest(string Username, string Password);
public record LoginResponse(string Token, DateTime ExpiresAtUtc, string Username);
public record RegisterRequest(string Username, string Email, string Password, string Role);
public record RegisterResponse(string UserId, string Email, string Username);