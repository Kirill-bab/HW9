using NUnit.Framework;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace WebService_Tests
{
    public class Tests
    {
        private HttpClient _httpClient;

        [SetUp]
        public void Setup()
        {
            // var json = JsonSerializer.Serialize(new ConnectionSettings() { BaseAddress = "http://localhost:5000" },new JsonSerializerOptions() {WriteIndented = true });
            // File.WriteAllText("connectionSettings.json", json);
            var connectionSettings = JsonSerializer.Deserialize<ConnectionSettings>(File.ReadAllText("connectionSettings.json"));

            if (!File.Exists("connectionSettings.json"))
            {
                Console.WriteLine("File with connection settings was not located!");
                throw new FileNotFoundException("connectionSettings.json");
            }

            Uri.TryCreate(connectionSettings.BaseAddress, UriKind.Absolute, out var uri);

            _httpClient = new HttpClient()
            {
                BaseAddress = uri
            };
        }

        [Test]
        public async Task Should_Return_Program_Info_With_OK_StatusCode()
        {
            var response = await _httpClient.GetAsync("/");
            Console.WriteLine("Test get request / performed!");
            var responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"get request / returned: {responseBody}\n with Status code: {response.StatusCode}");
            Assert.IsNotEmpty(responseBody, "get request / has failed!");
        }

        [Test]
        public async Task Should_Return_OK_StatusCode_When_Number_Is_Prime()
        {

            var response = await _httpClient.GetAsync("/primes/7");
            var responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Test get request /primes/7 performed!");
            Console.WriteLine($"get request /primes/7 returned: {responseBody}\n with Status code: {response.StatusCode}");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "get request /primes/7 has failed!");
        }

        [Test]
        public async Task Should_Return_NotFound_StatusCode_When_Number_Is_Not_Prime()
        {
            var response = await _httpClient.GetAsync($"/primes/4");
            var responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Test get request /primes/4 performed!");
            Console.WriteLine($"get request /primes/4 returned: {responseBody}\n with Status code: {response.StatusCode}");

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "get request /primes/7 has failed!");
        }

        [Test]
        public async Task Should_Return_Not_Empty_Primes_List_With_StatusCode_OK()
        {
            var response = await _httpClient.GetAsync("/primes?from=1&to=5");
            Console.WriteLine("Test get request /primes?from=1&to=5 performed!");

            var responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"get request /primes?from=1&to=5 returned: {responseBody}\n with Status code: {response.StatusCode}");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("[2, 3, 5]", responseBody);
        }

        [Test]
        public async Task Should_Return_Empty_Primes_List_With_StatusCode_OK()
        {
            var response = await _httpClient.GetAsync("/primes?from=-5&to=1");
            Console.WriteLine("Test get request /primes?from=-5&to=1 performed!");

            var responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"get request /primes?from=-5&to=1 returned: {responseBody}\n with Status code: {response.StatusCode}");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("[]", responseBody);
        }

        [Test]
        public async Task Should_Return_Nothing_With_StatusCode_BadRequest()
        {
            var response = await _httpClient.GetAsync("/primes?from=-5&to=abc");
            Console.WriteLine("Test get request /primes?from=-5&to=abc performed!");

            var responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"get request /primes?from=-5&to=abc returned: {responseBody}\n with Status code: {response.StatusCode}");

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}