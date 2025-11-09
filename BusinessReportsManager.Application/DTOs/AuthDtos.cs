namespace BusinessReportsManager.Application.DTOs;

public record LoginRequest(string Email, string Password);
public record LoginResponse(string Token, DateTime ExpiresAtUtc);