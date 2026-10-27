public enum CharacterId
{
    None,
    System,
    Narration,
    Unknown,
    Yuna,
    Ray,
    Yuan,
    Iris,
    Grayle
}

public static class CharacterIdUtility
{
    public static string GetString(CharacterId id)
    {
        switch (id)
        {
            case CharacterId.Narration: return string.Empty;
            case CharacterId.Unknown: return "???";
            case CharacterId.Yuna: return "유나";
            case CharacterId.Ray: return "레이";
            case CharacterId.Yuan: return "유안";
            case CharacterId.Iris: return "i-Ris";
            case CharacterId.Grayle: return "그레일 애쉬포드";
            case CharacterId.System: return "시스템";
            default:
                {
                    return string.Empty;
                }
        }
    }
}