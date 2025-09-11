var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing",
    "Bracing",
    "Chilly",
    "Cool",
    "Mild",
    "Warm",
    "Balmy",
    "Hot",
    "Sweltering",
    "Scorching",
};

var colors = new List<string> { "Purple", "Blue", "Black", "White", "Red" };

app.MapGet(
        "/weatherforecast",
        () =>
        {
            var forecast = Enumerable
                .Range(1, 5)
                .Select(index => new WeatherForecast(
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
                .ToArray();
            return forecast;
        }
    )
    .WithName("GetWeatherForecast");

//Calculator Challenge 1
app.MapGet(
    "/add/{a}/{b}",
    (int a, int b) =>
    {
        return new
        {
            operation = "add",
            inputa = a,
            inputb = b,
            sum = a + b,
        };
    }
);

app.MapGet(
    "/subtract/{a}/{b}",
    (int a, int b) =>
    {
        return new
        {
            operation = "subtract",
            inputa = a,
            inputb = b,
            difference = a - b,
        };
    }
);

app.MapGet(
    "/multiply/{a}/{b}",
    (int a, int b) =>
    {
        return new
        {
            operation = "multiply",
            inputa = a,
            inputb = b,
            product = a * b,
        };
    }
);

app.MapGet(
    "/divide/{a}/{b}",
    (double a, double b) =>
    {
        object result;

        if (b == 0)
        {
            result = new { operation = "divide", error = "Cannot divide by 0" };
        }
        else
        {
            result = new
            {
                operation = "divide",
                inputa = a,
                inputb = b,
                quotient = a / b,
            };
        }

        return result;
    }
);

//Challenge 2 String Manipulation
app.MapGet(
    "/text/reverse/{text}",
    (string text) =>
    {
        string reverse = "";
        for (int i = text.Length - 1; i >= 0; i--)
        {
            reverse += text[i];
        }

        return new { reverse };
    }
);

app.MapGet(
    "/text/uppercase/{text}",
    (string text) =>
    {
        return new { ouput = text.ToUpper() };
    }
);

app.MapGet(
    "/text/lowercase/{text}",
    (string text) =>
    {
        return new { output = text.ToLower() };
    }
);

app.MapGet(
    "/text/count/{text}",
    (string text) =>
    {
        int vowel = 0;
        for (int i = 0; i < text.Length; i++)
        {
            if ("aeiouy".Contains(text[i]))
            {
                vowel++;
            }
        }

        int words = text.Split(' ').Length;

        return new
        {
            characters = text.Length,
            vowelCount = vowel,
            wordCount = words,
        };
    }
);

app.MapGet(
    "/text/palindrome/{text}",
    (string text) =>
    {
        string reverse = "";
        for (int i = text.Length - 1; i >= 0; i--)
        {
            reverse += text[i];
        }

        if (reverse.ToLower() == text.ToLower())
        {
            return new { isPalindrome = true };
        }
        else
        {
            return new { isPalindrome = false };
        }
    }
);

//Challenge #3 Number Games
app.MapGet(
    "/numbers/fizzbuzz/{count}",
    (int count) =>
    {
        string fizzbuzz = "";
        int i = 1;
        while (i <= count)
        {
            if (i % 3 == 0 || i % 5 == 0)
            {
                if (i % 3 == 0)
                {
                    fizzbuzz += "Fizz";
                }
                if (i % 5 == 0 && i % 3 == 0)
                {
                    fizzbuzz += "Buzz ";
                }
                else if (i % 5 == 0)
                {
                    fizzbuzz += " Buzz ";
                }
            }
            else
            {
                fizzbuzz += " " + i + " ";
            }
            i++;
        }

        return new { Output = fizzbuzz };
    }
);

app.MapGet(
    "/numbers/prime/{count}",
    (int count) =>
    {
        string isPrime = "false";

        if (count <= 1)
        {
            return new { isPrime = "False" };
        }
        if (count == 2)
        {
            return new { isPrime = "True" };
        }
        if (count % 2 == 0)
        {
            return new { isPrime = "False" };
        }

        for (int i = 3; i <= Math.Sqrt(count); i += 2)
        {
            if (count % i == 0)
            {
                return new { isPrime = "False" };
            }
        }

        {
            return new { isPrime = "True" };
        }
    }
);

app.MapGet(
    "/numbers/fibonacci/{count}",
    (int count) =>
    {
        string Fibonacci = "";

        if (count == 0)
        {
            return new { Fibonacci = "0" };
        }
        if (count == 1)
        {
            return new { Fibonacci = "0 1" };
        }
        int temp1 = 0;
        int temp2 = 1;
        int newNum = 1;

        Fibonacci = "0 1 ";

        for (int i = 3; i <= count; i++)
        {
            newNum = temp1 + temp2;
            temp1 = temp2;
            temp2 = newNum;
            Fibonacci += newNum + " ";
        }

        {
            return new { Fibonacci };
        }
    }
);

app.MapGet(
    "/numbers/factors/{number}",
    (int number) =>
    {
        string Factors = "";

        for (int i = 1; i <= number; i++)
        {
            if (number % i == 0)
            {
                Factors += i + " ";
            }
        }

        {
            return new { Factors };
        }
    }
);

//Challenge #4

app.MapGet(
    "/date/today",
    () =>
    {
        string FinalOutput = "";
        List<DateTime> today = new List<DateTime>();
        var now = DateTimeOffset.Now;

        FinalOutput += now.ToString("O") + "\n";
        FinalOutput += now.ToString("mm-dd-yyyy") + "\n";
        FinalOutput += now.ToUniversalTime().ToString() + "\n";

        return FinalOutput;
    }
);

app.MapGet(
    "/date/age/{birthYear}",
    (string birthYear) =>
    {
        //string BirthYear = "";
        var now = DateTimeOffset.Now;
        {
            return $"You are likely {DateTime.UtcNow.Year - int.Parse(birthYear)} years old!";
        }
    }
);

app.MapGet(
    "/date/daysbetween/{date1}/{date2}",
    (DateOnly date1, DateOnly date2) =>
    {
        int daysBetween = Math.Abs(date2.DayNumber - date1.DayNumber);
        return daysBetween;
    }
);

app.MapGet(
    "/date/weekday/{date}",
    (DateOnly date) =>
    {
        return date.DayOfWeek.ToString();
    }
);

//Challenge #5
app.MapGet(
    "/colors",
    () =>
    {
        // var colors = new List<string> { "Purple", "Blue", "Black", "White", "Red" };
        return colors;
    }
);

app.MapGet(
    "/colors/random",
    () =>
    {
        // var colors = new List<string> { "Purple", "Blue", "Black", "White", "Red" };
        var index = Random.Shared.Next(colors.Count);

        return colors[index];
    }
);

app.MapGet(
    "/colors/search/{letter}",
    (string letter) =>
    {
        // var colors = new List<string> { "Purple", "Blue", "Black", "White", "Red" };

        var listOfColors = colors
            .Where(c =>
                !string.IsNullOrWhiteSpace(c)
                && !string.IsNullOrWhiteSpace(letter)
                && c.StartsWith(letter, StringComparison.OrdinalIgnoreCase)
            )
            .ToList();
        return listOfColors;
    }
);

app.MapPost(
    "/colors/add/{color}",
    (string color) =>
    {
        //colors.Add(color);

        if (string.IsNullOrWhiteSpace(color)) //Normalizes input
            color = color.Trim();
        colors.Add(char.ToUpperInvariant(color[0]) + color.Substring(1).ToLowerInvariant());
    }
);

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
