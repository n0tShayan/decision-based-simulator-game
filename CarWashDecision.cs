[System.Serializable]
public class CarWashDecision
{
    public string Description;
    public string LeftChoiceText;
    public string RightChoiceText;
    public CarWashChoice LeftChoice;
    public CarWashChoice RightChoice;
}

[System.Serializable]
public class CarWashChoice
{
    public int StockChange;
    public int SatisfactionChange;
    public int ProfitChange;
    public int MoraleChange;
}