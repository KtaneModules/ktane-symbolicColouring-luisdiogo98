class SymbolInfo
{
    public char symbol;
    public float charSize;
    public float yPos;
    public int[] colors;

    public SymbolInfo(char symbol, float charSize, float yPos, int[] colors)
    {
        this.symbol = symbol;
        this.charSize = charSize;
        this.yPos = yPos;
        this.colors = colors;
    }
}