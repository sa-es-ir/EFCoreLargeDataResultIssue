﻿// See https://aka.ms/new-console-template for more information
using Domain;
using PostgresqlProvider;

Console.WriteLine("Hello, World!");

var db = new PostgresqlDbContext();

var textTable2MB = new TextTable2MB { Text = "TextTable2MB" };
db.TextTable2MB.Add(textTable2MB);

await db.SaveChangesAsync();

var t = await db.TextTable2MB.FindAsync(textTable2MB.Id);

Console.ReadLine();
