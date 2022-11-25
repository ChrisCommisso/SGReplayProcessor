using ApiMultiPartFormData;


var builder = WebApplication.CreateBuilder(args);

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
