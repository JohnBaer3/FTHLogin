using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using InfiniteMeals.Meals.Model;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;
using InfiniteMeals.Model.Database;


namespace InfiniteMeals
{
    public partial class SelectMealOptions : ContentPage
    {
        private int mealOrdersCount = 0;
        public string myProperty { get; } = " ";
        UserLoginSession userSesh = new UserLoginSession();
        public ObservableCollection<MealsModel> Meals = new ObservableCollection<MealsModel>();

        private string foodbankID;
        private string kitchenZipcode;
        private string foodbankName;
        private string currentFilters;

        private void Vegan_Clicked(object sender, System.EventArgs e)
        {
            ImageButton Vegan_Clicked = (ImageButton)sender;
            if (Vegan_Clicked.BackgroundColor.Equals(Color.Transparent))
            {
                Vegan_Clicked.BackgroundColor = Color.FromHex("#d4af37");
                currentFilters += "&vegan";
                FindMeals(foodbankID, "https://dc3so1gav1.execute-api.us-west-1.amazonaws.com/dev/api/v2/inventory_filter/" + foodbankID + currentFilters);
            }
            else
            {
                Vegan_Clicked.BackgroundColor = Color.Transparent;
                currentFilters = filterOutOption("&vegan");
                FindMeals(foodbankID, "https://dc3so1gav1.execute-api.us-west-1.amazonaws.com/dev/api/v2/inventory_filter/" + foodbankID + currentFilters);
            }
        }

        private void Kosher_Clicked(object sender, System.EventArgs e)
        {
            ImageButton Kosher_Clicked = (ImageButton)sender;
            if (Kosher_Clicked.BackgroundColor.Equals(Color.Transparent))
            {
                Kosher_Clicked.BackgroundColor = Color.FromHex("#d4af37");
                currentFilters += "&kosher";
                FindMeals(foodbankID, "https://dc3so1gav1.execute-api.us-west-1.amazonaws.com/dev/api/v2/inventory_filter/" + foodbankID + currentFilters);
            }
            else
            {
                Kosher_Clicked.BackgroundColor = Color.Transparent;
                currentFilters = filterOutOption("&kosher");
                FindMeals(foodbankID, "https://dc3so1gav1.execute-api.us-west-1.amazonaws.com/dev/api/v2/inventory_filter/" + foodbankID + currentFilters);
            }
        }

        private void vegetarian_Clicked(object sender, System.EventArgs e)
        {
            ImageButton vegetarian_Clicked = (ImageButton)sender;
            if (vegetarian_Clicked.BackgroundColor.Equals(Color.Transparent))
            {
                vegetarian_Clicked.BackgroundColor = Color.FromHex("#d4af37");
                currentFilters += "&vegetarian";
                FindMeals(foodbankID, "https://dc3so1gav1.execute-api.us-west-1.amazonaws.com/dev/api/v2/inventory_filter/" + foodbankID + currentFilters);
            }
            else
            {
                vegetarian_Clicked.BackgroundColor = Color.Transparent;
                currentFilters = filterOutOption("&vegetarian");
                FindMeals(foodbankID, "https://dc3so1gav1.execute-api.us-west-1.amazonaws.com/dev/api/v2/inventory_filter/" + foodbankID + currentFilters);
            }
        }

        private void glutenfree_Clicked(object sender, System.EventArgs e)
        {
            ImageButton glutenfree_Clicked = (ImageButton)sender;
            if (glutenfree_Clicked.BackgroundColor.Equals(Color.Transparent))
            {
                glutenfree_Clicked.BackgroundColor = Color.FromHex("#d4af37");
                currentFilters += "&gluten";
                FindMeals(foodbankID, "https://dc3so1gav1.execute-api.us-west-1.amazonaws.com/dev/api/v2/inventory_filter/" + foodbankID + currentFilters);
            }
            else
            {
                glutenfree_Clicked.BackgroundColor = Color.Transparent;
                currentFilters = filterOutOption("&gluten");
                FindMeals(foodbankID, "https://dc3so1gav1.execute-api.us-west-1.amazonaws.com/dev/api/v2/inventory_filter/" + foodbankID + currentFilters);
            }
        }

        private String filterOutOption(String toRemove)
        {
            int positionToRemove = currentFilters.IndexOf(toRemove);
            String updatedFilter = currentFilters.Substring(0, positionToRemove) + currentFilters.Substring(positionToRemove + toRemove.Length, currentFilters.Length - positionToRemove - toRemove.Length);
            return updatedFilter;
        }


        protected async Task FindMeals(string foodbank_id, string url)
        {
            NoMealsLabel.IsVisible = false;
            var request = new HttpRequestMessage();

            if (url == "")
            {
                request.RequestUri = new Uri("https://dc3so1gav1.execute-api.us-west-1.amazonaws.com/dev/api/v2/inventory_filter/" + foodbankID);
            }
            else
            {
                request.RequestUri = new Uri(url);
            }

            request.Method = HttpMethod.Get;
            var client = new HttpClient();
            HttpResponseMessage response = await client.SendAsync(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                HttpContent content = response.Content;
                var kitchensString = await content.ReadAsStringAsync();
                JObject foodbanks = JObject.Parse(kitchensString);
                String todaysDate = DateTime.Now.ToString("MM/dd/yyyy");

                if (todaysDate[0] == '0')
                {
                    todaysDate = todaysDate.Substring(1);
                }
                todaysDate = todaysDate.Replace("/0", "/");
                this.Meals.Clear();
                foreach (var k in foodbanks["result"]["result"])
                {
                    string cleanedUpImageString = cleanUpImageString(k["fl_image"].ToString());
                    this.Meals.Add(new MealsModel()
                    {
                        imageString = cleanedUpImageString,
                        price = k["fl_value_in_dollars"].ToString(),
                        foodbank_id = foodbankID,
                        id = k["food_id"].ToString(),
                        food_name = k["fl_name"].ToString(),
                        delivery = Int32.Parse((string)k["delivery"]),
                        pickup = Int32.Parse((string)k["pickup"]),
                        qty = 0
                    });
                }
                mealsListView.ItemsSource = Meals;
            }
        }

        public SelectMealOptions(string foodbank_id, string foodbank_name, string zipcode, UserLoginSession userLoginSession)
        {
            InitializeComponent();

            userSesh = userLoginSession;

            SetBinding(TitleProperty, new Binding(foodbank_name));
            myProperty = foodbank_name;
            BindingContext = this;
            Console.WriteLine("foodbank id " + foodbank_id);
            foodbankID = foodbank_id +"?";
            kitchenZipcode = zipcode;

            FindMeals(foodbankID, "");
        }

        public String cleanUpImageString(String imageString)
        {
            String cleaned = "";
            foreach (var letter in imageString)
            {
                if (!(letter.Equals("?".ToCharArray()[0])))
                {
                    cleaned += letter;
                }
                else
                {
                    break;
                }
            }
            return cleaned;
        }

        async void Handle_Clicked(object sender, System.EventArgs e)
        {
            if (mealOrdersCount == 0)
            {
                await DisplayAlert("Order Error", "Please make an order to continue", "Continue");
            }
            else
            {
                foodbankID = foodbankID.Substring(0, foodbankID.Length - 1);
                var secondPage = new CheckOutPage(Meals, foodbankID, foodbankName, kitchenZipcode, userSesh);
                await Navigation.PushAsync(secondPage);
            }
        }

        private void reduceOrders(object sender, System.EventArgs e)
        {
            var button = (ImageButton)sender;
            var mealObject = (MealsModel)button.CommandParameter;

            if (mealObject != null)
            {
                if (mealObject.qty > 0)
                {
                    mealObject.qty -= 1;
                    mealOrdersCount -= 1;
                }
            }
        }

        private void addOrders(object sender, System.EventArgs e)
        {

            var button = (ImageButton)sender;
            var mealObject = (MealsModel)button.CommandParameter;

            if (mealObject != null)
            {
                if (mealObject.qty < 50)
                {
                    mealObject.qty += 1;
                    mealOrdersCount += 1;
                }
            }
        }

    }

}
