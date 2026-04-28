using Microsoft.Data.SqlClient;

var connStr = args.Length > 0
    ? args[0]
    : "Server=tcp:{DATABASE_SERVER};Initial Catalog={DATABASE_NAME};Persist Security Info=False;User ID={DATABASE_USER};Password={DATABASE_USER_PASSWORD};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

var titles = new[]
{
    "Senior Software Engineer", "Backend Developer", "Frontend Developer", "Full Stack Developer",
    "DevOps Engineer", "Data Engineer", "Machine Learning Engineer", "Cloud Architect",
    "Product Manager", "UX Designer", "QA Engineer", "Security Engineer",
    "Mobile Developer (iOS)", "Mobile Developer (Android)", "Platform Engineer",
    "Site Reliability Engineer", "Engineering Manager", "Principal Engineer",
    "Software Architect", "Staff Engineer"
};

var cities = new[]
{
    "Kuala Lumpur", "Singapore", "Sydney", "Melbourne", "London",
    "New York", "San Francisco", "Toronto", "Berlin", "Amsterdam",
    "Tokyo", "Seoul", "Bangalore", "Dubai", "Paris"
};

var countries = new[]
{
    "Malaysia", "Singapore", "Australia", "Australia", "United Kingdom",
    "United States", "United States", "Canada", "Germany", "Netherlands",
    "Japan", "South Korea", "India", "UAE", "France"
};

var descriptions = new[]
{
    "We are looking for a talented engineer to join our growing team. You will work on challenging problems at scale and collaborate with a passionate team.",
    "Join our world-class engineering team and build products used by millions. We value autonomy, ownership, and continuous learning.",
    "An exciting opportunity to work on cutting-edge technology. You will design and implement scalable solutions in a fast-paced environment.",
    "We are seeking a motivated professional to help us build the next generation of our platform. Strong fundamentals and a growth mindset are key.",
    "Help us scale our infrastructure and improve developer productivity. You will work closely with engineering and product teams to deliver high-quality software.",
};

using var conn = new SqlConnection(connStr);
await conn.OpenAsync();
Console.WriteLine("Connected to database.");

Guid employerId;
using (var cmd = new SqlCommand("SELECT TOP 1 id FROM [identity].user_profiles WHERE role = 'Employer'", conn))
{
    var result = await cmd.ExecuteScalarAsync();
    if (result is null || result == DBNull.Value)
    {
        Console.WriteLine("No employer found in identity.user_profiles. Please create an employer account first.");
        return;
    }
    employerId = (Guid)result;
    Console.WriteLine($"Using employer ID: {employerId}");
}

var statuses = new[] { "Draft", "Published", "Published", "Published", "Closed" };
var currencies = new[] { "USD", "USD", "MYR", "SGD", "GBP", "EUR", "AUD", "CAD" };
var rng = new Random(42);

const string insertSql = """
    INSERT INTO jobs.jobs (id, employer_id, title, description, city, country, salary_min, salary_max, salary_currency, status, created_at, published_at)
    VALUES (@id, @employerId, @title, @description, @city, @country, @salaryMin, @salaryMax, @currency, @status, @createdAt, @publishedAt)
    """;

int inserted = 0;
for (int i = 0; i < 100; i++)
{
    var titleBase = titles[i % titles.Length];
    var title = i < titles.Length ? titleBase : $"{titleBase} ({i / titles.Length + 1})";
    var cityIdx = i % cities.Length;
    var status = statuses[rng.Next(statuses.Length)];
    var createdAt = DateTime.UtcNow.AddDays(-rng.Next(0, 180));
    var publishedAt = status == "Draft" ? (DateTime?)null : createdAt.AddHours(rng.Next(1, 48));

    using var cmd = new SqlCommand(insertSql, conn);
    cmd.Parameters.AddWithValue("@id", Guid.NewGuid());
    cmd.Parameters.AddWithValue("@employerId", employerId);
    cmd.Parameters.AddWithValue("@title", title);
    cmd.Parameters.AddWithValue("@description", descriptions[i % descriptions.Length]);
    cmd.Parameters.AddWithValue("@city", cities[cityIdx]);
    cmd.Parameters.AddWithValue("@country", countries[cityIdx]);
    cmd.Parameters.AddWithValue("@salaryMin", (decimal)(rng.Next(5, 20) * 5000));
    cmd.Parameters.AddWithValue("@salaryMax", (decimal)(rng.Next(21, 30) * 5000));
    cmd.Parameters.AddWithValue("@currency", currencies[cityIdx % currencies.Length]);
    cmd.Parameters.AddWithValue("@status", status);
    cmd.Parameters.AddWithValue("@createdAt", createdAt);
    cmd.Parameters.AddWithValue("@publishedAt", publishedAt.HasValue ? (object)publishedAt.Value : DBNull.Value);

    await cmd.ExecuteNonQueryAsync();
    inserted++;
    Console.Write($"\rInserted {inserted}/100...");
}

Console.WriteLine($"\nDone. Seeded {inserted} jobs.");
