using var db = new EdgarContext();

// Note: This sample requires the database to be created before running.
Console.WriteLine($"Database path: {db.DbPath}.");

// Create
Console.WriteLine("Inserting a new blog");
db.Add(new EdgarCompanyInfo
{
    Cik = 4,
    EntityName = "Testing",
    Frame = "",
    Value = "Test Value"
});
db.SaveChanges();

// Read
Console.WriteLine("Querying for a blog");
var blog = db.EdgarCompanies
    .OrderBy(b => b.Cik)
    .First();
Console.WriteLine(blog.Value);

// Delete
Console.WriteLine("Delete the blog");
db.Remove(blog);
db.SaveChanges();

Console.ReadLine();