namespace buildwave.Entities;

public class UserSession
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string SessionToken { get; set; }
        = string.Empty;

    public string IpAddress { get; set; }
        = string.Empty;

    public string UserAgent { get; set; }
        = string.Empty;

    public DateTime CreatedAt { get; set; }
        = DateTime.UtcNow;

    public DateTime ExpiresAt { get; set; }

    public bool IsRevoked { get; set; }
        = false;

    public User? User { get; set; }
}