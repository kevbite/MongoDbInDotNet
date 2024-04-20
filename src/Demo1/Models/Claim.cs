namespace Demo1.Models;

public record Claim(
    Guid Id,
    decimal Amount,
    string Description
);