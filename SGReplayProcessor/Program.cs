using ApiMultiPartFormData;
using System.Timers;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);


using HttpClient client = new();
client.DefaultRequestHeaders.Accept.Clear();
client.DefaultRequestHeaders.Accept.Add(
    new MediaTypeWithQualityHeaderValue("*/*"));
client.DefaultRequestHeaders.Add("User-Agent", "ReplayProcessor");

static async Task<HttpResponseMessage> ProcessReplaysAsync(HttpClient client)
{
    return client.PostAsync("http://localhost:8080/", null).GetAwaiter().GetResult();
}
// Add services to the container.
void run_py(string cmd)
{
    var strCmdText = "/k python " + cmd;
    var psi = new ProcessStartInfo("cmd.exe");
    psi.UseShellExecute = true;
    psi.Arguments = strCmdText;
    Process.Start(psi);
}

builder.Services.AddControllers(options =>
{
    //..
    options.InputFormatters.Add(new MultipartFormDataFormatter());
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var aTimer = new System.Timers.Timer(1000); //One second, (use less to add precision, use more to consume less processor time
int lastHour = DateTime.Now.Hour;
aTimer.Elapsed += new ElapsedEventHandler((object o,ElapsedEventArgs t) => {
    Task.Run(() =>
    {
        OnTimedEventAsync(o, t);
    });
    
});
aTimer.Start();
void OnTimedEventAsync(object source, ElapsedEventArgs e)
{
    if (lastHour < DateTime.Now.Hour || (lastHour == 23 && DateTime.Now.Hour == 0))
    {
        lastHour = DateTime.Now.Hour;
        ProcessReplaysAsync(client).RunSynchronously();
    }

}
run_py(@"AutomatedSkugsReplay.py");
builder.Services.AddAuthentication();
           
var app = builder.Build();


Task.Run(async () => {
    Thread.Sleep(45000);
    await ProcessReplaysAsync(client);
});
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.UseAuthorization();



app.MapControllers();

app.Run();
