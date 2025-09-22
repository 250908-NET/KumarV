-- SETUP:
-- Create a database server (docker)
-- docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=<password>" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest
-- Connect to the server (Azure Data Studio / Database extension)
-- Test your connection with a simple query (like a select)
-- Execute the Chinook database (to create Chinook resources in your db)

-- On the Chinook DB, practice writing queries with the following exercises
/*SELECT DB_NAME() AS CurrentDB;
SELECT TOP (10) * FROM [MyDatabase].[dbo].[Album]; */
-- BASIC CHALLENGES
-- List all customers (full name, customer id, and country) who are not in the USA
SELECT * FROM dbo.Customer WHERE Country!='USA';

-- List all customers from Brazil
SELECT * FROM dbo.Customer WHERE Country='Brazil';

-- List all sales agents
SELECT * FROM dbo.Employee WHERE Title LIKE '%Sales%';

-- Retrieve a list of all countries in billing addresses on invoices
SELECT DISTINCT BillingCountry FROM dbo.Invoice; 

-- Retrieve how many invoices there were in 2009, and what was the sales total for that year?
SELECT COUNT(*), SUM(Total) FROM dbo.Invoice WHERE Year(InvoiceDate) = 2009;



-- (challenge: find the invoice count sales total for every year using one query)
SELECT Year(InvoiceDate), COUNT(*), SUM(Total) FROM dbo.Invoice GROUP BY Year(InvoiceDate);


-- how many line items were there for invoice #37
SELECT COUNT(InvoiceLineId) FROM dbo.InvoiceLine WHERE InvoiceId = 37;

-- how many invoices per country? BillingCountry  # of invoices -
SELECT BillingCountry, COUNT(InvoiceId) FROM dbo.Invoice GROUP BY BillingCountry;


-- Retrieve the total sales per country, ordered by the highest total sales first.
SELECT BillingCountry, SUM(Total) FROM dbo.Invoice GROUP BY BillingCountry ORDER BY SUM(Total) DESC;

-- JOINS CHALLENGES
-- Every Album by Artist
SELECT Album.Title, Artist.Name FROM Album INNER JOIN Artist ON Album.ArtistId = Artist.ArtistId;

-- All songs of the rock genre
SELECT Track.Name, Genre.Name FROM Track INNER JOIN Genre ON Track.GenreId = Genre.GenreId WHERE Genre.Name = 'Rock';

-- Show all invoices of customers from brazil (mailing address not billing)
SELECT * FROM Invoice INNER JOIN Customer ON Invoice.CustomerId = Customer.CustomerId WHERE Customer.Country = 'Brazil';

-- Show all invoices together with the name of the sales agent for each one
SELECT Employee.FirstName as 'Sales Agent', *  FROM Invoice INNER JOIN Customer ON Invoice.CustomerId = Customer.CustomerId INNER JOIN Employee ON Customer.SupportRepId = Employee.EmployeeId WHERE Employee.Title LIKE '%Sales%' ORDER BY Employee.FirstName;

-- Which sales agent made the most sales in 2009?
Select TOP 1 Employee.FirstName as 'Sales Agent', SUM(Invoice.Total) FROM Invoice INNER JOIN Customer ON Invoice.CustomerId = Customer.CustomerId INNER JOIN Employee ON Customer.SupportRepId = Employee.EmployeeId WHERE Employee.Title LIKE '%Sales%' AND Year(Invoice.InvoiceDate) = 2009 GROUP BY Employee.FirstName ORDER BY SUM(Invoice.Total) DESC;

-- How many customers are assigned to each sales agent?
Select Employee.FirstName + ' ' + Employee.LastName as 'Sales Agent', COUNT(CustomerId) as 'Customers' FROM Customer INNER JOIN Employee ON Customer.SupportRepId = Employee.EmployeeId GROUP BY Employee.FirstName, Employee.LastName; 

-- Which track was purchased the most in 2010?
Select TOP 1 Track.Name, SUM(InvoiceLine.Quantity) as 'Copies Sold' FROM Track INNER JOIN InvoiceLine ON Track.TrackId = InvoiceLine.TrackId INNER JOIN Invoice ON InvoiceLine.InvoiceId = Invoice.InvoiceId WHERE Track.TrackId = InvoiceLine.TrackId AND YEAR(Invoice.InvoiceDate) = 2010 GROUP BY Track.Name Order BY SUM(InvoiceLine.Quantity) DESC;

-- Show the top three best selling artists.
SELECT TOP 3 Artist.Name, SUM(InvoiceLine.Quantity) as 'Quantity' FROM Artist Inner join Album ON Artist.ArtistId = Album.ArtistId Inner Join Track ON Album.AlbumId = Track.AlbumId Inner Join InvoiceLine ON Track.TrackId = InvoiceLine.TrackId GROUP BY Artist.Name Order BY SUM(InvoiceLine.Quantity) DESC;

-- Which customers have the same initials as at least one other customer?



-- ADVACED CHALLENGES
-- solve these with a mixture of joins, subqueries, CTE, and set operators.
-- solve at least one of them in two different ways, and see if the execution
-- plan for them is the same, or different.

-- 1. which artists did not make any albums at all?

-- 2. which artists did not record any tracks of the Latin genre?

-- 3. which video track has the longest length? (use media type table)

-- 4. find the names of the customers who live in the same city as the
--    boss employee (the one who reports to nobody)

-- 5. how many audio tracks were bought by German customers, and what was
--    the total price paid for them?

-- 6. list the names and countries of the customers supported by an employee
--    who was hired younger than 35.

-- DML exercises

-- 1. insert two new records into the employee table.

-- 2. insert two new records into the tracks table.

-- 3. update customer Aaron Mitchell's name to Robert Walter

-- 4. delete one of the employees you inserted.

-- 5. delete customer Robert Walter.

/*SELECT * FROM dbo.spt_monitor;
GO*/