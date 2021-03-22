using System.Net.WebSockets;
using System.Data;
using System;
using System.Data.SqlClient;
using Dapper;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace DZ_18_03_21
{
	class Customer
	{
		public int Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string MiddleName { get; set; }
		public DateTime DateOfBirth { get; set; }
		public void Show()
		{
			System.Console.WriteLine(new String('-', 40));
			Console.WriteLine($"ID: {this.Id}");
			Console.WriteLine($"FirstName: {this.FirstName}");
			Console.WriteLine($"LastName: {this.LastName}");
			Console.WriteLine($"MidlleName: {this.MiddleName}");
			Console.WriteLine($"DateOfBirthday: {this.DateOfBirth.ToShortDateString()}");
			System.Console.WriteLine(new String('-', 40));
		}

	}
	enum ActionType
	{
		Create = 1,
		Read = 2,
		Update = 3,
		Delete = 4
	}
	class Program
	{
		const string conString = "Data source = 10.211.55.3;Initial catalog=AlifAcademy;user id=sa;password=1234";
		static async Task Main(string[] args)
		{
			bool isOn = true;
			while (isOn)
			{
				Console.WriteLine("1. Create");
				Console.WriteLine("2. Read");
				Console.WriteLine("3. Update");
				Console.WriteLine("4. Delete");
				Console.WriteLine("Enter to exit");
				if (!int.TryParse(Console.ReadLine(), out int choose))
				{
					isOn = false;
					break;
				}
				switch ((ActionType)choose)
				{
					case ActionType.Create:
						{
							await InsertToDB();
							break;
						}
					case ActionType.Read:
						{
							await SelectAllFromDB();
							break;
						}
					case ActionType.Update:
						{
							await UpdateByIdInDB();
							break;
						}
					case ActionType.Delete:
						{
							await DeleteByIdFromDB();
							break;
						}
					default:
						isOn = false;
						break;
				}
			}


		}
		static void WriteLineSuccessText(string text)
		{
			WriteLineWithColor(text, ConsoleColor.Green);
		}
		static void WriteLineFailText(string text)
		{
			WriteLineWithColor(text, ConsoleColor.Red);
		}
		static void WriteLineWithColor(string text, ConsoleColor color)
		{
			Console.ForegroundColor = color;
			Console.WriteLine(text);
			Console.ForegroundColor = ConsoleColor.White;
		}
		static async Task InsertToDB()
		{
			try
			{
				var cust = new Customer()
				{
					FirstName = ConsoleReadLineWithText("Enter FirstName: "),
					LastName = ConsoleReadLineWithText("Enter LastName: "),
					MiddleName = ConsoleReadLineWithText("Enter MiddleName: "),
					DateOfBirth = DateTime.Parse(ConsoleReadLineWithText("Enter DateOfBirth: "))
				};
				using (IDbConnection conn = new SqlConnection(conString))
				{
					var result = await conn.ExecuteAsync($"INSERT INTO Customers(FirstName,LastName,MiddleName,DateOfBirth) VALUES(@FirstName,@LastName,@MiddleName,@DateOfBirth)", cust);
					if (result > 0)
					{
						WriteLineSuccessText("Customer inserted");
					}
				}
			}
			catch (Exception e)
			{
				WriteLineFailText(e.Message);
			}
		}
		static async Task SelectAllFromDB()
		{
			List<Customer> list = new List<Customer>();
			try
			{
				using (IDbConnection conn = new SqlConnection(conString))
				{
					System.Console.WriteLine();
					System.Console.WriteLine();
					list = (await conn.QueryAsync<Customer>("SELECT * FROM Customers")).ToList();
					foreach (var per in list)
					{
						per.Show();
					}
					System.Console.WriteLine();
					System.Console.WriteLine();
				}
			}
			catch (Exception e)
			{
				WriteLineFailText(e.Message);
			}

		}
		static async Task UpdateByIdInDB()
		{
			var cust = new Customer()
			{
				Id = int.Parse(ConsoleReadLineWithText("Id: ")),
				FirstName = ConsoleReadLineWithText("Enter new FirstName: "),
				LastName = ConsoleReadLineWithText("Enter new LastName: "),
				MiddleName = ConsoleReadLineWithText("Enter new MiddleName: "),
				DateOfBirth = DateTime.Parse(ConsoleReadLineWithText("Enter new DateOfBirth: "))
			};

			try
			{
				using (IDbConnection conn = new SqlConnection(conString))
				{
					var result = await conn.ExecuteAsync("UPDATE Customers SET FirstName = @FirstName, LastName = @LastName, MiddleName = @MiddleName, DateOfBirth = @DateOfBirth WHERE Id = @Id", cust);
					if (result > 0)
					{
						WriteLineSuccessText("Customer has updated");
					}
					else
					{
						WriteLineFailText("Bad Id");
					}
				}
			}
			catch (Exception e)
			{
				WriteLineFailText(e.Message);
			}
		}
		static async Task DeleteByIdFromDB()
		{
			int id = int.Parse(ConsoleReadLineWithText("Id: "));
			try
			{
				using (IDbConnection conn = new SqlConnection(conString))
				{
					var result = await conn.ExecuteAsync("DELETE FROM Customers WHERE Id = @Id", new { Id = id });
					if (result > 0)
					{
						WriteLineSuccessText("Customer has deleted");
					}
					else
					{
						WriteLineFailText("Bad Id");
					}
				}
			}
			catch (Exception e)
			{
				WriteLineFailText(e.Message);
			}
		}
		static string ConsoleReadLineWithText(string text)
		{
			Console.WriteLine(text);
			return Console.ReadLine();
		}
	}
}
