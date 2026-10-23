namespace SharpExplorer.Domain.History;

public class DirectoryTextCriterionHistory : TextCriterionHistory
{
    public DirectoryTextCriterionHistory(int maxItems, string? fileName) : base(maxItems, fileName)
    {
    }

    public override string[] GetPossibleContinuations(string userInput)
    {
        var possibleContinuations = DirectoryPredictiveTexter.GetPossibleContinuations(userInput);

        if (possibleContinuations.Length == 0 && !string.IsNullOrWhiteSpace(userInput))
        {
            possibleContinuations = base.GetPossibleContinuations(userInput);
        }

        return possibleContinuations;
    }    
}