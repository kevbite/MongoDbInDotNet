namespace Demo1.Models;

public record Policy
{
    public Guid Id { get; set; }
    public string PolicyNumber { get; set; }
    public string PolicyHolder { get; set; }
    public decimal Premium { get; set; }
    public List<Claim> Claims { get; set; } = new();
};