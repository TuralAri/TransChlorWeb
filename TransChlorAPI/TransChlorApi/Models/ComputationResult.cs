namespace TransChlorApi.Models;

public class ComputationResult
{
    public float Time { get; set; } //get; set; generates getters and setters for the variable
    public List<double> Values { get; set; }
    public string Type { get; set; } //"humidity_relative" "moisture....", ....
    public int ComputationId { get; set; }
}