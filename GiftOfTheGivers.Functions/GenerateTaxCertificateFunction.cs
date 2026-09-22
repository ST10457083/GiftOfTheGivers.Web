using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace GiftOfTheGivers.Functions
{
    public class GenerateTaxCertificateFunction
    {
        private readonly ILogger _logger;

        public GenerateTaxCertificateFunction(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<GenerateTaxCertificateFunction>();
        }

        [Function("GenerateTaxCertificate")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("Tax certificate generation requested.");

            var query = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
            string donationId = query["donationId"] ?? "0";
            string donorName = query["donorName"] ?? "Anonymous Donor";
            string amount = query["amount"] ?? "0.00";
            string currency = query["currency"] ?? "ZAR";

            string certificateNumber = $"GOTG-{DateTime.Now:yyyyMMdd}-{donationId.PadLeft(6, '0')}";

            var responseObj = new
            {
                CertificateNumber = certificateNumber,
                DonorName = donorName,
                Amount = amount,
                Currency = currency,
                IssueDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Message = "Tax certificate generated successfully."
            };

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json");
            await response.WriteStringAsync(JsonSerializer.Serialize(responseObj));

            return response;
        }
    }
}
