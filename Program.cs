using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace EFTest
{
    using HandlerAction = Action<MyDbContext>;
    using HandlerFunc = Func<MyDbContext, bool>;

    class Program
    {
        static void Main(string[] args)
        {
            var table = new Dictionary<Char, HandlerFunc>() {
                { 'l', NoQuit(ListRecords)},
                { 'c', NoQuit(CreateRecord)},
                { 'd', NoQuit(DeleteRecord)},
                { 'u', NoQuit(UpdateRecord)},
                { 'q', _ => true},
            };

            for (;;)
            {
                Console.WriteLine();
                Console.WriteLine("Enter a command:");
                Console.WriteLine();
                Console.WriteLine("l: list records");
                Console.WriteLine("c: create record");
                Console.WriteLine("d: delete record");
                Console.WriteLine("u: update record");
                Console.WriteLine("q: quit");
                Console.WriteLine();
                Console.Write("> ");

                var input = Console.ReadLine();
                var command = input.FirstOrDefault();
                HandlerFunc handler;
                if (!table.TryGetValue(command, out handler))
                {
                    Console.Error.WriteLine($"Unknown command, '{command}'.");
                    continue;
                }

                using (var db = new MyDbContext())
                {
                    if (handler(db))
                        return;
                }
            }
        }

        static void ListRecords(MyDbContext db)
        {
            foreach (var thing in db.Things.AsNoTracking())
                DumpRecord(thing);
        }

        static void CreateRecord(MyDbContext db)
        {
            var record = new Thing
            {
                FirstName = PromptFor("First name", string.Empty),
                LastName = PromptFor("Last name", string.Empty),
                Email = PromptFor("Email", string.Empty)
            };

            db.Things.Add(record);
            db.SaveChanges();
            DumpRecord(record);
        }

        static void DeleteRecord(MyDbContext db)
        {
            Lookup("Enter the id of the record to delete: ", db, record =>
            {
                db.Things.Remove(record);
                db.SaveChanges();
            });
        }

        static void UpdateRecord(MyDbContext db)
        {
            Lookup("Enter the id of the record to update: ", db, record =>
            {
                record.FirstName = PromptFor("First name", record.FirstName);
                record.LastName = PromptFor("Last name", record.LastName);
                record.Email = PromptFor("Email", record.Email);

                db.SaveChanges();
                DumpRecord(record);
            });
        }

        static HandlerFunc NoQuit(HandlerAction handler)
        {
            return db =>
            {
                handler(db);
                return false;
            };
        }

        static void Lookup(string prompt, MyDbContext db, Action<Thing> handler)
        {
            Console.Write(prompt);
            var input = Console.ReadLine();

            int id;
            if (!int.TryParse(input, out id))
            {
                Console.Error.WriteLine($"Invalid input, '{input}'.");
                return;
            }

            var record = db.Things.Find(id);
            if (record == null)
            {
                Console.Error.WriteLine($"Failed to find a record with id, '{id}'.");
                return;
            }

            handler(record);
        }

        static string PromptFor(string prompt, string defaultValue = null)
        {
            if (!String.IsNullOrEmpty(defaultValue))
                Console.Write($"{prompt} [{defaultValue}]: ");
            else
                Console.Write($"{prompt}: ");

            var input = Console.ReadLine();

            return input?.Length > 0 ? input : defaultValue;
        }

        static void DumpRecord(Thing thing)
        {
            Console.WriteLine($"id: {thing.Id}; fn: {thing.FirstName}; ln: {thing.LastName}; e: {thing.Email}");
        }
    }
}
