using ApiMultiPartFormData;
using System.Timers;
using System.Net.Http;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);


using HttpClient client = new();
client.DefaultRequestHeaders.Accept.Clear();
client.DefaultRequestHeaders.Accept.Add(
    new MediaTypeWithQualityHeaderValue("*/*"));
client.DefaultRequestHeaders.Add("User-Agent", "ReplayProcessor");

await ProcessRepositoriesAsync(client);

static async Task<HttpResponseMessage> ProcessRepositoriesAsync(HttpClient client)
{
    return client.PostAsync("http://localhost:8080/", null).GetAwaiter().GetResult();
}
// Add services to the container.
void run_py(string cmd)
{
    var strCmdText = "python " + cmd;
    System.Diagnostics.Process.Start("CMD.exe", strCmdText);
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
aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
aTimer.Start();
void OnTimedEvent(object source, ElapsedEventArgs e)
{
    if (lastHour < DateTime.Now.Hour || (lastHour == 23 && DateTime.Now.Hour == 0))
    {
        lastHour = DateTime.Now.Hour;
        
    }

}
run_py("AutomatedSkugsReplay.py");
builder.Services.AddAuthentication();
           
var app = builder.Build();



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
