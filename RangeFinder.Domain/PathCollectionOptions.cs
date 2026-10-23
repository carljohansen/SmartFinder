namespace RangeFinder.Domain;

[Flags]
public enum PathCollectionOptions
{
    None = 0,
    AddGeneralWildcardIfBlank = 1 << 0
}