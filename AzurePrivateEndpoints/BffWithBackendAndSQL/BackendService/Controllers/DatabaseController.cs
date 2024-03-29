﻿using System;
using System.Data.SqlClient;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace BackendService.Controllers
{
    [Route("")]
    [ApiController]
    public class DatabaseController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public DatabaseController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpGet]
        public async Task<string> Get()
        {
            var resultText = new StringBuilder();
            try
            {
                var addresses = Dns.GetHostAddresses(configuration["ServerName"]);
                resultText.Append($"Resolved name '{configuration["ServerName"]}':\n");
                foreach (var hostAddress in addresses)
                {
                    resultText.AppendFormat("{0}\n", hostAddress);
                }
            }
            catch (Exception ex)
            {
                resultText.Append($"ERROR while resolving name '{configuration["ServerName"]}':\n");
                resultText.Append(ex.Message);
            }

            resultText.Append('\n');

            try
            {
                using var conn = new SqlConnection(configuration["ConnectionString"]);
                await conn.OpenAsync();
                using var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT 'Hi' AS Greet";
                var result = (string)await cmd.ExecuteScalarAsync();
                resultText.Append($"SQL Query Result:\n");
                resultText.Append(result);
            }
            catch (Exception ex)
            {
                resultText.Append($"ERROR while querying DB:\n");
                resultText.Append(ex.Message);
            }

            return resultText.ToString();
        }
    }
}
