using TXTextControl.Web;
using TXTextControl.Web.MVC.DocumentViewer;

var builder = WebApplication.CreateBuilder(args);

// adding CORS policy to allow all origins
builder.Services.AddCors(options =>
{
	options.AddDefaultPolicy(
			   builder =>
			   {
				   builder.AllowAnyOrigin()
						   .AllowAnyMethod()
						   .AllowAnyHeader();
			   });
});

// adding controllers for DocumentViewer Web API
builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();
app.MapGet("/", () => "TX Text Control .NET Server for ASP.NET Backend is up and running!");

app.UseRouting();

// adding CORS middleware
app.UseCors();
app.UseTXDocumentViewer();
app.MapControllers();

app.Run();