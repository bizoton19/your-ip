using System.Net;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
IPInfo GetIPInfo(HttpContext context)
{
    var remoteIpAddress = context.Connection.RemoteIpAddress;
    var forwardedIPHeader = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();

    var ipInfo = new IPInfo
    {
        DefaultIPAddress = remoteIpAddress?.ToString(),
        IPv4Address = remoteIpAddress?.MapToIPv4()?.ToString(),
        IPv6Address = remoteIpAddress?.MapToIPv6()?.ToString(),
        ForwardedIPAddress = forwardedIPHeader
    };

    return ipInfo;
}

app.MapGet("/ip", (HttpContext context) =>
{
    var ipInfo = GetIPInfo(context);
    return Results.Ok(ipInfo);
})
.WithName("GetIp")
.WithOpenApi();

app.Run();
record IPInfo
{
    public string DefaultIPAddress { get; init; } = string.Empty;
    public string IPv4Address { get; init; } = string.Empty;
    public string IPv6Address { get; init; } = string.Empty;
    public string ForwardedIPAddress { get; init; } = string.Empty;
}


