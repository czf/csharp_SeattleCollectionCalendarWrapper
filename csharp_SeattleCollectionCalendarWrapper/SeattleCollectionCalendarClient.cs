using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("NUnit.Tests")]
namespace SeattleCollectionCalendarWrapper
{
    public class SeattleCollectionCalendarClient : IDisposable
    {

        #region private
        private static readonly Uri BASE_URL = new Uri("https://www.seattle.gov/UTIL/WARP/CollectionCalendar/");
        private const string GET_CC_ADDRESS_ENDPOINT = "GetCCAddress?pAddress=";
        private const string GET_COLLECTION_DAYS_ENDPOINT = "GetCollectionDays?pAccount={0}&pAddress={1}&pJustChecking=&pApp=CC&pIE=&start={2}&end={3}";

        private bool disposedValue = false;
        private HttpClient _client;
        #endregion


        #region Constructors
        public SeattleCollectionCalendarClient() : this(new HttpClient()) { }
        
        public SeattleCollectionCalendarClient(HttpClient client)
        {
            _client = client;
            _client.BaseAddress = BASE_URL;
            _client.Timeout.Add(new TimeSpan(0, 3, 0));
        }
        #endregion Constructors
        
        #region IDisposable Support

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _client.Dispose();
                }

                disposedValue = true;
            }
        }


        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion

        #region public

        /// <summary>
        /// Lookup valid collection addresses based on the specified address.
        /// </summary>
        /// <param name="address">address to lookup</param>
        /// <returns>
        /// a list of addresss that the specified address may represent.  When empty, the specified address
        /// could be input incorrectly or doesn't have collection account
        /// </returns>
        /// <remarks>FYI: This is known to be a slow call.</remarks>
        public async Task<string[]> GetCcAddressAsync(string address)
        {
            string[] result = new string[0];

            using (HttpResponseMessage response = await _client.GetAsync(GET_CC_ADDRESS_ENDPOINT + address))
            {
                using (HttpContent content = response.Content)
                {
                    string json = await content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<string[]>(json);
                }
            }

            return result;
        }

        /// <summary>
        /// Synchronous call to GetAddressAsync 
        /// </summary>
        /// <param name="address">address to lookup</param>
        /// a list of addresss that the specified address may represent.  When empty, the specified address
        /// could be input incorrectly or doesn't have collection account
        public string[] GetCcAddress(string address)
        {
            Task<string[]> responseTask = GetCcAddressAsync(address);
            Task.WaitAll(responseTask);
            return responseTask.Result;
        }

        /// <summary>
        /// Lookup what will be collected and when collection will take place based on the specified address
        /// </summary>
        /// <param name="address">valid address. should be confired using GetCCAddres(Async)</param>
        /// <param name="start">start of date range for collections, will be converted to UTC</param>
        /// <param name="end">end of date range for collections, will be converted to UTC</param>
        /// <returns>unordered list of CollectionDates</returns>
        public async Task<List<CollectionDate>> GetCollectionDaysByAddressAsync(string address, 
            DateTimeOffset? start = null, DateTimeOffset? end = null)
        {
            return await GetCollectionDays(address: address, start: start, end: end);
        }

        /// <summary>
        /// Synchronous call to GetCollectionDaysByAddressAsync
        /// </summary>
        /// <param name="address">valid address. should be confired using GetCCAddres(Async)</param>
        /// <param name="start">start of date range for collections, will be converted to UTC</param>
        /// <param name="end">end of date range for collections, will be converted to UTC</param>
        /// <returns>unordered list of CollectionDates</returns>
        public List<CollectionDate> GetCollectionDaysByAddress(string address,
            DateTimeOffset? start = null, DateTimeOffset? end = null)
        {
            Task<List<CollectionDate>> responseTask = GetCollectionDays(address: address, start: start, end: end);
            Task.WaitAll(responseTask);
            return responseTask.Result;
        }


        /// <summary>
        /// Lookup what will be collected and when collection will take place based on the specified account number
        /// </summary>
        /// <param name="accountNumber">valid account</param>
        /// <param name="start">start of date range for collections, will be converted to UTC</param>
        /// <param name="end">end of date range for collections, will be converted to UTC</param>
        /// <returns>unordered list of CollectionDates</returns>
        public async Task<List<CollectionDate>> GetCollectionDaysByAccountNumberAsync(string accountNumber,
            DateTimeOffset? start = null, DateTimeOffset? end = null)
        {
            return await GetCollectionDays(accountNumber:accountNumber, start: start, end: end);
        }

        /// <summary>
        /// Synchronous call to GetCollectionDaysByAccountNumberAsync
        /// </summary>
        /// <param name="accountNumber">valid account</param>
        /// <param name="start">start of date range for collections, will be converted to UTC</param>
        /// <param name="end">end of date range for collections, will be converted to UTC</param>
        /// <returns>unordered list of CollectionDates</returns>
        public List<CollectionDate> GetCollectionDaysByAccountNumber(string accountNumber,
            DateTimeOffset? start = null, DateTimeOffset? end = null)
        {
            Task<List<CollectionDate>> responseTask = GetCollectionDays(accountNumber: accountNumber, start: start, end: end);
            Task.WaitAll(responseTask);
            return responseTask.Result;
        }

        #endregion public

        #region private helpers
        private async Task<List<CollectionDate>> GetCollectionDays(string address = null, string accountNumber = null,
            DateTimeOffset? start = null, DateTimeOffset? end = null )
        {
            List<CollectionDate> collectionDates = new List<CollectionDate>();
            if(address != null && accountNumber != null)
            {
                throw new ArgumentException("Can't provide address and account number");
            }
            if (String.IsNullOrEmpty(address) && String.IsNullOrEmpty(accountNumber))
            {
                throw new ArgumentException("Must provide address or account number");
            }


            //"GetCollectionDays?pAccount={0}&pAddress={1}&pJustChecking=&pApp=CC&pIE=&start={2}&end={3}";
            string fullEndpoint = string.Format(GET_COLLECTION_DAYS_ENDPOINT,
                accountNumber ?? string.Empty,
                address ?? string.Empty,
                start?.ToUniversalTime().ToUnixTimeSeconds().ToString() ?? "0",
                end?.ToUniversalTime().ToUnixTimeSeconds().ToString() ?? string.Empty);

            using (HttpResponseMessage response = await _client.GetAsync(fullEndpoint))
            {
                using (HttpContent content = response.Content)
                {
                    string json = await content.ReadAsStringAsync();
                    collectionDates = JsonConvert.DeserializeObject<List<CollectionDate>>(json);
                }
            }

            return collectionDates;
        }

        #endregion private helpers


    }

    
}
