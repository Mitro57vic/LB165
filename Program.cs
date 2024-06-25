using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;

namespace LB165
{
    public class Team
    {
        public ObjectId Id { get; set; }
        public string TeamName { get; set; }
        public string Stadt { get; set; }
        public string Conference { get; set; }
    }

    public class Spieler
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }
        public int Groesse { get; set; }
        public int Gewicht { get; set; }
    }

    public class Stats
    {
        public ObjectId Id { get; set; }
        public double Ppg { get; set; }
        public double Rpg { get; set; }
        public double Apg { get; set; }
        public ObjectId SpielerId { get; set; }
    }

    internal class Program
    {
        private static string connectionString = "mongodb://localhost:27017";
        private static string databaseName = "Nba";

        static void Main(string[] args)
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);

            var teamsCollection = database.GetCollection<Team>("Team");
            var spielerCollection = database.GetCollection<Spieler>("Spieler");
            var statsCollection = database.GetCollection<Stats>("Stats");
            var spieler2TeamsCollection = database.GetCollection<BsonDocument>("Spieler2Teams");

            Console.WriteLine("Willkommen zur Team- und Spieler-Verwaltung!");

            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("Bitte wählen Sie eine Aktion aus:");
                Console.WriteLine("1. Neues Team hinzufügen");
                Console.WriteLine("2. Neuen Spieler hinzufügen");
                Console.WriteLine("3. Spieler finden und anzeigen");
                Console.WriteLine("4. Spieler-Informationen aktualisieren");
                Console.WriteLine("5. Spieler löschen");
                Console.WriteLine("6. Beenden");

                string userInput = Console.ReadLine();

                switch (userInput)
                {
                    case "1":
                        AddNewTeam(teamsCollection);
                        break;
                    case "2":
                        AddNewSpieler(spielerCollection);
                        break;
                    case "3":
                        FindAndDisplaySpieler(spielerCollection);
                        break;
                    case "4":
                        UpdateSpieler(spielerCollection);
                        break;
                    case "5":
                        DeleteSpieler(spielerCollection, spieler2TeamsCollection);
                        break;
                    case "6":
                        Console.WriteLine("Programm wird beendet.");
                        return;
                    default:
                        Console.WriteLine("Ungültige Eingabe. Bitte wählen Sie eine der verfügbaren Optionen.");
                        break;
                }
            }
        }

        static void AddNewTeam(IMongoCollection<Team> teamsCollection)
        {
            Console.WriteLine();
            Console.WriteLine("Neues Team hinzufügen:");

            Console.Write("TeamName: ");
            string teamName = Console.ReadLine();

            Console.Write("Stadt: ");
            string stadt = Console.ReadLine();

            Console.Write("Conference: ");
            string conference = Console.ReadLine();

            var newTeam = new Team
            {
                TeamName = teamName,
                Stadt = stadt,
                Conference = conference
            };

            teamsCollection.InsertOne(newTeam);
            Console.WriteLine("Neues Team hinzugefügt!");
        }

        static void AddNewSpieler(IMongoCollection<Spieler> spielerCollection)
        {
            Console.WriteLine();
            Console.WriteLine("Neuen Spieler hinzufügen:");

            Console.Write("Name: ");
            string name = Console.ReadLine();

            Console.Write("Position: ");
            string position = Console.ReadLine();

            Console.Write("Größe: ");
            int groesse = int.Parse(Console.ReadLine());

            Console.Write("Gewicht: ");
            int gewicht = int.Parse(Console.ReadLine());

            var newSpieler = new Spieler
            {
                Name = name,
                Position = position,
                Groesse = groesse,
                Gewicht = gewicht
            };

            spielerCollection.InsertOne(newSpieler);
            Console.WriteLine("Neuer Spieler hinzugefügt!");
        }

        static void FindAndDisplaySpieler(IMongoCollection<Spieler> spielerCollection)
        {
            Console.WriteLine();
            Console.WriteLine("Spieler finden und anzeigen:");

            Console.Write("Spieler Name: ");
            string spielerName = Console.ReadLine();

            var filter = Builders<Spieler>.Filter.Eq("Name", spielerName);
            var spieler = spielerCollection.Find(filter).FirstOrDefault();

            if (spieler == null)
            {
                Console.WriteLine($"Spieler '{spielerName}' nicht gefunden.");
            }
            else
            {
                Console.WriteLine($"Spieler gefunden:");
                Console.WriteLine($"Name: {spieler.Name}");
                Console.WriteLine($"Position: {spieler.Position}");
                Console.WriteLine($"Größe: {spieler.Groesse}");
                Console.WriteLine($"Gewicht: {spieler.Gewicht}");
            }
        }


        static void UpdateSpieler(IMongoCollection<Spieler> spielerCollection)
        {
            Console.WriteLine();
            Console.WriteLine("Spieler-Informationen aktualisieren:");

            Console.Write("Spieler Name: ");
            string spielerName = Console.ReadLine();

            var filter = Builders<Spieler>.Filter.Eq("Name", spielerName);
            var spieler = spielerCollection.Find(filter).FirstOrDefault();

            if (spieler == null)
            {
                Console.WriteLine($"Spieler '{spielerName}' nicht gefunden.");
                return;
            }

            Console.WriteLine($"Aktuelle Informationen für Spieler '{spieler.Name}':");
            Console.WriteLine($"1. Name: {spieler.Name}");
            Console.WriteLine($"2. Position: {spieler.Position}");
            Console.WriteLine($"3. Größe: {spieler.Groesse}");
            Console.WriteLine($"4. Gewicht: {spieler.Gewicht}");

            Console.WriteLine("Welche Eigenschaft möchten Sie aktualisieren? (1-4)");
            int choice;
            if (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > 4)
            {
                Console.WriteLine("Ungültige Eingabe.");
                return;
            }

            string newValue;
            switch (choice)
            {
                case 1:
                    Console.Write("Neuer Name: ");
                    newValue = Console.ReadLine();
                    spieler.Name = newValue;
                    break;
                case 2:
                    Console.Write("Neue Position: ");
                    newValue = Console.ReadLine();
                    spieler.Position = newValue;
                    break;
                case 3:
                    Console.Write("Neue Größe: ");
                    int newGroesse;
                    if (!int.TryParse(Console.ReadLine(), out newGroesse))
                    {
                        Console.WriteLine("Ungültige Eingabe.");
                        return;
                    }
                    spieler.Groesse = newGroesse;
                    break;
                case 4:
                    Console.Write("Neues Gewicht: ");
                    int newGewicht;
                    if (!int.TryParse(Console.ReadLine(), out newGewicht))
                    {
                        Console.WriteLine("Ungültige Eingabe.");
                        return;
                    }
                    spieler.Gewicht = newGewicht;
                    break;
                default:
                    Console.WriteLine("Ungültige Eingabe.");
                    return;
            }

            var updateResult = spielerCollection.ReplaceOne(filter, spieler);
            if (updateResult.IsAcknowledged && updateResult.ModifiedCount > 0)
            {
                Console.WriteLine("Spieler-Informationen aktualisiert!");
            }
            else
            {
                Console.WriteLine("Fehler beim Aktualisieren der Spieler-Informationen.");
            }
        }

        static void DeleteSpieler(IMongoCollection<Spieler> spielerCollection, IMongoCollection<BsonDocument> spieler2TeamsCollection)
        {
            Console.WriteLine();
            Console.WriteLine("Spieler löschen:");

            Console.Write("Spieler Name: ");
            string spielerName = Console.ReadLine();

            var spielerFilter = Builders<Spieler>.Filter.Eq("Name", spielerName);
            var spieler = spielerCollection.Find(spielerFilter).FirstOrDefault();

            if (spieler == null)
            {
                Console.WriteLine($"Spieler '{spielerName}' nicht gefunden.");
                return;
            }

            var deleteResult = spielerCollection.DeleteOne(spielerFilter);
            if (deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0)
            {
                Console.WriteLine($"Spieler '{spielerName}' erfolgreich gelöscht.");

                
                var deleteFilter = Builders<BsonDocument>.Filter.Eq("spielerId", spieler.Id);
                spieler2TeamsCollection.DeleteMany(deleteFilter);
            }
            else
            {
                Console.WriteLine($"Fehler beim Löschen des Spielers '{spielerName}'.");
            }
        }
    }
}
