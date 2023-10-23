// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;

using BitPay.Logger;

namespace BitPay.Exceptions
{
    public class BitPayExceptionProvider
    {
        public const string GenericApiUnmappedErrorCode = "000000";
        
        /// <exception cref="BitPayGenericException">BitPayGenericException class</exception>
        public static void ThrowGenericExceptionWithMessage(string message)
        {
            LogErrorMessage(message);

            throw new BitPayGenericException(message);
        }
        
        /// <exception cref="BitPayApiException"></exception>
        public static void ThrowApiExceptionWithMessage(string message)
        {
            LogErrorMessage(message);
        
            throw new BitPayApiException(message, GenericApiUnmappedErrorCode);
        }
        
        /// <exception cref="BitPayApiException"></exception>
        public static void ThrowApiExceptionWithMessage(string message, string code)
        {
            LogErrorMessage(message);
        
            throw new BitPayApiException(message, code);
        }
        
        /// <exception cref="BitPayGenericException"></exception>
        public static void ThrowDeserializeResourceException(string resource, string exceptionMessage)
        {
            var message = String.Format("Failed to deserialize response ( {0} ) : {1}", resource, exceptionMessage);
            ThrowGenericExceptionWithMessage(message);
        }

        /// <exception cref="BitPayGenericException"></exception>
        public static void ThrowDeserializeException(String exceptionMessage)
        {
            String message = "Failed to deserialize BitPay server response : " + exceptionMessage;
            ThrowGenericExceptionWithMessage(message);
        }
        
        /// <exception cref="BitPayGenericException"></exception>
        public static void ThrowEncodeException(String exceptionMessage)
        {
            String message = "Failed to encode params : " + exceptionMessage;
            ThrowGenericExceptionWithMessage(message);
        }
        
        /// <exception cref="BitPayGenericException"></exception>
        public static void ThrowSerializeResourceException(String resource, String exceptionMessage)
        {
            String message = String.Format("Failed to serialize ( {0} ) : " + exceptionMessage, resource);
            ThrowGenericExceptionWithMessage(message);
        }
        
        /// <exception cref="BitPayGenericException"></exception>
        public static void ThrowSerializeParamsException(String exceptionMessage)
        {
            String message = "Failed to serialize params : " + exceptionMessage;
            ThrowGenericExceptionWithMessage(message);
        }
        
        /// <exception cref="BitPayValidationException"></exception>
        public static void ThrowValidationException(String message)
        {
            LogErrorMessage(message);
        
            throw new BitPayValidationException(message);
        }
        
        /// <exception cref="BitPayValidationException"></exception>
        public static void ThrowMissingParameterException()
        {
            var message = "Missing required parameter";
        
            LogErrorMessage(message);
        
            throw new BitPayValidationException(message);
        }
        
        /// <exception cref="BitPayValidationException"></exception>
        public static void ThrowInvalidCurrencyException(String currencyCode)
        {
            String message = String.Format("Currency code {0} must be a type of Model.Currency", currencyCode);
            ThrowValidationException(message);
        }
        
        private static void LogErrorMessage(string? message) 
        {
            if (message != null) {
                LoggerProvider.GetLogger().LogError(message);
            }
        }
    }
}
