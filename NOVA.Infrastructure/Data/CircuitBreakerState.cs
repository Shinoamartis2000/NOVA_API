namespace NOVA.Core.Models
{
    public enum CircuitBreakerState
    {
        Closed,     // Normal operation
        Open,       // Blocked due to failures
        HalfOpen    // Testing if service recovered
    }
}