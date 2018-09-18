#r "Microsoft.Azure.Documents.Client"

using System.Net;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents; 
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host; 

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log, IEnumerable<Rating> client)
{
    // parse query parameter
    string name = req.GetQueryNameValuePairs()
        .FirstOrDefault(q => string.Compare(q.Key, "name", true) == 0)
        .Value;

    if (name == null)
    {
        // Get request body
        dynamic data = await req.Content.ReadAsAsync<object>();
        name = data?.name;
    }

    if (client == null)
    {
        return req.CreateResponse(HttpStatusCode.NotFound);
    }
    else
    {
        return req.CreateResponse(HttpStatusCode.OK, client);
    }
}
    public class Rating
{
    public string id { get; set; }
    public string userId { get; set; }

    public string productId { get; set; }

    public string timestamp { get; set; }

    public string locationName { get; set; }

    public string rating { get; set; }

    public string userNotes { get; set; }


}