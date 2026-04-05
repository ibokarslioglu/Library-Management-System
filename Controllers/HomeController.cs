using Microsoft.AspNetCore.Mvc;/*ASP.NET Core MVC framework'ünü kullanır.*/
using rezervation.Models;/*Projedeki modelleri kullanmak için namespace'i ekler.*/
using System.Data.SqlClient;/*SQL Server veritabanı ile iletişim için gerekli olan namespace'i ekler.*/
using System.Diagnostics;/*Hata ayıklama ve izleme için gerekli olan namespace'i ekler.*/

namespace rezervation.Controllers/*Controller sınıflarını içeren namespace.*/
{
	public class HomeController : Controller/*HomeController sınıfı, ASP.NET Core MVC Controller sınıfından türetilir.*/
	{
		private const string ConnectionString = "Data Source=DESKTOP-58UCB9E\\SQLEXPRESS;Initial Catalog=Reservation;Integrated Security=True";

		private readonly ILogger<HomeController> _logger;/*Logger nesnesi, loglama işlemleri için kullanılır.*/

		public HomeController(ILogger<HomeController> logger)/*Logger nesnesini constructor üzerinden alır ve _logger alanına atar.*/
		{
			_logger = logger;
		}

		private List<Book> LoadPopularBooks(int take = 8)
		{
			var books = new List<Book>();
			try
			{
				using var connection = new SqlConnection(ConnectionString);
				connection.Open();

				const string popularQuery = @"
SELECT TOP (@take) b.BookID, b.BookName, b.AuthorName, b.PageNumber, b.Genre, COUNT(*) AS ReservationCount
FROM Reservations r
INNER JOIN Books b ON b.BookID = r.BookID
GROUP BY b.BookID, b.BookName, b.AuthorName, b.PageNumber, b.Genre
ORDER BY COUNT(*) DESC, b.BookName";

				using (var command = new SqlCommand(popularQuery, connection))
				{
					command.Parameters.AddWithValue("@take", take);
					using var reader = command.ExecuteReader();
					while (reader.Read())
					{
						books.Add(new Book
						{
							Id = Convert.ToInt32(reader["BookID"]),
							BookName = reader["BookName"]?.ToString() ?? string.Empty,
							AuthorName = reader["AuthorName"]?.ToString() ?? string.Empty,
							PageNumber = Convert.ToInt32(reader["PageNumber"]),
							Genre = reader["Genre"]?.ToString() ?? string.Empty,
							ReservationCount = Convert.ToInt32(reader["ReservationCount"])
						});
					}
				}

				if (books.Count > 0)
					return books;

				const string fallbackQuery = @"
SELECT TOP (@take) BookID, BookName, AuthorName, PageNumber, Genre
FROM Books
ORDER BY BookName";

				using (var command = new SqlCommand(fallbackQuery, connection))
				{
					command.Parameters.AddWithValue("@take", take);
					using var reader = command.ExecuteReader();
					while (reader.Read())
					{
						books.Add(new Book
						{
							Id = Convert.ToInt32(reader["BookID"]),
							BookName = reader["BookName"]?.ToString() ?? string.Empty,
							AuthorName = reader["AuthorName"]?.ToString() ?? string.Empty,
							PageNumber = Convert.ToInt32(reader["PageNumber"]),
							Genre = reader["Genre"]?.ToString() ?? string.Empty,
							ReservationCount = 0
						});
					}
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Populer kitaplar yuklenirken hata olustu.");
			}

			return books;
		}

		public IActionResult Index()/*Ana sayfa için action method.*/
		{
			return View();/*Varsayılan view'i döner (Index.cshtml).*/
		}
		public IActionResult Logout()
		{
			return View("Login");
		}
		public IActionResult Login()/*Login sayfası için action method.*/
		{
			return View();/*Varsayılan view'i döner (Login.cshtml).*/
		}
		public IActionResult Homepage()/*Ana sayfa için action method.*/
		{
			return View(new HomepageViewModel { PopularBooks = LoadPopularBooks() });/* Varsayılan view'i döner (Homepage.cshtml).*/
		}
		public ActionResult BookSearch(HomepageViewModel? model)/*Kitap arama işlemi için action method.*/
		{
			model ??= new HomepageViewModel();
			model.PopularBooks = LoadPopularBooks();

			if (string.IsNullOrWhiteSpace(model.BookName))
			{
				model.SearchResult = null;
				return View("Homepage", model);
			}

			using (SqlConnection connection = new SqlConnection(ConnectionString))
			{
				try
				{
					connection.Open();/*Veritabanı bağlantısını açar.*/

					string query = "SELECT * FROM Books WHERE BookName  = @bookName";/*SQL sorgusu, kitap adını parametre olarak kullanır.*/

					using (SqlCommand command = new SqlCommand(query, connection))
					{
						command.Parameters.AddWithValue("@bookName", model.BookName.Trim());/*SQL komutuna parametre ekler.*/
						using SqlDataReader reader = command.ExecuteReader();/*SQL sorgusunu çalıştırır ve sonuçları okur.*/

						if (reader.HasRows)
						{
							Book data = new Book();/*Yeni kitap nesnesi oluşturur.*/
							while (reader.Read())/*Sonuçları okur:*/
							{
								data.Id = Convert.ToInt32(reader["BookID"]);/*Kitap adını okur.*/
								data.BookName = reader["BookName"]?.ToString() ?? string.Empty;/*Kitap adını okur.*/
								data.AuthorName = reader["AuthorName"]?.ToString() ?? string.Empty;/*Yazar adını okur*/
								data.PageNumber = Convert.ToInt32(reader["PageNumber"]);/*Sayfa sayısını okur*/
								data.Genre = reader["Genre"]?.ToString() ?? string.Empty;/*Türü okur*/
							}
							model.SearchResult = data;
							return View("Homepage", model);/*Listeyi view'e döner.*/
						}

						Console.WriteLine("No rows found.");/*Konsola yazar*/
					}
				}
				catch (Exception ex)/*Hata yakalanır*/
				{
					Console.WriteLine("Hata: " + ex.Message);/*Hata mesajını konsola yazar*/
				}
				model.SearchResult = null;
				return View("Homepage", model);/*Hata varsa anasayfaya yönlendirir*/
			}
		}

		public IActionResult UserLogin(User user)/*Kullanıcı giriş işlemi için action method.*/
		{
			// SqlConnection nesnesini oluşturun ve bağlanın
			using (SqlConnection connection = new SqlConnection(ConnectionString))/*SQL bağlantısı oluşturulur ve kullanılır.*/
			{
				try
				{
					connection.Open();/*Veritabanı bağlantısını açar*/

					string query = "SELECT * FROM Users WHERE StudentNumber  = @studentNumber AND Password = @password";/*SQL sorgusu, kullanıcı adı ve şifresini kontrol eder.*/

					// SqlCommand nesnesini oluşturun
					using (SqlCommand command = new SqlCommand(query, connection))
					{
						// Parametreleri ekleyin

						command.Parameters.AddWithValue("@studentNumber", user.studentNumber);/* SQL komutuna kullanıcı adı parametresi ekler.*/
						command.Parameters.AddWithValue("@password", user.password);/*SQL komutuna şifre parametresi ekler.*/


						SqlDataReader reader = command.ExecuteReader();/*SQL sorgusunu çalıştırır ve sonuçları okur.*/

						// Döndürülen satırların olup olmadığını kontrol edin

						if (reader.HasRows)/*Sonuç varsa*/
						{
							return View("Homepage", new HomepageViewModel { PopularBooks = LoadPopularBooks() });/*Anasayfaya yönlendirir*/
						}
						else/*Sonuç yoksa*/
						{
							Console.WriteLine("No rows found.");/*Konsola yazar*/
						}
						connection.Close();/*Bağlantıyı kapatır*/
					}
				}
				catch (Exception ex)/*Hata yakalanır*/
				{
					Console.WriteLine("Hata: " + ex.Message);/*Hata mesajını konsola yazar*/
				}
				return View("Login");/*Hata varsa giriş sayfasına yönlendirir*/
			}

		}

		public IActionResult Privacy()/*Gizlilik sayfası için action method*/
		{
			return View();/*Varsayılan view'i döner (Privacy.cshtml)*/
		}
		[HttpPost]
		public IActionResult ReserveBook(Book book)
		{
			
			// SqlConnection nesnesini oluşturun ve bağlanın
			using (SqlConnection connection = new SqlConnection(ConnectionString))
			{
				Random random = new Random();/*Rastgele sayı üreteci oluşturur*/

				// 6 haneli random sayı üretin
				int resID = random.Next(100000, 999999);
				try
				{
					connection.Open();/*Veritabanı bağlantısını açar*/

					// INSERT INTO sorgusu
					string insertQuery = "INSERT INTO Reservations (ReservationID,IsActive,CreateDate,UpdateDate,BookID) VALUES (@reservationID, @active,@CreateDate,@UpdateDate,@bookID)";/*SQl ekleme sorgusu*/

					// SqlCommand nesnesini oluşturun
					using (SqlCommand command = new SqlCommand(insertQuery, connection))
					{
						// Parametreleri ekleyin
						command.Parameters.AddWithValue("@reservationID", resID);
						command.Parameters.AddWithValue("@active", "Y");
						command.Parameters.AddWithValue("@CreateDate", DateTime.Now);
						command.Parameters.AddWithValue("@UpdateDate", DateTime.Now);
						command.Parameters.AddWithValue("@bookID", book.Id);


						// Sorguyu çalıştırın
						int rowsAffected = command.ExecuteNonQuery();

						// Eğer gerekirse, etkilenen satır sayısını kontrol edin
						Console.WriteLine("Etkilenen satır sayısı: " + rowsAffected);
						connection.Close();/*Bağlantıyı kapatır*/
					}
				}
				catch (Exception ex)/*Hata yakalanır*/
				{
					Console.WriteLine("Hata: " + ex.Message);/*Hata mesajı konsola yazdırılır*/
				}
			}
			return View("Homepage", new HomepageViewModel { PopularBooks = LoadPopularBooks() });/*İşlem sonrası giriş sayfasına yönlendirilir*/
		}


		public IActionResult CreateUser(User user)/*Kullanıcı oluşturma işlemi için action method.*/
		{
			Random random = new Random();/*Rastgele sayı üreteci oluşturur*/

			// 6 haneli random sayı üretin
			int randomNumber = random.Next(100000, 999999);
			user.Id = randomNumber;/*Rastgele sayıyı kullanıcı ID'sine atar.*/
			// SqlConnection nesnesini oluşturun ve bağlanın
			using (SqlConnection connection = new SqlConnection(ConnectionString))
			{
				try
				{
					connection.Open();/*Veritabanı bağlantısını açar*/

					// INSERT INTO sorgusu
					string insertQuery = "INSERT INTO Users (UserID,Name,LastName,StudentNumber,CreateDate,UpdateDate,Password) VALUES (@userId, @userName,@userSurname,@userStudentNumber,@CreateDate,@UpdateDate,@userPassword)";/*SQl ekleme sorgusu*/

					// SqlCommand nesnesini oluşturun
					using (SqlCommand command = new SqlCommand(insertQuery, connection))
					{
						// Parametreleri ekleyin
						command.Parameters.AddWithValue("@userId", user.Id);
						command.Parameters.AddWithValue("@userName", user.name);
						command.Parameters.AddWithValue("@userSurname", user.surname);
						command.Parameters.AddWithValue("@userStudentNumber", user.studentNumber);
						command.Parameters.AddWithValue("@CreateDate", DateTime.Now);
						command.Parameters.AddWithValue("@UpdateDate", DateTime.Now);
						command.Parameters.AddWithValue("@userPassword", user.password);


						// Sorguyu çalıştırın
						int rowsAffected = command.ExecuteNonQuery();

						// Eğer gerekirse, etkilenen satır sayısını kontrol edin
						Console.WriteLine("Etkilenen satır sayısı: " + rowsAffected);
						connection.Close();/*Bağlantıyı kapatır*/
					}
				}
				catch (Exception ex)/*Hata yakalanır*/
				{
					Console.WriteLine("Hata: " + ex.Message);/*Hata mesajı konsola yazdırılır*/
				}
				return View("Login");/*İşlem sonrası giriş sayfasına yönlendirilir*/
			}
		}
		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]/*Yanıtın cachelenmesini engeller.*/
		public IActionResult Error()/* Hata sayfası için action method.*/
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });/*Error view'ini döner ve model olarak ErrorViewModel kullanır. Bu model, geçerli istek kimliğini içerir.*/
		}
	}
}

