using Microsoft.AspNetCore.Mvc.Testing;

namespace TaskApi.Tests;

/// spins up TaskApi app in memory for testing.
public class TestWebAppFactory : WebApplicationFactory<Program> { }
