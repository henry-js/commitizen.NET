using System.Text.Json;
using RulesEngine.Models;







var workflows = JsonSerializer.Deserialize<Workflow[]>(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "workflow.json")));
var re = new RulesEngine.RulesEngine(workflows);
int input1 = 300, input2 = 400, input3 = 500;
var resultList = await re.ExecuteAllRulesAsync("Discount", input1, input2, input3);

foreach (var result in resultList)
{
    Console.WriteLine($"Rule - {result.Rule.RuleName}, IsSuccess - {result.IsSuccess}");
}