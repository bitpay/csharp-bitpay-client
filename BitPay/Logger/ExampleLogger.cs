// Copyright (c) 2019 BitPay.
// All rights reserved.
//
// using Microsoft.Extensions.Logging;
//
// namespace BitPay.Logger
// {
//     public class ExampleLogger : IBitPayLogger
//     {
//         private readonly ILogger<ExampleLogger> _logger;
//
//         public ExampleLogger(ILogger<ExampleLogger> logger)
//         {
//             this._logger = logger;
//         }
//
//         public void LogRequest(string method, string endpoint, string? json)
//         {
//             this._logger.LogInformation("Request method: " + method + " Endpoint: " + endpoint + " Json: " + json);
//         }
//
//         public void LogResponse(string method, string endpoint, string? json)
//         {
//             this._logger.LogInformation("Response method: " + method + " Endpoint: " + endpoint + " Json: " + json);
//         }
//
//         public void LogError(string message)
//         {
//             this._logger.LogError(message);
//         }
//     }
// }