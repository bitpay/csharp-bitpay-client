// Copyright (c) 2019 BitPay.
// All rights reserved.

using System.Net;
using System.Net.Http;

using BitPay.Clients;
using BitPay.Exceptions;

using Xunit;

namespace BitPayUnitTest.Clients
{
    public class HttpResponseParserTest
    {
        [Fact]
        public void it_should_throws_bitpay_api_exception_for_response_with_errors()
        {
            HttpResponseMessage responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(
                    "{\"errors\":[{\"error\":\"Missing required parameter.\",\"param\":\"price\"},{\"error\":\"Missing required parameter.\",\"param\":\"currency\"}]}"),
                RequestMessage = new HttpRequestMessage(HttpMethod.Post, "any")
            };

            var exception =
                Assert.ThrowsAsync<BitPayApiException>(() => HttpResponseParser.ResponseToJsonString(responseMessage))
                    .Result;
            Assert.Equal("Missing required parameter price. Missing required parameter currency.", exception.Message);
        }
    }
}