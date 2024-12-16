using Microsoft.AspNetCore.RateLimiting;

namespace Quickie.Base;

/// <summary>
/// Ultimate base of quickie controllers (APIs)
/// </summary>
[EnableRateLimiting("Quickie-Rl-Policy")]
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public abstract class QuickieBaseApi : ControllerBase;