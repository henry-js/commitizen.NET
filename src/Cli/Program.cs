using System.Text.Json;
using RulesEngine.Models;

var workflows = JsonSerializer.Deserialize<Workflow[]>(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "workflow.json")));
var re = new RulesEngine.RulesEngine(workflows);
var input1 = new { country = "india", loyaltyFactor = 2, totalPurchasesToDate = 6000 };
var input2 = new { totalOrders = 3 };
var input3 = new { noOfVisitsPerMonth = 3 };

var resultList = await re.ExecuteAllRulesAsync("Discount", input1, input2, input3);

foreach (var result in resultList)
{
    Console.WriteLine($"Rule - {result.Rule.RuleName}, IsSuccess - {result.IsSuccess}");
}
