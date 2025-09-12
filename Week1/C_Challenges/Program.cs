using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization.Infrastructure;

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

var colors = new List<string> { "Purple", "Blue", "Black", "White", "Red" }; //challenge 5

string SimpleChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789"; //challenge 7
string ComplexChars =
    @"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!""#$%&'()*+,-./:;<=>?@[\]^_`{|}~";
; //challenege 7.2
string[] WordList = new[]
{
    "apple",
    "river",
    "stone",
    "cloud",
    "maple",
    "echo",
    "frost",
    "amber",
    "cedar",
    "comet",
    "delta",
    "pixel",
    "quartz",
    "raven",
    "solar",
    "tulip",
    "ocean",
    "ember",
    "violet",
    "wolf",
}; //challenge 7.3

string RandomFromAlphabet(int len, ReadOnlySpan<char> alphabet) //Challenge 7 helper function
{
    var output = new char[len];
    for (int i = 0; i < len; i++)
        output[i] = alphabet[RandomNumberGenerator.GetInt32(alphabet.Length)];
    return new string(output);
}

string Memorable(int words, char sep = '-') //Challenge 7.3 helper function
{
    var parts = new string[words];
    for (int i = 0; i < words; i++)
        parts[i] = WordList[RandomNumberGenerator.GetInt32(WordList.Length)];
    return string.Join(sep, parts);
}

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
    /* string reverse = ""; O(N^2) yikes
     for (int i = text.Length - 1; i >= 0; i--)
     {
         reverse += text[i];
     }

     return new { reverse }; */

    {
        var chars = text.ToCharArray();
        Array.Reverse(chars);
        return new { reverse = new string(chars) };
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

        int words = text.Trim().Split(' ').Length;

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
        colors.Add(char.ToUpper(color[0]) + color.Substring(1).ToLower());
    }
);

//Challenge 6 Temp Converter

app.MapGet(
    "/temp/celsius-to-fahrenheit/{temp}",
    (double temp) =>
    {
        double f;

        f = (temp * 9.0 / 5.0) + 32;

        return f;
    }
);

app.MapGet(
    "/temp/fahrenheit-to-celsius/{temp}",
    (double temp) =>
    {
        double c;

        c = (temp - 32) * 9.0 / 5.0;

        return c;
    }
);

app.MapGet(
    "/temp/kelvin-to-celsius/{temp}",
    (double temp) =>
    {
        double k;

        k = temp + 273.15;

        return k;
    }
);

app.MapGet(
    "/temp/compare/{temp1}/{unit1}/{temp2}/{unit2}",
    (double temp1, string type1, double temp2, string type2) =>
    {
        char u1 = char.ToUpper(type1.Trim()[0]);
        char u2 = char.ToUpper(type2.Trim()[0]);
        if (type1 == type2)
        {
            return $"The difference between the first temp minus the second temp is about {temp1 - temp2}";
        }

        double temp2InUnit1;

        if (u1 == 'C')
        {
            if (u2 == 'C')
                temp2InUnit1 = temp2;
            else if (u2 == 'F')
                temp2InUnit1 = (temp2 - 32.0) * 5.0 / 9.0;
            else
                temp2InUnit1 = temp2 - 273.15;

            double difference = temp1 - temp2InUnit1; // signed diff remember to abs later
            string relation =
                difference == 0 ? "equal to"
                : difference > 0 ? "hotter than"
                : "colder than";
            return $"{temp1:F2} C is {relation} {temp2:F2} {u2} by {Math.Abs(difference):F2} C";
        }
        else if (u1 == 'F')
        {
            if (u2 == 'C')
                temp2InUnit1 = (temp2 * 9.0 / 5.0) + 32.0;
            else if (u2 == 'F')
                temp2InUnit1 = temp2;
            else
                temp2InUnit1 = (temp2 - 273.15) * 9.0 / 5.0 + 32.0;

            double difference = temp1 - temp2InUnit1;
            string relation =
                difference == 0 ? "equal to"
                : difference > 0 ? "hotter than"
                : "colder than";
            return $"{temp1:F2} F is {relation} {temp2:F2} {u2} by {Math.Abs(difference):F2} F";
        }
        else
        {
            if (u2 == 'C')
                temp2InUnit1 = temp2 + 273.15;
            else if (u2 == 'F')
                temp2InUnit1 = (temp2 - 32.0) * 5.0 / 9.0 + 273.15;
            else
                temp2InUnit1 = temp2;

            double difference = temp1 - temp2InUnit1;
            string relation =
                difference == 0 ? "equal to"
                : difference > 0 ? "hotter than"
                : "colder than";
            return $"{temp1:F2} K is {relation} {temp2:F2} {u2} by {Math.Abs(difference):F2} K";
        }
    }
);

//Challenge #7

app.MapGet("/password/simple/{length}", (int length) => RandomFromAlphabet(length, SimpleChars));

app.MapGet("/password/complex/{length}", (int length) => RandomFromAlphabet(length, ComplexChars));

app.MapGet("/password/memorable/{words}", (int words) => Memorable(words));

app.MapGet(
    "/password/strength/{password}",
    (string password) =>
    {
        if (string.IsNullOrEmpty(password))
            return "weak (score 0, length 0)";

        int score = 0;
        if (password.Length >= 8)
            score++;
        if (password.Length >= 12)
            score++;

        bool lower = false,
            upper = false,
            digit = false,
            symbol = false;
        foreach (char c in password)
        {
            if (char.IsLower(c))
                lower = true;
            else if (char.IsUpper(c))
                upper = true;
            else if (char.IsDigit(c))
                digit = true;
            else
                symbol = true;
        }
        score += (lower ? 1 : 0) + (upper ? 1 : 0) + (digit ? 1 : 0) + (symbol ? 1 : 0);

        string strength =
            score <= 2 ? "weak"
            : score <= 4 ? "fair"
            : score <= 6 ? "strong"
            : "very-strong";

        return $"{strength} (score {score}, length {password.Length})";
    }
);

//Challenge 8
app.MapGet(
    "/validate/email/{email}",
    (string email) =>
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;
        const string pattern = @"^[^\s@]+@[^\s@]+\.[^\s@]+$";
        return Regex.IsMatch(email, pattern);
    }
);

app.MapGet(
    "/validate/phone/{phone}",
    (string phone) =>
    {
        if (string.IsNullOrWhiteSpace(phone))
            return false;

        const string pattern = @"^(?:\+?\d{3}[-. ]?)?(?:\(\d{3}\)|\d{3})[-. ]?\d{3}[-. ]?\d{4}$"; //checks for 10 digit number, I tried to make it optionally check area code currently
        return Regex.IsMatch(phone, pattern);
    }
);

app.MapGet(
    "/validate/creditcard/{credit}",
    (string credit) =>
    {
        if (string.IsNullOrWhiteSpace(credit))
            return false;

        int sum = 0;
        bool doubleIt = false;
        int digitCount = 0;

        for (int i = credit.Length - 1; i >= 0; i--)
        {
            char ch = credit[i];
            if (ch == ' ' || ch == '-')
                continue; // allow separators

            if (ch < '0' || ch > '9')
                return false; // invalid char
            int d = ch - '0';

            if (doubleIt)
            {
                d *= 2;
                if (d > 9)
                    d -= 9;
            }

            sum += d;
            doubleIt = !doubleIt;
            digitCount++;
        }

        return sum % 10 == 0;
    }
);

app.MapGet(
    "/validate/strongpassword/{password}",
    (string password) =>
    {
        if (string.IsNullOrWhiteSpace(password))
            return false;

        const string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9])(?=\S+$).{12,}$"; //this regex checks 1 capital 1 lowercase 1 special 1 number and length >= 12
        return Regex.IsMatch(password, pattern);
    }
);

//Challenge 9
app.MapGet(
    "/convert/length/{value}/{fromUnit}/{toUnit}",
    (double value, string fromUnit, string toUnit) =>
    {
        if (fromUnit == toUnit)
            return (object)value;

        // meters
        if (fromUnit == "meters")
        {
            if (toUnit == "feet")
                return (object)(value * 3.281);
            if (toUnit == "inches")
                return (object)(value * 39.37);
            return "Invalid toUnit. From 'meters' you can convert to: feet, inches. Valid units: meters, feet, inches.";
        }

        // feet
        if (fromUnit == "feet")
        {
            if (toUnit == "meters")
                return (object)(value / 3.281);
            if (toUnit == "inches")
                return (object)(value * 12);
            return "Invalid toUnit. From 'feet' you can convert to: meters, inches. Valid units: meters, feet, inches.";
        }

        // inches
        if (fromUnit == "inches")
        {
            if (toUnit == "meters")
                return (object)(value / 39.37);
            if (toUnit == "feet")
                return (object)(value / 12);
            return "Invalid toUnit. From 'inches' you can convert to: meters, feet. Valid units: meters, feet, inches.";
        }

        return "Invalid fromUnit. Valid units: meters, feet, inches.";
    }
);

app.MapGet(
    "/convert/weight/{value}/{fromUnit}/{toUnit}",
    (double value, string fromUnit, string toUnit) =>
    {
        if (fromUnit == toUnit)
            return (object)value;
        //kg
        if (fromUnit == "kg")
        {
            if (toUnit == "lbs")
                return (object)(value * 2.20462);
            if (toUnit == "ounces")
                return (object)(value * 35.274);
            return "Invalid toUnit. From 'kg' you can convert to: lbs, ounces. Valid units: kg, lbs, ounces.";
        }
        //lbs
        if (fromUnit == "lbs")
        {
            if (toUnit == "kg")
                return (object)(value / 2.20462);
            if (toUnit == "ounces")
                return (object)(value * 16);
            return "Invalid toUnit. From 'lbs' you can convert to: kg, ounces. Valid units: kg, lbs, ounces.";
        }
        //ounce
        if (fromUnit == "ounces")
        {
            if (toUnit == "kg")
                return (object)(value / 35.274);
            if (toUnit == "lbs")
                return (object)(value / 16);
            return "Invalid toUnit. From 'ounces' you can convert to: kg, lbs. Valid units: kg, lbs, ounces.";
        }

        return "Invalid fromUnit. Valid units: kg, lbs, ounces.";
    }
);

app.MapGet(
    "/convert/volume/{value}/{fromUnit}/{toUnit}",
    (double value, string fromUnit, string toUnit) =>
    {
        if (fromUnit == toUnit)
            return (object)value;

        // liters
        if (fromUnit == "liters")
        {
            if (toUnit == "gallons")
                return (object)(value * 0.264172);
            if (toUnit == "cups")
                return (object)(value * 4.227);
            return "Invalid toUnit. From 'liters' you can convert to: gallons, cups. Valid units: liters, gallons, cups.";
        }

        // gallons
        if (fromUnit == "gallons")
        {
            if (toUnit == "liters")
                return (object)(value * 3.785);
            if (toUnit == "cups")
                return (object)(value * 16);
            return "Invalid toUnit. From 'gallons' you can convert to: liters, cups. Valid units: liters, gallons, cups.";
        }

        // cups
        if (fromUnit == "cups")
        {
            if (toUnit == "liters")
                return (object)(value / 4.227);
            if (toUnit == "gallons")
                return (object)(value / 16);
            return "Invalid toUnit. From 'cups' you can convert to: liters, gallons. Valid units: liters, gallons, cups.";
        }

        return "Invalid fromUnit. Valid units: liters, gallons, cups.";
    }
);

app.MapGet(
    "/convert/list-units/{type}",
    (string type) =>
    {
        type = type.ToLower();

        if (type == "length")
            return "meters,feet,inches";
        if (type == "weight")
            return "kg,lbs,ounces";
        if (type == "volume")
            return "liters,gallons,cups";

        return "supported types: length,weight,volume";
    }
);

//Challenege 10
var forecasts = new List<WeatherForecast>();

// GET
app.MapGet("/forecast", () => forecasts.ToArray());

// POST
app.MapPost(
    "/weatherforecast",
    (WeatherForecast forecast) =>
    {
        forecasts.Add(forecast);
        return forecast;
    }
/* example weather forecast json
{
"date": "2025-09-12",
"temperatureC": 22,
"summary": "Mild"
}
*/
);

// DELETE
app.MapDelete(
    "/forecast/{date}",
    (DateOnly date) =>
    {
        int removed = forecasts.RemoveAll(f => f.Date == date);
        return removed;
    }
);

//Challenege 11

var sessions = new Dictionary<int, int>(); //<sessionId, guess>
int nextId = 0; // starting with 0 since I do ++nextId when giving out session ids

app.MapPost(
    "/game/start",
    () =>
    {
        var id = ++nextId;
        var secret = Random.Shared.Next(1, 101); // 1-100 inclusive
        sessions[id] = secret;

        return new { sessionId = id, message = "Game started! Guess a number 1-100." };
    }
);

app.MapPost(
    "/game/guess-number",
    (GuessIn g) =>
    {
        if (!sessions.TryGetValue(g.sessionId, out var answer))
            return $"Session {g.sessionId} not found. Start a new game.";

        if (g.guess == answer)
        {
            sessions.Remove(g.sessionId); // end
            return $"Correct! Session {g.sessionId} ended.";
        }

        return g.guess < answer ? "Guess Higher" : "Guess Lower";
    }
);

app.MapGet(
    "/game/rock-paper-scissors/{choice}",
    (string choice) =>
    {
        var opts = new[] { "rock", "paper", "scissors" };
        var user = choice.ToLower();
        if (Array.IndexOf(opts, user) < 0) //validation
            return "Choose rock, paper, or scissors.";

        var cpu = opts[Random.Shared.Next(3)];
        if (user == cpu)
            return $"Tie! ({cpu})";
        bool userWins =
            (user == "rock" && cpu == "scissors")
            || (user == "paper" && cpu == "rock")
            || (user == "scissors" && cpu == "paper");
        return userWins ? $"You win! (cpu: {cpu})" : $"You lose! (cpu: {cpu})";
    }
);

app.MapGet(
    "/game/dice/{sides}/{count}",
    (int sides, int count) =>
    {
        if (sides < 2 || count < 1)
            return [];
        var rolls = new int[count];
        for (int i = 0; i < count; i++)
            rolls[i] = Random.Shared.Next(1, sides + 1);
        return rolls;
    }
);

app.MapGet(
    "/game/coin-flip/{count}",
    (int count) =>
    {
        if (count < 1)
            return [];
        var flips = new string[count];
        for (int i = 0; i < count; i++)
            flips[i] = Random.Shared.Next(2) == 0 ? "H" : "T";
        return flips;
    }
);

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

record GuessIn(int sessionId, int guess);
