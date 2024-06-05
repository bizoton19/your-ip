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
app.UseStatusCodePages(async statusCodeContext 
    =>  await Results.Problem(statusCode: statusCodeContext.HttpContext.Response.StatusCode)
                 .ExecuteAsync(statusCodeContext.HttpContext));
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
    /*if(!context.Request.Headers.ContainsKey("X-Forwarded-For")){
      //throw new InvalidOperationException();
      var c = new Custom(){ Message="No X-Forwarded-For header present"};
      return Results.BadRequest<Custom>(c);
    }*/
    var ipInfo = GetIPInfo(context);
    Custom c1 = new Custom() {Message = ipInfo.ForwardedIPAddress};
    return Results.Ok<Custom>(c1);
})
.WithName("GetIp") 
.WithOpenApi();

app.Run();
record Custom 
{
   public  IPInfo iPInfo ;
   public string Message { get; init; } = string.Empty;


}
record IPInfo
{
    public string DefaultIPAddress { get; init; } = string.Empty;
    public string IPv4Address { get; init; } = string.Empty;
    public string IPv6Address { get; init; } = string.Empty;
    public string ForwardedIPAddress { get; init; } = string.Empty;
}


