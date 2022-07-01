[System.Serializable]
public struct MultiDimArrayPackage<TElement>
{
    public int RowIndex;
    public int ColumnIndex;
    public TElement Element;

    public MultiDimArrayPackage(int columnIndexArg, int rowIndexArg, TElement elementArg)
    {
        ColumnIndex = columnIndexArg;
        RowIndex = rowIndexArg;
        Element = elementArg;
    }
}