using System;
using System.Collections.Generic;
using BitPay.Models;
using BitPay.Utils;
using Newtonsoft.Json;
using System.Threading.Tasks;
using BitPay.Exceptions;

namespace BitPay.Clients
{
    public class AuthorizationClient
    {
        private readonly IBitPayClient _bitPayClient;
        private readonly IGuidGenerator _guidGenerator;
        private readonly AccessTokens _accessTokens;
        private readonly string _identity;

        public AuthorizationClient(IBitPayClient bitPayClient, IGuidGenerator guidGenerator, AccessTokens accessTokens,
            string identity)
        {
            _bitPayClient = bitPayClient ?? throw new ArgumentNullException(nameof(bitPayClient));
            _guidGenerator = guidGenerator ?? throw new ArgumentNullException(nameof(guidGenerator));
            _accessTokens = accessTokens ?? throw new ArgumentNullException(nameof(accessTokens));
            _identity = identity ?? throw new ArgumentNullException(nameof(identity));
        }

        public async Task AuthorizeClient(string pairingCode)
        {
            try
            {
                Token token = new Token
                {
                    Id = _identity, Guid = _guidGenerator.Execute(), PairingCode = pairingCode
                };
                var json = JsonConvert.SerializeObject(token);
                var response = await _bitPayClient.Post("tokens", json);
                var responseString = await HttpResponseParser.ResponseToJsonString(response);
                var tokens = JsonConvert.DeserializeObject<List<Token>>(responseString);
                foreach (var t in tokens) _accessTokens.AddToken(t.Facade, t.Value);
            }
            catch (Exception ex)
            {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException)))
                    throw new ClientAuthorizationException(ex);

                throw;
            }
        }
        
        /// <summary>
        ///     Request authorization (a token) for this client in the specified facade.
        /// </summary>
        /// <param name="facade">The facade for which authorization is requested.</param>
        /// <returns>A pairing code for this client. This code must be used to authorize this client at BitPay.com/api-tokens.</returns>
        /// <throws>ClientAuthorizationException ClientAuthorizationException class</throws>
        /// <throws>BitPayException BitPayException class</throws>
        public async Task<string> CreatePairingCodeForFacade(string facade)
        {
            try
            {
                var token = new Token
                {
                    Id = _identity,
                    Guid = _guidGenerator.Execute(),
                    Facade = facade
                };
                var json = JsonConvert.SerializeObject(token);
                var response = await _bitPayClient.Post("tokens", json).ConfigureAwait(false);
                var responseString = await HttpResponseParser.ResponseToJsonString(response).ConfigureAwait(false);
                var tokens = JsonConvert.DeserializeObject<List<Token>>(responseString);
                _accessTokens.AddToken(tokens[0].Facade, tokens[0].Value);

                return tokens[0].PairingCode;
            }
            catch (BitPayException ex)
            {
                throw new ClientAuthorizationException(ex, ex.GetApiCode());
            }
            catch (Exception ex)
            {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException)))
                    throw new ClientAuthorizationException(ex);

                throw;
            }
        }
    }
}