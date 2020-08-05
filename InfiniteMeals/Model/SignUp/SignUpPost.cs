using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

//Converts C# values into json, before sending it to the DB

namespace InfiniteMeals.Model.SignUp {

    // object to send to database when user attempts to sign up 
    // link: https://uavi7wugua.execute-api.us-west-1.amazonaws.com/dev/api/v2/signup
    public class SignUpPost {
        [JsonProperty("Email")]
        public string Email { get; set; }

        [JsonProperty("FirstName")]
        public string FirstName { get; set; }

        [JsonProperty("LastName")]
        public string LastName { get; set; }

        [JsonProperty("PhoneNumber")]
        public string PhoneNumber { get; set; }

        [JsonProperty("WeeklyUpdates")]
        public string WeeklyUpdates { get; set; }

        [JsonProperty("Referral")]
        public string Referral { get; set; }

        [JsonProperty("Password")]
        public string Password { get; set; }

        //[JsonProperty("City")]
        //public string City { get; set; }

        //[JsonProperty("State")]
        //public string State { get; set; }

        //[JsonProperty("Street")]
        //public string Street { get; set; }

        //[JsonProperty("Zipcode")]
        //public string Zipcode { get; set; }

        //[JsonProperty("Latitude")]
        //public double Latitude { get; set; }

        //[JsonProperty("Longitude")]
        //public double Longitude { get; set; }
    }
}
