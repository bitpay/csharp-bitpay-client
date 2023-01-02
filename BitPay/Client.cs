using System;
using System.IO;
using System.Net.Http;
using BitPay.Clients;
using BitPay.Utils;
using Microsoft.Extensions.Configuration;

namespace BitPay
{
    public class Client
    {
        private BitPayClient _bitPayClient;
        private AccessTokens _accessTokens;
        private GuidGenerator _guidGenerator;
        private string _identity;

        public Client(PosToken token)
        {
            InitPosClient(token, Environment.Prod);
        }
        
        public Client(PosToken token, Environment environment)
        {
            InitPosClient(token, environment);
        }

        public Client(Environment environment, PrivateKey privateKey, AccessTokens accessTokens)
        {
            var ecKey = GetEcKey(privateKey);
            var baseUrl = GetBaseUrl(environment);
            var httpClient = getHttpClient(baseUrl);
            
            _accessTokens = accessTokens;
            _bitPayClient = new BitPayClient(httpClient, baseUrl, ecKey);
            _guidGenerator = new GuidGenerator();
            CreateIdentity(ecKey);
        }

        public Client(ConfigFilePath configFilePath)
        {
            IConfiguration config = BuildConfigFromFile(configFilePath);
            _accessTokens = new AccessTokens(config);
        }

        private IConfiguration BuildConfigFromFile(ConfigFilePath configFilePath)
        {
            if (!File.Exists(configFilePath.Value()))
            {
                throw new Exception("Configuration file not found");
            }
            
            var builder = new ConfigurationBuilder().AddJsonFile(configFilePath.Value(), false, true);
            return builder.Build();
        }

        /// <summary>
        ///     Gets ECKey.
        /// </summary>
        /// <param name="privateKey">PrivateKey</param>
        /// <returns>EcKey</returns>
        private EcKey GetEcKey(PrivateKey privateKey)
        {
            if (File.Exists(privateKey.Value()) && KeyUtils.PrivateKeyExists(privateKey.Value()))
            {
                return KeyUtils.LoadEcKey().Result;
            }
            else
            {
                try
                {
                    return KeyUtils.CreateEcKeyFromString(privateKey.Value());
                }
                catch (Exception e)
                {
                    throw new Exception("Private Key file not found OR invalid key provided");
                }
            }
        }

        private void CreateIdentity(EcKey ecKey)
        {
            _identity = KeyUtils.DeriveSin(ecKey);
        }

        private void InitPosClient(PosToken token, Environment environment)
        {
            var baseUrl = GetBaseUrl(environment);
            var httpClient = getHttpClient(baseUrl);

            _accessTokens = new AccessTokens();
            _accessTokens.AddPos(token.Value());
            _bitPayClient = new BitPayClient(httpClient, baseUrl, null);
            _guidGenerator = new GuidGenerator();
        }

        private static HttpClient getHttpClient(string baseUrl)
        {
            return new HttpClient() {BaseAddress = new Uri(baseUrl)};
        }

        private static string GetBaseUrl(Environment environment)
        {
            return environment == Environment.Test ? Config.TestUrl : Config.ProdUrl;
        }
    }
}