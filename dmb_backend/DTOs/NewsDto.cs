namespace dmb_backend.DTOs;

public record NewsDto(int Id, string Title, string Text, DateTime CreatedAtUtc);