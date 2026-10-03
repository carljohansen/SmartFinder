namespace SharpExplorer.Domain.TextSearch;

public record FileLineTextMatch(string Line, int LineNumber, int StartPos, int Length);