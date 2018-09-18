#r "Newtonsoft.Json"
#r "Microsoft.Azure.Documents.Client"

using System;
using System.Net;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;



public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log, IAsyncCollector<object> outputDocument)
{
 
        var client = new HttpClient();

        // parse query parameter
        string userId = req.GetQueryNameValuePairs()
            .FirstOrDefault(q => string.Compare(q.Key, "userId", true) == 0)
            .Value;
    
        //Associate new id
        Guid g = Guid.NewGuid();
        DateTime date = DateTime.Now;

        string productId = "";
        string locationName = "";
        string rating = "";
        string userNotes = "";


        if (userId == null)
        {
            // Get request body
            dynamic data = await req.Content.ReadAsAsync<object>();
            userId = data?.userId;
            productId = data?.productId;
            locationName = data?.locationName;
            rating = data?.rating;
            userNotes = data?.userNotes;

        }


            // Validate userId
            var response = await client.GetAsync($"https://serverlessohlondonuser.azurewebsites.net/api/GetUser?userId={userId}");
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
               return  req.CreateResponse(HttpStatusCode.NotFound, "User not found. UserId: " + userId); 
               //(HttpStatusCode.BadRequest, "Please pass a name on the query string or in the request body")
               
            }

            // Validate productId
            response = await client.GetAsync($"https://serverlessohlondonproduct.azurewebsites.net/api/GetProduct?productId={productId}");
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
               return  req.CreateResponse(HttpStatusCode.NotFound, "Product not found. ProductId: " + productId); 
               //(HttpStatusCode.BadRequest, "Please pass a name on the query string or in the request body")
               
            }

            //Validate rating range
            int rangeInt = Int32.Parse(rating);
            if(rangeInt==0||rangeInt>5){
                return  req.CreateResponse(HttpStatusCode.BadRequest, "Rate out of range: " + rating); 
            }

        var newRating = new Rating();
        newRating.id = g;
        newRating.userId = userId;
        newRating.productId = productId;
        newRating.timestamp = date;
        newRating.locationName = locationName;
        newRating.rating = rating;
        newRating.userNotes = userNotes;
        var jsonResponse = JsonConvert.SerializeObject(newRating);
        
        await outputDocument.AddAsync(newRating);


        return new HttpResponseMessage(HttpStatusCode.OK) {
            Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
        };   
   
   
}

    public class Rating
{
    public Guid id { get; set; }
    public string userId { get; set; }

    public string productId { get; set; }

    public DateTime timestamp { get; set; }

    public string locationName { get; set; }

    public string rating { get; set; }

    public string userNotes { get; set; }


}