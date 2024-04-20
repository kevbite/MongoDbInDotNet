namespace Demo2.Models;

public record Claim(
    Guid Id,
    decimal Amount,
    string Description
);