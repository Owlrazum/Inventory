[System.Serializable]
public struct DictionaryPackage<TKey, TValue>
{
    public TKey Key;
    public TValue Value;

    public DictionaryPackage(TKey keyArg, TValue valueArg)
    {
        Key = keyArg;
        Value = valueArg;
    }
}