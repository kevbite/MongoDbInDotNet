namespace Demo2.Models;

public record Policy(
    Guid Id,
    string PolicyNumber,
    string PolicyHolder,
    decimal Premium
)
{
    public IReadOnlyList<Claim> Claims { get; private set; } = new List<Claim>();
}