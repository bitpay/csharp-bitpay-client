// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using BitPay.Exceptions;
using BitPay.Models;
using BitPay.Utils;

using Newtonsoft.Json;

namespace BitPay.Clients
{
    public class AuthorizationClient
    {
        private readonly IBitPayClient _bitPayClient;
        private readonly IGuidGenerator _guidGenerator;
        private readonly AccessTokens _accessTokens;
        private readonly string _identity;

        public AuthorizationClient(IBitPayClient bitPayClient, IGuidGenerator guidGenerator, AccessTokens accessTokens,
            string? identity)
        {
            _bitPayClient = bitPayClient ?? throw new ArgumentNullException(nameof(bitPayClient));
            _guidGenerator = guidGenerator ?? throw new ArgumentNullException(nameof(guidGenerator));
            _accessTokens = accessTokens ?? throw new ArgumentNullException(nameof(accessTokens));
            _identity = identity ?? throw new ArgumentNullException(nameof(identity));
        }

        /// <summary>
        ///     Authorize (pair) this client with the server using the specified pairing code.
        /// </summary>
        /// <param name="pairingCode">A code obtained from the server; typically from bitpay.com/api-tokens.</param>
        /// <exception cref="BitPayGenericException">BitPayGenericException class</exception>
        /// <exception cref="BitPayApiException">BitPayApiException class</exception>
        public async Task AuthorizeClient(string pairingCode)
        {
            Token token = new(
                id: _identity,
                resourceGuid: _guidGenerator.Execute()
            ) { PairingCode = pairingCode };

            string json = null!;
            
            try
            {
                json = JsonConvert.SerializeObject(token);
            }
            catch (Exception e)
            {
                BitPayExceptionProvider.ThrowGenericExceptionWithMessage(
                    "Failed to serialize Token object : " + e.Message);
            }
            
            var response = await _bitPayClient.Post("tokens", json).ConfigureAwait(false);
            var responseString = await HttpResponseParser.ResponseToJsonString(response).ConfigureAwait(false);

            List<Token> tokens = null!;
            
            try
            {
                tokens = JsonConvert.DeserializeObject<List<Token>>(responseString)!;
                
            }
            catch (Exception e)
            {
                BitPayExceptionProvider.ThrowDeserializeResourceException("Tokens", e.Message);
            }
            
            foreach (var t in tokens) _accessTokens.AddToken(t.Facade!, t.Value!);
        }
        
        /// <summary>
        ///     Request authorization (a token) for this client in the specified facade.
        /// </summary>
        /// <param name="facade">The facade for which authorization is requested.</param>
        /// <returns>A pairing code for this client. This code must be used to authorize this client at BitPay.com/api-tokens.</returns>
        /// <exception cref="BitPayGenericException">BitPayGenericException class</exception>
        /// <exception cref="BitPayApiException">BitPayApiException class</exception>
        public async Task<string> CreatePairingCodeForFacade(string facade)
        {
            var token = new Token(
                id: _identity,
                resourceGuid: _guidGenerator.Execute()
            ) { Facade = facade };

            string json = null!;
            
            try
            {
                json = JsonConvert.SerializeObject(token);
            }
            catch (Exception e)
            {
                BitPayExceptionProvider.ThrowSerializeResourceException("Token", e.Message);
            }
            
            var response = await _bitPayClient.Post("tokens", json).ConfigureAwait(false);
            var responseString = await HttpResponseParser.ResponseToJsonString(response).ConfigureAwait(false);
            var tokens = JsonConvert.DeserializeObject<List<Token>>(responseString)!;
            _accessTokens.AddToken(tokens[0].Facade!, tokens[0].Value!);
            
            return tokens[0].PairingCode!;
        }
    }
}