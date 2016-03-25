namespace RunningCounter.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Models;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;

    [TestClass]
    public class ApiFunctionalTests
    {
        private const string userName = "sebastian";
        private const string password = "123456";
        private const string apiUrl = "http://localhost.fiddler:28470/";

        [TestMethod]
        [Ignore]
        public void FunctionalLoginAndGetAllActivities()
        {
            using (var client = this.GetAuthorizedHttpClient())
            {
                HttpResponseMessage response = client.GetAsync("api/activities").Result;
                if (response.IsSuccessStatusCode)
                {
                    IEnumerable<Activity> activities = response.Content.ReadAsAsync<IEnumerable<Activity>>().Result;
                }
            }
        }


        [TestMethod]
        [Ignore]
        public void FunctionalLoginAndGetActivitiesBetweenDates()
        {
            using (var client = this.GetAuthorizedHttpClient())
            {
                var uri = string.Format("api/activities?{0}", this.BuildDatesFilterQuerystring());

                HttpResponseMessage response = client.GetAsync(uri).Result;
                if (response.IsSuccessStatusCode)
                {
                    IEnumerable<Activity> activities = response.Content.ReadAsAsync<IEnumerable<Activity>>().Result;
                }
            }
        }

        [TestMethod]
        [Ignore]
        public void FunctionalLoginAndCreateActitivy()
        {
            var newActivity = new Activity()
            {
                Kilometers = 100,
                Title = "Activity posted by API",
                Date = DateTime.UtcNow
            };

            using (var client = this.GetAuthorizedHttpClient())
            {
                HttpResponseMessage response = client.PostAsJsonAsync<Activity>("api/activities", newActivity).Result;

                response.EnsureSuccessStatusCode();
            }
        }

        private HttpClient GetAuthorizedHttpClient()
        {
            var client = new HttpClient();

            client.BaseAddress = new Uri(apiUrl);

            var loginRequestContent = string.Format("grant_type=password&username={0}&password={1}", userName, password);
            var loginResponse = client.PostAsync("/auth/token", new StringContent(loginRequestContent, Encoding.UTF8, "application/x-www-form-urlencoded")).Result;

            dynamic tokenResponse = JsonConvert.DeserializeObject(loginResponse.Content.ReadAsStringAsync().Result);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.access_token.Value);

            return client;
        }

        private string BuildDatesFilterQuerystring()
        {
            return string.Format("startDate={0}&endDate={1}&startTime={2}&endTime={3}",
                JsonConvert.SerializeObject(DateTime.UtcNow.AddDays(-2).Date).Replace("\"", ""),
                JsonConvert.SerializeObject(DateTime.UtcNow.AddDays(-1).Date).Replace("\"", ""),
                "17",
                "22");
        }
    }
}
