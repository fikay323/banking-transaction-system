namespace practice.Application.DTOs;

public record TransferRequestDto(string SourceAccountNumber, string DestinationAccountNumber, decimal Amount);
